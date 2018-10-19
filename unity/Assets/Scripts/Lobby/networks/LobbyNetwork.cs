using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour {

    private void Start()
    {
        if (!PhotonNetwork.connected)
        {
            Debug.Log("Connecting to server...");
            PhotonNetwork.ConnectUsingSettings("Remy");
        }
    }

    private void OnDisconnectedFromPhoton()
    {
        Debug.Log("Reconnecting the Server");
        PhotonNetwork.Reconnect();
    }

    private void OnConnectedToMaster()
    {
        print("Connected to master.");        
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        PhotonNetwork.automaticallySyncScene = false;
    }
   
}

