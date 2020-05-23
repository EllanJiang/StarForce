//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace StarForce
{
    public class VersionInfo
    {
        public bool ForceGameUpdate
        {
            get;
            set;
        }

        public string LatestGameVersion
        {
            get;
            set;
        }

        public int InternalGameVersion
        {
            get;
            set;
        }

        public int InternalResourceVersion
        {
            get;
            set;
        }

        public string GameUpdateUrl
        {
            get;
            set;
        }

        public int VersionListLength
        {
            get;
            set;
        }

        public int VersionListHashCode
        {
            get;
            set;
        }

        public int VersionListZipLength
        {
            get;
            set;
        }

        public int VersionListZipHashCode
        {
            get;
            set;
        }
    }
}
