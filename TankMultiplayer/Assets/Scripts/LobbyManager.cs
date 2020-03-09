using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Com.WangMingHsi.MyTankGame;

public class LobbyManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    #region Private Serializable Fields


    #endregion


    #region Private Fields


    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";


    #endregion


    Dictionary<string, RoomInfo> myRoomList = new Dictionary<string, RoomInfo>();//房間清單

    const string playerNamePrefKey = "PlayerName";

    Button ConnectBtn;
    Button CreatRoomBtn;
    Button CreatRoomSettingFinishBtn;
    Button JoinRandomRoomBtn;


    InputField NameInputField;//玩家名稱
    InputField CreateRoomNameInputField;//創建的房間名
    InputField maxPlayerNumInputField;//房間最大人數

    Text Connecttxt;

    GameObject ConnectPanel;
    GameObject CreatRoomPanel;
    GameObject RoomListPanel;
    GameObject RoomNameObj;
    GameObject ViewPort;

    public Transform gridLayout;
    bool isConectting = false;

    private void Awake()
    {

        PhotonNetwork.AutomaticallySyncScene = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            switch (transform.GetChild(i).name)
            {
                case "Canvas":
                    for (int j = 0; j < transform.GetChild(i).transform.childCount; j++)
                    {
                        switch (transform.GetChild(i).transform.GetChild(j).name)
                        {
                            case "JoinRandomRoomBtn":
                                JoinRandomRoomBtn = transform.GetChild(i).transform.GetChild(j).GetComponent<Button>();
                                JoinRandomRoomBtn.onClick.AddListener(JoinRandomRoom);
                                break;
                            case "RoomListPanel":
                                RoomListPanel = transform.GetChild(i).transform.GetChild(j).gameObject;
                                for (int k = 0; k < RoomListPanel.transform.childCount; k++)
                                {
                                    switch (RoomListPanel.transform.GetChild(k).name)
                                    {
                                        case "Viewport":
                                            ViewPort = RoomListPanel.transform.GetChild(k).gameObject;
                                            for (int l = 0; l < ViewPort.transform.childCount; l++)
                                            {
                                                switch (ViewPort.transform.GetChild(l).name)
                                                {
                                                    case "Content":
                                                        Debug.Log("sds");

                                                        gridLayout = RoomListPanel.transform.GetChild(k).transform.GetChild(l).transform;
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "Connecting":
                                ConnectPanel = transform.GetChild(i).transform.GetChild(j).gameObject;
                                for (int l = 0; l < ConnectPanel.transform.childCount; l++)
                                {
                                    switch (ConnectPanel.transform.GetChild(l).name)
                                    {
                                        case "ConnectBtn":
                                            ConnectBtn = ConnectPanel.transform.GetChild(l).GetComponent<Button>();
                                            ConnectBtn.onClick.AddListener(delegate { Connect(ConnectPanel, Connecttxt); });
                                            break;
                                        case "PlayerNameInputField":
                                            NameInputField = ConnectPanel.transform.GetChild(l).GetComponent<InputField>();
                                            for (int k = 0; k < NameInputField.transform.childCount; k++)
                                            {
                                                switch (NameInputField.transform.GetChild(k).name)
                                                {
                                                    case "Text":
                                                        NameInputField.onEndEdit.AddListener(SetPlayerName);
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "CreatRoomBtn":
                                CreatRoomBtn = transform.GetChild(i).transform.GetChild(j).GetComponent<Button>();
                                CreatRoomBtn.onClick.AddListener(CreatRoomPanelOn);
                                break;

                            case "ConnectText":
                                Connecttxt = transform.GetChild(i).transform.GetChild(j).GetComponent<Text>();
                                break;
                            case "CreatRoomPanel":
                                CreatRoomPanel = transform.GetChild(i).transform.GetChild(j).gameObject;
                                for (int k = 0; k < CreatRoomPanel.transform.childCount; k++)
                                {
                                    switch (CreatRoomPanel.transform.GetChild(k).name)
                                    {
                                        case "RoomNameInputField":
                                            CreateRoomNameInputField = CreatRoomPanel.transform.GetChild(k).GetComponent<InputField>();
                                            break;
                                        case "PlayerNumInputField":
                                            maxPlayerNumInputField = CreatRoomPanel.transform.GetChild(k).GetComponent<InputField>();
                                            break;
                                        case "SettingFinishBtn":
                                            CreatRoomSettingFinishBtn = CreatRoomPanel.transform.GetChild(k).GetComponent<Button>();
                                            CreatRoomSettingFinishBtn.onClick.AddListener(CreateRoom);
                                            break;
                                    }
                                }
                                break;
                        }

                    }
                    break;
            }
        }//找物件
        RoomNameObj = Resources.Load<GameObject>("Prefabs/RoomNameBtn");
        Connecttxt.enabled = true;
    }
    // Start is called before the first frame update
    void Start()
    {

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;


       
        string defaultName = string.Empty;
        if (NameInputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                NameInputField.text = defaultName;
            }
        }


        PhotonNetwork.NickName = defaultName;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            isConectting = true;
            Connecttxt.enabled = false;
        }
        else
            isConectting = false;


    }

    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }


    #region MonoBehaviourPunCallbacks Callbacks
    /// <summary>
    /// 連接
    /// </summary>
    /// <param name="Panel"></param>
    /// <param name="txt"></param>
    public void Connect(GameObject Panel, Text txt)
    {

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            // PhotonNetwork.JoinRandomRoom();

            if (PhotonNetwork.InLobby)
            {
                Debug.Log("你已經連接成功，暱稱已設定完成!");
                ConnectPanel.SetActive(false);
                RoomListPanel.SetActive(true);
                CreatRoomBtn.gameObject.SetActive(true);
                JoinRandomRoomBtn.gameObject.SetActive(true);
                CreatRoomBtn.gameObject.SetActive(true);
            }

        }
        else
        {
            //連到Master #Critical, we must first and foremost connect to Photon Online Server.

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;

        }
    }

    public void CreatRoomPanelOn()
    {
        CreatRoomPanel.SetActive(true);
    }
    /// <summary>
    /// 開房
    /// </summary>
    /// <param name="maxPlayers"></param>
    public void CreateRoom()
    {
        if (CreateRoomNameInputField.text != null && maxPlayerNumInputField.text != null)
        {
            byte maxPlayers;
            switch (maxPlayerNumInputField.text)
            {
                case "1":
                    maxPlayers = 1;
                    break;
                case "2":
                    maxPlayers = 2;
                    break;
                case "3":
                    maxPlayers = 3;
                    break;
                case "4":
                    maxPlayers = 4;
                    break;
                default:
                    Debug.Log("無效的數字");
                    return;

            }
            Debug.Log("房間創建成功");
            PhotonNetwork.CreateRoom(CreateRoomNameInputField.text, new RoomOptions() { MaxPlayers = maxPlayers });
            PhotonNetwork.LoadLevel("Room for 1");
        }
    }
    public override void OnConnectedToMaster()
    {
        if (isConectting)
        {
            PhotonNetwork.JoinLobby();//加入大廳

            Debug.Log("已加入大廳\nPUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }
    }



    /// <summary>
    /// 加入隨機房間失敗
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {


        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        // PhotonNetwork.CreateRoom(null, new RoomOptions());
        // PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.JoinLobby();
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom("我的房間", new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        PhotonNetwork.LoadLevel("Room for 1");

    }
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.LoadLevel("Room for 1");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }
    public override void OnJoinedRoom()
    {


        /*
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("我们進到Room For 1 !!");

            PhotonNetwork.LoadLevel("Room for 1");
        }
        */
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ConnectPanel.SetActive(true);
        Connecttxt.enabled = false;
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }
    #endregion

    /// <summary>
    /// 查看房間清單
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("roomListUpdated");

        for (int i = 0; i < gridLayout.childCount; i++)
        {
            if (gridLayout.GetChild(i).GetComponentInChildren<Text>().text.Equals(roomList[i].Name))
            {
                Destroy(gridLayout.GetChild(i).gameObject);

                if (roomList[i].PlayerCount == 0) //判斷如果此房間裡沒人就從清單中刪除
                {
                    roomList.Remove(roomList[i]);
                }
            }
        }

        foreach (var room in roomList)
        {
            GameObject newRoom = Instantiate(RoomNameObj, gridLayout.position, Quaternion.identity);
            newRoom.GetComponent<Button>().onClick.AddListener(delegate { ChooseRoom(room.Name); });
            newRoom.GetComponentInChildren<Text>().text = room.Name;
            newRoom.transform.SetParent(gridLayout);
        }
        /*
        //pr用来暂时保存ui上的房间button，获取后先将原先显示的房间ui销毁掉
        GameObject[] pr = GameObject.FindGameObjectsWithTag("Room");
        foreach (var r in pr)
        {
            Destroy(r);
        }

        //遍历房间信息改变了的房间，注意这个roomList不包括所有房间，而是有属性改变了的房间，比如新增的
        foreach (var r in roomList)
        {
            //清除关闭或不可显示或已经移除了的房间
            if (!r.IsOpen || !r.IsVisible || r.RemovedFromList)
            {
                if (myRoomList.ContainsKey(r.Name))//如果该房间之前有，现在没有了就去掉它
                {
                    myRoomList.Remove(r.Name);
                }
                continue;
            }
            //更新房间信息
            if (myRoomList.ContainsKey(r.Name))
            {
                myRoomList[r.Name] = r;
            }
            //添加新房间
            else
            {
                myRoomList.Add(r.Name, r);//如果该房间之前没有，现在有了就加进myRoomList
            }
        }

        foreach (var r in myRoomList.Values)
        {
            //这个函数用来在canvas上添加房间ui的，自己定义
            UpdateRoomList(r.Name);
        }

        Debug.Log("===roomList count:" + roomList.Count + "===myRoomList count:" + myRoomList.Count);
        // messageText.text = "===roomList count:" + roomList.Count + "===myRoomList count:" + myRoomList.Count;
        */
    }
    /// <summary>
    /// 更新房間清單
    /// </summary>
    /// <param name="name"></param>
    public void UpdateRoomList(string name)
    {
        //这里我已经把房间ui做成prefab保存在Resources文件夹了
        /*
        GameObject go = Instantiate(Resources.Load("Prefabs/JoinThisRoomBtn") as GameObject);
        go.transform.parent = roomInLobbyPanel.transform;//roomInLobbyPanel是我用来放显示房间列表的panel
        go.transform.localScale = Vector3.one;
        go.name = name;
        go.transform.Find("ff").GetComponentInChildren<Text>().text = name;//设置好文字为房间名
                                                                       //绑定选择房间事件（自定义）
        go.GetComponent<Button>().onClick.AddListener(delegate ()
        {
           ChooseRoom(go.name);
        });
        */
    }
    public void ChooseRoom(string go)
    {
        PhotonNetwork.JoinRoom(go);
        PhotonNetwork.LoadLevel("Room for 1");
    }

}
