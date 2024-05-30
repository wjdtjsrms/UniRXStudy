namespace Anipen.Subsystem.MRInput
{
    using R3;
    using System;
    using System.Collections.Generic;

    public class PinchSubsystem : IDisposable
    {
        private readonly List<ITapHandler> tapHandlers = new();
        private readonly List<IDoubleTapHandler> doubleTapHandlers = new();
        private readonly List<IDeviceMoveHandler> moveHandlers = new();
        private readonly List<IHoldHandler> holdHandlers = new();

        private IDisposable tapDisposable;
        private IDisposable doubleTapDisposable;
        private IDisposable moveDisposable;
        private IDisposable holdDisposable;

#if UNITY_EDITOR
        private readonly PinchProvider pinchProvider = new EditorPinchProvider();
#else
        private readonly PinchProvider pinchProvider = new VisionPinchProvider();
#endif

        public void Dispose()
        {
            Stop();
        }

        public virtual void Start()
        {
            tapDisposable = pinchProvider.OnTapSubject.Subscribe((data) => tapHandlers.ForEach((item) => item.OnTap(data)));
            doubleTapDisposable = pinchProvider.OnDoubleTapSubject.Subscribe((data) => doubleTapHandlers.ForEach((item) => item.OnDoubleTap(data)));
            moveDisposable = pinchProvider.OnMoveSubject.Subscribe((data) => moveHandlers.ForEach((item) => item.OnDeviceMove(data)));
            holdDisposable = pinchProvider.OnHoldSubject.Subscribe((data) => holdHandlers.ForEach((item) => item.OnHold(data)));
        }

        public virtual void Stop()
        {
            pinchProvider?.Dispose();
            tapDisposable?.Dispose();
            doubleTapDisposable?.Dispose();
            moveDisposable?.Dispose();
            holdDisposable?.Dispose();
        }

        #region Regist & Unregist Method
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