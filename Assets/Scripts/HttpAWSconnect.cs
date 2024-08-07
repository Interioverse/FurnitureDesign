using System;
using System.Collections;
using System.Text;
using Interioverse;
using UnityEngine;
using UnityEngine.Networking;

public class HttpAWSconnect : SingletonComponent<HttpAWSconnect>
{
    internal void GetFileData(int fileId, Action<string, UnityWebRequest.Result> loaddata)
    {
        StartCoroutine(getfile(fileId, loaddata));
    }

    IEnumerator getfile(int fileId, Action<string, UnityWebRequest.Result> data)
    {
        //string oldurl = "http://16.16.185.171:3000/api/common/get-data/"+fileId;
        string url = "https://backend.interioverse.io/api/common/get-data/" + fileId;

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        while (!request.isDone)
        {
            float progress = Mathf.Clamp01(request.downloadProgress);
            Debug.Log("Downloaded " + progress);
            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            data(request.downloadHandler.text, request.result);
        }
        else
        {
            data(request.error, request.result);
        }
    }

    //public void CreateUserSpecificProductsData(string UserData, int fileId, Action<string, UnityWebRequest.Result> ResponseData)
    //{
    //    StartCoroutine(Create(UserData, fileId, ResponseData));
    //}


    public void UpdateList(string UserData , int fileId, Action<string, UnityWebRequest.Result> ResponseData)
    {
        StartCoroutine(Create(UserData, fileId, ResponseData));
    }

    IEnumerator Create(string Userdata , int fileId, Action<string, UnityWebRequest.Result> data)
    {
        //string oldurl = "http://16.16.185.171:3000/api/common/file-upload";
        string url = "https://backend.interioverse.io/api/common/file-upload";
    
        WWWForm formData = new WWWForm();
        formData.AddField("file", Userdata);
        formData.AddField("userId", fileId);

        using (UnityWebRequest request = UnityWebRequest.Post(url, formData))
        {
          
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                data(request.downloadHandler.text, request.result);
            }
            else
            {
                data(request.error, request.result);
            }
        }
    }


    public void DeleteUserData(int userId, Action<string, UnityWebRequest.Result> ResponseData)
    {
        if (userId == 1 || userId == 0)
        {
            return;
        }
        StartCoroutine(DeleteData(userId, ResponseData));
    }

    IEnumerator DeleteData(int userId, Action<string, UnityWebRequest.Result> data)
    {
        //string oldurl = "http://16.16.185.171:3000/api/common/deleteData/"+fileId;
        string deleteDataURL = "https://backend.interioverse.io/api/common/delete-data/" + userId;

        UnityWebRequest request = UnityWebRequest.Get(deleteDataURL);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            data(request.downloadHandler.text, request.result);
        }
        else
        {
            data(request.error, request.result);
        }
    }
}
