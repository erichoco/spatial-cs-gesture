using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrabbableObject : GrabbableObject {
	Collider collider;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider>();
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {
		GameObject go = GameObject.Find("RigidRoundHand(Clone)");
		if (go == null) {
			snapObject();
		}
	}

	public override void OnGrab() {
		Debug.Log("[Grabbable] OnGrab");
		base.OnGrab();
		rb.isKinematic = false;
	}

	public override void OnRelease() {
		base.OnRelease();
		rb.isKinematic = true;
		snapObject();
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
