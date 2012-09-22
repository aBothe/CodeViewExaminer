using System;
using System.Runtime.InteropServices;

namespace CodeViewExaminer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_DOS_HEADER
	{      // DOS .EXE header
		public ushort e_magic;              // Magic number
		public ushort e_cblp;               // Bytes on last page of file
		public ushort e_cp;                 // Pages in file
		public ushort e_crlc;               // Relocations
		public ushort e_cparhdr;            // Size of header in paragraphs
		public ushort e_minalloc;           // Minimum extra paragraphs needed
		public ushort e_maxalloc;           // Maximum extra paragraphs needed
		public ushort e_ss;                 // Initial (relative) SS value
		public ushort e_sp;                 // Initial SP value
		public ushort e_csum;               // Checksum
		public ushort e_ip;                 // Initial IP value
		public ushort e_cs;                 // Initial (relative) CS value
		public ushort e_lfarlc;             // File address of relocation table
		public ushort e_ovno;               // Overlay number
		public ushort e_res_0;              // Reserved words
		public ushort e_res_1;              // Reserved words
		public ushort e_res_2;              // Reserved words
		public ushort e_res_3;              // Reserved words
		public ushort e_oemid;              // OEM identifier (for e_oeminfo)
		public ushort e_oeminfo;            // OEM information; e_oemid specific
		public ushort e_res2_0;             // Reserved words
		public ushort e_res2_1;             // Reserved words
		public ushort e_res2_2;             // Reserved words
		public ushort e_res2_3;             // Reserved words
		public ushort e_res2_4;             // Reserved words
		public ushort e_res2_5;             // Reserved words
		public ushort e_res2_6;             // Reserved words
		public ushort e_res2_7;             // Reserved words
		public ushort e_res2_8;             // Reserved words
		public ushort e_res2_9;             // Reserved words
		/// <summary>
		/// File address of new exe header.
		/// </summary>
		public uint e_lfanew;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_OPTIONAL_HEADER32
	{
		/// <summary>
		/// The unsigned integer that identifies the state of the image file. The most common number is 0x10B, which identifies it as a normal executable file. 0x107 identifies it as a ROM image, and 0x20B identifies it as a PE32+ executable.
		/// </summary>
		public ushort Magic;
		/// <summary>
		/// The linker major version number.
		/// </summary>
		public byte MajorLinkerVersion;
		/// <summary>
		/// The linker minor version number.
		/// </summary>
		public byte MinorLinkerVersion;
		/// <summary>
		/// The size of the code (text) section, or the sum of all code sections if there are multiple sections.
		/// </summary>
		public uint SizeOfCode;
		/// <summary>
		/// The size of the initialized data section, or the sum of all such sections if there are multiple data sections.
		/// </summary>
		public uint SizeOfInitializedData;
		public uint SizeOfUninitializedData;
		/// <summary>
		/// The address of the entry point relative to the image base when the executable file is loaded into memory. For program images, this is the starting address. For device drivers, this is the address of the initialization function. An entry point is optional for DLLs. When no entry point is present, this field must be zero.
		/// </summary>
		public uint AddressOfEntryPoint;
		/// <summary>
		/// The address that is relative to the image base of the beginning-of-code section when it is loaded into memory.
		/// </summary>
		public uint BaseOfCode;
		/// <summary>
		/// The address that is relative to the image base of the beginning-of-data section when it is loaded into memory.
		/// </summary>
		public uint BaseOfData;
		/// <summary>
		/// The preferred address of the first byte of image when loaded into memory; 
		/// must be a multiple of 64 K. 
		/// The default for DLLs is 0x10000000. 
		/// The default for Windows CE EXEs is 0x00010000. 
		/// The default for Windows NT, Windows 2000, Windows XP, Windows 95, Windows 98, and Windows Me is 0x00400000.
		/// </summary>
		public uint ImageBase;
		/// <summary>
		/// The alignment (in bytes) of sections when they are loaded into memory. 
		/// It must be greater than or equal to FileAlignment. 
		/// The default is the page size for the architecture.
		/// </summary>
		public uint SectionAlignment;
		/// <summary>
		/// The alignment factor (in bytes) that is used to align the raw data of sections in the image file. 
		/// The value should be a power of 2 between 512 and 64 K, inclusive. 
		/// The default is 512. 
		/// If the SectionAlignment is less than the architecture’s page size, then FileAlignment must match SectionAlignment.
		/// </summary>
		public uint FileAlignment;
		public ushort MajorOperatingSystemVersion;
		public ushort MinorOperatingSystemVersion;
		public ushort MajorImageVersion;
		public ushort MinorImageVersion;
		public ushort MajorSubsystemVersion;
		public ushort MinorSubsystemVersion;
		public uint Win32VersionValue;
		/// <summary>
		/// The size (in bytes) of the image, including all headers, as the image is loaded in memory. 
		/// It must be a multiple of SectionAlignment.
		/// </summary>
		public uint SizeOfImage;
		public uint SizeOfHeaders;
		public uint CheckSum;
		public ushort Subsystem;
		public ushort DllCharacteristics;
		public uint SizeOfStackReserve;
		public uint SizeOfStackCommit;
		public uint SizeOfHeapReserve;
		public uint SizeOfHeapCommit;
		/// <summary>
		/// Reserved, must be zero.
		/// </summary>
		public uint LoaderFlags;
		/// <summary>
		/// The number of data-directory entries in the remainder of the optional header. Each describes a location and size.
		/// </summary>
		public uint NumberOfRvaAndSizes;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_OPTIONAL_HEADER64
	{
		public ushort Magic;
		public Byte MajorLinkerVersion;
		public Byte MinorLinkerVersion;
		public uint SizeOfCode;
		public uint SizeOfInitializedData;
		public uint SizeOfUninitializedData;
		public uint AddressOfEntryPoint;
		public uint BaseOfCode;
		public UInt64 ImageBase;
		public uint SectionAlignment;
		public uint FileAlignment;
		public ushort MajorOperatingSystemVersion;
		public ushort MinorOperatingSystemVersion;
		public ushort MajorImageVersion;
		public ushort MinorImageVersion;
		public ushort MajorSubsystemVersion;
		public ushort MinorSubsystemVersion;
		public uint Win32VersionValue;
		public uint SizeOfImage;
		public uint SizeOfHeaders;
		public uint CheckSum;
		public ushort Subsystem;
		public ushort DllCharacteristics;
		public UInt64 SizeOfStackReserve;
		public UInt64 SizeOfStackCommit;
		public UInt64 SizeOfHeapReserve;
		public UInt64 SizeOfHeapCommit;
		public uint LoaderFlags;
		public uint NumberOfRvaAndSizes;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_FILE_HEADER
	{
		/// <summary>
		/// The number that identifies the type of target machine.
		/// </summary>
		public ushort Machine;
		/// <summary>
		/// The number of sections. This indicates the size of the section table, which immediately follows the headers.
		/// </summary>
		public ushort NumberOfSections;
		/// <summary>
		/// The low 32 bits of the number of seconds since 00:00 January 1, 1970 (a C run-time time_t value), that indicates when the file was created.
		/// </summary>
		public uint TimeDateStamp;
		/// <summary>
		/// The file offset of the COFF symbol table, or zero if no COFF symbol table is present. This value should be zero for an image because COFF debugging information is deprecated.
		/// </summary>
		public uint PointerToSymbolTable;
		/// <summary>
		/// The number of entries in the symbol table. This data can be used to locate the string table, which immediately follows the symbol table. This value should be zero for an image because COFF debugging information is deprecated.
		/// </summary>
		public uint NumberOfSymbols;
		/// <summary>
		/// The size of the optional header, which is required for executable files but not for object files. This value should be zero for an object file.
		/// </summary>
		public ushort SizeOfOptionalHeader;
		/// <summary>
		/// The flags that indicate the attributes of the file.
		/// </summary>
		public ushort Characteristics;
	}

	[StructLayout(LayoutKind.Sequential,Pack=1)]
	public struct IMAGE_DATA_DIRECTORY
	{
		public uint VirtualAddress;
		public uint Size;
	}

	public struct PeSectionHeader
	{
		/// <summary>
		/// Section name
		/// </summary>
		public string Name;
		/// <summary>
		/// The total size of the section when loaded into memory. 
		/// If this value is greater than SizeOfRawData, the section is zero-padded. 
		/// This field is valid only for executable images and should be set to zero for object files.
		/// </summary>
		public uint VirtualSize;
		/// <summary>
		/// For executable images, the address of the first byte of the section 
		/// relative to the image base when the section is loaded into memory. 
		/// For object files, this field is the address of the first byte before 
		/// relocation is applied; for simplicity, compilers should set this to zero. 
		/// Otherwise, it is an arbitrary value that is subtracted from offsets during relocation.
		/// </summary>
		public uint VirtualAddress;
		/// <summary>
		/// The size of the section (for object files) or the size of the 
		/// initialized data on disk (for image files). 
		/// For executable images, this must be a multiple of FileAlignment from the optional header. 
		/// If this is less than VirtualSize, the remainder of the section is zero-filled. 
		/// Because the SizeOfRawData field is rounded but the VirtualSize field is not, 
		/// it is possible for SizeOfRawData to be greater than VirtualSize as well. 
		/// When a section contains only uninitialized data, this field should be zero.
		/// </summary>
		public uint SizeOfRawData;
		/// <summary>
		/// The file pointer to the first page of the section within the COFF file. 
		/// For executable images, this must be a multiple of FileAlignment from the optional header. 
		/// For object files, the value should be aligned on a 4 byte boundary for best performance. 
		/// When a section contains only uninitialized data, this field should be zero.
		/// </summary>
		public uint PointerToRawData;
		/// <summary>
		/// The file pointer to the beginning of relocation entries for the section. This is set to zero for executable images or if there are no relocations.
		/// </summary>
		public uint PointerToRelocations;
		/// <summary>
		/// The file pointer to the beginning of line-number entries for the section. This is set to zero if there are no COFF line numbers. This value should be zero for an image because COFF debugging information is deprecated.
		/// </summary>
		public uint PointerToLinenumbers;
		/// <summary>
		/// The number of relocation entries for the section. This is set to zero for executable images.
		/// </summary>
		public ushort NumberOfRelocations;
		/// <summary>
		/// The number of line-number entries for the section. This value should be zero for an image because COFF debugging information is deprecated.
		/// </summary>
		public ushort NumberOfLinenumbers;
		/// <summary>
		/// The flags that describe the characteristics of the section
		/// </summary>
		public uint Characteristics;

		public override string ToString()
		{
			return Name+" header";
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_DEBUG_DIRECTORY
	{
		public uint Characteristics;
		public uint TimeDateStamp;
		public ushort MajorVersion;
		public ushort MinorVersion;
		public IMAGE_DEBUG_TYPE Type;
		public uint SizeOfData;
		public uint AddressOfRawData;
		public uint PointerToRawData;
	}

	public enum IMAGE_DEBUG_TYPE : uint
	{
		UNKNOWN = 0,
		COFF = 1,
		CODEVIEW,
		FPO,
		MISC,
		EXCEPTION,
		FIXUP,
		OMAP_TO_SRC,
		OMAP_FROM_SRC,
		BORLAND,
		RESERVED10,
		CLSID
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_TLS_DIRECTORY64 {
		public ulong StartAddressOfRawData;
		public ulong EndAddressOfRawData;
		public ulong AddressOfIndex;         // PDWORD
		public ulong AddressOfCallBacks;     // PIMAGE_TLS_CALLBACK *;
		public uint SizeOfZeroFill;
		public uint Characteristics;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct IMAGE_TLS_DIRECTORY32 {
		/// <summary>
		/// The starting address of the TLS template. The template is a block of data that is used to initialize TLS data. The system copies all of this data each time a thread is created, so it must not be corrupted. Note that this address is not an RVA; it is an address for which there should be a base relocation in the .reloc section.
		/// </summary>
		public uint StartAddressOfRawData;
		/// <summary>
		/// The address of the last byte of the TLS, except for the zero fill. As with the Raw Data Start VA field, this is a VA, not an RVA.
		/// </summary>
		public uint EndAddressOfRawData;
		/// <summary>
		/// The location to receive the TLS index, which the loader assigns. This location is in an ordinary data section, so it can be given a symbolic name that is accessible to the program.
		/// </summary>
		public uint AddressOfIndex;             // PDWORD
		/// <summary>
		/// The pointer to an array of TLS callback functions. The array is null-terminated, so if no callback function is supported, this field points to 4 bytes set to zero.
		/// </summary>
		public uint AddressOfCallBacks;         // PIMAGE_TLS_CALLBACK *
		/// <summary>
		/// The size in bytes of the template, beyond the initialized data delimited by the Raw Data Start VA and Raw Data End VA fields. The total template size should be the same as the total size of TLS data in the image file. The zero fill is the amount of data that comes after the initialized nonzero data.
		/// </summary>
		public uint SizeOfZeroFill;
		/// <summary>
		/// Reserved for possible future use by TLS flags.
		/// </summary>
		public uint Characteristics;
	}
}
