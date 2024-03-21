/*
* 文件名：InstantiateByAsset
* 文件描述：
* 作者：aronliang
* 创建时间：2023/08/17 19:44:56
* 修改记录：
*/

using UnityEngine;

public class InstantiateByAsset : MonoBehaviour
{
    public string text;

    void Start()
    {
        Debug.Log($"[InstantiateByAsset] text:{text}, 这个脚本通过挂载到资源的方式实例化");
    }
}