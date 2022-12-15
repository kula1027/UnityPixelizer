using System.Collections.Generic;
using UnityEngine;

public class PixelBlock {
    const int Default_AlphaCut = 10;

    public Color32[,] Pixels { get; private set; }

    public PixelBlock(Color32[,] pixels) {
        this.Pixels = pixels;
    }

    public static int AlphaAverage(PixelBlock pb) {
        int alphaSum = 0;
        foreach (Color32 pixel in pb.Pixels) {
            alphaSum += pixel.a;
        }

        return alphaSum / pb.Pixels.Length;
    }

    public static Color32 ColorAverage(PixelBlock pb, Dictionary<Color32, float> colorWeightTable, int alphaCut = Default_AlphaCut) {
        Vector3 colorSum = Vector3.zero;
        float divideSum = 0;
        foreach (Color32 pixel in pb.Pixels) {
            if (pixel.a > alphaCut) {
                if (colorWeightTable.TryGetValue(pixel, out float weight)) {
                    colorSum += new Vector3(pixel.r, pixel.g, pixel.b) * colorWeightTable[pixel];
                    divideSum += colorWeightTable[pixel];
                } else {
                    Debug.LogError($"color not found {pixel}");
                    break;
                }
            }
        }

        if (divideSum > 0) {
            colorSum /= divideSum;

            return new Color32((byte)colorSum.x, (byte)colorSum.y, (byte)colorSum.z, 255);
        } else {
            return new Color32(0, 0, 0, 0);
        }
    }

    public static Color32 ColorDominant(PixelBlock pb, Dictionary<Color32, float> colorWeightTable, int alphaCut = Default_AlphaCut) {
        return new Color32();
    }
}
