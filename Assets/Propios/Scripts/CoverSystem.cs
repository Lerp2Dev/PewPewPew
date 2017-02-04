using UnityEngine;
using System.Collections;

public class CoverSystem : Photon.MonoBehaviour {

	private Transform player = null;
	public float reqRadius = 3.0f;

	void Update() {		
		if(Camera.main == null)
			return;

		if (player == null || !player.gameObject.GetPhotonView().isMine) {
			if (GameObject.FindWithTag ("localPlayer") != null && GameObject.FindWithTag ("localPlayer").GetPhotonView().isMine) {
				player = GameObject.FindWithTag ("localPlayer").transform;
			} else {
				return;
			}
				
		}
		ArrayList arr = new ArrayList(Physics.OverlapSphere (transform.position, reqRadius));
		if (arr.Contains (player.collider)) {
			Camera.main.SendMessage ("OnCover", SendMessageOptions.DontRequireReceiver);
		}
	}
}