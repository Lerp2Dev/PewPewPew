using UnityEngine;
using System.Collections;

public class RoofTransparency : Photon.MonoBehaviour {

	public bool levelController = false;
	public float level = 10.0f;

	void Update(){
		if (Camera.main != null && renderer.isVisible && !levelController) {
			RaycastHit hit;
			if(Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity)){
				if(hit.collider.transform == this.transform){
					renderer.material.color = new Color (this.renderer.material.color.r, this.renderer.material.color.g, this.renderer.material.color.b, 0.25f);
					Camera.main.SendMessage ("OnCover", SendMessageOptions.DontRequireReceiver);
				}else{
					renderer.material.color = new Color(this.renderer.material.color.r, this.renderer.material.color.g, this.renderer.material.color.b, 1.0f);
				}
			}

		}else if(GameObject.FindWithTag ("localPlayer") != null){
				GameObject player = GameObject.FindWithTag("localPlayer");
				if(!player.GetPhotonView().isMine)
				return;

			if(player.transform.position.y < level){
				Camera.main.SendMessage ("OnCover", SendMessageOptions.DontRequireReceiver);
				renderer.enabled = false;
			}else{
				renderer.enabled = true;
			}
		}
	}
}