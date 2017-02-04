using UnityEngine;
using System.Collections;

public class ShotgunShell : Photon.MonoBehaviour {

	public Transform balaPrefab = null;
	public float rocio = 15.0f;
	public int Balas = 6;
	
	void Start(){
		if (photonView.isMine) {
						float rocioRange = rocio / 2.0f;
						for (int i = 0; i < Balas; i++) {
			
								float variancex = Random.Range (-rocioRange, rocioRange);
								Quaternion rotationx = Quaternion.AngleAxis (variancex, transform.right);
			
								float variancey = Random.Range (-rocioRange, rocioRange);
								Quaternion rotationy = Quaternion.AngleAxis (variancey, transform.up);
								float variancez = Random.Range (-rocioRange, rocioRange);
								Quaternion rotationz = Quaternion.AngleAxis (variancez, transform.forward);
								PhotonNetwork.Instantiate (balaPrefab.name, transform.position, transform.rotation * rotationx * rotationy * rotationz, 0);
						}
						PhotonNetwork.Destroy (this.gameObject);
		}
	}

}
