using UnityEngine;

namespace Anipen.Subsystem.MRInput
{
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

    public class MRInputData
    {
        public GameObject targetObject;
        public MRInputKind kind;
        public MRInputType type;
        public Vector3 inputDevicePosition;
        public Vector3 inputDeviceRotation;
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