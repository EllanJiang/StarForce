using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace StarForce
{
    public class DialogForm : UGuiForm
    {
        [SerializeField]
        private Text m_TitleText = null;

        [SerializeField]
        private Text m_MessageText = null;

        [SerializeField]
        private GameObject[] m_ModeObjects = null;

        [SerializeField]
        private Text[] m_ConfirmTexts = null;

        [SerializeField]
        private Text[] m_CancelTexts = null;

        [SerializeField]
        private Text[] m_OtherTexts = null;

        private int m_DialogMode = 1;
        private bool m_PauseGame = false;
        private object m_UserData = null;
        private GameFrameworkAction<object> m_OnClickConfirm = null;
        private GameFrameworkAction<object> m_OnClickCancel = null;
        private GameFrameworkAction<object> m_OnClickOther = null;

        public void OnConfirmButtonClick()
        {
            if (m_OnClickConfirm != null)
            {
                m_OnClickConfirm(m_UserData);
            }

            Close();
        }

        public void OnCancelButtonClick()
        {
            if (m_OnClickCancel != null)
            {
                m_OnClickCancel(m_UserData);
            }

            Close();
        }

        public void OnOtherButtonClick()
        {
            if (m_OnClickOther != null)
            {
                m_OnClickOther(m_UserData);
            }

            Close();
        }

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            DialogParams dialogParams = userData as DialogParams;
            if (dialogParams == null)
            {
                Log.Warning("DialogParams is invalid.");
                return;
            }

            RefreshDialogMode(dialogParams.Mode);

            m_TitleText.text = dialogParams.Title;
            m_MessageText.text = dialogParams.Message;

            RefreshPauseGame(dialogParams.PauseGame);

            m_UserData = dialogParams.UserData;

            RefreshConfirmText(dialogParams.ConfirmText);
            m_OnClickConfirm = dialogParams.OnClickConfirm;

            RefreshCancelText(dialogParams.CancelText);
            m_OnClickCancel = dialogParams.OnClickCancel;

            RefreshOtherText(dialogParams.OtherText);
            m_OnClickOther = dialogParams.OnClickOther;
        }

        protected internal override void OnClose(object userData)
        {
            if (m_PauseGame)
            {
                GameEntry.Base.ResumeGame();
            }

            RefreshDialogMode(1);

            m_TitleText.text = string.Empty;
            m_MessageText.text = string.Empty;

            RefreshPauseGame(false);

            m_UserData = null;

            RefreshConfirmText(string.Empty);
            m_OnClickConfirm = null;

            RefreshCancelText(string.Empty);
            m_OnClickCancel = null;

            RefreshOtherText(string.Empty);
            m_OnClickOther = null;

            base.OnClose(userData);
        }

        private void RefreshDialogMode(int dialogMode)
        {
            m_DialogMode = dialogMode;
            for (int i = 1; i <= m_ModeObjects.Length; i++)
            {
                m_ModeObjects[i - 1].SetActive(i == dialogMode);
            }
        }

        private void RefreshPauseGame(bool pauseGame)
        {
            m_PauseGame = pauseGame;
            if (pauseGame)
            {
                GameEntry.Base.PauseGame();
            }
        }

        private void RefreshConfirmText(string confirmText)
        {
            if (string.IsNullOrEmpty(confirmText))
            {
                confirmText = GameEntry.Localization.GetString("Button.Confirm");
            }

            for (int i = 0; i < m_ConfirmTexts.Length; i++)
            {
                m_ConfirmTexts[i].text = confirmText;
            }
        }

        private void RefreshCancelText(string cancelText)
        {
            if (string.IsNullOrEmpty(cancelText))
            {
                cancelText = GameEntry.Localization.GetString("Button.Cancel");
            }

            for (int i = 0; i < m_CancelTexts.Length; i++)
            {
                m_CancelTexts[i].text = cancelText;
            }
        }

        private void RefreshOtherText(string otherText)
        {
            if (string.IsNullOrEmpty(otherText))
            {
                otherText = GameEntry.Localization.GetString("Button.Other");
            }

            for (int i = 0; i < m_OtherTexts.Length; i++)
            {
                m_OtherTexts[i].text = otherText;
            }
        }
    }
}
