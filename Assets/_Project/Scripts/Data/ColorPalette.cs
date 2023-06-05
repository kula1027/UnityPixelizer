using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pixelizer {
    [CreateAssetMenu(fileName = "New ColorPalette", menuName = "ColorPalette", order = 1)]
    public class ColorPalette : ScriptableObject {
        public WeightedColor[] weightedColors;

        public Color32[] Colors {
            get {
                return weightedColors.Select(wColor => wColor.color).ToArray();
            }
        }

        public Dictionary<Color32, float> WeightTable {
            get {
                return weightedColors.ToDictionary(itm => itm.color, itm => itm.weight);
            }
        }

        public int ColorCount { get { return weightedColors.Length; } }
    }

    [Serializable]
    public class WeightedColor {

        public Color32 color;

        public float weight;
    }
}