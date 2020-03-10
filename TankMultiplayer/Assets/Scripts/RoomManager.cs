using System;
using System.Collections;


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.MyGame
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        int i, j, k, l;//迭代用

        Button LeaveRoomBtn;

        #region Photon Callbacks
        private void Awake()
        {
            for (i = 0; i < transform.childCount; i++)
            {
                switch (transform.GetChild(i).name)
                {
                    case "Canvas":
                        for (j = 0; j < transform.GetChild(i).transform.childCount;j++)
                        {
                            switch (transform.GetChild(i).transform.GetChild(j).name)
                            {
                                case "LeaveRoomBtn":
                                    LeaveRoomBtn = transform.GetChild(i).transform.GetChild(j).GetComponent<Button>();
                                    LeaveRoomBtn.onClick.AddListener(LeaveRoom);
                                    break;
                            }
                        }
                        break;
                }
            }
        }
        #region Photon Callbacks


        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                Debug.Log("有人加入房間");

                //LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                Debug.Log("有人退出房間");
                //LoadArena();
            }
        }


        #endregion
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            Debug.Log("你已離開房間");
            SceneManager.LoadScene("Launcher");
        }


        #endregion


        #region Public Methods

        /// <summary>
        /// 與房間斷開連接
        /// </summary>
        public void LeaveRoom()
        {
            StartCoroutine(startLeaveRoom());
        }

        IEnumerator startLeaveRoom()
        {
         
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;
           
            SceneManager.LoadScene("Launcher");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 讓室長操作
        /// </summary>
        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)//判斷是否是室長
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }


        #endregion
    }
}