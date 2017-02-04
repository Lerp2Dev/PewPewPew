using UnityEngine;
using System.Collections;

public class PlayerMotor : Photon.MonoBehaviour {

	public float Bodyspeed = 1.0f;
	public float walkspeed = 1.0f;
	public float Syncspeed = 1.0f;
	public float Gravity = -1.0f;
	public float Jumpforce = 10.0f;
	public float JumpLength = 5.0f;
	public float ShootingInterval = 0.5f;
	public float StepInterval = 0.5f;
	public float HP = 100.0f;
	public float FallSpeed = 20.0f;
	public float AlphaSpeed = 5.0f;
	public Texture DMGTex = null;
	public CharacterController C = null;
	public Transform Modelo = null;
	public Transform Cuerpo = null;
	public string RunAnim = "Run";
	public string BRunAnim = "BRun";
	public string idleAnim = "idle";
	public string shootAnim = "shoot";
	public Transform muzzle = null;
	public Transform torso = null;
	public Transform pelvis = null;
	public Transform FlarePrefab = null;
	public Transform BulletPrefab = null;
	public Transform minPosition = null;
	public Transform maxPosition = null;
	public Transform AimingCursor = null;
	public AudioSource ShootAudio = null;
	public AudioClip ShootingClip = null;
	public AudioSource StepAudio = null;
	public AudioClip[] StepClip = null;
	public AudioSource FallAudio = null;
	public AudioClip[] FallClip = null;
	public Flare FlashLightFlare = null;
	public GameObject waterSplash = null;
	public GameObject[] GunsAvaible = null;
	public TextMesh Name3D = null;
	public Vector3 Text3DAdjust = Vector3.zero;
	
	private float lastShot = -10.0f;
	private float LastStep = -10.0f;
	private float LastJump = -10.0f;
	private float LastLight = -10.0f;
	private float lastTeamCheck = -10.0f;
	private float Mainspeed = 0.0f;
	private float Alpha = 0.0f;
	private float barrelDist = 0.0f;
	private Vector3 newPos = Vector3.zero;
	private Vector3 oldPos = Vector3.zero;
	private Vector3 input = Vector3.zero;
	private Vector3 mouseWPoint = Vector3.zero;
	private Vector3 startGunCamEuler = Vector3.zero;
	private Quaternion remoteRotation = Quaternion.identity;
	private short lowerBody = 0;//0 = idle, 1 = walking
	private short upperBody = 0;//0 = idle, 1 = shooting
	private short flashlight = 0;//0 = off, 1 = on
	private bool Jumping = false;
	private bool isShooting = false;
	private bool isFPSAiming = false;
	private int gunID = 0;
	private Camera gunCamera = null;
	private Camera mainCamera = null;

	[HideInInspector]
	public bool isUnderwater = false;
	//#pragma warning disable 
	//private DayNightCycle Cycle = null;

	void Awake(){
		Name3D.gameObject.SetActive (false);
		if(photonView.isMine){
			FallSpeed = GameObject.FindWithTag("SpawnContainer").GetComponent<SpawnContainer>().MaxFall;
			tag = "localPlayer";
			photonView.RPC ("SetName", PhotonTargets.AllBuffered, PhotonNetwork.playerName);
			mainCamera = GameObject.FindWithTag ("MainCamera").camera;
		}else{
			muzzle.light.flare = FlashLightFlare;
			AimingCursor.renderer.enabled = false;
			tag = "Player";
			//Cycle = GameObject.FindWithTag("Cycle").GetComponent<DayNightCycle>();
			SetWeapon (gunID);
		}


		//SETUP
		SetWeapon (gunID);
		//SETUP

		newPos = transform.position;   
		oldPos = newPos; 
		remoteRotation = transform.rotation;
		Modelo.animation.CrossFade (idleAnim, 0.2f);
	}

	void Update () {
		if(photonView.isMine){

		//Movimiento
			if(C.isGrounded){
				if(!isFPSAiming){
					input = new Vector3(-Input.GetAxis("Vertical")*Mainspeed, 0.0f, Input.GetAxis("Horizontal")*Mainspeed);
				}else{
					input = new Vector3(0.0f, 0.0f, 0.0f);
				}
			}else{
				input = new Vector3(input.x, 0.0f, input.z);
			}

		if(!isUnderwater){
			Mainspeed = walkspeed;
		}else{
			Mainspeed = walkspeed*0.35f;
		}
		Alpha -= AlphaSpeed * Time.deltaTime;
		Alpha = Mathf.Clamp(Alpha, 0.0f, 1.0f);
		remoteRotation = transform.rotation;

		if(Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0 && C.isGrounded && !isShooting){
				Modelo.animation.CrossFade(idleAnim, 0.2f, PlayMode.StopAll);
				upperBody = 0;
				lowerBody = 0;
		}else if(Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0 && C.isGrounded && isShooting){
				Modelo.animation.CrossFade(idleAnim, 0.2f);
				Modelo.animation.Stop (RunAnim);
				lowerBody = 0;
		}else if((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && C.isGrounded && isShooting && !isFPSAiming){
			Modelo.animation.CrossFade (RunAnim, 0.2f);	
			if(Time.time > LastStep + StepInterval && C.isGrounded && !isUnderwater){
				StepAudio.clip = StepClip[Random.Range(0,StepClip.Length)];
				StepAudio.Play ();
				LastStep = Time.time;
			}
				lowerBody = 1;
		}else if((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && C.isGrounded && !isShooting && !isFPSAiming){
				Modelo.animation.CrossFade (RunAnim, 0.2f);	
				Modelo.animation.Stop (shootAnim);
				if(Time.time > LastStep + StepInterval && C.isGrounded && !isUnderwater){
					StepAudio.clip = StepClip[Random.Range(0,StepClip.Length)];
					StepAudio.Play ();
					LastStep = Time.time;
				}
				upperBody = 0;
				lowerBody = 1;
			}  
		
			if(Input.GetButtonDown("FlashLight") && Time.time > LastLight + 0.5f){
				if(flashlight == 0){
					muzzle.light.enabled = true;
					flashlight = 1;
					LastLight = Time.time;
				}else{
					muzzle.light.enabled = false;
					flashlight = 0;
					LastLight = Time.time;
				}
			}

			if(C.isGrounded){//Si esta tocando el suelo
				if(Input.GetButton("Jump") && Time.time > LastJump + JumpLength){
					input.y = 0;
					Jumping = true;
					LastJump = Time.time;
				
				}
			}else{//Si no..
				if(Jumping && Time.time < LastJump + JumpLength){//Y esta saltando
						input.y += Jumpforce * Time.deltaTime;
				}else{//Si no ha saltado y solo esta cayendo
					input.y -= Gravity * Time.deltaTime;
						Jumping = false;
				}
			}
		if(gunCamera == null){
				AimingCursor.renderer.enabled = false;
			if(Input.GetButton ("Fire1") && !isUnderwater && !isShooting){//Disparo
				Disparar ();
			}
		}else{
			if(isFPSAiming && Input.GetButton ("Fire1") && !isUnderwater && !isShooting){//Disparo
				Disparar ();
			}	
		}

		if(Time.time > lastRTime + 5.5f){
				lastReceived = null;
		}

			if(Input.GetButton ("AimedShot") && !isFPSAiming){
				AimingCursor.renderer.enabled = true;
			Ray r = Camera.main.ScreenPointToRay (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
			RaycastHit hit;
				if (Physics.Raycast (r, out hit)) {
					mouseWPoint = hit.point;
				}
			mouseWPoint.y = Mathf.Clamp (mouseWPoint.y, minPosition.position.y, maxPosition.position.y) -0.25f;
			AimingCursor.position = mouseWPoint;
		}else{
				AimingCursor.renderer.enabled = false;
		}

		//Codigo para mover todo el cuerpo al mouse
		if(!isFPSAiming){
			Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
			Vector3 dir = Input.mousePosition - pos;
			float angle2 = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.AngleAxis(-angle2, transform.up), Time.deltaTime*Bodyspeed); 
				MLook.enabled = false;
		}


		C.Move(input);//Movimiento final
		}else{//Sincronizacion con remoto
			SyncedMovement();
			//transform.position = Vector3.Lerp (syncStartPosition, input, Time.deltaTime*Syncspeed);
			transform.rotation = Quaternion.Slerp (transform.rotation, remoteRotation, Time.deltaTime*Syncspeed);

			if(upperBody == 0 && lowerBody == 0 && C.isGrounded){//idle
				Modelo.animation.CrossFade(idleAnim, 0.2f, PlayMode.StopAll);
			
			}else if(upperBody == 1 && lowerBody == 0 && !isUnderwater){//Quieto disparando
				Modelo.animation.CrossFade (shootAnim, 0.2f);
				Modelo.animation.Stop (RunAnim);
				if(Time.time > lastShot + ShootingInterval){
					//Sonido
					ShootAudio.clip = ShootingClip;
					ShootAudio.Play ();
					lastShot = Time.time;
				}
			} else if(upperBody == 1 && lowerBody == 1 && C.isGrounded){//Camindando y disparando
				Modelo.animation.CrossFade (RunAnim, 0.2f);	
				if(Time.time > LastStep + StepInterval && C.isGrounded && !isUnderwater){
					StepAudio.clip = StepClip[Random.Range(0,StepClip.Length)];
					StepAudio.Play ();
					LastStep = Time.time;
				}
			}else if(upperBody == 0 && lowerBody == 1 && C.isGrounded){//Camindando
				Modelo.animation.CrossFade (RunAnim, 0.2f);
				Modelo.animation.Stop (shootAnim);
				if(Time.time > LastStep + StepInterval && C.isGrounded && !isUnderwater){
					StepAudio.clip = StepClip[Random.Range(0,StepClip.Length)];
					StepAudio.Play ();
					LastStep = Time.time;
				}
			}
		
			if(flashlight == 0){
				muzzle.light.enabled = false;
			}else{
				muzzle.light.enabled = true;
			}

			if(Modelo != GunsAvaible[gunID]){//Cambiar armas si no es la actual en el modelo
				SetWeapon (gunID);
			}else if(GameObject.FindWithTag ("localPlayer") != null){//Si el arma es actual, ver que el jugador este en la misma area que el jugador local
				GameObject player = GameObject.FindWithTag ("localPlayer");
				if(player.GetPhotonView().isMine){
					if(player.transform.position.y > transform.position.y-2.5f && player.transform.position.y < transform.position.y+2.5f){
						Modelo.gameObject.SetActive (true);
					}else{
						Modelo.gameObject.SetActive (false);
					}
				}
			}
			if(Time.time > lastTeamCheck + 5.0f){
				PunTeams.Team playerTeam = photonView.owner.GetTeam ();
				PunTeams.Team ourPlayerTeam = PhotonNetwork.player.GetTeam ();
				if(playerTeam == ourPlayerTeam){//SOMOS DEL MISMO EQUIPO
					switch(ourPlayerTeam){
					case PunTeams.Team.blue ://SOMOS EQUIPO AZUL
						Modelo.GetComponent <GunScript>().Malla.materials[0].SetColor ("_OutlineColor", Color.blue);
						Name3D.gameObject.SetActive (true);
						break;
					case PunTeams.Team.red://SOMOS EQUIPO ROJO
						Modelo.GetComponent <GunScript>().Malla.materials[0].SetColor ("_OutlineColor", Color.blue);
						Name3D.gameObject.SetActive (true);
						break;
					case PunTeams.Team.none:
						Modelo.GetComponent <GunScript>().Malla.materials[0].SetColor ("_OutlineColor", Color.black);
						Name3D.gameObject.SetActive (false);
					break;
					
					}
				}else{//NO LO SOMOS
					Modelo.GetComponent <GunScript>().Malla.materials[0].SetColor ("_OutlineColor", Color.black);
					Name3D.gameObject.SetActive (false);
				}
				lastTeamCheck = Time.time;
			}
		}
		Name3D.transform.position = transform.position - Text3DAdjust;
		Name3D.transform.rotation = Quaternion.Euler (new Vector3(90.0f, -90.0f, 0.0f));
		HP = Mathf.Clamp (HP, 0.0f, 100.0f);
	}

	void FixedUpdate(){
	if(photonView.isMine){

		newPos = transform.position;

		if(C.isGrounded){
				Vector3 dir = newPos - oldPos;
				float dist = dir.magnitude;
			if (dist > 0){
					dir /= dist;
					RaycastHit hit;
				if(Physics.Raycast(oldPos, dir, out hit, dist)){
					if((newPos.y - oldPos.y) < FallSpeed && !isUnderwater){
							FallAudio.clip = FallClip[Random.Range (0,FallClip.Length)];
							FallAudio.Play ();
							photonView.RPC ("PlaySound", PhotonTargets.Others, 0);
							Damage(dist+(dist*0.5f), PhotonNetwork.player);
					}else if(isUnderwater && hit.collider.tag == "water"){
						Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); 
						PhotonNetwork.Instantiate(waterSplash.name, hit.point, rotation, 0);
					}
				}
			}
		oldPos = transform.position; 
		}
	}
	}

	void Disparar(){
		if (isShooting)
						return;

			isShooting = true;
			Modelo.animation.Play (idleAnim, PlayMode.StopSameLayer);
			Modelo.animation.Play (shootAnim, PlayMode.StopSameLayer);
		                       //Sonido
			ShootAudio.clip = ShootingClip;
			ShootAudio.Play ();
			//Instanciar
			PhotonNetwork.Instantiate (FlarePrefab.name, muzzle.position, muzzle.rotation, 0);

		if(gunCamera == null){
			upperBody = 1;

			if(Input.GetButton ("AimedShot") && Vector3.Distance (transform.position, mouseWPoint) > 5){
				Quaternion rot = Quaternion.LookRotation (-(muzzle.position - mouseWPoint));
				PhotonNetwork.Instantiate (BulletPrefab.name, muzzle.position - muzzle.forward * barrelDist, rot, 0);
			}else{
				PhotonNetwork.Instantiate (BulletPrefab.name, muzzle.position - muzzle.forward * barrelDist, transform.rotation, 0);
			}
		}else{
			if(Input.GetButton ("AimedShot")){
				PhotonNetwork.Instantiate (BulletPrefab.name, gunCamera.transform.position - gunCamera.transform.forward * barrelDist , gunCamera.transform.rotation, 0);
			}else{
				PhotonNetwork.Instantiate (BulletPrefab.name, muzzle.position - muzzle.forward * barrelDist, transform.rotation, 0);
			}
		}
		StartCoroutine ("ResetShooting");

	}

	IEnumerator ResetShooting(){
		yield return new WaitForSeconds (ShootingInterval);
		isShooting = false;
		upperBody = 0;
	}
	

	void SetWeapon(int i){
		Modelo.gameObject.SetActive (false);
		gunID = i;
		foreach (GameObject gun in GunsAvaible) {
			GunScript script = gun.GetComponent<GunScript>();
			if(i == script.id){
				barrelDist = script.barrelDist;
				muzzle = script.muzzle;
				ShootingInterval = script.ShootingInterval;
				torso = script.torso;
				pelvis = script.pelvis;
				FlarePrefab = script.FlarePrefab;
				BulletPrefab = script.BulletPrefab;
				ShootingClip = script.ShootingClip;
				script.Modelo.gameObject.SetActive(true);
				Modelo = script.Modelo;
				if(script.usesCamera){
					gunCamera = script.camera;
					startGunCamEuler = gunCamera.transform.localEulerAngles;
					gunCamera.gameObject.SetActive (false);
				}else{
					gunCamera = null;
				}
				break;
			}
		}
	
		if(flashlight == 0){
			muzzle.light.enabled = false;
			LastLight = Time.time;
		}else{
			muzzle.light.enabled = true;
			LastLight = Time.time;
		}
		Screen.lockCursor = false;
		Screen.showCursor = true;
		Modelo.animation[shootAnim].weight = 0.2f;
		Modelo.animation[shootAnim].layer = 2;
		Modelo.animation[shootAnim].AddMixingTransform(torso, true);
		Modelo.animation[RunAnim].weight = 0.5f;
		Modelo.animation[RunAnim].layer = 1;
		Modelo.animation[BRunAnim].AddMixingTransform(pelvis, true);
		Modelo.animation[BRunAnim].weight = 0.5f;
		Modelo.animation[BRunAnim].layer = 1;
		Modelo.animation[BRunAnim].AddMixingTransform(pelvis, true);
		Modelo.animation [idleAnim].weight = 1.0f;
		Modelo.animation [idleAnim].layer = -2;
		Modelo.animation.CrossFade (idleAnim, 0.2f);
	}

	public float pushPower = 2.0f;
	public float weight = 6.0f;
	
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		Rigidbody  body = hit.collider.attachedRigidbody;
		Vector3 force = Vector3.zero;
		
		// no rigidbody
		if (body == null || body.isKinematic) { return; }
		
		// We use gravity and weight to push things down, we use
		// our velocity and push power to push things other directions
		if (hit.moveDirection.y < -0.3f) {
			force = new Vector3 (0.0f, -0.5f, 0.0f) * Gravity * weight;
		} else {
			force = hit.controller.velocity * pushPower;
		}
		
		// Apply the push
		body.AddForceAtPosition(force, hit.point);
	}

	void cameraShake(){
		//photonView.RPC ("camShake", photonView.owner);
		Camera.main.BroadcastMessage ("camShake", SendMessageOptions.DontRequireReceiver);
	}

	/*[RPC]
	void camShake(){
		Camera.main.BroadcastMessage ("camShake", SendMessageOptions.DontRequireReceiver);
	}
	*/
	
	private PhotonPlayer lastReceived = null;
	private float lastRTime = -10.0f;

	[RPC]
	void Damage(float i, PhotonPlayer sender){
		if(!photonView.isMine)
			return;

		GameObject.FindWithTag ("MainCamera").BroadcastMessage ("Damage", i, SendMessageOptions.DontRequireReceiver);
		HP -= i;
		Alpha += i;
		if((HP < 0.0f || HP == 0.0f || HP < 1.0f) && !dead){
			if(sender != null && sender != PhotonNetwork.player){
				Camera.main.gameObject.SendMessage ("KillCamera", sender.name, SendMessageOptions.RequireReceiver);
				GameObject.FindWithTag ("GameController").GetPhotonView().RPC ("KilledSomeone", sender, PhotonNetwork.playerName);
				if(lastReceived != sender){
					ArrayList arr = new ArrayList(PhotonNetwork.playerList);
					if(arr.Contains (lastReceived)){
						GameObject.FindWithTag ("GameController").GetPhotonView().RPC ("AsistSomeone", lastReceived, PhotonNetwork.playerName);
					}
				}
			}else{
				GameObject.FindWithTag ("GameController").GetPhotonView().RPC ("MSGServer", PhotonTargets.All, "["+PhotonNetwork.playerName + " committed suicide..."+"]");
			}
			PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable() {{"deaths", int.Parse(PhotonNetwork.player.customProperties["deaths"].ToString ())+1}});
			PhotonNetwork.player.SetScore (PhotonNetwork.player.GetScore ()-10);
			PhotonNetwork.Instantiate(Cuerpo.name, transform.position, transform.rotation, 0);
			dead = true;
			PhotonNetwork.Destroy (this.gameObject);
		}//Agregar aqui en un else indicador de daño

		lastRTime = Time.time;
		lastReceived = sender;
	}
	private bool dead = false;

	[RPC]
	void PlaySound(int i){
		//0=Caida
		if (i == 0) {
			FallAudio.clip = FallClip [Random.Range (0, FallClip.Length)];
			FallAudio.Play ();
		}
	}

	[RPC]
	void SetName(string i){
		gameObject.name = i;
		Name3D.text = i;
	}

	void Heal(float i){//curar
		HP += i;
	}

	void OnGUI(){
		if(!photonView.isMine)
			return;

		if (gunCamera != null && !isUnderwater){
			if(Input.GetButton ("AimedShot")) {
				mainCamera.gameObject.SetActive (false);
				gunCamera.gameObject.SetActive (true);
				isFPSAiming = true;
				MLook.enabled = true;
				Screen.lockCursor = true;
				Screen.showCursor = false;
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), Crosshair, ScaleMode.StretchToFill, true);
			} else {
				isFPSAiming = false;
				MLook.enabled = false;
				Screen.lockCursor = false;
				Screen.showCursor = true;
				gunCamera.transform.localEulerAngles = startGunCamEuler;
				mainCamera.gameObject.SetActive (true);
				gunCamera.gameObject.SetActive (false);
			}
		}else{
			isFPSAiming = false;
			MLook.enabled = false;
			Screen.lockCursor = false;
			Screen.showCursor = true;
			mainCamera.gameObject.SetActive (true);
		}

		HPText.fontSize = (int)(Screen.width*HPIconSize.x/2);
		GUI.Label (new Rect(Screen.width*HPIconPos.x, Screen.height*HPIconPos.y, HPIconSize.x, HPIconSize.y), Mathf.Round (HP) + "%", HPText);
		GUI.DrawTexture (new Rect (HPPos.x , HPPos.y, Screen.width*HPSize.x, Screen.height*HPSize.y), HPContainer, ScaleMode.StretchToFill, true);

		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b,Alpha);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), DMGTex, ScaleMode.StretchToFill, true);

	}

	public Vector2 HPPos = Vector2.zero;
	public Vector2 HPSize = Vector2.zero;

	public Vector2 HPIconPos = Vector2.zero;
	public Vector2 HPIconSize = Vector2.zero;
	public GUIStyle HPText = null;
	/*
	public float IconSpace = 10.0f;
	public Texture HPIcon = null;
	*/
	public Texture HPContainer = null;
	public Texture Crosshair = null;
	public MouseLook MLook = null;
	
	private float lastSynchronizationTime = 0.0f;
	private float syncDelay = 0.0f;
	private float syncTime = 0.0f;
	private Vector3 syncEndPosition = Vector3.zero;
	private Vector3 syncStartPosition = Vector3.zero;

	void SyncedMovement(){
		syncTime += Time.deltaTime;
		transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){//Envio
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			stream.SendNext (upperBody);
			stream.SendNext (lowerBody);
			stream.SendNext (flashlight);
			stream.SendNext	(gunID);
		}else{//Recepcion y sync
			newPos = (Vector3)stream.ReceiveNext ();
			remoteRotation = (Quaternion)stream.ReceiveNext ();
			upperBody = (short)stream.ReceiveNext ();
			lowerBody = (short)stream.ReceiveNext ();
			flashlight = (short)stream.ReceiveNext ();
			gunID = (int)stream.ReceiveNext ();

			syncEndPosition = newPos;
			syncStartPosition = transform.position;

			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
		}
	}
}

