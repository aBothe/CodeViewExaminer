using System;
using System.Collections.Generic;
using System.Linq;
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
