using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeViewExaminer.CodeView
{
	/// <summary>
	/// This subsection contains the packed type records for the executable file. The first long word of
	/// the subsection contains the number of types in the table. This count is followed by a count-sized
	/// array of long offsets to the corresponding type record. As the sstGlobalTypes subsection is
	/// written, each type record is forced to start on a long word boundary. However, the length of the
	/// type string is not adjusted by the pad count. The remainder of the subsection contains the type
	/// records. This table is invalid for NB05 signatures.
	/// 
	/// Types are 48-K aligned as well as naturally aligned, so linear traversal of the type table is nontrivial.
	/// The 48-K alignment means that no type record crosses a 48-K boundary.
	/// </summary>
	public class sstGlobalTypes : SubsectionData
	{
		public string[] TypeNames;

		public override void Read(BinaryReader r)
		{
			var sig = r.ReadUInt32();
			var typeCount = r.ReadUInt32();
			var typeOffsets = new uint[typeCount];
			TypeNames = new string[typeCount];

			for (int i = 0; i < typeCount; i++)
				typeOffsets[i] = r.ReadUInt32();

			// For NB09 executables, the type string offset is from the first type record of the sstGlobalTypes subsection
			var typeBaseAddress = r.BaseStream.Position;


			for (int i = 0; i < typeCount; i++)
			{
				r.BaseStream.Position = typeBaseAddress + typeOffsets[i];
				TypeNames[i] = r.ReadString();
			}
		}
	}

	public enum LeafType : ushort
	{
		LF_MODIFIER_V1 = 0x0001
		,LF_POINTER_V1  = 0x0002
		,LF_ARRAY_V1    = 0x0003
		,LF_CLASS_V1    = 0x0004
		,LF_STRUCTURE_V1= 0x0005
		,LF_UNION_V1    = 0x0006
		,LF_ENUM_V1     = 0x0007
		,LF_PROCEDURE_V1= 0x0008
		,LF_MFUNCTION_V1= 0x0009
		,LF_VTSHAPE_V1  = 0x000a
		,LF_COBOL0_V1   = 0x000b
		,LF_COBOL1_V1   = 0x000c
		,LF_BARRAY_V1   = 0x000d
		,LF_LABEL_V1    = 0x000e
		,LF_NULL_V1     = 0x000f
		,LF_NOTTRAN_V1  = 0x0010
		,LF_DIMARRAY_V1 = 0x0011
		,LF_VFTPATH_V1  = 0x0012
		,LF_PRECOMP_V1  = 0x0013
		,LF_ENDPRECOMP_V1 =0x0014
		,LF_OEM_V1      = 0x0015
		,LF_TYPESERVER_V1 = 0x0016

		,LF_MODIFIER_V2 = 0x1001     /* variants with new 32-bit type indices (V2) */
		,LF_POINTER_V2  = 0x1002
		,LF_ARRAY_V2    = 0x1003
		,LF_CLASS_V2    = 0x1004
		,LF_STRUCTURE_V2= 0x1005
		,LF_UNION_V2    = 0x1006
		,LF_ENUM_V2     = 0x1007
		,LF_PROCEDURE_V2= 0x1008
		,LF_MFUNCTION_V2= 0x1009
		,LF_COBOL0_V2   = 0x100a
		,LF_BARRAY_V2   = 0x100b
		,LF_DIMARRAY_V2 = 0x100c
		,LF_VFTPATH_V2  = 0x100d
		,LF_PRECOMP_V2  = 0x100e
		,LF_OEM_V2      = 0x100f

		,LF_SKIP_V1     = 0x0200
		,LF_ARGLIST_V1  = 0x0201
		,LF_DEFARG_V1   = 0x0202
		,LF_LIST_V1     = 0x0203
		,LF_FIELDLIST_V1 = 0x0204
		,LF_DERIVED_V1  = 0x0205
		,LF_BITFIELD_V1 = 0x0206
		,LF_METHODLIST_V1 = 0x0207
		,LF_DIMCONU_V1  = 0x0208
		,LF_DIMCONLU_V1 = 0x0209
		,LF_DIMVARU_V1  = 0x020a
		,LF_DIMVARLU_V1 = 0x020b
		,LF_REFSYM_V1   = 0x020c

		,LF_SKIP_V2     = 0x1200    /* variants with new 32-bit type indices (V2) */
		,LF_ARGLIST_V2  = 0x1201
		,LF_DEFARG_V2   = 0x1202
		,LF_FIELDLIST_V2 = 0x1203
		,LF_DERIVED_V2  = 0x1204
		,LF_BITFIELD_V2 = 0x1205
		,LF_METHODLIST_V2 = 0x1206
		,LF_DIMCONU_V2  = 0x1207
		,LF_DIMCONLU_V2 = 0x1208
		,LF_DIMVARU_V2  = 0x1209
		,LF_DIMVARLU_V2 = 0x120a

		/* Field lists */
		,LF_BCLASS_V1   = 0x0400
		,LF_VBCLASS_V1  = 0x0401
		,LF_IVBCLASS_V1 = 0x0402
		,LF_ENUMERATE_V1= 0x0403
		,LF_FRIENDFCN_V1= 0x0404
		,LF_INDEX_V1    = 0x0405
		,LF_MEMBER_V1   = 0x0406
		,LF_STMEMBER_V1 = 0x0407
		,LF_METHOD_V1   = 0x0408
		,LF_NESTTYPE_V1 = 0x0409
		,LF_VFUNCTAB_V1 = 0x040a
		,LF_FRIENDCLS_V1= 0x040b
		,LF_ONEMETHOD_V1= 0x040c
		,LF_VFUNCOFF_V1 = 0x040d
		,LF_NESTTYPEEX_V1 = 0x040e
		,LF_MEMBERMODIFY_V1 = 0x040f

		,LF_BCLASS_V2   = 0x1400    /* variants with new 32-bit type indices (V2) */
		,LF_VBCLASS_V2  = 0x1401
		,LF_IVBCLASS_V2 = 0x1402
		,LF_FRIENDFCN_V2= 0x1403
		,LF_INDEX_V2    = 0x1404
		,LF_MEMBER_V2   = 0x1405
		,LF_STMEMBER_V2 = 0x1406
		,LF_METHOD_V2   = 0x1407
		,LF_NESTTYPE_V2 = 0x1408
		,LF_VFUNCTAB_V2 = 0x1409
		,LF_FRIENDCLS_V2= 0x140a
		,LF_ONEMETHOD_V2= 0x140b
		,LF_VFUNCOFF_V2 = 0x140c
		,LF_NESTTYPEEX_V2 = 0x140d

		,LF_ENUMERATE_V3= 0x1502
		,LF_ARRAY_V3    = 0x1503
		,LF_CLASS_V3    = 0x1504
		,LF_STRUCTURE_V3= 0x1505
		,LF_UNION_V3    = 0x1506
		,LF_ENUM_V3     = 0x1507
		,LF_MEMBER_V3   = 0x150d
		,LF_STMEMBER_V3 = 0x150e
		,LF_METHOD_V3   = 0x150f
		,LF_NESTTYPE_V3 = 0x1510
		,LF_ONEMETHOD_V3= 0x1511

		,LF_NUMERIC     = 0x8000    /* numeric leaf types */
		,LF_CHAR        = 0x8000
		,LF_SHORT       = 0x8001
		,LF_USHORT      = 0x8002
		,LF_LONG        = 0x8003
		,LF_ULONG       = 0x8004
		,LF_REAL32      = 0x8005
		,LF_REAL64      = 0x8006
		,LF_REAL80      = 0x8007
		,LF_REAL128     = 0x8008
		,LF_QUADWORD    = 0x8009
		,LF_UQUADWORD   = 0x800a
		,LF_REAL48      = 0x800b
		,LF_COMPLEX32   = 0x800c
		,LF_COMPLEX64   = 0x800d
		,LF_COMPLEX80   = 0x800e
		,LF_COMPLEX128  = 0x800f
		,LF_VARSTRING   = 0x8010
	}
}
