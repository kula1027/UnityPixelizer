using UnityEngine;
using UnityEngine.UI;

namespace Pixelizer {
    [ExecuteInEditMode]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class AspectRatioSetter : MonoBehaviour {
        [HideInInspector]
        [SerializeField] private AspectRatioFitter aspectRatioFitter;

        void Update() {
            if (!aspectRatioFitter) {
                aspectRatioFitter = GetComponent<AspectRatioFitter>();
            }
            if (aspectRatioFitter.aspectRatio != PixelizerWindow.TextureAspectRatio) {
                aspectRatioFitter.aspectRatio = PixelizerWindow.TextureAspectRatio;
            }
        }
    }

}