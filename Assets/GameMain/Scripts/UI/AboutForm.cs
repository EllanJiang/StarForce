using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StarForce
{
    public class AboutForm : UGuiForm
    {
        [SerializeField]
        private Text m_TitleText = null;

        [SerializeField]
        private Text m_DescriptionText = null;

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            // 换个音乐
            GameEntry.Sound.PlayMusic(3);

            m_TitleText.text = GameEntry.Localization.GetString("Game.Name");
            m_DescriptionText.text = GameEntry.Localization.GetString("Game.Description");
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

            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                Close();
            }
        }
    }
}
