using GameFramework.Localization;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public class SettingForm : UGuiForm
    {
        [SerializeField]
        private Text m_TitleText = null;

        [SerializeField]
        private Text m_MusicText = null;

        [SerializeField]
        private Toggle m_MusicMuteToggle = null;

        [SerializeField]
        private Slider m_MusicVolumeSlider = null;

        [SerializeField]
        private Text m_SoundText = null;

        [SerializeField]
        private Toggle m_SoundMuteToggle = null;

        [SerializeField]
        private Slider m_SoundVolumeSlider = null;

        [SerializeField]
        private Text m_UISoundText = null;

        [SerializeField]
        private Toggle m_UISoundMuteToggle = null;

        [SerializeField]
        private Slider m_UISoundVolumeSlider = null;

        [SerializeField]
        private Text m_LanguageText = null;

        [SerializeField]
        private Text m_LanguageTipsText = null;

        [SerializeField]
        private CanvasGroup m_LanguageTipsCanvasGroup = null;

        [SerializeField]
        private Text m_EnglishText = null;

        [SerializeField]
        private Toggle m_EnglishToggle = null;

        [SerializeField]
        private Text m_ChineseSimplifiedText = null;

        [SerializeField]
        private Toggle m_ChineseSimplifiedToggle = null;

        [SerializeField]
        private Text m_ChineseTraditionalText = null;

        [SerializeField]
        private Toggle m_ChineseTraditionalToggle = null;

        [SerializeField]
        private Text m_ConfirmText = null;

        [SerializeField]
        private Text m_CancelText = null;

        private Language m_SelectedLanguage = Language.Unspecified;

        public void OnMusicMuteChanged(bool isOn)
        {
            GameEntry.Sound.Mute("Music", !isOn);
            m_MusicVolumeSlider.gameObject.SetActive(isOn);
        }

        public void OnMusicVolumeChanged(float volume)
        {
            GameEntry.Sound.SetVolume("Music", volume);
        }

        public void OnSoundMuteChanged(bool isOn)
        {
            GameEntry.Sound.Mute("Sound", !isOn);
            m_SoundVolumeSlider.gameObject.SetActive(isOn);
        }

        public void OnSoundVolumeChanged(float volume)
        {
            GameEntry.Sound.SetVolume("Sound", volume);
        }

        public void OnUISoundMuteChanged(bool isOn)
        {
            GameEntry.Sound.Mute("UISound", !isOn);
            m_UISoundVolumeSlider.gameObject.SetActive(isOn);
        }

        public void OnUISoundVolumeChanged(float volume)
        {
            GameEntry.Sound.SetVolume("UISound", volume);
        }

        public void OnEnglishSelected(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            m_SelectedLanguage = Language.English;
            RefreshLanguageTips();
        }

        public void OnChineseSimplifiedSelected(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            m_SelectedLanguage = Language.ChineseSimplified;
            RefreshLanguageTips();
        }

        public void OnChineseTraditionalSelected(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            m_SelectedLanguage = Language.ChineseTraditional;
            RefreshLanguageTips();
        }

        public void OnSubmitButtonClick()
        {
            if (m_SelectedLanguage == GameEntry.Localization.Language)
            {
                Close();
                return;
            }

            GameEntry.Setting.SetString(Constant.Setting.Language, m_SelectedLanguage.ToString());
            GameEntry.Setting.Save();

            UnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Restart);
        }

        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_TitleText.text = GameEntry.Localization.GetString("Setting.Title");
            m_MusicText.text = GameEntry.Localization.GetString("Setting.Music");
            m_MusicMuteToggle.isOn = !GameEntry.Sound.IsMuted("Music");
            m_MusicVolumeSlider.value = GameEntry.Sound.GetVolume("Music");
            m_SoundText.text = GameEntry.Localization.GetString("Setting.Sound");
            m_SoundMuteToggle.isOn = !GameEntry.Sound.IsMuted("Sound");
            m_SoundVolumeSlider.value = GameEntry.Sound.GetVolume("Sound");
            m_UISoundText.text = GameEntry.Localization.GetString("Setting.UISound");
            m_UISoundMuteToggle.isOn = !GameEntry.Sound.IsMuted("UISound");
            m_UISoundVolumeSlider.value = GameEntry.Sound.GetVolume("UISound");
            m_LanguageText.text = GameEntry.Localization.GetString("Setting.Language");
            m_LanguageTipsText.text = GameEntry.Localization.GetString("Setting.LanguageTips");
            m_EnglishText.text = GameEntry.Localization.GetString("Language.English");
            m_ChineseSimplifiedText.text = GameEntry.Localization.GetString("Language.ChineseSimplified");
            m_ChineseTraditionalText.text = GameEntry.Localization.GetString("Language.ChineseTraditional");
            m_SelectedLanguage = GameEntry.Localization.Language;
            switch (m_SelectedLanguage)
            {
                case Language.English:
                    m_EnglishToggle.isOn = true;
                    break;
                case Language.ChineseSimplified:
                    m_ChineseSimplifiedToggle.isOn = true;
                    break;
                case Language.ChineseTraditional:
                    m_ChineseTraditionalToggle.isOn = true;
                    break;
                default:
                    break;
            }

            m_ConfirmText.text = GameEntry.Localization.GetString("Dialog.ConfirmButton");
            m_CancelText.text = GameEntry.Localization.GetString("Dialog.CancelButton");
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (m_LanguageTipsCanvasGroup.gameObject.activeSelf)
            {
                m_LanguageTipsCanvasGroup.alpha = 0.5f + 0.5f * Mathf.Sin(Mathf.PI * Time.time);
            }
        }

        private void RefreshLanguageTips()
        {
            m_LanguageTipsCanvasGroup.gameObject.SetActive(m_SelectedLanguage != GameEntry.Localization.Language);
        }
    }
}
