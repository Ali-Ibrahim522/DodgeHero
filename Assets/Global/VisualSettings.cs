using System;
using UnityEngine;

namespace Global {
    [Serializable]
    public class VisualSettings {
        public Color proposed;
        public Color hit;
        public Color missed;

        public void SetDefaults() {
            proposed = Color.cyan;
            hit = Color.green;
            missed = Color.red;
        }
    }
}