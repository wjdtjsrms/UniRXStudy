namespace Anipen.Subsystem.MRInput
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

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

    public class GestureSubsystem : IDisposable
    {
        private GestureDataStream dataStream;

        public GestureDataStream UpdateDataStream()
        {
            return dataStream;
        }

        public GestureSubsystem(GestureProvider gestureProvider)
        {

        }

        public void Dispose()
        {

        }

        public void Start()
        {
            Debug.Log("GestureSubsystem Start");
        }

        public void Stop()
        {
            Debug.Log("GestureSubsystem Stop");
        }
    }

    public class PinchSubsystem : IDisposable
    {
        private readonly List<ITapHandler> tapHandlers = new();
        private readonly List<IDoubleTapHandler> doubleTapHandlers = new();
        private readonly List<IDeviceMoveHandler> moveHandlers = new();
        private readonly List<IHoldHandler> holdHandlers = new();

        private PinchProvider pinchProvider;
        private PinchDataStream dataStream;

        public PinchSubsystem(PinchProvider pinchProvider)
        {
            this.pinchProvider = pinchProvider;
            dataStream = new();
        }

        public void Dispose()
        {


        }

        public PinchDataStream UpdateDataStream()
        {


            return dataStream;
        }

        public void Start()
        {
            Debug.Log("PinchSubsystem Start");
        }

        public void Stop()
        {
            Debug.Log("PinchSubsystem Stop");
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

    public abstract class GestureProvider
    {

    }

    public class VisionGestureProvider : GestureProvider
    {

    }

    public class EditorGestureProvider : GestureProvider
    {

    }

    public abstract class PinchProvider
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

    public class VisionPinchProvider : PinchProvider
    {

    }

    public class EditorPinchProvider : PinchProvider
    {

    }

    public interface ITapHandler
    {
        public void OnTap(InputData data);
    }

    public interface IDoubleTapHandler
    {
        public void OnDoubleTap(InputData data);
    }

    public interface IDeviceMoveHandler
    {
        public void OnDeviceMoveBegin();
        public void OnDeviceMove(InputData data);
        public void OnDeviceMoveEnd();
    }

    public interface IHoldHandler
    {
        public void OnHold(InputData data);
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

    public class InputData
    {
        public PinchData PinchData { get; private set; }
        public GestureData GestureData { get; private set; }

        public InputData() { }
        public InputData(PinchData pinchData, GestureData gestureData)
        {
            PinchData = pinchData;
            GestureData = gestureData;
        }
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

        public void Clear()
        {

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

        public void Clear()
        {

        }
    }
}