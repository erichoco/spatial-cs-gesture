using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class GestureController : MonoBehaviour {
	public enum State {
		Inactive, // Hand not grabbing target
		Active,   // Hand grabbing obejct, target allowed to rotate and translate
		Done,     // Hand still grabbing target and can translate target,
		          // but target cannot rotate
	}

	// Leap Motion
	Controller controller;
	int handIDActive;
	Vector3 thumbTip;
	Vector3 indexTip;
	Vector3 palmNormal;
	Vector3 prevPalmNormal;
	Vector3 origPalmNormal;

	// Scene
	PartController partController;

	GameObject target;
	Vector3 origAngle;
	Quaternion origTargetRot;

	// Control state
	State state;
	bool transformActive;
	bool grabReleased;

	void Awake () {
		controller = new Controller();
		handIDActive = -1;
		thumbTip = new Vector3();
		indexTip = new Vector3();
		palmNormal = new Vector3();
		origPalmNormal = new Vector3();

		target = new GameObject();
		origAngle = new Vector3();
		origTargetRot = new Quaternion();

		state = State.Inactive;
		transformActive = false;
		grabReleased = true;
	}

	// Use this for initialization
	void Start () {
		partController = GetComponent<PartController>();
	}

	// Update is called once per frame
	void Update () {
		if (!checkTarget()) {
			return;
		}

		Frame frame = controller.Frame();
		HandList hands = frame.Hands;

		if (target == null) {
			target = partController.GetCurrentPart();
			origAngle = target.transform.eulerAngles;
			origTargetRot = target.transform.rotation;
			Debug.Log("Gesture " + target);
		}
		switch (state) {
		case State.Inactive:
			handIDActive = -1;
			foreach (Hand h in hands) {
				if (checkGrabbingObject(h)) {
					handIDActive = h.Id;
					origPalmNormal = palmNormal;
					origAngle = target.transform.eulerAngles;
					origTargetRot = target.transform.rotation;
					state = State.Active;
					break;
				}
			}
			if (-1 == handIDActive) {
				// snapObject();
			}
			break;

		case State.Active:
			if (checkGrabbingObject(frame.Hand(handIDActive))) {
				// Debug.Log(handActive.PalmNormal);
				translateObject();
				rotateObject();
			} else {
				snapObject();
				state = State.Inactive;
			}
			if (checkRotationDone()) {
				snapObject();
				state = State.Done;
			}
			break;

		case State.Done:
			if (checkGrabbingObject(frame.Hand(handIDActive))) {
				translateObject();
			} else {
				// snapObject();
				state = State.Inactive;
			}
			break;

		default:
			break;
		}
	}

	bool checkTarget() {
		target = partController.GetCurrentPart();
		if (target == null) {
			return false;
		} else {
			origAngle = target.transform.eulerAngles;
			origTargetRot = target.transform.rotation;
			return true;
		}
	}

	bool checkGrabbingObject(Hand hand) {
		if (!hand.IsValid) return false;
		GameObject thumb = GameObject.Find("RigidRoundHand(Clone)/thumb/bone3");
		GameObject index = GameObject.Find("RigidRoundHand(Clone)/index/bone3");
		thumbTip = thumb.transform.position;
		indexTip = index.transform.position;
		prevPalmNormal = palmNormal;
		Vector v = hand.PalmNormal;
		palmNormal = new Vector3(v.x, -v.y, -v.z);

		if (target.GetComponent<Renderer>().bounds.Contains(thumbTip) &&
			target.GetComponent<Renderer>().bounds.Contains(indexTip)) {
			Debug.Log("[Gesture] Grab Object!");
			// target.GetComponent<Renderer>().material.color = new Color(0.5f, 1, 1);
			return true;
		} else {
			// target.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
			return false;
		}

	}

	bool checkRotationDone() {
		Vector3 angle = Quaternion.FromToRotation(origPalmNormal, palmNormal).eulerAngles;
		Debug.Log(angle);
		if ((angle.x > 70 && angle.x < 290) ||
			(angle.y > 70 && angle.y < 290) ||
			(angle.z > 70 && angle.z < 290)) {
			return true;
		} else {
			return false;
		}
	}

	void translateObject() {
		Vector3 midpoint = (thumbTip + indexTip) / 2;
		target.transform.position = midpoint;
	}

	void rotateObject() {
		target.transform.Rotate(
			Quaternion.FromToRotation(prevPalmNormal, palmNormal).eulerAngles,
			Space.World);
	}

	void snapObject() {
		Vector3 newAngle = new Vector3();
		newAngle.x = Mathf.Round(target.transform.eulerAngles.x/90) * 90;
		newAngle.y = Mathf.Round(target.transform.eulerAngles.y/90) * 90;
		newAngle.z = Mathf.Round(target.transform.eulerAngles.z/90) * 90;
		target.transform.eulerAngles = newAngle;
		origAngle = target.transform.eulerAngles;
		origTargetRot = target.transform.rotation;
	}
}
