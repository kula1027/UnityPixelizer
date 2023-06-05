using UnityEditor;
using UnityEngine;

namespace Pixelizer {
    public class Palettize_Hsv2 : PixelizerMethod {
        [SerializeField] private ColorPalette cp;

        public override void OnGUI() {
            cp = EditorGUILayout.ObjectField("Color Palette", cp, typeof(ColorPalette), true) as ColorPalette;

        }

        public override Texture2D Process(PixelizerIn input) {
            return Pixelizer.Palettize(input.Texture, cp.Colors, ColorCalc.HsvDist2, input.AlphaCut);
        }
    }
}