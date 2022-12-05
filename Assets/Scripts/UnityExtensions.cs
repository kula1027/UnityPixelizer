using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions {

    public static Texture2D CreateTexture2D(this RenderTexture renderTex) {
        Texture2D tex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        RenderTexture.active = renderTex;
        tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        tex.Apply();

        return tex;
    }

    public static int SqrDistance(this Color32 c0, Color32 c1) {
        int r = (int)c0.r - (int)c1.r;
        int g = (int)c0.g - (int)c1.g;
        int b = (int)c0.b - (int)c1.b;

        return r * r + g * g + b * b;
    }
}