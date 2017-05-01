using System;
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
	int axisFixed;

	// Time buffer for starting rotating and moving
	float palmFlatDuration;

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
		axisFixed = 0;

		palmFlatDuration = 0f;

		childRenderers = GetComponentsInChildren<Renderer>();
		initShaders = new List<Shader>();
		foreach (Renderer r in childRenderers) {
			initShaders.Add(r.material.shader);
		}
		highlightShader = Shader.Find("Self-Illumin/Outlined Diffuse");
	}

	// Update is called once per frame
	void FixedUpdate () {

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
					highlightChildren();
					initPalmRotation();
				}
				updatePalmRotation();
				updateTargetRotation();
			} else {
				rb.isKinematic = true;
				rb.angularVelocity = Vector3.zero;
				axisFixed = 0;
				if (isRotating) {
					isRotating = false;
					unhighlightChildren();
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
			// highlightChildren();
			isHovering = true;
	}

	public override void OnStopHover() {
		base.OnStopHover();
		if (isHovering) {
			// unhighlightChildren();
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

	// 0: x axis, 1: y axis, 2: z axis
	void fixAxis() {
		Quaternion delta = palm_rotation_ * Quaternion.Inverse(Quaternion.identity);
		Vector3 angle = delta.eulerAngles;
		if (angle.x >= 180) angle.x = 360 - angle.x;
		if (angle.y >= 180) angle.y = 360 - angle.y;
		if (angle.z >= 180) angle.z = 360 - angle.z;

		// Debug.Log(delta.eulerAngles);

		Vector3 eAngles = initAngle.eulerAngles;
		if (Mathf.Abs(angle.x) > Mathf.Abs(angle.y)
			&& Mathf.Abs(angle.x) > Mathf.Abs(angle.z)) {
			// Debug.Log("xxxx");
			eAngles.z = transform.eulerAngles.z;
		} else if (Mathf.Abs(angle.y) > Mathf.Abs(angle.x)
			&& Mathf.Abs(angle.y) > Mathf.Abs(angle.z)) {
			// Debug.Log("yyyy");
			eAngles.y = transform.eulerAngles.y;
		} else if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x)
			&& Mathf.Abs(angle.z) > Mathf.Abs(angle.y)) {
			// Debug.Log("zzzz");
			eAngles.x = transform.eulerAngles.x;
		}
		transform.eulerAngles = eAngles;
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

		int neg = 1;
		if (angle >= 180) {
			angle = 360 - angle;
			axis = -axis;
			neg = -1;
		}

		if (angle != 0) {
			// Quaternion delta = palm_rotation_ * Quaternion.Inverse(init_palm_rotation_);
			// Debug.Log(delta.eulerAngles);
			fixRotationAxis();

			// switch (axisFixed) {
			// 	case 1:
			// 		rb.angularVelocity = angle * (neg * Vector3.forward);
			// 		break;
			// 	case 2:
			// 		rb.angularVelocity = angle * (-neg * Vector3.up);
			// 		break;
			// 	case 3:
			// 		rb.angularVelocity = angle * (neg * Vector3.right);
			// 		break;
			// 	default:
			// 		rb.angularVelocity = (angle) * axis;
			// 		fixRotationAxis();
			// 		break;
			// }
			// rb.angularVelocity = (angle*1.5f) * axis;
		}
	}

	void fixRotationAxis() {
		Vector3 newAngle = initAngle.eulerAngles;
		// Debug.Log(axisFixed);
		Quaternion fromInitRotation = palm_rotation_ * Quaternion.Inverse(init_palm_rotation_);
		float fromInitAngle = 0.0f;
		Vector3 axis = Vector3.zero;
		fromInitRotation.ToAngleAxis(out fromInitAngle, out axis);

		if (fromInitAngle >= 180) {
			fromInitAngle = fromInitAngle - 360;
		}
		// Debug.Log("angle: "+ fromInitAngle);
		// Debug.Log("axis: " + axis);
		// Debug.Log("Fixed! " + axisFixed);
		switch (axisFixed) {
			case 1:
				// newAngle.x = transform.eulerAngles.x;
				// Debug.Log("X Orientation " + axis.x);
				transform.Rotate((Mathf.Sign(axis.x) * Vector3.right) * 1, Space.World);

				// transform.rotation = initAngle * Quaternion.AngleAxis(fromInitAngle,
				// 	(Mathf.Sign(axis.x) * Vector3.forward));
				Debug.Log("X " + transform.eulerAngles);
				break;
			case 2:
				// newAngle.y = transform.eulerAngles.y;
				transform.Rotate((Mathf.Sign(axis.y) * Vector3.up) * 1, Space.World);

				// transform.rotation = initAngle * Quaternion.AngleAxis(fromInitAngle,
				// 	(Mathf.Sign(axis.y) * Vector3.right));
				Debug.Log("Y " + transform.eulerAngles);
				break;
			case 3:
				// newAngle.z = transform.eulerAngles.z;
				transform.Rotate((Mathf.Sign(axis.z) * Vector3.forward) * 1, Space.World);

				// transform.rotation = initAngle * Quaternion.AngleAxis(fromInitAngle,
				// 	(-Mathf.Sign(axis.z) * Vector3.up));
				Debug.Log("Z " + transform.eulerAngles);
				break;
			case 0:
				axisFixed = checkFixRotationAxis();
				if (axisFixed > 0)
					Debug.Log("Fixed! " + axisFixed);
				// transform.rotation = initAngle * Quaternion.AngleAxis(fromInitAngle, axis);
				return;
			default:
				break;
		}
		// transform.eulerAngles = newAngle;
	}

	// 0: none, 1: x, 2: y, 3: z
	int checkFixRotationAxis() {
		// Check angle difference from initial position
		// Quaternion fromInitRotation = transform.rotation * Quaternion.Inverse(initAngle);
		Quaternion fromInitRotation = palm_rotation_ * Quaternion.Inverse(init_palm_rotation_);
		float fromInitAngle = 0.0f;
		Vector3 _ = Vector3.zero;
		fromInitRotation.ToAngleAxis(out fromInitAngle, out _);
		// Debug.Log("Init " + fromInitAngle);
		// Fix rotation to one direction
		if (fromInitAngle > 20) {
			Quaternion delta = palm_rotation_ * Quaternion.Inverse(init_palm_rotation_);
			Vector3 angle = delta.eulerAngles;
			if (angle.x >= 180) angle.x = 360 - angle.x;
			if (angle.y >= 180) angle.y = 360 - angle.y;
			if (angle.z >= 180) angle.z = 360 - angle.z;
			// Debug.Log(angle);

			if (Mathf.Abs(angle.x) > Mathf.Abs(angle.y)
				&& Mathf.Abs(angle.x) > Mathf.Abs(angle.z)) {
				return 1;
			} else if (Mathf.Abs(angle.y) > Mathf.Abs(angle.x)
				&& Mathf.Abs(angle.y) > Mathf.Abs(angle.z)) {
				return 2;
			} else if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x)
				&& Mathf.Abs(angle.z) > Mathf.Abs(angle.y)) {
				return 3;
			}
		}
		return 0;
	}

	bool checkRotationEnabled() {
		Hand hand = gController.GetCurrentHand();
		if (gController.IsControlEnabled() &&
			hand != null && hand.GrabStrength < 0.3) {
			if (palmFlatDuration > LeapStatic.minFlatTime) return true;
			else palmFlatDuration += 0.02f;

		} else {
			palmFlatDuration = 0;
		}
		return false;
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
