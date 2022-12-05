using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Pixelizer : MonoBehaviour {
    public Texture2D tex;

    [SerializeField] private RawImage targetImageViewer;

    [SerializeField] private RenderTexture inputRT;

    [SerializeField] private int blockSize = 4;

    [SerializeField] private Color32[] colorPalette;

    [SerializeField] private WeightedColor[] colorPal;

    private int order = 0;

    public void OnButtonPalettize() {
        targetImageViewer.texture = Palettize(tex, colorPalette);
    }

    public void OnButtonPixelize() {
        tex = inputRT.CreateTexture2D();

        tex = Palettize(tex, colorPalette);
        tex = Pixelize(tex);
        tex = Palettize(tex, colorPalette);

        targetImageViewer.texture = tex;
    }

    public void OnButtonWriteFile() {
        byte[] bytes = tex.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + $"Image{order++}" + ".png", bytes);
    }

    public Texture2D Palettize(Texture2D targetTex, Color32[] colorPaltte) {
        Color32[] inputPixels = targetTex.GetPixels32();

        for (int loop = 0; loop < inputPixels.Length; loop++) {
            Color32 pixel = inputPixels[loop];
            if (pixel.a < 10) { continue; }

            int closestDistance = 999999;
            Color32 closeColor = default;
            for (int loopP = 0; loopP < colorPalette.Length; loopP++) {
                int d = pixel.SqrDistance(colorPalette[loopP]);
                if (d < closestDistance) {
                    closestDistance = d;
                    closeColor = colorPalette[loopP];
                }
            }

            inputPixels[loop] = closeColor;
        }

        targetTex.SetPixels32(inputPixels);
        targetTex.Apply(false);

        return targetTex;
    }

    public Texture2D Pixelize(Texture2D targetTex) {
        Color32[] inputPixels = targetTex.GetPixels32();

        Vector2Int blockWH = new Vector2Int(
            targetTex.width / blockSize + (targetTex.width % blockSize > 0 ? 1 : 0),
            targetTex.height / blockSize + (targetTex.height % blockSize > 0 ? 1 : 0)
            );

        PixelBlock[,] pBlocks = new PixelBlock[blockWH.y, blockWH.x];

        //foreach blocks
        for (int by = 0; by < pBlocks.GetLength(0); by++) {
            int blockSizeY = CalcBlockSize(by, blockSize, targetTex.height);

            for (int bx = 0; bx < pBlocks.GetLength(1); bx++) {
                int blockSizeX = CalcBlockSize(bx, blockSize, targetTex.width);

                Color32[,] blockPixels = new Color32[blockSizeY, blockSizeX];

                int samplePointX = bx * blockSize;
                int samplePointY = by * blockSize;
                //foreach blocks' pixels
                for (int py = 0; py < blockPixels.GetLength(0); py++) {
                    for (int px = 0; px < blockPixels.GetLength(1); px++) {
                        int inputPixelIdx = (samplePointY + py) * targetTex.width + (samplePointX + px);
                        blockPixels[py, px] = inputPixels[inputPixelIdx];
                    }
                }

                pBlocks[by, bx] = new PixelBlock(blockPixels);
            }
        }

        targetTex.Reinitialize(pBlocks.GetLength(1), pBlocks.GetLength(0));

        for (int y = 0; y < pBlocks.GetLength(0); y++) {
            for (int x = 0; x < pBlocks.GetLength(1); x++) {
                if (pBlocks[y, x].AlphaAverage() > 10) {
                    targetTex.SetPixel(x, y, pBlocks[y, x].ColorAverage());
                } else {
                    targetTex.SetPixel(x, y, new Color32(0, 0, 0, 0));
                }
            }
        }

        targetTex.name = "PixelizedOutput";
        targetTex.Apply(false);

        return targetTex;
    }

    private static int CalcBlockSize(int idx, int defaultSize, int pixelLength) {
        int size = defaultSize;
        if ((idx + 1) * defaultSize > pixelLength) {
            size = (idx + 1) * defaultSize - pixelLength;
        }

        return size;
    }

    private void OnDestroy() {
        if (tex) {
            Destroy(tex);
        }

        if (targetImageViewer.texture) {
            Destroy(targetImageViewer.texture);
        }
    }

    private class PixelBlock {
        private Color32[,] pixels;

        public PixelBlock(Color32[,] pixels) {
            this.pixels = pixels;
        }

        public int AlphaAverage() {
            int alphaSum = 0;
            foreach (Color32 pixel in pixels) {
                alphaSum += pixel.a;
            }

            return alphaSum / pixels.Length;
        }

        public Color32 ColorAverage() {
            Vector3Int colorSum = Vector3Int.zero;
            int avgCount = 0;
            foreach (Color32 pixel in pixels) {
                if (pixel.a > 10) {
                    colorSum += new Vector3Int(pixel.r, pixel.g, pixel.b);
                    avgCount++;
                }
            }

            colorSum /= avgCount;

            return new Color32((byte)colorSum.x, (byte)colorSum.y, (byte)colorSum.z, 255);
        }

        public Color32 DominantColor() {
            return new Color32();
        }
    }
}