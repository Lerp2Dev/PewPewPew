using UnityEngine;
using System.Collections;

public class QualityControl : MonoBehaviour {

	public float amount = 1.0f;
	public Behaviour medium;
	public Behaviour low;

	void Awake(){
		amount = PlayerPrefs.GetInt ("VDetails", 2);
		if(amount == 2){
			QualitySettings.SetQualityLevel (2);
		}else if(amount == 1){
			QualitySettings.SetQualityLevel (1);
			medium.enabled = false;
		}else if(amount == 0){
			QualitySettings.SetQualityLevel (0);
			medium.enabled = false;
			low.enabled = false;
		}
		AudioListener.volume = PlayerPrefs.GetFloat ("SVolume", 1.0f);
	}

}
