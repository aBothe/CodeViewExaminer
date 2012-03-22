using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeViewExaminer.PortableExecutable
{
	public class PeSectionReader
	{
		public static PeSectionHeader[] ReadSectionHeaders(PeHeader header,BinaryReader r)
		{
			var sectionHeaders = new List<PeSectionHeader>();

			for (ushort i = 0; i < header.FileHeader.NumberOfSections; i++)
			{
				var sectionHeader = new PeSectionHeader();

				sectionHeader.Name= Encoding.UTF8.GetString(r.ReadBytes(8)).TrimEnd('\0');
				sectionHeader.VirtualSize = r.ReadUInt32();
				sectionHeader.VirtualAddress = r.ReadUInt32();
				sectionHeader.SizeOfRawData = r.ReadUInt32();
				sectionHeader.PointerToRawData = r.ReadUInt32();
				sectionHeader.PointerToRelocations = r.ReadUInt32();
				sectionHeader.PointerToLinenumbers = r.ReadUInt32();
				sectionHeader.NumberOfRelocations = r.ReadUInt16();
				sectionHeader.NumberOfLinenumbers = r.ReadUInt16();
				sectionHeader.Characteristics = r.ReadUInt32();

				sectionHeaders.Add(sectionHeader);
			}

			return sectionHeaders.ToArray();
		}

		public static CodeSection[] ReadSections(PeHeader peHeader, PeSectionHeader[] sectionHeaders, BinaryReader r, params ISectionHandler[] Handlers)
		{
			var l = new List<CodeSection>();

			foreach (var secHdr in sectionHeaders)
			{
				var sec = ReadSectionContents(peHeader, secHdr, r, Handlers);

				if (sec != null)
					l.Add(sec);
			}

			return l.ToArray();
		}

		public static CodeSection ReadSectionContents(PeHeader peHeader,PeSectionHeader sectionHeader,BinaryReader r, params ISectionHandler[] Handlers)
		{
			foreach (var handler in Handlers)
				if (handler != null && handler.CanHandle(sectionHeader.Name))
				{
					r.BaseStream.Seek(sectionHeader.PointerToRawData,SeekOrigin.Begin);

					return handler.Handle(peHeader,sectionHeader, r);
				}

			return null;
		}
	}

	public interface ISectionHandler
	{
		bool CanHandle(string SectionName);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Header"></param>
		/// <param name="r">Will be positioned at the section's begin</param>
		/// <returns></returns>
		CodeSection Handle(PeHeader PeHeader,PeSectionHeader Header,BinaryReader Reader);
	}

	public abstract class CodeSection
	{
		public PeSectionHeader SectionHeader;
	}
}
