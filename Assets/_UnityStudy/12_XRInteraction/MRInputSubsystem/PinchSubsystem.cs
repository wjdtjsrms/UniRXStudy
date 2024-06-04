namespace Anipen.Subsystem.MRInput
{
    using R3;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class PinchSubsystem : IDisposable
    {
        private readonly List<ITapHandler> tapHandlers = new();
        private readonly List<IDoubleTapHandler> doubleTapHandlers = new();
        private readonly List<IDeviceMoveHandler> moveHandlers = new();
        private readonly List<IHoldHandler> holdHandlers = new();

        private readonly CancellationTokenSource cancellationTokenSource = new();

#if UNITY_EDITOR_WIN
        private readonly PinchProvider pinchProvider = new WindowPinchProvider();
#else
        private readonly PinchProvider pinchProvider = new VisionOSPinchProvider();
#endif

        public void Dispose()
        {
            Stop();
        }

        public virtual void Start()
        {
            var d = Disposable.CreateBuilder();

            pinchProvider.Start();

            pinchProvider.OnTapSubject.Subscribe((data) => tapHandlers.ForEach((item) => item.OnTap(data))).AddTo(ref d);
            pinchProvider.OnDoubleTapSubject.Subscribe((data) => doubleTapHandlers.ForEach((item) => item.OnDoubleTap(data))).AddTo(ref d);

            pinchProvider.OnMoveSubject.Subscribe((data) => 
            {
                switch(data.inputPhase)
                {
                    case MRInputPhase.Begin:
                        moveHandlers.ForEach((item) => item.OnMoveBegin(data));
                        break;
                    case MRInputPhase.Running:
                        moveHandlers.ForEach((item) => item.OnMoving(data));
                        break;
                    case MRInputPhase.End:
                        moveHandlers.ForEach((item) => item.OnMoveEnd(data));
                        break;
                }
            }).AddTo(ref d);

            pinchProvider.OnHoldSubject.Subscribe((data) =>
            {
                switch (data.inputPhase)
                {
                    case MRInputPhase.Begin:
                        holdHandlers.ForEach((item) => item.OnHoldBegin(data));
                        break;
                    case MRInputPhase.Running:
                        holdHandlers.ForEach((item) => item.OnHolding(data));
                        break;
                    case MRInputPhase.End:
                        holdHandlers.ForEach((item) => item.OnHoldEnd(data));
                        break;
                }
               
            }).AddTo(ref d);

            d.RegisterTo(cancellationTokenSource.Token);
        }

        public virtual void Stop()
        {
            pinchProvider?.Dispose();
            cancellationTokenSource?.Cancel();
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