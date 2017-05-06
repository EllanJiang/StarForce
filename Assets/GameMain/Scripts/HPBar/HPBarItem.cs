using GameFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StarForce
{
    public class HPBarItem : MonoBehaviour
    {
        private const float AnimationSeconds = 1f;
        private const float KeepSeconds = 1f;
        private const float FadeOutSeconds = 1f;

        [SerializeField]
        private Slider m_HPBar = null;

        private RectTransform m_CachedTransform = null;
        private CanvasGroup m_CachedCanvasGroup = null;
        private Entity m_Owner = null;
        private float m_Height = 0f;

        public Entity Owner
        {
            get
            {
                return m_Owner;
            }
        }

        public void Init(Entity owner, float fromHPRatio, float toHPRatio)
        {
            if (owner == null || !owner.IsAvailable)
            {
                Log.Error("Owner is invalid.");
                return;
            }

            if (m_Owner != owner)
            {
                m_HPBar.value = fromHPRatio;
            }

            m_Owner = owner;

            StopAllCoroutines();
            StartCoroutine(HPBarCo(toHPRatio, AnimationSeconds, KeepSeconds, FadeOutSeconds));
        }

        public bool Refresh()
        {
            if (m_Owner == null || !Owner.IsAvailable || m_CachedCanvasGroup.alpha <= 0f)
            {
                return false;
            }

            Vector3 worldPosition = m_Owner.CachedTransform.position + Vector3.up * m_Height;
            m_CachedTransform.position = GameEntry.Scene.MainCamera.WorldToScreenPoint(worldPosition);

            return true;
        }

        public void Reset()
        {
            m_HPBar.value = 1f;
            m_Owner = null;
        }

        private void Start()
        {
            m_CachedTransform = GetComponent<RectTransform>();
            if (m_CachedTransform == null)
            {
                Log.Error("RectTransform is invalid.");
                return;
            }

            m_CachedCanvasGroup = GetComponent<CanvasGroup>();
            if (m_CachedCanvasGroup == null)
            {
                Log.Error("CanvasGroup is invalid.");
                return;
            }
        }

        private IEnumerator HPBarCo(float value, float animationDuration, float keepDuration, float fadeOutDuration)
        {
            yield return m_HPBar.SmoothValue(value, animationDuration);
            yield return new WaitForSeconds(keepDuration);
            yield return m_CachedCanvasGroup.FadeToAlpha(0f, fadeOutDuration);
        }
    }
}
