using UnityEngine;

[ExecuteInEditMode]
public class CameraNormals : MonoBehaviour {

    public Material mat;

    private Camera cam;

    private void Awake() {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        mat.SetMatrix("_viewToWorld", cam.cameraToWorldMatrix);
        Graphics.Blit(source, destination, mat);
    }
}
