using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyShortcut : MonoBehaviour {
	public FuseEvent fuse;

	Vector3 initPos;

	// Use this for initialization
	void Start () {
		// initPos should be the same with createLoc in CreatePart__
		initPos = new Vector3(-40, 25, 100);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("s")) {
			if (fuse != null) {
				SimpleData.WriteDataPoint("Skip_Scene", "", "", "", "", "Incomplete_Construction");
				fuse.LevelDone();
			}
		} else if (Input.GetKeyDown("r")) {
			GameObject go = LeapStatic.GetControlObject();
			if (go != null) {
				go.transform.position = initPos;
			}
		} else if (Input.GetKeyDown("return")) {
			if (fuse != null) {
				fuse.initiateFuse();
			}
		} else if (Input.GetKeyDown("space")) {
			GestureController gc = GameObject.Find("HandController").GetComponent<GestureController>();
			gc.SwitchMode();
			Text t = GameObject.Find("mode panel/Text").GetComponent<Text>();
			if (gc.GetMode() == 0) {
				t.text = "MOVE";
			} else if (gc.GetMode() == 1) {
				t.text = "ROTATE";
			}
		}
	}
}
