//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;

namespace StarForce
{
    public static class WebUtility
    {
        public static string EscapeString(string stringToEscape)
        {
            return Uri.EscapeDataString(stringToEscape);
        }

        public static string UnescapeString(string stringToUnescape)
        {
            return Uri.UnescapeDataString(stringToUnescape);
        }
    }
}
