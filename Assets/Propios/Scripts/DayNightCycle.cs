using UnityEngine;
using System.Collections;

public class DayNightCycle : Photon.MonoBehaviour {

	public Light Sun = null;
	public Light Moon = null;
	public float Dayspeed = 0.005f;
	public bool isNight = false;
	
	private Vector3 Hour = Vector3.zero;
	private float lastSynchronizationTime = 0.0f;
	private float syncDelay = 0.0f;
	private float syncTime = 0.0f;
	private Vector3 syncEndPosition;
	private Quaternion syncStartPosition;

	void Start(){
		Hour = transform.eulerAngles;
	}

	void Update () {
		if (PhotonNetwork.isMasterClient) {
				#pragma warning disable 
				transform.RotateAround (transform.right, Time.deltaTime* Dayspeed);
				Hour = transform.eulerAngles;
			if(Sun.transform.position.y < Moon.transform.position.y){//IsNight
				Sun.enabled = false;
				Moon.enabled = true;
			}else{
				Sun.enabled = true;
				Moon.enabled = false;
			}

		} else {
			SyncedMovement();
			if(Sun.transform.position.y < Moon.transform.position.y){//IsNight
				Sun.enabled = false;
				Moon.enabled = true;
				isNight = true;
			}else{
				Sun.enabled = true;
				Moon.enabled = false;
				isNight = false;
			}
		}
	}
	
	void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		if(PhotonNetwork.isMasterClient){
			photonView.RPC ("SetTime", newPlayer, Hour);
		}
	}

	[RPC]
	void SetTime(Vector3 Hour){
		Quaternion newRot = Quaternion.Euler(Hour);
		transform.rotation = newRot;
	}

	void SyncedMovement(){
		syncTime += Time.deltaTime;
		Quaternion newRot = Quaternion.Euler(syncEndPosition);
		transform.rotation = Quaternion.Slerp (syncStartPosition, newRot, syncTime / syncDelay);
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){//Envio
			stream.SendNext (Hour);
		}else{//Recepcion y sync
			Hour = (Vector3)stream.ReceiveNext ();

			syncEndPosition = Hour;
			syncStartPosition = transform.rotation;
			
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
		}
	}	
}
