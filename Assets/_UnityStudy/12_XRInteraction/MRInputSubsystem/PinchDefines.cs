using UnityEngine;

namespace Anipen.Subsystem.MRInput
{
    public enum MRInputKind
    {
        None, Indirect, Direct
    }

    public enum MRInputPhase
    {
        None, Begin, Running, End
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
        public void OnMoveBegin(PinchData data);
        public void OnMoving(PinchData data);
        public void OnMoveEnd(PinchData data);
    }

    public interface IHoldHandler
    {
        public void OnHoldBegin(PinchData data);
        public void OnHolding(PinchData data);
        public void OnHoldEnd(PinchData data);
    }

    public class MRInputData
    {
        public GameObject targetObject;
        public MRInputKind kind;
        public Vector3 inputDevicePosition;
        public Vector3 inputDeviceRotation;
    }

    public class PinchData
    {
        public MRInputData inputData = new();
        public MRInputPhase inputPhase = MRInputPhase.None;
        public bool isPrimaryData = true;
    }
}