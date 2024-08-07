using Interioverse;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProjectManager : SingletonComponent<ProjectManager>
{
    [SerializeField] GameObject nonNetworkPlayerPrefab, onScreenControl, indicator, CADETCanvas;
    public DesktopPlayerMovement desktopPlayerMovement;
    [SerializeField] private TextMeshProUGUI loadingText;
    //[SerializeField] private List<GameObject> canvas;
    [SerializeField] private GameObject detailsPanel, RTE;
    [SerializeField] MoveObject moveObject;
    [SerializeField] Button profileButton, designButton, adminPageButton;
    public Material[] laminateMaterials, wallMaterials;
    public GameObject sensorTwo;
    string[] files;

    [Header("Progress Objects")]
    [SerializeField] Image progressbar;
    [SerializeField] TextMeshProUGUI progressTxt;
    //Vector3 targetParentScale, targetScale;
    //bool isTopBottom, isRightLeft;
    bool dataLoaded;

    public static string productFileJSONData;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            onScreenControl.SetActive(true);
            indicator.SetActive(false);
        }

        if (designButton && adminPageButton)
        {
            designButton.onClick.AddListener(() => CreateNewFurniture());
            adminPageButton.onClick.AddListener(() => GoToAdminPage());
        }
    }

    public void CallFromData()
    {
        //#if UNITY_EDITOR
        //        filePath = Application.dataPath + "/StreamingAssets/Products.json";
        //#endif

        /* Photon view code - Do not delete */
        //if (desktopPlayerMovement.GetComponent<PhotonView>().IsMine)
        //{
        //    desktopPlayerMovement.enabled = true;
        //}
        //else
        //{
        //    desktopPlayerMovement.enabled = false;
        //}

        desktopPlayerMovement.enabled = true;
        laminateMaterials = UniversalData.Instance.laminatesMaterials;
        wallMaterials = UniversalData.Instance.wallMaterials;
    }

    public void HandlePrivateRoomEntry()
    {
        desktopPlayerMovement.enabled = false;
        privateRoomPanel.SetActive(true);
    }

    public void ControlPanels(bool value)
    {
        if (value)
        {
            desktopPlayerMovement.enabled = false;
            loginPanel.SetActive(true);
        }
        else
        {
            desktopPlayerMovement.enabled = true;
            loginPanel.SetActive(false);
        }
    }

    public void ClosePrivatePanel()
    {
        desktopPlayerMovement.enabled = true;
        privateRoomPanel.SetActive(false);

        //Vector3 newPosition = desktopPlayerMovement.transform.position + Vector3.right * 25 * Time.deltaTime;
        //desktopPlayerMovement.transform.position = newPosition;
    }

    public GameObject loginPanel;

    public void LoginWithOTP(string _name)
    {
        UniversalData.guestLogin = false;
        UniversalData.isDesigner = false;
        Login(_name);
    }

    public void Login(string _name)
    {
        UniversalData.loginStatus = true;
        ControlPanels(false);
        moveObject.Move();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);

        if (!dataLoaded)
        {
            HttpAWSconnect.Instance.GetFileData(1, ResponseData);
        }
        else
        {
            profileButton.gameObject.SetActive(true);
        }
    }

    internal void GuestLogin(string _name)
    {
        CADETCanvas.SetActive(false);
        UniversalData.guestLogin = true;
        UniversalData.loginStatus = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.GetFileData(1, ResponseData);
    }

    public List<string> fileNames;

    [SerializeField] GameObject topPanel;

    private void ResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            JObject dataObject = (JObject)JObject.Parse(data)["data"];
            string fileData = dataObject["file"].Value<string>();
            if (fileData == null || fileData == "")
            {
                productFileJSONData = "{\"Products\": []}";
            }
            else
            {
                JObject fileObject = JObject.Parse(fileData);
                JArray usersArray = (JArray)fileObject["Products"];
                productFileJSONData = "{\"Products\":" + usersArray + "}".ToString();
                //productFileJSONData = Regex.Replace(productFileJSONData, @"\r\n|\s+", "");
                if (productFileJSONData == null || productFileJSONData == "")
                {
                    productFileJSONData = "{\"Products\": []}";
                }
            }
            LoadTheModelsHere(productFileJSONData);
        }
    }

    internal bool GenerateUserPrefab(MovePlayer[] movePlayers)
    {
        GameObject user = Instantiate(nonNetworkPlayerPrefab, transform.parent.transform) as GameObject;
        user.name = "Guest User";
        desktopPlayerMovement = user.GetComponent<DesktopPlayerMovement>();
        foreach (MovePlayer movePlayer in movePlayers)
        {
            movePlayer.UpdateTheObjectandDPM(user);
        }
        //user.transform.localPosition = playerLastLocation;
        //user.transform.localEulerAngles = playerLastRotation;
        user.transform.localPosition = new Vector3(5, 0.14f, -12);
        user.transform.localEulerAngles = new Vector3(0, 270, 0);
        sensorTwo.SetActive(true);
        return true;
    }

    [SerializeField] Transform[] placesToSpawn;
    [SerializeField] GameObject cube;
    int index;
    int numberOfApprovedFurnitures;

    void LoadTheModelsHere(string data)
    {
        numberOfApprovedFurnitures = 0;
        //#if UNITY_EDITOR
        //        /* this is for Local save fuctionality */
        //        var jsonString = File.ReadAllText(filePath);
        //        var jsonObj = JObject.Parse(jsonString);
        //        JArray productsArrary = (JArray)jsonObj["Products"];
        //#endif

        /* this is for server save fuctionality */
        var jsonObj = JObject.Parse(data);
        JArray productsArrary = (JArray)jsonObj["Products"];

        if (productsArrary.Count > 0)
        {
            foreach (var product in productsArrary)
            {
                if (product["isApproved"].ToObject<int>() == 1)
                {
                    numberOfApprovedFurnitures++;
                }
            }

            if (numberOfApprovedFurnitures > 0)
            {
                foreach (var product in productsArrary)
                {
                    if (product["isApproved"].ToObject<int>() == 1)
                    {
                        if (index > 7)
                        {
                            //Activate the next button to load next furnitures
                            index = 0;
                            break;
                        }

                        JArray componentsArrary = (JArray)product["components"];
                        foreach (var element in componentsArrary)
                        {
                            GameObject newCube = Instantiate(cube, placesToSpawn[index].transform) as GameObject;
                            placesToSpawn[index].name = product["productName"].ToString();
                            newCube.name = "Cube_" + placesToSpawn[index].transform.childCount;
                            newCube.transform.localPosition = new Vector3(element["positionValue"]["x"].ToObject<float>(), element["positionValue"]["y"].ToObject<float>(), element["positionValue"]["z"].ToObject<float>());
                            newCube.transform.localEulerAngles = new Vector3(element["rotationValues"]["x"].ToObject<float>(), element["rotationValues"]["y"].ToObject<float>(), element["rotationValues"]["z"].ToObject<float>());
                            newCube.transform.localScale = new Vector3(element["scaleValues"]["x"].ToObject<float>(), element["scaleValues"]["y"].ToObject<float>(), element["scaleValues"]["z"].ToObject<float>());

                            BGDesign BGD = placesToSpawn[index].GetComponent<BGDesign>();
                            BGD.BG.material = MatchTheMaterial(product["wallDesign"].ToString(), BGD);
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

                                //currentFace = cubeFacePlanes.CreatePlaneFor(propertyName);

                                propertyName = propertyName.Replace(" Face", "");
                                Material newSunMat = currentFace.GetComponent<Renderer>().material;
                                newSunMat.name = (string)propertyObj["sunmicaMatName"];
                                JToken jSunToken = propertyObj["sunmicaMatColor"];
                                //targetScale = currentFace.transform.localScale;
                                foreach (Material _material in laminateMaterials)
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
                                        /* Never delete this line*/
                                        //TextureAdjust(newSunMat, targetScale, targetParentScale, isTopBottom, isRightLeft);
                                        break;
                                    }
                                }
                                newSunMat.color = new Color(jSunToken["r"].ToObject<float>(), jSunToken["g"].ToObject<float>(), jSunToken["b"].ToObject<float>(), jSunToken["a"].ToObject<float>());
                            }

                            newCube.transform.localEulerAngles = originalRotation;

                        }

                        Transform UIplaceToSpawn = placesToSpawn[index].transform.Find("UI Spawner");
                        //Spawn a details panel and add respective details to panel
                        GameObject newPanel = Instantiate(detailsPanel, UIplaceToSpawn) as GameObject;
                        if (!FD)
                        {
                            FD = newPanel.transform.GetChild(0).GetComponent<FurnitureDetails>();
                        }
                        Transform targetBed = placesToSpawn[index].transform;
                        FurnitureDetails _FD = FD;
                        FD._name.text = product["productName"].ToString();
                        //FD._designer.text = product["designerName"].ToString();
                        //FD._category.text = product["furnitureCategory"].ToString();
                        FD._description.text = product["furnitureDetails"].ToString();
                        FD._price.text = product["price"].ToString();
                        FD.ViewIn360.onClick.AddListener(() => ViewIn360Bed(targetBed));
                        //FD.AddToCart.onClick.AddListener(() => AddBedToCart((int)product["id"]));
                        FD.AddToCart.onClick.AddListener(() => AddBedToCart(product, _FD));
                        //FD.RemoveFromCart.onClick.AddListener(() => RemoveFromCart((int)product["id"], (float)product["price"]));
                        FD.BuyBed.onClick.AddListener(() => BuyBed((int)product["id"]));
                        //FD.transform.parent.parent = this.transform;

                        index++;
                        FD = null;

                        UIplaceToSpawn.parent = this.transform;
                    }
                }
                dataLoaded = true;
            }
        }
        else
        {
            Debug.Log(productsArrary.Count);
        }
        desktopPlayerMovement.enabled = true;

        if (UniversalData.isAdmin && UniversalData.isDesigner)
        {
            profileButton.gameObject.SetActive(true);
            adminPageButton.gameObject.SetActive(true);
        }
        else if (UniversalData.isDesigner && !UniversalData.isAdmin)
        {
            profileButton.gameObject.SetActive(true);
            adminPageButton.gameObject.SetActive(false);
        }
        else if (!UniversalData.guestLogin)
        {
            profileButton.gameObject.SetActive(true);
            adminPageButton.gameObject.SetActive(false);
        }

        if (!UniversalData.smartDevice)
        {
            designButton.gameObject.SetActive(true);
        }

        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    designButton.gameObject.SetActive(false);
        //}

        topPanel.SetActive(true);
    }

    public void PrivateRoom(bool value)
    {
        if (value)
        {
            profileButton.gameObject.SetActive(false);
            adminPageButton.gameObject.SetActive(false);
            designButton.gameObject.SetActive(false);
            return;
        }
        else if (UniversalData.isAdmin && UniversalData.isDesigner)
        {
            profileButton.gameObject.SetActive(true);
            adminPageButton.gameObject.SetActive(true);
        }
        else if (UniversalData.isDesigner && !UniversalData.isAdmin)
        {
            profileButton.gameObject.SetActive(true);
            adminPageButton.gameObject.SetActive(false);
        }
        else if (!UniversalData.guestLogin)
        {
            profileButton.gameObject.SetActive(true);
            adminPageButton.gameObject.SetActive(false);
        }

        if (!UniversalData.smartDevice)
        {
            designButton.gameObject.SetActive(true);
        }

        //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    designButton.gameObject.SetActive(false);
        //}
    }

    /* Never delete this line of codes*/
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

    public SmoothOrbitCam smoothOrbitCam;
    public Details details;
    Transform bed;

    //void ViewIn360Bed(Transform theBed, string nm, string prc, string des)
    void ViewIn360Bed(Transform theBed)
    {
        CameraCapture.Instance.CaptureCameraView();
        bed = Instantiate(theBed, smoothOrbitCam.target) as Transform;
        //bed.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        bed.position = new Vector3(0, -0.35f, 0);
        bed.localScale = Vector3.one * 0.6f;
        smoothOrbitCam.showRoom.SetActive(false);
        smoothOrbitCam.View360Room.SetActive(true);
        //details.SetDetails(nm, prc, des);
    }

    public void BackToShowRoom()
    {
        smoothOrbitCam.View360Room.SetActive(false);
        smoothOrbitCam.showRoom.SetActive(true);
        DestroyImmediate(bed.gameObject);
    }

    void BuyBed(int productID)
    {
        //Buy the specific Product
    }

    [SerializeField] UserDatabase userDatabase;
    [SerializeField] GameObject loginScreen;
    [SerializeField] Cart cart;

    void AddBedToCart(JToken product, FurnitureDetails _FD)
    {
        if (!userDatabase)
        {
            userDatabase = FindObjectOfType<UserDatabase>();
        }

        //if user is logged in
        if (!UniversalData.guestLogin)
        {
            if (userDatabase.AddToCart(product) == 1)
            {
                ////Spawn prefab to cart content
                //GameObject itemAdded = Instantiate(cart.itemPrefab, cart.content);
                //itemAdded.name = product["id"].ToString();
                //itemAdded.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = product["productName"].ToString();
                //itemAdded.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = product["price"].ToString() + " INR";
                //itemAdded.transform.Find("Remove").GetComponent<Button>().onClick.AddListener(() => RemoveFromCart((int)product["id"], (float)product["price"]));
                cart.SpawnItem((int)product["id"], product["productName"].ToString(), (float)product["price"]);
                //cart.ChangeTheTotalAmount((float)product["price"]);
                _FD.cartAdded.SetActive(true);
                StartCoroutine(Hide(_FD));
            }
            else if (userDatabase.AddToCart(product) == 0)
            {
                _FD.alreadyInCart.SetActive(true);
                StartCoroutine(Hide(_FD));
            }

            if (!cart.addAddress.gameObject.activeSelf)
            {
                cart.emptyCart.SetActive(false);
                cart.addAddress.gameObject.SetActive(true);
                cart.totalAmount.gameObject.SetActive(true);
            }
        }
        else
        {
            loginPanel.SetActive(true);
            loginScreen.SetActive(true);
        }
    }

    IEnumerator Hide(FurnitureDetails _FD)
    {
        yield return new WaitForSeconds(1);
        _FD.cartAdded.SetActive(false);
        _FD.alreadyInCart.SetActive(false);
        //_FD.gameObject.SetActive(false);
    }

    [SerializeField] TextMeshProUGUI statusText;

    void RemoveFromCart(int productID, float productPrice)
    {
        if (!userDatabase)
        {
            userDatabase = FindObjectOfType<UserDatabase>();
        }
        if (userDatabase.RemoveFromCart(productID) == 1)
        {
            foreach (Transform item in cart.content)
            {
                if (item.name == productID.ToString())
                {
                    DestroyImmediate(item.gameObject);
                    cart.ChangeTheTotalAmount(-productPrice);
                    statusText.color = Color.green;
                    statusText.text = "Item removed from cart successfully!";
                    Invoke("ClearStatus", 2);
                }
            }

            if (cart.content.childCount <= 0)
            {
                cart.emptyCart.SetActive(true);
                cart.addAddress.gameObject.SetActive(false);
                cart.totalAmount.gameObject.SetActive(false);
            }
        }
        else if (userDatabase.RemoveFromCart(productID) == 0)
        {
            statusText.color = Color.red;
            statusText.text = "Item not found in the cart";
            Invoke("ClearStatus", 2);
        }
    }

    void ClearStatus()
    {
        statusText.text = "";
    }

    FurnitureDetails FD;
    [SerializeField] Material assignedMat, defaultMat;
    [SerializeField] GameObject privateRoomPanel;

    Material MatchTheMaterial(string matName, BGDesign BGD)
    {
        assignedMat = defaultMat;
        foreach (Material _material in wallMaterials)
        {
            if (_material.name == matName)
            {
                BGD.myMaterial = _material;
                assignedMat = _material;
                break;
            }
        }
        return assignedMat;
    }

    void CreateNewFurniture()
    {
        UniversalData.Instance.LoadDesignScene();
    }

    void GoToAdminPage()
    {
        UniversalData.Instance.LoadAdminScene();
    }

    public void OpenedInMobile(string platForm)
    {
        UniversalData.smartDevice = true;
        onScreenControl.SetActive(true);
    }
}
