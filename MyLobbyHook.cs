using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class MyLobbyHook : LobbyHook {

public override void OnLobbyServerSceneLoadedForPlayer (NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)     //before game and lobby
    {
        LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
        Player gPlayer = gamePlayer.GetComponent<Player>();

        gPlayer.playerName = lPlayer.playerName;        //transfer from player script, runs hooks
        gPlayer.playerColor = lPlayer.playerColor;
    }
}
