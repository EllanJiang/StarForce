using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public abstract class TargetableObjectData : EntityData
    {
        [SerializeField]
        private CampType m_Camp = CampType.Player;

        [SerializeField]
        private int m_HP = 0;

        public TargetableObjectData(int entityId, int typeId)
            : base(entityId, typeId)
        {

        }

        /// <summary>
        /// 角色阵营。
        /// </summary>
        public CampType Camp
        {
            get
            {
                return m_Camp;
            }
            set
            {
                m_Camp = value;
            }
        }

        /// <summary>
        /// 当前生命。
        /// </summary>
        public int HP
        {
            get
            {
                return m_HP;
            }
            set
            {
                m_HP = value;
            }
        }

        /// <summary>
        /// 最大生命。
        /// </summary>
        public abstract int MaxHP
        {
            get;
        }

        /// <summary>
        /// 生命百分比。
        /// </summary>
        public float HPRatio
        {
            get
            {
                return MaxHP > 0 ? (float)HP / MaxHP : 0f;
            }
        }
    }
}
