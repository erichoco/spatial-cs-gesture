using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class LeapFuse : MonoBehaviour {
	private GameObject eventSystem;

	private Dictionary<String, GameObject> fusedDict = new Dictionary<string, GameObject>();
	private List<GameObject> fusedAttachList = new List<GameObject>() ;
	private List<GameObject> controlAttachList = new List<GameObject>();

	void AddToList(GameObject parent, List<GameObject> list) {
		if (parent == null)
			return;

		switch (parent.name) {
			//tutorial1
			case "tutorial1_box":
				list.Add(GameObject.Find("box_cone_attach"));
				list.Add(GameObject.Find("box_pyr_attach"));
				list.Add(GameObject.Find("box_tri_attach"));
				break;
			case "tutorial1_conePrefab(Clone)":
				list.Add(GameObject.Find("cone_box_attach"));
				break;
			case "tutorial1_pyrPrefab(Clone)":
				list.Add(GameObject.Find("pyr_box_attach"));
			break;
			case "tutorial1_triPrefab(Clone)":
				list.Add(GameObject.Find("tri_box_attach"));
				break;

			//tutorial2
			case "tutorial2_longbox":
				list.Add(GameObject.Find("longbox_bigbox_attach"));
				list.Add(GameObject.Find("longbox_smallbox_yellow_attach"));
				list.Add(GameObject.Find("longbox_tallbox_attach"));
				break;
			case "tutorial2_bigboxPrefab(Clone)":
				list.Add(GameObject.Find("bigbox_longbox_attach"));
				list.Add(GameObject.Find("bigbox_smallbox_blue_attach"));
				break;
			case "tutorial2_smallbox_bluePrefab(Clone)":
				list.Add(GameObject.Find("smallbox_blue_bigbox_attach"));
				break;
			case "tutorial2_tallboxPrefab(Clone)":
				list.Add(GameObject.Find("tallbox_longbox_attach"));
				break;
			case "tutorial2_smallbox_yellowPrefab(Clone)":
				list.Add(GameObject.Find("smallbox_yellow_longbox_attach"));
				break;

			//construction
			case "BodyPrefab(Clone)":
				list.Add(GameObject.Find("Body_Bottom_Attach"));
				list.Add(GameObject.Find("Body_Top_Attach"));
				list.Add(GameObject.Find("Body_Side_Attach"));
				break;
			case "calfPrefab(Clone)":
				list.Add(GameObject.Find("Calf_Bottom_Attach"));
				list.Add(GameObject.Find("Calf_Top_Attach"));
				break;
			case "trimPrefab(Clone)":
				list.Add(GameObject.Find("Top_Trim_Attach"));
				break;
			case "ToePrefab(Clone)":
				list.Add(GameObject.Find("Toe_Bottom_Attach"));
				list.Add(GameObject.Find("Toe_Side_Attach"));
				break;
			case "ToeSolePrefab(Clone)":
				list.Add(GameObject.Find("Sole_Toe_Top_Attach"));
				list.Add(GameObject.Find("Sole_Toe_Side_Attach"));
				break;
			case "rocket_boots_start":
				list.Add(GameObject.Find("Sole_Heel_Top_Attach"));
				list.Add(GameObject.Find("Sole_Heel_Side_Attach"));
				break;

			//axe
			case "startObject":
				list.Add(GameObject.Find("shaft_haft_attach"));
				list.Add(GameObject.Find("shaft_trapezoid_attach"));
				break;
			case "bottom_pointPrefab(Clone)":
				list.Add(GameObject.Find("bottom_point_head_attach"));
				break;
			case "trapezoidPrefab(Clone)":
				list.Add(GameObject.Find("trapezoid_head_attach"));
				list.Add(GameObject.Find("trapezoid_shaft_attach"));
				break;
			case "top_pointPrefab(Clone)":
				list.Add(GameObject.Find("top_point_head_attach"));
				break;
			case "headPrefab(Clone)":
				list.Add(GameObject.Find("head_bottom_point_attach"));
				list.Add(GameObject.Find("head_top_point_attach"));
				list.Add(GameObject.Find("head_trapezoid_attach"));
				break;
			case "haftPrefab(Clone)":
				list.Add(GameObject.Find("haft_shaft_attach"));
				break;

			//key 1
			case "dangly_T_complete":
				list.Add(GameObject.Find("dangly_T_upright_L_attach"));
				list.Add(GameObject.Find("dangly_T_upright_T_attach"));
				list.Add(GameObject.Find("dangly_T_walking_pants_attach"));
				break;
			case "upright_rectPrefab(Clone)":
				list.Add(GameObject.Find("upright_rect_walking_pants_attach"));
				break;
			case "upright_LPrefab(Clone)":
				list.Add(GameObject.Find("upright_L_dangly_T_attach"));
				list.Add(GameObject.Find("upright_L_waluigi_attach"));
				break;
			case "upright_TPrefab(Clone)":
				list.Add(GameObject.Find("upright_T_dangly_T_attach"));
				break;
			case "waluigiPrefab(Clone)":
				list.Add(GameObject.Find("waluigi_upright_L_attach"));
				break;
			case "walking_pantsPrefab(Clone)":
				list.Add(GameObject.Find("walking_pants_dangly_T_attach"));
				list.Add(GameObject.Find("walking_pants_upright_rect_attach"));
				break;
			default:
				break;
		}
	}

	void Start () {
		eventSystem = GameObject.Find("EventSystem");
	}

	void FixedUpdate () {
		updateAttachList();
		setAttachObjects();
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
					AddToList(go, fusedAttachList);
				} else {
					goToControl = go;
					AddToList(goToControl, controlAttachList);
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
