using GameFramework.DataTable;
using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public class AsteroidData : TargetableObjectData
    {
        [SerializeField]
        private int m_MaxHP = 0;

        [SerializeField]
        private float m_Speed = 0f;

        public AsteroidData(int entityId, int typeId)
            : base(entityId, typeId)
        {
            IDataTable<DRAsteroid> dtAsteroid = GameEntry.DataTable.GetDataTable<DRAsteroid>();
            DRAsteroid drAsteroid = dtAsteroid.GetDataRow(TypeId);
            if (drAsteroid == null)
            {
                return;
            }

            m_MaxHP = drAsteroid.MaxHP;
            m_Speed = drAsteroid.Speed;
        }

        public override int MaxHP
        {
            get
            {
                return m_MaxHP;
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
