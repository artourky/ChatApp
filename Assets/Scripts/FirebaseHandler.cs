using UnityEngine;

public class FirebaseHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Log("Received a new message from: " + e.Message.From);
    }

    void Log(string msg)
    {
        print("[FirebaseHandler] " + msg);
    }
}
