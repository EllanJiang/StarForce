namespace AirForce
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public static partial class GameEntry
    {
        public static ConfigComponent Config
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            Config = UnityGameFramework.Runtime.GameEntry.GetComponent<ConfigComponent>();
        }
    }
}
