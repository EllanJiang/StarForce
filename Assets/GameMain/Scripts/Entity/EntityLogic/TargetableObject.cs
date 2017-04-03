using GameFramework;
using UnityEngine;

namespace AirForce
{
    /// <summary>
    /// 可作为目标的实体类。
    /// </summary>
    public abstract class TargetableObject : Entity
    {
        [SerializeField]
        private TargetableObjectData m_TargetableObjectData = null;

        public abstract ImpactData GetImpactData();

        public abstract void ApplyImpact(ImpactData impactData);

        protected void ApplyDamage(int damageHP)
        {
            m_TargetableObjectData.HP -= damageHP;
            if (m_TargetableObjectData.HP <= 0)
            {
                OnDead();
            }
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            CachedTransform.SetLayerRecursively(Constant.Layer.TargetableObjectLayerId);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_TargetableObjectData = userData as TargetableObjectData;
            if (m_TargetableObjectData == null)
            {
                Log.Error("Targetable object data is invalid.");
                return;
            }
        }

        protected virtual void OnImpact(Entity entity)
        {

        }

        protected virtual void OnDead()
        {
            GameEntry.Entity.HideEntity(Entity);
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject go = other.gameObject;
            Entity entity = go.GetComponent<Entity>();
            if (entity == null)
            {
                return;
            }

            OnImpact(entity);
        }
    }
}
