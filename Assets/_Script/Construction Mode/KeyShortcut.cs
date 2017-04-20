using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyShortcut : MonoBehaviour {
	public FuseEvent fuse;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("s")) {
			SimpleData.WriteDataPoint("Skip_Scene", "", "", "", "", "Incomplete_Construction");
			fuse.LevelDone();
		}
	}
}
