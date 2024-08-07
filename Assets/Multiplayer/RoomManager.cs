using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class RoomManager : MonoBehaviourPunCallbacks
{
    GameObject player, nonNetworkUser;
    [SerializeField] PhotonView playerPrefab;
    [SerializeField] TextMeshProUGUI statusText, roomName;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] Button createRoom, joinButton, back;
    public Button leaveRoom;
    [SerializeField] GameObject scrollBar, leaveRoomPanel, loading;
    [SerializeField] Camera tempCamera;
    [SerializeField] MoveObject moveObject;
    [SerializeField] MovePlayer[] movePlayers;
    //[SerializeField] DesktopPlayerMovement desktopPlayerMovement;

    private void Awake()
    {
        PhotonNetwork.NickName = UniversalData.userName;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        createRoom.onClick.AddListener(CreatePrivateRoom);
        joinButton.onClick.AddListener(JoinPrivateRoom);
        leaveRoom.onClick.AddListener(LeaveRoom);
        back.onClick.AddListener(Back);
        loading.SetActive(true);
    }

    private static bool CheckIsReady()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.NickName = UniversalData.userName;
            PhotonNetwork.ConnectUsingSettings();
        }

        return PhotonNetwork.IsConnectedAndReady;
    }

    #region Photon Callback Methods

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        scrollBar.SetActive(false);
        InputControl(true);
        HandleStatusText(Color.red, "Room does not exist");
    }

    public override void OnConnected()
    {
        Debug.Log("OnConnected is called. The server is available!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        loading.SetActive(false);
        HandleStatusText(Color.white, "");
        back.gameObject.SetActive(true);
        scrollBar.SetActive(false);
        InputControl(true);
    }

    private void Back()
    {
        scrollBar.SetActive(false);
    }

    private void InputControl(bool value)
    {
        roomNameInputField.gameObject.SetActive(value);
        back.gameObject.SetActive(value);

        if (UniversalData.isDesigner)
        {
            createRoom.gameObject.SetActive(value);
            joinButton.gameObject.SetActive(value);
        }
        else
        {
            joinButton.gameObject.SetActive(value);
        }
    }

    public override void OnCreatedRoom()
    {
        //Debug.Log("A room is created with the name: " + PhotonNetwork.CurrentRoom.Name);
        HandleStatusText(Color.white, "Room created please enter");
    }
    #endregion

    #region Public Methods

    void CreatePrivateRoom()
    {
        if (roomNameInputField.text == "" || roomNameInputField.text == null)
        {
            HandleStatusText(Color.red, "Room name should not be empty");
            return;
        }
        else if (roomNameInputField.text.Length < 4)
        {
            HandleStatusText(Color.red, "Please enter minimum 4 characters");
            return;
        }
        StartCoroutine("PrivateRoom");
    }

    IEnumerator PrivateRoom()
    {
        bool roomCreated = false;
        string roomName = roomNameInputField.text;
        byte maxPlayers;
        maxPlayers = (byte)8;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 1000 };
        scrollBar.SetActive(true);
        InputControl(false);

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);

        //PhotonNetwork.CreateRoom(roomName, roomOptions, null);

        // Wait for room creation with a timeout of 10 seconds
        float startTime = Time.time;
        while (!roomCreated && Time.time - startTime < 15f)
        {
            if (PhotonNetwork.InRoom)
            {
                // Room created successfully
                roomCreated = true;
            }
            yield return null; // Wait for the next frame
        }

        // If room creation failed or exceeded the timeout, log an error
        if (!roomCreated)
        {
            scrollBar.SetActive(false);
            InputControl(true);
            HandleStatusText(Color.red, "Room creation failed, please refresh and try again");
        }
    }

    public override void OnJoinedRoom()
    {
        leaveRoom.gameObject.SetActive(true);
        scrollBar.SetActive(false);
        InputControl(false);
        //string joinedRoomName = PhotonNetwork.CurrentRoom.Name;
        roomName.text = "Room name:" +"\n"+ PhotonNetwork.CurrentRoom.Name;
        roomName.gameObject.SetActive(true);

        nonNetworkUser = ProjectManager.Instance.desktopPlayerMovement.gameObject;
        DestroyImmediate(nonNetworkUser);

        player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity) as GameObject;
        //player.GetComponent<DesktopPlayerMovement>().roomManager = this;
        //desktopPlayerMovement = player.GetComponent<DesktopPlayerMovement>();
        if (UniversalData.isDesigner)
        {
            player.name = "Network Player " + UniversalData.userId.ToString();
        }
        else
        {
            player.name = UniversalData.userName;
        }

        UniversalData.Instance.playerSpawned = true;
        ProjectManager.Instance.PrivateRoom(true);

        movePlayers = FindObjectsOfType<MovePlayer>();
        foreach (MovePlayer movePlayer in movePlayers)
        {
            movePlayer.UpdateTheObjectandDPM(player);
        }
        //UniversalData.Instance.BothExecuted();
    }

    //public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    //{
    //    string username = newPlayer.NickName;
    //    GameObject notification = Instantiate(notificationPrefab, Vector3.zero, Quaternion.identity);
    //    notification.GetComponentInChildren<Text>().text = username;

    //    Destroy(notification, 5f);
    //}

    //public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    //{
    //    Debug.Log("Left room");
    //}

    public void LeaveRoom()
    {
        if (CheckIsReady())
        {
            //Check if presentation is ON if Yes then close it and then leave the room
            if (UniversalData.isDesigner)
            {
                player.GetComponent<DesktopPlayerMovement>().Close();
            }

            PhotonNetwork.LeaveRoom();
            leaveRoom.gameObject.SetActive(false);
            roomName.text = "";
            roomName.gameObject.SetActive(false);
            if (player)
            {
                DestroyImmediate(player);
            }
            UniversalData.Instance.playerSpawned = false;

            tempCamera.gameObject.SetActive(true);
            leaveRoomPanel.SetActive(true);
            InputControl(true);
            Invoke("GenerateUserPrefab", 1f);
        }
        else
        {
            LeaveRoom();
        }
    }

    void GenerateUserPrefab()
    {
        //ProjectManager.Instance.GenerateUserPrefab(playerLastLocation, playerLastRotation);
        if (ProjectManager.Instance.GenerateUserPrefab(movePlayers))
        {
            leaveRoomPanel.SetActive(false);
            tempCamera.gameObject.SetActive(false);
            ProjectManager.Instance.PrivateRoom(false);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        scrollBar.SetActive(false);
        InputControl(true);
        HandleStatusText(Color.red, "Room creation failed, please try again");
    }

    public void JoinPrivateRoom()
    {
        if (roomNameInputField.text == "" || roomNameInputField.text == null)
        {
            HandleStatusText(Color.red, "Please enter private room code");
            return;
            //PhotonNetwork.JoinOrCreateRoom(defaultRoom, new RoomOptions { IsVisible = false, MaxPlayers = 10 }, TypedLobby.Default);
        }
        else
        {
            scrollBar.SetActive(true);
            string roomName = roomNameInputField.text;
            InputControl(false);
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    void HandleStatusText(Color textColor, string message)
    {
        statusText.color = textColor;
        statusText.text = message;
        Invoke("ClearStatusText", 2);
    }

    void ClearStatusText()
    {
        statusText.color = Color.white;
        statusText.text = "";
    }

    //public void LoadLoginScene()
    //{
    //    StartCoroutine(LoadYourAsyncScene());
    //    //SceneManager.LoadScene("LoginScene");
    //}

    //IEnumerator LoadYourAsyncScene()
    //{
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LoginScene");
    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //}

    #endregion
}
