using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class CustomGrabbableObject : GrabbableObject {
	Rigidbody rb;
	GameObject hand;
	GestureController gController;

	bool isHandExist;
	bool isHovering;
	bool isReleased;
	Quaternion initAngle;
	Vector3 initUp;
	Vector3 initForward;

	// Hand Angles
	bool isRotating;
	Quaternion palm_rotation_;
	Quaternion init_palm_rotation_;
	Quaternion rotation_from_palm_;

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
		isHovering = false;
		isReleased = true;
		initAngle = transform.rotation;
		initUp = transform.up;
		initForward = transform.forward;

		childRenderers = GetComponentsInChildren<Renderer>();
		initShaders = new List<Shader>();
		foreach (Renderer r in childRenderers) {
			initShaders.Add(r.material.shader);
		}
		highlightShader = Shader.Find("Self-Illumin/Outlined Diffuse");
	}

	// Update is called once per frame
	void Update () {

		// Hand disappears
		if (isHandExist && hand == null) {
			snapObject();
			isHandExist = false;
			return;
		} else if (!isHandExist) {
			hand = GameObject.Find("RigidRoundHand(Clone)");
			isHandExist = (hand != null);
		}

		// if (!isReleased && checkRotationAngle()) {
		// 	OnComplete();
		// }

		// 0: Moving Mode, 1: Rotation Mode
		if (gController.GetMode() == 0) {
			rb.angularVelocity = Vector3.zero;
			isRotating = false;

		} else if (gController.GetMode() == 1) {
			rb.velocity = Vector3.zero;

			// Rotation enabled
			if (checkRotationEnabled()) {
				rb.isKinematic = false;
				if (!isRotating) {
					isRotating = true;
					initPalmRotation();
				}
				updatePalmRotation();
				updateTargetRotation();
			} else {
				rb.isKinematic = true;
				rb.angularVelocity = Vector3.zero;
				if (isRotating) {
					isRotating = false;
					snapObject();
				}
			}

		} else {
			rb.isKinematic = true;
		}
	}

	// public bool IsGrabbed() {
	// 	return !isReleased;
	// }

	public override void OnGrab() {
		// if (!isReleased || !gController.IsControlEnabled()) return;
		if (!gController.IsControlEnabled() || gController.GetMode() != 0) {
			rb.isKinematic = true;
			return;
		}
		base.OnGrab();
		rb.isKinematic = false;
		isReleased = false;
	}

	public override void OnRelease() {
		if (gController.GetMode() != 0) return;
		base.OnRelease();
		rb.isKinematic = true;
		isReleased = true;
		// snapObject();
		// unhighlightChildren();
	}

	public override void OnStartHover() {
		base.OnStartHover();
		// if (isReleased && gController.GetMode() != 0) {
		// }
			highlightChildren();
			isHovering = true;
	}

	public override void OnStopHover() {
		base.OnStopHover();
		if (isHovering) {
			unhighlightChildren();
		}
		isHovering = false;
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
		float angle = Quaternion.Angle(initAngle, transform.rotation);
		if (angle > 50) {
			logRotationAxis();
		}
		initAngle = transform.rotation;
		initUp = transform.up;
		initForward = transform.forward;
	}

	void initPalmRotation() {
		HandModel hand_model = hand.GetComponent<HandModel>();
		palm_rotation_ = hand_model.GetPalmRotation();
		rotation_from_palm_ = Quaternion.Inverse(palm_rotation_) * transform.rotation;
		init_palm_rotation_ = palm_rotation_;
	}

	void updatePalmRotation() {
		HandModel hand_model = hand.GetComponent<HandModel>();
		palm_rotation_ = Quaternion.Slerp(palm_rotation_, hand_model.GetPalmRotation(),
									  1.0f - 0.4f); // rotationFiltering);
	}

	void updateTargetRotation() {
		Quaternion target_rotation = palm_rotation_ * rotation_from_palm_;
		Quaternion delta_rotation = target_rotation *
								Quaternion.Inverse(transform.rotation);

		float angle = 0.0f;
		Vector3 axis = Vector3.zero;
		delta_rotation.ToAngleAxis(out angle, out axis);

		if (angle >= 180) {
			angle = 360 - angle;
			axis = -axis;
		}
		if (angle != 0) {
			rb.angularVelocity = angle * axis;
		}
	}

	bool checkRotationEnabled() {
		Hand hand = gController.GetCurrentHand();
		return isHovering && gController.IsControlEnabled() &&
			hand != null && hand.GrabStrength > 0.7;
	}

	bool checkRotationAngle() {
		float delta = Quaternion.Angle(initAngle, transform.rotation);
		return delta > 70;
	}

	void logRotationAxis() {
		Vector3 crossUp = Vector3.Cross(initUp, transform.up);
		if (crossUp.x > 0.5) {
			// Debug.Log("+x");
			SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "X");
		} else if (crossUp.x < -0.5) {
			// Debug.Log("-x");
			SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "X");
		} else if (crossUp.y > 0.5) {
			// Debug.Log("+y");
			SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Y");
		} else if (crossUp.y < -0.5) {
			// Debug.Log("-y");
			SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Y");
		} else if (crossUp.z > 0.5) {
			// Debug.Log("+z");
			SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Z");
		} else if (crossUp.z < -0.5) {
			// Debug.Log("-z");
			SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Z");
		} else {
			Vector3 crossForward = Vector3.Cross(initForward, transform.forward);
			if (crossForward.x > 0.5) {
				// Debug.Log("+x");
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "X");
			} else if (crossForward.x < -0.5) {
				// Debug.Log("-x");
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "X");
			} else if (crossForward.y > 0.5) {
				// Debug.Log("+y");
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Y");
			} else if (crossForward.y < -0.5) {
				// Debug.Log("-y");
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Y");
			} else if (crossForward.z > 0.5) {
				// Debug.Log("+z");
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Z");
			} else if (crossForward.z < -0.5) {
				// Debug.Log("-z");
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Z");
			}
		}
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
