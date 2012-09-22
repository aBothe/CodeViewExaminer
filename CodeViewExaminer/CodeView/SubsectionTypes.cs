using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace CodeViewExaminer.CodeView
{
	/// <summary>
	/// This describes the basic information about an object module, 
	/// including code segments, module name, and the number 
	/// of segments for the modules that follow. 
	/// Directory entries for sstModules precede all other subsection directory entries.
	/// </summary>
	public class sstModule : SubsectionData
	{
		/// <summary>
		/// Overlay number.
		/// </summary>
		public ushort ovlNumber;
		/// <summary>
		/// Index into sstLibraries subsection if this module was linked from a library
		/// </summary>
		public ushort iLib;
		/// <summary>
		/// Debugging style for this module. Currently only "CV" is defined. A
		/// module can have only one debugging style. If a module contains
		/// debugging information in an unrecognized style, the information will
		/// be discarded.
		/// </summary>
		public ushort Style;
		/// <summary>
		/// Detailed information about each segment to which code is
		/// contributed. This is an array of cSeg count segment information
		/// descriptor structures.
		/// </summary>
		public SegInfo[] Segments;

		public string Name;

		[StructLayout(LayoutKind.Sequential,Pack=1)]
		public struct SegInfo
		{
			/// <summary>
			/// Segment that this structure describes.
			/// </summary>
			public ushort Seg;
			/// <summary>
			/// Padding to maintain alignment This field is reserved for future use
			/// and must be emitted as zeroes.
			/// </summary>
			ushort pad;
			/// <summary>
			/// Offset in segment where the code starts.
			/// </summary>
			public uint offset;
			/// <summary>
			/// Count or number of bytes of code in the segment.
			/// </summary>
			public uint cbSeg;
		}

		public override void Read(BinaryReader reader)
		{
			ovlNumber = reader.ReadUInt16();
			iLib = reader.ReadUInt16();
			var cSeg = reader.ReadUInt16();
			Style = reader.ReadUInt16();

			Segments = new SegInfo[cSeg];
			for (int i = 0; i < cSeg; i++)
				Segments[i] = Misc.FromBinaryReader<SegInfo>(reader);

			Name = reader.ReadString();
		}
	}

	/// <summary>
	/// The linker emits one of these subsections for every object file that contains a $$TYPES segment.
	/// CVPACK combines all of these subsections into an sstGlobalTypes subsection and deletes the
	/// sstTypes tables. The sstTypes table contains the contents of the $$TYPES segment, except that
	/// addresses within the $$TYPES segment have been fixed by the linker. (See also sstPreComp.)
	/// </summary>
	public class sstTypes : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// The linker fills each subsection of this type with entries for the public symbols of a module. The
	/// CVPACK utility combines all of the sstPublics subsections into an sstGlobalPub subsection.
	/// This table has been replaced with the sstPublicSym, but is retained for compatibility with
	/// previous linkers.
	/// </summary>
	public class sstPublic : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// This table replaces the sstPublic subsection. The format of the public symbols contained in this
	/// table is that of an S_PUB16 or S_PUB32 symbol. 
	/// This allows an executable to contain both 16:16 and 16:32 public symbols for mixed-mode
	/// executable files. As with symbols sections, public section records must start on a 4-byte
	/// boundary.
	/// </summary>
	public class sstPublicSym : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// The linker emits one of these subsections for every object file that contains a $$SYMBOLS
	/// segment. The sstSymbols table contains the contents of the $$SYMBOLS segment, except that
	/// addresses within the $$SYMBOLS segment have been fixed by the linker. The CVPACK utility
	/// moves global symbols from the sstSymbols subsection to the sstGlobalSum subsection during
	/// packing. When the remaining symbols are written executables, the subsection type is changed to
	/// sstAlignSym.
	/// </summary>
	public class sstSymbols : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// CVPACK writes the remaining unpacked symbols for a module back to the executable in a
	/// subsection of this type. All symbols have been padded to fall on a long word boundary, and the
	/// lexical scope linkage fields have been initialized.
	/// </summary>
	public class sstAlignSym : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// The linker fills in each subsection of this type with information obtained from any LINNUM
	/// records in the module. This table has been replaced by the sstSrcModule, but is retained for
	/// compatibility with previous linkers. 
	/// CVPACK rewrites sstSrcLnSeg tables to sstSrcModule tables.
	/// </summary>
	public class sstSrcLnSeg : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// The following table describes the source line number for addressing mapping information for a
	/// module. The table permits the description of a module containing multiple source files with
	/// each source file contributing code to one or more code segments. The base addresses of the
	/// tables described below are all relative to the beginning of the sstSrcModule table.
	/// </summary>
	public class sstSrcModule : SubsectionData
	{
		/// <summary>
		/// An array of base offsets from the beginning of the sstSrcModule table.
		/// </summary>
		public uint[] baseSrcFile;

		public uint[] starts;
		public uint[] ends;

		/// <summary>
		/// An array of segment indices that receive code from this module
		/// </summary>
		public ushort[] segmentIndices;

		public SourceFileInformation[] FileInfo;

		/// <summary>
		/// The file table describes the code segments that receive code from each source file.
		/// </summary>
		public struct SourceFileInformation
		{
			/// <summary>
			/// An array of offsets for the line/address mapping tables for each of the
			/// segments that receive code from this source file.
			/// Only required for initial analysis.
			/// </summary>
			public uint[] baseSrcLn;

			public uint[] segmentStartOffsets;
			public uint[] segmentEndOffsets;

			public string SourceFileName;

			public SourceSegmentInfo[] Segments;

			public override string ToString()
			{
				return SourceFileName;
			}
		}

		public struct SourceSegmentInfo
		{
			public ushort SegmentId;

			public uint[] Offsets;
			public ushort[] Lines;
		}

		public override void Read(BinaryReader r)
		{
			var sstSrcModuleBaseAddress = r.BaseStream.Position;

			// Number of source files contributing code to segments.
			var cFile = r.ReadUInt16();
			// Number of code segments receiving code from this module.
			var cSeg = r.ReadUInt16();

			baseSrcFile = new uint[cFile];
			for (int i = 0; i < cFile; i++)
				baseSrcFile[i] = r.ReadUInt32();

			/*
			 * An array of two 32-bit offsets per segment that receives code from
			 * this module. The first offset is the offset within the segment of the
			 * first byte of code from this module. The second offset is the ending
			 * address of the code from this module. The order of these pairs
			 * corresponds to the ordering of the segments in the seg array. Zeroes
			 * in these entries means that the information is not known, and the file
			 * and line tables described below need to be examined to determine if
			 * an address of interest is contained within the code from this module.
			 */
			starts = new uint[cSeg];
			ends = new uint[cSeg];
			for (int i = 0; i < cSeg; i++)
			{
				starts[i] = r.ReadUInt32();
				ends[i] = r.ReadUInt32();
			}

			segmentIndices=new ushort[cSeg];
			for (int i = 0; i < cSeg; i++)
				segmentIndices[i] = r.ReadUInt16();

			//if (cSeg % 2==1) r.ReadUInt16();

			FileInfo=new SourceFileInformation[cFile];

			for (int i = 0; i < cFile; i++)
			{
				r.BaseStream.Position = sstSrcModuleBaseAddress + baseSrcFile[i];

				var sfi=new SourceFileInformation();

				var cSeg2 = r.ReadUInt16();
				var pad=r.ReadUInt16(); // pad

				if (pad != 0)
					throw new Exception("Wrong pad value!");

				sfi.baseSrcLn = new uint[cSeg2];

				for (int j = 0; j < cSeg2; j++)
					sfi.baseSrcLn[j] = r.ReadUInt32();

				sfi.segmentStartOffsets=new uint[cSeg2];
				sfi.segmentEndOffsets = new uint[cSeg2];
				for (int j = 0; j < cSeg2; j++)
				{
					sfi.segmentStartOffsets[j] = r.ReadUInt32();
					sfi.segmentEndOffsets[j] = r.ReadUInt32();
				}

				sfi.SourceFileName = r.ReadString();


				sfi.Segments = new SourceSegmentInfo[cSeg2];
				for (int j = 0; j < cSeg2; j++)
				{
					r.BaseStream.Position = sstSrcModuleBaseAddress + sfi.baseSrcLn[j];

					var ssi = new SourceSegmentInfo();

					ssi.SegmentId = r.ReadUInt16();
					var cPair = r.ReadUInt16();

					ssi.Offsets = new uint[cPair];
					for (int k = 0; k < cPair; k++)
						ssi.Offsets[k] = r.ReadUInt32();

					ssi.Lines = new ushort[cPair];
					for (int k = 0; k < cPair; k++)
						ssi.Lines[k] = r.ReadUInt16();

					sfi.Segments[j] = ssi;
				}

				FileInfo[i] = sfi;
			}
		}
	}

	/// <summary>
	/// There can be at most one sstLibraries SubSection. The format is an array of length-prefixed
	/// names, which define all the library files used during linking. The order of this list defines the
	/// library index number (seethe sstModules subsection). The first entry should be empty, i.e., a
	/// zero-length string, because library indices are 1-based.
	/// </summary>
	public class sstLibraries : SubsectionData
	{
		public string[] Libraries;

		public override void Read(BinaryReader reader)
		{
			var libs = new List<string>();

			var absEndOffset = reader.BaseStream.Position + Header.Size;
			
			while(reader.BaseStream.Position<absEndOffset)
				libs.Add(reader.ReadString());

			Libraries = libs.ToArray();
		}
	}

	/// <summary>
	/// This subsection contains globally compacted symbols. The format of the table is a header
	/// specifying the symbol and address hash functions, the length of the symbol information, the
	/// length of the symbol hash function data, and the length of address hash function data. This is
	/// followed by the symbol information, which followed by the symbol hash tables, and then
	/// followed by the address hash tables. When the pack utility writes the sstGlobals subsection,
	/// each symbol is zero-padded such that the following symbol starts on a long boundary, and the
	/// length field is adjusted by the pad count. Note that symbol and/or address hash data can be
	/// discarded and the globally packed symbols are linearly searched. A hash function index 0
	/// means that no hash data exists.
	/// </summary>
	public class sstGlobalSym : SubsectionData
	{
		/// <summary>
		/// Index of the symbol hash function.
		/// </summary>
		public ushort symhash;
		/// <summary>
		/// Index of the address hash function.
		/// </summary>
		public ushort addrhash;
		/// <summary>
		/// Count or number of bytes in the symbol table.
		/// </summary>
		public uint cbSymbol;
		/// <summary>
		/// Count or number of bytes in the symbol hash table.
		/// </summary>
		public uint cbSymHash;
		/// <summary>
		/// Count or number of bytes in the address hashing table.
		/// </summary>
		public uint cbAddrHash;

		public override void Read(BinaryReader reader)
		{
			symhash = reader.ReadUInt16();
			addrhash = reader.ReadUInt16();
			cbSymbol = reader.ReadUInt32();
			cbSymHash = reader.ReadUInt32();
			cbAddrHash = reader.ReadUInt32();
		}

		public struct HashTable
		{
			/// <summary>
			/// Each ulong entry is a file offset from the beginning of
			/// the chain table to the first chain item for each hash
			/// bucket.
			/// </summary>
			public uint[] firstBucketAddresses;
			/// <summary>
			/// Each ulong entry is the count of items in the chain for
			/// each hash bucket.
			/// </summary>
			public uint[] bucketSizes;
		}
	}

	/// <summary>
	/// This subsection contains the globally compacted public symbols from the sstPublics. 
	/// The format of the table is a header specifying the symbol and address hash functions, 
	/// the length of the symbol information, the length of the symbol hash function data, 
	/// and the length of address hash function data. 
	/// This is followed by symbol information, which is followed by the symbol hash
	/// tables, and then followed by the address hash tables. 
	/// When the pack utility writes the sstGlobals subsection, 
	/// each symbol is zero-padded such that the following symbol 
	/// starts on a long boundary, and the length field of 
	/// the symbol is adjusted by the pad count. 
	/// Note that symbol and/or address hash data can be discarded 
	/// and the globally packed symbolscan be linearly searched in low-memory situations. 
	/// A hash function index 0 means that no hash data exists.
	/// </summary>
	public class sstGlobalPub : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	

	/// <summary>
	/// This table is emitted by the Pcode MPC program when a segmented executable is processed into
	/// a non-segmented executable file. 
	/// The table contains the mapping from segment indices to frame numbers.
	/// </summary>
	public class sstMPC : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// This table contains the mapping between the logical segment indices used in the symbol table
	/// and the physical segments where the program was loaded.
	/// 
	/// There is one sstSegMap per executable or DLL.
	/// </summary>
	public class sstSegMap : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// The sstSegName table contains all of the logical segment and class names. The table is an array
	/// of zero-terminated strings. Each string is indexed by its beginning from the start of the table.
	/// See sstSegMap.
	/// </summary>
	public class sstSegName : SubsectionData
	{
		public string[] Names;

		public override void Read(BinaryReader reader)
		{
			var absEndOffset = reader.BaseStream.Position + Header.Size;

			var names=new List<string>();

			var s = "";

			while (reader.BaseStream.Position < absEndOffset)
			{
				if (reader.PeekChar() == 0)
				{
					reader.ReadByte();
					names.Add(s);
					s = "";
				}
				else
					s += (char)reader.ReadByte();
			}

			Names = names.ToArray();
		}
	}

	/// <summary>
	/// The linker emits one of these sections for every OMF object that has the $$TYPES table flagged
	/// as sstPreComp and for every COFF object that contains a .debug$P section. During packing, the
	/// CVPACK utility processes modules with a types table having the sstPreComp index before
	/// modules with types table having the sstTypes index.
	/// </summary>
	public class sstPreComp : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// This subsection contains a list of all of the sources files that contribute code to any module
	/// (compiland) in the executable. File names are partially qualified relative to the compilation
	/// directory.
	/// </summary>
	public class sstFileIndex : SubsectionData
	{
		/// <summary>
		/// Count or number of modules in the executable.
		/// </summary>
		public uint moduleCount;
		/// <summary>
		/// Count or total number of file name references.
		/// </summary>
		public uint filenameRefCount;
		/// <summary>
		/// Array of indices into the NameOffset table for each module. 
		/// Each index is the start of the file name references for each module.
		/// </summary>
		public ushort[] modStart;
		/// <summary>
		/// Number of file name references per module.
		/// </summary>
		public ushort[] modReferenceCount;
		/// <summary>
		/// Array of offsets into the Names table. For each module, the offset to
		/// first referenced file name is at NameRef[ModStart] and continues for
		/// modReferenceCount entries.
		/// </summary>
		public uint[] nameRef;
		/// <summary>
		/// List of zero-terminated file names. Each file name is partially
		/// qualified relative to the compilation directory
		/// </summary>
		public string[] names;

		public override void Read(BinaryReader r)
		{
			var initPos = r.BaseStream.Position;

			moduleCount = r.ReadUInt16();

			filenameRefCount = r.ReadUInt16();

			modStart = new ushort[moduleCount];
			for (int i = 0; i < moduleCount; i++)
				modStart[i] = r.ReadUInt16();

			modReferenceCount = new ushort[moduleCount];
			for (int i = 0; i < moduleCount; i++)
				modReferenceCount[i] = r.ReadUInt16();

			nameRef = new uint[filenameRefCount];
			for (int i = 0; i < filenameRefCount; i++)
				nameRef[i] = r.ReadUInt32();

			var remainingBytes = Header.Size - (r.BaseStream.Position - initPos);

			var names = new List<string>();

			for (var i = remainingBytes; i != 0; )
			{
				var s = r.ReadString();

				if (string.IsNullOrEmpty(s))
					break;

				names.Add(s);
				remainingBytes -= 1 + s.Length;
			}

			this.names = names.ToArray();
		}
	}

	/// <summary>
	/// This subsection is structured exactly like the sstGlobalPub and sstGlobalSym subsections. It
	/// contains S_PROCREF for all static functions, as well as S_DATAREF for static module level
	/// data and non-static data that could not be included (due to type conflicts) in the sstGlobalSym
	/// subsection.
	/// </summary>
	public class sstStaticSym : SubsectionData
	{

		public override void Read(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
	}
}
