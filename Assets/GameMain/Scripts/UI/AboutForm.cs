using UnityEngine;
using UnityEngine.UI;

namespace StarForce
{
    public class AboutForm : UGuiForm
    {
        [SerializeField]
        private RectTransform m_Transform = null;

        [SerializeField]
        private float m_DelaySeconds = 1f;

        [SerializeField]
        private float m_ScrollSpeed = 1f;

        [SerializeField]
        private float m_ScrollMax = 0f;

        [SerializeField]
        private float m_ScrollMin = 0f;

        [SerializeField]
        private Text m_TitleText = null;

        [SerializeField]
        private Text m_DescriptionText = null;

        [SerializeField]
        private Text m_WebsiteText = null;

        [SerializeField]
        private Text m_SummaryText = null;

        [SerializeField]
        private Text m_IntroductionText = null;

        private float m_ElapseSeconds = 0f;

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ElapseSeconds = 0f;
            m_Transform.SetLocalPositionY(0f);

            // 换个音乐
            GameEntry.Sound.PlayMusic(3);

            m_TitleText.text = GameEntry.Localization.GetString("Game.Name");
            m_DescriptionText.text = GameEntry.Localization.GetString("Game.Description");
            m_WebsiteText.text = GameEntry.Localization.GetString("Game.Website");
            m_SummaryText.text = GameEntry.Localization.GetString("Game.Summary");
            m_IntroductionText.text = GameEntry.Localization.GetString("Game.Introduction");
        }

        protected internal override void OnClose(object userData)
        {
            base.OnClose(userData);

            // 还原音乐
            GameEntry.Sound.PlayMusic(1);
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            m_ElapseSeconds += elapseSeconds;
            if (m_ElapseSeconds > m_DelaySeconds)
            {
                m_Transform.AddLocalPositionY(m_ScrollSpeed * elapseSeconds);
                if (m_Transform.localPosition.y > m_ScrollMax)
                {
                    m_Transform.SetLocalPositionY(m_ScrollMin);
                }
            }
        }
    }
}
