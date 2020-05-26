using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using GhostGen;
using Photon.Pun;
using Photon.Realtime;
using Zenject;

public class NetworkManager : MonoBehaviourPunCallbacks, IInitializable, ILateDisposable, IOnEventCallback
{
    public const string kGameVersion = "0.1.0";
    public const int kMaxPlayers = 4;

    public event Action<byte , object , int > onCustomEvent;

    public event Action onCreatedRoom;
    public event Action onJoinedLobby;
    public event Action onJoinedRoom;
    public event Action onLeftLobby;
    public event Action onLeftRoom;
    public event Action<List<RoomInfo>> onReceivedRoomListUpdate;
    public event Action<Player> onPlayerConnected;
    public event Action<Player> onPlayerDisconnected;

    public void Initialize()
    {
        PhotonNetwork.GameVersion = kGameVersion;
//        PhotonNetwork.OnEventCall += onCustomEventCallback;
    }

    public void LateDispose()
    {
//        PhotonNetwork.OnEventCall -= onCustomEventCallback;

        onCreatedRoom = null;
        onJoinedLobby = null;
        onJoinedRoom = null;
        onLeftLobby = null;
        onLeftRoom = null;
        onReceivedRoomListUpdate = null;
        onPlayerConnected = null;
        onPlayerDisconnected = null;

        if (PhotonNetwork.IsConnected)
        {
            Disconnect();
        }
    }

    public bool Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Network: Already Connected!");
            return false;
        }

        //PhotonNetwork.autoJoinLobby = true;
        return PhotonNetwork.ConnectUsingSettings();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public bool isConnected
    {
        get { return PhotonNetwork.IsConnected; }
    }

    public Player GetPlayerById(int playerId)
    {
        Player[] playerList = PhotonNetwork.PlayerList;

        int count = playerList.Length;
        for(int i = 0; i < count; ++i)
        {
            if(playerId == playerList[i].ActorNumber)
            {
                return playerList[i];
            }
        }
        return null;
    }

    /// PUN Callbacks
    public override void OnCreatedRoom()
    {
        if(onCreatedRoom != null)
        {
            onCreatedRoom();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (onCustomEvent != null)
        {
            onCustomEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender );
        }
    }

    public virtual void ON(object[] codeAndMsg)
    {
        Debug.Log(string.Format("Error:{0}, {1}", codeAndMsg[0], codeAndMsg[1]));
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("-On Received Room list update: "  + roomList.Count);
        if (onReceivedRoomListUpdate != null)
        {
            onReceivedRoomListUpdate(roomList);
        }
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("-Joining Lobby-");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby: " + PhotonNetwork.CurrentLobby.Type.ToString());
        safeCall(onJoinedLobby);
    }

    public override void OnJoinedRoom()
    {
        safeCall(onJoinedRoom);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(onPlayerConnected != null)
        {
            onPlayerConnected(newPlayer);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (onPlayerDisconnected != null)
        {
            onPlayerDisconnected(otherPlayer);
        }
    }

    public override void OnLeftRoom()
    {
        safeCall(onLeftRoom);
    }

    public override void OnLeftLobby()
    {
        safeCall(onLeftLobby);
    }

    
    private void onCustomEventCallback(byte eventCode, object content, int senderId)
    {
        if(onCustomEvent != null)
        {
//            onCustomEvent(eventCode, content, senderId);
        }
    }

    private void safeCall(Action callback)
    {
        if(callback != null)
        {
            callback();
        }
    }
}
