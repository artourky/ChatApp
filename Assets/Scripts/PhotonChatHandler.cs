﻿using UnityEngine;
using UnityEngine.UI;

// This is only a crude implementation of a chatting app we should use PhotonChat instead!
public class PhotonChatHandler : MonoBehaviour {

    public Text roomName;
    public Text connectionState;
    public Text playersNumber;
    public Text roomCreatedby;

    public Button joinOrLeaveRoom;
    public Button sendMsg;

    public InputField myMsg;

    public ScrollRect scrollRect;

    public PhotonView photonView;

    Text scrollText;

    bool amIConnected = false;
    bool haveIpressedJoin = false;

    int msgsNumber = 0;

    string nickname = "Nickname";
    string roomId = "";

    // TODO: Ask for a nickname before, check for peercreated status

    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings("v0.0.1");
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

    public void JoinOrLeaveRoom()
    {
        if (!haveIpressedJoin)
        {
            if (!PhotonNetwork.inRoom)
            {
                //if (roomId != "")
                //{
                //    PhotonNetwork.JoinRoom(roomId);
                //}
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

    void LeavingTheRoom()
    {
        photonView.RPC("SomeoneLeft", PhotonTargets.Others);
        haveIpressedJoin = false;
        joinOrLeaveRoom.GetComponentInChildren<Text>().text = "Join Room";
        PhotonNetwork.LeaveRoom();
        joinOrLeaveRoom.interactable = true;
    }

    void OnPhotonRandomJoinFailed()
    {
        Log("Random Room Fail!");
        //roomId = "";
        roomCreatedby.text = "Room created by: Me!";
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        Log("Joined A Room!");
        roomName.text = "eu/" + PhotonNetwork.room.Name;
        //roomId = PhotonNetwork.room.Name;

        if (roomCreatedby.text == "")
        {
            roomCreatedby.text = "Room created by: Someone else!";
        }

        myMsg.interactable = sendMsg.interactable = true;
    }

    void OnLeftRoom()
    {
        Log("I've left the room.");
        roomName.text = "No room yet!";
        LeavingTheRoom();
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
