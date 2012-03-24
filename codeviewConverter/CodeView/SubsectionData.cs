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
				case SubsectionType.sstModule:
					sd = new sstModule();
					break;
				case SubsectionType.sstSrcModule:
					sd = new sstSrcModule();
					break;
				case SubsectionType.sstLibraries:
					sd = new sstLibraries();
					break;
				case SubsectionType.sstGlobalSym:
					sd = new sstGlobalSym();
					break;
				case SubsectionType.sstGlobalTypes:
					sd = new sstGlobalTypes();
					break;
				case SubsectionType.sstSegName:
					sd = new sstSegName();
					break;
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
}
