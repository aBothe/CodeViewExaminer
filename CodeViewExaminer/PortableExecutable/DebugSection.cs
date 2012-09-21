using System.IO;
using CodeViewExaminer.CodeView;

namespace CodeViewExaminer.PortableExecutable
{
	public class DebugSectionReader : ISectionHandler
	{
		public bool CanHandle(string SectionName)
		{
			return SectionName==".debug";
		}

		public CodeSection Handle(PeHeader PeHeader, PeSectionHeader hdr, BinaryReader r)
		{
			var entryInfo = Misc.FromBinaryReader<IMAGE_DEBUG_DIRECTORY>(r);

			if (entryInfo.PointerToRawData == 0)
				return null;

			r.BaseStream.Position = entryInfo.PointerToRawData;

			if (entryInfo.Type == IMAGE_DEBUG_TYPE.CODEVIEW)
				return new CodeViewDebugSection { 
					EntryInformation=entryInfo,
					SectionHeader=hdr,
					Data = CodeViewReader.Read(entryInfo,r),
				};

			return new DebugSection { SectionHeader=hdr, EntryInformation=entryInfo };
		}
	}

	public class DebugSection : CodeSection
	{
		public IMAGE_DEBUG_DIRECTORY EntryInformation;
	}

	/// <summary>
	/// A dedicated section object which stores CodeView information
	/// </summary>
	public class CodeViewDebugSection : DebugSection
	{
		public CodeViewData Data;
	}
}
