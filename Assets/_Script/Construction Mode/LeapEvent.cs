using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeapEvent : MonoBehaviour {

    private bool underSetting;
    private bool oneHandSetting;

    private Slider slider1;
    private Slider slider2;
    private Slider slider3;

    private GameObject sliderRotateVelocity;
    private GameObject sliderRotateDistance;
    private GameObject sliderTranslateSpeed;
    private GameObject sliderFuseSensitivity;
    private GameObject sliderFuseSpeed;
    private GameObject sliderDragSpeed;
    private GameObject switchButton;
    private GameObject motionSetting;

    public void switchHandMode()
    {
        if (oneHandSetting)
        {
            clickSetting();
            oneHandSetting = false;
            clickSetting();
        }
        else
        {
            clickSetting();
            oneHandSetting = true;
            clickSetting();
        }

    }

    public void clickSetting()
    {
        if (underSetting)
        {
            if (oneHandSetting)
            {
                sliderRotateVelocity.SetActive(false);
                sliderRotateDistance.SetActive(false);
                sliderTranslateSpeed.SetActive(false);

                LeapStatic.dragVelocity = 10 * slider1.value + 5;
                LeapStatic.swipeMinDistance = 300 * slider2.value + 100;
                LeapStatic.swipeMinVelocity = 800 * slider3.value + 400;
            }
            else
            {
                sliderFuseSensitivity.SetActive(false);
                sliderFuseSpeed.SetActive(false);
                sliderDragSpeed.SetActive(false);

                LeapStatic.grabViewFactor = 1300 - slider1.value * 1000;
                LeapStatic.connectTimeLimited = 3.5f - slider2.value * 3;
            }
            switchButton.SetActive(false);
            underSetting = false;
        }
        else
        {
            if (oneHandSetting)
            {
                sliderRotateVelocity.SetActive(true);
                sliderRotateDistance.SetActive(true);
                sliderTranslateSpeed.SetActive(true);

                slider3 = (Slider)sliderRotateVelocity.GetComponent(typeof(Slider));
                slider2 = (Slider)sliderRotateDistance.GetComponent(typeof(Slider));
                slider1 = (Slider)sliderTranslateSpeed.GetComponent(typeof(Slider));

                slider1.value = (LeapStatic.dragVelocity - 5) / 10;
                slider2.value = (LeapStatic.swipeMinDistance - 100) / 300;
                slider3.value = (LeapStatic.swipeMinVelocity - 400) / 800;
            }
            else
            {
                sliderFuseSensitivity.SetActive(true);
                sliderFuseSpeed.SetActive(true);
                sliderDragSpeed.SetActive(true);

                slider1 = (Slider)sliderFuseSensitivity.GetComponent(typeof(Slider));
                slider2 = (Slider)sliderFuseSpeed.GetComponent(typeof(Slider));
                slider3 = (Slider)sliderDragSpeed.GetComponent(typeof(Slider));

                slider3.value = (1300 - LeapStatic.grabViewFactor) / 1000;
                slider2.value = (3.5f - LeapStatic.connectTimeLimited) / 3;
            }
            switchButton.SetActive(true);
            underSetting = true;
        }
    }

	// Use this for initialization
	void Start () {
        motionSetting = GameObject.Find("LeapSetting");
        switchButton = GameObject.Find("Switch");
        sliderRotateVelocity = GameObject.Find("Slider_Rotate_Veloctiy");
        sliderRotateDistance = GameObject.Find("Slider_Rotate_Distance");
        sliderTranslateSpeed = GameObject.Find("Slider_Translate_Speed");
        sliderFuseSensitivity = GameObject.Find("Slider_Fuse_Sensitivity");
        sliderFuseSpeed = GameObject.Find("Slider_Fuse_Speed");
        sliderDragSpeed = GameObject.Find("Slider_Drag_Speed");

        switchButton.SetActive(false);
        sliderRotateVelocity.SetActive(false);
        sliderRotateDistance.SetActive(false);
        sliderTranslateSpeed.SetActive(false);
        sliderFuseSensitivity.SetActive(false);
        sliderFuseSpeed.SetActive(false);
        sliderDragSpeed.SetActive(false);

        oneHandSetting = true;
        underSetting = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
