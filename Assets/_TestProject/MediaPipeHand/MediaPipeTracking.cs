using UnityEngine;

public class MediaPipeTracking
{
    private readonly int port = -1;
    private MediaPipeReceive udpReceive = new();

    public MediaPipeTracking(bool isLeft)
    {
        port = isLeft ? 5052 : 5053;
    }

    public void Start()
    {
        if (port == -1)
            return;

        udpReceive.Start(port);
    }

    public void Stop()
    {
        udpReceive.Stop();
    }

    public bool TryUpdateData(out Vector3[] handData)
    {
        string data = udpReceive.data;
        handData = new Vector3[21];

        if (data.Length <= 2)
            return false;

        // Remove '[' Data ']'
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        string[] points = data.Split(',');

        for (int i = 0; i < 21; i++)
        {
            float x = 7 - float.Parse(points[i * 3]) / 100;
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            handData[i] = new Vector3(x, y, z);
        }

        return true;
    }
}
