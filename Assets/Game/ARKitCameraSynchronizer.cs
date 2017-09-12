using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ARKitCameraSynchronizer : MonoBehaviour {
	public UnityARGeneratePlane arGeneratePlane;
	public Camera arCamera;
	public Camera renderCamera;

	void Awake() {
		Debug.Assert(arGeneratePlane != null);
		Debug.Assert(arCamera != null);

		Debug.Assert(renderCamera != null);
	}

	void LateUpdate () {
		// 인식된 바닥이 하나도 없으면 좌표를 계산할수 없으니 추가 카메라를 이용한 렌더링이 필요없다
		var anchorManager = arGeneratePlane.ARAnchorManager;
		List<ARPlaneAnchorGameObject> arpags = anchorManager.GetCurrentPlaneAnchors ();
		if(arpags.Count == 0) {
			renderCamera.enabled = false;

		} else {
			// 첫번째 plane으로 하드코딩
			// plane을 여러개 대응하고 싶으면 그때가서 생각하자
			ARPlaneAnchorGameObject ap = arpags[0];
			renderCamera.enabled = true;
			Synchronize(ap);

			// AR plane 추적 정지
			// destory를 하면 추적 자체가 끊긴다
			// 콜백만 제거하기
			anchorManager.StopAnchor ();
		}
	}

	void Synchronize(ARPlaneAnchorGameObject ap) {
		var plane_mat = ap.gameObject.transform.localToWorldMatrix;
		var plane_invMat = plane_mat.inverse;
		var arcam_mat = arCamera.transform.localToWorldMatrix;
		var mat = plane_invMat * arcam_mat;
		renderCamera.transform.FromMatrix(mat);
		renderCamera.projectionMatrix = arCamera.projectionMatrix;
	}
}