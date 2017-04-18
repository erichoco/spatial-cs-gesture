using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapController : MonoBehaviour {
	GameObject camera;

	// Use this for initialization
	void Start () {
		camera = GameObject.Find("ConstructionCamRig");
	}

	// Update is called once per frame
	void Update () {
		transform.localRotation = camera.transform.localRotation;
	}
}
