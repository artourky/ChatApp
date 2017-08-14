using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public static UIHandler instanceUIHandler;

    public GameObject welcomePanel;
    public GameObject hideMe;

    public Text firebaseToken;

    public InputField friendToken;
    public Button callHim;

    private void Awake()
    {
        if (instanceUIHandler == null)
        {
            instanceUIHandler = this;
        }
    }

    public void RandomChat()
    {
        Log("Normal Chat it is!");

        welcomePanel.SetActive(false);
        PhotonChatHandler.ConnectToPhoton();
    }

    public void FriendChat()
    {
        Log("Friend Chat wooooooooooooooooah");

        hideMe.SetActive(true);
    }

    public void CallHim()
    {
        Log("Calling the friend " + friendToken.text);

        StartCoroutine(FirebaseHandler.instanceFbHandler.SendHttpReq(friendToken.text, "test", true));
    }

    void Log(string msg)
    {
        print("[UIHandler] " + msg);
    }
}
