using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CodeViewExaminer.CodeView
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SubsectionDirectoryHeader
    {
        /// <summary>
        /// Length of directory header
        /// </summary>
        public ushort HeaderSize;
        /// <summary>
        /// Length of each directory entry.
        /// </summary>
        public ushort DirectoryEntrySize;
        /// <summary>
        /// Number of directory entries.
        /// </summary>
        public uint DirectoryCount;
        /// <summary>
        /// Offset from lfaBase of next directory. This field is currently unused,
        /// but is intended for use by the incremental linker to point to the next
        /// directory containing Symbol and Type OMF information from an
        /// incremental link.
        /// </summary>
        public uint lfoNextDir;
        /// <summary>
        /// Flags describing directory and subsection tables. 
        /// No values have been defined for this field.
        /// </summary>
        public uint flags;
    }

    [StructLayout(LayoutKind.Sequential,Pack=1)]
    public struct DirectoryEntryHeader
    {
        /// <summary>
        /// Subdirectory index. See the table below for a listing of the valid subsection indices.
        /// </summary>
        public SubsectionType Type;
        /// <summary>
        /// Module index. This number is 1 based and zero (0) is never a valid
        /// index. The index = 0xffff is reserved for tables that are not associated
        /// with a specific module. These tables include sstLibraries,
        /// sstGlobalSym, sstGlobalPub, and sstGlobalTypes.
        /// </summary>
        public ushort iMod;
        /// <summary>
        /// Offset from the base address lfaBase.
        /// </summary>
        public uint ContentOffset;
        /// <summary>
        /// Number of bytes in subsection.
        /// </summary>
        public uint Size;
    }

    public enum SubsectionType : ushort
    {
        sstModule =0x120,
        sstTypes =0x121,
        sstPublic = 0x122,
        sstPublicSym = 0x123,
        sstSymbols = 0x124,
        sstAlignSym = 0x125,
        sstSrcLnSeg = 0x126,
        sstSrcModule = 0x127,
        sstLibraries = 0x128,
        sstGlobalSym = 0x129,
        sstGlobalPub = 0x12a,
        sstGlobalTypes = 0x12b,
        sstMPC = 0x12c,
        sstSegMap = 0x12d,
        sstSegName = 0x12e,
        sstPreComp = 0x12f,
        unused = 0x130,
        reserved = 0x131,
        reserved2 = 0x132,
        sstFileIndex = 0x133,
        sstStaticSym = 0x134,
    }
}
