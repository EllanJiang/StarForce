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

            GameEntry.Sound.PlaySound(10000);
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            CachedTransform.Translate(Vector3.forward * m_BulletData.Speed * elapseSeconds, Space.World);
        }
    }
}
