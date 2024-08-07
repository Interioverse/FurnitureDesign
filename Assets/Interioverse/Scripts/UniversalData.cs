using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class UniversalData : MonoBehaviour
{
    public static UniversalData Instance;
    public GameObject welcomePanel, scrollBar;
    public GameObject loadingScene, designScene, adminScene, noInternetScene;
    //[SerializeField] Product product;
    public static bool loginStatus, guestLogin, isAdmin, checkAdmin, isDesigner;
    public static string userName, fileName, userData, valid_token, userFileID;
    public static int userId;
    public VideoPlayer videoPlayer;
    public static bool smartDevice;

    public const string laminatesMaterialsPath = "Laminates";
    public const string wallMaterialsPath = "Wall Designs";

    //public const string abstractMatPath = "Laminates/Abstract";
    //public const string glossyMatPath = "Laminates/Glossy";
    //public const string matteSolidMatPath = "Laminates/MatteSolids";
    //public const string woodGrainsMatPath = "Laminates/WoodGrains";
    //public Material[] abstractMaterials, glossyMaterials, matteSolidMaterials, woodGrainsMaterials;

    public Material[] laminatesMaterials, wallMaterials;
    [SerializeField] Button enter;

    //internal static string fileID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitialCall();

        enter.onClick.AddListener(() => Enter());
    }

    public void InitialCall()
    {
        enter.gameObject.SetActive(false);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            videoPlayer.Stop();
            noInternetScene.SetActive(true);
        }
        else
        {
            videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "Playback video.mp4");
            videoPlayer.Play();
            noInternetScene.SetActive(false);
            DownloadData();
        }
    }

    private void DownloadData()
    {
        HttpAWSconnect.Instance.GetFileData(0, UserData);
        laminatesMaterials = Resources.LoadAll<Material>(laminatesMaterialsPath);
        wallMaterials = Resources.LoadAll<Material>(wallMaterialsPath);

        //abstractMaterials = Resources.LoadAll<Material>(abstractMatPath);
        //glossyMaterials = Resources.LoadAll<Material>(glossyMatPath);
        //matteSolidMaterials = Resources.LoadAll<Material>(matteSolidMatPath);
        //woodGrainsMaterials = Resources.LoadAll<Material>(woodGrainsMatPath);
    }

    void UserData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            JObject dataObject = (JObject)JObject.Parse(data)["data"];
            string fileData = dataObject["file"].Value<string>();
            JObject fileObject = JObject.Parse(fileData);
            JArray usersArray = (JArray)fileObject["Users"];
            userData = "{\"Users\":" + usersArray + "}".ToString();
        }
        else
        {
            userData = "{\"Users\":[]}";
        }

        dataDownloaded = true;
        BothExecuted();
    }

    public bool dataDownloaded, playerSpawned;

    public void BothExecuted()
    {
        //if (dataDownloaded && playerSpawned)
        if (dataDownloaded)
        {
            scrollBar.SetActive(false);
            enter.gameObject.SetActive(true);
        }
    }

    internal void LoadMainScene()
    {
        loadingScene.SetActive(true);
        designScene.SetActive(false);
        adminScene.SetActive(false);
    }

    internal void LoadDesignScene()
    {
        //SceneManager.LoadScene(1);
        loadingScene.SetActive(false);
        designScene.SetActive(true);
        adminScene.SetActive(false);
    }

    internal void LoadAdminScene()
    {
        loadingScene.SetActive(false);
        designScene.SetActive(false);
        adminScene.SetActive(true);
    }

    public void Enter()
    {
        welcomePanel.SetActive(false);
        ProjectManager.Instance.CallFromData();
    }

    public void CheckConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            noInternetScene.SetActive(true);
            return;
        }
        noInternetScene.SetActive(false);
    }
}