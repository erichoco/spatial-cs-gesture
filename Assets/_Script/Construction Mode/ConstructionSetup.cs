using UnityEngine;
using System;
using System.Collections;

public class ConstructionSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SimpleData.WriteStringToFile("LeapData.txt", "***This is another try at " + DateTime.Now.ToString() + ".");
        LeapStatic.resetConstructionObject("construction");
	}

	// Update is called once per frame
	void Update () {

	}
}
