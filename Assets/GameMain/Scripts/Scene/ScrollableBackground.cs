using UnityEngine;

namespace AirForce
{
    public class ScrollableBackground : MonoBehaviour
    {
        [SerializeField]
        private float m_ScrollSpeed = -0.25f;

        [SerializeField]
        private float m_TileSize = 30f;

        private Transform m_CachedTransform = null;
        private Vector3 m_StartPosition = Vector3.zero;

        private void Start()
        {
            m_CachedTransform = transform;
            m_StartPosition = m_CachedTransform.position;
        }

        private void Update()
        {
            float newPosition = Mathf.Repeat(Time.time * m_ScrollSpeed, m_TileSize);
            m_CachedTransform.position = m_StartPosition + Vector3.forward * newPosition;
        }
    }
}
