using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pixelizer {
    public class PixelBlock {

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

        public static Color32 ColorAverage(PixelBlock pb, int alphaCut = Pixelizer.Default_AlphaCut) {
            Vector4 colorSum = Vector4.zero;
            float divideSum = 0;
            foreach (Color32 pixel in pb.Pixels) {
                if (pixel.a > alphaCut) {
                    colorSum += new Vector4(pixel.r, pixel.g, pixel.b, pixel.a);
                    divideSum++;
                }
            }

            if (divideSum > 0) {
                colorSum /= divideSum;

                return new Color32((byte)colorSum.x, (byte)colorSum.y, (byte)colorSum.z, (byte)colorSum.w);
            } else {
                return new Color32(1, 1, 1, 0);
            }
        }

        public static Color32 ColorAverage(PixelBlock pb, Dictionary<Color32, float> colorWeightTable, int alphaCut = Pixelizer.Default_AlphaCut) {
            Vector4 colorSum = Vector4.zero;
            float divideSum = 0;
            foreach (Color32 pixel in pb.Pixels) {
                if (pixel.a > alphaCut) {
                    if (colorWeightTable.TryGetValue(pixel, out float weight)) {
                        colorSum += new Vector4(pixel.r, pixel.g, pixel.b, pixel.a) * colorWeightTable[pixel];
                        divideSum += colorWeightTable[pixel];
                    } else {
                        Debug.LogError($"color not found {pixel}");
                        break;
                    }
                }
            }

            if (divideSum > 0) {
                colorSum /= divideSum;

                return new Color32((byte)colorSum.x, (byte)colorSum.y, (byte)colorSum.z, (byte)colorSum.w);
            } else {
                return new Color32(0, 0, 0, 0);
            }
        }

        public static Color32 ColorDominant(PixelBlock pb, Dictionary<Color32, float> weightTable, float alphaCutWeight, int alphaCut = Pixelizer.Default_AlphaCut) {
            Dictionary<Color32, float> weightSum = new();


            if (!weightTable.ContainsKey(new Color32(0, 0, 0, 0))) {
                weightTable.Add(new Color32(0, 0, 0, 0), alphaCutWeight);
            }

            foreach (Color32 pixel in pb.Pixels) {
                Color32 c = pixel;

                if (c.a <= alphaCut) {
                    c = new Color32(0, 0, 0, 0);
                }

                if (!weightSum.ContainsKey(c)) {
                    weightSum.Add(c, 0);
                }

                weightSum[c] += weightTable[c];
            }

            Color32 dominantColor = weightSum.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            return dominantColor;
        }
    }
}