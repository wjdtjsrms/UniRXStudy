using Anipen.Subsystem.MRInput;
using UnityEngine;

public class MRInputTest : MonoBehaviour, ITapHandler
{
    private void Start()
    {
        MRInputManager.Instance.PinchSubsystem.RegistTabEvent(this);
    }

    private void OnDestroy()
    {
        MRInputManager.Instance?.PinchSubsystem.UnregistTabEvent(this);
    }

    public void OnTap(PinchData data)
    {
        Debug.Log(data);
    }
}