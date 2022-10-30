//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;

namespace StarForce
{
    public class ScrollableBackground : MonoBehaviour
    {
        [SerializeField]
        private float m_ScrollSpeed = -0.25f;

        [SerializeField]
        private float m_TileSize = 30f;

        [SerializeField]
        private BoxCollider m_VisibleBoundary = null;

        [SerializeField]
        private BoxCollider m_PlayerMoveBoundary = null;

        [SerializeField]
        private BoxCollider m_EnemySpawnBoundary = null;

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

        public BoxCollider VisibleBoundary
        {
            get
            {
                return m_VisibleBoundary;
            }
        }

        public BoxCollider PlayerMoveBoundary
        {
            get
            {
                return m_PlayerMoveBoundary;
            }
        }

        public BoxCollider EnemySpawnBoundary
        {
            get
            {
                return m_EnemySpawnBoundary;
            }
        }
    }
}
