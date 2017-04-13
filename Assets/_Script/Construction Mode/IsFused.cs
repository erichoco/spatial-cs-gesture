using UnityEngine;
using System.Collections;

public class IsFused : MonoBehaviour {

	public bool isFused;
	public string locationTag;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void OnFuse () {
		isFused = true;

		CustomGrabbableObject custom = GetComponent<CustomGrabbableObject>();
		if (custom != null) custom.enabled = false;

		Collider collider = GetComponent<BoxCollider>();
		if (collider != null) collider.enabled = false;

		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null) Destroy(rb);
	}
}
