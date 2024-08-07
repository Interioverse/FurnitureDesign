using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFaceSnap : MonoBehaviour
{
    [SerializeField] float snapDistance;
    private bool _isSnapped = false;
    private Transform _otherTransform;
    GetClick getClick;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is already snapped to something else
        if (_isSnapped) return;

        // Check if the other object is a cube
        var otherObject = other.gameObject;
        if (otherObject.tag != "Selectable") return;

        getClick = otherObject.GetComponent<GetClick>();
        // Get the closest vertex on each cube
        var thisMesh = GetComponent<MeshFilter>().sharedMesh;
        var otherMesh = otherObject.GetComponent<MeshFilter>().sharedMesh;

        var thisVertices = thisMesh.vertices;
        var otherVertices = otherMesh.vertices;

        var thisClosestVertex = transform.TransformPoint(thisVertices[0]);
        var otherClosestVertex = otherObject.transform.TransformPoint(otherVertices[0]);

        foreach (var vertex in thisVertices)
        {
            var worldPosition = transform.TransformPoint(vertex);
            var otherWorldPosition = otherObject.transform.TransformPoint(otherVertices[0]);

            if (Vector3.Distance(worldPosition, otherWorldPosition) < Vector3.Distance(thisClosestVertex, otherClosestVertex))
            {
                thisClosestVertex = worldPosition;
                otherClosestVertex = otherObject.transform.TransformPoint(otherVertices[0]);
            }
        }

        //// Get the relative direction of the collision
        //Vector3 relativeDirection = transform.InverseTransformDirection(other.transform.position - transform.position);

        //// Check which axis is closest to the relative direction
        //float xAbs = Mathf.Abs(relativeDirection.x);
        //float yAbs = Mathf.Abs(relativeDirection.y);
        //float zAbs = Mathf.Abs(relativeDirection.z);

        //if (xAbs > yAbs && xAbs > zAbs)
        //{
        //    Debug.Log("Trigger happened on X axis");
        //    Debug.Log(transform.localScale.x);
        //}
        //else if (yAbs > xAbs && yAbs > zAbs)
        //{
        //    Debug.Log("Trigger happened on Y axis");
        //    Debug.Log(transform.localScale.y);
        //}
        //else
        //{
        //    Debug.Log("Trigger happened on Z axis");
        //    Debug.Log(transform.localScale.z);
        //}

        foreach (var vertex in otherVertices)
        {
            var worldPosition = otherObject.transform.TransformPoint(vertex);
            var thisWorldPosition = transform.TransformPoint(thisVertices[0]);

            if (Vector3.Distance(worldPosition, thisWorldPosition) < Vector3.Distance(otherClosestVertex, thisClosestVertex))
            {
                otherClosestVertex = worldPosition;
                thisClosestVertex = transform.TransformPoint(thisVertices[0]);
            }
        }

        // Snap the object if it's close enough
        if (Vector3.Distance(thisClosestVertex, otherClosestVertex) < snapDistance)
        {
            // Align the vertices
            transform.position = transform.position - thisClosestVertex + otherClosestVertex;

            otherObject.transform.position = getClick.positionVector;
            getClick.runtimeTransformHandle.axes = HandleAxes.NONE;
            // Set the objects as snapped
            _isSnapped = true;
            _otherTransform = otherObject.transform;
            StartCoroutine(LockModel());
        }
    }

    IEnumerator LockModel()
    {
        gameObject.isStatic = true;
        yield return new WaitForSeconds(1);
        gameObject.isStatic = false;
        getClick.runtimeTransformHandle.axes = HandleAxes.XYZ;
    }

    private void OnTriggerExit(Collider other)
    {
        // Reset the objects if they were previously snapped
        if (_isSnapped && _otherTransform == other.transform)
        {
            _isSnapped = false;
            _otherTransform = null;
        }
    }
}