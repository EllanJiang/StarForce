/*
* 文件名：VersionInfo
* 文件描述：版本信息
* 作者：aronliang
* 创建时间：2023/05/24 16:58:42
* 修改记录：
*/

[System.Serializable]
public class VersionInfo
{
    public bool ForceGameUpdate;

    public string LatestGameVersion;

    public int InternalGameVersion;

    public int InternalResourceVersion;

    public string GameUpdateUrl;

    public int VersionListLength;

    public int VersionListHashCode;

    public int VersionListCompressedLength;

    public int VersionListCompressedHashCode;
}