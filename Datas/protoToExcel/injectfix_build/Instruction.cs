/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

namespace IFix.Core
{
    public enum Code
    {
        Cpblk,
        Xor,
        Pop,
        Ldind_I1,
        Ldc_I8,
        Shr_Un,
        Ble,
        Ldflda,
        Conv_Ovf_I4_Un,
        Stelem_I2,
        Conv_U2,
        Stind_I4,
        Sub_Ovf_Un,
        Conv_I8,
        Stind_I2,
        Stelem_I,
        Ldelem_U4,
        Ldelem_I1,
        Call,
        Mul_Ovf_Un,
        Localloc,
        Newobj,
        Conv_Ovf_U2,
        Conv_Ovf_I2_Un,
        Conv_R8,
        Ldind_I4,
        Ldloc,
        Ldc_R4,
        Stelem_Ref,
        Conv_U4,
        Starg,
        Conv_Ovf_I,
        Throw,
        Beq,
        Stelem_R8,
        Unbox_Any,
        Brtrue,
        Conv_Ovf_I4,
        Ldind_U4,
        Tail,
        Ldlen,
        Conv_U,
        Sub_Ovf,
        Stelem_I1,
        Newarr,
        Blt_Un,
        Stelem_I4,
        Conv_I4,
        Rem,
        Ldelem_I8,
        Or,
        Initobj,
        Conv_Ovf_U_Un,
        Add_Ovf_Un,
        Callvirt,
        Ldind_R4,
        Mul_Ovf,
        Conv_Ovf_U4_Un,
        Nop,
        Ldelem_Ref,
        Ckfinite,
        Conv_Ovf_I1_Un,
        Jmp,
        Bgt_Un,
        CallExtern,
        Div,
        Conv_Ovf_U1,
        Add,
        Stsfld,
        Callvirtvirt,
        Stind_I8,
        Br,
        Stfld,
        Stloc,
        Stind_I1,
        Ceq,
        Conv_Ovf_U1_Un,
        Unbox,
        Stelem_Any,
        Break,
        Initblk,
        Ldelem_Any,
        Ldc_I4,
        Div_Un,
        Sub,
        Ldind_I,
        Stind_R4,
        Ldelem_U1,
        Conv_Ovf_I8,
        Stind_Ref,
        Ldc_R8,
        Bne_Un,
        Ret,
        Clt_Un,
        Ldvirtftn2,
        Not,
        Conv_I,
        Stelem_R4,
        //Calli,
        Isinst,
        Ldelem_U2,
        Conv_Ovf_I1,
        Ldtype, // custom
        No,
        Bge,
        Conv_I1,
        Clt,
        Cpobj,
        Stobj,
        Mkrefany,
        Ble_Un,
        Conv_U8,
        Dup,
        Bgt,
        Ldelema,
        Conv_Ovf_U4,
        Ldind_Ref,
        Ldelem_I,
        Ldelem_I2,
        Mul,
        Conv_R_Un,
        Ldftn,
        Cgt,
        Leave,
        Conv_U1,
        Ldelem_R4,
        Ldelem_R8,
        Rethrow,
        Conv_Ovf_U8_Un,
        Shr,
        Conv_Ovf_I_Un,
        Conv_Ovf_U8,
        Cgt_Un,
        Box,
        Switch,
        Refanyval,
        Stelem_I8,
        Volatile,
        Constrained,
        Ldstr,
        Conv_Ovf_I2,
        Ldind_I8,
        Stind_R8,
        Ldelem_I4,
        Ldind_R8,
        Conv_R4,
        Ldind_I2,
        Brfalse,
        Ldnull,
        Ldsflda,
        Bge_Un,
        Ldobj,
        Readonly,
        And,
        Refanytype,
        Endfinally,
        Ldvirtftn,
        Neg,
        Ldsfld,
        Ldtoken,
        Ldind_U1,
        Stind_I,
        Ldarg,
        Sizeof,
        Ldarga,
        Conv_Ovf_I8_Un,

        //Pseudo instruction
        StackSpace,
        Unaligned,
        Shl,
        Endfilter,
        Conv_I2,
        Conv_Ovf_U2_Un,
        Ldfld,
        Arglist,
        Conv_Ovf_U,
        Rem_Un,
        Ldloca,
        Castclass,
        Ldind_U2,
        Newanon,
        Add_Ovf,
        Blt,
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct Instruction
    {
        /// <summary>
        /// 指令MAGIC
        /// </summary>
        public const ulong INSTRUCTION_FORMAT_MAGIC = 3067433598421528260;

        /// <summary>
        /// 当前指令
        /// </summary>
        public Code Code;

        /// <summary>
        /// 操作数
        /// </summary>
        public int Operand;
    }

    public enum ExceptionHandlerType
    {
        Catch = 0,
        Filter = 1,
        Finally = 2,
        Fault = 4
    }

    public sealed class ExceptionHandler
    {
        public System.Type CatchType;
        public int CatchTypeId;
        public int HandlerEnd;
        public int HandlerStart;
        public ExceptionHandlerType HandlerType;
        public int TryEnd;
        public int TryStart;
    }
}