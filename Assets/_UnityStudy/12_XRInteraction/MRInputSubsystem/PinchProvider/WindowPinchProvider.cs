namespace Anipen.Subsystem.MRInput
{
    using Cysharp.Threading.Tasks;
    using R3;
    using System;
    using System.Threading;
    using UnityEngine;

    public class WindowPinchProvider : PinchProvider
    {
        #region Member
        private bool isTap = false;

        private bool isHoldCheck = false;
        private bool isHold = false;

        private bool isMoveCheck = false;
        private bool isMove = false;
        #endregion

        #region Tap Stream
        protected override Observable<PinchData> GetTapStream() => GetMultiTapStream(targetTapCount: 1);

        protected override Observable<PinchData> GetDoubleTapStream() => GetMultiTapStream(targetTapCount: 2);

        private Observable<PinchData> GetMultiTapStream(int targetTapCount)
        {
            if (targetTapCount < 1)
                return default;

            var data = new PinchData
            {
                inputPhase = MRInputPhase.Begin
            };

            targetTapCount *= 2;

            var tapStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Select(_ => Input.mousePosition)
                .Merge(Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)).Select(_ => Input.mousePosition));

            var tapBufferStream = tapStream
                .Select(pos =>
                {
                    isTap = true;
                    return pos;
                })
                .Chunk(tapStream.Debounce(TimeSpan.FromMilliseconds(TAP_THRESHOLD)))
                .Where(tapBuffer =>
                {
                    isTap = false;
                    return tapBuffer.Length == targetTapCount;
                })
                .Select(tapBuffer => tapBuffer[targetTapCount - 2])
                .Select(tapInput => data);

            return tapBufferStream;
        }
        #endregion

        #region Move Stream
        protected override Observable<PinchData> GetMoveStream()
        {
            return CreateMovingStream().Merge(CreateMoveStartStream()).Merge(CreateMoveEndStream());
        }

        private Observable<PinchData> CreateMoveStartStream()
        {
            var data = new PinchData
            {
                inputPhase = MRInputPhase.Begin
            };

            var moveStartStream = Observable.EveryUpdate()
                .WhereAwait(async (_, ct) =>
                {
                    await UniTask.WaitUntil(() => !isTap);
                    return Input.GetMouseButton(0);
                }, AwaitOperation.Switch)
                .Where(_ => !isMove && Input.mousePositionDelta.sqrMagnitude > MOVE_THRESHOLD)
                .Select(_ =>
                {
                    isMove = true;
                    return data;
                });

            return moveStartStream;
        }

        private Observable<PinchData> CreateMovingStream()
        {
            var data = new PinchData
            {
                inputPhase = MRInputPhase.Running
            };

            var movingStream = Observable.EveryUpdate()
               .Where(_ => Input.GetMouseButton(0) && isMove)
               .Where(_ => Input.mousePositionDelta.sqrMagnitude > MOVE_THRESHOLD)
               .Select(_ => data);

            return movingStream;
        }

        private Observable<PinchData> CreateMoveEndStream()
        {
            var data = new PinchData
            {
                inputPhase = MRInputPhase.End
            };

            var holdStartStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0) && (isMove && !isMoveCheck))
                .WhereAwait(async (_, ct) =>
                {
                    isMoveCheck = true;
                    bool holdSuccess = await HoldCheck(waitSeconds: HOLD_THRESHOLD - 0.1f, ct);
                    isMoveCheck = false;

                    return holdSuccess;
                }, AwaitOperation.Switch);

            var mouseUPStream = Observable.EveryUpdate()
               .Where(_ => isMove && Input.GetMouseButtonUp(0));

            var moveEndStream = mouseUPStream.Merge(holdStartStream)
                .Select(_ =>
                {
                    isMove = false;
                    return data;

                });

            return moveEndStream;
        }
        #endregion

        #region Hold Stream
        protected override Observable<PinchData> GetHoldStream()
        {
            return CreateHoldingStream().Merge(CreateHoldStartStream()).Merge(CreateHoldEndsStream());
        }

        private Observable<PinchData> CreateHoldStartStream()
        {
            var data = new PinchData
            {
                inputPhase = MRInputPhase.Begin
            };

            var holdStartStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0) && (!isHold && !isHoldCheck))
                .WhereAwait(async (_, ct) =>
                {
                    isHoldCheck = true;
                    bool holdSuccess = await HoldCheck(waitSeconds: HOLD_THRESHOLD, ct);
                    isHoldCheck = false;

                    return holdSuccess;
                }, AwaitOperation.Switch)
                .Select(_ =>
                {
                    isHold = true;
                    return data;
                });

            return holdStartStream;
        }

        private Observable<PinchData> CreateHoldingStream()
        {
            var data = new PinchData
            {
                inputPhase = MRInputPhase.Running
            };

            var holdingStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0) && isHold)
                .Select(_ => data);

            return holdingStream;
        }

        private Observable<PinchData> CreateHoldEndsStream()
        {
            var data = new PinchData
            {
                inputPhase = MRInputPhase.End
            };

            var moveStartStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0))
                .Where(_ => Input.mousePositionDelta.sqrMagnitude > MOVE_THRESHOLD);

            var mouseUpStream = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonUp(0));

            var holdEndStream = moveStartStream.Merge(mouseUpStream)
                .Where(_ => isHold)
                .Select(_ =>
                {
                    isHold = false;
                    return data;
                });

            return holdEndStream;
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

                if (Input.mousePositionDelta.sqrMagnitude > MOVE_THRESHOLD)
                    currentTime = 0f;
            }

            return true;
        }
        #endregion
    }
}