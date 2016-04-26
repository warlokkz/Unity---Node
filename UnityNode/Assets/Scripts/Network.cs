using UnityEngine;
using System.Collections;
using SocketIO;
using System;
using System.Collections.Generic;

public class Network : MonoBehaviour {

    static SocketIOComponent socket;

    public GameObject myPlayer;

    public Spawner spawner;

	// Use this for initialization
	void Start () {
        socket = GetComponent<SocketIOComponent>();
        socket.On("open", OnConnected);
        socket.On("register", onRegister);
        socket.On("spawn", OnSpawned);
        socket.On("move", OnMove);
        socket.On("follow", OnFollow);
        socket.On("disconnected", onDisconnected);
        socket.On("requestPosition", OnRequestPosition);
        socket.On("updatePosition", OnUpdatePosition);

	}

    private void onRegister(SocketIOEvent e) {
        Debug.Log("Successfully registered, with id " + e.data);
        spawner.AddPlayer(e.data["id"].str, myPlayer);
    }

    private void OnFollow(SocketIOEvent e) {
        Debug.Log("follow request " + e.data);
        var player = spawner.FindPlayer(e.data["id"].str);

        var targetTransform = spawner.FindPlayer(e.data["targetId"].str).transform;

        var target = player.GetComponent<Targeter>();

        Debug.Log(player); 
        target.target = targetTransform;
    }

    private void OnUpdatePosition(SocketIOEvent e) {

        var position = GetVectorFromJSON(e);

        var player = spawner.FindPlayer(e.data["id"].str);

        player.transform.position = position;
    }

    private void OnRequestPosition(SocketIOEvent e) {
        Debug.Log("Server is requesting position");
        socket.Emit("updatePosition", VectorToJSON(myPlayer.transform.position));
    }

    private void onDisconnected(SocketIOEvent e) {
        // destroy Object
        var id = e.data["id"].str;
        spawner.Remove(id);
    }

    private void OnMove(SocketIOEvent e) {

        var position = GetVectorFromJSON(e);

        var player = spawner.FindPlayer(e.data["id"].str);

        var navigatePos = player.GetComponent<Navigator>();

        navigatePos.NavigateTo( position );

    }

    private void OnConnected (SocketIOEvent e) {
        Debug.Log("connected");
	}

    private void OnSpawned ( SocketIOEvent e ) {
        var player = spawner.SpawnPlayer(e.data["id"].str);

        if(e.data ["x"])
        {
            Vector3 movePosition = GetVectorFromJSON(e);

            var navigatePos = player.GetComponent<Navigator>();

            navigatePos.NavigateTo(movePosition);
        }

    }

    public static void Follow (string id) {
        Debug.Log("following network player id " + Network.PlayerIdToJSON(id));
        // send follower player id
        socket.Emit("follow", Network.PlayerIdToJSON(id));
    }

    public static void Move (Vector3 position) {
        // send position to node
        socket.Emit("move", Network.VectorToJSON(position));
    }

    private static Vector3 GetVectorFromJSON (SocketIOEvent e) {
        return new Vector3(e.data["x"].n, 0, e.data["z"].n);
    }


    public static JSONObject VectorToJSON (Vector3 vector) {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("x", vector.x);
        j.AddField("z", vector.z);
        return j;
    }

    public static JSONObject PlayerIdToJSON (string id) {
        JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
        j.AddField("targetId", id);
        return j;
    }
}
