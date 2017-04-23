using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    /// <summary>
    /// 装甲类。
    /// </summary>
    public class Armor : Entity
    {
        private const string AttachPoint = "Armor Point";

        [SerializeField]
        private ArmorData m_ArmorData = null;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_ArmorData = userData as ArmorData;
            if (m_ArmorData == null)
            {
                Log.Error("Armor data is invalid.");
                return;
            }

            GameEntry.Entity.AttachEntity(Entity, m_ArmorData.OwnerId, AttachPoint);
        }

        protected internal override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            Name = string.Format("Armor of {0}", parentEntity.Name);
            CachedTransform.localPosition = Vector3.zero;
        }
    }
}
