using UnityEngine;
using System.Collections;

public class Bullet : Photon.MonoBehaviour
{
    
    public float lifetime = 2.0f;
    
    public GameObject impactEffect = null; // default impact effect
	public GameObject impactBEffect = null;//BLood effect
	public float damage;             // damage bullet applies to a target
    public float speed;              // bullet speed
	public float force = 5.0f;
	public bool destroyHit = false; //Destroy on hit
	public bool distInstance = false;//Distant instantiate.
	public float distInst = 1.0f;//Distance to instantiate.
	public LayerMask hitMask;

    private Vector3 velocity = Vector3.zero; // bullet velocity
	private Vector3 newPos = Vector3.zero;   // bullet's new position
    private Vector3 oldPos = Vector3.zero;   // bullet's previous location
    private bool hasHit = false;             // has the bullet hit something?
	private float shotTime = 999999999.0f;

	public void Awake() // information sent from gun to bullet to change bullet properties
    {   
		shotTime = Time.time;
        newPos = transform.position;   // bullet's new position
        oldPos = newPos;               // bullet's old position
        velocity = speed * transform.forward; // bullet's velocity determined by direction and bullet speed
    }

   // Update is called once per frame
    void Update(){

		if(photonView.isMine){

		if(Time.time > shotTime + lifetime){
			PhotonNetwork.Destroy (this.gameObject);
		}

       if (hasHit){
            return;
		}
        // assume we move all the way
        newPos += velocity * Time.deltaTime;

        // Check if we hit anything on the way
        Vector3 dir = newPos - oldPos;
        float dist = dir.magnitude;

        if (dist > 0){
            // normalize
            dir /= dist;
			RaycastHit hit;
            if(Physics.Raycast(oldPos, dir, out hit, dist, hitMask)){ 
				newPos = hit.point;   
				if(hit.collider != null){
					if(hit.rigidbody){
						hit.rigidbody.AddForceAtPosition(force * transform.forward, hit.point);
					}
					if(!distInstance){
						if(hit.collider.tag != "Player"){
							Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);        
							PhotonNetwork.Instantiate(impactEffect.name, hit.point, rotation, 0);
							PhotonNetwork.Destroy (gameObject);
							hasHit = true;
						}else{
							PhotonView i = hit.collider.gameObject.GetPhotonView ();
							if(PhotonNetwork.room.customProperties["mode"].ToString () == "DeathMatch" || i.owner.GetTeam () != PhotonNetwork.player.GetTeam ()){
								i.RPC ("Damage", i.owner, damage, PhotonNetwork.player);
							}
							Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);    
							PhotonNetwork.Instantiate(impactBEffect.name, hit.point, rotation, 0);
							PhotonNetwork.Destroy (gameObject);
							hasHit = true;
						}
					}else{
						if(hit.collider.tag != "Player"){
							Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);        
								PhotonNetwork.Instantiate(impactEffect.name, transform.position-transform.forward * distInst, rotation, 0);
							PhotonNetwork.Destroy (gameObject);
							hasHit = true;
						}else{
							PhotonView i = hit.collider.gameObject.GetPhotonView ();
							if(PhotonNetwork.room.customProperties["mode"].ToString () == "DeathMatch" ||  i.owner.GetTeam () != PhotonNetwork.player.GetTeam ()){
									i.RPC ("Damage", i.owner, damage, PhotonNetwork.player);
							}
							Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);    
							PhotonNetwork.Instantiate(impactBEffect.name, transform.position-transform.forward * distInst, rotation, 0);
							PhotonNetwork.Destroy (gameObject);
							hasHit = true;
						}
					}
				}
			}
		}

		oldPos = transform.position;  // set old position to current position
        transform.position = newPos;  // set current position to the new position
		return;

		}else{
			transform.position += velocity * Time.deltaTime;
		}
	}
}


/////19-Abril-2015 10:29 a.m.
/// NOTAS:
/// Hice un cambio en las RPCs donde PhotonTargets antes eran .All
/// Ahora solo se las manda al usuario que es dueño del objeto atacado
