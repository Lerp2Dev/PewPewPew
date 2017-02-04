using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Aubergine/RadialBlur")]
public class PP_RadialBlur : PostProcessBase {
	public float amount = 0.5f; 	//This value is between 0 to 1 (0 is left, 1 is right)
	public float speed = 0.5f; 	//This value is between
	void Awake () {
		base.material.SetFloat ("_CenterX", amount);
		base.material.SetFloat ("_CenterY", amount);
	}

	void OnEnable () {
		base.shader = Shader.Find("Hidden/Aubergine/RadialBlur");
	}
	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		base.material.SetFloat ("_CenterX", amount);
		base.material.SetFloat ("_CenterY", amount);
		Graphics.Blit (source, destination, material);
		amount -= speed * Time.deltaTime;
		amount = Mathf.Clamp01 (amount);
	}
}