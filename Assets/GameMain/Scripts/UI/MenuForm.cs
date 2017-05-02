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

        public void OnSettingButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.SettingForm);
        }

        public void OnAboutButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.AboutForm);
        }

        public void OnQuitButtonClick()
        {
            GameEntry.UI.OpenDialog(new DialogParams()
            {
                Mode = 2,
                Title = GameEntry.Localization.GetString("AskQuitGame.Title"),
                Message = GameEntry.Localization.GetString("AskQuitGame.Message"),
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
            m_StartText.text = GameEntry.Localization.GetString("Menu.StartButton");
            m_SettingText.text = GameEntry.Localization.GetString("Menu.SettingButton");
            m_AboutText.text = GameEntry.Localization.GetString("Menu.AboutButton");
            m_QuitText.text = GameEntry.Localization.GetString("Menu.QuitButton");
            m_QuitText.gameObject.SetActive(Application.platform != RuntimePlatform.IPhonePlayer);
        }

        protected internal override void OnClose(object userData)
        {
            m_ProcedureMenu = null;

            base.OnClose(userData);
        }
    }
}
