using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Anipen.Subsystem.MeidaPipeHand.Example
{
    public class PinchSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject rightSpawnPrefab;
        [SerializeField] private GameObject leftSpawnPrefab;
        [SerializeField] private Transform polySpatialCameraTransform;

        private static readonly List<XRHandSubsystem> subsystemsReuse = new();
        private XRHandSubsystem handSubsystem;
        private XRHandJoint rightIndexTipJoint;
        private XRHandJoint rightThumbTipJoint;
        private XRHandJoint leftIndexTipJoint;
        private XRHandJoint leftThumbTipJoint;
        private bool activeRightPinch;
        private bool activeLeftPinch;
        private float scaledThreshold;
        private const float pinchThreshold = 0.02f;

        private void Start()
        {
            GetHandSubsystem();
            scaledThreshold = pinchThreshold / polySpatialCameraTransform.localScale.x;
        }

        private void Update()
        {
            if (!CheckHandSubsystem())
                return;

            var updateSuccessFlags = handSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
            {
                // assign joint values
                rightIndexTipJoint = handSubsystem.rightHand.GetJoint(XRHandJointID.IndexTip);
                rightThumbTipJoint = handSubsystem.rightHand.GetJoint(XRHandJointID.ThumbTip);

                DetectPinch(rightIndexTipJoint, rightThumbTipJoint, ref activeRightPinch, true);
            }

            if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
            {
                // assign joint values
                leftIndexTipJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
                leftThumbTipJoint = handSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

                DetectPinch(leftIndexTipJoint, leftThumbTipJoint, ref activeLeftPinch, false);
            }
        }

        private void GetHandSubsystem()
        {
            SubsystemManager.GetSubsystems(subsystemsReuse);
            for (var i = 0; i < subsystemsReuse.Count; ++i)
            {
                var handSubsystem = subsystemsReuse[i];
                if (handSubsystem.running)
                {
                    this.handSubsystem = handSubsystem;
                    if (!CheckHandSubsystem())
                        return;

                    this.handSubsystem.Start();
                    break;
                }
            }
        }

        private bool CheckHandSubsystem()
        {
            if (handSubsystem == null)
            {
                enabled = false;
                return false;
            }

            return true;
        }

        private void DetectPinch(XRHandJoint index, XRHandJoint thumb, ref bool activeFlag, bool right)
        {
            var spawnObject = right ? rightSpawnPrefab : leftSpawnPrefab;

            if (index.trackingState != XRHandJointTrackingState.None &&
                thumb.trackingState != XRHandJointTrackingState.None)
            {
                Vector3 indexPOS = Vector3.zero;
                Vector3 thumbPOS = Vector3.zero;

                if (index.TryGetPose(out Pose indexPose))
                {
                    // adjust transform relative to the PolySpatial Camera transform
                    indexPOS = polySpatialCameraTransform.InverseTransformPoint(indexPose.position);
                }

                if (thumb.TryGetPose(out Pose thumbPose))
                {
                    // adjust transform relative to the PolySpatial Camera adjustments
                    thumbPOS = polySpatialCameraTransform.InverseTransformPoint(thumbPose.position);
                }

                var pinchDistance = Vector3.Distance(indexPOS, thumbPOS);

                if (pinchDistance <= scaledThreshold)
                {
                    if (!activeFlag)
                    {
                        Instantiate(spawnObject, indexPOS, Quaternion.identity);
                        activeFlag = true;
                    }
                }
                else
                {
                    activeFlag = false;
                }
            }
        }
    }
}