using UnityEngine;
using System.Collections;

public class Explosion : Photon.MonoBehaviour {

	public float damage = 1.0f;
	public float radius = 1.0f;
	public LayerMask Mask;
	public LayerMask hitMask;

	void Start(){

		Collider[] Targets = Physics.OverlapSphere (transform.position, radius, Mask);//Crear esfera de explosion, en radio.
		foreach (Collider target in Targets) {//Por cada objeto tocado en la esfera.
	
	if(photonView.isMine){

			//Obtener la distancia
		Vector3 dir = target.transform.position - transform.position;
		float dist = dir.magnitude;
			
		if (dist > 0) {//La distancia tiene que ser mayor a cero
				// normalizar
			dir /= dist;//Direccion de target es = a direccion/distancia
			RaycastHit hit;

				//Calcular daño
			float hitPoints = radius / dist * damage;

			if (Physics.Raycast (transform.position, dir, out hit, dist, hitMask)) {//Revisar obstaculos
				//Si es jugador, hacer daño al objeto tocado.
				if (hit.collider.tag == "Player" || hit.collider.tag == "localPlayer") {//Si es objeto photon
					hit.collider.gameObject.GetPhotonView ().RPC ("Damage", hit.collider.gameObject.GetPhotonView ().owner, hitPoints, PhotonNetwork.player);
				}
			} else {//Si no se toco nada, el jugador debe estar directo delante de la explosion, hacer daño.
				if (target.tag == "Player" || target.tag == "localPlayer") {//Si es objeto photon
					target.gameObject.GetPhotonView ().RPC ("Damage", target.gameObject.GetPhotonView ().owner, hitPoints, PhotonNetwork.player);
				}
			}

		} else {//Si la distancia es menor a cero, el objeto esta casi dentro de la explosion...
			//Hacer daño si es un jugador.
			if (target.tag == "Player" || target.tag == "localPlayer") {//Si es objeto photon
				target.gameObject.GetPhotonView ().RPC ("Damage", target.gameObject.GetPhotonView ().owner, 1000.0f, PhotonNetwork.player);
			}
		}
	}
			if(target.tag == "localPlayer"){
				target.BroadcastMessage ("cameraShake", SendMessageOptions.DontRequireReceiver);
			}
		}

	}

	void DestroyDisShit(){//Borrar este objeto de mierda en Network
		PhotonNetwork.Destroy (gameObject);//Solo hazlo ya!, MIERDA!.
	}

}
