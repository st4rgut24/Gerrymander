using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Connection : Singleton<Connection>
{
    WebSocket websocket;

    string connectedPlayerId;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        
        DontDestroyOnLoad(this.gameObject);
    }

    public async void SendReadyState()
    {
        Message readyMessage = new Message("ready", "");
        string jsonMessage = JsonConvert.SerializeObject(readyMessage);

        SendWebSocketMessage(jsonMessage);
    }

    public async void SendLeaveState()
    {
        Message leaveMessage = new Message("leave", "");
        string jsonMessage = JsonConvert.SerializeObject(leaveMessage);

        // Convert the message to a JSON string
        SendWebSocketMessage(jsonMessage);
        GameManager.Instance.PVP = false;
    }

    public void SendDivideRoomPlay(int roomId)
    {
        DividePlay play = new DividePlay("divide", connectedPlayerId, roomId.ToString());
        string jsonMessage = JsonConvert.SerializeObject(play);

        SendWebSocketMessage(jsonMessage);
    }

    public void SendFillRoomPlay(int rooId)
    {
        // TODO: 
    }

    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket(""); // TODO: REPLACE WITH IP ADDRESS

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            GameManager.Instance.PVP = false;
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            GameManager.Instance.PVP = false;
        };

        websocket.OnMessage += (bytes) =>
        {
            // Convert bytes to string
            string message = System.Text.Encoding.UTF8.GetString(bytes);

            // Handle the message
            Debug.Log("Received message " + message);
            HandleMessage(message);
        };

        // waiting for messages
        await websocket.Connect();
    }


    // Method to handle incoming messages
    private void HandleMessage(string message)
    {
        Debug.Log("message " + message);

        // Define a class to match the expected structure
        Dictionary<string, string> messageData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

        if (messageData != null)
        {
            string messageType = messageData["type"];

            if (messageType == "match")
            {                
                connectedPlayerId = messageData["message"];
                Debug.Log("Match message received: " + messageData["message"]);
                ElectionDetailsManager.Instance.BeginMatch();
            }
            if (messageType == "divide")
            {
                // contains the room the opponent touched

                // and then divide that room
                string dividedRoomId = messageData["roomId"];
                Debug.Log("divided room id received " + dividedRoomId);

                int id = int.Parse(dividedRoomId);
                Map.Instance.DivideRoomById(id);
            }
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage(string jsonMessage)
    {
        // Convert the message to a JSON string
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            Debug.Log("Connected to websocket server");
            // Send the JSON message as a text
            await websocket.SendText(jsonMessage);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    // Class to match the structure of the incoming JSON message
    [System.Serializable]
    public class DividePlay
    {
        public string type;
        public string otherPlayerId;
        public string roomId;

        public DividePlay(string t, string otherId, string roomId)
        {
            type = t;
            otherPlayerId = otherId;
            this.roomId = roomId;            
        }
    }

    // Class to match the structure of the incoming JSON message
    [System.Serializable]
    public class Message
    {
        public string type;
        public string message;

        public Message(string t, string m)
        {
            type = t;
            message = m;
        }
    }
}