using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTransformDetails : MonoBehaviour
{
    //[SerializeField] List<float> zValues = new List<float>(0.006f, 0.012f, 0.016f, 0.025f);
    [SerializeField] float[] zValues = { 0.006f, 0.012f, 0.016f, 0.025f };
    Vector3 previousPosition;
    Vector3 previousRotation;
    Vector3 previousScale;

    public Vector3 positionVector;
    public Vector3 rotationVector;
    public Vector3 scaleVector;
    public float zVectorValue = 0.016f;
    public int zIndexValue = 2;
    public bool isTransformChanging;

    private void Start()
    {
        previousPosition = transform.localPosition;
        previousRotation = transform.localEulerAngles;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, zVectorValue);
        previousScale = transform.localScale;
        isTransformChanging = false;

        SaveValues();
    }

    private void Update()
    {
        bool positionChanged = transform.localPosition != previousPosition;
        bool rotationChanged = transform.localEulerAngles != previousRotation;
        bool scaleChanged = transform.localScale != previousScale;

        if (positionChanged || rotationChanged || scaleChanged)
        {
            // At least one transform value has changed
            if (!isTransformChanging)
            {
                isTransformChanging = true;
            }
        }
        else
        {
            // Transform values have stopped changing
            if (isTransformChanging)
            {
                isTransformChanging = false;
            }
        }

        // Update the previous transform values
        previousPosition = transform.localPosition;
        previousRotation = transform.localEulerAngles;
        previousScale = transform.localScale;

        if (isTransformChanging && (AppManager.Instance.inputFieldFocused == false))
        {
            SaveValues();
        }
        else
        {
            if (transform.localScale.z != zVectorValue)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, zVectorValue);
            }
        }

        if (transform.localScale.x > 2.4f)
        {
            scaleVector = new Vector3(2.4f, scaleVector.y, scaleVector.z);
            transform.localScale = scaleVector;
        }
        else if (transform.localScale.x < 0.02f)
        {
            scaleVector = new Vector3(0.02f, scaleVector.y, scaleVector.z);
            transform.localScale = scaleVector;
        }
        if (transform.localScale.y > 2.4f)
        {
            scaleVector = new Vector3(scaleVector.x, 2.4f, scaleVector.z);
            transform.localScale = scaleVector;
        }
        else if (transform.localScale.y < 0.02f)
        {
            scaleVector = new Vector3(scaleVector.x, 0.02f, scaleVector.z);
            transform.localScale = scaleVector;
        }
    }

    public void SaveValues()
    {
        positionVector = transform.localPosition;
        rotationVector = transform.localEulerAngles;
        scaleVector = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //scaleVector = new Vector3(transform.localScale.x, transform.localScale.y, zValues[zIndexValue]);
        ////scaleVector = transform.localScale;

        AppManager.Instance.ObserveChanges(positionVector, rotationVector, scaleVector);
    }

    public void SetValues()
    {
        positionVector = transform.localPosition;
        rotationVector = transform.localEulerAngles;
        scaleVector = transform.localScale;
    }
}
