using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public class MenuForm : UGuiForm
    {
        [SerializeField]
        private Text m_TitleText = null;

        [SerializeField]
        private Text m_DescriptionText = null;

        [SerializeField]
        private Text m_StartText = null;

        [SerializeField]
        private Text m_SettingText = null;

        [SerializeField]
        private Text m_AboutText = null;

        [SerializeField]
        private Text m_QuitText = null;

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_TitleText.text = GameEntry.Localization.GetString("Game.Name");
            m_DescriptionText.text = GameEntry.Localization.GetString("Game.Description");
            m_StartText.text = GameEntry.Localization.GetString("Button.Start");
            m_SettingText.text = GameEntry.Localization.GetString("Button.Setting");
            m_AboutText.text = GameEntry.Localization.GetString("Button.About");
            m_QuitText.text = GameEntry.Localization.GetString("Button.Quit");
            m_QuitText.gameObject.SetActive(Application.platform != RuntimePlatform.IPhonePlayer);
        }

        public void OnStartButtonClick()
        {

        }

        public void OnSettingsButtonClick()
        {

        }

        public void OnAboutButtonClick()
        {

        }

        public void OnQuitButtonClick()
        {
            UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit);
        }
    }
}
