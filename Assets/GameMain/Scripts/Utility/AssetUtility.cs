namespace StarForce
{
    public static class AssetUtility
    {
        public static string GetDataTableAsset(string assetName)
        {
            return string.Format("Assets/GameMain/DataTables/{0}.txt", assetName);
        }

        public static string GetDictionaryAsset(string assetName)
        {
            return string.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.xml", GameEntry.Localization.Language.ToString(), assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return string.Format("Assets/GameMain/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return string.Format("Assets/GameMain/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return string.Format("Assets/GameMain/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return string.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return string.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetSpriteAsset(string assetName)
        {
            return string.Format("Assets/GameMain/UI/Sprites/{0}.png", assetName);
        }

        public static string GetTextureAsset(string assetName)
        {
            return string.Format("Assets/GameMain/UI/Textures/{0}.png", assetName);
        }
    }
}
