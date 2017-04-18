using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class LeapFuse : MonoBehaviour {
	private GameObject eventSystem;

	private Dictionary<String, GameObject> fusedDict;
	private List<GameObject> fusedAttachList;
	private List<GameObject> controlAttachList;

	void Start () {
		eventSystem = GameObject.Find("EventSystem");
		fusedDict = new Dictionary<string, GameObject>();
		fusedAttachList = new List<GameObject>();
		controlAttachList = new List<GameObject>();
	}

	void FixedUpdate () {
		updateAttachList();
		setAttachObjects();
	}

	// Add child objects for attachment to list
	void addAttachList(GameObject parent, List<GameObject> list) {
		if (parent == null) return;
		Transform parentT = parent.transform;
		foreach (Transform childT in parentT) {
			if (childT.CompareTag("Attach")) {
				list.Add(childT.gameObject);
			}
		}
	}

	// Update object in control to control list
	// Update object fused to fused list
	void updateAttachList() {
		controlAttachList.Clear();
		GameObject goToControl = null;

		foreach (String part in LeapStatic.objectName) {
			// From object list in LeapStatic
			// Get object to attach to &
			// Get object on screen to control
			GameObject go = GameObject.Find(part);
			if (!fusedDict.ContainsKey(part) && go != null) {
				if (go.GetComponent<IsFused>().isFused) {
					fusedDict.Add(part, go);
					addAttachList(go, fusedAttachList);
				} else {
					goToControl = go;
					addAttachList(goToControl, controlAttachList);
				}
			}
		}
	}

	// Set object to attach & object to be attached to in SelectPart
	void setAttachObjects() {
		foreach (GameObject control in controlAttachList) {
			foreach (GameObject fused in fusedAttachList) {
				if (!fused.GetComponent<FuseBehavior>().isFused &&
					Vector3.Distance(control.transform.position, fused.transform.position) < 40 &&
					eventSystem.GetComponent<FuseEvent>().IsFuseMappingExist(fused, control)) {
					// Debug.Log("[LeapFuse] " + control);
					// Debug.Log("[LeapFuse] " + fused);
					// Debug.Log("[LeapFuse] setting objects");
					eventSystem.GetComponent<SelectPart>().SetSelectedFuseTo(fused);
					eventSystem.GetComponent<SelectPart>().SetSelectedObject(control);
				}
			}
		}
	}
}
