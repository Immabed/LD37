using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraAspect : MonoBehaviour {

	Camera cam;
	[SerializeField]
	float defaultAspect = 16 / 9;
	
	// Update is called once per frame
	void Update () {
		// Ensure 
		if (cam == null)
			cam = GetComponent<Camera>();
		var aspect = cam.aspect;
		if (aspect >= defaultAspect){
			cam.orthographicSize = 5;
		}
		else {
			cam.orthographicSize = (defaultAspect * 5f) / aspect;
		}
	}
}
