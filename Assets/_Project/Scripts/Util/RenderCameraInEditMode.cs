using UnityEngine;

[ExecuteInEditMode]
public class RenderCameraInEditMode : MonoBehaviour {

    private Camera cam;
    void Update() {
        if (cam == null) {
            cam = GetComponent<Camera>();
        }

        cam.Render();
    }
}
