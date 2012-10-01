using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeViewExaminer.CodeView;
using CodeViewExaminer.PortableExecutable;

namespace CodeViewExaminer
{
	/// <summary>
	/// Central class that provides extraction of meta information from a portable executable file.
	/// </summary>
	public class ExecutableMetaInfo
	{
		#region Properties
		public PeHeader PEHeader { get; private set; }
		public PeSectionHeader[] SectionHeaders { get; private set; }
		public CodeSection[] CodeSections { get; private set; }

		public CodeViewDebugSection CodeViewSection { get; private set; }
		private sstSrcModule[] sourceModuleSections;
		#endregion

		#region Constructor/IO
		private ExecutableMetaInfo() { }

		public static ExecutableMetaInfo ExtractFrom(string executableFile)
		{
			using (var stream = new FileStream(executableFile, FileMode.Open, FileAccess.Read))
			using (var br = new BinaryReader(stream))
			{
				var emi = new ExecutableMetaInfo();
				emi.Read(br);
				return emi;
			}
		}
		#endregion

		#region Init
		protected void Read(BinaryReader br)
		{
			// Read initial headers
			PEHeader = PeHeaderReader.Read(br);

			// Read out section information
			SectionHeaders = PeSectionReader.ReadSectionHeaders(PEHeader, br);

			// Read out section data
			CodeSections = PeSectionReader.ReadSections(PEHeader, SectionHeaders,
				br,
				new DebugSectionReader(),
				new ImportSectionReader()
				//new TlsSectionReader()
				//,new ExportSectionReader()
				);

			foreach(var codeSection in CodeSections)
				if (codeSection is CodeViewDebugSection)
				{
					CodeViewSection = (CodeViewDebugSection)codeSection;

					// Scan its sections for the sstSrcModule subsection -- in this one, all offset<>line couples are stored
					var sstSrcModules = new List<sstSrcModule>();
					foreach (var ss in CodeViewSection.Data.SubsectionDirectory.Sections)
						if (ss is sstSrcModule)
							sstSrcModules.Add((sstSrcModule)ss);
					sourceModuleSections = sstSrcModules.ToArray();

					break;
				}
		}
		#endregion

		public bool TryDetermineCodeLocation(uint virtualMemoryOffset, out string module, out ushort line)
		{
			// Convert the memory offset into a section offset
			var CodeOffset = 
				virtualMemoryOffset - 
				PEHeader.OptionalHeader32.ImageBase - 
				PEHeader.OptionalHeader32.BaseOfCode;

			module = null;
			line = 0;

			if (sourceModuleSections!=null)
				foreach(var src in sourceModuleSections)
					foreach (var fi in src.FileInfo) // Enum over all program sources
							for (uint k = 0; k < fi.Segments.Length; k++)
								// (All segments do have start&end offsets for making searching the offset faster)
								if (CodeOffset >= fi.segmentStartOffsets[k] &&
									CodeOffset <= fi.segmentEndOffsets[k])
								{
									// If our code is within this segment, the code must be inside this file
									module = fi.SourceFileName;

									var segment = fi.Segments[k];

									// Break at the first line that lies behind the line offset and decrease m to select the actual line again.
									// If there's one line in the segment only, m remains 0 and will be selected then
									int m = 0;
									for (; m < segment.Lines.Length; m++)
										if (CodeOffset < segment.Offsets[m])
										{
											m--;
											break;
										}

									line = segment.Lines[m];
									return true;
								}

			return false;
		}

		public bool TryGetOffsetByCodeLocation(string module, ushort line, out uint virtualMemoryOffset)
		{
			virtualMemoryOffset = 0;

			if (sourceModuleSections != null)
				foreach (var src in sourceModuleSections)
					foreach (var fi in src.FileInfo)
						if(fi.SourceFileName == module)
							for (uint k = 0; k < fi.Segments.Length; k++)
							{
								var segment = fi.Segments[k];

								int m = 0;
								for (; m < segment.Lines.Length; m++)
									if (line == segment.Lines[m])
									{
										virtualMemoryOffset =
											PEHeader.OptionalHeader32.ImageBase +
											PEHeader.OptionalHeader32.BaseOfCode +
											segment.Offsets[m];
										return true;
									}
							}

			return false;
		}
	}
}
