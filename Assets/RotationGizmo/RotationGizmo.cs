﻿using UnityEngine;
using System.Collections;

public class RotationGizmo : MonoBehaviour
{
	public GameObject mainCamera;
	public GameObject toRotate;
	public bool tutorialOn;

	public GameObject xGizmo;
	public GameObject yGizmo;
	public GameObject zGizmo;
	public int xRots = 0;
	public int yRots = 0;
	public int zRots = 0;

	bool rotating = false;

	void Start ()
	{
		Disable();
	}

	void OnEnable()
	{
		// Reset rotation count.
		xRots = 0;
		yRots = 0;
		zRots = 0;
	}
	
	void Update ()
	{
		// LookAts.
		Vector3 lookToward = mainCamera.transform.position;

		// X
		xGizmo.transform.LookAt(new Vector3(xGizmo.transform.position.x, lookToward.y, lookToward.z));
		Vector3 xTemp = xGizmo.transform.localEulerAngles;
		xTemp.z = 90;
		xGizmo.transform.localEulerAngles = xTemp;

		// Y
		yGizmo.transform.LookAt(new Vector3(lookToward.x, xGizmo.transform.position.y, lookToward.z));

		// Z
		zGizmo.transform.LookAt(new Vector3(lookToward.x, lookToward.y, xGizmo.transform.position.z));
		Vector3 zTemp = zGizmo.transform.localEulerAngles;
		zTemp.z = 90;
		zGizmo.transform.localEulerAngles = zTemp;


		// Raycasts.
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hitInfo = new RaycastHit();
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && (tutorialOn || BatterySystem.GetPower() > 0))
			{
				//Debug.Log(hitInfo.transform.name);
				switch(hitInfo.transform.name)
				{
					case "XUp":
						xRots++;
						if (Mathf.Approximately(xGizmo.transform.localEulerAngles.y, 180f))
							StartCoroutine(Rotate(90f, 0f, 0f));
						else
							StartCoroutine(Rotate(-90f, 0f, 0f));
						break;

					case "XDown":
						xRots++;
						if (Mathf.Approximately(xGizmo.transform.localEulerAngles.y, 180f))
							StartCoroutine(Rotate(-90f, 0f, 0f));
						else
							StartCoroutine(Rotate(90f, 0f, 0f));
						break;

					case "YLeft":
						yRots++;
						StartCoroutine(Rotate(0f, 90f, 0f));
						break;

					case "YRight":
						yRots++;
						StartCoroutine(Rotate(0f, -90f, 0f));
						break;

					case "ZUp":
						zRots++;
						if (Mathf.Approximately(zGizmo.transform.localEulerAngles.y, 270f))
							StartCoroutine(Rotate(0f, 0f, -90f));
						else
							StartCoroutine(Rotate(0f, 0f, 90f));
						break;

					case "ZDown":
						zRots++;
						if (Mathf.Approximately(zGizmo.transform.localEulerAngles.y, 270f))
							StartCoroutine(Rotate(0f, 0f, 90f));
						else
							StartCoroutine(Rotate(0f, 0f, -90f));
						break;

					default:
						break;
				}
			}
		}
        
    }

    IEnumerator Rotate(float x, float y, float z)
	{
		// Integration for battery power.
		if(!tutorialOn) {
			BatterySystem.SubPower(1);
		}

		// Set rotate flag and start rotating.
		// Flag is reset every frame to ensure the check only runs at the end of all queued rotations.
		rotating = true;
		for (int i = 0; i < 45; i++)
		{
			toRotate.transform.Rotate(x / 45f, y / 45f, z / 45f, Space.World);
			rotating = true;
			yield return null;
		}
		rotating = false;
		yield return null;	// Wait a frame to see if another active rotation resets this flag to true.
		if (!rotating)
			StartCoroutine(CheckRotation());
	}

	IEnumerator CheckRotation()
	{
		yield return null;
		
		Vector3 rot = toRotate.transform.eulerAngles;

		// X Rounding
		if (rot.x != 0 && rot.x % 90 < 45)
		{
			rot.x -= (rot.x % 90);
		}
		else if (rot.x != 0 && rot.x % 90 >= 45)
		{
			rot.x += (90 - (rot.x % 90));
		}

		// Y Rounding
		if (rot.y != 0 && rot.y % 90 < 45)
		{
			rot.y -= (rot.y % 90);
		}
		else if (rot.y != 0 && rot.y % 90 >= 45)
		{
			rot.y += (90 - (rot.y % 90));
		}

		// Z Rounding
		if (rot.z != 0 && rot.z % 90 < 45)
		{
			rot.z -= (rot.z % 90);
		}
		else if (rot.z != 0 && rot.z % 90 >= 45)
		{
			rot.z += (90 - (rot.z % 90));
		}

		rot.x = Mathf.RoundToInt(rot.x);
		rot.y = Mathf.RoundToInt(rot.y);
		rot.z = Mathf.RoundToInt(rot.z);

		if (rot.x == 360)
			rot.x = 0;
		if (rot.y == 360)
			rot.y = 0;
		if (rot.z == 360)
			rot.z = 0;

		//Debug.Log(rot);

		toRotate.transform.eulerAngles = rot;
		rotating = false;
	}

	//Warning - will probably break on X and Z rotations if used outside of tutorial
	public void runManualRotation(GameObject objectToRotate, float x, float y, float z) {
		toRotate = objectToRotate;
		StartCoroutine(Rotate(x, y, z));
	}

	public void Disable()
	{
		toRotate = null;
		transform.position = new Vector3(-1000f, -1000f, -1000f);
	}

	public GameObject Enable(GameObject objectToRotate)
	{
		//Debug.Log("Enabled the rotation gizmo.");
		toRotate = objectToRotate;
		transform.position = toRotate.transform.position;
		return objectToRotate;
	}

    public void GestureControl(self_defined_gesture_type gesture,Vector3 velocity)
    {
        switch (gesture)
        {
            case self_defined_gesture_type.rotate_one_hand_x_clockwise:
                xRots++;
                if (Mathf.Approximately(xGizmo.transform.localEulerAngles.y, 180f))
                    StartCoroutine(Rotate(90f, 0f, 0f));
                else
                    StartCoroutine(Rotate(-90f, 0f, 0f));
                break;
            case self_defined_gesture_type.rotate_one_hand_x_counterclockwise:
                xRots++;
                if (Mathf.Approximately(xGizmo.transform.localEulerAngles.y, 180f))
                    StartCoroutine(Rotate(-90f, 0f, 0f));
                else
                    StartCoroutine(Rotate(90f, 0f, 0f));
                break;

            case self_defined_gesture_type.rotate_one_hand_y_clockwise:
                yRots++;
                StartCoroutine(Rotate(0f, 90f, 0f));
                break;

            case self_defined_gesture_type.rotate_one_hand_y_counterclockwise:
                yRots++;
                StartCoroutine(Rotate(0f, -90f, 0f));
                break;

            case self_defined_gesture_type.rotate_one_hand_z_clockwise:
                zRots++;
                if (Mathf.Approximately(zGizmo.transform.localEulerAngles.y, 270f))
                    StartCoroutine(Rotate(0f, 0f, -90f));
                else
                    StartCoroutine(Rotate(0f, 0f, 90f));
                break;

            case self_defined_gesture_type.rotate_one_hand_z_counterclockwise:
                zRots++;
                if (Mathf.Approximately(zGizmo.transform.localEulerAngles.y, 270f))
                    StartCoroutine(Rotate(0f, 0f, 90f));
                else
                    StartCoroutine(Rotate(0f, 0f, -90f));
                break;

            case self_defined_gesture_type.move_one_hand:
                toRotate.transform.position+=velocity*6;
                break;
            default:
                break;
        }
    }
}
