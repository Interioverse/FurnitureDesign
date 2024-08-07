#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MaterialCreator : MonoBehaviour
{
    void Start()
    {
        // Specify the subfolder path within Resources
        string subfolderPath = "Laminates/Glossy";

        // Load all textures in the specified subfolder
        Texture2D[] textures = Resources.LoadAll<Texture2D>(subfolderPath);

        if (textures.Length == 0)
        {
            Debug.LogError("No textures found in the folder: " + subfolderPath);
            return;
        }

        Debug.Log("Found " + textures.Length + " textures in the folder: " + subfolderPath);

        // Iterate through each texture and create a material
        foreach (Texture2D texture in textures)
        {
            Debug.Log("Creating material for texture: " + texture.name);

            // Create a new material
            Material material = new Material(Shader.Find("Standard"));
            material.SetTexture("_MainTex", texture);

            material.SetFloat("_Metallic", 0.75f);
            material.SetFloat("_Glossiness", 0.75f);

#if UNITY_EDITOR
            // Save the material to the Resources folder (Editor only)
            string materialPath = "Assets/Resources/" + subfolderPath + "/" + texture.name + ".mat";
            AssetDatabase.CreateAsset(material, materialPath);
#endif
        }

#if UNITY_EDITOR
        // Refresh the Asset Database to make sure the new materials are recognized
        AssetDatabase.Refresh();
#endif
    }
}
