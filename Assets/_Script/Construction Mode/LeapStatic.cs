using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LeapStatic : MonoBehaviour {
    //parameters
    //one hand
    public static float swipeMinVelocity = 800f;
    public static float swipeMinDistance = 150f;
    public static float dragVelocity = 10f;
    public static int dragStable = 2;

    //two hands
    public static float grabViewFactor = 500f; //higher -> less sensitive
    public static float connectTimeLimited = 2f; //higher -> velocity lower allowed

    //bottom panel
    public static int numConstructionObject;
    public static List<String> constructionObject = new List<String>();

    //fuse
    public static List<String> objectName = new List<string>();

    //scene
    public static string currScene;


    public static void CreatePartLeap(int active){
        if(currScene == "tutorial1")
        {
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
        }
        else if (currScene == "construction")
        {
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
                constructionObject.Add("Cone");
                constructionObject.Add("Pyr");
                constructionObject.Add("Tri");
                numConstructionObject = 3;

                objectName.Add("tutorial1_box");
                objectName.Add("tutorial1_conePrefab(Clone)");
                objectName.Add("tutorial1_pyrPrefab(Clone)");
                objectName.Add("tutorial1_triPrefab(Clone)");

                break;
            case "construction":
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
            default:
                break;

        }

        recordParams();
    }

    //public static void dataRecord(string message, string filename)
    //{
    //    File.WriteAllText(filename, message);
    //}

    public static void recordParams()
    {
        SimpleData.WriteStringToFile("LeapData.txt", "The swipeMinVeloctiy is :" + swipeMinVelocity + ".");
        SimpleData.WriteStringToFile("LeapData.txt", "The swipeMinDistance is :" + swipeMinDistance + ".");
        SimpleData.WriteStringToFile("LeapData.txt", "The dragVelocity is :" + dragVelocity + ".");
        SimpleData.WriteStringToFile("LeapData.txt", "The grabViewFactor is :" + grabViewFactor + ".");
        SimpleData.WriteStringToFile("LeapData.txt", "The connectTimeLimited is :" + connectTimeLimited + ".");
        //dataRecord("The swipeMinVeloctiy is :" + swipeMinVelocity+, filename);
        //dataRecord("The swipeMinDistance is :" + swipeMinDistance + ".\n", filename);
        //dataRecord("The dragVelocity is :" + dragVelocity + ".\n", filename);
        //dataRecord("The grabViewFactor is :" + grabViewFactor + ".\n", filename);
        //dataRecord("The connectTimeLimited is :" + connectTimeLimited + ".\n", filename);
    }

    // Use this for initialization
    void Start () {


    }

	// Update is called once per frame
	void Update () {
        //Debug.Log("Now the scene is :" + SceneManager.SetActiveScene);
        //LeapStatic.resetConstructionObject();
    }
}
