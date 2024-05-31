namespace Anipen.Subsystem.MRInput
{
    using R3;
    using System;
    using UnityEngine;

    public abstract class PinchProvider : IDisposable
    {
        protected const int MULTI_TAP_WAIT_TIME = 200;
        protected const int HOLD_THRESHOLD = 1500000;

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

    public class SiliconMacPinchProvider : PinchProvider
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

    public class WindowPinchProvider : PinchProvider
    {
        protected override Observable<PinchData> GetTapStream() => GetMultiTapStream(targetTapCount: 1);

        protected override Observable<PinchData> GetDoubleTapStream() => GetMultiTapStream(targetTapCount: 2);

        protected override Observable<PinchData> GetHoldStream()
        {
            // Stream for mouse down events
            var mouseDownStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Select(_ => new PinchData()); ;

            // Stream for mouse up events
            var mouseUpStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonUp(0))
                .Select(_ => new PinchData()); ;

            var test = mouseDownStream
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(1))
            .TakeUntil(mouseUpStream))
            .Where(_ => Input.GetMouseButtonUp(0))
            .Select(_ => new PinchData());

            return test;
        }

        protected override Observable<PinchData> GetMoveStream()
        {
            return Observable.EveryUpdate().Select(_ => (new PinchData()));
        }

        private Observable<PinchData> GetMultiTapStream(int targetTapCount)
        {
            if (targetTapCount < 1)
                return default;

            targetTapCount *= 2;

            var tapStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Merge(Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)));

            var tapBufferStream = tapStream.Chunk(tapStream.Debounce(TimeSpan.FromMilliseconds(MULTI_TAP_WAIT_TIME)))
            .Where(tapBuffer => tapBuffer.Length == targetTapCount)
            .Select(tapBuffer => tapBuffer[targetTapCount - 2])
            .Select(tapInput => new PinchData());

            return tapBufferStream;
        }
    }
}