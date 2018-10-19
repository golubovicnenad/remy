using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{

    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; }
    private PhotonView PhotonView;
    private int PlayersInGame = 0;
    //private ExitGames.Client.Photon.Hashtable m_playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
    //private PlayerMovement CurrentPlayer;
   // private Coroutine m_pingCoroutine;

    // Use this for initialization
    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
               

        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 30;

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay")
        {
            if (PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
    }

    private void MasterLoadedGame()
    { 
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }

    private void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    private void RPC_LoadedGameScene(/*PhotonPlayer photonPlayer*/)
    {
        //PlayerManagement.Instance.AddPlayerStats(photonPlayer);

        PlayersInGame++;
        if (PlayersInGame == PhotonNetwork.playerList.Length)
        {
            print("All players are in the game scene.");
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }
    /*
    public void NewHealth(PhotonPlayer photonPlayer, int health)
    {
        PhotonView.RPC("RPC_NewHealth", photonPlayer, health);
    }

    [PunRPC]
    private void RPC_NewHealth(int health)
    {
        if (CurrentPlayer == null)
            return;

        if (health <= 0)
            PhotonNetwork.Destroy(CurrentPlayer.gameObject);
        else
            CurrentPlayer.Health = health;
    }
    */
    [PunRPC]
    private void RPC_CreatePlayer()
    {
        float randomValue = Random.Range(0f, 5f);
        //GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "NewPlayer"), Vector3.up * randomValue, Quaternion.identity, 0);
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "NewPlayer"), Vector3.up * randomValue, Quaternion.identity, 0);
        //CurrentPlayer = obj.GetComponent<PlayerMovement>();
    }



}