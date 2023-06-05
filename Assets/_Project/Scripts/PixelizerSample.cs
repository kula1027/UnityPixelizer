using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelizer {
    public class PixelizerSample : MonoBehaviour {
        private Texture2D outColor;
        private Texture2D outNormal;

        [SerializeField] private RawImage outputColor;
        [SerializeField] private RawImage outputNormal;

        [SerializeField] private RenderTexture inputRt;

        [SerializeField] private RenderTexture inputRtNormals;

        [SerializeField] private int blockSize;

        [SerializeField] private ColorPalette cp;

        private int order = 0;

        public void OnButtonPalettize() {
            if (outColor) {
                Destroy(outColor);
            }

            Texture2D srcColor = inputRt.CreateTexture2D();
            Texture2D srcNormal = inputRtNormals.CreateTexture2D();

            Texture2D texPal = Pixelizer.Palettize(srcColor, cp.Colors, ColorCalc.HsvDist);
            //outColor = Pixelizer.Pixelize(srcColor, blockSize, cp.WeightTable, (pb, dict) => PixelBlock.ColorAverage(pb, dict));
            //outColor = Pixelizer.Pixelize(srcColor, blockSize, cp.WeightTable, (pb, dict) => PixelBlock.ColorAverage(pb, 60));
            //outNormal = Pixelizer.Pixelize(srcNormal, blockSize, null, (pb, dict) => PixelBlock.ColorAverage(pb));

            Destroy(srcColor);
            Destroy(texPal);

            outputColor.texture = outColor;
            outputNormal.texture = outNormal;
        }

        public void OnButtonPixelize() {
            if (outColor) {
                Destroy(outColor);
            }

            Texture2D srcColor = inputRt.CreateTexture2D();
            Texture2D srcNormal = inputRtNormals.CreateTexture2D();

            Texture2D texPal = Pixelizer.Palettize(srcColor, cp.Colors, ColorCalc.HsvDist);
            //outColor = Pixelizer.Pixelize(texPal, blockSize, (pb) => PixelBlock.ColorAverage(pb, dict, 250));

            outNormal = Pixelizer.Pixelize(srcNormal, blockSize, (pb) => PixelBlock.ColorAverage(pb));

            Destroy(srcColor);
            Destroy(texPal);

            outputColor.texture = outColor;
            outputNormal.texture = outNormal;
        }

        public void OnButtonWriteFile() {
            WriteToFile(outColor, $"color{order}");
            WriteToFile(outNormal, $"normal{order}");

            order++;
        }

        private void WriteToFile(Texture2D tex, string name) {
            byte[] bytes = tex.EncodeToPNG();
            string dirPath = Application.dataPath + "/../pixelized/";
            if (!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = dirPath + $"{name}.png";
            File.WriteAllBytes(filePath, bytes);

            Debug.Log($"File Written: {filePath}");
        }

        private void OnDestroy() {
            if (outColor) {
                Destroy(outColor);
            }
            if (outNormal) {
                Destroy(outNormal);
            }

            if (outputColor.texture) {
                Destroy(outputColor.texture);
            }
        }
    }
}