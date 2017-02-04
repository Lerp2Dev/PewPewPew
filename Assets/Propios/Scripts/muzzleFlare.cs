using UnityEngine;
using System.Collections;

public class muzzleFlare : Photon.MonoBehaviour {
	public bool rrandom = false;
	public bool randomPitch = false;
	public bool SND = false;
	public bool netDestroy = false;
	public float Delay = 0.2f;
	public AudioSource AudioSRC = null;
	public AudioClip AudioCLP = null;
	public AudioClip[] RNDSND = null;

	private float lastNet = 99999.0f;
	void Start () {

		if(RNDSND.Length > 0){
			AudioSRC.clip = RNDSND[Random.Range (0,RNDSND.Length)];
			AudioSRC.Play ();
		}

		if (randomPitch) {
			AudioSRC.pitch = Random.Range (1.0f, 1.55f);
		}

		if(rrandom){
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range (0, 360));
		}
		if(SND){
			AudioSRC.clip = AudioCLP;
			AudioSRC.Play ();
		}
		if(netDestroy){
			lastNet = Time.time;
		}else{
			Destroy (this.gameObject, Delay);
		}
	}

	void Update(){
		if(netDestroy && Time.time > lastNet + Delay && photonView.isMine){
			PhotonNetwork.Destroy (this.gameObject);
		}
	}
}