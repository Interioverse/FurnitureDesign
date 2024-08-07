//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using TMPro;
//using UnityEngine.EventSystems;
//using System;
//using Newtonsoft.Json.Linq;

//public class Furniture : MonoBehaviour
//{
//    [SerializeField] GameObject cube;
//    [SerializeField] RuntimeTransformHandle runtimeTransformHandle;
//    [SerializeField] SmoothOrbitCam smoothOrbitCam;
//    public FurnitureData furnitureData = new FurnitureData();

//    [SerializeField] int componentsCount;
//    [SerializeField] TMP_InputField _productName, _designerName, _furnitureDescription, _price;
//    [SerializeField] TMP_Dropdown _furnitureCategory;
//    [SerializeField] TextMeshProUGUI warningMessage, saveText;
    
//    Transform newFurniture;
//    //int index;
//    GetClick gc;
//    string filePath;

//    private void Start()
//    {
//        filePath = Application.dataPath + "/StreamingAssets/FurnitureData.json";
//        furnitureData.id = UnityEngine.Random.Range(0, 999999999);
//    }

//    public void SetArrayCount()
//    {
//        newFurniture = runtimeTransformHandle.newFurniture;
//        componentsCount = newFurniture.childCount;
//        furnitureData.numberOfComponentsFound = componentsCount;
//        //furnitureData.components = new Components[componentsCount];
//        //index = componentsCount;
//        smoothOrbitCam.enableZooming = false;
//    }

//    public void CloseDetailsWindow()
//    {
//        smoothOrbitCam.enableZooming = true;
//    }

//    public void SaveDetails()
//    {
//        if (_productName.text == "" || _designerName.text == "" || _furnitureDescription.text == "" || _price.text == "")
//        {
//            warningMessage.text = "Field should not be empty";
//            return;
//        }
//        else if(_furnitureCategory.value == 0)
//        {
//            warningMessage.text = "Please select the Category";
//            return;
//        }

//        warningMessage.text = "";
//        furnitureData.numberOfComponentsFound = componentsCount;
//        furnitureData.productName = _productName.text;
//        furnitureData.designerName = _designerName.text;
//        furnitureData.furnitureCatValue = _furnitureCategory.value;
//        furnitureData.furnitureCategory = _furnitureCategory.options[furnitureData.furnitureCatValue].text;
//        furnitureData.furnitureDetails = _furnitureDescription.text;
//        furnitureData.price = float.Parse(_price.text);

//        int i = 0;
//        foreach (Transform item in newFurniture)
//        {
//            gc = item.GetComponent<GetClick>();
//            if (gc)
//            {
//                furnitureData.components[i].positionValues = gc.positionVector;
//                furnitureData.components[i].rotationValues = gc.rotationVector;
//                furnitureData.components[i].scaleValues = gc.scaleVector;
//                i++;
//            }
//        }
//        //Array.Resize(ref furnitureData.components, index);
//        SaveFurnitureData();
//    }

//    void SaveFurnitureData()
//    {
//        string furnitureDetails = JsonUtility.ToJson(furnitureData);
//        File.WriteAllText(filePath, furnitureDetails);

//        saveText.text = "Product saved succefully";
//        Invoke("ClearMessages", 2);
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.F))
//        {
//            GetTheModelDetails();
//        }
//    }

//    void GetTheModelDetails()
//    {
//        string furnitureDetails = File.ReadAllText(filePath);
//        furnitureData = JsonUtility.FromJson<FurnitureData>(furnitureDetails);
//        componentsCount = furnitureData.numberOfComponentsFound;
//        _productName.text = furnitureData.productName;
//        _designerName.text = furnitureData.designerName;
//        _furnitureCategory.value = furnitureData.furnitureCatValue;
//        _furnitureDescription.text = furnitureData.furnitureDetails;
//        _price.text = furnitureData.price.ToString();

//        for (int i = 0; i < furnitureData.numberOfComponentsFound; i++)
//        {
//            GameObject newCube = Instantiate(cube, transform) as GameObject;
//            newCube.name = "Cube_" + transform.childCount;
//            newCube.transform.localPosition = furnitureData.components[i].positionValues;
//            newCube.transform.localEulerAngles = furnitureData.components[i].rotationValues;
//            newCube.transform.localScale = furnitureData.components[i].scaleValues;
//        }

//        runtimeTransformHandle.AssignTarget(transform);
//    }

//    void ClearMessages()
//    {
//        warningMessage.text = "";
//        saveText.text = "";
//    }
//}

//[System.Serializable]
//public class FurnitureData
//{
//    public int id, numberOfComponentsFound, furnitureCatValue;
//    public string productName;
//    public string designerName;
//    public string furnitureCategory;
//    public string furnitureDetails;
//    public float price;

//    public Components[] components;
//}

//[System.Serializable]
//public class Components
//{
//    public Vector3 positionValues;
//    public Vector3 rotationValues;
//    public Vector3 scaleValues;
//}
