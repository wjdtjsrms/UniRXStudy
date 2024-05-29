namespace Anipen.Subsystem.MRInput
{
    using Cysharp.Threading.Tasks;
    using R3;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

    public enum MRInputKind
    {
        None, Indirect, Direct
    }

    public enum MRInputPhase
    {
        None, Begin, Running, Ended
    }

    public enum MRInputType
    {
        None, Tap, DoubleTap, Hold, Move
    }

    public class MRInputData
    {
        public GameObject targetObject;
        public MRInputKind kind;
        public MRInputType type;
        public Vector3 inputDevicePosition;
        public Vector3 inputDeviceRotation;
    }

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

        private readonly PinchProvider pinchProvider;

        public PinchSubsystem(PinchProvider pinchProvider)
        {
            this.pinchProvider = pinchProvider;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            pinchProvider.Start();

            tapDisposable = pinchProvider.OnTapSubject.Subscribe((data) => tapHandlers.ForEach((item) => item.OnTap(data)));
            doubleTapDisposable = pinchProvider.OnDoubleTapSubject.Subscribe((data) => doubleTapHandlers.ForEach((item) => item.OnDoubleTap(data)));
            moveDisposable = pinchProvider.OnMoveSubject.Subscribe((data) => moveHandlers.ForEach((item) => item.OnDeviceMove(data)));
            holdDisposable = pinchProvider.OnHoldSubject.Subscribe((data) => holdHandlers.ForEach((item) => item.OnHold(data)));
        }

        public void Stop()
        {
            pinchProvider?.Dispose();

            tapDisposable?.Dispose();
            doubleTapDisposable?.Dispose();
            moveDisposable?.Dispose();
            holdDisposable?.Dispose();
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

    public abstract class PinchProvider : IDisposable
    {
        public Observable<PinchData> OnTapSubject { get; private set; }
        public Observable<PinchData> OnDoubleTapSubject { get; private set; }
        public Observable<PinchData> OnMoveSubject { get; private set; }
        public Observable<PinchData> OnHoldSubject { get; private set; }

        public void Start()
        {
            OnTapSubject = SetTapSubject();
            OnDoubleTapSubject = SetDoubleTapSubject();
            OnMoveSubject = SetMoveSubject();
            OnHoldSubject = SetHoldSubject();
        }

        public virtual void Dispose() { }

        protected abstract Observable<PinchData> SetTapSubject();
        protected abstract Observable<PinchData> SetDoubleTapSubject();
        protected abstract Observable<PinchData> SetMoveSubject();
        protected abstract Observable<PinchData> SetHoldSubject();
    }

    public class VisionPinchProvider : PinchProvider
    {
        protected override Observable<PinchData> SetDoubleTapSubject()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }

        protected override Observable<PinchData> SetHoldSubject()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }

        protected override Observable<PinchData> SetMoveSubject()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }

        protected override Observable<PinchData> SetTapSubject()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }
    }

    public class EditorPinchProvider : PinchProvider
    {
        protected override Observable<PinchData> SetDoubleTapSubject()
        {
            var tapStream = Observable.EveryUpdate()
                .Select(_ => Touch.activeTouches)
                .Where(touches => touches.Count() > 0 && touches[0].phase.Equals(TouchPhase.Began))
                .Select(_ => new PinchData());

            return tapStream;
        }

        protected override Observable<PinchData> SetHoldSubject()
        {
            return Observable.EveryUpdate().Select(_ => (new PinchData()));
        }

        protected override Observable<PinchData> SetMoveSubject()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }

        protected override Observable<PinchData> SetTapSubject()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }
    }

    public interface ITapHandler
    {
        public void OnTap(PinchData data);
    }

    public interface IDoubleTapHandler
    {
        public void OnDoubleTap(PinchData data);
    }

    public interface IDeviceMoveHandler
    {
        public void OnDeviceMove(PinchData data);
    }

    public interface IHoldHandler
    {
        public void OnHold(PinchData data);
    }

    public class PinchData
    {
        public MRInputData firstInputData = new();
        public MRInputData secondInputData = new();

        public float holdTime = 0.0f;
        public bool isBoth = false;

        public MRInputPhase inputPhase;
    }

}