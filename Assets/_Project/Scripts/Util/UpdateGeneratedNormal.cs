using UnityEngine;
using UnityEngine.UI;
namespace Pixelizer {

    [ExecuteInEditMode]
    public class UpdateGeneratedNormal : MonoBehaviour {

        private RawImage rawImage;

        void Update() {
            if (rawImage == null) rawImage = GetComponent<RawImage>();

            rawImage.texture = PixelizerWindow.GeneratedNormal;

        }
    }
}