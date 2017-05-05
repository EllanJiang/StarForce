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
        private CanvasGroup m_CanvasGroup = null;

        [SerializeField]
        private Slider m_HPBar = null;

        private Entity m_Owner = null;

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

        public bool CheckAvailable()
        {
            if (m_Owner == null || !Owner.IsAvailable || m_CanvasGroup.alpha <= 0f)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            m_HPBar.value = 1f;
            m_Owner = null;
        }

        private IEnumerator HPBarCo(float value, float animationDuration, float keepDuration, float fadeOutDuration)
        {
            yield return m_HPBar.SmoothValue(value, animationDuration);
            yield return new WaitForSeconds(keepDuration);
            yield return m_CanvasGroup.FadeToAlpha(0f, fadeOutDuration);
        }
    }
}
