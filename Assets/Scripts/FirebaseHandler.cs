using Firebase;
using UnityEngine;
using Firebase.Database;
using System.Collections;
using Firebase.Unity.Editor;
using UnityEngine.Networking;
using System.Collections.Generic;

public class FirebaseHandler : MonoBehaviour {

    [HideInInspector]
    public static FirebaseHandler instanceFirebHandler;

    [HideInInspector]
    public string myToken = "";

    [HideInInspector]
    public string friendToken = "";

    [HideInInspector]
    public int secretCode = -1;

    [HideInInspector]
    public bool amITheMaster = false;

    [HideInInspector]
    public bool haveICr8edRoom = false;

    DatabaseReference refFirebaseDB;

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

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://chatapp-c18dc.firebaseio.com/");

        // Get the root reference location of the database.
        refFirebaseDB = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void AddMeToDB()
    {
        // Set data on Firebase
        refFirebaseDB.Child("users").Child(PhotonChatHandler.nickname).SetValueAsync(myToken);
    }

    /// <summary>
    /// Getting Friend from Database
    /// </summary>
    public void GetFriendFromDB(string friendName)
    {
        // Retrieve data from Firebase
        FirebaseDatabase.DefaultInstance
            .GetReference("users")
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Log("Err with retreiving");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Do something with snapshot...

                    foreach (KeyValuePair<string, object> item in snapshot.Value as Dictionary<string, object>)
                    {
                        if (item.Key == friendName)
                        {
                            Log("friend token should be: " + item.Value);
                            friendToken = item.Value.ToString();
                            //UIHandler.instanceUIHandler.friendToken.text = friendToken;
                            StartCoroutine(SendHttpReq(friendToken, "", false));
                        }
                    }
                }
            });
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        myToken = token.Token;
        UIHandler.instanceUIHandler.firebaseToken.text = UIHandler.instanceUIHandler.friendToken.text = myToken;

        Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        //Log("Received a new message from: " + e.Message.From + " and the Message is I hope: " + e.Message.Data["meetMe@"]);

        // e.Message.From << I HOPE IT'S The Firebase Token << It's NOT :(( Firebase Token will be e.Message.Data["myOwnFBT"]

        if (e.Message.Data["myOwnFBT"] != "")
        {
            friendToken = e.Message.Data["myOwnFBT"];
            Log("I've received a msg from: " + friendToken);
            // Receieved a Challenge && Am I The slave ??
            if (bool.Parse(e.Message.Data["AmITheMaster"]))
            {
                // Was the room created, Can I join now?
                if (bool.Parse(e.Message.Data["HaveICr8edRoom"]) && !PhotonNetwork.inRoom)
                {
                    Log("Joining the Chat");

                    PhotonChatHandler.ConnectToPhoton();
                    return;
                }

                secretCode = int.Parse(e.Message.Data["meetMe@"]);

                Log("Receieved a Challenge");
                UIHandler.instanceUIHandler.answerChat.gameObject.SetActive(true);
            }
            // Received a Response && I'm the Master && also Did I already create a room
            else if (!bool.Parse(e.Message.Data["AmITheMaster"]))
            {
                // TODO: Check 'secretCode' and then... cr8 room and send a signal again to let him join
                Log("Got a response");

                if (int.Parse(e.Message.Data["meetMe@"]) == secretCode)
                {
                    Log("Right Secret Code!");

                    PhotonChatHandler.ConnectToPhoton();
                }
                else
                {
                    Log("Wrong Secret Code!");

					// Testing if we can send a Data message from Firebase console
					Log("[\"meetMe@\"] contains " + e.Message.Data["meetMe@"]);
                }
            }
        }
    }

    public IEnumerator SendHttpReq(string to, string message, bool IsItANotification)
    {
        UnityWebRequest www = new UnityWebRequest("https://fcm.googleapis.com/fcm/send", "POST");

        /* 
         * // Sending a notification and data message
         * {
         *   "to" : "bk3RNwTe3H0:CI2k_HHwgIpoDKCIZvvDMExUdFQ3P1...",
         *   "notification" : {
         *     ...
         *   },
         *   "data" : {
         *      "customKey" : "customValue",
         *      ...
         *   }
         * }
         **/

        string notificationMsg = "{ \"notification\" : " +
            "{ \"body\" : \"" + message + "\"," +
            " \"sound\" : \"default\"," +
            " \"icon\" : \"myicon\"" +
            " }," +
            " \"to\" : \"" + to + "\" }";

        string dataMsg = "{ \"data\" : " +
            "{ \"meetMe@\" : \"" + secretCode + "\"," +
            " \"AmITheMaster\" : \"" + amITheMaster.ToString().ToLower() + "\"," +
            " \"HaveICr8edRoom\" : \"" + haveICr8edRoom.ToString().ToLower() + "\"," +
            " \"myOwnFBT\" : \"" + myToken + "\"" +
            " }," +
            " \"to\" : \"" + to + "\" }";

        if (IsItANotification)
        {
            Log("notf: " + notificationMsg);
            www.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(notificationMsg));
        }
        else
        {
            Log("data: " + dataMsg);
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
