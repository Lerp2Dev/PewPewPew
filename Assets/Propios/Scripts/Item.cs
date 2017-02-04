using UnityEngine;
using System.Collections;

public class Item : Photon.MonoBehaviour {

	public float CheckRadius = 5.25f;
	public string MSG = string.Empty;
	//Heal = Curar(cantidad)
	public float unit = 25.0f; 
	public float rotSpeed = 2.125f;

	private bool picked = false;

	void Update () {
		transform.Rotate (Vector3.up* Time.deltaTime * rotSpeed);
		Collider[] checks = Physics.OverlapSphere (transform.position, CheckRadius);
		foreach(Collider i in checks){
			if(!Physics.Linecast (transform.position, i.transform.position)){
				if(Input.GetButton ("Pickup") && i.tag == "localPlayer" && i.gameObject.GetPhotonView().isMine && !picked){
					i.SendMessage (MSG, unit, SendMessageOptions.RequireReceiver);
					photonView.RPC ("Picked", PhotonTargets.MasterClient);
					picked = true;
				}
			}
		}
	}

	[RPC]
	void Picked(){
		PhotonNetwork.Destroy (photonView);
	}
}
