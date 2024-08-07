using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class MaterialData
{
    public string materialName;
    public Color materialColor;
    //public Texture2D materialTexture;
}

[System.Serializable]
public class SunmicaData
{
    public string sunmicaMatName, faceSide;
    public Color sunmicaMatColor;
}

public class Product : MonoBehaviour
{
    [SerializeField] GameObject cube, productCard, content;
    [SerializeField] Button closeButton, saveButton, updateButton;
    [SerializeField] SmoothOrbitCam smoothOrbitCam;
    [SerializeField] TMP_InputField _productName, _designerName, _furnitureDescription, _price;
    [SerializeField] TMP_Dropdown _furnitureCategory;
    [SerializeField] TextMeshProUGUI statusText, nothingToDisplay;
    [SerializeField] Transform newFurniture;
    string tempComponent, newComponent, newdata, tempSumComp;
    GetClick gc;
    GetTransformDetails gtd;
    CubeFacePlanes cfp;
    Material gm, smm;
    string filePath;
    int id, selectedProduct;
    int componentCount, furnitureCatValue;
    string productName, designerName, furnitureCategory, furnitureDetails;
    string _wallDesign = "Wall design 1";
    float price;
    SunmicaData sunmicaMaterialData = new SunmicaData();
    //[SerializeField] MaterialsList materialsList;
    Material[] materials;
    string valid_token;

    private void Awake()
    {
        valid_token = UniversalData.valid_token;
    }

    void Start()
    {
        materials = UniversalData.Instance.laminatesMaterials;
        closeButton.onClick.AddListener(() => CloseAction());
        saveButton.onClick.AddListener(() => AddProduct());
        updateButton.onClick.AddListener(() => UpdateProduct());
        yes.onClick.AddListener(() => DeleteProduct());
        no.onClick.AddListener(() => deleteConfirmationPanel.SetActive(false));

        selectedProduct = 0;
        newFurniture = this.transform;
#if UNITY_EDITOR
        filePath = Path.Combine(Application.streamingAssetsPath, "Products.json");
#endif
        GetMyDesigns();
    }

    [SerializeField] GameObject BGWallPanel;

    public void ToggleBGSelection()
    {
        if (_furnitureCategory.value == 1)
        {
            BGWallPanel.SetActive(true);
        }
        else
        {
            BGWallPanel.SetActive(false);
        }
    }

    internal void AssignBackgroundDesign(Material material)
    {
        _wallDesign = material.name.Replace(" (Instance)", "");
    }

    // Do not delete
    //void GetAllMyProducts()
    //{
    //    //try
    //    //{
    //    // var jsonString = File.ReadAllText(filePath);
    //    var jsonObj = JObject.Parse(jsondata);
    //    JArray productsArrary = (JArray)jsonObj["Products"];

    //    if (productsArrary.Count > 0)
    //    {
    //        nothingToDisplay.text = "";
    //        foreach (var product in productsArrary)
    //        {
    //            GameObject button = Instantiate(productCard, content.transform) as GameObject;
    //            button.name = (string)product["id"];
    //            button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => GetSpecificProduct((int)product["id"]));
    //            button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = (string)product["productName"];
    //            button.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ShowDeleteConfirmationPanel((int)product["id"]));
    //        }
    //    }
    //    else
    //    {
    //        id = 0;
    //        nothingToDisplay.text = "Nothing to display";
    //    }
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    Debug.LogError(ex);
    //    //}
    //}

    void DeleteSpecificProductCard(string _pName, int _productID)
    {
        foreach (Transform item in content.transform)
        {
            if (item.name == _pName)
            {
                DestroyImmediate(item.gameObject);
                break;
            }
        }

        HandleNote();

        if (selectedProduct == _productID)
        {
            ClearDetailsField();
        }
    }

    public void SetArrayCount()
    {
        componentCount = newFurniture.childCount;
        if (smoothOrbitCam)
        {
            smoothOrbitCam.enableZooming = false;
        }

        if (id == 0)
        {
            saveButton.gameObject.SetActive(true);
            updateButton.gameObject.SetActive(false);
        }
        else
        {
            saveButton.gameObject.SetActive(false);
            updateButton.gameObject.SetActive(true);
        }
    }

    string sc1 = "( ' )";
    string sc2 = "( \\ )";
    int approvedBool, sunmiceBool, iSum;
    JObject jsonObj;

    void AddProduct()
    {
        if (Validate())
        {
            closeButton.interactable = false;
            saveButton.interactable = false;
            id = UnityEngine.Random.Range(0, 999999999);
            componentCount = newFurniture.childCount;
            productName = _productName.text;
            designerName = _designerName.text;
            furnitureCatValue = _furnitureCategory.value;
            furnitureCategory = _furnitureCategory.options[furnitureCatValue].text;
            furnitureDetails = _furnitureDescription.text;
            price = float.Parse(_price.text);
            if (UniversalData.isAdmin)
            {
                approvedBool = 1;
            }
            else
            {
                approvedBool = 0;
            }

            int i = 0;
            foreach (Transform item in newFurniture)
            {
                //gc = item.GetComponent<GetClick>();
                gtd = item.GetComponent<GetTransformDetails>();
                cfp = item.GetComponent<CubeFacePlanes>();
                gm = item.GetComponent<Renderer>().material;

                if (gtd)
                {
                    iSum = 0;
                    tempSumComp = "";

                    if (cfp.planesCreated)
                    {
                        foreach (Transform sunmiceItem in item)
                        {
                            smm = sunmiceItem.GetComponent<Renderer>().material;
                            sunmicaMaterialData.sunmicaMatName = smm.name;
                            sunmicaMaterialData.faceSide = sunmiceItem.name;
                            sunmicaMaterialData.sunmicaMatName = sunmicaMaterialData.sunmicaMatName.Replace(" (Instance)", "");
                            sunmicaMaterialData.sunmicaMatColor = smm.color;

                            string sunmicaMatJson = JsonUtility.ToJson(sunmicaMaterialData);

                            if (iSum > 0)
                            {
                                tempSumComp = string.Concat(tempSumComp, ", ", sunmicaMatJson);
                            }
                            else
                            {
                                tempSumComp = string.Concat(sunmicaMatJson);
                            }
                            iSum++;

                        }
                        sunmiceBool = 1;

                        tempSumComp = "[" + tempSumComp + "]";
                    }
                    else
                    {
                        sunmiceBool = 0;
                        tempSumComp = "[]";
                    }

                    string transformValues = "{'positionValue': {'x': " + gtd.positionVector.x + ", 'y': " + gtd.positionVector.y + ", 'z': " + gtd.positionVector.z + "}" + "," +
                                             "'rotationValues': { 'x': " + gtd.rotationVector.x + ", 'y': " + gtd.rotationVector.y + ", 'z': " + gtd.rotationVector.z + "}" + "," +
                                             "'scaleValues': { 'x': " + gtd.scaleVector.x + ", 'y': " + gtd.scaleVector.y + ", 'z': " + gtd.scaleVector.z + "}" + "," +
                                             "'sunmica': " + sunmiceBool + "," +
                                             "'zIndex': " + gtd.zIndexValue + "," +
                                             "'smMaterialProperties': " + tempSumComp + "}";

                    if (i > 0)
                    {
                        tempComponent = string.Concat(tempComponent, ", ", transformValues);
                    }
                    else
                    {
                        tempComponent = string.Concat(transformValues);
                    }
                    i++;
                }
            }

            newComponent = "[" + tempComponent + "]";
            newdata = "{'id': " + id + ", 'numberOfComponentsFound': " + componentCount + ", 'furnitureCatValue': " + furnitureCatValue + ", 'productName': '" + productName + "', 'designerName': '" + designerName + "', 'furnitureCategory': '" + furnitureCategory + "', 'furnitureDetails': '" + furnitureDetails + "', 'price': " + price + ", 'wallDesign': '" + _wallDesign + "', 'isApproved': " + approvedBool + ", 'components': " + newComponent + "}";

#if UNITY_EDITOR
            /* Do not delete this - this is working for local path */
            var json = File.ReadAllText(filePath);
            var jsonObj = JObject.Parse(json);

            var components = jsonObj.GetValue("Products") as JArray;
            var freshData = JObject.Parse(newdata);
            components.Add(freshData);
            jsonObj["Products"] = components;
            string newJsonResult = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(filePath, newJsonResult);
            //saveText.text = "Product saved succefully";
            HandleStatusText(Color.green, thisAction + " successfull!", true, 0.5f);
            selectedProduct = id;
            AddSpecificProductCard(id, productName);
            Invoke("ClearText", 0.5f);

#else
            /* Do not delete this - this is working for server path */
            // Debug.Log("New data" + newdata);
            // var json = File.ReadAllText(jsondata);
            if (userSpecificJSONData != null)
            {
                jsonObj = JObject.Parse(userSpecificJSONData);
            }

            JArray components = (JArray)jsonObj["Products"];
            //JArray components = jsonObj.GetValue("Products") as JArray;
            var freshData = JObject.Parse(newdata);
            components.Add(freshData);
            jsonObj["Products"] = components;
            string newJsonResult = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

            userSpecificJSONData = newJsonResult;
            //File.WriteAllText(jsondata, newJsonResult);
            currentNameOfProduct = _productName.text;
            Callsave("Add");
            //UpdateJSONData(newJsonResult);
            selectedProduct = id;
            //AddSpecificProductCard(id, productName);
#endif
        }
    }

    string currentNameOfProduct = "";

    void UpdateCardName(string currentName, string updatedName)
    {
        foreach (Transform card in content.transform)
        {
            if (card.name == currentName)
            {
                card.name = updatedName;
                card.GetComponentInChildren<TextMeshProUGUI>().text = updatedName;
                currentNameOfProduct = updatedName;
                break;
            }
        }
    }

    void AddSpecificProductCard(int _id, string _name)
    {
        GameObject button = Instantiate(productCard, content.transform) as GameObject;
        button.name = _name;
        button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => GetSpecificProduct(_id));
        button.transform.GetComponentInChildren<TextMeshProUGUI>().text = _name;
        button.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ShowDeleteConfirmationPanel(_id));

        HandleNote();
    }

    void UpdateProduct()
    {
        if (Validate())
        {
            closeButton.interactable = false;
            updateButton.interactable = false;
            //#if UNITY_EDITOR
            //            /* this is for local save fuctionality */
            //            var jsonString = File.ReadAllText(filePath);
            //            var jsonObj = JObject.Parse(jsonString);
            //            JArray productsArrary = (JArray)jsonObj["Products"];
            //#endif
            /* This is for Github interaction */
            var jsonObj = JObject.Parse(userSpecificJSONData);
            JArray productsArrary = (JArray)jsonObj["Products"];

            foreach (var product in productsArrary.Where(obj => obj["id"].Value<int>() == selectedProduct))
            {
                product["numberOfComponentsFound"] = newFurniture.childCount;
                product["productName"] = _productName.text;
                product["designerName"] = _designerName.text;
                product["furnitureCatValue"] = _furnitureCategory.value;
                product["furnitureDetails"] = _furnitureDescription.text;
                product["price"] = float.Parse(_price.text);
                product["wallDesign"] = _wallDesign;

                if (UniversalData.isAdmin)
                {
                    product["isApproved"] = 1;
                }
                else
                {
                    product["isApproved"] = 0;
                }

                JArray componentsArrary = (JArray)product["components"];
                componentsArrary.RemoveAll();

                foreach (Transform item in newFurniture)
                {
                    gtd = item.GetComponent<GetTransformDetails>();
                    cfp = item.GetComponent<CubeFacePlanes>();
                    gm = item.GetComponent<Renderer>().material;
                    if (gtd)
                    {
                        iSum = 0;
                        tempSumComp = "";

                        if (cfp.planesCreated)
                        {
                            foreach (Transform sunmiceItem in item)
                            {
                                smm = sunmiceItem.GetComponent<Renderer>().material;
                                sunmicaMaterialData.sunmicaMatName = smm.name;
                                sunmicaMaterialData.faceSide = sunmiceItem.name;
                                sunmicaMaterialData.sunmicaMatName = sunmicaMaterialData.sunmicaMatName.Replace(" (Instance)", "");
                                sunmicaMaterialData.sunmicaMatColor = smm.color;

                                string sunmicaMatJson = JsonUtility.ToJson(sunmicaMaterialData);

                                if (iSum > 0)
                                {
                                    tempSumComp = string.Concat(tempSumComp, ", ", sunmicaMatJson);
                                }
                                else
                                {
                                    tempSumComp = string.Concat(sunmicaMatJson);
                                }
                                iSum++;

                            }
                            sunmiceBool = 1;

                            tempSumComp = "[" + tempSumComp + "]";
                        }
                        else
                        {
                            sunmiceBool = 0;
                            tempSumComp = "[]";
                        }

                        string transformValues = "{'positionValue': {'x': " + gtd.positionVector.x + ", 'y': " + gtd.positionVector.y + ", 'z': " + gtd.positionVector.z + "}" + "," +
                                                "'rotationValues': { 'x': " + gtd.rotationVector.x + ", 'y': " + gtd.rotationVector.y + ", 'z': " + gtd.rotationVector.z + "}" + "," +
                                                "'scaleValues': { 'x': " + gtd.scaleVector.x + ", 'y': " + gtd.scaleVector.y + ", 'z': " + gtd.scaleVector.z + "}" + "," +
                                                //"'materialProperties': " + matJson + "," +
                                                "'sunmica': " + sunmiceBool + "," +
                                                "'zIndex': " + gtd.zIndexValue + "," +
                                                "'smMaterialProperties': " + tempSumComp + "}";

                        var newComp = JObject.Parse(transformValues);
                        componentsArrary.Add(newComp);
                    }
                }

                product["components"] = componentsArrary;
            }

            //#if UNITY_EDITOR
            //            /* Local file code functionality */
            //            jsonObj["Products"] = productsArrary;
            //            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            //            File.WriteAllText(filePath, output);
            //            saveText.text = "Product saved succefully";
            //            Invoke("ClearText", 0.5f);
            //#endif

            /* Server file code functionality */
            jsonObj["Products"] = productsArrary;
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            userSpecificJSONData = output;
            Callsave("Update");
        }
    }

    [SerializeField] GameObject deleteConfirmationPanel;
    [SerializeField] Button yes, no;

    void ShowDeleteConfirmationPanel(int productID)
    {
        transformHandlesUI.SetActive(false);
        productsListPanel.SetActive(false);
        deleteConfirmationPanel.SetActive(true);
        dltProductID = productID;
    }

    int dltProductID;

    void DeleteProduct()
    {
        DeleteProduct(dltProductID);
    }

    string pName;
    int pID;

    void DeleteProduct(int productID)
    {
        //#if UNITY_EDITOR
        //            /* this is for local save fuctionality */
        //            var jsonString = File.ReadAllText(filePath);
        //            var jObject = JObject.Parse(jsonString);
        //            JArray productsArrary = (JArray)jObject["Products"];
        //#endif
        /* this is for server save fuctionality */
        yes.interactable = false;
        no.interactable = false;
        var jObject = JObject.Parse(userSpecificJSONData);
        JArray productsArrary = (JArray)jObject["Products"];

        var product = productsArrary.FirstOrDefault(obj => obj["id"].Value<int>() == productID);
        pName = product["productName"].ToString();
        pID = productID;
        productsArrary.Remove(product);
        string output = JsonConvert.SerializeObject(jObject, Formatting.Indented);
        userSpecificJSONData = output;
        Callsave("Delete");
        //DeleteSpecificProductCard(pName, productID);
        //deleteConfirmationPanel.SetActive(false);
    }

    void HandleNote()
    {
        if (content.transform.childCount > 0)
        {
            nothingToDisplay.text = "";
        }
        else
        {
            nothingToDisplay.text = "Nothing to display";
        }
    }

    public void StartNewDesign()
    {
        if (!newFurniture)
        {
            newFurniture = this.transform;
        }

        if (newFurniture.childCount > 0)
        {
            AppManager.Instance.target = newFurniture;
            AppManager.Instance.DeleteThisObject();
        }
        ClearDetailsField();
    }

    public void ClearDetailsField()
    {
        id = 0;
        selectedProduct = id;
        _productName.text = "";
        _designerName.text = "";
        _furnitureCategory.value = 0;
        _furnitureDescription.text = "";
        _price.text = "";
    }

    bool Validate()
    {
        if (_productName.text == "" || _designerName.text == "" || _furnitureDescription.text == "" || _price.text == "")
        {
            HandleStatusText(Color.red, "Field should not be empty", false, 2);
            return false;
        }
        else if (_productName.text.Contains("'") || _productName.text.Contains("\\"))
        {
            HandleStatusText(Color.red, "Please do not use apostrophe " + sc1 + " and backslash " + sc2 + "", false, 3);
            return false;
        }
        else if (_designerName.text.Contains("'") || _designerName.text.Contains("\\"))
        {
            HandleStatusText(Color.red, "Please do not use apostrophe " + sc1 + " and backslash " + sc2 + "", false, 3);
            return false;
        }
        else if (_furnitureCategory.value == 0)
        {
            HandleStatusText(Color.red, "Please select the Category", false, 2);
            return false;
        }
        else if (_furnitureDescription.text.Contains("'") || _furnitureDescription.text.Contains("\\"))
        {
            HandleStatusText(Color.red, "Please do not use apostrophe " + sc1 + " and backslash " + sc2 + "", false, 3);
            return false;
        }
        return true;
    }

    void Callsave(string performed)
    {
        thisAction = performed;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.UpdateList(userSpecificJSONData, currentUserID, AffectedChanges);
    }

    string thisAction;

    private void AffectedChanges(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            HandleStatusText(Color.green, thisAction + " successfull!", true, 0.5f);
        }
        else
        {
            HandleStatusText(Color.red, thisAction + " not successfull", true, 0.5f);
        }
    }

    /*--------------------------------------- Important codes -----------------------------------------*/

    int currentUserID;

    void GetMyDesigns()
    {

#if UNITY_EDITOR
        string dataAsJson = File.ReadAllText(filePath);
        JObject jsonObject = JObject.Parse(dataAsJson);
        GetAllMyProducts(jsonObject);
#else
        /* Do not delete */
        if (UniversalData.guestLogin || !UniversalData.isDesigner)
        {
            nothingToDisplay.text = "Nothing to display";
            return;
        }
        else
        {
            currentUserID = UniversalData.userId;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UniversalData.Instance.noInternetScene.SetActive(true);
                return;
            }
            UniversalData.Instance.noInternetScene.SetActive(false);
            HttpAWSconnect.Instance.GetFileData(currentUserID, ReceivedResponseData);
        }
#endif
    }

    private void ReceivedResponseData(string data, UnityWebRequest.Result result)
    {
#if UNITY_EDITOR

#else
        /* DO not Delete */
        if (result == UnityWebRequest.Result.Success)
        {
            JObject dataObject = (JObject)JObject.Parse(data)["data"];
            string fileData = dataObject["file"].Value<string>();
            JObject fileObject = JObject.Parse(fileData);
            JArray usersArray = (JArray)fileObject["Products"];
            userSpecificJSONData = "{\"Products\":" + usersArray + "}".ToString();
            //productFileJSONData = Regex.Replace(productFileJSONData, @"\r\n|\s+", "");
            if (userSpecificJSONData == null || userSpecificJSONData == "")
            {
                userSpecificJSONData = "{\"Products\": []}";
            }
        }
        else
        {
            userSpecificJSONData = "{\"Products\": []}";
        }

        JObject jsonObj = JObject.Parse(userSpecificJSONData);
        GetAllMyProducts(jsonObj);
#endif
    }

    string userSpecificJSONData;

    public void GetAllMyProducts(JObject jsonObj)
    {
#if UNITY_EDITOR
        /* this is for Local save fuctionality */
        JArray productsArrary = (JArray)jsonObj["Products"];
#else
        /* this is for server save fuctionality */
        JArray productsArrary = (JArray)jsonObj["Products"];
#endif
        if (productsArrary.Count > 0)
        {
            nothingToDisplay.text = "";
            foreach (var product in productsArrary)
            {
                GameObject button = Instantiate(productCard, content.transform) as GameObject;
                button.name = (string)product["productName"];
                button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => GetSpecificProduct((int)product["id"]));
                button.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = (string)product["productName"];
                button.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ShowDeleteConfirmationPanel((int)product["id"]));
            }
        }
        else
        {
            id = 0;
            nothingToDisplay.text = "Nothing to display";
        }
    }

    Vector3 targetParentScale, targetScale;
    bool isTopBottom, isRightLeft;

    public void GetSpecificProduct(int value)
    {
        Debug.Log(value);
        if (selectedProduct == value)
        {
            return;
        }

        //if (materialsList.materials.Length <= 0)
        //{
        //    materialsList.LoadAllMaterials();
        //}

        if (!newFurniture)
        {
            newFurniture = this.transform;
        }

        if (newFurniture.childCount > 0)
        {
            AppManager.Instance.target = newFurniture;
            AppManager.Instance.DeleteThisObject();
        }

#if UNITY_EDITOR
        /* this is for local save fuctionality */
        var jsonString = File.ReadAllText(filePath);
        var jsonObj = JObject.Parse(jsonString);
        JArray productsArrary = (JArray)jsonObj["Products"];
#else
        /* this is for server save fuctionality */
        var jsonObj = JObject.Parse(userSpecificJSONData);
        JArray productsArrary = (JArray)jsonObj["Products"];
#endif
        foreach (var product in productsArrary.Where(obj => obj["id"].Value<int>() == value))
        {
            id = value;
            selectedProduct = id;
            componentCount = product["numberOfComponentsFound"].ToObject<int>();
            _productName.text = product["productName"].ToString();
            currentNameOfProduct = _productName.text;
            _designerName.text = product["designerName"].ToString();
            _furnitureCategory.value = product["furnitureCatValue"].ToObject<int>();
            _furnitureDescription.text = product["furnitureDetails"].ToString();
            _price.text = product["price"].ToString();
            _wallDesign = product["wallDesign"].ToString();

            JArray componentsArrary = (JArray)product["components"];

            foreach (var element in componentsArrary)
            {
                GameObject newCube = Instantiate(cube, newFurniture.transform) as GameObject;
                newCube.name = "Cube_" + UnityEngine.Random.Range(1, 9999999);
                GetTransformDetails getTransformDetails = newCube.GetComponent<GetTransformDetails>();
                newCube.transform.localPosition = new Vector3(element["positionValue"]["x"].ToObject<float>(), element["positionValue"]["y"].ToObject<float>(), element["positionValue"]["z"].ToObject<float>());
                newCube.transform.localEulerAngles = new Vector3(element["rotationValues"]["x"].ToObject<float>(), element["rotationValues"]["y"].ToObject<float>(), element["rotationValues"]["z"].ToObject<float>());
                getTransformDetails.zIndexValue = element["zIndex"].ToObject<int>();
                getTransformDetails.zVectorValue = element["scaleValues"]["z"].ToObject<float>();
                newCube.transform.localScale = new Vector3(element["scaleValues"]["x"].ToObject<float>(), element["scaleValues"]["y"].ToObject<float>(), element["scaleValues"]["z"].ToObject<float>());
                targetParentScale = newCube.transform.localScale;
                //if (element["sunmica"].ToObject<int>() == 1)
                //{

                //}

                CubeFacePlanes cubeFacePlanes = newCube.GetComponent<CubeFacePlanes>();
                cubeFacePlanes.planesCreated = true;
                JObject componentObj = (JObject)element;
                JArray smMaterialPropertiesArray = (JArray)componentObj["smMaterialProperties"];

                // Access the details within the smMaterialProperties array
                foreach (JToken property in smMaterialPropertiesArray)
                {
                    GameObject currentFace = null;
                    JObject propertyObj = (JObject)property;
                    string propertyName = (string)propertyObj["faceSide"];

                    if (newCube.transform.localEulerAngles != Vector3.zero)
                    {
                        Vector3 targetRotationValues = newCube.transform.localEulerAngles;
                        newCube.transform.localEulerAngles = Vector3.zero;

                        foreach (var face in cubeFacePlanes.sideFaces)
                        {
                            if (face.name == propertyName)
                            {
                                currentFace = face;
                            }
                        }

                        //currentFace = cubeFacePlanes.CreatePlaneFor(propertyName);
                        newCube.transform.localEulerAngles = targetRotationValues;
                    }
                    else
                    {
                        foreach (var face in cubeFacePlanes.sideFaces)
                        {
                            if (face.name == propertyName)
                            {
                                currentFace = face;
                            }
                        }
                    }

                    propertyName = propertyName.Replace(" Face", "");
                    Material newSunMat = currentFace.GetComponent<Renderer>().material;
                    newSunMat.name = (string)propertyObj["sunmicaMatName"];
                    JToken jSunToken = propertyObj["sunmicaMatColor"];
                    targetScale = currentFace.transform.localScale;
                    foreach (Material _material in materials)
                    {
                        if (_material.name == newSunMat.name && _material.mainTexture)
                        {
                            isTopBottom = isRightLeft = false;
                            newSunMat.mainTexture = _material.mainTexture;
                            if (propertyName == "Top" || propertyName == "Bottom")
                            {
                                isTopBottom = true;
                            }
                            else if (propertyName == "Left" || propertyName == "Right")
                            {
                                isRightLeft = true;
                            }
                            /*Do not delete this line of code*/
                            //TextureAdjust(newSunMat, targetScale, targetParentScale, isTopBottom, isRightLeft);
                            break;
                        }
                    }
                    newSunMat.color = new Color(jSunToken["r"].ToObject<float>(), jSunToken["g"].ToObject<float>(), jSunToken["b"].ToObject<float>(), jSunToken["a"].ToObject<float>());
                }
            }
            break;
        }
    }

    /*Do not delete this line of code*/
    //private void TextureAdjust(Material materialInstance, Vector3 targetScale, Vector3 targetParentScale, bool isTopBottom, bool isRightLeft)
    //{
    //    materialInstance.mainTextureScale = new Vector2(targetScale.x + targetParentScale.x, targetScale.y + targetParentScale.y);
    //    if (isTopBottom)
    //    {
    //        materialInstance.mainTextureScale = new Vector2(materialInstance.mainTextureScale.x, (targetParentScale.z * 10));
    //    }
    //    else if (isRightLeft)
    //    {
    //        materialInstance.mainTextureScale = new Vector2((targetParentScale.z * 10), materialInstance.mainTextureScale.y);
    //    }
    //}

    void HandleStatusText(Color _color, string status, bool value, float waitTime)
    {
        statusText.color = _color;
        statusText.text = status;
        StartCoroutine(StatusClear(value, waitTime));
    }

    IEnumerator StatusClear(bool value, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        statusText.text = "";
        if (value)
        {
            CloseAddDetailsPanel();
            yes.interactable = value;
            no.interactable = value;
            closeButton.interactable = value;
            saveButton.interactable = value;
            updateButton.interactable = value;
        }
    }

    private void CloseAddDetailsPanel()
    {
        if (thisAction == "Add")
        {
            CloseAction();
            AddSpecificProductCard(selectedProduct, productName);
        }
        if (thisAction == "Update")
        {
            CloseAction();
            if (currentNameOfProduct != _productName.text)
            {
                UpdateCardName(currentNameOfProduct, _productName.text);
            }
        }
        if (thisAction == "Delete")
        {
            deleteConfirmationPanel.SetActive(false);
            DeleteSpecificProductCard(pName, pID);
        }
        transformHandlesUI.SetActive(true);
        productsListPanel.SetActive(true);
        thisAction = "";
    }

    private void CloseAction()
    {
        AppManager.Instance.addDetailsPanel.SetActive(false);
        AppManager.Instance.customizserPanel.SetActive(true);
        if (smoothOrbitCam)
        {
            smoothOrbitCam.enableZooming = true;
        }
    }

    [SerializeField] GameObject transformHandlesUI, productsListPanel;
}