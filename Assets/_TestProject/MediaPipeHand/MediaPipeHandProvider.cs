using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

public class MeidaPipeHandProvider : XRHandSubsystemProvider
{
    NativeArray<bool> jointsInLayout;
    public MeidaPipeHandProvider()
    {
    }

    public static string DescriptorId => "MeidaPipe_Hands";
    public int numStartCalls { get; private set; }
    public int numStopCalls { get; private set; }
    public int numDestroyCalls { get; private set; }
    public int numGetHandLayoutCalls { get; private set; }
    public int numTryUpdateHandsCalls { get; private set; }
    public XRHandSubsystem.UpdateType mostRecentUpdateType { get; private set; }

    public bool leftHandIsTracked { get; set; } = true;

    public bool rightHandIsTracked { get; set; } = true;

    private readonly MediaPipeTracking leftHandTracking = new(true);
    private readonly MediaPipeTracking rightHandTracking = new(false);
    private MediaPipeHandManager handManager;

    public override void Start()
    {
        handManager = Object.FindObjectOfType<MediaPipeHandManager>();

        leftHandTracking.Start();
        rightHandTracking.Start();

        leftHandIsTracked = true;
        rightHandIsTracked = true;
        ++numStartCalls;
    }

    public override void Stop()
    {
        leftHandTracking.Stop();
        rightHandTracking.Stop();

        ++numStopCalls;
    }

    public override void Destroy()
    {
        ++numDestroyCalls;
    }

    public override void GetHandLayout(NativeArray<bool> jointsInLayout)
    {
        ++numGetHandLayoutCalls;

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
        mostRecentUpdateType = updateType;
        ++numTryUpdateHandsCalls;

        int ii = -1;

        if (leftHandTracking.TryUpdateData(out var leftData))
        {
            for (int i = 0; i < jointsInLayout.Length; i++)
            {
                if (jointsInLayout[i] == false)
                    continue;

                ii++;
                leftHandJoints[i] = UpdateHand(leftData[ii], XRHandJointIDUtility.FromIndex(i), true);
            }
        }

        ii = -1;

        if (rightHandTracking.TryUpdateData(out var rightData))
        {
            for (int i = 0; i < jointsInLayout.Length; i++)
            {
                if (jointsInLayout[i] == false)
                    continue;

                ii++;
                rightHandJoints[i] = UpdateHand(rightData[ii], XRHandJointIDUtility.FromIndex(i), false);
            }
        }

        var successFlags = XRHandSubsystem.UpdateSuccessFlags.All;

        if (!leftHandIsTracked)
            successFlags &= ~XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints & ~XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose;

        if (!rightHandIsTracked)
            successFlags &= ~XRHandSubsystem.UpdateSuccessFlags.RightHandJoints & ~XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;

        return successFlags;
    }

    private XRHandJoint UpdateHand(Vector3 handPos, XRHandJointID jointID, bool isLeft)
    {
        var handTR = handManager.handPos;
        var targetPos = (handPos / 20f) + handTR.position;

        handManager.tempJointPosition.position = targetPos;
        handManager.tempJointPosition.rotation = Quaternion.identity;
        handManager.tempJointPosition.RotateAround(handTR.position, Vector3.up, handTR.eulerAngles.y);

        return XRHandProviderUtility.CreateJoint(
        isLeft ? Handedness.Left : Handedness.Right,
        XRHandJointTrackingState.Pose,
        jointID,
        new Pose(handManager.tempJointPosition.position, Quaternion.identity));
    }
}