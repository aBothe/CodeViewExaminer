using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeViewExaminer;
using CodeViewExaminer.CodeView;
using CodeViewExaminer.PortableExecutable;

namespace ExaminterTests
{
	class Program
	{
		static void Main(string[] args)
		{
			var exe = "TestProgram\\myprogram.exe";

			var emi = ExecutableMetaInfo.ExtractFrom(exe);

			string module;
			ushort line;
			emi.TryDetermineCodeLocation(0x00403022, out module, out line);

			Console.WriteLine(module + ":" + line);

			var sw = new StringWriter();

			foreach (var codeSection in emi.CodeSections)
				if (codeSection is CodeViewDebugSection)
				{
					var cv = (CodeViewDebugSection)codeSection;

					foreach (var ss in cv.Data.SubsectionDirectory.Sections)
					{
						if (ss is sstSrcModule)
						{
							var src = (sstSrcModule)ss;

							foreach (var fi in src.FileInfo)
							{
								sw.WriteLine(fi.SourceFileName + ":");
								for (uint k = 0; k < fi.Segments.Length; k++)
								{
									var segment = fi.Segments[k];
									sw.WriteLine("\tSegment #" + k + " (" + fi.segmentStartOffsets[k] + " - " + fi.segmentEndOffsets[k] + ")");

									for (int m = 0; m < segment.Lines.Length; m++)
										sw.WriteLine("\t\tLine " + segment.Lines[m] + " @ 0x" + string.Format("{0:X8} = {0}", segment.Offsets[m]));
								}
							}
						}
					}
				}
				else if (codeSection is ExportSection)
				{

				}

			sw.Flush();
			File.WriteAllText(exe + ".log", sw.ToString());
		}
	}
}
