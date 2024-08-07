using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Photon.Voice.Unity;

public class DesktopPlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 50f;
    public Camera mainCamera;
    public bool networkPlayer;
    [SerializeField] AudioSource audioSource;
    //public RoomManager roomManager;
    [SerializeField] Button micriphoneButton, getDesigns, close;
    //[SerializeField] DesignersProducts designersProducts;
    [SerializeField] GameObject slides;
    [SerializeField] Transform controlCanvas;
    [SerializeField] Vector3 controlCanvasPosition;
    [SerializeField] TextMeshProUGUI networkName;
    [SerializeField] Presentation presentation;

    private void Awake()
    {
        /* Photon view code - do not delete */
        if (networkPlayer)
        {
            //roomManager = FindObjectOfType<RoomManager>();
            transform.parent = GameObject.Find("Loading Scene").transform;
            transform.localPosition = new Vector3(3, -0.14f, -12);
            transform.localEulerAngles = new Vector3(0, 270, 0);
            //designersProducts = FindObjectOfType<DesignersProducts>();
            micriphoneButton.onClick.AddListener(MuteUnmuteControl);

            if (photonView.IsMine)
            {   
                ProjectManager.Instance.loginPanel.SetActive(false);
                ProjectManager.Instance.desktopPlayerMovement = this;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().isKinematic = false;
                transform.GetChild(0).gameObject.SetActive(true);
                //GameObject sensorObj = GameObject.Find("Sensors Two")as GameObject;
                //sensorObj.GetComponent<MoveObject>().CommonCode(2, false);
                ProjectManager.Instance.ClosePrivatePanel();
                //UniversalData.Instance.playerSpawned = true;
                //UniversalData.Instance.BothExecuted();

                controlCanvas.localPosition = controlCanvasPosition;
                networkName.transform.parent.gameObject.SetActive(false);

                if (UniversalData.isDesigner)
                {
                    getDesigns.gameObject.SetActive(true);
                    close.gameObject.SetActive(true);
                }
            }

            networkName.text = photonView.Owner.NickName;
            //previous.onClick.AddListener(Previous);
            //next.onClick.AddListener(Next);
            close.onClick.AddListener(Close);
            getDesigns.onClick.AddListener(GetDesigns);
        }
    }

    #region RPC methods

    public void Close()
    {
        photonView.RPC("CloseDesign", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void CloseDesign()
    {
        presentation.StopPresentation();
    }

    int designerID;

    void GetDesigns()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        designerID = UniversalData.userId;
        if (productsData == "")
        {
            HttpAWSconnect.Instance.GetFileData(designerID, ProductsData);
        }
        else
        {
            Debug.Log("Return");
            return;
        }
    }

    #endregion

    string productsData = "";

    void ProductsData(string data, UnityWebRequest.Result result)
    {
        JObject parsedResponse = JsonConvert.DeserializeObject<JObject>(data);
        JToken dataToken = parsedResponse["data"];
        if (dataToken.Type == JTokenType.Null)
        {
            //productsData = "{\"Products\":[]}";
            productsData = "";
            return;
        }

        if (result == UnityWebRequest.Result.Success)
        {
            JObject dataObject = (JObject)JObject.Parse(data)["data"];
            string fileData = dataObject["file"].Value<string>();
            JObject fileObject = JObject.Parse(fileData);
            JArray productsArray = (JArray)fileObject["Products"];
            productsData = "{\"Products\":" + productsArray + "}".ToString();
        }
        else
        {
            //productsData = "{\"Products\":[]}";
            productsData = "";
            return;
        }

        JObject jsonObj = JObject.Parse(productsData);
        presentation.GetAllMyProducts(jsonObj, productsData);
    }

    void Update()
    {
        /*Photon view code - do not delete */
        if (networkPlayer)
        {
            if (photonView.IsMine)
            {
                CommonMovement();
            }
        }
        else
        {
            CommonMovement();
        }
    }

    private void CommonMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.position += transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        transform.Rotate(0f, horizontalInput * rotateSpeed * Time.deltaTime, 0f);
        Vector3 newPosition = transform.position + transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        transform.position = newPosition;

        Quaternion newRotation = transform.rotation * Quaternion.Euler(0f, horizontalInput * rotateSpeed * Time.deltaTime, 0f);
        newRotation.x = 0f;
        newRotation.z = 0f;
        transform.rotation = newRotation;
    }

    [SerializeField] Sprite mute, unmute;
    [SerializeField] private Image microphone;
    [SerializeField] PhotonVoiceView photonVoiceView;
    bool muteMice;

    private void MuteUnmuteControl()
    {
        muteMice = !muteMice;
        photonVoiceView.SpeakerInUse.GetComponent<AudioSource>().enabled = !muteMice;
        photonVoiceView.SpeakerInUse.GetComponent<Speaker>().enabled = !muteMice;
        photonView.RPC("MuteUnmute", RpcTarget.AllBuffered, muteMice);
    }

    [PunRPC]
    public void MuteUnmute(bool _muteMice)
    {
        if (_muteMice)
        {
            microphone.sprite = mute;
        }
        else
        {
            microphone.sprite = unmute;
        }
        photonVoiceView.SpeakerInUse.GetComponent<AudioSource>().enabled = !_muteMice;
        photonVoiceView.SpeakerInUse.GetComponent<Speaker>().enabled = !_muteMice;
    }

    private void OnApplicationQuit()
    {
        if (networkPlayer && photonView.IsMine)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}