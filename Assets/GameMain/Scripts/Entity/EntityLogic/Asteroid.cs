using GameFramework;
using UnityEngine;

namespace AirForce
{
    /// <summary>
    /// 小行星类。
    /// </summary>
    public class Asteroid : Entity
    {
        [SerializeField]
        private AsteroidData m_AsteroidData = null;

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
        }
    }
}
