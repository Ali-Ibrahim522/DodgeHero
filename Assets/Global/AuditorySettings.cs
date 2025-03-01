using System;
using UnityEngine.Audio;

namespace Global {
    [Serializable]
    public class AuditorySettings {
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;

        public void SetDefaults() {
            masterVolume = 0f;
            musicVolume = 0f;
            sfxVolume = 0f;
        }
    }
}
