using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class GestureRecognizer : MonoBehaviour {

	private GameObject handController;

	// Gripping States (grip => GrabIntensity > 0.95)
	private bool isHandGripped;
	private float gripDuration;

	private bool isMenuOpened;

	// Grabbing States
	private float grabDuration;

	// Clapping States
	private const int POS_LIST_SIZE = 5;
	private List<Vector3> leftHandPosList;
	private List<Vector3> rightHandPosList;
	private Vector3 prevLeftHandPos;
	private Vector3 prevRightHandPos;
	private float clapDuration;
	private bool isHandClapped;

	// Gesture




	// Use this for initialization
	void Start () {
		handController = GameObject.Find("HandController");
		isHandGripped = false;
		gripDuration = 0f;

		grabDuration = 0f;

		leftHandPosList = new List<Vector3>(POS_LIST_SIZE);
		rightHandPosList = new List<Vector3>(POS_LIST_SIZE);
		prevLeftHandPos = new Vector3(0, 0, 0);
		prevRightHandPos = new Vector3(0, 0, 0);
		clapDuration = 0f;
		isHandClapped = false;

		isMenuOpened = false;
	}

	// Update is called once per frame
	void Update () {
	}

	public GESTURE_TYPE Recognize(Frame frame)
	{
		HandList hands = frame.Hands;
		GestureList gestures = frame.Gestures();

		GESTURE_TYPE gestureType = GESTURE_TYPE.NONE;
		switch (frame.Hands.Count)
		{
			// Two hands detected: Opening/Closing Menu, Rotating View, Connecting
			case 2:
			if (gripReleased(hands))
			{
				gestureType = (isMenuOpened)? GESTURE_TYPE.CLOSE_MENU:
											  GESTURE_TYPE.OPEN_MENU;
				isMenuOpened = !isMenuOpened;
			}
			else if (grabStarted(hands))
				gestureType = GESTURE_TYPE.VIEW;

			else if (clapDone(hands))
				gestureType = GESTURE_TYPE.CONNECT;

			break;

			// Singe hand detected: Selecting from Menu, Moving, Rotating (X/Y/Z) Object
			case 1:
			break;

			default:
			break;
		}

		return gestureType;
	}


	private bool isGripping(Hand hand)
	{
		return hand.GrabStrength > 0.95;
	}
	private bool isGrabbing(Hand hand)
	{
		return hand.GrabStrength > 0.1 && hand.GrabStrength < 0.95;
	}

	private bool isClapping(Vector3 curLeftHandPos, Vector3 curRightHandPos)
	{
		Vector3 stickDirection = prevRightHandPos - prevLeftHandPos;
		Vector3 leftDirection = curLeftHandPos - prevLeftHandPos;
		Vector3 rightDirection = curRightHandPos - prevRightHandPos;
		Vector3 connectDirection = leftDirection + rightDirection;
		int factor = 5;

		prevLeftHandPos = curLeftHandPos;
		prevRightHandPos = curRightHandPos;

		return ((Math.Abs(stickDirection.y) < Math.Abs(stickDirection.x) || Math.Abs(stickDirection.y) < Math.Abs(stickDirection.z)) &&
			(Math.Abs(leftDirection.y) < Math.Abs(leftDirection.x) || Math.Abs(leftDirection.y) < Math.Abs(leftDirection.z)) &&
			(Math.Abs(rightDirection.y) < Math.Abs(rightDirection.x) || Math.Abs(rightDirection.y) < Math.Abs(rightDirection.z)) &&
			(Math.Abs(leftDirection.x) + Math.Abs(leftDirection.z)) > factor &&
			(Math.Abs(rightDirection.x) + Math.Abs(rightDirection.z)) > factor &&
			(connectDirection.x + connectDirection.z) < factor);
	}

	private Vector3 listAverage(List<Vector3> l)
	{
		Vector3 sum = new Vector3(0, 0, 0);
		foreach (Vector3 i in l) sum += i;
		return sum / l.Count;
	}

	private List<Vector3> getCurHandPos(HandList hands)
	{
		List<Vector3> curHandPos = new List<Vector3>();

		Hand leftHand = hands[0], rightHand = hands[1];
		if (hands[1].IsLeft)
		{
			leftHand = hands[1];
			rightHand = hands[0];
		}

		leftHandPosList.Add(handController.transform.TransformPoint(leftHand.PalmPosition.ToUnityScaled()));
		rightHandPosList.Add(handController.transform.TransformPoint(rightHand.PalmPosition.ToUnityScaled()));
		if (leftHandPosList.Count != rightHandPosList.Count)
		{
			leftHandPosList.Clear();
			rightHandPosList.Clear();
		}
		else if (leftHandPosList.Count == POS_LIST_SIZE)
		{
			curHandPos.Add(listAverage(leftHandPosList));
			curHandPos.Add(listAverage(rightHandPosList));
			leftHandPosList.RemoveAt(0);
			rightHandPosList.RemoveAt(0);
		}
		return curHandPos;
	}

	private bool gripReleased(HandList hands)
	{
		if (isGripping(hands[0]) && isGripping(hands[1]))
			gripDuration += 0.02f;

		if (!isHandGripped)
		{
			if (gripDuration > LeapStatic.minGripTime)
			{
				isHandGripped = true;
				Debug.Log("[Gesture] Grip Enabled");
			}
		}
		else
		{
			if (!isGripping(hands[0]) && !isGripping(hands[1]))
			{
				Debug.Log("[Gesture] Grip Released");
				gripDuration = 0f;
				isHandGripped = false;
				return gripDuration < LeapStatic.maxGripTime;
			}
		}
		return false;
	}

	private bool grabStarted(HandList hands)
	{
		if (isGrabbing(hands[0]) && isGrabbing(hands[1]))
		{
			grabDuration += 0.02f;
			return grabDuration > LeapStatic.minGrabTime;
		}
		grabDuration = 0f;
		return false;
	}

	private bool clapDone(HandList hands)
	{
		List<Vector3> curHandPos = getCurHandPos(hands);

		if (curHandPos.Count == 2)
		{
			Vector3 curLeftHandPos = curHandPos[0], curRightHandPos = curHandPos[1];

			if (isHandClapped)
			{
				// Clap released
				if (Vector3.Distance(curLeftHandPos, curRightHandPos) > 20f)
				{
					isHandClapped = false;
					clapDuration = 0f;
					return true;
				}
			}
			else
			{
				if (isClapping(curLeftHandPos, curRightHandPos))
					clapDuration += 0.02f;

				// Clapped
				if (Vector3.Distance(curLeftHandPos, curRightHandPos) < 10f)
				{
					if (clapDuration <= LeapStatic.maxClapTime)
						isHandClapped = true;
					clapDuration = 0f;
				}
			}
		}

		return false;
	}
}
