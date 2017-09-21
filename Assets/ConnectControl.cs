using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectControl : MonoBehaviour {
	SelectPart sp;

	// Use this for initialization
	void Start () {
		sp = GetComponent<SelectPart>();
	}

	// Update is called once per frame
	void Update () {
		GameObject selectedObject = sp.getSelectedObject();
		GameObject selectedFuseTo = sp.getSelectedFuseTo();
		if (selectedObject != null && selectedFuseTo != null) {

		}
	}
}
