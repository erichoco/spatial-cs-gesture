/**
 * Camera orbit controls.
 */

using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
	const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
	const string INPUT_MOUSE_X = "Mouse X";
	const string INPUT_MOUSE_Y = "Mouse Y";
	const float MIN_CAM_DISTANCE = 80f;
	const float MAX_CAM_DISTANCE = 160f;
	public Vector3 orbitPoint = new Vector3(-70f, 30f, 100f);

	// how fast the camera orbits
	[Range(2f, 15f)]
	public float orbitSpeed = 6f;

	// how fast the camera zooms in and out
	[Range(5f,20f)]
	public float zoomSpeed = 7f;

	// the current distance from pivot point (locked to Vector3.zero)
	float distance = 0f;

    public void rotateLeft()
    {
        transform.RotateAround(new Vector3(-100, 30, 100), new Vector3(0, 1, 0), 25.0f);
    }

    public void rotateRight()
    {
        transform.RotateAround(new Vector3(-100, 30, 100), new Vector3(0, -1, 0), 25.0f);
    }

    public void rotateUp()
    {
        transform.RotateAround(new Vector3(-100, 30, 100), new Vector3(1, 0, 0), 25.0f);

    }

    public void rotateDown()
    {
        transform.RotateAround(new Vector3(-100, 30, 100), new Vector3(-1, 0, 0), 25.0f);

    }

    void Start()
	{
		//distance = Vector3.Distance(transform.position, Vector3.zero);
		distance = Vector3.Distance(transform.position, orbitPoint);
	}

	void LateUpdate()
	{
		// orbits
		if( Input.GetMouseButton(0) )
		{
			float rot_x = Input.GetAxis(INPUT_MOUSE_X);
			float rot_y = -Input.GetAxis(INPUT_MOUSE_Y);
			Vector3 eulerRotation = transform.localRotation.eulerAngles;

			eulerRotation.x += rot_y * orbitSpeed;
			eulerRotation.y += rot_x * orbitSpeed;

			eulerRotation.z = 0f;

			transform.localRotation = Quaternion.Euler( eulerRotation );
			transform.position = (transform.localRotation * (Vector3.forward * -distance)) + orbitPoint;
		}

		if( Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL) != 0f )
		{
			float delta = Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL);

			distance -= delta * (distance/MAX_CAM_DISTANCE) * (zoomSpeed * 1000) * Time.deltaTime;
			distance = Mathf.Clamp(distance, MIN_CAM_DISTANCE, MAX_CAM_DISTANCE);
			transform.position = (transform.localRotation * (Vector3.forward * -distance)) + orbitPoint;
		}

       
    }

    public void GestureControl(self_defined_gesture_type gesture)
    {
        switch (gesture)
        {
            case self_defined_gesture_type.rotate_two_hand_x_clockwise:
                rotateDown();
                break;
            case self_defined_gesture_type.rotate_two_hand_x_counterclockwise:
                rotateUp();
                break;
            case self_defined_gesture_type.rotate_two_hand_y_clockwise:
                rotateLeft();
                break;
            case self_defined_gesture_type.rotate_two_hand_y_counterclockwise:
                rotateRight();
                break;
            default:
                break;
        }
    }

    public void GrabView(float rot_x, float rot_y)
    {
        Vector3 eulerRotation = transform.localRotation.eulerAngles;
        eulerRotation.x += rot_y * orbitSpeed;
        eulerRotation.y += rot_x * orbitSpeed;

        eulerRotation.z = 0f;

        transform.localRotation = Quaternion.Euler(eulerRotation);
        transform.position = (transform.localRotation * (Vector3.forward * -distance)) + orbitPoint;

    }

    public void ScrollView(float delta)
    {
        distance -= delta * (distance / MAX_CAM_DISTANCE) * (zoomSpeed * 1000) * Time.deltaTime;
        distance = Mathf.Clamp(distance, MIN_CAM_DISTANCE, MAX_CAM_DISTANCE);
        transform.position = (transform.localRotation * (Vector3.forward * -distance)) + orbitPoint;
    }
}
