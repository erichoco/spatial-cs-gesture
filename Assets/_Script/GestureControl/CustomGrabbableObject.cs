using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Leap;

public class CustomGrabbableObject : GrabbableObject {
	Collider collider;
	Rigidbody rb;


	GameObject hand;
	bool isHandExist;
	bool isGrabbed;

	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider>();
		rb = GetComponent<Rigidbody>();
		hand = null;

		isHandExist = false;
		isGrabbed = false;
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
	}

	public bool IsGrabbed() {
		return isGrabbed;
	}

	public override void OnGrab() {
		base.OnGrab();
		rb.isKinematic = false;
		isGrabbed = true;
	}

	public override void OnRelease() {
		base.OnRelease();
		rb.isKinematic = true;
		snapObject();

		isGrabbed = false;
	}

	// Fix object angle in 3 directions to 0, 90, 180, or 270.
	void snapObject() {
		Vector3 newAngle = new Vector3();
		newAngle.x = Mathf.Round(transform.eulerAngles.x/90) * 90;
		newAngle.y = Mathf.Round(transform.eulerAngles.y/90) * 90;
		newAngle.z = Mathf.Round(transform.eulerAngles.z/90) * 90;
		transform.eulerAngles = newAngle;
	}
}
