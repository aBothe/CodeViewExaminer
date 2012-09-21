using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CodeViewExaminer.PortableExecutable
{
	/*
	 * Original code taken from http://code.cheesydesign.com/?p=572
	 * (by JohnStewien, posted on MAY 5, 2010)
	 */

	/// <summary>
	/// Reads in the header information of the Portable Executable format.
	/// </summary>
	public class PeHeaderReader
	{
		PeHeaderReader() { }

		public static PeHeader ReadFrom(string filePath)
		{
			using (FileStream stream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				return Read(stream);
			}
		}

		public static PeHeader Read(Stream s)
		{
			return Read(new BinaryReader(s));
		}

		public static PeHeader Read(BinaryReader reader)
		{
			var hdr = new PeHeader();

			// Reset position to file start!
			reader.BaseStream.Seek(0, SeekOrigin.Begin);

			hdr.DosHeader = Misc.FromBinaryReader<IMAGE_DOS_HEADER>(reader);

			// Add 4 bytes to the offset
			reader.BaseStream.Seek(hdr.DosHeader.e_lfanew, SeekOrigin.Begin);

			var ntHeadersSignature = reader.ReadUInt32();
			hdr.FileHeader = Misc.FromBinaryReader<IMAGE_FILE_HEADER>(reader);

			if (hdr.Is32BitHeader)
				hdr.OptionalHeader32 = Misc.FromBinaryReader<IMAGE_OPTIONAL_HEADER32>(reader);
			else
				hdr.OptionalHeader64 = Misc.FromBinaryReader<IMAGE_OPTIONAL_HEADER64>(reader);

			var dirDirectoryCount=hdr.Is32BitHeader?
				hdr.OptionalHeader32.NumberOfRvaAndSizes:
				hdr.OptionalHeader64.NumberOfRvaAndSizes;

			if (dirDirectoryCount > 0)
			{
				var ddl = new List<IMAGE_DATA_DIRECTORY>((int)dirDirectoryCount);

				for (int i = 0; i < dirDirectoryCount; i++)
					ddl.Add(Misc.FromBinaryReader<IMAGE_DATA_DIRECTORY>(reader));

				hdr.DataDirectories = ddl.ToArray();
			}

			return hdr;
		}
	}

	/// <summary>
	/// Provides PE header information such as the date the assembly was compiled.
	/// </summary>
	public class PeHeader
	{
		/// <summary>
		/// The DOS header
		/// </summary>
		public IMAGE_DOS_HEADER DosHeader;
		/// <summary>
		/// The file header
		/// </summary>
		public IMAGE_FILE_HEADER FileHeader;
		/// <summary>
		/// Optional 32 bit file header
		/// </summary>
		public IMAGE_OPTIONAL_HEADER32 OptionalHeader32;
		/// <summary>
		/// Optional 64 bit file header
		/// </summary>
		public IMAGE_OPTIONAL_HEADER64 OptionalHeader64;

		public IMAGE_DATA_DIRECTORY[] DataDirectories;

		/// <summary>
		/// Gets if the file header is 32 bit or not
		/// </summary>
		public bool Is32BitHeader
		{
			get
			{
				UInt16 IMAGE_FILE_32BIT_MACHINE = 0x0100;
				return (IMAGE_FILE_32BIT_MACHINE & FileHeader.Characteristics) == IMAGE_FILE_32BIT_MACHINE;
			}
		}

		/// <summary>
		/// Gets the timestamp from the file header
		/// </summary>
		public DateTime TimeStamp
		{
			get
			{
				// Timestamp is a date offset from 1970
				DateTime returnValue = new DateTime(1970, 1, 1, 0, 0, 0);

				// Add in the number of seconds since 1970/1/1
				returnValue = returnValue.AddSeconds(FileHeader.TimeDateStamp);
				// Adjust to local timezone
				returnValue += TimeZone.CurrentTimeZone.GetUtcOffset(returnValue);

				return returnValue;
			}
		}
	}
}
