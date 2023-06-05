using UnityEngine;
namespace Pixelizer {
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
    }
}