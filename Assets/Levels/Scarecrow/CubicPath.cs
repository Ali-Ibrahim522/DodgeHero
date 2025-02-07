using UnityEngine;

namespace Levels.Scarecrow {
    [System.Serializable]
    public class CubicPath {
        [SerializeField] private Vector3 start;
        [SerializeField] private Vector3 c1;
        [SerializeField] private Vector3 c2;
        [SerializeField] private Vector3 end;

        public CubicPath(Vector3 start, Vector3 c1, Vector3 c2, Vector3 end) {
            this.start = start;
            this.c1 = c1;
            this.c2 = c2;
            this.end = end;
        }

        public Vector3 GetPositionOnCurve(float elapsed) {
            float u = 1f - elapsed;
            float t2 = elapsed * elapsed;
            float u2 = u * u;
            float u3 = u2 * u;
            float t3 = t2 * elapsed;
        
            return (u3) * start + (3f * u2 * elapsed) * c1 + (3f * u * t2) * c2 + (t3) * end;
        }

        public Vector3 GetDestination() => end;
    
        public Vector3 GetStart() => start;
    }
}