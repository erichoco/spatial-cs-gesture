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

            //tutorial2
            case "tutorial2_longbox":
                list.Add(GameObject.Find("longbox_bigbox_attach"));
                list.Add(GameObject.Find("longbox_smallbox_yellow_attach"));
                list.Add(GameObject.Find("longbox_tallbox_attach"));
                break;
            case "tutorial2_bigboxPrefab(Clone)":
                list.Add(GameObject.Find("bigbox_longbox_attach"));
                list.Add(GameObject.Find("bigbox_smallbox_blue_attach"));
                break;
            case "tutorial2_smallbox_bluePrefab(Clone)":
                list.Add(GameObject.Find("smallbox_blue_bigbox_attach"));
                break;
            case "tutorial2_tallboxPrefab(Clone)":
                list.Add(GameObject.Find("tallbox_longbox_attach"));
                break;
            case "tutorial2_smallbox_yellowPrefab(Clone)":
                list.Add(GameObject.Find("smallbox_yellow_longbox_attach"));
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

            //axe
            case "startObject":
                list.Add(GameObject.Find("shaft_haft_attach"));
                list.Add(GameObject.Find("shaft_trapezoid_attach"));
                break;
            case "bottom_pointPrefab(Clone)":
                list.Add(GameObject.Find("bottom_point_head_attach"));
                break;
            case "trapezoidPrefab(Clone)":
                list.Add(GameObject.Find("trapezoid_head_attach"));
                list.Add(GameObject.Find("trapezoid_shaft_attach"));
                break;
            case "top_pointPrefab(Clone)":
                list.Add(GameObject.Find("top_point_head_attach"));
                break;
            case "headPrefab(Clone)":
                list.Add(GameObject.Find("head_bottom_point_attach"));
                list.Add(GameObject.Find("head_top_point_attach"));
                list.Add(GameObject.Find("head_trapezoid_attach"));
                break;
            case "haftPrefab(Clone)":
                list.Add(GameObject.Find("haft_shaft_attach"));
                break;

            //key 1
            case "dangly_T_complete":
                list.Add(GameObject.Find("dangly_T_upright_L_attach"));
                list.Add(GameObject.Find("dangly_T_upright_T_attach"));
                list.Add(GameObject.Find("dangly_T_walking_pants_attach"));
                break;
            case "upright_rectPrefab(Clone)":
                list.Add(GameObject.Find("upright_rect_walking_pants_attach"));
                break;
            case "upright_LPrefab(Clone)":
                list.Add(GameObject.Find("upright_L_dangly_T_attach"));
                list.Add(GameObject.Find("upright_L_waluigi_attach"));
                break;
            case "upright_TPrefab(Clone)":
                list.Add(GameObject.Find("upright_T_dangly_T_attach"));
                break;
            case "waluigiPrefab(Clone)":
                list.Add(GameObject.Find("waluigi_upright_L_attach"));
                break;
            case "walking_pantsPrefab(Clone)":
                list.Add(GameObject.Find("walking_pants_dangly_T_attach"));
                list.Add(GameObject.Find("walking_pants_upright_rect_attach"));
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
        GameObject tmpObject;
        //find the object controlling and add the object which already be fused into fused dictionary
        foreach (String part in LeapStatic.objectName)
        {
            if (!fused.ContainsKey(part))
            {
                try
                {
                    tmpObject = GameObject.Find(part);
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
        }

        foreach (GameObject controlAttach in controlAttachList)
        {
            foreach (GameObject fusedAttach in fusedAttachList)
            {
                if (fusedAttach.GetComponent<FuseBehavior>().isFused)
                {
                    attached = fusedAttach;
                    attachedFound = true;
                    continue;
                }

                if (Vector3.Distance(controlAttach.transform.position, fusedAttach.transform.position) < 100 && eventSystem.GetComponent<FuseEvent>().ifFuseMapping(fusedAttach,controlAttach))
                {
                    eventSystem.GetComponent<SelectPart>().setSelectedFuseTo(fusedAttach);
                    eventSystem.GetComponent<SelectPart>().setSelectedObject(controlAttach);
                }
                
            }
            //if already attached
            if (attachedFound)
            {
                fusedAttachList.Remove(attached);
                attachedFound = false;
            }
            

        }


        
    }
}
