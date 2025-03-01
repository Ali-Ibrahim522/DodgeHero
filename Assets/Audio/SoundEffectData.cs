using UnityEngine;

namespace Audio {
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundEffectData", order = 1)]
    public class SoundEffectData : ScriptableObject {
        public AudioClip clip;
        public float pitchVariance;
        public float volume;
        public int maxCount = 5;
    }
}
