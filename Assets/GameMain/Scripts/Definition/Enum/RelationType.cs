//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace StarForce
{
    /// <summary>
    /// 关系类型。
    /// </summary>
    public enum RelationType : byte
    {
        /// <summary>
        /// 未知的。
        /// </summary>
        Unknown,

        /// <summary>
        /// 友好的。
        /// </summary>
        Friendly,

        /// <summary>
        /// 中立的。
        /// </summary>
        Neutral,

        /// <summary>
        /// 敌对的。
        /// </summary>
        Hostile
    }
}
