using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Leap;

public class CustomGrabbableObject : GrabbableObject {
	Rigidbody rb;
	GameObject hand;

	bool isHandExist;
	bool isReleased;
	Quaternion initAngle;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		hand = null;

		isHandExist = false;
		isReleased = true;
		initAngle = transform.rotation;
	}

	// Update is called once per frame
	void Update () {
		if (isHandExist) {
			if (hand == null) {
				snapObject();
				isHandExist = false;
			}
		} else {
			hand = GameObject.Find("RigidRoundHand(Clone)");
			isHandExist = (hand != null);
		}

		if (!isReleased && checkRotationAngle()) {
			OnComplete();
		}
	}

	// public bool IsGrabbed() {
	// 	return isGrabbed;
	// }

	public override void OnGrab() {
		if (!isReleased) return;
		base.OnGrab();
		rb.isKinematic = false;
		isReleased = false;
	}

	public override void OnRelease() {
		base.OnRelease();
		rb.isKinematic = true;
		isReleased = true;
		snapObject();
	}

	public void OnComplete() {
		rb.isKinematic = true;
		snapObject();
		// Debug.Log("On Complete!");
	}

	// Fix object angle in 3 directions to 0, 90, 180, or 270.
	void snapObject() {
		Vector3 newAngle = new Vector3();
		newAngle.x = Mathf.Round(transform.eulerAngles.x/90) * 90;
		newAngle.y = Mathf.Round(transform.eulerAngles.y/90) * 90;
		newAngle.z = Mathf.Round(transform.eulerAngles.z/90) * 90;
		transform.eulerAngles = newAngle;
		initAngle = transform.rotation;
	}

	bool checkRotationAngle() {
		float delta = Quaternion.Angle(initAngle, transform.rotation);
		return delta > 70;
	}
}
