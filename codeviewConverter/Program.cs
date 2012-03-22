using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeViewExaminer.PortableExecutable;

namespace CodeViewExaminer
{
	class Program
	{
		static void Main(string[] args)
		{
			var exe = "HelloWorldTest.exe";

			var stream = new FileStream(exe, FileMode.Open, FileAccess.Read);
			var br = new BinaryReader(stream);

			// Read initial headers
			var peHdr = PeHeaderReader.Read(br);

			// Read out section information
			var peSectionHeaders = PeSectionReader.ReadSectionHeaders(peHdr, br);

			// Read out section data
			var sections = PeSectionReader.ReadSections(peHdr, 
				peSectionHeaders, 
				br, 
				new DebugSectionReader(),
				new TlsSectionReader());

			return;
		}
	}
}
