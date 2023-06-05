using UnityEngine;

namespace Pixelizer {
    public class PixelizerIn {
        public Texture2D Texture { get; set; }
        public int BlockSize { get; set; }
        public int AlphaCut { get; set; }
    }
}