using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Friend : MonoBehaviour {
    private MainMenuUI MMUI;
    public Color OnlineColor;
    public GameObject FriendsArea;
    // Use this for initialization
    void Start()
    {
        MMUI = GameObject.FindGameObjectWithTag("MainMenuUIScript").GetComponent<MainMenuUI>();
        this.transform.GetChild(1).gameObject.GetComponent<Text>().text = MMUI.FriendEmails[0];
        MMUI.FriendEmails.RemoveAt(0);

        if (MMUI.FriendOnlineStatuses[0])
        {
            this.transform.GetChild(2).gameObject.GetComponent<Image>().color = OnlineColor;
        }
        MMUI.FriendOnlineStatuses.RemoveAt(0);

        this.transform.SetParent(GameObject.FindGameObjectWithTag("FriendsArea").gameObject.transform);
        this.transform.localScale = new Vector3(1, 1, 1);

        

    }
	
	// Update is called once per frame

}
