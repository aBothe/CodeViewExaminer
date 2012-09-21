using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeViewExaminer.CodeView
{
    public class SubsectionDirectory
    {
        public SubsectionDirectoryHeader Header;
        public SubsectionData[] Sections;

        public static SubsectionDirectory Read(long lfaBase,BinaryReader r)
        {
            // Read directory section header
            var hdr = Misc.FromBinaryReader<SubsectionDirectoryHeader>(r);

            var sd = new SubsectionDirectory {
                Header=hdr
            };

            // Read entry headers
            var eheaders = new DirectoryEntryHeader[(int)hdr.DirectoryCount];
            for (int i = 0; i < hdr.DirectoryCount; i++)
                eheaders[i]=Misc.FromBinaryReader<DirectoryEntryHeader>(r);

            // Read directory entry contents
            sd.Sections = new SubsectionData[hdr.DirectoryCount];
            for(int i=0;i<hdr.DirectoryCount;i++)
                sd.Sections[i] = SubsectionData.Read(lfaBase, eheaders[i], r);

            return sd;
        }
    }
}
