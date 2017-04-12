using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartController : MonoBehaviour {
	// Parts used in current scene. Add in Unity Editor.
	// Note that the index follows the order part will appear.
	// parts[0] is the first part.
	public List<GameObject> parts;

	List<string> partNames;
	int currentPartIdx;

	void Awake () {
		initPartNames();
		currentPartIdx = 0;
	}

	void Start () {
		GameObject.Find("RotationGizmo").SetActive(false);
	}

	void Update () {

	}

	void initPartNames() {
		partNames = new List<string>();
		foreach (GameObject go in parts) {
			// partNames.Add(go.transform.name + "(Clone)");
			partNames.Add(go.transform.name);
		}
	}

	public GameObject GetCurrentPart() {
		return GameObject.Find(partNames[currentPartIdx]);
	}
}
