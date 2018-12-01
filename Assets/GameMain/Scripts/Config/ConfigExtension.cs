//------------------------------------------------------------
// Game Framework v3.x
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using UnityGameFramework.Runtime;

namespace StarForce
{
    public static class ConfigExtension
    {
        public static void LoadConfig(this ConfigComponent configComponent, string configName, object userData = null)
        {
            if (string.IsNullOrEmpty(configName))
            {
                Log.Warning("Config name is invalid.");
                return;
            }

            configComponent.LoadConfig(configName, AssetUtility.GetConfigAsset(configName), Constant.AssetPriority.ConfigAsset, userData);
        }
    }
}
