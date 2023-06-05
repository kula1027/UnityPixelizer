using UnityEngine;
using UnityEngine.UI;
namespace Pixelizer {

    [ExecuteInEditMode]
    public class UpdateGeneratedColor : MonoBehaviour {

        private RawImage rawImage;

        void Update() {
            if (rawImage == null) rawImage = GetComponent<RawImage>();

            rawImage.texture = PixelizerWindow.GeneratedColor;

        }
    }
}