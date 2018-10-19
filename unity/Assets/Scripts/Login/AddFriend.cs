using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriend : MonoBehaviour {
    public Text FriendToAdd;
    public WWWForm form;


    public void AddAFriend()
    {
        StartCoroutine(AddFirstFriendRelationToDB());
    }

    public IEnumerator AddFirstFriendRelationToDB()
    {
        string User1Email = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().UserEmail;
        string User2Email = FriendToAdd.text.ToString();

        form = new WWWForm();
        form.AddField("user1", User1Email);
        form.AddField("user2", User2Email);

        WWW w = new WWW("http://localhost:8080/action_add_friend.php", form);
        yield return w;
        Debug.Log(w.text.ToString());
        if (string.IsNullOrEmpty(w.error))
        {
            //success...
            if (w.text.Contains("Wrong username or password"))
            {
            }
           
        }
        else
        {
            //error
            //Text_Login_Feedback.text = "Došlo je do greške pri unosu. Proverite unete informacije.";
        }
        StartCoroutine(AddSecondFriendRelationToDB()); 

    }

    public IEnumerator AddSecondFriendRelationToDB()
    {
        string User1Email = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().UserEmail;
        string User2Email = FriendToAdd.text.ToString();

        form = new WWWForm();
        form.AddField("user1", User2Email);
        form.AddField("user2", User1Email);

        WWW w = new WWW("http://localhost:8080/action_add_friend.php", form);
        yield return w;
        Debug.Log(w.text.ToString());
        if (string.IsNullOrEmpty(w.error))
        {
            //success...
            if (w.text.Contains("Wrong username or password"))
            {

            }

        }
        else
        {
            //error
            //Text_Login_Feedback.text = "Došlo je do greške pri unosu. Proverite unete informacije.";
        }
        DeleteFriendReqFromDB();

    }
    public IEnumerator DeleteFrFromDB()
    {
        string User1Email = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().UserEmail;
        string User2Email = FriendToAdd.text.ToString();

        form = new WWWForm();
        form.AddField("RecEmail", User1Email);
        form.AddField("SenEmail", User2Email);

        WWW w = new WWW("http://localhost:8080/delete_friend_req", form);
        yield return w;
        Debug.Log(w.text.ToString());
        if (string.IsNullOrEmpty(w.error))
        {
            //success...


        }
        else
        {
            //error
            //Text_Login_Feedback.text = "Došlo je do greške pri unosu. Proverite unete informacije.";
        }

    }
    public void DeleteFriendReqFromDB() {
        StartCoroutine(DeleteFrFromDB());
        this.transform.parent.gameObject.SetActive(false);
    }


    //LOBBY-----------------------------------------------------------------
    public void OnClickJoinLobby()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        PhotonNetwork.automaticallySyncScene = false;

        GameObject lobbyCanvasObj = MainCanvasManager.Instance.LobbyCanvas.gameObject;
        GameObject roomCanvasObj = MainCanvasManager.Instance.CurrentRoomCanvas.gameObject;
      

        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();
        CurrentRoomCanvas roomCanvas = roomCanvasObj.GetComponent<CurrentRoomCanvas>();
        lobbyCanvas.gameObject.SetActive(true);
        roomCanvas.gameObject.SetActive(true);

    }
    public void OnClickLeaveLobby()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.LobbyCanvas.gameObject;
        GameObject roomCanvasObj = MainCanvasManager.Instance.CurrentRoomCanvas.gameObject;
       

        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();
        CurrentRoomCanvas roomCanvas = roomCanvasObj.GetComponent<CurrentRoomCanvas>();
        lobbyCanvas.gameObject.SetActive(false);
        roomCanvas.gameObject.SetActive(false);
    }

    private void OnJoinedLobby()
    {
        Debug.Log("Joined lobby.");

    }
    private void OnReceivedRoomListUpdate()
    {
        PhotonNetwork.automaticallySyncScene = false;
       // PhotonNetwork.JoinLobby(TypedLobby.Default);
    }


}
