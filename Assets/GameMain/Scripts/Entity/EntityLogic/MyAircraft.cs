using GameFramework;
using UnityEngine;

namespace AirForce
{
    public class MyAircraft : Aircraft
    {
        [SerializeField]
        private MyAircraftData m_MyAircraftData = null;

        [SerializeField]
        private Rect m_Boundary = new Rect(-6f, -4f, 12f, 12f);

        private Vector3 m_TargetPosition = Vector3.zero;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_MyAircraftData = userData as MyAircraftData;
            if (m_MyAircraftData == null)
            {
                Log.Error("My aircraft data is invalid.");
                return;
            }
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (Input.GetMouseButton(0))
            {
                Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                m_TargetPosition = new Vector3(point.x, 0f, point.z);

                for (int i = 0; i < m_Weapons.Count; i++)
                {
                    m_Weapons[i].TryAttack();
                }
            }

            Vector3 direction = m_TargetPosition - CachedTransform.localPosition;
            if (direction.sqrMagnitude <= Vector3.kEpsilon)
            {
                return;
            }

            Vector3 speed = Vector3.ClampMagnitude(direction.normalized * m_MyAircraftData.Speed * elapseSeconds, direction.magnitude);
            CachedTransform.localPosition = new Vector3
            (
                Mathf.Clamp(CachedTransform.localPosition.x + speed.x, m_Boundary.xMin, m_Boundary.xMax),
                0f,
                Mathf.Clamp(CachedTransform.localPosition.z + speed.z, m_Boundary.yMin, m_Boundary.yMax)
            );
        }
    }
}
