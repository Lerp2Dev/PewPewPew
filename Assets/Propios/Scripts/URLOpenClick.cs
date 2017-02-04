using UnityEngine;
using System.Collections;

public class URLOpenClick : MonoBehaviour {

	public string WebPage = "http://www.feliarts.hol.es/";
	
	void OnMouseUp(){
		Application.OpenURL (WebPage);
	}
}
