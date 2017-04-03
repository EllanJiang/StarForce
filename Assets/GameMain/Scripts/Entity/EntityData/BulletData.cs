using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public class BulletData : TargetableObjectData
    {
        [SerializeField]
        private int m_Attack = 0;

        [SerializeField]
        private float m_Speed = 0f;

        private int m_AttackerId;

        public BulletData(int entityId, int typeId, int attackerId, int attack)
            : base(entityId, typeId)
        {
            m_Attack = attack;
            m_Speed = 20f;
            m_AttackerId = attackerId;
        }

        public int AttackerId
        {
            get
            {
                return m_AttackerId;
            }
        }

        public override int MaxHP
        {
            get
            {
                return 0;
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
    }
}
