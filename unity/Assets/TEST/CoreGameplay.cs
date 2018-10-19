using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.SceneManagement;

public class CoreGameplay : PunBehaviour

{
    private PunTurnManager turnManager;
    public bool StartGame;
    public List<GameObject> StructuredDeck;
    public List<GameObject> ShuffledDeck;
    public List<GameObject> PoolDeck;
    public List<GameObject> DiscardDeck;
    public List<string> iStihCards;
    public List<string> AllPlayersNames;
    public List<string> LocalCards;
    public List<string> TableCards;
    public Text textToListAllPlayersInRoom;
    public Text MyCardsText;
    public Text userName;
    bool checkMasterClient;
    public string cardsNames;
    public string cardsToSend;
    public GameObject Hand;
    public GameObject cardPrefab;
    public Transform LocalHand;
    public GameObject iStih;
    public bool InitialisedFirstTime;

    public GameObject TableCardRPC;

    public GameObject OpenUpButton;
    public GameObject ClearOpeningSelection;

    public GameObject PoolPileIndicator;
    public GameObject DiscardPileIndicator;
    //TURN MANAGMENT
    public GameObject MasterIndicator;
    public GameObject TurnIndicator;
    PhotonPlayer[] AllPlayers;
    public PhotonPlayer ActivePlayer;
    public PhotonPlayer NextPlayer;
    public int ActivePlayerIndex;
    public int PlayersInGame;
    public Text InGameText;
    public Text LocalMsg;
    public InputField ChatInput;
    public string LocalUsername;
    //INGAME VARIABLES
    public int TurnTime;
    public bool isMyTurn;
    public bool PoolCardDrawn;
    public bool HandCardDiscarded;
    public bool isOpen;
    public Text OpeningSelectionValueText;
    public int OpeningSelectionValue;
    public Transform LocalPlayerStih;
    public Transform LeftPlayerStih;
    public Transform CenterPlayerStih;
    public Transform RightPlayerStih;
    public GameObject StihPrefab;
    private int signToCheck;
    private int valueToCheck;
    private int differentSignsCount;
    public List<int> valuesToCheckIncremental;
    public Slider LocalTurnSlider;
    public bool IncrementStih;
    public bool SignStih;
    public List<Sprite> LocalCardDesign;
    public List<Image> CardDesignImages;
    public Image LocalAvatar;
    public List<Sprite> Avatars;
    public Transform Table;
    public bool masterCliendIntialized;
    public bool initialiseData;
    public void GameStart()
    {
        StartGame = true;
        InitializePlayersRPC();


    }
    public GameObject WinPanel;
    public GameObject LosePanel;
    public void ShuffleDeck()
    {
        int numberOfCards = StructuredDeck.Count;
        for (int i = 0; i <= numberOfCards - 1; i++)
        {
            int RandomCard = UnityEngine.Random.Range(0, StructuredDeck.Count - 1);
            ShuffledDeck.Add(StructuredDeck[RandomCard]);
            StructuredDeck.RemoveAt(RandomCard);
        }
    }
    public void AreWeMaster()
    {
        if (checkMasterClient && StartGame)
        {
            if (PhotonNetwork.isMasterClient)
            {
                //MasterIndicator.SetActive(true);
                if (!masterCliendIntialized)
                {
                    StartCoroutine(TurnSwitch());
                    masterCliendIntialized = true;
                }
            }
            else
            {
                MasterIndicator.SetActive(false);
            }
        }
    }


    private void Start()
    {
        StartCoroutine(InitialWait());
    }


    //#region Connection
    //public void ConnectToMasterSrv()
    //{
    //    if (userName.text != "")
    //    {
    //        PhotonNetwork.AuthValues = new AuthenticationValues();
    //        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
    //        PhotonNetwork.AuthValues.AddAuthParameter("user", userName.text);
    //        PhotonNetwork.AuthValues.AddAuthParameter("Nickname", userName.text);
    //        PhotonNetwork.playerName = userName.text;
    //        PhotonNetwork.AuthValues.UserId = userName.text;
    //        PhotonNetwork.ConnectUsingSettings("v1");
    //    }
    //    else
    //    {
    //        Debug.Log("You need to provide a username");
    //    }

    //}

    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected!");
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.IsVisible = true;
    //    roomOptions.MaxPlayers = 4;
    //    PhotonNetwork.JoinOrCreateRoom("MyMatch", roomOptions, TypedLobby.Default);
    //    checkMasterClient = true;
    //    populateAllPlayersList();

    //}
    //#endregion
    IEnumerator InitialWait()
    {
        PhotonNetwork.playerName = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().UserName;
        SetLocalAvatar();
        yield return new WaitForSeconds(5);
        checkMasterClient = true;
        populateAllPlayersList();
        photonView.RPC("StopLoadingScreen", PhotonTargets.All);
        ShuffleDeck();
        SetLocalCardDesign();


        if (PhotonNetwork.player.IsMasterClient)
        {
            CallGetCardsRPC();
            GameStart();

        }
    }
    public GameObject LoadingScreen;
    [PunRPC]
    void StopLoadingScreen()
    {
        LoadingScreen.SetActive(false);
    }
    public List<GameObject> DeckToReshufle;
    [PunRPC]
    void Reshuffle()
    {
        if (ShuffledDeck.Count == 0)
        {
            foreach (Transform card in Table)
            {
                DeckToReshufle.Add(card.gameObject);
                Debug.Log("Card added to DeckToReshufle: " + card.gameObject.name);
            }
            ShuffledDeck.Clear();
            int numberOfCards = DeckToReshufle.Count;
            for (int i = 0; i <= numberOfCards - 1; i++)
            {
                int RandomCard = UnityEngine.Random.Range(0, DeckToReshufle.Count - 1);
                ShuffledDeck.Add(DeckToReshufle[RandomCard]);
                DeckToReshufle.RemoveAt(RandomCard);
            }
            PoolDeck = ShuffledDeck;
            photonView.RPC("ClearTable", PhotonTargets.All);
        }
    }
    [PunRPC]
    void ClearTable()
    {
        foreach (Transform card in Table)
        {
            card.transform.SetParent(GameObject.FindGameObjectWithTag("Garbage").transform);
        }
    }

    public Image LoserPanelWinnerAvatar;
    public Text LoserPanelWinnerUsername;
    [PunRPC]
    void isGameOver(string Username, int WinnerAvatarIndex)
    {
        LosePanel.SetActive(true);
        LoserPanelWinnerAvatar.sprite = Avatars[WinnerAvatarIndex - 1];
        LoserPanelWinnerUsername.text = Username;
    }
    #region PUNS
    [PunRPC]
    void GetCardsRPC(string cards)
    {
        MyCardsText.text = "";
        MyCardsText.text = cards;
        foreach (string myString in cards.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
            LocalCards.Add(myString);
            Instantiate(cardPrefab, LocalHand);
        }
    }
    [PunRPC]
    void SendStih(string s, PhotonMessageInfo info)
    {

        if (s == "stih")
        {
            Instantiate(iStih, GameObject.Find(info.sender.NickName).transform.GetChild(0).transform);
        }
        else
        {
            iStihCards.Add(s);
            Instantiate(cardPrefab, GameObject.Find(info.sender.NickName).transform.GetChild(0).transform.GetChild(GameObject.Find(info.sender.NickName).transform.GetChild(0).childCount - 1));
        }
    }
    [PunRPC]
    void SendStihRemote(string s, string username, PhotonMessageInfo info)
    {

        if (s == "stih")
        {
            Debug.Log("Stih instantiated!");
            Instantiate(iStih, GameObject.Find(username).transform.GetChild(0).transform);
        }
        else
        {
            iStihCards.Add(s);
            Debug.Log("Card in stih: " + s);
            Instantiate(cardPrefab, GameObject.Find(username).transform.GetChild(0).transform.GetChild(GameObject.Find(username).transform.GetChild(0).childCount - 1));
        }
    }
    [PunRPC]
    public void SetActivePlayerBoard(string activeuser)
    {
        Debug.Log("SetActivePlayerBoard CALLED!");
        Debug.Log("ActiveUser: " + activeuser);
        foreach (string username in ActivePlayerUsernames)
        {
            Debug.Log("Username: " + activeuser);
            if (username == activeuser)
            {
                GameObject.Find(activeuser).transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                GameObject.Find(username).transform.GetChild(1).gameObject.SetActive(false);
            }
        }

    }

    public Sprite remoteAvatar;
    [PunRPC]
    void SetRemoteAvatars(string avatarName, PhotonMessageInfo info)
    {
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            if (p.NickName == info.sender.NickName)
            {
                foreach (Sprite s in Avatars)
                {
                    if (s.name == avatarName)
                    {
                        remoteAvatar = s;
                    }
                }
                GameObject.Find(info.sender.NickName).transform.GetChild(3).transform.GetChild(0).gameObject.GetComponent<Image>().sprite = remoteAvatar;
            }
        }
    }
    public Sprite localAvatar;
    public void SetLocalAvatar()
    {
        LocalAvatar.sprite = Avatars[GameObject.Find("UserData").GetComponent<UserData>().Avatar - 1];
    }

    public GameObject PlayerOnStihDelete;
    [PunRPC]
    void DeleteStihsOnPlayer(string username)
    {
        PlayerOnStihDelete = GameObject.Find(username);
        foreach (Transform t in PlayerOnStihDelete.transform.GetChild(0))
        {
            Destroy(t.gameObject);
        }
    }

    [PunRPC]
    void SetNextPlayerTurnRPC(bool myturn)
    {

        isMyTurn = myturn;
        PoolCardDrawn = false;
        HandCardDiscarded = false;
        LocalTurnSlider.maxValue = TurnTime;
        LocalTurnSlider.value = TurnTime;
        TableCardRPC.SetActive(false);
        if (myturn)
        {
            PoolPileIndicator.SetActive(true);
        }

    }
    public GameObject LocalPlayer;
    public GameObject LeftPlayer;
    public GameObject CenterPlayer;
    public GameObject RightPlayer;

    public List<string> ActivePlayerUsernames;


    [PunRPC]
    void InitializePlayers(int playersInGame, PhotonMessageInfo info)
    {
        if (playersInGame == 1)
        {
            LocalPlayer.transform.Find("LocalPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.playerName;
            LocalPlayer.name = PhotonNetwork.playerName;
            ActivePlayerUsernames.Add(PhotonNetwork.playerName);
            RightPlayer.SetActive(false);
            LeftPlayer.SetActive(false);
            CenterPlayer.SetActive(false);
        }
        if (playersInGame == 2)
        {
            LocalPlayer.transform.Find("LocalPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.playerName;
            LocalPlayer.name = PhotonNetwork.playerName;
            RightPlayer.transform.Find("RightPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.otherPlayers[0].NickName;
            RightPlayer.name = PhotonNetwork.otherPlayers[0].NickName;
            ActivePlayerUsernames.Add(PhotonNetwork.playerName);
            ActivePlayerUsernames.Add(PhotonNetwork.otherPlayers[0].NickName);
            LeftPlayer.SetActive(false);
            CenterPlayer.SetActive(false);
        }
        if (playersInGame == 3)
        {
            LocalPlayer.transform.Find("LocalPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.playerName;
            LocalPlayer.name = PhotonNetwork.playerName;
            RightPlayer.transform.Find("RightPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.otherPlayers[0].NickName;
            RightPlayer.name = PhotonNetwork.otherPlayers[0].NickName;
            CenterPlayer.transform.Find("CenterPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.otherPlayers[1].NickName;
            CenterPlayer.name = PhotonNetwork.otherPlayers[1].NickName;
            ActivePlayerUsernames.Add(PhotonNetwork.playerName);
            ActivePlayerUsernames.Add(PhotonNetwork.otherPlayers[0].NickName);
            ActivePlayerUsernames.Add(PhotonNetwork.otherPlayers[1].NickName);
            LeftPlayer.SetActive(false);
        }
        if (playersInGame == 4)
        {
            LocalPlayer.transform.Find("LocalPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.playerName;
            LocalPlayer.name = PhotonNetwork.playerName;
            RightPlayer.transform.Find("RightPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.otherPlayers[0].NickName;
            RightPlayer.name = PhotonNetwork.otherPlayers[0].NickName;
            CenterPlayer.transform.Find("CenterPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.otherPlayers[1].NickName;
            CenterPlayer.name = PhotonNetwork.otherPlayers[1].NickName;
            LeftPlayer.transform.Find("LeftPlayerUsername").gameObject.GetComponent<Text>().text = PhotonNetwork.otherPlayers[2].NickName;
            LeftPlayer.name = PhotonNetwork.otherPlayers[2].NickName;
            ActivePlayerUsernames.Add(PhotonNetwork.playerName);
            ActivePlayerUsernames.Add(PhotonNetwork.otherPlayers[0].NickName);
            ActivePlayerUsernames.Add(PhotonNetwork.otherPlayers[1].NickName);
            ActivePlayerUsernames.Add(PhotonNetwork.otherPlayers[2].NickName);

        }
        photonView.RPC("SetRemoteAvatars", PhotonTargets.Others, LocalAvatar.sprite.name);
    }

    [PunRPC]
    void GetPoolCardRPC(string poolcard)
    {
        LocalCards.Add(poolcard);
        Instantiate(cardPrefab, LocalHand);
        PoolCardDrawn = true;
    }
    [PunRPC]
    void M_ReturnPoolCardRPC(string sender)
    {
        string cardToSend = PoolDeck[0].name;
        PoolDeck.RemoveAt(0);
        foreach (PhotonPlayer p in PhotonNetwork.playerList)
        {
            if (p.NickName == sender)
            {
                PhotonPlayer reciever = p;
                photonView.RPC("GetPoolCardRPC", reciever, cardToSend);
                return;
            }
            else
            {
                Debug.Log("Reciever not found");
            }
        }
    }

    [PunRPC]
    void DiscardHandCardRPC(string discardedCardName)
    {
        TableCardRPC.SetActive(true);
        TableCards.Add(discardedCardName);
        Instantiate(cardPrefab, GameObject.FindGameObjectWithTag("Table").transform);
        HandCardDiscarded = true;
    }

    [PunRPC]
    void NextTurnRPC()
    {
        StopAllCoroutines();
        GetNextTurn();
        StartCoroutine(TurnSwitch());
    }
    #endregion


    [PunRPC]
    void CreateRemoteStih()
    {

    }
    [PunRPC]
    void AddCardToRemoteStih()
    {

    }
    public void DiscardHandCard(string discardedCardName)
    {
        if (!HandCardDiscarded && PoolCardDrawn)
        {
            populateAllPlayersList();
            TableCards.Add(discardedCardName);
            HandCardDiscarded = true;
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
                if (p.NickName != PhotonNetwork.playerName)
                {
                    photonView.RPC("DiscardHandCardRPC", p, discardedCardName);
                }
            }
        }
    }
    [PunRPC]
    void SendMsg(string msg, PhotonMessageInfo info)
    {
        InGameText.text = info.sender.NickName + ": " + msg + "\n" + InGameText.text;
    }
    [PunRPC]
    void CardsInHand(string username, int cardsInHand)
    {
        GameObject.Find(username).transform.GetChild(5).GetComponent<Text>().text = cardsInHand.ToString();
    }
    public void GetPoolCard()
    {
        if (isMyTurn && !PoolCardDrawn)
        {
            populateAllPlayersList();
            PoolPileIndicator.SetActive(false);
            photonView.RPC("M_ReturnPoolCardRPC", PhotonTargets.MasterClient, PhotonNetwork.playerName);
        }
    }
    public void CallGetCardsRPC()
    {
        populateAllPlayersList();
        foreach (PhotonPlayer p in AllPlayers)
        {
            cardsNames = "";
            for (int i = 0; i < 14; i++)
            {
                cardsNames = cardsNames + ShuffledDeck[0].name;
                cardsNames = cardsNames + "\n";
                ShuffledDeck.RemoveAt(0);
            }
            photonView.RPC("GetCardsRPC", p, cardsNames);
        }
        PoolDeck = ShuffledDeck;    //all cards from shuffled deck go into pool deck
    }

    public void InitializePlayersRPC()
    {
        populateAllPlayersList();
        photonView.RPC("InitializePlayers", PhotonTargets.All, PhotonNetwork.playerList.Length);
    }

    public void SendStihRPC(GameObject Stih)
    {
        photonView.RPC("SendStih", PhotonTargets.Others, Stih);
    }
    //public void GetAllPlayers()
    //{
    //    AllPlayersNames.Clear();
    //    textToListAllPlayersInRoom.text = "";
    //    PhotonPlayer[] AllPlayers = PhotonNetwork.playerList;
    //    if (AllPlayers.Length == 0)
    //    {
    //        Debug.Log("List is empty");
    //    }
    //    foreach (PhotonPlayer p in AllPlayers)
    //    {
    //        AllPlayersNames.Add(p.UserId);
    //        textToListAllPlayersInRoom.text = textToListAllPlayersInRoom.text + p.UserId + "\n";
    //        Debug.Log(p.UserId);
    //    }
    //}
    public void SendMsgRPC()
    {
        photonView.RPC("SendMsg", PhotonTargets.All, LocalMsg.text);
        ChatInput.text = "";
    }

    public void SetLocalCardDesign()
    {
        foreach (Image cd in CardDesignImages)
        {
            if (cd.IsActive()) {
                cd.sprite = LocalCardDesign[GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().CardDesign - 1];
            }
        }
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }

    public void populateAllPlayersList()
    {
        PhotonView photonView = PhotonView.Get(this);
        AllPlayers = PhotonNetwork.playerList;
    }

    public void GetNextTurn()
    {
        populateAllPlayersList();

        if (ActivePlayerIndex > AllPlayers.Length - 1) //resets turn order, starts from first player in order
        {
            ActivePlayerIndex = 0;
        }
        ActivePlayer = AllPlayers[ActivePlayerIndex];
        foreach (PhotonPlayer p in AllPlayers)
        {
            if (p == ActivePlayer)
            {
                photonView.RPC("SetNextPlayerTurnRPC", p, true);
                photonView.RPC("SetActivePlayerBoard", PhotonTargets.All, p.NickName);
            }
            else
            {
                photonView.RPC("SetNextPlayerTurnRPC", p, false);
            }
        }
        ActivePlayerIndex++;
    }

    public IEnumerator TurnSwitch()
    {
        if (!InitialisedFirstTime)
        {
            TurnTime = 1;
            InitialisedFirstTime = true;
        }
        yield return new WaitForSeconds(TurnTime);
        TurnTime = 30;
        GetNextTurn();
        StartCoroutine(TurnSwitch());
    }

    public void NextTurn()
    {
        populateAllPlayersList();
        photonView.RPC("NextTurnRPC", PhotonTargets.MasterClient);

    }
    public Image WinnerAvatar;
    public Text WinnerText;
    public string WinnerAvatarIndex;

    public void IsItMyTurn()
    {
        if (isMyTurn)
        {
            initialiseData = true;
            TurnIndicator.SetActive(true);
            LocalTurnSlider.value -= Time.deltaTime;
        }
        else
        {
            TurnIndicator.SetActive(false);
            PoolPileIndicator.SetActive(false);
            if (initialiseData)
            {
                ReturnAllCardsFromOpenSelectionToHand();
                CardInHandsRPC();

                int LocalCardsInHand = 0;
                foreach (Transform card in LocalHand)
                {
                    if (card.gameObject.GetActive())
                    {
                        LocalCardsInHand++;
                    }
                }
                Debug.Log("CardsInHand " + LocalCardsInHand.ToString());
                if (LocalCardsInHand == 0)
                {
                    Debug.Log("ENTERED WIN CHECK " + LocalCardsInHand.ToString());
                    WinPanel.SetActive(true);
                    WinnerAvatar.sprite = LocalAvatar.sprite;
                    WinnerText.text = PhotonNetwork.player.NickName;
                    photonView.RPC("isGameOver", PhotonTargets.Others, PhotonNetwork.player.NickName, GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().Avatar);
                }
                photonView.RPC("Reshuffle", PhotonTargets.MasterClient);
                initialiseData = false;
            }

        }
    }

    private void Update()
    {
        AreWeMaster();
        IsItMyTurn();
        if (Input.GetKeyDown(KeyCode.Return) && LocalMsg.text != "")
        {
            SendMsgRPC();
        }
    }

    /*  -instantiate pool                               DONE
        -get one pool card                              DONE
        -end turn
            -timeout - (radnom card on discard pile)
            -player places a card on discard pile 
            -UPDATE ALL PLAYERS
        -create stih
        -put in other stih (check when can/cannot)
        -reshuffle
        -win-loss condition / screen
        -update database
            
        
     
     
     */
    public List<GameObject> CardsToSort;
    public int LowestCardValue;
    private GameObject LowestCard;

    public void SortHand()
    {
        CardsToSort.Clear();
        LowestCard = null;
        LowestCardValue = 0;
        for (int i = 0; i < Hand.transform.childCount; i++)
        {
            LowestCardValue = Hand.transform.GetChild(i).GetComponent<CardScript>().CardValue;
            LowestCard = Hand.transform.GetChild(i).gameObject;
            for (int j = i + 1; j < Hand.transform.childCount; j++)
            {
                if (Hand.transform.GetChild(j).GetComponent<CardScript>().CardValue < LowestCardValue)
                {
                    LowestCardValue = Hand.transform.GetChild(j).GetComponent<CardScript>().CardValue;
                    LowestCard = Hand.transform.GetChild(j).gameObject;
                }
            }
            LowestCard.transform.SetSiblingIndex(i);
        }
    }

    public void ClearSelection()
    {
        foreach (Transform card in LocalHand.transform)
        {
            card.GetChild(0).GetComponent<Toggle>().isOn = false;
        }
    }
    public List<CardScript> selectedCards;
    public List<CardScript> selectedJokers;
    public List<CardScript> allSelectedCards;
    public bool StihCanForm;
    public void CheckIfSelectionIsValid()
    {
        StihCanForm = false;
        selectedCards.Clear();
        selectedJokers.Clear();
        allSelectedCards.Clear();
        IncrementStih = false;
        SignStih = false;

        foreach (Transform card in LocalHand.transform)
        {
            if (card.name != LocalHand.transform.name)
            {
                if (card.GetChild(0).GetComponent<Toggle>().isOn)
                {
                    allSelectedCards.Add(card.gameObject.GetComponent<CardScript>());
                    Debug.Log(card.gameObject.name);
                    selectedCards.Add(card.gameObject.GetComponent<CardScript>());
                    if (card.gameObject.GetComponent<CardScript>().isJoker)
                    {
                        selectedJokers.Add(card.gameObject.GetComponent<CardScript>());
                        selectedCards.Remove(card.gameObject.GetComponent<CardScript>());
                    }
                }
            }
        }

        int selectionCardsCount = selectedCards.Count;
        if (selectionCardsCount + selectedJokers.Count <= 2)
        {
            return;
        }

        //CheckIfSignIsSame(selectedCards);
        //CheckIfValueIsSame(selectedCards);
        //CheckIfAllSignsDifferent(selectedCards);
        CheckOnlyOneAce(selectedCards);
        //CheckIfCardValuesAreIncremental(selectedCards, selectedJokers);

        if (CheckIfValueIsSame(selectedCards) && CheckIfAllSignsDifferent(selectedCards, selectedJokers) && allSelectedCards.Count <= 4)
        {
            SignStih = true;
            CreateStih(allSelectedCards, LocalPlayerStih);
            if (isOpen)
            {
                photonView.RPC("SendStih", PhotonTargets.Others, "stih");
                foreach (CardScript card in allSelectedCards)
                {
                    photonView.RPC("SendStih", PhotonTargets.Others, card.name);
                }
            }
            return;
        }
        if (CheckIfSignIsSame(selectedCards) && CheckIfCardValuesAreIncremental(selectedCards, selectedJokers))
        {
            IncrementStih = true;
            CreateStih(allSelectedCards, LocalPlayerStih);
            if (isOpen)
            {
                photonView.RPC("SendStih", PhotonTargets.Others, "stih");
                foreach (CardScript card in allSelectedCards)
                {
                    photonView.RPC("SendStih", PhotonTargets.Others, card.name);
                }
            }
            return;
        }
    }
    #region StihCheck
    bool CheckIfSignIsSame(List<CardScript> selectedCards)
    {
        foreach (CardScript cs in selectedCards)
        {
            if (cs == selectedCards[0])
            {
                signToCheck = cs.CardSign;
            }
            if (cs.CardSign != signToCheck)
            {
                Debug.Log("Signs are not the same");

                return false;

            }
        }

        Debug.Log("Signs are same");
        return true;
    }

    bool CheckIfValueIsSame(List<CardScript> selectedCards)
    {
        foreach (CardScript cs in selectedCards)
        {
            if (cs == selectedCards[0])
            {
                valueToCheck = cs.CardValue;
            }
            if (cs.CardValue != valueToCheck)
            {

                Debug.Log("Values are not the same");
                return false;
            }
        }

        Debug.Log("Values are the same");
        return true;
    }
    public int JokerValuesToBeAdded;
    bool CheckIfCardValuesAreIncremental(List<CardScript> selectedCards, List<CardScript> selectedJokers)
    {
        JokerValuesToBeAdded = 0;
        valuesToCheckIncremental.Clear();
        foreach (CardScript cv in selectedCards)
        {
            valuesToCheckIncremental.Add(cv.CardValue);
        }

        valuesToCheckIncremental.Sort();
        for (int i = valuesToCheckIncremental.Count - 1; i > 0; i--)
        {
            Debug.Log(("FIRST: " + valuesToCheckIncremental[i].ToString() + " - " + valuesToCheckIncremental[i - 1].ToString() + " = " + (valuesToCheckIncremental[i] - valuesToCheckIncremental[i - 1]).ToString()));
            if (valuesToCheckIncremental[i] - valuesToCheckIncremental[i - 1] != 1)
            {
                if ((valuesToCheckIncremental[i] - valuesToCheckIncremental[i - 1] == 2) && selectedJokers.Count != 0)
                {
                    JokerValuesToBeAdded += valuesToCheckIncremental[i - 1] + 1;
                    if (selectedJokers.Count != 0)
                    {
                        selectedJokers.RemoveAt(0);
                    }
                    else
                    {

                        Debug.Log("Cards are not incremental");
                        return false;
                    }
                }
                else
                {

                    Debug.Log("Cards are not incremental");
                    return false;
                }
            }
        }
        for (int k = 0; k < selectedJokers.Count; k++)
        {
            JokerValuesToBeAdded += valuesToCheckIncremental[valuesToCheckIncremental.Count - 1] + 1 + k;
        }

        Debug.Log("Cards are incremental");
        return true;
    }
    public List<int> signsToCheck;
    public int cardValueSample;
    bool CheckIfAllSignsDifferent(List<CardScript> selectedCards, List<CardScript> selectedJokers)
    {
        JokerValuesToBeAdded = 0;
        signsToCheck.Clear();
        signsToCheck.Add(1);
        signsToCheck.Add(2);
        signsToCheck.Add(3);
        signsToCheck.Add(4);
        foreach (CardScript cv in selectedCards)
        {
            if (signsToCheck.Contains(cv.CardSign))
            {
                signsToCheck.Remove(cv.CardSign);
            }
            else
            {

                Debug.Log("Not unique signs");
                return false;
            }
        }
        if (selectedJokers.Count != 0)
        {
            foreach (CardScript cs in selectedCards)
            {
                if (!cs.isJoker)
                {
                    cardValueSample = cs.CardValue;
                }
            }
            if (cardValueSample == 1)
            {
                foreach (CardScript cs in selectedJokers)
                {
                    JokerValuesToBeAdded += 10;
                }
            }
            else if (cardValueSample >= 10)
            {
                foreach (CardScript cs in selectedJokers)
                {
                    JokerValuesToBeAdded += 10;
                }
            }
            else
            {
                foreach (CardScript cs in selectedJokers)
                {
                    JokerValuesToBeAdded += cardValueSample;
                }
            }
        }

        Debug.Log("Unique signs");
        return true;
    }
    public int numberOfAces;
    public CardScript AceCardScript;
    bool CheckOnlyOneAce(List<CardScript> selectedCards)
    {
        numberOfAces = 0;
        foreach (CardScript c in selectedCards)
        {
            if (c.CardValue == 1 || c.CardValue == 11)
            {
                numberOfAces++;
                AceCardScript = c;
            }
        }
        if (numberOfAces == 1)
        {

            SetOneAceValue(AceCardScript, selectedCards);
            return true;
        }
        else
        {

            return false;
        }
    }
    void SetOneAceValue(CardScript AceCardScript, List<CardScript> selectedCards)
    {
        foreach (CardScript c in selectedCards)
        {
            if (c.CardValue == 10 || c.CardValue == 12)
            {
                Debug.Log("Ace value set to 11");
                AceCardScript.CardValue = 11;
                return;
            }
            if (c.CardValue == 2)
            {
                Debug.Log("Ace value set to 1");
                AceCardScript.CardValue = 1;
                return;
            }
        }
    }
    #endregion
    #region CreateStih
    public List<CardScript> SelectedCardsForOpening;
    public void CreateStih(List<CardScript> allSelectedCards, Transform PlayerStihTransform)
    {
        SelectedCardsForOpening.Clear();
        Instantiate(StihPrefab, PlayerStihTransform);
        foreach (CardScript cs in allSelectedCards)
        {
            //TO DO: JOKER AND VALUES TO ADD
            SelectedCardsForOpening.Add(cs);
            if (cs.isAce)
            {
                OpeningSelectionValue += 10;
            }
            else if (cs.CardValue > 10)
            {
                OpeningSelectionValue += 10;
            }
            else
            {
                OpeningSelectionValue += cs.CardValue;
            }
            Instantiate(cardPrefab, PlayerStihTransform.GetChild(PlayerStihTransform.childCount - 1));
        }
        foreach (CardScript cs in SelectedCardsForOpening)
        {
            cs.gameObject.transform.GetChild(0).gameObject.GetComponent<Toggle>().isOn = false;
            cs.gameObject.SetActive(false);
        }
        OpeningSelectionValue += JokerValuesToBeAdded;
        OpeningSelectionValueText.text = OpeningSelectionValue.ToString();
    }

    public void ReturnAllCardsFromOpenSelectionToHand()
    {
        if (!isOpen)
        {
            OpeningSelectionValue = 0;
            OpeningSelectionValueText.text = "0";
            foreach (CardScript cs in SelectedCardsForOpening)
            {
                cs.gameObject.SetActive(true);
            }
            foreach (Transform stih in LocalPlayerStih)
            {
                if (stih.tag == "Stih")
                {
                    Destroy(stih.gameObject);
                }
            }
        }
    }

    public void OpenUp()
    {
        if (OpeningSelectionValue >= 51)
        {
            isOpen = true;
            SelectedCardsForOpening.Clear();
            OpeningSelectionValueText.gameObject.SetActive(false);
            iStihCards.Clear();
            OpenUpButton.SetActive(false);
            ClearOpeningSelection.SetActive(false);
            foreach (Transform stih in LocalPlayerStih)
            {
                photonView.RPC("SendStih", PhotonTargets.Others, "stih");
                foreach (Transform card in stih)
                {
                    photonView.RPC("SendStih", PhotonTargets.Others, card.name);
                }
            }
        }
    }

    public void RefreshStihsOnAddCard(string username)
    {
        photonView.RPC("DeleteStihsOnPlayer", PhotonTargets.Others, username);
        Debug.Log(username);
        iStihCards.Clear();
        foreach (Transform stih in GameObject.Find(username).transform.GetChild(0))
        {
            photonView.RPC("SendStihRemote", PhotonTargets.Others, "stih", username);
            foreach (Transform card in stih)
            {
                photonView.RPC("SendStihRemote", PhotonTargets.Others, card.name, username);
            }
        }
    }
    public int currentCardsInDeck;
    public void CardInHandsRPC()
    {
        currentCardsInDeck = 0;
        foreach (Transform cardInHand in Hand.transform)
        {
            if (cardInHand.gameObject.GetActive())
            {
                currentCardsInDeck++;
            }
        }
        LocalPlayer.transform.GetChild(5).gameObject.GetComponent<Text>().text = currentCardsInDeck.ToString();
        photonView.RPC("CardsInHand", PhotonTargets.Others, this.LocalPlayer.name, currentCardsInDeck);
    }
    #endregion
}
