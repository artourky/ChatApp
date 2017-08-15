using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public static UIHandler instanceUIHandler;

    public GameObject welcomePanel;
    public GameObject hideMe;
    public GameObject signIn;

    public Text firebaseToken;

    public InputField friendToken;
    public InputField nickName;

    public Button callHim;
    public Button answerChat;
    public Button logIn;

    private void Awake()
    {
        if (instanceUIHandler == null)
        {
            instanceUIHandler = this;
        }
    }

    public void LogIn()
    {
        PhotonChatHandler.nickname = nickName.text;
        Log("My Name is: " + PhotonChatHandler.nickname);

        FirebaseHandler.instanceFirebHandler.AddMeToDB();
        signIn.SetActive(false);
    }

    public void RandomChat()
    {
        Log("Normal Chat it is!");

        welcomePanel.SetActive(false);
        PhotonChatHandler.ConnectToPhoton();
    }

    public void FriendChat()
    {
        Log("Friend Chat woah");

        FirebaseHandler.instanceFirebHandler.GetFriendFromDB();
        hideMe.SetActive(true);
    }

    public void CallHim()
    {
        Log("Calling the friend " + friendToken.text);

        FirebaseHandler.instanceFirebHandler.secretCode = Random.Range(0, 256);
        FirebaseHandler.instanceFirebHandler.amITheMaster = true;
        StartCoroutine(FirebaseHandler.instanceFirebHandler.SendHttpReq(FirebaseHandler.instanceFirebHandler.friendToken, null, false));
    }

    public void AnswerChat()
    {
        Log("Answering Chat Request");

        // TODO: Reply to the (Caller) and let them that you are here // or should i just cr8 the room and w8 for them to join??
        StartCoroutine(FirebaseHandler.instanceFirebHandler.SendHttpReq(/* HERE SHOULD BE THE FireBase Token */"", null, false));
    }

    void Log(string msg)
    {
        print("[UIHandler] " + msg);
    }
}
