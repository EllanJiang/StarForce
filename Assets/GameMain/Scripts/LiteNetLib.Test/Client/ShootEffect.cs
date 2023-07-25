/*
* 文件名：ShootEffect
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/25 12:02:39
* 修改记录：
*/

using System;
using LiteNetLib.Test.Shared;
using UnityEngine;

namespace LiteNetLib.Test.Client
{
    /// <summary>
    /// 激光特效
    /// </summary>
    public class ShootEffect : MonoBehaviour
    {
        [SerializeField] private LineRenderer _trailRenderer;
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioSource _target;
        [SerializeField] private AudioClip[] _shootClips;
        [SerializeField] private AudioClip[] _hitClips;

        private GameTimer _aliveTimer = new GameTimer(0.3f);
        private Action<ShootEffect> _onDeathCallback;
        private Vector3[] _positions = new Vector3[2];
    
        public void Init(Action<ShootEffect> onDeathCallback)
        {
            _onDeathCallback = onDeathCallback;
            gameObject.SetActive(false);
        }
    
        public void Spawn(Vector2 from, Vector2 to)
        {
            _source.transform.position = from;
            _target.transform.position = to;

            _trailRenderer.transform.position = from;
            _positions[0] = from;
            _positions[1] = to;
            _trailRenderer.SetPositions(_positions);
            gameObject.SetActive(true);
        
            _source.PlayOneShot(_shootClips.GetRandomElement());
            _target.PlayOneShot(_hitClips.GetRandomElement());
        }

        private void OnDeath()
        {
            _onDeathCallback(this);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            _aliveTimer.Update(Time.deltaTime, OnDeath);
            float t1 = _aliveTimer.Time / _aliveTimer.MaxTime;
            float t2 = _aliveTimer.Time / (_aliveTimer.MaxTime * 2f);
            Color a = new Color(1f, 1f, 0f, 1f);
            Color b = new Color(1f, 1f, 0f, 0f);
            _trailRenderer.startColor = Color.Lerp(a, b, t1);
            _trailRenderer.endColor = Color.Lerp(a, b, t2);
        }
    }

}