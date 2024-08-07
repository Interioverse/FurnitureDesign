//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Interioverse;
//using System;
//using UnityEngine.Networking;
//using System.Text;

//public class HttpConnector : SingletonComponent<HttpConnector>
//{
//    public void GetBoxFolderInformation(string url,string token, Action<string> data)
//    {
//        StartCoroutine(getFileFinfo(url,token, data));
//    }

  
//    IEnumerator getFileFinfo(string uri, string token, Action<string> data)
//    {
//        UnityWebRequest request = UnityWebRequest.Get(uri);
//        request.SetRequestHeader("Content-Type", "application/json");
//        request.SetRequestHeader("Authorization", "Bearer " + token);

//        yield return request.SendWebRequest();

//        if (request.result == UnityWebRequest.Result.Success)
//        {
//             data(request.downloadHandler.text);
//        }
//        else
//        {
//            data("");
//            Debug.Log("Error While Sending: " + request.error);
//        }
//    }


//    public void DowloadFile(string url, string token, Action<byte[]> data)
//    {
//        StartCoroutine(Download(url, token, data));
//    }

//    IEnumerator Download(string uri, string token, Action<byte[]> data)
//    {
//        UnityWebRequest request = UnityWebRequest.Get(uri);
//        request.SetRequestHeader("Content-Type", "application/json");
//        request.SetRequestHeader("Authorization", "Bearer " + token);

//        yield return request.SendWebRequest();

//        if (request.result == UnityWebRequest.Result.Success)
//        {
//            data(request.downloadHandler.data);
//        }
//        else
//        {
//            data(null);
//            Debug.Log("Error While Sending: " + request.error);
//        }
//    }


//    public void SaveNewJsonfiletoBox(string jsonstring,string filename)
//    {
//        StartCoroutine(savetoserver(jsonstring,filename));
//    }

//    IEnumerator savetoserver(string Jsonvalue,string fileName)
//    {
//        string parentFolderId = "208763345630";

//        byte[] fileBytes = Encoding.UTF8.GetBytes(Jsonvalue);

//        string apiUrl = "https://upload.box.com/api/2.0/files/content";

//        WWWForm formData = new WWWForm();
//        formData.AddField("name", fileName);
//        formData.AddField("parent_id", parentFolderId);
//        formData.AddBinaryData("file", fileBytes, fileName, "application/octet-stream");

//        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, formData))
//        {
//            request.SetRequestHeader("Authorization", "Bearer " + UniversalData.valid_token);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                Debug.Log("Response: " + request.downloadHandler.text);
//                Debug.Log("File uploaded successfully!");
//            }
//            else
//            {
//                Debug.LogError("File upload failed. Error: " + request.error);
//            }
//        }
//    }


//    public void UpdateJsonfiletoBox(string jsonstring, string fileID)
//    {
//        StartCoroutine(updatetoserver(jsonstring, fileID));
//    }

//    IEnumerator updatetoserver(string Jsonvalue, string fileID)
//    {
//        byte[] fileBytes = Encoding.UTF8.GetBytes(Jsonvalue);

//        string apiUrl = "https://upload.box.com/api/2.0/files/"+fileID+"/content"; // id Example 1228443754138

//        WWWForm formData = new WWWForm();
//        formData.AddBinaryData("file", fileBytes, "application/json");

//        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, formData))
//        {
//            request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
//            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
//            request.SetRequestHeader("Authorization", "Bearer " + UniversalData.valid_token);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                Debug.Log("File updated successfully!");
//            }
//            else
//            {
//                Debug.LogError("File upload failed. Error: " + request.error);
//            }
//        }
//    }

//    public void CreateUserSpecificProductsData(string jsonstring, string filename, Action<string> data)
//    {
//        StartCoroutine(CreateUserJSONFile(jsonstring, filename, data));
//    }

//    IEnumerator CreateUserJSONFile(string Jsonvalue, string fileName, Action<string> data)
//    {
//        //  string fileName = "anandbiradar002.json";

//        string parentFolderId = "208763345630";

//        byte[] fileBytes = Encoding.UTF8.GetBytes(Jsonvalue);

//        string apiUrl = "https://upload.box.com/api/2.0/files/content";

//        WWWForm formData = new WWWForm();
//        formData.AddField("name", fileName);
//        formData.AddField("parent_id", parentFolderId);
//        formData.AddBinaryData("file", fileBytes, fileName, "application/octet-stream");

//        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, formData))
//        {
//            request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
//            request.SetRequestHeader("Access-Control-Allow-Origin", "*");
//            request.SetRequestHeader("Authorization", "Bearer " + UniversalData.valid_token);

//            yield return request.SendWebRequest();

//            if (request.result == UnityWebRequest.Result.Success)
//            {
//                //Debug.Log("Response: " + request.downloadHandler.text);
//                Debug.Log("File uploaded successfully!");
//                data(request.downloadHandler.text);
//            }
//            else
//            {
//                Debug.LogError("File upload failed. Error: " + request.error);
//                data(request.error);
//            }
//        }
//    }
//}
