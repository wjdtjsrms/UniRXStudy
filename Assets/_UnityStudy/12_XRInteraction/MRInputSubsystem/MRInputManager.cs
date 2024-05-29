namespace Anipen.Subsystem.MRInput
{
    using JSGCode.Util;
    using System.Collections.Generic;

    public class MRInputManager : SingletonMonoBehaviour<MRInputManager>
    {
        private readonly List<ITapHandler> tapHandlers = new();
        private readonly List<IDoubleTapHandler> doubleTapHandlers = new();
        private readonly List<IDeviceMoveHandler> moveHandlers = new();
        private readonly List<IHoldHandler> holdHandlers = new();

        private PinchSubsystem pinchSubsystem;
        private GestureSubsystem gestureSubsystem;

        private void Awake()
        {
            pinchSubsystem = new PinchSubsystem(new EditorPinchProvider());
            pinchSubsystem.Start();

            gestureSubsystem = new GestureSubsystem(new EditorGestureProvider());
            gestureSubsystem.Start();
        }

        private void OnDestroy()
        {
            pinchSubsystem?.Stop();
            gestureSubsystem?.Stop();
        }

        private void Update()
        {
            var pinchData = pinchSubsystem.UpdateDataStream().ReadStream();
            var gestureData = gestureSubsystem.UpdateDataStream().ReadStream();

            var inputData = new InputData(pinchData, gestureData);

            switch (pinchData.firstInputData.type)
            {
                case MRInputType.Tap:

                    foreach (var handler in tapHandlers)
                        handler.OnTap(inputData);

                    break;

                case MRInputType.DoubleTap:

                    foreach (var handler in doubleTapHandlers)
                        handler.OnDoubleTap(inputData);

                    break;

                case MRInputType.Hold:

                    if (pinchData.secondInputData != null && pinchData.secondInputData.type == MRInputType.Move)
                    {

                        break;
                    }

                    foreach (var handler in holdHandlers)
                        handler.OnHold(inputData);
                    break;

                case MRInputType.Move:
                    foreach (var handler in moveHandlers)
                        handler.OnDeviceMove(inputData);

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
}