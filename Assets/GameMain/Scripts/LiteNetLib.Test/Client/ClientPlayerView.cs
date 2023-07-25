/*
* 文件名：ClientPlayerView
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 12:01:35
* 修改记录：
*/

using UnityEngine;

namespace LiteNetLib.Test.Client
{
    /// <summary>
    /// 自己模型
    /// </summary>
    public class ClientPlayerView : MonoBehaviour, IPlayerView
    {
        [SerializeField] private TextMesh _name;
        private ClientPlayer _player;
        private Camera _mainCamera;

        public static ClientPlayerView Create(ClientPlayerView prefab, ClientPlayer player)
        {
            Quaternion rot = Quaternion.Euler(0f, player.Rotation, 0f);
            var obj = Instantiate(prefab, player.Position, rot);
            obj._player = player;
            obj._name.text = player.Name;
            obj._mainCamera = Camera.main;
            return obj;
        }

        private void Update()
        {
            var vert = Input.GetAxis("Vertical");
            var horz = Input.GetAxis("Horizontal");
            var fire = Input.GetAxis("Fire1");
            
            Vector2 velocty = new Vector2(horz, vert);

            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = mousePos - _player.Position;
            float rotation = Mathf.Atan2(dir.y, dir.x);
            _player.SetInput(velocty, rotation, fire > 0f);

            //插值玩家位置
            float lerpT = ClientLogic.LogicTimer.LerpAlpha;
            transform.position = Vector2.Lerp(_player.LastPosition, _player.Position, lerpT); //从上一帧位置插值到当帧位置
            float angle = Mathf.Lerp(_player.LastRotation, _player.Rotation, lerpT);                 //从上一帧的旋转角度插值到当帧的角度
            transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg );
        }
        
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}