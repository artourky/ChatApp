using UnityEngine;
using UnityEngine.UI;

// This is only a crude implementation of a chatting app we should use PhotonChat instead!
public class PhotonChatHandler : MonoBehaviour {

    #region UIFields
    public Text roomName;
    public Text connectionState;
    public Text playersNumber;
    public Text roomCreatedby;

    private Text scrollText;

    public Button joinOrLeaveRoom;
    public Button sendMsg;

    public InputField myMsg;

    public ScrollRect scrollRect;
    #endregion

    public PhotonView photonView;

    bool amIConnected = false;
    bool haveIpressedJoin = false;

    int msgsNumber = 0;

    string nickname = "Nickname";
    string roomId = "";

    // TODO: Ask for a Nickname before //, check for PeerCreated status << DONE

    public static void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings("v0.0.1");
    }
    
    void Awake()
    {
        //// WARNING: CROWDED CONSOLE!
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        
        roomName.text = "No room yet!";
        connectionState.text = "Not yet in a room!";
    }

    // Use this for initialization
    void Start () {
        myMsg.interactable = joinOrLeaveRoom.interactable = sendMsg.interactable = false;
        scrollText = scrollRect.GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        connectionState.text = PhotonNetwork.connectionStateDetailed.ToString();

        Log("Connection State: " + PhotonNetwork.connectionStateDetailed.ToString());
        Log("Am I the Master Client? " + PhotonNetwork.isMasterClient);

        if (!amIConnected && PhotonNetwork.connectedAndReady)
        {
            Log("I've connected to the Server and ready to Join a room!");
            joinOrLeaveRoom.interactable = sendMsg.interactable = true;
            amIConnected = true;
        }

        if (!PhotonNetwork.inRoom)
        {
            sendMsg.interactable = false;
            playersNumber.text = "";
        }
        else
        {
            playersNumber.text = "No. of Players In Room: " + PhotonNetwork.room.PlayerCount;
        }
	}

    // This is a UI Button for Joining or Leaving the Room
    public void JoinOrLeaveRoom()
    {
        if (!haveIpressedJoin)
        {
            if (!PhotonNetwork.inRoom)
            {
                haveIpressedJoin = true;
                joinOrLeaveRoom.GetComponentInChildren<Text>().text = "Leave Room";
                Log("Joining Random Room!");
                PhotonNetwork.JoinRandomRoom();
            }
        }
        else
        {
            if (PhotonNetwork.inRoom)
            {
                Log("Leaving the Room!");
                LeavingTheRoom();
            }
        }
    }

    void OnPhotonRandomJoinFailed()
    {
        Log("Random Room Fail!");
        roomCreatedby.text = "Room created by: Me!";
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        Log("Joined A Room!");
        roomName.text = "eu/" + PhotonNetwork.room.Name;

        if (roomCreatedby.text == "")
        {
            roomCreatedby.text = "Room created by: Someone else!";
        }

        myMsg.interactable = sendMsg.interactable = true;
    }

    void LeavingTheRoom()
    {
        photonView.RPC("SomeoneLeft", PhotonTargets.Others);

        PhotonNetwork.LeaveRoom();

        ResetUI();
    }

    private void ResetUI()
    {
        haveIpressedJoin = false;

        joinOrLeaveRoom.GetComponentInChildren<Text>().text = "Join Room";
        joinOrLeaveRoom.interactable = true;
    }

    // Sometimes this get called due to random disconnection
    void OnLeftRoom()
    {
        Log("I've left the room.");
        roomName.text = "No room yet!";

        ResetUI();
    }

    void OnDisconnectedFromPhoton()
    {
        Log("I got disconnected from Photon due to what?");
        Log("am I connected? " + PhotonNetwork.connectedAndReady);
        amIConnected = false;
    }

    public void SendMessage()
    {
        msgsNumber++;
        Log("Sending A Message!");
        Log("Input Field text: " + myMsg.text);
        scrollText.text = scrollText.text + "\n[Me] " + myMsg.text;
        photonView.RPC("YoOthers", PhotonTargets.Others, myMsg.text);
        myMsg.text = "";
    }

    [PunRPC]
    public void YoOthers(string msg)
    {
        //newMail++;
        //otherMsg = "You've got " + newMail + " mail(s).";
        //messageState.text = "Recieved Message!";
        //messageIs.text = otherMsg;

        msgsNumber++;
        Log("I received a msg!");
        scrollText.text = scrollText.text + "\n[Other] " + msg;
    }

    [PunRPC]
    public void SomeoneLeft()
    {
        Log("Someone left the room!");
        scrollText.text = scrollText.text + "\n[Server] SOMEONE LEFT THE ROOM!";
    }

    void Log(string msg)
    {
        print("[PhotonChatHandler] " + msg);
    }
}
