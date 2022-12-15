using System;
using System.Collections.Generic;
using UnityEngine;

public class Pixelizer : MonoBehaviour {
    const int Default_AlphaCut = 10;

    public static Texture2D Palettize(
        Texture2D srcTex,
        Color32[] paletteColors,
        Func<Color32, Color32, int> fncColorCompare,
        int alphaCut = Default_AlphaCut) {

        Color32[] inputPixels = srcTex.GetPixels32();

        for (int loop = 0; loop < inputPixels.Length; loop++) {
            if (inputPixels[loop].a < alphaCut) {
                inputPixels[loop] = new Color32(0, 0, 0, 0);
                continue;
            }

            Color32 pixel = inputPixels[loop];

            int closestDistance = int.MaxValue;
            Color32 closeColor = default;

            for (int loop2 = 0; loop2 < paletteColors.Length; loop2++) {
                int d = fncColorCompare(pixel, paletteColors[loop2]);
                if (d < closestDistance) {
                    closestDistance = d;
                    closeColor = paletteColors[loop2];
                }
            }

            inputPixels[loop] = closeColor;
        }

        srcTex.SetPixels32(inputPixels);
        srcTex.Apply(false);

        return srcTex;
    }

    public static Texture2D Pixelize(
        Texture2D srcTex,
        int blockSize,
        Dictionary<Color32, float> weightDict,
        Func<PixelBlock, Dictionary<Color32, float>, Color32> fncBlockToPixel) {

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

        srcTex.Reinitialize(pBlocks.GetLength(1), pBlocks.GetLength(0));

        for (int y = 0; y < pBlocks.GetLength(0); y++) {
            for (int x = 0; x < pBlocks.GetLength(1); x++) {
                srcTex.SetPixel(x, y, fncBlockToPixel(pBlocks[y, x], weightDict));
            }
        }

        srcTex.name = "PixelizedOutput";
        srcTex.Apply(false);

        return srcTex;
    }

    private static int CalcBlockSize(int idx, int defaultSize, int pixelLength) {
        int size = defaultSize;
        if ((idx + 1) * defaultSize > pixelLength) {
            size = (idx + 1) * defaultSize - pixelLength;
        }

        return size;
    }
}