using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithDistance : MonoBehaviour
{
    public Transform cameraTransform;
    public float minScale = 0.1f;
    public float maxScale = 1f;
    public float[] distanceValues = new float[] { 2f, 5f, 8f, 11f, 15f };
    public float[] scaleValues = new float[] { 1f, 0.7f, 0.5f, 0.3f, 0.1f };

    float currentDistance;
    float currentScale;

    private void Start()
    {
        currentScale = transform.localScale.x;
    }

    private void Update()
    {
        currentDistance = Vector3.Distance(transform.position, cameraTransform.position);

        for (int i = 0; i < distanceValues.Length; i++)
        {
            if (currentDistance <= distanceValues[i])
            {
                currentScale = Mathf.Lerp(maxScale, minScale, scaleValues[i]);
                break;
            }
        }

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}


//public class ScaleWithDistance : MonoBehaviour
//{
//    public float scaleSpeed = 0.2f;
//    public float minScale = 0.2f;
//    public float maxScale = 1f;
//    public float distanceThreshold = 10f;

//    public float currentDistance;
//    public float currentScale;
//    public bool isCameraMoving;

//    [SerializeField] SmoothOrbitCam cameraTransform;

//    private void Start()
//    {
//        currentScale = transform.localScale.x;
//        isCameraMoving = false;
//    }

//    private void Update()
//    {
//        if (isCameraMoving && !cameraTransform.EnableOrbiting)
//        {
//            currentDistance = Vector3.Distance(transform.position, cameraTransform.transform.position);

//            if (currentDistance > distanceThreshold)
//            {
//                currentScale = Mathf.Clamp(currentScale + (scaleSpeed * Time.deltaTime), minScale, maxScale);
//            }
//            else
//            {
//                currentScale = Mathf.Clamp(currentScale - (scaleSpeed * Time.deltaTime), minScale, maxScale);
//            }

//            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
//        }
//    }

//    private void LateUpdate()
//    {
//        if (cameraTransform.transform.hasChanged)
//        {
//            isCameraMoving = true;
//        }
//        else
//        {
//            isCameraMoving = false;
//        }

//        cameraTransform.transform.hasChanged = false;
//    }
//}



//public class ScaleWithDistance : MonoBehaviour
//{
//    public Transform cameraTransform;
//    public float scaleSpeed = 0.2f;
//    public float minScale = 0.5f;
//    public float maxScale = 1f;
//    public float distanceThreshold = 10f;

//    public float currentDistance;
//    public float currentScale;

//    private void Start()
//    {
//        currentScale = transform.localScale.x;
//    }

//    private void Update()
//    {
//        currentDistance = Vector3.Distance(transform.position, cameraTransform.position);

//        if (currentDistance > distanceThreshold)
//        {
//            currentScale = Mathf.Clamp(currentScale + (scaleSpeed * Time.deltaTime), minScale, maxScale);
//        }
//        else
//        {
//            currentScale = Mathf.Clamp(currentScale - (scaleSpeed * Time.deltaTime), minScale, maxScale);
//        }

//        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
//    }
//}

