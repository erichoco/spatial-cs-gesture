using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class CustomGrabbableObject : MonoBehaviour {
	Rigidbody rb;
	GestureController gController;

	GameObject hand;
	bool isHandExist;

	// Time buffer for starting rotating and moving
	float palmFlatDuration;
	bool isControlling;

	// Hand Position (using some naming convention from Leap Motion GrabbingHand.cs)
	Vector3 initPosition;
	Vector3 palm_position_;
	Vector3 init_palm_position_;

	// Hand Angles
	int axisFixed; // 0: None, 1: x-axis, 2: y-axis, 3: z-axis
	Quaternion initRotation;
	Quaternion palm_rotation_;
	Quaternion init_palm_rotation_;
	// Quaternion rotation_from_palm_;

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

		palmFlatDuration = 0f;
		isControlling = false;

		initPosition = transform.position;
		palm_position_ = new Vector3();
		init_palm_position_ = new Vector3();

		axisFixed = 0;
		initRotation = transform.rotation;
		palm_rotation_ = new Quaternion();
		init_palm_rotation_ = new Quaternion();

		childRenderers = GetComponentsInChildren<Renderer>();
		initShaders = new List<Shader>();
		foreach (Renderer r in childRenderers) {
			initShaders.Add(r.material.shader);
		}
		highlightShader = Shader.Find("Self-Illumin/Outlined Diffuse");
	}

	// Update is called once per frame
	void FixedUpdate () {

		// Check if hand disappears
		if (!isHandExist) {
			hand = GameObject.Find("RigidRoundHand(Clone)");
			isHandExist = (hand != null);
		} else {
			if (hand == null) {
				fixObject();
				isHandExist = false;
				return;
			}
		}

		// 0: Moving Mode, 1: Rotation Mode
		if (gController.GetMode() == 0) {
			rb.angularVelocity = Vector3.zero;

			// Moving starts
			if (checkControlEnabled()) {
				rb.isKinematic = false;
				if (!isControlling) {
					isControlling = true;
					highlightChildren();
					initPalmPosition();
				}
				updatePalmPosition();
				updateTargetPosition();
			}
			// Moving stops
			else {
				if (isControlling) {
					isControlling = false;
					unhighlightChildren();
					fixObject();
				}
				rb.isKinematic = true;
			}

		} else if (gController.GetMode() == 1) {
			rb.velocity = Vector3.zero;
			rb.isKinematic = true;

			// Rotation starts
			if (checkControlEnabled()) {
				if (!isControlling) {
					isControlling = true;
					highlightChildren();
					initPalmRotation();
				}
				updatePalmRotation();
				updateTargetRotation();

			}
			// Rotation stops
			else {
				if (isControlling) {
					isControlling = false;
					unhighlightChildren();
					fixObject();
				}
				axisFixed = 0;
			}

		} else {
			rb.isKinematic = true;
		}
	}

	// Check if hand is flat long enough to enable controlling (moving & rotating)
	bool checkControlEnabled() {
		Hand hand = gController.GetCurrentHand();

		if (gController.IsControlEnabled() &&
			hand != null && hand.GrabStrength < LeapStatic.maxFlatStrength) {

			if (palmFlatDuration > LeapStatic.minFlatTime) return true;
			else palmFlatDuration += 0.02f;

		} else {
			palmFlatDuration = 0;
		}

		return false;
	}

	void initPalmPosition() {
		HandModel hand_model = hand.GetComponent<HandModel>();
		palm_position_ = hand_model.GetPalmPosition();
		init_palm_position_ = palm_position_;
	}

	void updatePalmPosition() {
		HandModel hand_model = hand.GetComponent<HandModel>();
		Vector3 current_palm_position_ = hand_model.GetPalmPosition();
		Vector3 delta_pinch = current_palm_position_ - palm_position_;
		palm_position_ += (1.0f - 0.6f) // positionFiltering
								* delta_pinch;
	}

	void updateTargetPosition() {
		Vector3 target_position = initPosition + (palm_position_ - init_palm_position_);
		Vector3 velocity = (target_position - transform.position) / Time.deltaTime;
		rb.velocity = velocity;
	}

	void initPalmRotation() {
		HandModel hand_model = hand.GetComponent<HandModel>();
		palm_rotation_ = hand_model.GetPalmRotation();
		// Not using @ericchiu
		// rotation_from_palm_ = Quaternion.Inverse(palm_rotation_) * transform.rotation;
		init_palm_rotation_ = palm_rotation_;
	}

	void updatePalmRotation() {
		HandModel hand_model = hand.GetComponent<HandModel>();
		palm_rotation_ = Quaternion.Slerp(palm_rotation_, hand_model.GetPalmRotation(),
									  1.0f - 0.4f); // rotationFiltering);
	}

	void updateTargetRotation() {
		/* Not using the target rotation from Leap Motion @ericchiu */
		/*
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
		*/

		// Check angle difference from initial position
		Quaternion deltaPalmRotation = palm_rotation_ * Quaternion.Inverse(init_palm_rotation_);
		float angle = 0.0f;
		Vector3 axis = Vector3.zero;
		deltaPalmRotation.ToAngleAxis(out angle, out axis);
		angle = convertAngle(angle);

		switch (axisFixed) {
			case 1:
				transform.Rotate((Mathf.Sign(axis.x) * Vector3.right) * 1.2f, Space.World);
				break;
			case 2:
				transform.Rotate((Mathf.Sign(axis.y) * Vector3.up) * 1.2f, Space.World);
				break;
			case 3:
				transform.Rotate((Mathf.Sign(axis.z) * Vector3.forward) * 1.2f, Space.World);
				break;
			case 0:
				axisFixed = getFixRotationAxis(angle, deltaPalmRotation.eulerAngles);
				break;
			default:
				Debug.Log("Fixed Axis value error.");
				break;
		}
	}

	// Get the direction to fix rotation
	// @return 0: none, 1: x axis, 2: y axis, 3: z axis
	int getFixRotationAxis(float angle, Vector3 angles) {
		if (angle > LeapStatic.minStartRotationAngle) {
			angles.x = convertAngle(angles.x);
			angles.y = convertAngle(angles.y);
			angles.z = convertAngle(angles.z);

			if (Mathf.Abs(angles.x) > Mathf.Abs(angles.y)
				&& Mathf.Abs(angles.x) > Mathf.Abs(angles.z)) {
				return 1;
			} else if (Mathf.Abs(angles.y) > Mathf.Abs(angles.x)
				&& Mathf.Abs(angles.y) > Mathf.Abs(angles.z)) {
				return 2;
			} else if (Mathf.Abs(angles.z) > Mathf.Abs(angles.x)
				&& Mathf.Abs(angles.z) > Mathf.Abs(angles.y)) {
				return 3;
			}
		}
		return 0;
	}

	// Fix object angle in 3 directions to 0, 90, 180, or 270.
	void fixObject() {
		Vector3 newAngle = new Vector3();
		newAngle.x = Mathf.Round(transform.eulerAngles.x/90) * 90;
		newAngle.y = Mathf.Round(transform.eulerAngles.y/90) * 90;
		newAngle.z = Mathf.Round(transform.eulerAngles.z/90) * 90;
		transform.eulerAngles = newAngle;
		float angle = Quaternion.Angle(initRotation, transform.rotation);
		if (angle > 50) {
			logRotationAxis();
		}

		initPosition = transform.position;
		initRotation = transform.rotation;
	}

	// 0: x axis, 1: y axis, 2: z axis
	void fixAxis() {
		Quaternion delta = palm_rotation_ * Quaternion.Inverse(Quaternion.identity);
		Vector3 angle = delta.eulerAngles;
		if (angle.x >= 180) angle.x = 360 - angle.x;
		if (angle.y >= 180) angle.y = 360 - angle.y;
		if (angle.z >= 180) angle.z = 360 - angle.z;

		// Debug.Log(delta.eulerAngles);

		Vector3 eAngles = initRotation.eulerAngles;
		if (Mathf.Abs(angle.x) > Mathf.Abs(angle.y)
			&& Mathf.Abs(angle.x) > Mathf.Abs(angle.z)) {
			eAngles.z = transform.eulerAngles.z;
		} else if (Mathf.Abs(angle.y) > Mathf.Abs(angle.x)
			&& Mathf.Abs(angle.y) > Mathf.Abs(angle.z)) {
			eAngles.y = transform.eulerAngles.y;
		} else if (Mathf.Abs(angle.z) > Mathf.Abs(angle.x)
			&& Mathf.Abs(angle.z) > Mathf.Abs(angle.y)) {
			eAngles.x = transform.eulerAngles.x;
		}
		transform.eulerAngles = eAngles;
	}

	void logRotationAxis() {
		switch (axisFixed) {
			case 1:
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "X");
				break;
			case 2:
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Y");
				break;
			case 3:
				SimpleData.WriteDataPoint("Rotate_Object", gameObject.name, "", "", "", "Z");
				break;
			case 0:
				Debug.Log("Logging when rotation is done.");
				break;
			default:
				break;
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

	// Convert angle > 180
	float convertAngle(float angle) {
		if (angle >= 180) return 360 - angle;
		else return angle;
	}
}
