namespace Anipen.Subsystem.MRInput
{
    using Cysharp.Threading.Tasks;
    using R3;
    using System;
    using System.Linq;
    using System.Threading;
    using UnityEngine;
    using UnityEngine.Networking;

    public abstract class PinchProvider : IDisposable
    {
        protected const int TAP_THRESHOLD = 200;
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
            var tapStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0));

            var tapBufferStream = tapStream
            .WhereAwait(async (_, ct) => await HoldCheck(waitSeconds: 1.0f, ct), AwaitOperation.Switch)
            .Select(tapInput => new PinchData());

            return tapBufferStream;

            static async UniTask<bool> HoldCheck(float waitSeconds, CancellationToken ct)
            {
                var currentTime = 0f;

                while (!ct.IsCancellationRequested && currentTime < waitSeconds)
                {
                    if (Input.GetMouseButtonUp(0))
                        return false;

                    await UniTask.Yield();
                    currentTime += Time.deltaTime;

                    if (Input.mousePositionDelta.sqrMagnitude > 0.05f)
                        currentTime = 0f;
                }

                return true;
            }
        }

        protected override Observable<PinchData> GetMoveStream()
        {
            var tapStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButton(0)).Select(tapInput => new PinchData()); ;

            tapStream = tapStream
            .Where(_ => Input.mousePositionDelta.sqrMagnitude > 0.05f);

            return tapStream;
        }

        private Observable<PinchData> GetMultiTapStream(int targetTapCount)
        {
            if (targetTapCount < 1)
                return default;

            targetTapCount *= 2;

            var tapStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => Input.mousePosition)
            .Merge(Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)).Select(_ => Input.mousePosition));

            var tapBufferStream = tapStream.Chunk(tapStream.Debounce(TimeSpan.FromMilliseconds(TAP_THRESHOLD)))
            .Where(tapBuffer => tapBuffer.Length == targetTapCount)
            .Select(tapBuffer => tapBuffer[targetTapCount - 2])
            .Select(tapInput => new PinchData());

            return tapBufferStream;
        }
    }
}