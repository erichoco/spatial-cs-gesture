﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LeapStatic : MonoBehaviour {
	//parameters
	//one hand
	public static float swipeMinVelocity = 1500f;
	public static float swipeMinDistance = 150f;
	public static float minSwipeInterval = 1f;
	public static float dragVelocity = 10f;
	public static int dragStable = 2;

	// For April 2017 Study @ericchiu
	public static float minStartRotationAngle = 20f;
	public static float minFlatTime = 0.5f;

	//two hands
	public static float grabViewFactor = 400f; //higher -> less sensitive
	public static float connectTimeLimited = 2f; //higher -> velocity lower allowed

	public static float minGripTime = 0.1f;
	public static float maxGripTime = 0.5f;
	public static float minGrabTime = 0.2f;
	public static float maxClapTime = 2f; //higher -> velocity lower allowed


	//bottom panel
	public static int numConstructionObject;
	public static List<String> constructionObject = new List<String>();

	//fuse
	public static List<String> objectName = new List<string>();

	//scene
	public static string currScene;

	public static void CreatePartLeap(int active){
		if(currScene == "tutorial1") {
			CreatePartTutorial1 createPart;
			createPart = (CreatePartTutorial1)GameObject.Find("EventSystem").GetComponent(typeof(CreatePartTutorial1));
			switch (active) {
				case 0:
					createPart.createCone();
					break;
				case 1:
					createPart.createPyr();
					break;
				case 2:
					createPart.createTri();
					break;
				default:
					break;
			}
		} else if (currScene == "tutorial2") {
			CreatePartTutorial2 createPart;
			createPart = (CreatePartTutorial2)GameObject.Find("EventSystem").GetComponent(typeof(CreatePartTutorial2));
			switch (active)
			{
				case 0:
					createPart.createSmallboxYellow();
					break;
				case 1:
					createPart.createTallbox();
					break;
				case 2:
					createPart.createSmallboxBlue();
					break;
				case 3:
					createPart.createBigbox();
					break;
				default:
					break;
			}
		} else if (currScene == "construction") {
			CreatePart createPart;
			createPart = (CreatePart)GameObject.Find("EventSystem").GetComponent(typeof(CreatePart));
			switch (active)
			{
				case 0:
					createPart.createBody();
					break;
				case 1:
					createPart.createCalf();
					break;
				case 2:
					createPart.createTrim();
					break;
				case 3:
					createPart.createToe();
					break;
				case 4:
					createPart.createToeSole();
					break;
				default:
					break;
			}
		} else if (currScene == "axe") {
			CreatePartAxe createPart;
			createPart = (CreatePartAxe)GameObject.Find("EventSystem").GetComponent(typeof(CreatePartAxe));
			switch (active)
			{
				case 0:
					createPart.createHaft();
					break;
				case 1:
					createPart.createHead();
					break;
				case 2:
					createPart.createTopPoint();
					break;
				case 3:
					createPart.createTrapezoid();
					break;
				case 4:
					createPart.createBottomPoint();
					break;
				default:
					break;
			}
		} else if (currScene == "key1") {
			CreatePartKey1 createPart;
			createPart = (CreatePartKey1)GameObject.Find("EventSystem").GetComponent(typeof(CreatePartKey1));
			switch (active)
			{
				case 0:
					createPart.createUprightL();
					break;
				case 1:
					createPart.createUprightT();
					break;
				case 2:
					createPart.createWaluigi();
					break;
				case 3:
					createPart.createWalkingPants();
					break;
				case 4:
					createPart.createUprightRect();
					break;
				default:
					break;
			}
		}
	}

	public static void resetConstructionObject(string sceneName)
	{
		currScene = sceneName;
		constructionObject.Clear();
		objectName.Clear();
		switch (sceneName)
		{
			case "tutorial1":
				//SimpleData.WriteStringToFile("LeapData.txt", Time.time + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 0);//time,one/two,L/R,Gesture,Success,Scene

				constructionObject.Add("Cone");
				constructionObject.Add("Pyr");
				constructionObject.Add("Tri");
				numConstructionObject = 3;

				objectName.Add("tutorial1_box");
				objectName.Add("tutorial1_conePrefab(Clone)");
				objectName.Add("tutorial1_pyrPrefab(Clone)");
				objectName.Add("tutorial1_triPrefab(Clone)");
				break;

			case "tutorial2":
				//SimpleData.WriteStringToFile("LeapData.txt", Time.time + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 1);//time,one/two,L/R,Gesture,Success,Scene

				constructionObject.Add("SmallboxYellow");
				constructionObject.Add("Tallbox");
				constructionObject.Add("SmallboxBlue");
				constructionObject.Add("Bigbox");
				numConstructionObject = 4;

				objectName.Add("tutorial2_longbox");
				objectName.Add("tutorial2_smallbox_yellowPrefab(Clone)");
				objectName.Add("tutorial2_tallboxPrefab(Clone)");
				objectName.Add("tutorial2_smallbox_bluePrefab(Clone)");
				objectName.Add("tutorial2_bigboxPrefab(Clone)");
				break;

			case "rocketBoots":

				constructionObject.Add("Ballfoot");
				constructionObject.Add("Midfoot");
				constructionObject.Add("Trim");
				constructionObject.Add("Toe");
				constructionObject.Add("Calf");
				constructionObject.Add("Widening");
				numConstructionObject = 6;

				objectName.Add("startObject");
				objectName.Add("ballfootPrefab(Clone)");
				objectName.Add("midfootPrefab(Clone)");
				objectName.Add("calf_harderPrefab(Clone)");
				objectName.Add("toeHarderPrefab(Clone)");
				objectName.Add("trimHarderPrefab(Clone)");
				objectName.Add("wideningPrefab(Clone)");
				break;

			case "sledgehammer":

				constructionObject.Add("BottomPointRight");
				constructionObject.Add("BottomPointLeft");
				constructionObject.Add("Haft");
				constructionObject.Add("Head");
				constructionObject.Add("SmallTip");
				constructionObject.Add("Trapezoid");
				constructionObject.Add("TopPointLeft");
				constructionObject.Add("TopPointRight");
				constructionObject.Add("SmallTrapezoid");
				constructionObject.Add("Tip");
				constructionObject.Add("Spike");
				numConstructionObject = 11;

				objectName.Add("startObject");
				objectName.Add("bottom_point_leftPrefab(Clone)");
				objectName.Add("bottom_point_rightPrefab(Clone)");
				objectName.Add("haft_harderPrefab(Clone)");
				objectName.Add("head_harderPrefab(Clone)");
				objectName.Add("small_tipPrefab(Clone)");
				objectName.Add("small_trapezoidPrefab(Clone)");
				objectName.Add("spikePrefab(Clone)");
				objectName.Add("tipPrefab(Clone)");
				objectName.Add("top_point_leftPrefab(Clone)");
				objectName.Add("top_point_rightPrefab(Clone)");
				objectName.Add("trapezoid_harderPrefab(Clone)");
				break;

			case "construction":
				//SimpleData.WriteStringToFile("LeapData.txt", Time.time + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 2);//time,one/two,L/R,Gesture,Success,Scene
				constructionObject.Add("Body");
				constructionObject.Add("Calf");
				constructionObject.Add("Trim");
				constructionObject.Add("Toe");
				constructionObject.Add("ToeSole");
				numConstructionObject = 5;

				objectName.Add("rocket_boots_start");
				objectName.Add("BodyPrefab(Clone)");
				objectName.Add("calfPrefab(Clone)");
				objectName.Add("trimPrefab(Clone)");
				objectName.Add("ToePrefab(Clone)");
				objectName.Add("ToeSolePrefab(Clone)");
				break;

			case "axe":
				//SimpleData.WriteStringToFile("LeapData.txt", Time.time + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 3);//time,one/two,L/R,Gesture,Success,Scene
				constructionObject.Add("Haft");
				constructionObject.Add("Head");
				constructionObject.Add("TopPoint");
				constructionObject.Add("Trapezoid");
				constructionObject.Add("BottomPoint");
				numConstructionObject = 5;

				objectName.Add("startObject");
				objectName.Add("haftPrefab(Clone)");
				objectName.Add("headPrefab(Clone)");
				objectName.Add("top_pointPrefab(Clone)");
				objectName.Add("trapezoidPrefab(Clone)");
				objectName.Add("bottom_pointPrefab(Clone)");
				break;

			case "key1":
				//SimpleData.WriteStringToFile("LeapData.txt", Time.time + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 0 + ";" + 4);//time,one/two,L/R,Gesture,Success,Scene

				constructionObject.Add("UprightL");
				constructionObject.Add("UprightT");
				constructionObject.Add("Waluigi");
				constructionObject.Add("WalkingPants");
				constructionObject.Add("UprightRect");
				numConstructionObject = 5;

				objectName.Add("dangly_T_complete");
				objectName.Add("upright_LPrefab(Clone)");
				objectName.Add("upright_LPrefab(Clone)");
				objectName.Add("waluigiPrefab(Clone)");
				objectName.Add("walking_pantsPrefab(Clone)");
				objectName.Add("upright_rectPrefab(Clone)");
				break;

			default:
				break;
		}

		//recordParams();
	}

	// Get object currently controlling
	public static GameObject GetControlObject() {
		GameObject activeGo = null;
		foreach (string part in objectName) {
			GameObject go = GameObject.Find(part);
			if (go != null && !go.GetComponent<IsFused>().isFused) {
				// Should only be one object in control;
				activeGo = go;
				break;
			}
		}
		return activeGo;
	}

	/* Returns the indices of active objects in @constructionObject */
	public static List<int> GetActiveObjects()
	{
		List<int> activeObjects = new List<int>();
		Transform[] children = GameObject.Find("bottom panel").GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (constructionObject.Contains(child.name) &&
				child.gameObject.GetComponent<Button>().interactable)
			{
				// Debug.Log("[LeapStatic]: " + child.name);
				activeObjects.Add(constructionObject.IndexOf(child.name));
			}
		}
		return activeObjects;
	}

	public static void recordParams()
	{
		SimpleData.WriteStringToFile("LeapData.txt", "The swipeMinVeloctiy is :" + swipeMinVelocity + ".");
		SimpleData.WriteStringToFile("LeapData.txt", "The swipeMinDistance is :" + swipeMinDistance + ".");
		SimpleData.WriteStringToFile("LeapData.txt", "The dragVelocity is :" + dragVelocity + ".");
		SimpleData.WriteStringToFile("LeapData.txt", "The grabViewFactor is :" + grabViewFactor + ".");
		SimpleData.WriteStringToFile("LeapData.txt", "The connectTimeLimited is :" + connectTimeLimited + ".");
	}

	void Start () {

	}

	void Update () {

	}
}