using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeViewExaminer.PortableExecutable
{
	public class ExportSectionReader : ISectionHandler
	{

		public bool CanHandle(string SectionName)
		{
			return SectionName == ".edata";
		}

		public CodeSection Handle(PeHeader PeHeader, PeSectionHeader Header, BinaryReader r)
		{
			var cs = new ExportSection { SectionHeader = Header };

			// Read Export directory table
			var exportDirTbl = Misc.FromBinaryReader<ExportDirectoryTable>(r);

			// Read export address table
			//r.BaseStream.Position = PeHeader.OptionalHeader32.ImageBase + exportDirTbl.OrdinalTableRVA;
			var exportAddressTable = new List<ExportOrdinalTableEntry>(exportDirTbl.AddressTableEntries);

			for (int i = exportDirTbl.AddressTableEntries; i > 0; i--)
			{
				var u = r.ReadUInt32();
				var u2 = r.ReadUInt32();
				exportAddressTable.Add(new ExportOrdinalTableEntry
				{
					ExportRVA = u,
					ForwarderRVA = u2
				});
			}

			// Export Name Pointer Table
			//r.BaseStream.Position = PeHeader.OptionalHeader32.ImageBase + exportDirTbl.NamePointerRVA;
			var exportNamePointers = new List<uint>(exportDirTbl.NamePointerCount);

			for (int i = exportDirTbl.NamePointerCount; i > 0; i--)
				exportNamePointers.Add(r.ReadUInt32());

			// Export Ordinal Table
			//r.BaseStream.Position = PeHeader.OptionalHeader32.ImageBase + exportDirTbl.OrdinalTableRVA;
			var symbolAddressLookup = new List<ushort>(exportDirTbl.AddressTableEntries);

			// Export name table
			var exportNames = new List<string>(exportDirTbl.NamePointerCount);

			for (int i = exportDirTbl.NamePointerCount; i > 0; i--)
			{
				var sb = new StringBuilder();
				char c;
				while ((c = (char)r.ReadByte()) != '\0')
					sb.Append(c);
				exportNames.Add(sb.ToString());
			}

			return cs;
		}
	}

	/// <summary>
	/// The export address table contains the address of exported entry points 
	/// and exported data and absolutes. 
	/// An ordinal number is used as an index into the export address table.
	/// 
	/// Each entry in the export address table is a field that uses 
	/// one of two formats in the following table. 
	/// If the address specified is not within the export section 
	/// (as defined by the address and length that are indicated in the optional header), 
	/// the field is an export RVA, which is an actual address in code or data. Otherwise, 
	/// the field is a forwarder RVA, which names a symbol in another DLL.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ExportOrdinalTableEntry
	{
		/// <summary>
		/// The address of the exported symbol when loaded into memory, relative to the image base. 
		/// For example, the address of an exported function.
		/// </summary>
		public uint ExportRVA;
		/// <summary>
		/// The pointer to a null-terminated ASCII string in the export section. 
		/// This string must be within the range that is given by the 
		/// export table data directory entry.
		/// This string gives the DLL name and the name of 
		/// the export (for example, “MYDLL.expfunc”) or the DLL name 
		/// and the ordinal number of the export (for example, “MYDLL.#27”).
		/// </summary>
		public uint ForwarderRVA;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ExportDirectoryTable
	{
		/// <summary>
		/// Reserved, must be 0.
		/// </summary>
		public uint ExportFlags;
		/// <summary>
		/// The time and date that the export data was created.
		/// </summary>
		public uint TimeDateStamp;
		/// <summary>
		/// The major version number. 
		/// The major and minor version numbers can be set by the user.
		/// </summary>
		public ushort MajorVersion;
		/// <summary>
		/// The minor version number.
		/// </summary>
		public ushort MinorVersion;
		/// <summary>
		/// The address of the ASCII string that contains the name of the DLL. 
		/// This address is relative to the image base.
		/// </summary>
		public uint NameRVA;
		/// <summary>
		/// The starting ordinal number for exports in this image. 
		/// This field specifies the starting ordinal number for the export address table. 
		/// It is usually set to 1.
		/// </summary>
		public uint OrdinalBase;
		/// <summary>
		/// The number of entries in the export address table.
		/// </summary>
		public int AddressTableEntries;
		/// <summary>
		/// The number of entries in the name pointer table. 
		/// This is also the number of entries in the ordinal table.
		/// </summary>
		public int NamePointerCount;
		/// <summary>
		/// The address of the export address table, relative to the image base.
		/// </summary>
		public uint ExportTableRVA;
		/// <summary>
		/// The address of the export name pointer table, relative to the image base. 
		/// The table size is given by the Number of Name Pointers field.
		/// </summary>
		public uint NamePointerRVA;
		/// <summary>
		/// The address of the ordinal table, relative to the image base.
		/// </summary>
		public uint OrdinalTableRVA;
	}

	/// <summary>
	/// The export data section, named .edata, contains information about symbols 
	/// that other images can access through dynamic linking. 
	/// Exported symbols are generally found in DLLs, but DLLs can also import symbols.
	/// </summary>
	public class ExportSection : CodeSection
	{
		public ExportFunction[] ExportedFunctions;
	}

	public class ExportFunction
	{
		public readonly string Name;
		public readonly uint FunctionAddress;

		public readonly bool IsForwarded;
		public readonly bool ForwardName;
	}
}
