using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class DebugDisplay : MonoBehaviour {
	public Text text;

	public Button btn_viewport;
	int viewport_count = 0;

	public UnityARCameraManager cameraManager;
	public UnityARGeneratePlane arGeneratePlane;

	public Camera arCamera;
	public Camera renderCamera;

	void Start () {
		Debug.Assert(text != null);

		btn_viewport.onClick.AddListener(delegate() {
			if(viewport_count % 2 == 0) {
				renderCamera.rect = new Rect(0.5f, 0.5f, 1, 1);
			} else {
				renderCamera.rect = new Rect(0, 0, 1, 1);
			}
			viewport_count += 1;
		});
	}

	void DumpCamera(Camera cam, StringBuilder sb, string label) {
		var tr = cam.transform;
		var pos = tr.position;
		var rot = tr.rotation;
		var scale = tr.lossyScale;
		sb.AppendLine(label);
		sb.AppendFormat("pos = {0}, {1}, {2}\n", pos.x, pos.y, pos.z);
		sb.AppendFormat("rot = {0}, {1}, {2}, {3}\n", rot.x, rot.y, rot.z, rot.w);
		sb.AppendFormat("scale = {0}, {1}, {2}\n", scale.x, scale.y, scale.z);
		sb.AppendFormat("near/far = {0}, {1}\n", cam.nearClipPlane, cam.farClipPlane);
		sb.AppendFormat("fov = {0}\n", cam.fieldOfView);
	}

	void Refresh() {
		var sb = new StringBuilder();

		// AR Camera information
		{
			var session = cameraManager.Session;
			Matrix4x4 matrix = session.GetCameraPose();
			var localPosition = UnityARMatrixOps.GetPosition(matrix);
			var localRotation = UnityARMatrixOps.GetRotation (matrix);

			var pos = localPosition;
			var rot = localRotation;
			sb.AppendLine("## Session");
			sb.AppendFormat("{0}, {1}, {2}\n", pos.x, pos.y, pos.z);
			sb.AppendFormat("{0}, {1}, {2}, {3}\n", rot.x, rot.y, rot.z, rot.w);
		}

		if(arCamera != null) {
			DumpCamera(arCamera, sb, "## AR Camera");
		}

		if(renderCamera != null) {
			DumpCamera(renderCamera, sb, "## Render Camera");
		}

		// Plane informations
		var anchorManager = arGeneratePlane.ARAnchorManager;
		if(anchorManager != null) {
			List<ARPlaneAnchorGameObject> arpags = anchorManager.GetCurrentPlaneAnchors ();
			for(var i = 0 ; i  < arpags.Count ; i++) {
				ARPlaneAnchor ap = arpags [i].planeAnchor;
				sb.AppendFormat("## Plane {0}\n", i);
				sb.AppendFormat("Center: x:{0}, y:{1}, z:{2}\n", ap.center.x, ap.center.y, ap.center.z);
				sb.AppendFormat("Extent: x:{0}, y:{1}, z:{2}\n", ap.extent.x, ap.extent.y, ap.extent.z);
				sb.AppendFormat("Matrix\n");
				for(int j = 0 ; j < 4 ; j++) {
					var m = ap.transform;
					sb.AppendFormat("{0} {1} {2} {3}\n", m[j,0], m[j,1], m[j,2], m[j,3]);
				}
				var go = arpags[i].gameObject;
				var tr = go.transform;
				var pos = tr.position;
				sb.AppendFormat("pos =  {0}, {1}, {2}\n", pos.x, pos.y, pos.z);
			}
		}

		text.text = sb.ToString();
	}

	bool useLog = true;
	
	void Update() {
		if(Input.touchCount >= 2) {
			useLog = !useLog;
		}
		if(useLog) {
			Refresh();
		}
	}

}
