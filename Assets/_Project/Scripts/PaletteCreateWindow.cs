using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pixelizer {
    public class PaletteCreateWindow : EditorWindow {
        [MenuItem("Pixelizer/PaletteCreate")]
        static void Init() {
            PaletteCreateWindow window = (PaletteCreateWindow)GetWindow(typeof(PaletteCreateWindow));
            window.Show();
        }

        Texture2D srcTex;
        void OnGUI() {
            srcTex = EditorGUILayout.ObjectField(
                  "Input Color Texture",
                  srcTex,
                  typeof(Texture2D), true)
                  as Texture2D;

            if (GUILayout.Button("Create Palette")) {
                if (srcTex != null) {
                    Debug.Log("Create Palette");

                    HashSet<Color32> colors = new HashSet<Color32>();
                    Color32[] pixels = srcTex.GetPixels32();
                    foreach (Color32 c in pixels) {
                        if (!colors.Contains(c)) {
                            colors.Add(c);
                        }
                    }

                    List<WeightedColor> wClrs = new List<WeightedColor>();
                    foreach (Color32 c in colors) {
                        wClrs.Add(new WeightedColor {
                            color = c,
                            weight = 1
                        });
                    }

                    ColorPalette cp = CreateInstance<ColorPalette>();
                    cp.weightedColors = wClrs.ToArray();

                    AssetDatabase.CreateAsset(cp, $"Assets/{srcTex.name}.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = cp;
                }
            }
        }
    }
}