using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AirForce
{
    /// <summary>
    /// 武器类。
    /// </summary>
    public class Weapon : Entity
    {
        private const string AttachPoint = "Weapon Point";

        [SerializeField]
        private WeaponData m_WeaponData = null;

        private float m_NextAttackTime = 0f;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_WeaponData = userData as WeaponData;
            if (m_WeaponData == null)
            {
                Log.Error("Weapon data is invalid.");
                return;
            }

            GameEntry.Entity.AttachEntity(Entity, m_WeaponData.OwnerId, AttachPoint);
        }

        protected internal override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            Name = string.Format("Weapon of {0}", parentEntity.Name);
            CachedTransform.localPosition = Vector3.zero;
        }

        public void TryAttack()
        {
            if (Time.time < m_NextAttackTime)
            {
                return;
            }

            m_NextAttackTime = Time.time + m_WeaponData.AttackInterval;
            GameEntry.Entity.ShowBullet(new BulletData(GameEntry.Entity.GenerateSerialId(), m_WeaponData.BulletId, Id, m_WeaponData.Attack)
            {
                Position = CachedTransform.position,
            });
        }
    }
}
