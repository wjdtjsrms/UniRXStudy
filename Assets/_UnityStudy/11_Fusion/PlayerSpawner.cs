using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var playerObject = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
            Runner.SetPlayerObject(player, playerObject);
        }
    }
}