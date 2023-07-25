/*
* 文件名：RemotePlayerView
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 12:02:18
* 修改记录：
*/

using UnityEngine;

namespace LiteNetLib.Test.Client
{
    /// <summary>
    /// 远端玩家模型
    /// </summary>
    public class RemotePlayerView : MonoBehaviour, IPlayerView
    {
        private RemotePlayer _player;

        public static RemotePlayerView Create(RemotePlayerView prefab, RemotePlayer player)
        {
            Quaternion rot = Quaternion.Euler(0f, player.Rotation, 0f);
            var obj = Instantiate(prefab, player.Position, rot);
            obj._player = player;
            return obj;
        }

        //更新远端玩家位置
        private void Update()
        {
            _player.UpdatePosition(Time.deltaTime);
            transform.position = _player.Position;
            transform.rotation =  Quaternion.Euler(0f, 0f, _player.Rotation * Mathf.Rad2Deg );
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}