using System;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public class MyAircraftData : AircraftData
    {
        [SerializeField]
        private string m_Name = null;

        public MyAircraftData(int entityId, int typeId)
            : base(entityId, typeId)
        {

        }

        /// <summary>
        /// 角色名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
    }
}
