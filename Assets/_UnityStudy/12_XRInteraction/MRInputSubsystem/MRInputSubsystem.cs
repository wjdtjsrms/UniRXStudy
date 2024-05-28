namespace Anipen.Subsystem.MRInput
{
    using Cysharp.Threading.Tasks;
    using JSGCode.Util;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum MRInputKind
    {
        None, Indirect, Direct
    }

    public enum MRInputPhase
    {
        None, Began, Running, Ended
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

    public class MRInputSubsystem : IDisposable
    {
        private readonly List<ITapHandler> tapHandlers = new();
        private readonly List<IDoubleTapHandler> doubleTapHandlers = new();
        private readonly List<IDeviceMoveHandler> moveHandlers = new();
        private readonly List<IHoldHandler> holdHandlers = new();

        private MRInputProvider inputProvider;

        public MRInputSubsystem(MRInputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        public void Dispose()
        {


        }

        public void Start()
        {
            Debug.Log("Start");
        }

        public void Stop()
        {
            Debug.Log("Stop");
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

    public abstract class MRInputProvider
    {
        private event Action<MRInputData> TapEvent;
        private event Action<MRInputData> DoubleTapEvent;
        private event Action<MRInputData> DeviceMoveEvent;
        private event Action<MRInputData, float> HoldEvent;
        private event Action<MRInputData> RotateEvent;
        private event Action<MRInputData> PressAndDragEvent;

        protected void Start() { }
        protected void Stop() { }
        protected void CheckTap() { }
        protected void CheckDoubleTap() { }
        protected void CheckDeviceMove() { }
    }

    public class VisionInputProvider : MRInputProvider
    {

    }

    public class EditorMRInputProvider : MRInputProvider
    {

    }

    public interface ITapHandler
    {
        public void OnTap(MRInputData data);
    }

    public interface IDoubleTapHandler
    {
        public void OnDoubleTap(MRInputData data);
    }

    public interface IDeviceMoveHandler
    {
        public void OnDeviceMoveBegin();
        public void OnDeviceMove(MRInputData data);
        public void OnDeviceMoveEnd();
    }

    public interface IHoldHandler
    {
        public void OnHoldBegin(MRInputData data);
        public void OnHold(MRInputData data, float holdTime);
        public void OnHoldEnd(MRInputData data);
    }

    public class MRInputEventHelper : MonoBehaviour
    {
        private List<Action> actions;

        private void Awake()
        {
            actions = new List<Action>();
        }

        private void OnDestroy()
        {

        }
    }

    public class PinchData
    {
        public MRInputData firstInputData = new();
        public MRInputData secondInputData = new();
        public MRInputPhase inputPhase;
        public bool IsReadable = false;
        public float holdTime = 0.0f;
        public bool isBoth = false;
    }

    public class GestureData
    {

    }

    public class PinchDataStream
    {
        // change R3?
        private Queue<PinchData> pinchDatas = new();

        public PinchData ReadStream()
        {
            // return pinchDatas.Dequeue();
            return new PinchData();
        }

        public async void WriteStream(PinchData data)
        {
            await UniTask.Delay(500);
            pinchDatas.Enqueue(data);
        }

        private void CheckDoubleTap()
        {

        }

        private void CheckHold()
        {

        }

        private void CheckMove()
        {

        }
    }

    public class GestureDataStream
    {
        private Queue<GestureData> gestureDatas = new();

        public GestureData ReadStream()
        {
            // return gestureDatas.Dequeue();
            return new GestureData();
        }

        public void WriteStream(GestureData data)
        {
            gestureDatas.Enqueue(data);
        }
    }
}