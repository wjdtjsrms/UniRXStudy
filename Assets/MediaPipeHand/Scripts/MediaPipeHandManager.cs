using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

namespace Anipen.Subsystem.MeidaPipeHand
{
    public class MediaPipeHandManager : MonoBehaviour
    {
        [SerializeField] private Transform handTR;
        [SerializeField] private Transform tempJointPosition;

        private MediaPipeHandSubsystem handSubsystem;
        private XRHandProviderUtility.SubsystemUpdater subsystemUpdater;

        public Transform HandTR => handTR;
        public Transform TempJointPosition => tempJointPosition;

        private void Awake()
        {
            var currentHandSubsystem = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(currentHandSubsystem);
            foreach (var handSubsystem in currentHandSubsystem)
            {
                if (handSubsystem.running)
                    handSubsystem.Stop();
            }

            var descriptors = new List<XRHandSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(descriptors);
            for (int i = 0; i < descriptors.Count; ++i)
            {
                var descriptor = descriptors[i];
                if (descriptor.id == MeidaPipeHandProvider.DESCRIPTOR_ID)
                {
                    handSubsystem = descriptor.Create() as MediaPipeHandSubsystem;
                    break;
                }
            }

            if (handSubsystem == null)
            {
                Debug.LogError("Couldn't find Device Simulator hands subsystem.", this);
                return;
            }

            subsystemUpdater = new XRHandProviderUtility.SubsystemUpdater(handSubsystem);
            subsystemUpdater.Start();
            handSubsystem.Start();
        }
    }
}