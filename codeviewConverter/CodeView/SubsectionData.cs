using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeViewExaminer.CodeView
{
	public abstract class SubsectionData
	{
		public static SubsectionData Read(long lfaBase, DirectoryEntryHeader hdr, BinaryReader r)
		{
			SubsectionData sd = null;

			switch (hdr.Type)
			{
				case SubsectionType.sstFileIndex:
					sd = new sstFileIndex();
					break;
			}

			if (sd != null)
			{
				sd.Header = hdr;
				r.BaseStream.Position = lfaBase + hdr.ContentOffset;
				sd.Read(r);
			}

			return sd;
		}

		public DirectoryEntryHeader Header;

		public abstract void Read(BinaryReader reader);
	}

	public class sstSourceModule : SubsectionData
	{

	}

	public class sstFileIndex : SubsectionData
	{
		/// <summary>
		/// Count or number of modules in the executable.
		/// </summary>
		public uint moduleCount;
		/// <summary>
		/// Count or total number of file name references.
		/// </summary>
		public uint filenameRefCount;
		/// <summary>
		/// Array of indices into the NameOffset table for each module. 
		/// Each index is the start of the file name references for each module.
		/// </summary>
		public ushort[] modStart;
		/// <summary>
		/// Number of file name references per module.
		/// </summary>
		public ushort[] modReferenceCount;
		/// <summary>
		/// Array of offsets into the Names table. For each module, the offset to
		/// first referenced file name is at NameRef[ModStart] and continues for
		/// modReferenceCount entries.
		/// </summary>
		public uint[] nameRef;
		/// <summary>
		/// List of zero-terminated file names. Each file name is partially
		/// qualified relative to the compilation directory
		/// </summary>
		public string[] names;

		public override void Read(BinaryReader r)
		{
			var initPos = r.BaseStream.Position;

			moduleCount = r.ReadUInt16();

			filenameRefCount = r.ReadUInt16();

			modStart = new ushort[moduleCount];
			for (int i = 0; i < moduleCount; i++)
				modStart[i] = r.ReadUInt16();

			modReferenceCount = new ushort[moduleCount];
			for (int i = 0; i < moduleCount; i++)
				modReferenceCount[i] = r.ReadUInt16();

			nameRef = new uint[filenameRefCount];
			for (int i = 0; i < filenameRefCount; i++)
				nameRef[i] = r.ReadUInt32();

			var remainingBytes = Header.Size - (r.BaseStream.Position - initPos);

			var names = new List<string>();
			
			for (var i = remainingBytes; i != 0; )
			{
				var s=r.ReadString();

				if (string.IsNullOrEmpty(s))
					break;

				names.Add(s);
				remainingBytes -= 1 + s.Length;
			}

			this.names = names.ToArray();
		}
	}
}
