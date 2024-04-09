using Fusion;
using UnityEngine;

public class FusionManager : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    private NetworkRunner networkRunner;

    private void Start()
    {
        Connect("Hello World");
    }

    private NetworkRunner CreateNetworkRunner()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab, transform);
        }

        return networkRunner;
    }

    public async void Connect(string sessionName)
    {
        if (networkRunner == null)
            CreateNetworkRunner();

        var gameArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 5,
        };

        StartGameResult result = await networkRunner.StartGame(gameArgs);
    }
}
