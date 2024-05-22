using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

namespace Anipen.Subsystem.MeidaPipeHand
{
    public class MeidaPipeHandProvider : XRHandSubsystemProvider
    {
        private readonly MediaPipeTracking leftHandTracking = new(true);
        private readonly MediaPipeTracking rightHandTracking = new(false);
        private MediaPipeHandManager handManager;
        private NativeArray<bool> jointsInLayout;
        private NativeArray<XRHandJoint> leftHandJoints;
        private NativeArray<XRHandJoint> rightHandJoints;
        private static readonly float HAND_SCALE = 0.05f;

        public static string DESCRIPTOR_ID => "MeidaPipe_Hands";
        public XRHandSubsystem.UpdateType MostRecentUpdateType { get; private set; }
        public bool LeftHandIsTracked { get; set; } = true;
        public bool RightHandIsTracked { get; set; } = true;

        public override void Start()
        {
            handManager = Object.FindObjectOfType<MediaPipeHandManager>();

            if (handManager == null)
            {
                Debug.LogError("Couldn't find MediaPipeHandManager");
                return;
            }

            leftHandTracking.Start();
            rightHandTracking.Start();

            LeftHandIsTracked = true;
            RightHandIsTracked = true;
        }

        public override void Stop()
        {
            leftHandTracking?.Stop();
            rightHandTracking?.Stop();

            leftHandJoints.Dispose();
            rightHandJoints.Dispose();
            jointsInLayout.Dispose();
        }

        public override void Destroy()
        {
            Stop();
            handManager = null;
        }

        public override void GetHandLayout(NativeArray<bool> jointsInLayout)
        {
            jointsInLayout[XRHandJointID.Palm.ToIndex()] = false;
            jointsInLayout[XRHandJointID.Wrist.ToIndex()] = true;

            jointsInLayout[XRHandJointID.ThumbMetacarpal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.ThumbProximal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.ThumbDistal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.ThumbTip.ToIndex()] = true;

            jointsInLayout[XRHandJointID.IndexMetacarpal.ToIndex()] = false;
            jointsInLayout[XRHandJointID.IndexProximal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.IndexIntermediate.ToIndex()] = true;
            jointsInLayout[XRHandJointID.IndexDistal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.IndexTip.ToIndex()] = true;

            jointsInLayout[XRHandJointID.MiddleMetacarpal.ToIndex()] = false;
            jointsInLayout[XRHandJointID.MiddleProximal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.MiddleIntermediate.ToIndex()] = true;
            jointsInLayout[XRHandJointID.MiddleDistal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.MiddleTip.ToIndex()] = true;

            jointsInLayout[XRHandJointID.RingMetacarpal.ToIndex()] = false;
            jointsInLayout[XRHandJointID.RingProximal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.RingIntermediate.ToIndex()] = true;
            jointsInLayout[XRHandJointID.RingDistal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.RingTip.ToIndex()] = true;

            jointsInLayout[XRHandJointID.LittleMetacarpal.ToIndex()] = false;
            jointsInLayout[XRHandJointID.LittleProximal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.LittleIntermediate.ToIndex()] = true;
            jointsInLayout[XRHandJointID.LittleDistal.ToIndex()] = true;
            jointsInLayout[XRHandJointID.LittleTip.ToIndex()] = true;

            this.jointsInLayout = jointsInLayout;
        }

        public override XRHandSubsystem.UpdateSuccessFlags TryUpdateHands(
            XRHandSubsystem.UpdateType updateType,
            ref Pose leftHandRootPose,
            NativeArray<XRHandJoint> leftHandJoints,
            ref Pose rightHandRootPose,
            NativeArray<XRHandJoint> rightHandJoints)
        {
            this.leftHandJoints = leftHandJoints;
            this.rightHandJoints = rightHandJoints;
            MostRecentUpdateType = updateType;

            int mediaPipeJointIndex = -1;

            if (leftHandTracking.TryUpdateData(out var leftData))
            {
                for (int unityJointIndex = 0; unityJointIndex < jointsInLayout.Length; unityJointIndex++)
                {
                    if (jointsInLayout[unityJointIndex] == false)
                        continue;

                    mediaPipeJointIndex++;
                    leftHandJoints[unityJointIndex] = GetJoint(leftData[mediaPipeJointIndex], unityJointIndex, isLeft: true);
                }
            }

            mediaPipeJointIndex = -1;

            if (rightHandTracking.TryUpdateData(out var rightData))
            {
                for (int unityJointIndex = 0; unityJointIndex < jointsInLayout.Length; unityJointIndex++)
                {
                    if (jointsInLayout[unityJointIndex] == false)
                        continue;

                    mediaPipeJointIndex++;
                    rightHandJoints[unityJointIndex] = GetJoint(rightData[mediaPipeJointIndex], unityJointIndex, isLeft: false);
                }
            }

            var successFlags = XRHandSubsystem.UpdateSuccessFlags.All;

            if (!LeftHandIsTracked)
                successFlags &= ~XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints & ~XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose;

            if (!RightHandIsTracked)
                successFlags &= ~XRHandSubsystem.UpdateSuccessFlags.RightHandJoints & ~XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;

            return successFlags;
        }

        private XRHandJoint GetJoint(Vector3 handPos, int jointIndex, bool isLeft)
        {
            var handTR = handManager.HandTR;
            var targetPos = (handPos * HAND_SCALE) + handTR.position;

            var handedNess = isLeft ? Handedness.Left : Handedness.Right;
            var currentJointIndex = XRHandJointIDUtility.FromIndex(jointIndex);

            handManager.TempJointPosition.SetPositionAndRotation(targetPos, Quaternion.identity);
            handManager.TempJointPosition.RotateAround(handTR.position, Vector3.up, handTR.eulerAngles.y);

            var jointPose = new Pose(handManager.TempJointPosition.position, Quaternion.identity);

            return XRHandProviderUtility.CreateJoint(handedNess, XRHandJointTrackingState.Pose, currentJointIndex, jointPose);
        }
    }
}