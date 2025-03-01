using UnityEngine;

namespace Audio {
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MusicData", order = 2)]
    public class MusicData : ScriptableObject {
        public AudioClip musicClip;
        public string musicID;
    }
}
