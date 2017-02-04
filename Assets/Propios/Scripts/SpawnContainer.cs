using UnityEngine;
using System.Collections;

public class SpawnContainer : MonoBehaviour {
	
	public Transform[] Spawn = null;
	public Transform[] SpawnTeamA = null;
	public Transform[] SpawnTeamB = null;
	public float MaxFall = -15.0f;

	public float MapHeight = 0.0f;
	public float MapCHeight = 0.0f;

	public float Radius = 20.0f;

	void Start(){
		Camera.main.SendMessage ("SetSettings", new Vector3 (MapHeight, MapCHeight, 0.0f), SendMessageOptions.RequireReceiver);
	}

	public Transform[] getSpawn(){
		if (PhotonNetwork.room.customProperties ["mode"].ToString () == "DeathMatch") {
			return Spawn;
		} else {
			if(PhotonNetwork.player.GetTeam () == PunTeams.Team.blue){
				return SpawnTeamA;
			}else{
				return SpawnTeamB;
			}
		}
	}

}
