namespace Anipen.Subsystem.MRInput
{
    using Cysharp.Threading.Tasks;
    using R3;
    using System;
    using System.Linq;
    using System.Threading;
    using UnityEngine;

    public abstract class PinchProvider : IDisposable
    {
        protected const int TAP_THRESHOLD = 200;
        protected const int HOLD_THRESHOLD = 1500000;

        public Observable<PinchData> OnTapSubject { get; private set; }
        public Observable<PinchData> OnDoubleTapSubject { get; private set; }

        public Observable<PinchData> OnMoveStartSubject { get; private set; }
        public Observable<PinchData> OnMoveSubject { get; private set; }
        public Observable<PinchData> OnMoveEndSubject { get; private set; }

        public Observable<PinchData> OnHoldStartSubject { get; private set; }
        public Observable<PinchData> OnHoldSubject { get; private set; }
        public Observable<PinchData> OnHoldEndSubject { get; private set; }

        public virtual void Start()
        {
            OnTapSubject = GetTapStream();
            OnDoubleTapSubject = GetDoubleTapStream();

            OnMoveStartSubject = GetMoveStartStream();
            OnMoveSubject = GetMoveStream();
            OnMoveEndSubject = GetMoveEndStream();

            OnHoldStartSubject = GetHoldStartStream();
            OnHoldSubject = GetHoldStream();
            OnHoldEndSubject = GetHoldEndStream();
        }

        public virtual void Dispose() { }

        protected abstract Observable<PinchData> GetTapStream();
        protected abstract Observable<PinchData> GetDoubleTapStream();

        protected abstract Observable<PinchData> GetMoveStartStream();
        protected abstract Observable<PinchData> GetMoveStream();
        protected abstract Observable<PinchData> GetMoveEndStream();

        protected abstract Observable<PinchData> GetHoldStartStream();
        protected abstract Observable<PinchData> GetHoldStream();
        protected abstract Observable<PinchData> GetHoldEndStream();
    }

    //public class VisionOSPinchProvider : PinchProvider
    //{
    //}

    public class WindowPinchProvider : PinchProvider
    {
        private bool isHold = false;
        private bool isMove = false;

        #region Tap Stream
        protected override Observable<PinchData> GetTapStream() => GetMultiTapStream(targetTapCount: 1);

        protected override Observable<PinchData> GetDoubleTapStream() => GetMultiTapStream(targetTapCount: 2);

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
        #endregion

        #region Move Stream
        protected override Observable<PinchData> GetMoveStartStream()
        {
            var tapStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0) && !isMove)
                .Where(_ => Input.mousePositionDelta.sqrMagnitude > 0.05f)
                .Select(_ =>
                {

                    isMove = true;
                    return new PinchData();
                });

            return tapStream;
        }

        protected override Observable<PinchData> GetMoveStream()
        {
            var tapStream = Observable.EveryUpdate()
               .Where(_ => Input.GetMouseButton(0) && isMove)
               .Where(_ => Input.mousePositionDelta.sqrMagnitude > 0.05f)
               .Select(_ =>
               {

                   return new PinchData();
               });

            return tapStream;
        }

        protected override Observable<PinchData> GetMoveEndStream()
        {
            var holdStream = Observable.EveryUpdate()
            .Where(_ => isMove && Input.GetMouseButtonDown(0))
            .WhereAwait(async (_, ct) => await HoldCheck(waitSeconds: 1.0f, ct), AwaitOperation.Switch)
            .Select(_ => new PinchData());

            var t = Observable.EveryUpdate()
               .Where(_ => isMove && Input.GetMouseButtonUp(0))
                           .Select(_ => new PinchData()); ;

            var tapStream2 = holdStream.Merge(t);

            var tapStream = tapStream2
               .Select(_ =>
               {
                   isMove = false;
                   return new PinchData();

               });

            return tapStream;
        }
        #endregion

        #region Hold Stream
        protected override Observable<PinchData> GetHoldStartStream()
        {
            // action으로 hold percent 받기

            var tapStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0) && !isHold)
                .WhereAwait(async (_, ct) => await HoldCheck(waitSeconds: 1.0f, ct), AwaitOperation.Switch)
                .Select(_ =>
                {
                    isHold = true;
                    return new PinchData();
                });

            return tapStream;
        }

        protected override Observable<PinchData> GetHoldStream()
        {
            var tapStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0) && isHold)
                .Select(tapInput =>
                {

                    return new PinchData();
                });

            return tapStream;
        }

        protected override Observable<PinchData> GetHoldEndStream()
        {
            var holdStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0) && isHold)
                .Where(_ => Input.mousePositionDelta.sqrMagnitude > 0.05f);

            var holdUpStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonUp(0) && isHold);

            var tapStream = holdStream.Merge(holdUpStream)
                .Select(_ =>
                {

                    isHold = false;
                    return new PinchData();
                });

            return tapStream;
        }

        private async UniTask<bool> HoldCheck(float waitSeconds, CancellationToken ct)
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
        #endregion
    }
}