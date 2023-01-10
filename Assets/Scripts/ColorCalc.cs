using UnityEngine;

public static class ColorCalc {
    public static int RgbEucSqrDistPerceptive(Color32 c0, Color32 c1) {
        int r = c0.r - c1.r;
        int g = c0.g - c1.g;
        int b = c0.b - c1.b;

        return (int)(r * r * 0.3f + g * g * 0.59f + b * b * 0.11f);
    }

    public static int RgbEucSqrDist(Color32 c0, Color32 c1) {
        int r = c0.r - c1.r;
        int g = c0.g - c1.g;
        int b = c0.b - c1.b;

        return r * r + g * g + b * b;
    }
}
