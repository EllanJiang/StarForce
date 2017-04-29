using GameFramework;
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

        private ProcedureMenu m_ProcedureMenu = null;

        public void OnStartButtonClick()
        {
            m_ProcedureMenu.StartGame();
        }

        public void OnSettingsButtonClick()
        {

        }

        public void OnAboutButtonClick()
        {

        }

        public void OnQuitButtonClick()
        {
            GameEntry.UI.OpenDialog(new DialogParams()
            {
                Mode = 2,
                Title = GameEntry.Localization.GetString("Title.AskQuitGame"),
                Message = GameEntry.Localization.GetString("Message.AskQuitGame"),
                OnClickConfirm = delegate (object userData) { UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
            });
        }

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureMenu = (ProcedureMenu)userData;
            if (m_ProcedureMenu == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }

            m_TitleText.text = GameEntry.Localization.GetString("Game.Name");
            m_DescriptionText.text = GameEntry.Localization.GetString("Game.Description");
            m_StartText.text = GameEntry.Localization.GetString("Button.Start");
            m_SettingText.text = GameEntry.Localization.GetString("Button.Setting");
            m_AboutText.text = GameEntry.Localization.GetString("Button.About");
            m_QuitText.text = GameEntry.Localization.GetString("Button.Quit");
            m_QuitText.gameObject.SetActive(Application.platform != RuntimePlatform.IPhonePlayer);
        }

        protected internal override void OnClose(object userData)
        {
            m_ProcedureMenu = null;

            base.OnClose(userData);
        }
    }
}
