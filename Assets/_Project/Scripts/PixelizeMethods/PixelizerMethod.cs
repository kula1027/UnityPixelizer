using UnityEngine;


namespace Pixelizer {
    public abstract class PixelizerMethod {
        public string Name => GetType().Name;
        public abstract void OnGUI();

        public abstract Texture2D Process(PixelizerIn input);
    }
}