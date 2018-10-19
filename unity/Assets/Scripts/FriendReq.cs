using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendReq : MonoBehaviour {
    public GetFriendReqs GFReq;
	// Use this for initialization
	void Start () {
        GFReq = GameObject.FindGameObjectWithTag("NotificationArea").GetComponent<GetFriendReqs>();
        this.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("NotificationArea").transform);
        this.transform.localScale = new Vector3(1, 1, 1);
        if (GFReq.FriendReqEmails.Count != 0)
        {
            this.transform.GetChild(1).GetComponent<Text>().text = GFReq.FriendReqEmails[0];
            GFReq.FriendReqEmails.RemoveAt(0);
        }

    }
	
	// Update is called once per frame

}
