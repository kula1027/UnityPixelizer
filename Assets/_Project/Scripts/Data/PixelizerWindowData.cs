using UnityEngine;
namespace Pixelizer {

    [CreateAssetMenu(fileName = "New PixelizerWindowData", menuName = "PixelizerWindowData", order = 1)]
    public class PixelizerWindowData : ScriptableObject {
        public RenderTexture inputRtColor;
        public RenderTexture inputRtNormal;

        public Vector2Int inputSize = new Vector2Int(1920, 1080);
        public int blockSize = 4;
        public int alphaCut = 32;
        public string savePath;
        public string fileNameColor = "color_";
        public string fileNameNormal = "normal_";
        public int fileNum;

        public string genAll_targetPath;
        public string genAll_savePath;

    }
}