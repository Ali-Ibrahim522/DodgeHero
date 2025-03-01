using UnityEngine;

namespace Global {
    public class SessionScore {
        public Sprite ResultsSprite;
        public long Score;
        public int MaxCombo;
        public int TotalHits;
        public float Time;
    }
    public static class SessionScoreManager {
        private static SessionScore _score;
        public static void StoreScore(SessionScore newScore) => _score = newScore;
        public static SessionScore LoadScore() => _score;
    }
}
