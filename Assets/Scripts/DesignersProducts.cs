using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;
using System;

public class DesignersProducts : MonoBehaviour
{
    [SerializeField] GameObject cube, productsHolder, approvalPanel;
    public TextMeshProUGUI note;
    [SerializeField] List<GameObject> spawnPoints;
    string mail;
    //Vector3 targetParentScale, targetScale;
    //bool isTopBottom, isRightLeft;
    [SerializeField] SmoothOrbitCam smoothOrbitCam;
    [SerializeField] Button previous, next, yes, no;
    int currentIndex, currentFileID;

    private void Start()
    {
        previous.onClick.AddListener(() => PreviousProduct());
        next.onClick.AddListener(() => NextProduct());
        yes.onClick.AddListener(() => Delete());
        no.onClick.AddListener(() => CancelDelete());
    }

    public void PreviousProduct()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = 0;
            return;
        }

        foreach (GameObject product in spawnPoints)
        {
            product.SetActive(false);
        }

        spawnPoints[currentIndex].SetActive(true);
        smoothOrbitCam.target = spawnPoints[currentIndex].transform;
    }

    public void NextProduct()
    {
        currentIndex++;
        if (currentIndex >= numberOfProducts)
        {
            currentIndex = numberOfProducts - 1;
            return;
        }

        foreach (GameObject product in spawnPoints)
        {
            product.SetActive(false);
        }

        spawnPoints[currentIndex].SetActive(true);
        smoothOrbitCam.target = spawnPoints[currentIndex].transform;
    }

    public void FetchAllFurnitures(int fileID, string email, string data)
    {
        if (currentFileID != fileID)
        {
            mail = email;
            note.text = "Hang on, loading the products\n...";

            foreach (GameObject spawnPoint in spawnPoints)
            {
                DestroyImmediate(spawnPoint);
            }

            spawnPoints.Clear();
            currentFileID = fileID;
            LoadTheModelsHere(data);
        }
        else
        {
            return;
        }
    }

    void CreateGameObjectsWithLayer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newSpawnPoint = new GameObject("SpawnPlace " + (i + 1));
            newSpawnPoint.transform.parent = productsHolder.transform;
            spawnPoints.Add(newSpawnPoint);
            if (i > 0)
            {
                newSpawnPoint.layer = LayerMask.NameToLayer("Cube");
            }
        }
    }

    int numberOfProducts;

    void LoadTheModelsHere(string data)
    {
        userSpecificJSONData = data;
        int index = 0;

        var jsonObj = JObject.Parse(data);
        JArray productsArrary = (JArray)jsonObj["Products"];

        numberOfProducts = productsArrary.Count;

        if (productsArrary.Count > 0)
        {
            CreateGameObjectsWithLayer(productsArrary.Count);

            foreach (var product in productsArrary)
            {
                JArray componentsArrary = (JArray)product["components"];

                foreach (var element in componentsArrary)
                {
                    GameObject newCube = Instantiate(cube, spawnPoints[index].transform) as GameObject;
                    //spawnPoints[index].gameObject.layer = 8;
                    spawnPoints[index].name = product["productName"].ToString();
                    //newCube.name = "Cube_" + spawnPoints[index].transform.childCount;
                    newCube.transform.localPosition = new Vector3(element["positionValue"]["x"].ToObject<float>(), element["positionValue"]["y"].ToObject<float>(), element["positionValue"]["z"].ToObject<float>());
                    newCube.transform.localEulerAngles = new Vector3(element["rotationValues"]["x"].ToObject<float>(), element["rotationValues"]["y"].ToObject<float>(), element["rotationValues"]["z"].ToObject<float>());
                    newCube.transform.localScale = new Vector3(element["scaleValues"]["x"].ToObject<float>(), element["scaleValues"]["y"].ToObject<float>(), element["scaleValues"]["z"].ToObject<float>());

                    //targetParentScale = newCube.transform.localScale;
                    Vector3 originalRotation = newCube.transform.localEulerAngles;
                    newCube.transform.localEulerAngles = Vector3.zero;

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

                        foreach (var face in cubeFacePlanes.sideFaces)
                        {
                            if (face.name == propertyName)
                            {
                                currentFace = face;
                            }
                        }

                        //currentFace = cubeFacePlanes.CreatePlaneFor(propertyName) as GameObject;
                        propertyName = propertyName.Replace(" Face", "");
                        Material newSunMat = currentFace.GetComponent<Renderer>().material;
                        newSunMat.name = (string)propertyObj["sunmicaMatName"];
                        JToken jSunToken = propertyObj["sunmicaMatColor"];
                        //targetScale = currentFace.transform.localScale;
                        foreach (Material _material in UniversalData.Instance.laminatesMaterials)
                        {
                            if (_material.name == newSunMat.name && _material.mainTexture)
                            {
                                //isTopBottom = isRightLeft = false;
                                newSunMat.mainTexture = _material.mainTexture;
                                //if (propertyName == "Top" || propertyName == "Bottom")
                                //{
                                //    isTopBottom = true;
                                //}
                                //else if (propertyName == "Left" || propertyName == "Right")
                                //{
                                //    isRightLeft = true;
                                //}
                                ///*Do not delete this line of code*/
                                ////TextureAdjust(newSunMat, targetScale, targetParentScale, isTopBottom, isRightLeft);
                                break;
                            }
                        }
                        newSunMat.color = new Color(jSunToken["r"].ToObject<float>(), jSunToken["g"].ToObject<float>(), jSunToken["b"].ToObject<float>(), jSunToken["a"].ToObject<float>());
                    }
                    newCube.transform.localEulerAngles = originalRotation;
                }

                GameObject newPanel = Instantiate(approvalPanel, spawnPoints[index].transform) as GameObject;
                FD = newPanel.transform.GetComponent<FurnitureDetails>();
                Transform targetBed = spawnPoints[index].transform;
                FurnitureDetails _FD = FD;
                FD.approvedName.text = product["productName"].ToString();
                FD.approvedPrice.text = product["price"].ToString();
                if ((int)product["isApproved"] == 1)
                {
                    FD.currentStatus.color = Color.green;
                }
                else
                {
                    FD.currentStatus.color = Color.yellow;
                }
                FD.EditBed.onClick.AddListener(() => LoadDesignSceneWithThisBed((int)product["id"]));
                FD.pending.onClick.AddListener(() => Pending((int)product["id"], _FD));
                FD.approved.onClick.AddListener(() => Approve((int)product["id"], _FD));
                FD.delete.onClick.AddListener(() => DeleteProduct((int)product["id"]));
                index++;
                FD = null;
            }

            foreach (GameObject item in spawnPoints)
            {
                item.SetActive(false);
            }

            spawnPoints[0].SetActive(true);
            smoothOrbitCam.target = spawnPoints[0].transform;
            previous.gameObject.SetActive(true);
            next.gameObject.SetActive(true);
            currentIndex = 0;
            note.text = "";
        }
        else
        {
            previous.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            note.text = "No designs found for the designer\n...";
        }
    }

    FurnitureDetails FD;

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

    void LoadDesignSceneWithThisBed(int prodID)
    {
        //UniversalData.Instance.LoadDesignScene();
    }
    
    void Pending(int prodID, FurnitureDetails _FD)
    {
        UpdateProduct(prodID, _FD.approvedPrice.text, 0, _FD.approvedName.text);
        _FD.currentStatus.color = Color.yellow;
        _FD._status.color = Color.red;
        _FD._status.text = "Not Approved";
        StartCoroutine(ResetStatus(_FD));
    }

    IEnumerator ResetStatus(FurnitureDetails _FD)
    {
        yield return new WaitForSeconds(2);
        _FD._status.color = Color.black;
        _FD._status.text = "status";
    }

    void Approve(int prodID, FurnitureDetails _FD)
    {
        UpdateProduct(prodID, _FD.approvedPrice.text, 1, _FD.approvedName.text);
        _FD.currentStatus.color = Color.green;
        _FD._status.color = Color.green;
        _FD._status.text = "Approved";
        StartCoroutine(ResetStatus(_FD));
    }

    string userSpecificJSONData;
    string updatedFurnitureData;
    int updatedFurnitureID;

    const string productsFileID = "1222090467023";
    [SerializeField] GameObject deleteConfirmationPanel;

    void UpdateProduct(int selectedProduct, string price, int actionTaken, string name)
    {
        var userSpecificjsonObj = JObject.Parse(userSpecificJSONData);
        JArray userSpecificProductsArrary = (JArray)userSpecificjsonObj["Products"];

        foreach (var product in userSpecificProductsArrary.Where(obj => obj["id"].Value<int>() == selectedProduct))
        {
            product["productName"] = name;
            product["price"] = float.Parse(price);
            product["isApproved"] = actionTaken;
            updatedFurnitureID = selectedProduct;
            updatedFurnitureData = product.ToString();
        }

        userSpecificjsonObj["Products"] = userSpecificProductsArrary;
        string output = JsonConvert.SerializeObject(userSpecificjsonObj, Formatting.Indented);
        userSpecificJSONData = output;

        SaveUserSpecificJSONData();

        //Debug.Log("updatedFurnitureID " + updatedFurnitureID);

        var productFilejsonObj = JObject.Parse(ProjectManager.productFileJSONData);
        JArray productsArray = (JArray)productFilejsonObj["Products"];
        bool isProductPresent = CheckProduct(productsArray, updatedFurnitureID);

        if (isProductPresent && actionTaken == 1)
        {
            //print("This is already approved");
            return;
        }
        else if (isProductPresent && actionTaken == 0)
        {
            // Remove the product from products list
            var productsToRemove = productsArray.Where(obj => obj["id"].Value<int>() == updatedFurnitureID).ToList();

            foreach (var productToRemove in productsToRemove)
            {
                productToRemove.Remove();
            }

            string updatedJson = JsonConvert.SerializeObject(productFilejsonObj, Formatting.Indented);
            ProjectManager.productFileJSONData = updatedJson;
            //Debug.Log(ProjectManager.productFileJSONData);
            SaveToProductsJson();

            //foreach (var product in productsArray.Where(obj => obj["id"].Value<int>() == updatedFurnitureID))
            //{
            //    RemoveElementById(productFilejsonObj, updatedFurnitureID);
            //    string updatedJson = JsonConvert.SerializeObject(productFilejsonObj, Formatting.Indented);
            //    ProjectManager.productFileJSONData = updatedJson;
            //    SaveToProductsJson();
            //}
        }
        else if (actionTaken == 1)
        {
            var freshData = JObject.Parse(updatedFurnitureData);
            productsArray.Add(freshData);
            productFilejsonObj["Products"] = productsArray;
            string newJsonResult = JsonConvert.SerializeObject(productFilejsonObj, Formatting.Indented);
            ProjectManager.productFileJSONData = newJsonResult;
            SaveToProductsJson();
        }
        else
        {
            return;
        }
    }

    public static void RemoveElementById(JObject jsonObject, int idToRemove)
    {
        JArray productsArray = (JArray)jsonObject["Products"];

        // Find and remove the element with the specified id
        foreach (JObject product in productsArray)
        {
            if (product["id"].Value<int>() == idToRemove)
            {
                product.Remove();
                break;
            }
        }
    }

    bool CheckProduct(JArray productsArray, int desiredId)
    {
        // Iterate through the "Products" array and check if the desired id is present
        foreach (JToken product in productsArray.Where(obj => obj["id"].Value<int>() == desiredId))
        {
            return true;
        }

        return false;
    }

    int deleteProductID;
    void DeleteProduct(int productID)
    {
        deleteProductID = productID;
        deleteConfirmationPanel.SetActive(true);
    }

    void Delete()
    {
        int deleteIndex = -1;
        ////#if UNITY_EDITOR
        ////            /* this is for local save fuctionality */
        ////            var jsonString = File.ReadAllText(filePath);
        ////            var jObject = JObject.Parse(jsonString);
        ////            JArray productsArrary = (JArray)jObject["Products"];
        ////#endif

        /* this is for server save fuctionality */
        var jObject = JObject.Parse(userSpecificJSONData);
        JArray productsArrary = (JArray)jObject["Products"];

        var product = productsArrary.FirstOrDefault(obj => obj["id"].Value<int>() == deleteProductID);
        productsArrary.Remove(product);
        string output = JsonConvert.SerializeObject(jObject, Formatting.Indented);
        userSpecificJSONData = output;
        SaveUserSpecificJSONData();

        foreach (Transform child in productsHolder.transform)
        {
            deleteIndex++;
            if (child.gameObject.activeSelf)
            {
                spawnPoints.RemoveAt(deleteIndex);
                DestroyImmediate(child.gameObject);
                numberOfProducts--;
                deleteConfirmationPanel.SetActive(false);
                deleteIndex = -1;
                break;
            }
        }

        if (productsHolder.transform.childCount > 0)
        {
            currentIndex = -1;
            NextProduct();
        }
        else
        {
            smoothOrbitCam.target = null;
            previous.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            note.text = "No designs found for the designer\n...";
        }
    }

    void CancelDelete()
    {
        deleteConfirmationPanel.SetActive(false);
    }

    public void SaveUserSpecificJSONData()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.UpdateList(userSpecificJSONData, currentFileID, AddedResponseData);
    }

    private void AddedResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            //print("Action taken successfully!");
        }
        else
        {
            print("Something went wrong");
        }
    }

    public void SaveToProductsJson()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.UpdateList(ProjectManager.productFileJSONData, 1, AddedResponseData);
    }

    public void StopPresentation()
    {
        foreach (GameObject spawnPoint in spawnPoints)
        {
            DestroyImmediate(spawnPoint);
        }
        currentFileID = 0;
        spawnPoints.Clear();
    }
}
