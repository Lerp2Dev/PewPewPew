using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {
	public int id = 0;//Identificador
	public float ShootingInterval = 0.15f;//Intervalo de disparo.
	public float barrelDist = 2.0f;
	public Transform muzzle = null;//Punta del arma
	public Transform torso = null;//Transform del hueso torso.
	public Transform pelvis = null;//Transform del hueso pelvis.
	public Transform FlarePrefab = null;//Transform del destello.
	public Transform BulletPrefab = null;//Bala del arma.
	public Transform Modelo = null;//Modelo 3D
	public AudioClip ShootingClip = null;//Audio de disparo.
	public bool usesCamera = false;
	public Renderer Malla = null;
	#pragma warning disable 
	public Camera camera;
}
