using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour {
	public string Nombre = string.Empty;
	public bool Wait4Load = false;
	public bool DelayLoad = false;
	public bool isJumpAble = false;
	public float Delay = 1.0f;
	private float last = -10.0f;

	void Start () {
		last = Time.time;
		if (!Wait4Load && Application.CanStreamedLevelBeLoaded (Nombre)) {
			Application.LoadLevel (Nombre);
		}
	}

	void Update(){
		if (!DelayLoad) {
			if (Wait4Load && Application.CanStreamedLevelBeLoaded (Nombre)) {
				Application.LoadLevel (Nombre);
			}
		} else {
			if(Time.time > last + Delay && Application.CanStreamedLevelBeLoaded (Nombre)){
				Application.LoadLevel (Nombre);
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape) && isJumpAble && Application.CanStreamedLevelBeLoaded (Nombre)){
			Application.LoadLevel (Nombre);
		}

	}
}

