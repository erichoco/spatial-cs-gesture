using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Leap;

public class CustomGrabbableObject : GrabbableObject {
	Rigidbody rb;
	GameObject hand;
	GestureController gController;

	bool isHandExist;
	bool isReleased;
	Quaternion initAngle;

	// Shader for Highlighting
	Renderer[] childRenderers;
	List<Shader> initShaders;
	Shader highlightShader;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		gController = GameObject.Find("HandController").GetComponent<GestureController>();
		hand = null;

		isHandExist = false;
		isReleased = true;
		initAngle = transform.rotation;

		childRenderers = GetComponentsInChildren<Renderer>();
		initShaders = new List<Shader>();
		foreach (Renderer r in childRenderers) {
			initShaders.Add(r.material.shader);
		}
		highlightShader = Shader.Find("Self-Illumin/Outlined Diffuse");
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
	// 	return !isReleased;
	// }

	public override void OnGrab() {
		if (!isReleased) return;
		if (!gController.IsControlEnabled()) return;
		base.OnGrab();
		rb.isKinematic = false;
		isReleased = false;
	}

	public override void OnRelease() {
		base.OnRelease();
		rb.isKinematic = true;
		isReleased = true;
		snapObject();
		unhighlightChildren();
	}

	public override void OnStartHover() {
		base.OnStartHover();
		if (isReleased) {
			highlightChildren();
		}
	}

	public override void OnStopHover() {
		base.OnStopHover();
		unhighlightChildren();
	}

	public void OnComplete() {
		rb.isKinematic = true;
		snapObject();
		unhighlightChildren();
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

	void highlightChildren() {
		foreach (Renderer r in childRenderers) {
			r.material.shader = highlightShader;
		}
	}

	void unhighlightChildren() {
		for (int i = 0; i < childRenderers.Length; i++) {
			childRenderers[i].material.shader = initShaders[i];
		}
	}
}
