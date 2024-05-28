using Anipen.Subsystem.MRInput;
using JSGCode.Util;
using System.Collections.Generic;

public class MRInputManager : SingletonMonoBehaviour<MRInputManager>
{
    private readonly List<ITapHandler> tapHandlers = new();
    private readonly List<IDoubleTapHandler> doubleTapHandlers = new();
    private readonly List<IDeviceMoveHandler> moveHandlers = new();
    private readonly List<IHoldHandler> holdHandlers = new();

    private readonly PinchDataStream pinchDataStream = new();
    private readonly GestureDataStream gestureDataStream = new();

    private MRInputSubsystem inputSubsystem;

    private void Awake()
    {
        inputSubsystem = new MRInputSubsystem(new VisionInputProvider(), pinchDataStream);
    }

    private void Update()
    {
        var pinchData = pinchDataStream.ReadStream();
        var gestureData = gestureDataStream.ReadStream();

        switch (pinchData.firstInputData.type)
        {
            case MRInputType.Tap:

                foreach (var handler in tapHandlers)
                    handler.OnTap(pinchData.firstInputData);

                break;

            case MRInputType.DoubleTap:

                foreach (var handler in doubleTapHandlers)
                    handler.OnDoubleTap(pinchData.firstInputData);

                break;

            case MRInputType.Hold:

                if (pinchData.secondInputData != null && pinchData.secondInputData.type == MRInputType.Move)
                {

                    break;
                }

                foreach (var handler in holdHandlers)
                    handler.OnHold(pinchData.firstInputData, pinchData.holdTime);
                break;

            case MRInputType.Move:

                if (pinchData.secondInputData != null && pinchData.secondInputData.type == MRInputType.Move)
                {

                    break;
                }

                if (pinchData.inputPhase == MRInputPhase.Began)
                {
                    foreach (var handler in moveHandlers)
                        handler.OnDeviceMoveBegin();
                }
                else if (pinchData.inputPhase == MRInputPhase.Running)
                {
                    foreach (var handler in moveHandlers)
                        handler.OnDeviceMove(pinchData.firstInputData);
                }
                else if (pinchData.inputPhase == MRInputPhase.Ended)
                {
                    foreach (var handler in moveHandlers)
                        handler.OnDeviceMoveEnd();
                }
                break;
        }
    }

    #region Regist & Unregist Event
    public void RegistTabEvent(ITapHandler handler)
    {
        if (!tapHandlers.Contains(handler))
            tapHandlers.Add(handler);
    }

    public void UnregistTabEvent(ITapHandler handler)
    {
        if (tapHandlers.Contains(handler))
            tapHandlers.Remove(handler);
    }

    public void RegistDoubleTabEvent(IDoubleTapHandler handler)
    {
        if (!doubleTapHandlers.Contains(handler))
            doubleTapHandlers.Add(handler);
    }

    public void UnregistDoubleTabEvent(IDoubleTapHandler handler)
    {
        if (doubleTapHandlers.Contains(handler))
            doubleTapHandlers.Remove(handler);
    }

    public void RegistMoveEvent(IDeviceMoveHandler handler)
    {
        if (!moveHandlers.Contains(handler))
            moveHandlers.Add(handler);
    }

    public void UnregistMoveEvent(IDeviceMoveHandler handler)
    {
        if (moveHandlers.Contains(handler))
            moveHandlers.Remove(handler);
    }

    public void RegistHoldEvent(IHoldHandler handler)
    {
        if (!holdHandlers.Contains(handler))
            holdHandlers.Add(handler);
    }

    public void UnregistHoldEvent(IHoldHandler handler)
    {
        if (holdHandlers.Contains(handler))
            holdHandlers.Remove(handler);
    }
    #endregion
}
