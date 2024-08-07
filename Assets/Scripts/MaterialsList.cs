using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public enum PanelOptions
{
    sunmicaMaterials,
    wallMaterials,
    both
}

public class MaterialsList : MonoBehaviour
{
    // Define a class to hold information about a folder and its materials
    [System.Serializable]
    public class FolderMaterials
    {
        public string folderName;
        public List<Material> mats;
    }

    //public List<FolderMaterials> folderMaterialsList = new List<FolderMaterials>();

    [SerializeField] Button prefabMaterial;
    //[SerializeField] string folderName;
    public Material[] materials;
    //[SerializeField] RuntimeTransformHandle runtimeTransformHandle;
    [SerializeField] Product product;
    [SerializeField] GameObject parent;
    [SerializeField] ExpandableUI expandableUI;

    [SerializeField]
    [Header("ColorOptions")]
    private PanelOptions materialsNeeded;

    public GameObject _parent;

    public string myNatPath;

    void Start()
    {
        if (materialsNeeded == PanelOptions.wallMaterials)
        {
            materials = UniversalData.Instance.wallMaterials;
        }
        else if (materialsNeeded == PanelOptions.sunmicaMaterials)
        {
            myNatPath = "Laminates/" + _parent.name;
            materials = Resources.LoadAll<Material>(myNatPath);
        }

        LoadAllMaterials();
    }

    private int selectedMaterialIndex = -1;

    public void LoadAllMaterials()
    {
        //materials = Resources.LoadAll<Material>("Wall Designs");

        for (int i = 0; i < materials.Length; i++)
        {
            int currentIndex = i; // Capture the index for the click listener
            Material material = materials[currentIndex];

            Button newMat = Instantiate(prefabMaterial, transform) as Button;
            Image _image = newMat.GetComponent<Image>();
            newMat.GetComponent<MyMat>().myMaterial = material;
            _image.color = material.color;

            if (material.mainTexture)
            {
                Sprite sprite = Sprite.Create((Texture2D)material.mainTexture,
                    new Rect(0, 0, material.mainTexture.width, material.mainTexture.height),
                    new Vector2(0.5f, 0.5f));
                _image.sprite = sprite;
            }

            newMat.onClick.AddListener(() => OnMaterialButtonClick(currentIndex));
        }

        if (parent)
        {
            parent.SetActive(false);
        }
    }

    //private void GetTheMaterials()
    //{
    //    string folderPath = "Assets/Resources/Laminates";

    //    string[] folders = Directory.GetDirectories(folderPath);

    //    // Loop through each folder
    //    foreach (string folder in folders)
    //    {
    //        string folderName = Path.GetFileName(folder);
    //        if (folderName == _parent.name)
    //        {
    //            FolderMaterials folderMaterials = new FolderMaterials();
    //            folderMaterials.folderName = Path.GetFileName(folder);
    //            folderMaterials.mats = new List<Material>();

    //            string[] materialPaths = Directory.GetFiles(folder, "*.mat");

    //            foreach (string materialPath in materialPaths)
    //            {
    //                //index++;
    //                //print("index " + index);
    //                //expandableUI.indexValue = index;

    //                // Load the material from the Resources folder
    //                Material material = Resources.Load<Material>("Laminates/" + folderMaterials.folderName + "/" + Path.GetFileNameWithoutExtension(materialPath));
    //                if (material != null)
    //                {
    //                    folderMaterials.mats.Add(material);

    //                    Button newMat = Instantiate(prefabMaterial, transform) as Button;
    //                    Image _image = newMat.GetComponent<Image>();

    //                    newMat.GetComponent<MyMat>().myMaterial = material;
    //                    _image.color = material.color;

    //                    if (material.mainTexture)
    //                    {
    //                        Sprite sprite = Sprite.Create((Texture2D)material.mainTexture,
    //                            new Rect(0, 0, material.mainTexture.width, material.mainTexture.height),
    //                            new Vector2(0.5f, 0.5f));
    //                        _image.sprite = sprite;
    //                    }

    //                    Image selected = newMat.transform.GetChild(0).GetComponent<Image>();

    //                    //newMat.onClick.AddListener(() => OnMaterialBtnClick(currentFolderIndex, index));
    //                    newMat.onClick.AddListener(() => OnMaterialBtnClick(selected, material));
    //                }
    //                else
    //                {
    //                    Debug.LogError("Failed to load material: " + materialPath);
    //                }
    //            }

    //            folderMaterialsList.Add(folderMaterials);
    //        }
    //    }
    //}

    private void OnMaterialBtnClick(Image buttonImage, Material material)
    {
        //if (material == previousMaterial)
        //{
        //    return;
        //}

        if (expandableUI.previousImage)
        {
            expandableUI.previousImage.color = Color.black;
        }

        buttonImage.color = Color.green;

        if (!AppManager.Instance.addDetailsPanel.activeSelf)
        {
            AppManager.Instance.SetMaterial(material);
        }
        else
        {
            product.AssignBackgroundDesign(material);
        }

        expandableUI.previousImage = buttonImage;
    }


    private void OnMaterialButtonClick(int clickedIndex)
    {
        // Update the color of the selected material's child
        if (selectedMaterialIndex != -1)
        {
            Button selectedButton = transform.GetChild(selectedMaterialIndex).GetComponent<Button>();
            Image selectedImage = selectedButton.transform.GetChild(0).GetComponent<Image>();
            selectedImage.color = Color.black;
        }

        // Set the color of the clicked material's child to green
        Button clickedButton = transform.GetChild(clickedIndex).GetComponent<Button>();
        Image clickedImage = clickedButton.transform.GetChild(0).GetComponent<Image>();
        clickedImage.color = Color.green;

        // Update the selected material index
        selectedMaterialIndex = clickedIndex;

        // Now you can perform other actions as needed
        if (!AppManager.Instance.addDetailsPanel.activeSelf)
        {
            //Here we can directly pass the material also
            AppManager.Instance.SetMaterial(materials[clickedIndex]);
        }
        else
        {
            //Here we can directly pass the material also
            product.AssignBackgroundDesign(materials[clickedIndex]);
        }
    }
}

