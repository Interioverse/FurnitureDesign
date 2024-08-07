using System.Collections.Generic;
using UnityEngine;
using System;

public class CubeFacePlanes : MonoBehaviour
{
    public GameObject cube;  // Reference to the cube object
    public GameObject planePrefab;  // Prefab for the plane
    [SerializeField] float width = 0.0035f;
    [SerializeField] float planeLocalPos = 0.505f;
    [SerializeField] float quadScale = 1.01f;
    public bool planesCreated = true;
    public List<GameObject> sideFaces;

    public void ApplySunmica()
    {
        if (cube.transform.childCount > 0)
        {
            planesCreated = true;
        }
        else
        {
            planesCreated = false;
        }

        if (!planesCreated)
        {
            CreatePlanes();
            planesCreated = true;
        }
        else
        {
            AppManager.Instance.CopyPaste("Sunmica is already applied");
            Invoke("ClearText", 1);
        }
    }

    //public Transform[] childTransforms;
    internal void RemoveSunmice()
    {
        if (cube.transform.childCount > 0)
        {
            for (int i = cube.transform.childCount - 1; i >= 0; i--)
            {
                Transform childTransform = cube.transform.GetChild(i);
                Destroy(childTransform.gameObject);
            }
            planesCreated = false;
            //sideFaces.Clear();
        }
        else
        {
            AppManager.Instance.CopyPaste("Sunmica is not applied");
            Invoke("ClearText", 1);
        }
    }

    void ClearText()
    {
        AppManager.Instance.DeactivateCopyPastePanel();
    }

    //private void Start()
    //{
    //    ApplySunmica();
    //}

    void CreatePlanes()
    {
        // Get the mesh filter component of the cube
        MeshFilter cubeMeshFilter = cube.GetComponent<MeshFilter>();
        if (cubeMeshFilter != null)
        {
            // Get the cube's mesh
            Mesh cubeMesh = cubeMeshFilter.mesh;

            // Get the vertices and triangles of the cube's mesh
            Vector3[] vertices = cubeMesh.vertices;
            int[] triangles = cubeMesh.triangles;

            // Create a dictionary to store face normals as keys
            // and the corresponding triangles as values
            Dictionary<Vector3, List<int>> faceDictionary = new Dictionary<Vector3, List<int>>();

            // Iterate through each triangle of the cube
            for (int i = 0; i < triangles.Length; i += 3)
            {
                // Get the vertices of the current triangle
                Vector3 v1 = cube.transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v2 = cube.transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v3 = cube.transform.TransformPoint(vertices[triangles[i + 2]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

                // Check if the face normal already exists in the dictionary
                if (faceDictionary.ContainsKey(faceNormal))
                {
                    // If it exists, add the current triangle to the list of triangles for that normal
                    faceDictionary[faceNormal].Add(i);
                }
                else
                {
                    // If it doesn't exist, create a new entry in the dictionary
                    faceDictionary.Add(faceNormal, new List<int> { i });
                }
            }

            // Iterate through the unique face normals in the dictionary
            foreach (Vector3 faceNormal in faceDictionary.Keys)
            {
                // Get the list of triangles for the current face normal
                List<int> triangleIndices = faceDictionary[faceNormal];

                // Calculate the center of the face using the vertices of the face
                Vector3 center = Vector3.zero;
                foreach (int triangleIndex in triangleIndices)
                {
                    Vector3 v1 = cube.transform.TransformPoint(vertices[triangles[triangleIndex + 0]]);
                    Vector3 v2 = cube.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
                    Vector3 v3 = cube.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);
                    center += (v1 + v2 + v3) / 3f;
                }
                center /= triangleIndices.Count;

                // Create a plane at the center of the face
                GameObject cubeFace = Instantiate(planePrefab, center, Quaternion.identity);

                /* Rotation if the cubeFace is a cube */
                //// Set the rotation of the plane to align with the face normal
                //cubeFace.transform.rotation = Quaternion.LookRotation(cube.transform.TransformDirection(faceNormal));

                // Set the scale of the plane to match the size of the face
                Vector3 scale = new Vector3(
                    Vector3.Distance(cube.transform.TransformPoint(vertices[triangles[triangleIndices[0]]]), cube.transform.TransformPoint(vertices[triangles[triangleIndices[0] + 1]])),
                    Vector3.Distance(cube.transform.TransformPoint(vertices[triangles[triangleIndices[0] + 1]]), cube.transform.TransformPoint(vertices[triangles[triangleIndices[0] + 2]])),
                    width
                );

                //Vector3 scaleFactor = new Vector3(0.005f, 0.005f, 0);
                //cubeFace.transform.localScale = scale;

                // Make the plane a child of the cube
                cubeFace.transform.parent = cube.transform;
                cubeFace.transform.localScale = new Vector3(quadScale, quadScale, quadScale);

                string sideName = GetSideName(faceNormal);
                cubeFace.name = sideName + " Face";

                cubeFace.transform.rotation = Quaternion.LookRotation(cube.transform.TransformDirection(faceNormal));

                // Reverse the Y rotation for all faces except "Top" and "Bottom"
                Vector3 currentRotation = cubeFace.transform.rotation.eulerAngles;
                if (!cubeFace.name.Contains("Top") && !cubeFace.name.Contains("Bottom"))
                {
                    cubeFace.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + 180f, currentRotation.z);
                }
                else
                {
                    if (cubeFace.name.Contains("Top"))
                    {
                        Vector3 currentRot = cubeFace.transform.rotation.eulerAngles;
                        cubeFace.transform.rotation = Quaternion.Euler(90f, currentRot.y, currentRot.z);
                    }
                    else
                    {
                        // Reverse the X rotation for "Top" and "Bottom" faces
                        cubeFace.transform.rotation = Quaternion.Euler(currentRotation.x + 180f, currentRotation.y, currentRotation.z);
                    }
                }

                if (cubeFace.name.Contains("Front"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, 0, planeLocalPos);
                }
                else if (cubeFace.name.Contains("Back"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, 0, -planeLocalPos);
                }
                else if (cubeFace.name.Contains("Top"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, planeLocalPos, 0);
                }
                else if (cubeFace.name.Contains("Bottom"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, -planeLocalPos, 0);
                }
                else if (cubeFace.name.Contains("Left"))
                {
                    cubeFace.transform.localPosition = new Vector3(-planeLocalPos, 0, 0);
                }
                else if (cubeFace.name.Contains("Right"))
                {
                    cubeFace.transform.localPosition = new Vector3(planeLocalPos, 0, 0);
                }

                /* Uncomment this later - if Cube face is a cube not Quad */
                //if (cubeFace.name.Contains("Bottom") || cubeFace.name.Contains("Top"))
                //{
                //    cubeFace.transform.localScale += new Vector3((cubeFace.transform.localScale.z * 2), 0, 0);
                //}
                /* --------------------------- */
            }
        }
    }

    string GetSideName(Vector3 faceNormal)
    {
        if (faceNormal == Vector3.up)
            return "Top";
        else if (faceNormal == Vector3.down)
            return "Bottom";
        else if (faceNormal == Vector3.forward)
            return "Front";
        else if (faceNormal == Vector3.back)
            return "Back";
        else if (faceNormal == Vector3.left)
            return "Left";
        else if (faceNormal == Vector3.right)
            return "Right";

        return "Unknown";
    }

    GameObject cubeFace;

    public GameObject CreatePlaneFor(string faceName)
    {
        // Get the mesh filter component of the cube
        MeshFilter cubeMeshFilter = cube.GetComponent<MeshFilter>();
        if (cubeMeshFilter != null)
        {
            // Get the cube's mesh
            Mesh cubeMesh = cubeMeshFilter.mesh;

            // Get the vertices and triangles of the cube's mesh
            Vector3[] vertices = cubeMesh.vertices;
            int[] triangles = cubeMesh.triangles;

            // Create a dictionary to store face normals as keys
            // and the corresponding triangles as values
            Dictionary<Vector3, List<int>> faceDictionary = new Dictionary<Vector3, List<int>>();

            // Iterate through each triangle of the cube
            for (int i = 0; i < triangles.Length; i += 3)
            {
                // Get the vertices of the current triangle
                Vector3 v1 = cube.transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v2 = cube.transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v3 = cube.transform.TransformPoint(vertices[triangles[i + 2]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

                // Check if the face normal matches the desired face
                if (GetSideName(faceNormal).Equals(faceName, StringComparison.OrdinalIgnoreCase))
                {
                    // Check if the face normal already exists in the dictionary
                    if (faceDictionary.ContainsKey(faceNormal))
                    {
                        // If it exists, add the current triangle to the list of triangles for that normal
                        faceDictionary[faceNormal].Add(i);
                    }
                    else
                    {
                        // If it doesn't exist, create a new entry in the dictionary
                        faceDictionary.Add(faceNormal, new List<int> { i });
                    }
                }
            }

            // Iterate through the unique face normals in the dictionary
            foreach (Vector3 faceNormal in faceDictionary.Keys)
            {
                // Get the list of triangles for the current face normal
                List<int> triangleIndices = faceDictionary[faceNormal];

                // Calculate the center of the face using the vertices of the face
                Vector3 center = Vector3.zero;
                foreach (int triangleIndex in triangleIndices)
                {
                    Vector3 v1 = cube.transform.TransformPoint(vertices[triangles[triangleIndex]]);
                    Vector3 v2 = cube.transform.TransformPoint(vertices[triangles[triangleIndex + 1]]);
                    Vector3 v3 = cube.transform.TransformPoint(vertices[triangles[triangleIndex + 2]]);
                    center += (v1 + v2 + v3) / 3f;
                }
                center /= triangleIndices.Count;

                // Create a plane at the center of the face
                cubeFace = Instantiate(planePrefab, center, Quaternion.identity);
                //cubeFace.name = cube + "_" + System.DateTime.Now;
                // Set the rotation of the plane to align with the face normal
                //cubeFace.transform.rotation = Quaternion.LookRotation(cube.transform.TransformDirection(faceNormal));

                // Set the scale of the plane to match the size of the face
                Vector3 scale = new Vector3(
                    Vector3.Distance(cube.transform.TransformPoint(vertices[triangles[triangleIndices[0]]]), cube.transform.TransformPoint(vertices[triangles[triangleIndices[0] + 1]])),
                    Vector3.Distance(cube.transform.TransformPoint(vertices[triangles[triangleIndices[0] + 1]]), cube.transform.TransformPoint(vertices[triangles[triangleIndices[0] + 2]])),
                    width
                );

                //cubeFace.transform.localScale = scale;

                // Make the plane a child of the cube
                cubeFace.transform.parent = cube.transform;
                cubeFace.transform.localScale = new Vector3(quadScale, quadScale, quadScale);

                string sideName = GetSideName(faceNormal);
                cubeFace.name = sideName + " Face";

                cubeFace.transform.rotation = Quaternion.LookRotation(cube.transform.TransformDirection(faceNormal));
                // Reverse the Y rotation for all faces except "Top" and "Bottom"
                Vector3 currentRotation = cubeFace.transform.rotation.eulerAngles;
                if (!cubeFace.name.Contains("Top") && !cubeFace.name.Contains("Bottom"))
                {
                    cubeFace.transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + 180f, currentRotation.z);
                }
                else
                {
                    if (cubeFace.name.Contains("Top"))
                    {
                        Vector3 currentRot = cubeFace.transform.rotation.eulerAngles;
                        cubeFace.transform.rotation = Quaternion.Euler(90f, currentRot.y, currentRot.z);
                    }
                    else
                    {
                        // Reverse the X rotation for "Top" and "Bottom" faces
                        cubeFace.transform.rotation = Quaternion.Euler(currentRotation.x + 180f, currentRotation.y, currentRotation.z);
                    }
                }

                //if (cubeFace.name.Contains("Bottom") || cubeFace.name.Contains("Top"))
                //{
                //    cubeFace.transform.localScale += new Vector3((cubeFace.transform.localScale.z * 2), 0, 0);
                //}

                if (cubeFace.name.Contains("Front"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, 0, planeLocalPos);
                }
                else if (cubeFace.name.Contains("Back"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, 0, -planeLocalPos);
                }
                else if (cubeFace.name.Contains("Top"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, planeLocalPos, 0);
                }
                else if (cubeFace.name.Contains("Bottom"))
                {
                    cubeFace.transform.localPosition = new Vector3(0, -planeLocalPos, 0);
                }
                else if (cubeFace.name.Contains("Left"))
                {
                    cubeFace.transform.localPosition = new Vector3(-planeLocalPos, 0, 0);
                }
                else if (cubeFace.name.Contains("Right"))
                {
                    cubeFace.transform.localPosition = new Vector3(planeLocalPos, 0, 0);
                }
            }
        }
        return cubeFace;
    }
}
