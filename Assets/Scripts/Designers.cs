using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

public class Designers : MonoBehaviour
{
    public GameObject userPrefab;
    //private string filePath;
    [SerializeField] Transform content;
    UserWrapper userWrapper;
    [SerializeField] GameObject deletePanel;
    [SerializeField] Button Yes, No, createDesignerButton, updateDesinerButton;
    [SerializeField] GameObject adminActivities;
    [SerializeField] SignupManager signupManager;
    [SerializeField] TextMeshProUGUI headText;
    [SerializeField] TextMeshProUGUI statusText;

    private void Awake()
    {
        if (updateDesinerButton)
        {
            updateDesinerButton.onClick.AddListener(() => UpdateUserData());
        }
    }

    private void Start()
    {
//#if UNITY_EDITOR
//        filePath = Path.Combine(Application.streamingAssetsPath, "Users.json");
//        string jsonData = File.ReadAllText(filePath);
//#endif
        userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        List<User> nonAdminUsers = userWrapper.Users.FindAll(user => !user.isAdmin);

        if (nonAdminUsers.Count <= 0)
        {
            //noUsersFound.SetActive(true);
            return;
        }

        //noUsersFound.SetActive(false);
        foreach (User user in nonAdminUsers)
        {
            SpawnUserPrefab(user);
        }
    }

    public void OpenAdminActivities()
    {
        headText.text = "Create user";
        createDesignerButton.gameObject.SetActive(true);
        updateDesinerButton.gameObject.SetActive(false);
        adminActivities.SetActive(true);
    }

    public void CloseAdminActivities()
    {
        signupManager.ClearFields();
        adminActivities.SetActive(false);
    }

    internal void AddCard(User newUser)
    {
        userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        SpawnUserPrefab(newUser);
    }

    void SpawnUserPrefab(User newUser)
    {
        GameObject newUserCard = Instantiate(userPrefab, content);
        newUserCard.name = newUser.userName;
        newUserCard.GetComponent<HoverImageController>().userFileID = newUser.fileId;
        newUserCard.GetComponent<HoverImageController>().userEmail = newUser.email;
        TextMeshProUGUI nameText = newUserCard.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        //TextMeshProUGUI userIDText = newUser.transform.Find("userID").GetComponent<TextMeshProUGUI>();
        Button deleteButton = newUserCard.transform.Find("Delete").GetComponent<Button>();
        Button updateButton = newUserCard.transform.Find("Update").GetComponent<Button>();

        nameText.text = newUser.userName;
        //userIDText.text = userID.ToString();

        deleteButton.onClick.AddListener(() => DeleteUser(newUserCard, newUser));
        updateButton.onClick.AddListener(() => OpenUpdateScreen(newUser.id));
    }

    private void OpenUpdateScreen(int userID)
    {
        headText.text = "Update user";
        createDesignerButton.gameObject.SetActive(false);
        updateDesinerButton.gameObject.SetActive(true);
        adminActivities.SetActive(true);

        var jsonObj = JObject.Parse(UniversalData.userData);
        JArray usersArrary = (JArray)jsonObj["Users"];

        foreach (var user in usersArrary.Where(obj => obj["id"].Value<int>() == userID))
        {
            signupManager.userName.text = user["userName"].ToString();
            signupManager.email.text = user["email"].ToString();
            signupManager.phoneNumber.text = user["phoneNumber"].ToString();
            signupManager.password.text = user["password"].ToString();
            signupManager.cnfPassword.text = user["password"].ToString();
            userId = userID;
        }
    }

    int userId;

    void UpdateUserData()
    {
        signupManager.UpdateDesignerAccount(userId);
    }

    private void DeleteUser(GameObject newUser, User _user)
    {
        deletePanel.SetActive(true);
        Yes.onClick.AddListener(() => Delete(newUser, _user));
        No.onClick.AddListener(() => {
            deletePanel.SetActive(false);
            return;
        });
    }

    string updatedJsonData;
    GameObject deleteUser;

    void Delete(GameObject newUser, User _user)
    {
        Yes.interactable = No.interactable = false;
        int userIndex = userWrapper.Users.FindIndex(user => user.id == _user.id);

        if (userIndex != -1)
        {
            userWrapper.Users.RemoveAt(userIndex);
            updatedJsonData = JsonUtility.ToJson(userWrapper);
            deleteUser = newUser;
            userId = _user.id;
            //#if UNITY_EDITOR
            //            File.WriteAllText(filePath, updatedJsonData);
            //#endif
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UniversalData.Instance.noInternetScene.SetActive(true);
                return;
            }
            UniversalData.Instance.noInternetScene.SetActive(false);
            HttpAWSconnect.Instance.UpdateList(updatedJsonData, 0, ResponseData);       
        }
        else
        {
            //Debug.Log($"User with ID {_user.id} not found!");
        }
    }

    void ResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            UniversalData.userData = updatedJsonData;
            statusText.color = Color.green;
            statusText.text = "User deleted successfully";
            //Invoke("ClearText", 2);

            /* Delete the product file created */
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UniversalData.Instance.noInternetScene.SetActive(true);
                return;
            }
            UniversalData.Instance.noInternetScene.SetActive(false);
            HttpAWSconnect.Instance.DeleteUserData(userId, DeleteResponseData);
        }
        else
        {
            statusText.color = Color.red;
            statusText.text = "Something went wrong, user not deleted";
            Invoke("ClearText", 2);
        }
    }

    private void DeleteResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            statusText.color = Color.green;
            statusText.text = "User data deleted successfully";
            Invoke("ClearText", 2);
        }
        else
        {
            statusText.color = Color.red;
            statusText.text = result + " user data not deleted";
            Invoke("ClearText", 2);
        }
    }

    void ClearText()
    {
        deletePanel.SetActive(false);
        DestroyImmediate(deleteUser);
        Yes.interactable = No.interactable = true;

        statusText.color = Color.black;
        statusText.text = "Once you delete, data will be lost permanently";
    }
}
