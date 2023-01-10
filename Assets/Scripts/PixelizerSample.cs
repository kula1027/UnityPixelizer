using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PixelizerSample : MonoBehaviour {
    public Texture2D tex;

    [SerializeField] private RawImage targetImageViewer;

    [SerializeField] private RenderTexture inputRT;

    [SerializeField] private int blockSize = 4;

    [SerializeField] private ColorPalette cp;

    private int order = 0;

    public void OnButtonPalettize() {
        if (tex) {
            Destroy(tex);
        }

        tex = inputRT.CreateTexture2D();

        tex = Pixelizer.Palettize(tex, cp.Colors, ColorCalc.RgbEucSqrDistPerceptive);
        tex = Pixelizer.Pixelize(tex, blockSize, cp.WeightTable, (pb, dict) => PixelBlock.ColorDominant(pb, dict));

        targetImageViewer.texture = tex;
    }

    public void OnButtonPixelize() {
        if (tex) {
            Destroy(tex);
        }

        tex = inputRT.CreateTexture2D();

        tex = Pixelizer.Palettize(tex, cp.Colors, ColorCalc.RgbEucSqrDistPerceptive);
        tex = Pixelizer.Pixelize(tex, blockSize, cp.WeightTable, (pb, dict) => PixelBlock.ColorAverage(pb, dict));
        tex = Pixelizer.Palettize(tex, cp.Colors, ColorCalc.RgbEucSqrDistPerceptive);

        targetImageViewer.texture = tex;
    }

    public void OnButtonWriteFile() {
        byte[] bytes = tex.EncodeToPNG();
        string dirPath = Application.dataPath + "/../pixelized/";
        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }

        string filePath = dirPath + $"pixelized{order++}" + ".png";
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"File Written: {filePath}");
    }

    private void OnDestroy() {
        if (tex) {
            Destroy(tex);
        }

        if (targetImageViewer.texture) {
            Destroy(targetImageViewer.texture);
        }
    }
}