using UnityEngine;
using System.Collections;

public class ItemSpawner : Photon.MonoBehaviour {

	public Transform[] Points;
	public Transform[] Items;
	public float CheckDelay = 7.125f;
	private float lastCheck = -10.0f;
	
	void Update () {
		if (PhotonNetwork.isMasterClient && Time.time > lastCheck + CheckDelay) {
			for(int y = 0; y<Points.Length; y++){
				y = Random.Range (0,Points.Length);
				if(Points[y].childCount == 0){
					GameObject i = PhotonNetwork.Instantiate (Items[Random.Range (0,Items.Length)].name, Points[y].position, Points[y].rotation, 0) as GameObject;
					i.transform.parent = Points[y];
					lastCheck = Time.time;
					break;
				}
			}
			lastCheck = Time.time;
		}
	}

}
