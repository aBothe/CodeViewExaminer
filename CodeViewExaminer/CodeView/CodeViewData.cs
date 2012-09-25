using System;
using System.Collections.Generic;
using System.Text;

namespace CodeViewExaminer.CodeView
{
	public class CodeViewData
	{
        /// <summary>
        /// The absolute base address for CV-related things
        /// </summary>
        public long lfaBase;
        /// <summary>
        /// Offset of the subsection directory.
        /// </summary>
        public uint lfoDirectory;

        public SubsectionDirectory SubsectionDirectory;
	}
}
