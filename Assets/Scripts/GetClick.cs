using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClick : MonoBehaviour
{
    public RuntimeTransformHandle runtimeTransformHandle;
    public Vector3 positionVector;
    public Vector3 rotationVector;
    public Vector3 scaleVector;

    public bool isTansperent;
    //[SerializeField] DesignManager designManager;
    [SerializeField] float catchTimeWindow = 0.5f;
    float lastClickTime = 0;

    private void Awake()
    {
        runtimeTransformHandle = FindObjectOfType<RuntimeTransformHandle>();
        //designManager = FindObjectOfType<DesignManager>();
        SaveValues();
    }

    private void Update()
    {
        SaveValues();
    }

    private void OnMouseDown()
    {
        if (Time.time - lastClickTime < catchTimeWindow)
        {
            runtimeTransformHandle.AssignTarget(this.transform);
        }
        lastClickTime = Time.time;
    }

    public void SaveValues()
    {
        positionVector = transform.localPosition;
        rotationVector = transform.localEulerAngles;
        scaleVector = transform.localScale;
    }
}
