# ARKit-origin-sample

Use first detected plane as origin.

Use two camera
* AR Camera
    * clear flags : Depth only
    * culling mask : ARKitPlane
    * depth : -1
* Render Camera
    * clear flags : Don't clear
    * culling mask : everything except ARKitPlane
    * depth : 0

0. Detect plane
1. Calculate plane-camera transformation matrix

ARKitCameraSynchronizer.cs

```c#
void Synchronize(ARPlaneAnchorGameObject ap) {
    var plane_mat = ap.gameObject.transform.localToWorldMatrix;
    var plane_invMat = plane_mat.inverse;
    var arcam_mat = arCamera.transform.localToWorldMatrix;
    var mat = plane_invMat * arcam_mat;
    renderCamera.transform.FromMatrix(mat);
    renderCamera.projectionMatrix = arCamera.projectionMatrix;
}
```

2. Apply camera transform matrix to render camera
3. Render ARKitPlane with AR Camera
4. Render content with Render Camera
