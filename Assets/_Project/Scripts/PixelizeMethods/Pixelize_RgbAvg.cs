using UnityEngine;

namespace Pixelizer {
    public class Pixelize_RgbAvg : PixelizerMethod {

        public override void OnGUI() {
        }

        public override Texture2D Process(PixelizerIn input) {
            return Pixelizer.Pixelize(input.Texture, input.BlockSize, pb => PixelBlock.ColorAverage(pb, input.AlphaCut));
        }
    }
}