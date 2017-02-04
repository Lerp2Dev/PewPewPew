using UnityEngine;
using System.Collections;

public class TextureMovement : MonoBehaviour {

	public string property = string.Empty;
	public Vector2 Dir = Vector2.zero;
	public float Speed = 0.5f;

	void Update(){
		renderer.material.SetTextureOffset (property, renderer.material.GetTextureOffset (property)+Dir * Time.deltaTime * Speed);
	}
}
