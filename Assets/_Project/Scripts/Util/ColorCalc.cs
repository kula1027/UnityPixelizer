using UnityEngine;
namespace Pixelizer {
    /// <summary>
    /// HSV 계산 출처 https://stackoverflow.com/questions/35113979/calculate-distance-between-colors-in-hsv-space
    /// </summary>
    public static class ColorCalc {
        public static float RgbDistPerceptive(Color32 c0, Color32 c1) {
            int r = c0.r - c1.r;
            int g = c0.g - c1.g;
            int b = c0.b - c1.b;

            return (int)(r * r * 0.3f + g * g * 0.59f + b * b * 0.11f);
        }

        public static float RgbDist(Color32 c0, Color32 c1) {
            int r = c0.r - c1.r;
            int g = c0.g - c1.g;
            int b = c0.b - c1.b;

            return r * r + g * g + b * b;
        }

        public static float HsvDist(ColorHSV c0, ColorHSV c1) {
            float a = Mathf.Sin(c0.h) * c0.s * c0.v - Mathf.Sin(c1.h) * c1.s * c1.v;
            float b = Mathf.Cos(c0.h) * c0.s * c0.v - Mathf.Cos(c1.h) * c1.s * c1.v;
            float c = c0.v - c1.v;

            return a * a + b * b + c * c;
        }
        public static float HsvDist(Color32 c0, Color32 c1)
            => HsvDist(new ColorHSV(c0), new ColorHSV(c1));


        public static float HsvDist2(ColorHSV c0, ColorHSV c1) {
            float hDist = Mathf.Abs(c0.h - c1.h);
            hDist = hDist > 0.5f ? 1 - hDist : hDist;

            float a = Mathf.Sin(c0.h) * c0.s * c0.v - Mathf.Sin(c1.h) * c1.s * c1.v;
            float b = Mathf.Cos(c0.h) * c0.s * c0.v - Mathf.Cos(c1.h) * c1.s * c1.v;
            float c = c0.v - c1.v;

            return a * a + b * b + c * c + hDist * hDist;
        }
        public static float HsvDist2(Color32 c0, Color32 c1)
            => HsvDist2(new ColorHSV(c0), new ColorHSV(c1));


        public static float HsvDist3(ColorHSV c0, ColorHSV c1, float weightH, float weightS, float weightV) {
            float dh = Mathf.Abs(c0.h - c1.h);
            dh = dh > 0.5f ? 1 - dh : dh;
            float ds = Mathf.Abs(c0.s - c1.s);
            float dv = Mathf.Abs(c1.v - c0.v);

            return (dh * dh) * weightH + (ds * ds) * weightS + (dv * dv) * weightV;
        }
        public static float HsvDist3(Color32 c0, Color32 c1, float weightH, float weightS, float weightV)
            => HsvDist3(new ColorHSV(c0), new ColorHSV(c1), weightH, weightS, weightV);

    }
}