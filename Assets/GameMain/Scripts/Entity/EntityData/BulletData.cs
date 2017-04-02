using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public class BulletData : AccessoryObjectData
    {
        [SerializeField]
        private int m_Attack = 0;

        [SerializeField]
        private float m_Speed = 0f;

        [SerializeField]
        private float m_LifeTime = 0f;

        public BulletData(int entityId, int typeId, int ownerId, int attack)
            : base(entityId, typeId, ownerId)
        {
            m_Attack = attack;
            m_Speed = 20f;
            m_LifeTime = 3f;
        }

        public int Attack
        {
            get
            {
                return m_Attack;
            }
        }

        public float Speed
        {
            get
            {
                return m_Speed;
            }
        }

        public float LifeTime
        {
            get
            {
                return m_LifeTime;
            }
        }
    }
}
