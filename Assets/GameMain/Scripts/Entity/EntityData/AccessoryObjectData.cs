using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public abstract class AccessoryObjectData : EntityData
    {
        [SerializeField]
        private int m_OwnerId = 0;

        public AccessoryObjectData(int entityId, int typeId, int ownerId)
            : base(entityId, typeId)
        {
            m_OwnerId = ownerId;
        }

        /// <summary>
        /// 拥有者编号。
        /// </summary>
        public int OwnerId
        {
            get
            {
                return m_OwnerId;
            }
        }
    }
}
