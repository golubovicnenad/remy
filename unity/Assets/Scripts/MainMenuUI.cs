using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class MainMenuUI : MonoBehaviour {
	public UserData UData;
	public Text UserName;
	public Text ShortDescription;
	public int AvatarChoise;
	public int CardDesignChoise;
	public Animator MainMenuAnim;
    public Animator SocialAnim;
	public Text Updated_UserName;
	public Text Updated_ShortDescription;
	public int Updated_AvatarChoise;
	public int Updated_CardDesignChoise;

	public List<Image> Avatars;
	public List<Image> CardDesigns;

	public string UsernameToUpdate;
	public string DescriptionToUpdate;

	public InputField UsernameInput;
	public InputField DescriptionInput;
    // Use this for initialization

    //FRIENDS
    public GameObject FriendsArea;
    public GameObject Friend;
    public List<FriendInfo> FriendsListPhoton;
    public List<string> FriendList;

    public string CurrentFriendEmail;
    public bool CurrentFriendOnlineStats;

    public List<string> FriendEmails;
    public List<bool> FriendOnlineStatuses;
    public int CurrentFriendsCount;
    public bool ShouldCreateFriend;
    public bool friendCheck;

    public int FriendsRefreshRate; //here goes initial value only

    public void ChatFlyIn(){
		MainMenuAnim.SetTrigger ("ChatFlyIn");
	}

	public void ChatFlyOut(){
		MainMenuAnim.SetTrigger ("ChatFlyOut");
	}

    public void SocialFlyIn()
    {
        SocialAnim.SetTrigger("SocialFlyIn");
    }
    public void SocialFlyOut()
    {
        SocialAnim.SetTrigger("SocialFlyOut");
    }


    public void UpdateData(){
		if (Updated_UserName.Equals("")) {
			UsernameToUpdate = UserName.text;
		} else {
			UsernameToUpdate = Updated_UserName.text;
		}
		if(Updated_ShortDescription.Equals("")){
			DescriptionToUpdate = ShortDescription.text;
		} else {
			DescriptionToUpdate = Updated_ShortDescription.text;
		}
		if (Updated_AvatarChoise == 0) {
			Updated_AvatarChoise = UData.Avatar;
		} 
		if (Updated_CardDesignChoise == 0) {
			Updated_CardDesignChoise = UData.CardDesign;
		} 


		StartCoroutine (RequestUserRegistration ());
		UData.UserName = UsernameToUpdate;
		UData.Description = DescriptionToUpdate;
		UData.Avatar = Updated_AvatarChoise;
		UData.CardDesign = Updated_CardDesignChoise;
		RefreshProfile (); // OVO I NE MORA...MOZDA
		// update data in UserData script
	
	}

	public void RefreshProfile(){
		GameObject.FindGameObjectWithTag ("CurrentAvatarChoise").GetComponent<Image> ().sprite = Avatars [UData.Avatar - 1].sprite;
		GameObject.FindGameObjectWithTag ("CardChoiseOutline").GetComponent<Image> ().transform.position = CardDesigns[UData.CardDesign - 1].gameObject.transform.position ;
		UsernameInput.text = "";
		DescriptionInput.text = "";
		UsernameInput.textComponent.text = "";
		DescriptionInput.textComponent.text = "";
		UserName.text = UData.UserName;
		ShortDescription.text = UData.Description;
	}
	public void OpenProfilePanel(){
		GameObject.FindGameObjectWithTag ("ProfilePanel").GetComponent<Animator> ().SetTrigger("ProfilePanelPopUp");
	}
	public void CloseProfilePanel(){
		GameObject.FindGameObjectWithTag ("ProfilePanel").GetComponent<Animator> ().SetTrigger("ProfilePanelClose");
	}

	public IEnumerator RequestUserRegistration(){
		string userEmail = UData.UserEmail;
		string userName = UsernameToUpdate;
		string userDescrtiption = DescriptionToUpdate;
		int userAvatar = Updated_AvatarChoise;
		int userCardDesign = Updated_CardDesignChoise;

		WWWForm form = new WWWForm ();
		form.AddField ("email", userEmail);
		form.AddField ("username", userName);
		form.AddField ("description", userDescrtiption);
		form.AddField ("avatar", userAvatar);
		form.AddField ("card", userCardDesign);
		WWW w = new WWW ("http://localhost:8080/action_update_data.php", form);
		yield return w;
		Debug.Log (w.text);
	}
	void Start () {
		UData = GameObject.FindGameObjectWithTag ("UserData").gameObject.GetComponent<UserData>();
		UserName.text = UData.UserName;
		ShortDescription.text = UData.Description;
		AvatarChoise = UData.Avatar;
		CardDesignChoise = UData.CardDesign;

		RefreshProfile ();
        StartCoroutine(GetFriends());
    }
    public Regex re;
    public string result;
    IEnumerator GetFriends()
    {
        FriendList.Clear();
        string userEmail = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().UserEmail;
        WWWForm form = new WWWForm();
        form.AddField("userEmail", userEmail);
        WWW w = new WWW("http://localhost:8080/get_all_friends.php", form);
        yield return w;
        if (!w.text.Contains("No friends")) { 
            result = w.text;
            result = result.Substring(9);
            string regularExpressionPattern = @"\[(.*?)\]";
            re = new Regex(regularExpressionPattern);
            Debug.Log("re.Matches(result).Count" + re.Matches(result).Count.ToString());
        

        if (re.Matches(result).Count != 0)
        {
            foreach (Match m in re.Matches(result))
            {
                string EmailResult = m.Value;
                string EmailResult2 = EmailResult.Substring(2, EmailResult.Length - 2);
                EmailResult2 = EmailResult2.Remove(EmailResult2.Length - 2);
                Debug.Log("PRIJATELJ: " + EmailResult2);

                if (!FriendList.Contains(EmailResult2))
                {
                    FriendList.Add(EmailResult2);
                }
            }
            string[] friendsList = FriendList.ToArray();
            PhotonNetwork.FindFriends(friendsList);
            } 
        }
        else
        {
            Debug.Log("No friends!");
        }
        StartCoroutine(WaitForFriends());
    }


    IEnumerator WaitForFriends()
    {
        yield return new WaitForSeconds(30);
        if (FriendsArea.transform.childCount != 0) {
            foreach (GameObject Friend in GameObject.FindGameObjectsWithTag("Friend"))
            {
                Destroy(Friend.gameObject);
            }
        }
        if (PhotonNetwork.Friends != null)
        {
            FriendsListPhoton = PhotonNetwork.Friends;
        }
 
        if (PhotonNetwork.Friends != null)
        {
            foreach (FriendInfo FI in FriendsListPhoton)
            {
                CurrentFriendsCount++;
                FriendEmails.Add(FI.UserId);
                FriendOnlineStatuses.Add(FI.IsOnline);
                Instantiate(Friend, FriendsArea.transform.position, Quaternion.identity);

            }
            PhotonNetwork.Friends.Clear();
        }
        FriendsRefreshRate = 30;
        CurrentFriendsCount = 0;
        StartCoroutine(GetFriends());
    }

    void CreateFriends() {


    }

    // Update is called once per frame
    void Update () {

}


}

