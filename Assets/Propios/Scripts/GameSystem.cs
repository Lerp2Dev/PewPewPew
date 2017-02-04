using UnityEngine;
using System.Collections;

public class GameSystem : Photon.MonoBehaviour{

	public string PlayerName = "Player";
	public string GameVersion = "1.0a";
	public string PrefabJugador = "localPlayer";
	public float conInterval = 1.0f;
	public float ListInterval = 1.5f;
	public Vector2 lobbySize = Vector2.zero;
	public Vector2 creationSize = Vector2.zero;
	public Vector2 creationPos = Vector2.zero;
	public Vector2 msgSize = Vector2.zero;
	public Vector2 msgPos = Vector2.zero;
	public float EnemyCheckRadious = 75.0f;
	public AudioSource EventSND = null;
	public AudioSource MusicSND = null;
	public AudioClip[] MTanzasSNDs = null;
	public AudioClip AsistSND = null;
	public string[] Maps = null;
	public string[] creditos = null;
	public string MainMenu = "DevTest";

	public GameModes GameMode = GameModes.DeathMatch;
	public enum GameModes {DeathMatch = 0, TeamDeathMatch = 1};

	public gameMenu MenuState = gameMenu.Main;
	public enum gameMenu {Main = 0, Lobby = 1, Game = 2, Settings = 3, Credits = 4};

	private float lastConTry = -10.0f; 
	private float lastGameList = -10.0f;
	private float lastChat = -10.0f;
	private float lastPlayer = -10.0f;
	private float CAdjust = 1.0f;//Camera adjust
	private float gameTimeLimit = 20.0f;//Minutos que deben pasar para terminar la ronda
	private float gameTime = 0.0f;//Minutos pasados
	private float lastTimeUpdate = -10.0f;//La ultima vez que se actualizaron los minutos
	private int VDetails = 0;//Visual details
	private float SVolume = 1.0f;//Sound volume
	private RoomInfo[] RoomList;
	private ArrayList Messages = new ArrayList();
	private ArrayList KillMessages = new ArrayList();
	private PhotonPlayer[] playerList;
	private PhotonPlayer[] playerListA;
	private PhotonPlayer[] playerListB;
	private bool chatting = false;
	private bool gameStarted = false;//Detectar si inicio el juego o no.
	private string chatField = "";
	private Vector2 featureScroll = Vector2.zero;//Scroll de info del room
	private Vector2 mapScroll = Vector2.zero;//Scroll del selector de mapas
	private Vector2 settingScroll = Vector2.zero;//Scroll settings
	void Start(){
		DontDestroyOnLoad (this);
		PlayerName = PlayerPrefs.GetString ("name", "Player" + Random.Range (0, 999));
		nmbrPartida = PlayerPrefs.GetString ("Rname", "Game" + Random.Range (0, 999));
		CAdjust = PlayerPrefs.GetFloat ("CAdjust", 1.0f);
		VDetails = PlayerPrefs.GetInt ("VDetails", 0);
		QualitySettings.SetQualityLevel (VDetails);
		SVolume = PlayerPrefs.GetFloat ("SVolume", 1.0f);
		AudioListener.volume = SVolume;
	}

	void Update(){
		if(!PhotonNetwork.connected && Time.time > lastConTry + conInterval){
			PhotonNetwork.ConnectUsingSettings(GameVersion);
			lastConTry = Time.time;
		}
		if(PhotonNetwork.connected){

			if(Time.time > lastGameList + ListInterval && !PhotonNetwork.inRoom){
				PhotonNetwork.playerName = PlayerName;
				RoomList = PhotonNetwork.GetRoomList();
				lastGameList = Time.time;
			}

			if(PhotonNetwork.inRoom){

				if(Time.time > lastPlayer + 5.0f){
					getPlayerList ();
					lastPlayer = Time.time;
				}

				if(Time.time > lastTimeUpdate + 1.0f){
					if(gameStarted){//Cuando pasen 1 segundo..
						if(!gameTime.ToString ().Contains (".6")){
							gameTime += 0.01f;
						}else{
							gameTime = Mathf.RoundToInt(gameTime);
							gameTime += 1.0f;
						}
						gameTime = float.Parse (gameTime.ToString ("0.00"));
						lastTimeUpdate = Time.time;
					}
				}
				if(PhotonNetwork.isMasterClient){
					if(gameStarted && gameTime == gameTimeLimit || gameTime > gameTimeLimit){
						endMatch();
					}
				}else{
					if(!gameStarted){
						if(GameObject.FindWithTag ("localPlayer") != null && GameObject.FindWithTag ("localPlayer").GetPhotonView().isMine){
							PhotonNetwork.Destroy (GameObject.FindWithTag ("localPlayer"));
						}
					}
				}
			}

		}

		if (KillMessages.Count > 0 && Time.time > lastKMSG + killInterval) {
			KillMessages.RemoveAt (KillMessages.Count-1);
			lastKMSG = Time.time;
		}
	}

	private string Map = "DevTest";
	private string nmbrPartida = "Game";
	private int ExpectedPlayers = 2;
	private string LocalIP = "192.168.0.0";
	private int LocalPort = 5055;

	public Texture2D TextContainer = null;
	public GUIStyle InfoText = null;
	public GUISkin GameStyle = null;
	public GUISkin emptySkin = null;
	public float killInterval = 1.0f;
	public float infoSpace = 1.0f;
	public Vector2 Info = Vector2.zero;
	public Vector2 InfoSize  = Vector2.zero;
	public Vector2 TextPos = Vector2.zero;
	public Vector2 TextSize = Vector2.zero;
	private float lastKMSG = -10.0f;
	public Vector2 test = Vector2.zero;
	public Vector2 test2 = Vector2.zero;
	void OnGUI(){

		GUI.skin = GameStyle;

		if (Application.loadedLevel == 1 || Application.loadedLevel == 0)
						return;

		GUI.color = Color.white;
		if(!PhotonNetwork.inRoom){//Dentro de lobby
			switch (MenuState){
			//SI EL MENU ES EL PRINCIPAL
			case gameMenu.Main:
				GUI.color = Color.black;
				GUIStyle button = GUIStyle.none;
				button = emptySkin.box;
				button.fontSize = Mathf.RoundToInt (Screen.height*0.01f+Screen.width*0.05f);
				if(GUI.Button (new Rect(Screen.width/1.5f-50.0f, Screen.height/2.5f-75, Screen.width*0.35f, Screen.width*-0.01f - Screen.height*-0.15f), "Search", button)){
					MenuState = gameMenu.Lobby;
				}
				if(GUI.Button (new Rect(Screen.width/1.5f-50.0f, Screen.height/2.5f, Screen.width*0.35f, Screen.width*-0.01f - Screen.height*-0.145f), "Create", button)){
					MenuState = gameMenu.Game;
				}
				if(GUI.Button (new Rect(Screen.width/1.5f-50.0f, Screen.height/2.5f+75.0f, Screen.width*0.35f, Screen.width*-0.01f - Screen.height*-0.15f), "Options", button)){
					MenuState = gameMenu.Settings;
				}
				if(GUI.Button (new Rect(Screen.width/1.5f-50.0f, Screen.height/2.5f+150.0f, Screen.width*0.35f, Screen.width*-0.01f - Screen.height*-0.15f), "Credits", button)){
					MenuState = gameMenu.Credits;
				}
				GUI.color = Color.white;
			break;//FIN DE MENU PRNCIPAL
		
			//SI EL MENU ES EL LOBBY
			case gameMenu.Lobby:

			GUILayout.BeginArea(new Rect(0, 0, Screen.width*lobbySize.x, Screen.height*lobbySize.y));
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("| Game list |");
			GUILayout.Space (2.5f);

			GUI.color = new Color(0.25f, 0.25f, 0.25f, 0.55f);
			GUILayout.BeginHorizontal ("box");
			GUI.color = Color.white;

			if (PhotonNetwork.PhotonServerSettings.ToString().Contains ("OfflineMode")){
				GUILayout.Label ("Mode: ");
				if(GUILayout.Button ("Online")){
					PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
				}
				if(GUILayout.Button ("LAN")){
					PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.SelfHosted;
				}
			
			}else if(PhotonNetwork.PhotonServerSettings.ToString().Contains ("PhotonCloud")){
				GUILayout.Label ("Continent: ");
				if(GUILayout.Button ("EU")){
					PhotonNetwork.PhotonServerSettings.PreferredRegion = CloudRegionCode.eu;
				}

				if(GUILayout.Button ("US")){
					PhotonNetwork.PhotonServerSettings.PreferredRegion = CloudRegionCode.us;
				}

				if(GUILayout.Button ("Asia")){
					PhotonNetwork.PhotonServerSettings.PreferredRegion = CloudRegionCode.asia;
				}

				if(GUILayout.Button ("JP")){
					PhotonNetwork.PhotonServerSettings.PreferredRegion = CloudRegionCode.jp;
				}
				GUILayout.Label (" | Modes: ");
				if(GUILayout.Button ("LAN")){
					PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.SelfHosted;
				}
				if(GUILayout.Button ("Offline")){
					PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.OfflineMode;
				}				
				GUILayout.Label (" | Protocols: ");
				if(GUILayout.Button ("UDP")){
					PhotonNetwork.PhotonServerSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
				}
				if(GUILayout.Button("TCP")){
					PhotonNetwork.PhotonServerSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Tcp;
				}
			}else if(PhotonNetwork.PhotonServerSettings.ToString().Contains ("SelfHosted")){
				GUILayout.Label ("Configurations: ");
				LocalIP = GUILayout.TextField (LocalIP);
				LocalPort = int.Parse (GUILayout.TextField(LocalPort.ToString()));
				if(GUILayout.Button("Connect")){
					PhotonNetwork.PhotonServerSettings.ServerAddress = LocalIP;
					PhotonNetwork.PhotonServerSettings.ServerPort = LocalPort;
				}
				GUILayout.Label (" | Modes: ");
				if(GUILayout.Button ("Online")){
					PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
				}
				if(GUILayout.Button ("Offline")){
					PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.OfflineMode;
				}
				GUILayout.Label (" | Protocol: ");
				if(GUILayout.Button ("UDP")){
					PhotonNetwork.PhotonServerSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
				}
				if(GUILayout.Button("TCP")){
					PhotonNetwork.PhotonServerSettings.Protocol = ExitGames.Client.Photon.ConnectionProtocol.Tcp;
				}
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			GUILayout.EndHorizontal ();

			GUI.color = new Color(0.25f, 0.25f, 0.25f, 0.55f);
			GUILayout.BeginVertical("box");
			GUI.color = Color.white;

		if(PhotonNetwork.connectedAndReady){

			if(RoomList != null){
				
				foreach (RoomInfo Room in RoomList){
						if(Room.open && Application.CanStreamedLevelBeLoaded(Room.customProperties["map"].ToString())){
						GUILayout.BeginHorizontal ("box");
						if(Room.isLocalClientInside){
								GUI.color = Color.green;
								GUILayout.Label (Room.name);
								GUI.color = Color.white;
						}else{
							GUILayout.Label (Room.name);
						}
						GUILayout.Space (7);
							GUILayout.Label (Room.playerCount + " / " + Room.maxPlayers);
						GUILayout.Space (7);
							GUILayout.Label ("Map: " + Room.customProperties["map"]);
						GUILayout.Space (7);
							GUILayout.Label ("Game mode: " + Room.customProperties["mode"].ToString ());
						GUILayout.Space (7);

							if(GUILayout.Button ("Join")){
								PlayerPrefs.SetString ("name", PlayerName);
								PhotonNetwork.LoadLevel (Room.customProperties["map"].ToString());
								PhotonNetwork.JoinRoom (Room.name);
							}
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal ();
					}
				}
			}
		}else{
			GUILayout.Label ("Sincronizando con el servidor, aguanta vara...");
		}
				GUILayout.FlexibleSpace();
				if(GUILayout.Button("Back")){
					MenuState = gameMenu.Main;
				}
				GUILayout.EndVertical ();
			GUILayout.EndVertical ();
		GUILayout.EndArea ();
				break; //FIN DE MENU LOBBY


		//Menu crear juego
		case gameMenu.Game:

		GUILayout.BeginArea(new Rect(Screen.width/2.0f, 0.0f, Screen.width/2.0f, Screen.height));
		GUILayout.BeginVertical("box");//Caja de opciones

			GUI.color = new Color(0.25f, 0.25f, 0.25f, 0.55f);
			GUILayout.BeginVertical("box");//Contenido de cada menu...
			GUI.color = Color.white;

		if(PhotonNetwork.connectedAndReady){//SI ESTAMOS CONECTADOS A LA NUBE Y ESTAMOS LISTOS
					GUI.color = new Color(0.25f, 0.25f, 0.25f, 0.35f);
				GUILayout.BeginHorizontal ("box");
					GUI.color = Color.white;

					if(Application.CanStreamedLevelBeLoaded (Map)){
						GUILayout.Label ("Create game with name: ");
							GUILayout.Space (7);
						nmbrPartida = GUILayout.TextField (nmbrPartida, 13);
							GUILayout.Space (7);
						GUILayout.Label ("Map: " + Map);
							GUILayout.Space (7);
					}else{
						GUILayout.Label ("Loading... " + Mathf.Round (Application.GetStreamProgressForLevel (Map) * 100) + "%", "box");
					}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
					if(GUILayout.Button ("<")){
						GameMode = GameModes.DeathMatch;
					}
					GUILayout.Label ("| Game mode: " + GameMode.ToString () + " |");
					if(GUILayout.Button (">")){
						GameMode = GameModes.TeamDeathMatch;
					}
					GUILayout.FlexibleSpace ();

				GUILayout.EndHorizontal ();
					GUILayout.Label ("Select map:");
					GUI.color = new Color(0.25f, 0.25f, 0.25f, 0.55f);
				GUILayout.BeginVertical("box");//SELECCION DE MAPAS
					GUI.color = Color.white;

					mapScroll = GUILayout.BeginScrollView(mapScroll);
					foreach(string selection in Maps){
						if(GUILayout.Button(selection)){
							Map = selection;
						}
					}
					GUILayout.EndScrollView ();
				GUILayout.EndVertical();
			
			}else{
			GUILayout.Label ("Sincronizando con el servidor de mierda...");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical ();
		GUILayout.FlexibleSpace();
				if(GUILayout.Button ("Create")){
					PlayerPrefs.SetString ("name", PlayerName);
					PlayerPrefs.SetString ("Rname", nmbrPartida);
					//Hacer que ignore obsolete warning
					#pragma warning disable 
					PhotonNetwork.CreateRoom (nmbrPartida, true, true, 8, new ExitGames.Client.Photon.Hashtable(){{"map", Map}, {"mode", GameMode.ToString()}}, new string[]{"map", "mode"});
					PhotonNetwork.LoadLevel (Map);
				}
				if(GUILayout.Button ("Back")){
					MenuState = gameMenu.Main;
				}
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
				break;//FIN DE MENU DE GAME CREATION

			//MENU DE SETTINGS
			case gameMenu.Settings:

			GUILayout.BeginArea(new Rect(Screen.width*msgPos.x, Screen.height*msgPos.y, Screen.width*msgSize.x, Screen.height*msgSize.y));
					GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.85f);
			GUILayout.BeginVertical("box");
					GUI.color = Color.white;
			GUILayout.Label ("| Info messages |");
					GUI.color = new Color(0.25f, 0.25f, 0.25f, 0.55f);
				GUILayout.BeginVertical("box");
					GUI.color = Color.white;
				scrollMSGs = GUILayout.BeginScrollView(scrollMSGs);
			if(PhotonNetwork.connectedAndReady){

				foreach(string msg in Messages){
					if(msg.Contains("{")){
						GUI.color = Color.green;
						GUILayout.Label (msg);
						GUI.color = Color.white;
					}
				}

			}else{
				GUILayout.Label ("Synchronizing with server...");
			}
				GUILayout.FlexibleSpace();
				GUILayout.EndScrollView ();
				GUILayout.EndVertical ();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical ();
			GUILayout.EndArea ();

			GUILayout.BeginArea (new Rect(Screen.width/2, 0.0f, Screen.width/2, Screen.height));
				GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.85f);
			GUILayout.BeginVertical ("box");
				GUI.color = Color.white;
			settingScroll = GUILayout.BeginScrollView (settingScroll);
					GUILayout.BeginHorizontal ();

				GUILayout.Label ("Player name: ");
				PlayerName = GUILayout.TextField (PlayerName, 13);
					GUILayout.EndHorizontal ();

				GUILayout.Label ("Camera adjust:");
				CAdjust = GUILayout.HorizontalSlider(CAdjust, 0.3f, 1.0f);
				GUILayout.Label ("Visual details:");
				VDetails = (int)GUILayout.HorizontalSlider(VDetails, 0, 2);
				GUILayout.Label ("Sound volume:");
				SVolume = GUILayout.HorizontalSlider(SVolume, 0.0f, 1.0f);
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace ();
				if(GUILayout.Button ("Save")){
					PlayerPrefs.SetString ("name", PlayerName);
					PlayerPrefs.SetFloat ("CAdjust", CAdjust);
					PlayerPrefs.SetInt ("VDetails", VDetails);
					PlayerPrefs.SetFloat ("SVolume", SVolume);
					QualitySettings.SetQualityLevel (VDetails);
					AudioListener.volume = SVolume;
				}
				if(GUILayout.Button ("Back")){
					MenuState = gameMenu.Main;
				}
			GUILayout.EndVertical ();
			GUILayout.EndArea ();


				break;//FIN DE SWITCH InROOM y MENU SETTINGS
			
			case gameMenu.Credits://MENU CREDITOS

				GUILayout.BeginArea(new Rect(Screen.width/2, 0.0f, Screen.width/2, Screen.height));
					GUI.color = new Color(0.35f, 0.35f, 0.35f, 0.8f);
				GUILayout.BeginVertical ("box");
					GUI.color = Color.white;
				GUILayout.Label ("Credits:");
				GUILayout.Space(2);
				foreach(string credito in creditos){
					GUILayout.Label (credito);
					GUILayout.Space(2);
				}
				GUILayout.FlexibleSpace ();
				if(GUILayout.Button("Back")){
					MenuState = gameMenu.Main;
				}
				GUILayout.EndArea();
				
			break;//FIN CREDITOS
			}//FIN DE SWITCH(){}

		}else{//En un juego
		if(!waitingSpawn){//Espereando spawneo

			if(GameObject.FindWithTag ("localPlayer") == null){
					GUILayout.BeginArea(new Rect(Screen.width*creationPos.x, Screen.height*creationPos.y, Screen.width*creationSize.x, Screen.height*creationSize.y));
							GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
						GUILayout.BeginVertical("box");
							GUI.color = Color.white;		

					GUILayout.Label ("| Game menu |");
							GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.65f);
						GUILayout.BeginVertical("box");
							GUI.color = Color.white;
				
				
				//Si no soy el hoster...
				if(gameStarted){
					//Si no tiene equipo:
					if(PhotonNetwork.room.customProperties["mode"].ToString() != "DeathMatch" && PhotonNetwork.player.GetTeam () == PunTeams.Team.none){
								GUI.color = Color.cyan;
							if(GUILayout.Button ("Join blue team")){
								PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
							}
								GUI.color = Color.red;
							if(GUILayout.Button ("Join red team")){
								PhotonNetwork.player.SetTeam (PunTeams.Team.red);
							}
								GUI.color = Color.white;
					}else{//Si tiene uno:
						if(GUILayout.Button("Join the game")){
							Camera.main.gameObject.SendMessage("ClearTarget", SendMessageOptions.RequireReceiver);
							SpawnPlayer();
						}
					}
				}else{//Esperar a que el master decida el inicio...
					if(!PhotonNetwork.isMasterClient){
						GUILayout.Label ("Wait until host starts the match...");
					}else{
						if(GUILayout.Button ("Start match")){
							gameStarted = true;
							gameTime = 0.0f;
							photonView.RPC ("setMatchInfo", PhotonTargets.Others, gameTime, gameTimeLimit, gameStarted);
						}

						GUILayout.BeginHorizontal ();
							GUILayout.Label ("Set time limit:");
							gameTimeLimit = Mathf.Clamp (int.Parse ((GUILayout.TextField ( gameTimeLimit.ToString() ))), 1.0f, 50.0f);
						GUILayout.EndHorizontal ();	
						
					}
				}
					
				if(GUILayout.Button("Leave")){
					chatting = false;
					MenuState = gameMenu.Main;
					PhotonNetwork.LeaveRoom ();
				}
				
				if(PhotonNetwork.isMasterClient && gameStarted){//Opciones de cambio admin
							//GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.45f);
						//featureScroll = GUILayout.BeginScrollView (featureScroll ,"box");
							//GUI.color = Color.white;


						/* SELECCION DE MODO DE JUEGO Y MAPA NUEVO
							GUILayout.BeginHorizontal ();
						if(GUILayout.Button ("<")){
							GameMode = GameModes.DeathMatch;
							PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable{{"mode", GameMode.ToString ()}});
						}
						GUILayout.Label ("Game mode: " + GameMode.ToString ());
						if(GUILayout.Button (">")){
							GameMode = GameModes.TeamDeathMatch;
							PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable{{"mode", GameMode.ToString ()}});
						}
							GUILayout.EndHorizontal ();	
						GUILayout.FlexibleSpace();
						GUILayout.Label ("Select new map, match will restart...");
									GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.45f);
								mapScroll = GUILayout.BeginScrollView (mapScroll, "box");
									GUI.color = Color.white;
							foreach(string selection in Maps){
								if(Application.loadedLevelName != selection && Application.CanStreamedLevelBeLoaded (selection) && GUILayout.Button(selection)){
									Map = selection;
									photonView.RPC ("changeMap", PhotonTargets.All, Map);
								}
							}
								GUILayout.EndScrollView ();
					GUILayout.EndScrollView ();
						*/
				}

					GUILayout.FlexibleSpace();
				GUILayout.EndVertical ();
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical ();
				GUILayout.EndArea ();
			}else{//Jugador spawneado y jugando!
				if(Input.GetButton ("Scoreboard")){
						//Si es deathMatch
					if(PhotonNetwork.room.customProperties["mode"].ToString() == "DeathMatch"){

						GUILayout.BeginArea(new Rect(0, 0, Screen.width*lobbySize.x, Screen.height*lobbySize.y));
						
								GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.45f);
							GUILayout.BeginVertical("box");
								GUI.color = Color.white;	

						GUILayout.Label ("|| Score board |" + "| Game name: '" + PhotonNetwork.room.name +"' | Players: " + PhotonNetwork.room.playerCount + " / " + PhotonNetwork.room.maxPlayers + " | Map: " + PhotonNetwork.room.customProperties["map"] + " | Mode: " + PhotonNetwork.room.customProperties["mode"] + " ||");

							GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.55f);
						GUILayout.BeginVertical("box");
							GUI.color = Color.white;
						//LISTA DE JUGADORES, PUNTAJES
						if(playerList.Length > 0){
							foreach (PhotonPlayer player in playerList){
									if(player == PhotonNetwork.player){
										GUI.color = new Color(0.5f, 0.5f, 1.0f, 0.55f);
									}else{
										GUI.color = new Color(1.0f, 0.5f, 0.5f, 0.55f);
									}
								GUILayout.BeginHorizontal ("box");
									GUI.color = Color.white;

									GUILayout.Label ("Name: " + player.name + " | Kills: " + player.customProperties["kills"] + " | Deaths: " + player.customProperties["deaths"] + " | Asists: " + player.customProperties["assist"] + " | Score: " + player.GetScore ());
								if(PhotonNetwork.isMasterClient && player != PhotonNetwork.player && GUILayout.Button ("Kick")){
									PhotonNetwork.CloseConnection (player);
								}
										GUILayout.FlexibleSpace();
										GUILayout.EndHorizontal ();
							}
						}		
						GUILayout.FlexibleSpace();
						GUILayout.EndVertical ();
						GUILayout.FlexibleSpace();
						GUILayout.EndVertical ();
						GUILayout.EndArea ();				
					}else{//Si no es DeathMatch y es TEAMDeathMatch
								//PANEL EQUIPO AZUL
							GUILayout.BeginArea(new Rect(0, 0, Screen.width*lobbySize.x, Screen.height*lobbySize.y/2));
								
								GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.45f);
							GUILayout.BeginVertical("box");
								GUI.color = Color.white;

							GUILayout.Label ("|| Score board |" + "| Game name: '" + PhotonNetwork.room.name +"' | Players: " + PhotonNetwork.room.playerCount + " / " + PhotonNetwork.room.maxPlayers + " | Map: " + PhotonNetwork.room.customProperties["map"].ToString() + " | Mode: " + PhotonNetwork.room.customProperties["mode"].ToString() + " ||");
							
							if(GUILayout.Button ("Swap team")){//Cuando se oprime swap
								if(GameObject.FindWithTag ("localPlayer") != null){//Si el jugador
									GameObject Player = GameObject.FindWithTag ("localPlayer");
									PhotonView PlayerP = GameObject.FindWithTag ("localPlayer").GetPhotonView ();
									if(PhotonNetwork.player.GetTeam () == PunTeams.Team.blue){
										PhotonNetwork.player.SetTeam (PunTeams.Team.red);
									}else{
										PhotonNetwork.player.SetTeam (PunTeams.Team.blue);
									}
									if(PlayerP.isMine){
										PlayerP.RPC ("Damage", PlayerP.owner, 100000.0f, PhotonNetwork.player);
									}
								}
							}
							
								GUI.color = new Color(0.2f, 0.2f, 1.0f, 0.55f);
							GUILayout.BeginVertical("box");
								GUI.color = Color.white;
							//LISTA DE JUGADORES, PUNTAJES
							if(playerList.Length > 0){
								foreach (PhotonPlayer player in playerListA){
										if(player.GetTeam () == PunTeams.Team.blue){
													GUI.color = new Color(0.65f, 0.65f, 1.0f, 0.55f);
												GUILayout.BeginHorizontal ("box");
													GUI.color = Color.white;
										
											GUILayout.Label ("Name: " + player.name + " | Kills: " + player.customProperties["kills"] + " | Deaths: " + player.customProperties["deaths"] + " | Asists: " + player.customProperties["assist"] + "| Score: " + player.GetScore ());
												if(PhotonNetwork.isMasterClient && player != PhotonNetwork.player && GUILayout.Button ("Kick")){
													PhotonNetwork.CloseConnection (player);
												}
											GUILayout.FlexibleSpace();
											GUILayout.EndHorizontal ();
										}
								}
							}		
							GUILayout.FlexibleSpace();
							GUILayout.EndVertical ();
							GUILayout.FlexibleSpace();
							GUILayout.EndVertical ();
							GUILayout.EndArea ();
							///////////////////////////
							//Panel equipo ROJO

							GUILayout.BeginArea(new Rect(0, Screen.height*lobbySize.y/2, Screen.width*lobbySize.x, Screen.height*lobbySize.y/2));
								GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.45f);
							GUILayout.BeginVertical("box");
								GUI.color = Color.white;

								GUI.color = new Color(1.0f, 0.2f, 0.2f, 0.55f);
							GUILayout.BeginVertical("box");
								GUI.color = Color.white;
							//LISTA DE JUGADORES, PUNTAJES
							if(playerList.Length > 0){
								foreach (PhotonPlayer player in playerListB){
									if(player.GetTeam () == PunTeams.Team.red){
										GUI.color = new Color(1.0f, 0.65f, 0.65f, 0.55f);
										GUILayout.BeginHorizontal ("box");
										GUI.color = Color.white;
										
										GUILayout.Label ("Name: " + player.name + " | Kills: " + player.customProperties["kills"] + " | Deaths: " + player.customProperties["deaths"] + " | Asists: " + player.customProperties["assist"] + "| Score: " + player.GetScore ());
										if(PhotonNetwork.isMasterClient && player != PhotonNetwork.player && GUILayout.Button ("Kick")){
											PhotonNetwork.CloseConnection (player);
										}
										GUILayout.FlexibleSpace();
										GUILayout.EndHorizontal ();
									}
								}
							}		
							GUILayout.FlexibleSpace();
							GUILayout.EndVertical ();
							GUILayout.FlexibleSpace();
							GUILayout.EndVertical ();
							GUILayout.EndArea ();

					}//Fin de condicion else ifNot DM
				}
				
					if(Input.GetButtonUp ("Chat") && chatting == false && Time.time > lastChat + 0.5f){
						scrollMSGs.y = 999999.0f;
						chatting = true;
						lastChat = Time.time;
				}

				if(chatting == false){
					if(Time.time < lastMSG + 7.25f){
						//Chat
							GUILayout.BeginArea(new Rect(Screen.width*msgPos.x, Screen.height*msgPos.y, Screen.width*msgSize.x, Screen.height*msgSize.y));
								GUI.color = new Color(1f, 1f, 1f, 0.2f);
							GUILayout.BeginVertical("box");
							GUILayout.BeginHorizontal ();
						GUILayout.Label ("| Chat messages | Match time: " + float.Parse (gameTime.ToString ("0.00")) + " |");
						if(GUILayout.Button("Top")){
							scrollMSGs.y = 0.0f;
						}
						if(GUILayout.Button("Bottom")){
							scrollMSGs.y = 9999999999.0f;
						}
							GUILayout.FlexibleSpace ();
							GUILayout.EndHorizontal ();
							GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
						GUILayout.BeginVertical("box");
						scrollMSGs = GUILayout.BeginScrollView(scrollMSGs);
							//[] = Muerte | {} = Mensaje de servidor
							foreach(string msg in Messages){
								if(msg.Contains("{")){
									GUI.color = Color.green;
									GUILayout.Label (msg);
									GUI.color = Color.white;
								}else{
									GUILayout.Label (msg);
								}
							}

						GUILayout.FlexibleSpace();
						GUILayout.EndScrollView ();
						GUILayout.EndVertical ();
						GUILayout.FlexibleSpace();
						GUILayout.EndVertical ();
						GUILayout.EndArea ();
				}
			}else{
						//Chat
						GUILayout.BeginArea(new Rect(Screen.width*msgPos.x, Screen.height*msgPos.y, Screen.width*msgSize.x, Screen.height*msgSize.y));
							GUI.color = new Color(1f, 1f, 1f, 0.45f);
						GUILayout.BeginVertical("box");
							GUI.color = Color.white;
						GUILayout.BeginHorizontal ();

						GUILayout.Label ("| Chat messages | Match time: " + float.Parse (gameTime.ToString ("0.00")) + " |");					
						if(GUILayout.Button("Top")){
							scrollMSGs.y = 0.0f;
						}
						if(GUILayout.Button("Bottom")){
							scrollMSGs.y = 9999999999.0f;
						}
						if(GUILayout.Button("Leave")){
							chatting = false;
							MenuState = gameMenu.Main;
							PhotonNetwork.LeaveRoom();
						}
						GUILayout.FlexibleSpace ();
						GUILayout.EndHorizontal ();
							GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.55f);
						GUILayout.BeginVertical("box");
							GUI.color = Color.white;
						scrollMSGs = GUILayout.BeginScrollView(scrollMSGs);
						
						foreach(string msg in Messages){
							if(msg.Contains("{")){
								GUI.color = Color.green;
								GUILayout.Label (msg);
								GUI.color = Color.white;
							}else{
								GUILayout.Label (msg);
							}
						}	
						if(Time.time > lastChat + 0.125f){
							if(Input.GetKeyDown("enter") || Event.current.keyCode == KeyCode.Return){
								chatField = FilterString(chatField);
								if(chatField != string.Empty){
									photonView.RPC ("ReceiveMSG", PhotonTargets.AllBufferedViaServer, PhotonNetwork.playerName, chatField);
									chatField = string.Empty;
									chatting = false;
									lastChat = Time.time;
							}else{
									chatField = string.Empty;
									chatting = false;
									lastChat = Time.time;
								}
							}
						}
						GUILayout.FlexibleSpace();
						GUILayout.EndScrollView ();
						GUILayout.EndVertical ();
						GUILayout.FlexibleSpace();
						GUI.SetNextControlName("ChatField");
						chatField = GUILayout.TextField (chatField);
						GUI.FocusControl ("ChatField");
						GUILayout.EndVertical ();
						GUILayout.EndArea ();
			}
			}
			//SISTEMA DE NOTIFICACIONES
				InfoText.fontSize = (int)(Screen.width*TextSize.x/2);

			if(KillMessages.Count > 0){//Si no esta vacio
					float movement = (float)(Time.time - lastKMSG)-(Time.deltaTime*2);
					float TextX = 0.0f;
					float TexX = 0.0f;
					int num = 0;
				for(int i=KillMessages.Count-1; i>-1; i--){
					if(num == 3){
						num = 0;
						break;
					}
					if(num==0){
						TextX = Mathf.Lerp (Screen.width, Screen.width-TextPos.x, movement);
						TexX = Mathf.Lerp (Screen.width, Screen.width-Info.x, movement);
					}else{
						TextX = Screen.width-TextPos.x;
						TexX = Screen.width-Info.x;
					}

					GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
					GUI.DrawTexture (new Rect(TexX, Screen.height*Info.y+num*infoSpace, Screen.width*InfoSize.x, Screen.height*InfoSize.y), TextContainer, ScaleMode.StretchToFill, true); 
					GUI.color = getTextColor(KillMessages[i].ToString ());
					GUI.Label (new Rect(TextX, Screen.height*TextPos.y+num*infoSpace, Screen.width*TextSize.x, TextSize.y), KillMessages[i].ToString(), InfoText);
					GUI.color = Color.white;
						num += 1;
				}
			}
		}else{
				GUI.Label (new Rect(Screen.width/2, Screen.height/2, Screen.width, Screen.height), "Searching for a safe place..." + countdownTimer);
				if(GUI.Button (new Rect(Screen.width/2, Screen.height/2+20, 55, 30),"Spawn")){
					countdownTimer = 1;
				}
		}
		}

	}

	private bool waitingSpawn = false;
	private int countdownTimer = 10;
	void SpawnPlayer(){
		Transform[] Spawn = GameObject.FindWithTag ("SpawnContainer").GetComponent <SpawnContainer>().getSpawn();
		EnemyCheckRadious = GameObject.FindWithTag ("SpawnContainer").GetComponent <SpawnContainer> ().Radius;
		waitingSpawn = true;
		bool found = false;
		Transform spawnpoint = Spawn[Random.Range(0,Spawn.Length)];
		//i is asigned but not used
		#pragma warning disable 0219
			foreach(Transform i in Spawn){
			 Transform t = Spawn[Random.Range (0, Spawn.Length)];
				bool enemyInside = false;
			Collider[] hits = Physics.OverlapSphere(t.position ,EnemyCheckRadious);
				foreach(Collider hit in hits){
					if(hit.tag == "Player"){
						enemyInside = true;
						break;
					}else{
						continue;
					}
				}
				if(!enemyInside){
					spawnpoint = t;
					found = true;
					waitingSpawn = false;
					break;
				}else{
					continue;
				}
			}
		if(found == true || countdownTimer == 0){
			PhotonNetwork.Instantiate (PrefabJugador, spawnpoint.position, spawnpoint.rotation, 0);
			countdownTimer = 10;
			lastKilled = 0;
			waitingSpawn = false;
		}else{
			StartCoroutine(WaitSpawn());
		}
	}

	string FilterString(string old){
		string newS = string.Empty;
		foreach (char i in old) {
			if(i != char.Parse ("{") && i != char.Parse ("(") && i != char.Parse ("[") && i != char.Parse ("]") && i != char.Parse (")") && i != char.Parse ("}")){
				newS = newS + i.ToString ();
			}
		}
		return newS;
	}

	IEnumerator WaitSpawn(){
		yield return new WaitForSeconds(1.0f);
		countdownTimer -= 1;
		SpawnPlayer();
	}

	//EVENTOS
	private Vector2 scrollMSGs = Vector2.zero;
	private float lastMSG = -10.0f;
	/*RPC CAMBIO DE MAPA
	[RPC]
	void changeMap(string newMap){
		if(GameObject.FindWithTag ("localPlayer") != null && GameObject.FindWithTag ("localPlayer").GetPhotonView().isMine){
			PhotonNetwork.Destroy (GameObject.FindWithTag ("localPlayer"));
		}
		gameTime = 0.0f;
		gameStarted = false;
		PhotonNetwork.player.SetTeam (PunTeams.Team.none);
		PhotonNetwork.player.SetScore (0);
		PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { {"kills", 0}, {"deaths", 0}, {"assist", 0} });

		PhotonNetwork.LoadLevel (newMap);
	}
	*/
	[RPC]
	void ReceiveMSG(string sender, string MSG){
		Messages.Add (sender + ": " + MSG);
		scrollMSGs.y = 999999.0f;
		lastMSG = Time.time;
	}

	[RPC]
	void MSGServer(string MSG){
		if (!MSG.Contains ("[") && !MSG.Contains ("(")) {
			Messages.Add (MSG);
			lastMSG = Time.time;
		} else {
			KillMessages.Add (MSG);
			lastKMSG = Time.time;
		}
		scrollMSGs.y = 999999.0f;
	} 

	private int lastKilled = 0;

	[RPC]
	void KilledSomeone(string killed){
		lastKilled += 1;
		if (lastKilled > 4) {
			EventSND.clip = MTanzasSNDs [Random.Range (0, MTanzasSNDs.Length)];
			EventSND.Play ();
			photonView.RPC ("MSGServer", PhotonTargets.All, "(" + PhotonNetwork.playerName + " esta haciendo una MaTaNzA!" + ")");
		} 

		photonView.RPC ("MSGServer", PhotonTargets.All, "["+PhotonNetwork.playerName + " killed " + killed+"]");
		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable() {{"kills", int.Parse(PhotonNetwork.player.customProperties["kills"].ToString ())+1}});
		PhotonNetwork.player.SetScore (PhotonNetwork.player.GetScore ()+10);
		scrollMSGs.y = 999999.0f;
		lastMSG = Time.time;
	}

	[RPC]
	void AsistSomeone(string killed){
		EventSND.clip = AsistSND;
		EventSND.Play ();
		photonView.RPC ("MSGServer", PhotonTargets.All, "("+ PhotonNetwork.playerName + " assisted on killing " + killed+")");
		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable() {{"assist", int.Parse(PhotonNetwork.player.customProperties["assist"].ToString ())+1}});
		PhotonNetwork.player.SetScore (PhotonNetwork.player.GetScore ()+5);
		scrollMSGs.y = 999999.0f;
		lastMSG = Time.time;
	}

	[RPC]
	void restartMatch(){
		gameTime = 0.0f;
		gameStarted = false;
		PhotonNetwork.player.SetTeam (PunTeams.Team.none);
		PhotonNetwork.player.SetScore (0);
		PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { {"kills", 0}, {"deaths", 0}, {"assist", 0} });
		if(GameObject.FindWithTag ("localPlayer") != null && GameObject.FindWithTag ("localPlayer").GetPhotonView().isMine){
			PhotonNetwork.Destroy (GameObject.FindWithTag ("localPlayer"));
		}
	}

	[RPC]
	void setMatchInfo(float gameTimer, float gameTLimit, bool gameStart){
		gameTime = gameTimer;
		gameTimeLimit = gameTLimit;
		gameStarted = gameStart;

	}

	void endMatch(){
		string winner = getWinner ();
		photonView.RPC ("MSGServer", PhotonTargets.All, "["+ winner + " HAVE WON THIS MATCH!"+"]");
		photonView.RPC ("MSGServer", PhotonTargets.All, "{"+ winner + " HAVE WON THIS MATCH!"+"}");
		photonView.RPC ("restartMatch", PhotonTargets.All);
		gameStarted = false;
		gameTime = 0.0f;
	}

	string getWinner(){//HACER EL SORT DE JUEGO PLAYERLIST
		//Si es deathMatch
		if (PhotonNetwork.room.customProperties ["mode"].ToString () == "DeathMatch") {
			Hashtable scores = new Hashtable();

			foreach(PhotonPlayer player in playerList){//Calculo de puntaje de jugador por jugador
				scores.Add (player.name , player.GetScore ());
			}
			//Tomamos los nombres y los ponemos en una lista
			//Hacemos otra nueva con los puntajes
			ArrayList arrayScores = new ArrayList();
			Hashtable newScores = new Hashtable();
			//Ponemos los puntajes en una lista nueva de uno por uno
			//Y los nombres en la otra de uno por uno, con los puntajes.
			foreach (string key in scores.Keys) {
				arrayScores.Add (scores[key]);
				if(!newScores.ContainsKey (scores[key])){//Si contiene el puntaje, ignorar repetidos
					newScores.Add(scores[key], key);	
				}
			}
			//Acomodamos la lista
			arrayScores.Sort ();
			//Hacemos una lista nueva que contenga los nombres
			ArrayList arrayNames = new ArrayList();
			foreach (int scoreInt in arrayScores){
				arrayNames.Add( newScores[scoreInt]);
			}

			return arrayNames[arrayNames.Count - 1].ToString ();
		} else {//Si es por equipos
			int scoresA = 0;
			int scoresB = 0;
			foreach(PhotonPlayer playerA in playerListA){
				scoresA += playerA.GetScore ();
			}

			foreach(PhotonPlayer playerB in playerListB){
				scoresB += playerB.GetScore ();
			}
			//Si el azul tuvo mas puntaje...
			if(scoresA != scoresB){
				if(scoresA > scoresB){
					return "BLUE TEAM";
				}else{//Si el rojo...
					return "RED TEAM";
				}
			}else{
				return "BOTH TEAMS";
			}
		}
		return "ERROR 404 player not found...";
	}

	void getPlayerList(){//Obtener lista de jugadores 
		//Si el juego es free for all:
		if(PhotonNetwork.room.customProperties["mode"].ToString () == "DeathMatch"){
			playerList = PhotonNetwork.playerList;
		}else{//Si el juego es por equipos:
			playerList = PhotonNetwork.playerList;

			ArrayList playersA = new ArrayList();
			ArrayList playersB = new ArrayList();

			foreach (PhotonPlayer player in playerList){
				//Si el jugador es del equipo azul:
				if(player.GetTeam () == PunTeams.Team.blue){
					playersA.Add (player);
				}else{
					playersB.Add (player);
				}
			}

			playerListA = (PhotonPlayer[])playersA.ToArray (typeof (PhotonPlayer));
			playerListB = (PhotonPlayer[])playersB.ToArray (typeof (PhotonPlayer));

		}
	}

	Color getTextColor(string msg){
		if(msg.Contains("[")){
			return Color.red;
		}else if(msg.Contains("(")){
			return Color.yellow;
		}else{
			return new Color(1.0f, 1.0f, 1.0f, 0.5f);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (!PhotonNetwork.inRoom)
						return;

		if(PhotonNetwork.isMasterClient){//Envio
			stream.SendNext (gameTime);
			stream.SendNext (gameTimeLimit);
			stream.SendNext (gameStarted);
		}else{//Recepcion y sync
			gameTime = (float)stream.ReceiveNext ();
			gameTimeLimit = (float)stream.ReceiveNext ();
			gameStarted = (bool)stream.ReceiveNext ();
		}
	}

	void OnJoinedLobby(){
		Messages.Add ("{You got into lobby...}");
		scrollMSGs.y = 999999.0f;
	} 

	void OnConnectedToMaster(){
		Messages.Add ("{Connected to master...}");
		scrollMSGs.y = 999999.0f;
	}
	
	void OnFailedToConnectToPhoton(){
		Messages.Add ("{Failed on connect to cloud...}");
		scrollMSGs.y = 999999.0f;
	}

	void OnDisconnectedFromPhoton(){
		Messages.Add ("{Disconnected from cloud...}");
		scrollMSGs.y = 999999.0f;
	}

	void OnPhotonCreateRoomFailed(){
		Messages.Add ("{Failed on creating a game...}");
		PhotonNetwork.LoadLevel (MainMenu);
		scrollMSGs.y = 999999.0f;
	}

	void OnPhotonJoinRoomFailed(){
		Messages.Add ("{Couldnt join to game...}");
		PhotonNetwork.LoadLevel (MainMenu);
		scrollMSGs.y = 999999.0f;
	}

	void OnCreatedRoom(){
		Messages.Add ("{You created a game...}");
		gameTime = 0.0f;
		gameTimeLimit = 20.0f;
		gameStarted = false;
		PhotonNetwork.player.SetTeam (PunTeams.Team.none);
		PhotonNetwork.player.SetScore (0);
		PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { {"kills", 0}, {"deaths", 0}, {"assist", 0} });
		scrollMSGs.y = 999999.0f;
	}

	void OnLeftLobby(){
		Messages.Add ("{You got out of lobby...}");
		scrollMSGs.y = 999999.0f;
	}

	void OnConnectionFail(){
		Messages.Add ("{Connection failed!...}");
		scrollMSGs.y = 999999.0f;
	}

	void OnJoinedRoom(){
		Messages.Add ("{You joined a game...}");
		getPlayerList ();
		lastPlayer = Time.time;
		PhotonNetwork.player.SetTeam (PunTeams.Team.none);
		PhotonNetwork.player.SetScore (0);
		PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { {"kills", 0}, {"deaths", 0}, {"assist", 0}});
		scrollMSGs.y = 999999.0f;
		gameTime = 0.0f;
		gameTimeLimit = 20.0f;
		gameStarted = false;
	}

	void OnPhotonRandomJoinFailed(){
		Messages.Add ("{Couldnt join a random game...}");
		PhotonNetwork.LoadLevel (MainMenu);
		scrollMSGs.y = 999999.0f;
	}

	void OnPhotonMaxCccuReached(){
		Messages.Add ("{SERVER IS FULL, try again later...}");
		scrollMSGs.y = 999999.0f;
	}

	void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		if(PhotonNetwork.isMasterClient){
			photonView.RPC ("MSGServer", PhotonTargets.AllBufferedViaServer, "{"+newPlayer.name + " joined the game!"+"}");
			photonView.RPC ("setMatchInfo", newPlayer, gameTime, gameTimeLimit, gameStarted);
		}
		getPlayerList ();
		lastPlayer = Time.time;
		scrollMSGs.y = 999999.0f;
	}

	void OnPhotonPlayerDisconnected (PhotonPlayer otherPlayer){
		if(PhotonNetwork.isMasterClient){
			photonView.RPC ("MSGServer", PhotonTargets.AllBufferedViaServer, "{"+otherPlayer.name + " disconnected!"+"}");
			PhotonNetwork.RemoveRPCs (otherPlayer);
			PhotonNetwork.DestroyPlayerObjects (otherPlayer);
		}
		scrollMSGs.y = 999999.0f;
	}

	void OnLeftRoom (){
		Messages.Add ("{You leaved the game...}");
		PhotonNetwork.LoadLevel (MainMenu);
		scrollMSGs.y = 999999.0f;
	}
	

}
