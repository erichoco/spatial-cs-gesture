using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class LeapFuse : MonoBehaviour {
    //construction.unity scene
    private GameObject heel;
    private GameObject body;
    private GameObject calf;
    private GameObject trim;
    private GameObject toe;
    private GameObject toeSole;

    private GameObject toControl;
    private GameObject eventSystem;

    private GameObject attached;
    private bool attachedFound = false;


    private Dictionary<String,GameObject> fused = new Dictionary<string, GameObject>();
    private List<GameObject> fusedAttachList = new List<GameObject>() ;
    private List<GameObject> controlAttachList = new List<GameObject>();
    

    void AddToList(GameObject parent, List<GameObject> list)
    {
        switch (parent.name)
        {
            //tutorial1
            case "tutorial1_box":
                list.Add(GameObject.Find("box_cone_attach"));
                list.Add(GameObject.Find("box_pyr_attach"));
                list.Add(GameObject.Find("box_tri_attach"));
                break;
            case "tutorial1_conePrefab(Clone)":
                list.Add(GameObject.Find("cone_box_attach"));
                break;
            case "tutorial1_pyrPrefab(Clone)":
                list.Add(GameObject.Find("pyr_box_attach"));
            break;
            case "tutorial1_triPrefab(Clone)":
                list.Add(GameObject.Find("tri_box_attach"));
                break;

            //construction
            case "BodyPrefab(Clone)":
                list.Add(GameObject.Find("Body_Bottom_Attach"));
                list.Add(GameObject.Find("Body_Top_Attach"));
                list.Add(GameObject.Find("Body_Side_Attach"));
                break;
            case "calfPrefab(Clone)":
                list.Add(GameObject.Find("Calf_Bottom_Attach"));
                list.Add(GameObject.Find("Calf_Top_Attach"));
                break;
            case "trimPrefab(Clone)":
                list.Add(GameObject.Find("Top_Trim_Attach"));
                break;
            case "ToePrefab(Clone)":
                list.Add(GameObject.Find("Toe_Bottom_Attach"));
                list.Add(GameObject.Find("Toe_Side_Attach"));
                break;
            case "ToeSolePrefab(Clone)":
                list.Add(GameObject.Find("Sole_Toe_Top_Attach"));
                list.Add(GameObject.Find("Sole_Toe_Side_Attach"));
                break;
            case "rocket_boots_start":
                list.Add(GameObject.Find("Sole_Heel_Top_Attach"));
                list.Add(GameObject.Find("Sole_Heel_Side_Attach"));
                break;
            default:
                break;
        }
    }


    // Use this for initialization
    void Start () {
        eventSystem = GameObject.Find("EventSystem");
    }

   

    void FixedUpdate () {
        //clear the control attach list
        controlAttachList.Clear();
    
        //find the object controlling and add the object which already be fused into fused dictionary
        foreach (String part in LeapStatic.objectName)
        {
            if (!fused.ContainsKey(part))
            {
                try
                {
                    GameObject tmpObject = GameObject.Find(part);
                    if (tmpObject.GetComponent<IsFused>().isFused)
                    {
                        fused.Add(part, tmpObject);
                        
                        AddToList(tmpObject, fusedAttachList);

                        Debug.Log("Add " + tmpObject.name + " to fused list.");

                        toControl = null;
                    }
                    else
                    {
                        toControl = tmpObject;
                        //Debug.Log("The controlled object is :" + toControl.name);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        try//if toControl is null, there is an exception
        {
            AddToList(toControl, controlAttachList);
        }catch(Exception ex) {
            //Debug.Log(ex.Message);
        }

        foreach (GameObject controlAttach in controlAttachList)
        {
            //Debug.Log("This controled attach is :" + controlAttach.name);
            foreach (GameObject fusedAttach in fusedAttachList)
            {
                //if (fusedAttach.name == "Body_Top_Attach" && controlAttach.name == "Calf_Bottom_Attach")
                //Debug.Log("The distance between" + fusedAttach.name + " and " + controlAttach.name + " is :" + Vector3.Distance(fusedAttach.transform.position, controlAttach.transform.position));
                if (fusedAttach.GetComponent<FuseBehavior>().isFused)
                {
                    attached = fusedAttach;
                    attachedFound = true;
                    continue;
                }

                if (Vector3.Distance(controlAttach.transform.position, fusedAttach.transform.position) < 70 && eventSystem.GetComponent<FuseEvent>().ifFuseMapping(fusedAttach,controlAttach))
                {
                    eventSystem.GetComponent<SelectPart>().setSelectedFuseTo(fusedAttach);
                    eventSystem.GetComponent<SelectPart>().setSelectedObject(controlAttach);
                }
                //Debug.Log("The parent of :"+fusedAttach.name+" is : "+fusedAttach.transform.parent.name);
            }
            //if already attached
            if (attachedFound)
            {
                fusedAttachList.Remove(attached);
                attachedFound = false;
            }
            
            //Debug.Log("Attach object " + controlAttach.name + "'s position is " + controlAttach.transform.position.x + " " + controlAttach.transform.position.y + " " + controlAttach.transform.position.z);

        }


        
    }
}
