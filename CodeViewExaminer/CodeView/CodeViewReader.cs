using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CodeViewExaminer.CodeView
{
	public class CodeViewReader
	{
		public const string CodeViewSignature = "NB09";

		IMAGE_DEBUG_DIRECTORY ddir;
		BinaryReader r;
		CodeViewData cvData=new CodeViewData();

		/// <summary>
		/// Reads debug information from a file handle.
		/// The hFile parameter has to be closed afterwards manually(!).
		/// </summary>
		public static CodeViewData Read(IntPtr hFile, long debugInfoOffset, long debugInfoSize)
		{
			if (debugInfoSize == 0)
				return null;

			using(var file = new FileStream(hFile, FileAccess.Read))
			using(var r = new BinaryReader(file))
			{
				file.Position = debugInfoOffset;
				var cvReader = new CodeViewReader { r = r };
				cvReader.DoRead();

				return cvReader.cvData;
			}
		}

		public static CodeViewData Read(IMAGE_DEBUG_DIRECTORY ddir,BinaryReader r)
		{
			var cvReader = new CodeViewReader { ddir=ddir,	r=r	};

			cvReader.DoRead();

			return cvReader.cvData;
		}

		void DoRead()
		{
			/*
			 * For more info, see codeviewNB09.pdf, point 7. "Symbol and Type Format for Microsoft Executables" 
			 * (pdf page 71)
			 */

			cvData.lfaBase = r.BaseStream.Position;

			// Ensure that there's the right CodeView4 signature
			var signature = Encoding.ASCII.GetString(r.ReadBytes(4));
			if (signature != CodeViewSignature)
				throw new InvalidDataException("Invalid CodeView Format: Signature '"+CodeViewSignature+"' expected, '"+signature+"' found at position "+cvData.lfaBase);

			// Read 'Subsection Directory' address
			cvData.lfoDirectory = r.ReadUInt32();

			r.BaseStream.Position = cvData.lfaBase + cvData.lfoDirectory;

            cvData.SubsectionDirectory = SubsectionDirectory.Read(cvData.lfaBase,r);
		}
	}
}
