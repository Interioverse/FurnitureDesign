using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNotification : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject notificationPrefab;
    [SerializeField] TextMeshProUGUI notificationText;

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        notificationText.text = newPlayer.NickName + " has joined the room";
        notificationPrefab.SetActive(true);
        StartCoroutine(InactivateNotification());
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        notificationText.text = otherPlayer.NickName + " has left the room";
        notificationPrefab.SetActive(true);
        StartCoroutine(InactivateNotification());
    }

    IEnumerator InactivateNotification()
    {
        yield return new WaitForSeconds(3);
        notificationPrefab.SetActive(false);
        StopCoroutine("InactivateNotification");
    }
}