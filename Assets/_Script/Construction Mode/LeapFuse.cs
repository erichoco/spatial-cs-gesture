using UnityEngine;
using System.Collections;

public class LeapFuse : MonoBehaviour {

    private Transform bodyTopAttach = GameObject.Find("Body").transform.FindChild("Body_Top_Attach");

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("The position of body top attach is :"+ bodyTopAttach.GetComponent<FuseBehavior>().isFused);

	}
}
