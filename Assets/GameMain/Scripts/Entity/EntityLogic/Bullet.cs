using GameFramework;
using UnityEngine;

namespace AirForce
{
    /// <summary>
    /// 子弹类。
    /// </summary>
    public class Bullet : Entity
    {
        [SerializeField]
        private BulletData m_BulletData = null;

        private float m_ElapseSeconds = 0f;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_BulletData = userData as BulletData;
            if (m_BulletData == null)
            {
                Log.Error("Bullet data is invalid.");
                return;
            }

            m_ElapseSeconds = 0f;
            GameEntry.Sound.PlaySound(10000);
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            m_ElapseSeconds += elapseSeconds;
            if (m_ElapseSeconds >= m_BulletData.LifeTime)
            {
                GameEntry.Entity.HideEntity(Entity);
                return;
            }

            CachedTransform.Translate(Vector3.forward * m_BulletData.Speed * elapseSeconds, Space.World);
        }
    }
}
