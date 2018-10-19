using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendFriendReqs : MonoBehaviour {
    public Text RecieverEmail;
    public InputField ReqTextToClear;
    // Use this for initialization

    public void SendReq()
    {
        StartCoroutine(SendFriendRequest());
    }
    public IEnumerator SendFriendRequest()
    {
        string sender = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().UserEmail;
        string reciever = RecieverEmail.text;

        WWWForm form = new WWWForm();
        form.AddField("RecEmail", reciever);
        form.AddField("SenEmail", sender);

        WWW w = new WWW("http://localhost:8080/action_send_friend_req.php", form);
        yield return w;
        Debug.Log(w.text.ToString());
        if (string.IsNullOrEmpty(w.error))
        {
            if (w.text.Contains("User does not exist"))
            {
                
            }
            if(w.text.Contains("Req already sent!"))
            {
                             
            }
            if(w.text.Contains("Req sent!"))
            {

            }
        }

        ReqTextToClear.text = "";
    }
}
