using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (ChatGui))]
public class NamePickGui : MonoBehaviour
{
    private const string UserNamePlayerPref = "NamePickUserName";

    public ChatGui chatNewComponent;

    public InputField idInput;

	public Text Username;

    public void Start()
    {
        this.chatNewComponent = FindObjectOfType<ChatGui>();


        string prefsName = PlayerPrefs.GetString(NamePickGui.UserNamePlayerPref);
        if (!string.IsNullOrEmpty(prefsName))
        {
            this.idInput.text = prefsName;
        }

		StartChat ();
    }


    // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then StartChat.
    public void EndEditOnEnter()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            this.StartChat();
        }
    }

    public void StartChat()
    {
        ChatGui chatNewComponent = FindObjectOfType<ChatGui>();
		chatNewComponent.UserName = GameObject.FindGameObjectWithTag ("UserData").GetComponent<UserData> ().UserName;
		chatNewComponent.Connect();
        enabled = false;

		PlayerPrefs.SetString(NamePickGui.UserNamePlayerPref, GameObject.FindGameObjectWithTag ("UserData").GetComponent<UserData> ().UserName);
    }
}