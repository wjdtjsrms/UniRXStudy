namespace Anipen.Subsystem.MRInput
{
    using R3;

    public class VisionOSPinchProvider : PinchProvider
    {
        protected override Observable<PinchData> GetDoubleTapStream()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }

        protected override Observable<PinchData> GetHoldStream()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }

        protected override Observable<PinchData> GetMoveStream()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }

        protected override Observable<PinchData> GetTapStream()
        {
            return Observable.EveryUpdate().Select(_ => new PinchData());
        }
    }
}