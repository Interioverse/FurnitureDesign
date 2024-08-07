using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MaterialListLoader : MonoBehaviour
{
    [System.Serializable]
    public class FolderMaterials
    {
        public string folderName;
        public List<Material> materials;
    }

    public List<FolderMaterials> folderMaterialsList = new List<FolderMaterials>();

    void Start()
    {
        string folderPath = "Assets/Resources/Laminates";

        string[] folders = Directory.GetDirectories(folderPath);

        foreach (string folder in folders)
        {
            FolderMaterials folderMaterials = new FolderMaterials();
            folderMaterials.folderName = Path.GetFileName(folder);
            folderMaterials.materials = new List<Material>();

            string[] materialPaths = Directory.GetFiles(folder, "*.mat");

            foreach (string materialPath in materialPaths)
            {
                // Load the material from the Resources folder
                Material material = Resources.Load<Material>("Laminates/" + folderMaterials.folderName + "/" + Path.GetFileNameWithoutExtension(materialPath));

                if (material != null)
                {
                    folderMaterials.materials.Add(material);
                }
                else
                {
                    Debug.LogError("Failed to load material: " + materialPath);
                }
            }

            folderMaterialsList.Add(folderMaterials);
        }
    }
}