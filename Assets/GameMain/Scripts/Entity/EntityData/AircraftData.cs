using GameFramework.DataTable;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AirForce
{
    [Serializable]
    public abstract class AircraftData : TargetableObjectData
    {
        [SerializeField]
        private ThrusterData m_ThrusterData = null;

        [SerializeField]
        private List<WeaponData> m_WeaponDatas = new List<WeaponData>();

        [SerializeField]
        private List<ArmorData> m_ArmorDatas = new List<ArmorData>();

        [SerializeField]
        private int m_MaxHP = 0;

        [SerializeField]
        private int m_Defense = 0;

        public AircraftData(int entityId, int typeId)
            : base(entityId, typeId)
        {
            IDataTable<DRAircraft> dtAircraft = GameEntry.DataTable.GetDataTable<DRAircraft>();
            DRAircraft drAircraft = dtAircraft.GetDataRow(TypeId);
            if (drAircraft == null)
            {
                return;
            }

            m_ThrusterData = new ThrusterData(GameEntry.Entity.GenerateSerialId(), drAircraft.ThrusterId, Id);

            for (int index = 0, weaponId = 0; (weaponId = drAircraft.GetWeaponIds(index)) > 0; index++)
            {
                AttachWeaponData(new WeaponData(GameEntry.Entity.GenerateSerialId(), weaponId, Id));
            }

            for (int index = 0, armorId = 0; (armorId = drAircraft.GetArmorIds(index)) > 0; index++)
            {
                AttachArmorData(new ArmorData(GameEntry.Entity.GenerateSerialId(), armorId, Id));
            }

            HP = m_MaxHP;
        }

        /// <summary>
        /// 最大生命。
        /// </summary>
        public override int MaxHP
        {
            get
            {
                return m_MaxHP;
            }
        }

        /// <summary>
        /// 防御。
        /// </summary>
        public int Defense
        {
            get
            {
                return m_Defense;
            }
        }

        /// <summary>
        /// 速度。
        /// </summary>
        public float Speed
        {
            get
            {
                return m_ThrusterData.Speed;
            }
        }

        public ThrusterData GetThrusterData()
        {
            return m_ThrusterData;
        }

        public List<WeaponData> GetAllWeaponDatas()
        {
            return m_WeaponDatas;
        }

        public void AttachWeaponData(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                return;
            }

            if (m_WeaponDatas.Contains(weaponData))
            {
                return;
            }

            m_WeaponDatas.Add(weaponData);
        }

        public void DetachWeaponData(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                return;
            }

            m_WeaponDatas.Remove(weaponData);
        }

        public List<ArmorData> GetAllArmorDatas()
        {
            return m_ArmorDatas;
        }

        public void AttachArmorData(ArmorData armorData)
        {
            if (armorData == null)
            {
                return;
            }

            if (m_ArmorDatas.Contains(armorData))
            {
                return;
            }

            m_ArmorDatas.Add(armorData);
            RefreshData();
        }

        public void DetachArmorData(ArmorData armorData)
        {
            if (armorData == null)
            {
                return;
            }

            m_ArmorDatas.Remove(armorData);
            RefreshData();
        }

        private void RefreshData()
        {
            m_MaxHP = 0;
            m_Defense = 0;
            for (int i = 0; i < m_ArmorDatas.Count; i++)
            {
                m_MaxHP += m_ArmorDatas[i].MaxHP;
                m_Defense += m_ArmorDatas[i].Defense;
            }

            if (HP > m_MaxHP)
            {
                HP = m_MaxHP;
            }
        }
    }
}
