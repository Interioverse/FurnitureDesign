using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    public GameObject cube;
    public float quadSize = 0.1006f;
    public float quadPosition = 0.503f;
    public Material cubeMaterial;

    private void Start()
    {
        // Generate the cube
        cube = gameObject;
        GenerateCube();
    }

    private void GenerateCube()
    {
        // Create planes for each face of the cube
        CreatePlane(cube, Vector3.up, (Vector3.up * quadPosition), 0f, 0f, "Top");             // Top face
        CreatePlane(cube, Vector3.down, (Vector3.down * quadPosition), 0f, 180f, "Bottom");      // Bottom face
        CreatePlane(cube, Vector3.forward, (Vector3.forward * quadPosition), 90f, 0f, "Front");     // Front face
        CreatePlane(cube, Vector3.back, (Vector3.back * quadPosition), 90f, 180f, "Back");       // Back face
        CreatePlane(cube, Vector3.right, (Vector3.right * quadPosition), 90f, 90f, "Right");      // Right face
        CreatePlane(cube, Vector3.left, (Vector3.left * quadPosition), 90f, -90f, "Left");       // Left face

        // Position the cube in the center of the scene
        cube.transform.position = Vector3.zero;
    }

    private void CreatePlane(GameObject parent, Vector3 normal, Vector3 position, float rotationX, float rotationY, string faceName)
    {
        // Create a plane for the face
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        // Set the size of the plane to match the cube size
        plane.transform.localScale = new Vector3(quadSize, quadSize, quadSize);

        // Rotate the plane to face the correct direction
        plane.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // Set the position of the plane
        plane.transform.position = position;

        // Set the material for the plane
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        planeRenderer.material = cubeMaterial;

        // Parent the plane to the cube object
        plane.transform.parent = parent.transform;

        // Set the name of the face
        plane.name = faceName;
    }
}
