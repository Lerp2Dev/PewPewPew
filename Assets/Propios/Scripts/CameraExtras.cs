using UnityEngine;
using System.Collections;

public class CameraExtras : MonoBehaviour {

	public float shakeAmount = 2.55f;

	private Vector3 shake = Vector3.zero;

	void camShake(){
		shake = transform.localPosition;
		shake += new Vector3 (Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount));
		transform.localPosition = shake;
	}

}
