using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
public class GetFriendReqs : MonoBehaviour {
    private Text ReqSender;
    public GameObject NotificationArea;
    public GameObject FriendRequest;
    public List<string> FriendReqEmails;
    private int FriendReqsRefreshRate;
    
	// Use this for initialization
	void Start () {
        FriendReqsRefreshRate = 5;
        StartCoroutine(GetFriendRequests());
	}
	
    IEnumerator GetFriendRequests()
    {
        yield return new WaitForSeconds(FriendReqsRefreshRate);
        string userEmail = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().UserEmail;
        FriendReqsRefreshRate = 20;
        WWWForm form = new WWWForm();
        form.AddField("userEmail", userEmail);
        WWW w = new WWW("http://localhost:8080/action_get_friend_req.php", form);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            foreach (GameObject FR in GameObject.FindGameObjectsWithTag("FriendReq"))
            {
                Destroy(FR.gameObject);
            }
            if (w.text.Contains("No friend reqs"))
            {

            }
            else
            {
                

                string result = w.text;
                result = result.Substring(9);
                string regularExpressionPattern = @"\[(.*?)\]";

                Regex re = new Regex(regularExpressionPattern);

                foreach (Match m in re.Matches(result))
                {
                    string EmailResult = m.Value;
                    string EmailResult2 = EmailResult.Substring(2, EmailResult.Length - 2 );
                    EmailResult2 = EmailResult2.Remove(EmailResult2.Length - 2);
                    Debug.Log(EmailResult2);

                    if (!FriendReqEmails.Contains(EmailResult2))
                    {
                        FriendReqEmails.Add(EmailResult2);
                        Instantiate(FriendRequest, NotificationArea.transform.position, Quaternion.identity);
                    }
                }
            }
        }    
        StartCoroutine(GetFriendRequests());
    }
}


	// Update is called once per frame


