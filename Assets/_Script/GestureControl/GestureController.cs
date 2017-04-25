using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class GestureController : MonoBehaviour {
	Controller controller;

	// Grabbing
	float grabDuration;
	float grabDuration1;

	// Clapping
	const int POS_LIST_SIZE = 5;
	List<Vector3> leftHandPosList;
	List<Vector3> rightHandPosList;
	Vector3 prevLeftHandPos;
	Vector3 prevRightHandPos;
	float maxClapTime;
	float clapDuration;
	bool isHandClapped;

	// Game
	public string SceneName;
	FuseEvent fuseEvent;
	CameraControls cameraControl;

	bool controlEnabled;

	// Boundaries of gesture movements
	public Vector3 MaxMovement = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
	public Vector3 MinMovement = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);

	void Start () {
		controller = new Controller();

		grabDuration = 0f;
		grabDuration1 = 0f;

		leftHandPosList = new List<Vector3>();
		rightHandPosList = new List<Vector3>();
		prevLeftHandPos = new Vector3();
		prevRightHandPos = new Vector3();
		isHandClapped = false;
		maxClapTime = 2f;
		clapDuration = 0f;

		LeapStatic.resetConstructionObject(SceneName);
		fuseEvent = GameObject.Find("EventSystem").GetComponent<FuseEvent>();
		cameraControl = GameObject.Find("ConstructionCamRig").GetComponent<CameraControls>();
		GameObject.Find("RotationGizmo").SetActive(false);

		controlEnabled = false;
	}

	void Update () {
		Frame frame = controller.Frame();
		HandList hands = frame.Hands;

		controlEnabled = false;
		if (hands.Count == 2) {
			if (checkOneGrabStarted(hands)) {
				Vector grabVelocity = frame.Hands[0].PalmVelocity;
				cameraControl.GrabView(grabVelocity.x / LeapStatic.grabViewFactor, -grabVelocity.y / LeapStatic.grabViewFactor);
			}
			/* else if (checkClapped(hands)) {
			 	fuseEvent.initiateFuse();
			}*/
		} else if (hands.Count == 1) {
			controlEnabled = true;
		}
	}

	public bool IsControlEnabled() {
		return controlEnabled;
	}

	bool checkTwoGrabStarted(HandList hands) {
		if (hands[0].GrabStrength > 0.8 && hands[1].GrabStrength > 0.8) {
			grabDuration1 += 0.02f;
			return grabDuration1 > LeapStatic.minGrabTime;
		}
		grabDuration1 = 0f;
		return false;
	}

	bool checkOneGrabStarted(HandList hands) {
		if (hands[0].GrabStrength > 0.8 || hands[1].GrabStrength > 0.8) {
			grabDuration += 0.02f;
			return grabDuration > LeapStatic.minGrabTime;
		}
		grabDuration = 0f;
		return false;
	}

	bool checkGrabStarted(HandList hands) {
		if (isGrabbing(hands[0]) && isGrabbing(hands[1])) {
			grabDuration += 0.02f;
			return grabDuration > LeapStatic.minGrabTime;
		}
		grabDuration = 0f;
		return false;
	}

	bool checkClapped (HandList hands) {
		List<Vector3> curHandPos = getCurHandPos(hands);

		if (curHandPos.Count < 2) {
			return false;
		}

		Vector3 curLeftHandPos = curHandPos[0], curRightHandPos = curHandPos[1];

		if (isHandClapped) {
			// Clap released
			if (Vector3.Distance(curLeftHandPos, curRightHandPos) > 20f) {
				isHandClapped = false;
				clapDuration = 0f;
				return true;
			}
		} else {
			if (isClapping(curLeftHandPos, curRightHandPos)) {
				clapDuration += 0.02f;
			}
			// Clapped (still need to release to complete gesture)
			if (Vector3.Distance(curLeftHandPos, curRightHandPos) < 10f) {
				if (clapDuration <= maxClapTime) {
					isHandClapped = true;
				}
				clapDuration = 0f;
			}
		}
		return false;
	}

	bool isGrabbing(Hand hand) {
		return hand.GrabStrength > 0.1 && hand.GrabStrength < 0.8;
	}

	bool isClapping (Vector3 curLeftHandPos, Vector3 curRightHandPos) {
		Vector3 stickDirection = prevRightHandPos - prevLeftHandPos;
		Vector3 leftDirection = curLeftHandPos - prevLeftHandPos;
		Vector3 rightDirection = curRightHandPos - prevRightHandPos;
		Vector3 connectDirection = leftDirection + rightDirection;
		int factor = 5;

		prevLeftHandPos = curLeftHandPos;
		prevRightHandPos = curRightHandPos;

		return ((Mathf.Abs(stickDirection.y) < Mathf.Abs(stickDirection.x) || Mathf.Abs(stickDirection.y) < Mathf.Abs(stickDirection.z)) &&
			(Mathf.Abs(leftDirection.y) < Mathf.Abs(leftDirection.x) || Mathf.Abs(leftDirection.y) < Mathf.Abs(leftDirection.z)) &&
			(Mathf.Abs(rightDirection.y) < Mathf.Abs(rightDirection.x) || Mathf.Abs(rightDirection.y) < Mathf.Abs(rightDirection.z)) &&
			(Mathf.Abs(leftDirection.x) + Mathf.Abs(leftDirection.z)) > factor &&
			(Mathf.Abs(rightDirection.x) + Mathf.Abs(rightDirection.z)) > factor &&
			(connectDirection.x + connectDirection.z) < factor);
	}

	List<Vector3> getCurHandPos (HandList hands) {
		List<Vector3> curHandPos = new List<Vector3>();
		Hand leftHand = hands[0], rightHand = hands[1];
		if (hands[1].IsLeft) {
			leftHand = hands[1];
			rightHand = hands[0];
		}

		leftHandPosList.Add(transform.TransformPoint(leftHand.PalmPosition.ToUnityScaled()));
		rightHandPosList.Add(transform.TransformPoint(rightHand.PalmPosition.ToUnityScaled()));
		if (leftHandPosList.Count != rightHandPosList.Count) {
			leftHandPosList.Clear();
			rightHandPosList.Clear();
		} else if (leftHandPosList.Count == POS_LIST_SIZE) {
			curHandPos.Add(listAverage(leftHandPosList));
			curHandPos.Add(listAverage(rightHandPosList));
			leftHandPosList.RemoveAt(0);
			rightHandPosList.RemoveAt(0);
		}
		return curHandPos;
	}

	Vector3 listAverage(List<Vector3> l) {
		Vector3 sum = new Vector3(0, 0, 0);
		foreach (Vector3 i in l) sum += i;
		return sum / l.Count;
	}

}
