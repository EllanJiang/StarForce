using GameFramework.DataTable;
using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public class ThrusterData : AccessoryObjectData
    {
        [SerializeField]
        private float m_Speed = 0f;

        public ThrusterData(int entityId, int typeId, int ownerId)
            : base(entityId, typeId, ownerId)
        {
            IDataTable<DRThruster> dtThruster = GameEntry.DataTable.GetDataTable<DRThruster>();
            DRThruster drThruster = dtThruster.GetDataRow(TypeId);
            if (drThruster == null)
            {
                return;
            }

            m_Speed = drThruster.Speed;
        }

        /// <summary>
        /// 速度。
        /// </summary>
        public float Speed
        {
            get
            {
                return m_Speed;
            }
        }
    }
}
