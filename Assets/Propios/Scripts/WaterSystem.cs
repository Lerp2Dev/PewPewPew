using UnityEngine;
using System.Collections;

public class WaterSystem : Photon.MonoBehaviour {
	public PlayerMotor player = null;
	public GameObject waterMoving = null;
	public float waterInterval = 1.0f;
	public float distance = 1.0f;
	private float lastWater = -10.0f;

	void Update () {

		if (Camera.main == null)
						return;

		if (player == null || !player.gameObject.GetPhotonView().isMine) {
			if (GameObject.FindWithTag ("localPlayer") != null && GameObject.FindWithTag ("localPlayer").GetPhotonView().isMine) {
				player = GameObject.FindWithTag ("localPlayer").GetComponent<PlayerMotor>();
			} else {
				return;
			}
		} else {
			if(player.transform.position.y < this.transform.position.y){
				player.isUnderwater = true;
				Camera.main.SendMessage ("OnCover", SendMessageOptions.RequireReceiver);
				RaycastHit hit;
				if(Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y+distance, player.transform.position.z), -Vector3.up, out hit,Mathf.Infinity)){
					if(hit.collider == this.collider && Time.time > lastWater + waterInterval){
						Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);    
						PhotonNetwork.Instantiate(waterMoving.name, hit.point, rotation, 0);
						lastWater = Time.time;
					}
				}
			}else{
				player.isUnderwater = false;
			}

		}
	}
}
