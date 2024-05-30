using Anipen.Subsystem.MRInput;
using UnityEngine;

public class MRInputTest : MonoBehaviour, ITapHandler, IDoubleTapHandler
{
    private void OnEnable()
    {
        MRInputManager.Instance.PinchSubsystem.RegistTabEvent(this);
        MRInputManager.Instance.PinchSubsystem.RegistDoubleTabEvent(this);
    }

    private void OnDisable()
    {
        MRInputManager.Instance?.PinchSubsystem.UnregistTabEvent(this);
        MRInputManager.Instance?.PinchSubsystem.UnregistDoubleTabEvent(this);
    }

    public void OnTap(PinchData data)
    {
        Debug.Log("OnTap");
    }

    public void OnDoubleTap(PinchData data)
    {
        Debug.Log("OnDoubleTap");
    }
}