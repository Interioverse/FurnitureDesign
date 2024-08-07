using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGDesign : MonoBehaviour
{
    public Renderer BG;
    public Material myMaterial;
    //public Vector3 localEularAngle;
    public GameObject modelToRotate;
    public Transform projectManager, buttonsParent;
    //public Transform sliderParent;
    //public Slider rotateSlider;
    public Button zero, ninty, oneEighty, twoSeventy;

    private void Start()
    {
        //localEularAngle = transform.localEulerAngles;
        modelToRotate = this.gameObject;
        //sliderParent = rotateSlider.transform.parent;
        //transform.localEulerAngles = Vector3.zero;
        //sliderParent.parent = transform.parent;
        //rotateSlider.onValueChanged.AddListener(OnSliderValueChanged);

        buttonsParent.parent = projectManager;

        zero.onClick.AddListener(() => AngleCall(0));
        ninty.onClick.AddListener(() => AngleCall(90));
        oneEighty.onClick.AddListener(() => AngleCall(180));
        twoSeventy.onClick.AddListener(() => AngleCall(270));
    }

    private void AngleCall(int eularAngle)
    {
        transform.localEulerAngles = new Vector3(0, eularAngle, 0);
    }

    //private void OnSliderValueChanged(float value)
    //{
    //    transform.localEulerAngles = new Vector3(0, rotateSlider.value, 0);
    //}

    internal void AssignBackgroundDesign(Material material)
    {
        BG.material = material;
    }
}
