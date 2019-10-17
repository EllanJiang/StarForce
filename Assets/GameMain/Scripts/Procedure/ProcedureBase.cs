//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace StarForce
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        public abstract bool UseNativeDialog
        {
            get;
        }
    }
}
