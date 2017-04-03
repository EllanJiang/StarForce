using GameFramework;
using UnityEngine;

namespace AirForce
{
    /// <summary>
    /// 小行星类。
    /// </summary>
    public class Asteroid : TargetableObject
    {
        [SerializeField]
        private AsteroidData m_AsteroidData = null;

        private Vector3 m_RotateSphere = Vector3.zero;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_AsteroidData = userData as AsteroidData;
            if (m_AsteroidData == null)
            {
                Log.Error("Asteroid data is invalid.");
                return;
            }

            m_RotateSphere = Random.insideUnitSphere;
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            CachedTransform.Translate(Vector3.back * m_AsteroidData.Speed * elapseSeconds, Space.World);
            CachedTransform.Rotate(m_RotateSphere * m_AsteroidData.AngularSpeed * elapseSeconds, Space.Self);
        }
    }
}
