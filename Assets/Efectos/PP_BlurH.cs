using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Aubergine/BlurH")]
public class PP_BlurH : PostProcessBase {
	//This value represents the blur multiplier
	public float blurMultiplier = 1f;
	public float speed = 0.2f;
	void Awake () {
		base.material.SetFloat("_BlurMulti", blurMultiplier);
	}
	void OnEnable () {
		base.shader = Shader.Find("Hidden/Aubergine/BlurH");
	}

	void Damage(float value){	
		blurMultiplier += value * 0.1f;
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		base.material.SetFloat("_BlurMulti", blurMultiplier);
		Graphics.Blit (source, destination, base.material);
		blurMultiplier -= speed * Time.deltaTime;
		blurMultiplier = Mathf.Clamp (blurMultiplier, 0.0f, 5.0f);
	}
}