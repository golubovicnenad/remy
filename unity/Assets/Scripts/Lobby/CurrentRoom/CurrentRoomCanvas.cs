using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour {
    public void OnClickStartsync()
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.LoadLevel(2);

        PhotonNetwork.room.IsVisible = true;
    }
	
    public void OnClickStartDelayed()
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        PhotonNetwork.LoadLevel(2);
    }
}
