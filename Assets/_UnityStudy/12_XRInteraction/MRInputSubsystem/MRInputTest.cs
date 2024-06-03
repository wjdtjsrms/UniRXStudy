using Anipen.Subsystem.MRInput;
using UnityEngine;

public class MRInputTest : MonoBehaviour, ITapHandler, IDoubleTapHandler, IHoldHandler, IDeviceMoveHandler
{
    private void OnEnable()
    {
        MRInputManager.Instance.PinchSubsystem.RegistTabEvent(this);
        MRInputManager.Instance.PinchSubsystem.RegistDoubleTabEvent(this);
        MRInputManager.Instance.PinchSubsystem.RegistHoldEvent(this);
        MRInputManager.Instance.PinchSubsystem.RegistMoveEvent(this);
    }

    private void OnDisable()
    {
        MRInputManager.Instance?.PinchSubsystem.UnregistTabEvent(this);
        MRInputManager.Instance?.PinchSubsystem.UnregistDoubleTabEvent(this);
        MRInputManager.Instance?.PinchSubsystem.UnregistHoldEvent(this);
        MRInputManager.Instance?.PinchSubsystem.UnregistMoveEvent(this);
    }

    public void OnTap(PinchData data)
    {
        Debug.Log("OnTap");
    }

    public void OnDoubleTap(PinchData data)
    {
        Debug.Log("OnDoubleTap");
    }

    public void OnHold(PinchData data)
    {
        Debug.Log("OnHold");
    }

    public void OnDeviceMove(PinchData data)
    {
        Debug.Log("OnDeviceMove");
    }
}