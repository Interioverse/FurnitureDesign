using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Interioverse;

public class SessionController : SingletonComponent<SessionController>
{
    public void GetAccess(Action<string> result)
    {
        StartCoroutine(GetAccessToken(result));
    }

    private IEnumerator GetAccessToken(Action<string> result)
    {
        Dictionary<string, string> content = new Dictionary<string, string>();

        content.Add("client_id", "xjfcn93xej5pj0t0bbnoyvf2og9h5f9g");
        content.Add("client_secret", "Ufg6ST4y3AFahBbewgUi79eFaGDL70DJ");
        content.Add("grant_type", "client_credentials");
        content.Add("box_subject_type", "user");
        content.Add("box_subject_id", "308239208");

        UnityWebRequest request = UnityWebRequest.Post("https://api.box.com/oauth2/token", content);
        //Send request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string resultContent = request.downloadHandler.text;
            Token json = JsonUtility.FromJson<Token>(resultContent);
            result(json.access_token);
        }
        else
        {
            result("");
        }
    }
}
