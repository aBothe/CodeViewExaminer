using System;
using System.Collections.Generic;
using System.Text;

namespace CodeViewExaminer.PortableExecutable
{
	public class ImportSectionReader : ISectionHandler
	{
		public bool CanHandle(string SectionName)
		{
			return SectionName == ".idata";
		}

		public CodeSection Handle(PeHeader PeHeader, PeSectionHeader Header, System.IO.BinaryReader Reader)
		{
			var imps = new ImportSection();


			return imps;
		}
	}

	public class ImportSection : CodeSection
	{

	}
}
