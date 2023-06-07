using System;
using System.IO;
using UnityEngine;

namespace Pixelizer {

    public class Pixelizer {
        public const int Default_AlphaCut = 10;

        public static Texture2D Palettize(
            Texture2D srcTex,
            Color32[] paletteColors,
            Func<Color32, Color32, float> colorCompare,
            int alphaCut = Default_AlphaCut) {

            Color32[] srcPixels = srcTex.GetPixels32();

            for (int loop = 0; loop < srcPixels.Length; loop++) {
                if (srcPixels[loop].a < alphaCut) {
                    srcPixels[loop] = new Color32(0, 0, 0, 0);
                    continue;
                }

                Color32 pixel = srcPixels[loop];

                float closestDistance = int.MaxValue;
                Color32 closeColor = default;

                for (int loop2 = 0; loop2 < paletteColors.Length; loop2++) {
                    float d = colorCompare(pixel, paletteColors[loop2]);
                    if (d < closestDistance) {
                        closestDistance = d;
                        closeColor = paletteColors[loop2];
                    }
                }

                srcPixels[loop] = closeColor;
            }

            srcTex.name = "PalettizedOutput";

            srcTex.SetPixels32(srcPixels);
            srcTex.Apply(false);

            return srcTex;
        }

        public static Texture2D Palettize(
            Texture2D srcTex,
            Func<Color32, Color32> forEachPixel,
            int alphaCut = Default_AlphaCut) {

            Color32[] srcPixels = srcTex.GetPixels32();

            for (int loop = 0; loop < srcPixels.Length; loop++) {
                if (srcPixels[loop].a < alphaCut) {
                    srcPixels[loop] = new Color32(0, 0, 0, 0);
                    continue;
                }

                srcPixels[loop] = forEachPixel(srcPixels[loop]);
            }

            srcTex.name = "PalettizedOutput";

            srcTex.SetPixels32(srcPixels);
            srcTex.Apply(false);

            return srcTex;
        }

        public static Texture2D Pixelize(
            Texture2D srcTex,
            int blockSize,
            Func<PixelBlock, Color32> funcBlockToPixel) {

            Color32[] inputPixels = srcTex.GetPixels32();

            Vector2Int blockWH = new Vector2Int(
                srcTex.width / blockSize + (srcTex.width % blockSize > 0 ? 1 : 0),
                srcTex.height / blockSize + (srcTex.height % blockSize > 0 ? 1 : 0)
                );

            //foreach blocks
            PixelBlock[,] pBlocks = new PixelBlock[blockWH.y, blockWH.x];
            for (int by = 0; by < pBlocks.GetLength(0); by++) {
                int blockSizeY = CalcBlockSize(by, blockSize, srcTex.height);

                for (int bx = 0; bx < pBlocks.GetLength(1); bx++) {
                    int blockSizeX = CalcBlockSize(bx, blockSize, srcTex.width);

                    Color32[,] blockPixels = new Color32[blockSizeY, blockSizeX];

                    int samplePointX = bx * blockSize;
                    int samplePointY = by * blockSize;
                    //foreach blocks' pixels
                    for (int py = 0; py < blockPixels.GetLength(0); py++) {
                        for (int px = 0; px < blockPixels.GetLength(1); px++) {
                            int inputPixelIdx = (samplePointY + py) * srcTex.width + (samplePointX + px);
                            blockPixels[py, px] = inputPixels[inputPixelIdx];
                        }
                    }

                    pBlocks[by, bx] = new PixelBlock(blockPixels);
                }
            }

            srcTex.Reinitialize(pBlocks.GetLength(1), pBlocks.GetLength(0), TextureFormat.RGBA32, false);
            srcTex.filterMode = FilterMode.Point;
            srcTex.wrapMode = TextureWrapMode.Clamp;
            srcTex.name = "PixelizedOutput";

            for (int y = 0; y < pBlocks.GetLength(0); y++) {
                for (int x = 0; x < pBlocks.GetLength(1); x++) {
                    srcTex.SetPixel(x, y, funcBlockToPixel(pBlocks[y, x]));
                }
            }

            srcTex.Apply(false);

            return srcTex;
        }

        private static int CalcBlockSize(int idx, int defaultSize, int pixelLength) {
            int size = defaultSize;
            if ((idx + 1) * defaultSize > pixelLength) {
                size = pixelLength - (idx * defaultSize);
            }

            return size;
        }

        public static void WriteToFile(Texture2D tex, string dirPath, string name) {
            byte[] bytes = tex.EncodeToPNG();
            if (!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = dirPath + $"/{name}.png";
            File.WriteAllBytes(filePath, bytes);

            Debug.Log($"File Written: {filePath}");
        }
    }
}