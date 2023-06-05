using UnityEditor;
using UnityEngine;

namespace Pixelizer {
    public class Palettize_HsvWeight : PixelizerMethod {

        [SerializeField] private ColorPalette cp;
        [SerializeField] private float weightH = 1;
        [SerializeField] private float weightS = 1;
        [SerializeField] private float weightV = 1;

        public override void OnGUI() {
            cp = EditorGUILayout.ObjectField("Color Palette", cp, typeof(ColorPalette), true) as ColorPalette;

            weightH = EditorGUILayout.FloatField("Weight H", weightH);
            weightS = EditorGUILayout.FloatField("Weight S", weightS);
            weightV = EditorGUILayout.FloatField("Weight V", weightV);
        }

        public override Texture2D Process(PixelizerIn input) {
            return Pixelizer.Palettize(
                input.Texture, cp.Colors,
                (c0, c1) => ColorCalc.HsvDist3(c0, c1, weightH, weightS, weightV),
                input.AlphaCut);
        }
    }
}