using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public class BulletData : AccessoryObjectData
    {
        [SerializeField]
        private Vector3 m_StartPosition = Vector3.zero;

        [SerializeField]
        private int m_Attack = 0;

        [SerializeField]
        private float m_Speed = 0f;

        [SerializeField]
        private float m_LifeTime = 0f;

        public BulletData(int entityId, int typeId, int ownerId, Vector3 startPosition, int attack)
            : base(entityId, typeId, ownerId)
        {
            m_StartPosition = startPosition;
            m_Attack = attack;
            m_Speed = 20f;
            m_LifeTime = 3f;
        }

        public Vector3 StartPosition
        {
            get
            {
                return m_StartPosition;
            }
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
