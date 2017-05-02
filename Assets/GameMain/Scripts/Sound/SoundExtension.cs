using GameFramework;
using GameFramework.DataTable;
using GameFramework.Sound;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public static class SoundExtension
    {
        private const float FadeVolumeDuration = 1f;

        public static int PlayMusic(this SoundComponent soundComponent, int musicId, object userData = null)
        {
            soundComponent.StopAllSounds("Music", FadeVolumeDuration);

            IDataTable<DRMusic> dtMusic = GameEntry.DataTable.GetDataTable<DRMusic>();
            DRMusic drMusic = dtMusic.GetDataRow(musicId);
            if (drMusic == null)
            {
                Log.Warning("Can not load music '{0}' from data table.", musicId.ToString());
                return -1;
            }

            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = 64,
                Loop = true,
                VolumeInSoundGroup = 1f,
                FadeInSeconds = FadeVolumeDuration,
                SpatialBlend = 0f,
            };

            return soundComponent.PlaySound(AssetUtility.GetMusicAsset(drMusic.AssetName), "Music", playSoundParams, null, userData);
        }

        public static void StopMusic(this SoundComponent soundComponent)
        {
            soundComponent.StopAllSounds("Music", FadeVolumeDuration);
        }

        public static int PlaySound(this SoundComponent soundComponent, int soundId, Entity bindingEntity = null, object userData = null)
        {
            IDataTable<DRSound> dtSound = GameEntry.DataTable.GetDataTable<DRSound>();
            DRSound drSound = dtSound.GetDataRow(soundId);
            if (drSound == null)
            {
                Log.Warning("Can not load sound '{0}' from data table.", soundId.ToString());
                return -1;
            }

            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = drSound.Priority,
                Loop = drSound.Loop,
                VolumeInSoundGroup = drSound.Volume,
                SpatialBlend = drSound.SpatialBlend,
            };

            return soundComponent.PlaySound(AssetUtility.GetSoundAsset(drSound.AssetName), "Sound", playSoundParams, bindingEntity != null ? bindingEntity.Entity : null, userData);
        }

        public static int PlayUISound(this SoundComponent soundComponent, int uiSoundId, object userData = null)
        {
            IDataTable<DRUISound> dtUISound = GameEntry.DataTable.GetDataTable<DRUISound>();
            DRUISound drUISound = dtUISound.GetDataRow(uiSoundId);
            if (drUISound == null)
            {
                Log.Warning("Can not load UI sound '{0}' from data table.", uiSoundId.ToString());
                return -1;
            }

            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = drUISound.Priority,
                Loop = false,
                VolumeInSoundGroup = drUISound.Volume,
                SpatialBlend = 0f,
            };

            return soundComponent.PlaySound(AssetUtility.GetUISoundAsset(drUISound.AssetName), "UISound", playSoundParams, userData);
        }

        public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return true;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return true;
            }

            return soundGroup.Mute;
        }

        public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Mute = mute;

            GameEntry.Setting.SetBool(string.Format(Constant.Setting.SoundGroupMuted, soundGroupName), mute);
            GameEntry.Setting.Save();
        }

        public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return 0f;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return 0f;
            }

            return soundGroup.Volume;
        }

        public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Volume = volume;

            GameEntry.Setting.SetFloat(string.Format(Constant.Setting.SoundGroupVolume, soundGroupName), volume);
            GameEntry.Setting.Save();
        }
    }
}
