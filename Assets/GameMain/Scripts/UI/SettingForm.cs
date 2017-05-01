using UnityEngine;
using UnityEngine.UI;

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
        private Text m_ChineseSimplifiedText = null;

        [SerializeField]
        private Text m_ChineseTraditionalText = null;

        [SerializeField]
        private Text m_ConfirmText = null;

        [SerializeField]
        private Text m_CancelText = null;

        public void OnMusicMuteChanged(bool value)
        {
            GameEntry.Sound.Mute("Music", !value);
            m_MusicVolumeSlider.gameObject.SetActive(value);
        }

        public void OnMusicVolumeChanged(float value)
        {
            GameEntry.Sound.SetVolume("Music", value);
        }

        public void OnSoundMuteChanged(bool value)
        {
            GameEntry.Sound.Mute("Sound", !value);
            m_SoundVolumeSlider.gameObject.SetActive(value);
        }

        public void OnSoundVolumeChanged(float value)
        {
            GameEntry.Sound.SetVolume("Sound", value);
        }

        public void OnUISoundMuteChanged(bool value)
        {
            GameEntry.Sound.Mute("UISound", !value);
            m_UISoundVolumeSlider.gameObject.SetActive(value);
        }

        public void OnUISoundVolumeChanged(float value)
        {
            GameEntry.Sound.SetVolume("UISound", value);
        }

        public void OnSubmitButtonClick()
        {
            Close();
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
    }
}
