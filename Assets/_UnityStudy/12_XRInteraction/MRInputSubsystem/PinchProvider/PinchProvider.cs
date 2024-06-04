namespace Anipen.Subsystem.MRInput
{
    using R3;
    using System;

    public abstract class PinchProvider : IDisposable
    {
        protected const int TAP_THRESHOLD = 200;
        protected const float HOLD_THRESHOLD = 1.0f;
        protected const float MOVE_THRESHOLD = 0.05f;

        public Observable<PinchData> OnTapSubject { get; private set; }
        public Observable<PinchData> OnDoubleTapSubject { get; private set; }
        public Observable<PinchData> OnMoveSubject { get; private set; }
        public Observable<PinchData> OnHoldSubject { get; private set; }

        public virtual void Start()
        {
            OnTapSubject = GetTapStream();
            OnDoubleTapSubject = GetDoubleTapStream();
            OnMoveSubject = GetMoveStream();
            OnHoldSubject = GetHoldStream();
        }

        public virtual void Dispose() { }

        protected abstract Observable<PinchData> GetTapStream();
        protected abstract Observable<PinchData> GetDoubleTapStream();
        protected abstract Observable<PinchData> GetMoveStream();
        protected abstract Observable<PinchData> GetHoldStream();
    }
}