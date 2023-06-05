using UnityEngine;

namespace Pixelizer {
    public struct ColorHSV {
        public float h;
        public float s;
        public float v;

        public ColorHSV(Color32 clr) {
            Color.RGBToHSV(clr, out h, out s, out v);
        }
    }
}