using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FirebaseHandler : MonoBehaviour {

    public static FirebaseHandler instanceFirebHandler;

    public string myToken = "";

    public int secretCode = -1;

    public bool amITheMaster = false;
    public bool haveICr8edRoom = false;

    private void Awake()
    {
        if (instanceFirebHandler == null)
        {
            instanceFirebHandler = this;
        }
    }

    // Use this for initialization
    void Start () {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        myToken = token.Token;
        UIHandler.instanceUIHandler.firebaseToken.text = UIHandler.instanceUIHandler.friendToken.text = myToken;

        Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Log("Received a new message from: " + e.Message.From + " and the Message is I hope: " + e.Message.Data["meetMe@"]);
    }

    public IEnumerator SendHttpReq(string to, string message, bool IsItANotification)
    {
        UnityWebRequest www = new UnityWebRequest("https://fcm.googleapis.com/fcm/send", "POST");

        /* 
         * // Sending a notification message
         * {
         *   "to" : "bk3RNwTe3H0:CI2k_HHwgIpoDKCIZvvDMExUdFQ3P1...",
         *   "notification" : {
         *     ...
         *   }
         * }
         **/

        string notificationMsg = "{ \"notification\" : " +
            "{ \"body\" : \"" + message + "\"," +
            " \"sound\" : \"default\"," +
            " \"icon\":\"myicon\"" +
            " }," +
            " \"to\" : \"" + to + "\" }";

        string dataMsg = "{ \"data\" : " +
            "{" +
            "\"meetMe@\":\"" + secretCode + "\"," +
            "\"AmITheMaster\":\"" + amITheMaster + "\"" +
            "\"HaveICr8edRoom\":\"" + haveICr8edRoom + "\"" +
            "}," +
            " \"to\" : \"" + to + "\" }";

        if (IsItANotification)
        {
            www.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(notificationMsg));
        }
        else
        {
            www.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(dataMsg));
        }

        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "key=AAAA4z8Rr2g:APA91bFvpxvvRE9IKIsxTkNXKz2bksYciqExAEnC6n43kKnKJEWrbHSL-vlu4jGjxu7Tnk8gpEQ4duWIDhMeqTQ8-C-cGPfbhV5wXJvfqkC8gwkrHuNc4z-AUqJ6VEuCpi37oAZ16YiA"); //ChatApp - Server key

        yield return www.Send();

        if (www.isNetworkError)
        {
            Log(www.error);
        }
        else
        {
            Log("Form Complete! and " + www.responseCode);
            Log("downloadHandler: " + www.downloadHandler.text);
        }
    }

    void Log(string msg)
    {
        print("[FirebaseHandler] " + msg);
    }
}
