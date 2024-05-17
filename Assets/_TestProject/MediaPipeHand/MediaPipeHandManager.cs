using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

public class MediaPipeHandManager : MonoBehaviour
{
    public Transform handPos;
    public Transform tempJointPosition;

    MediaPipeHandSubsystem handSubsystem;
    XRHandProviderUtility.SubsystemUpdater m_SubsystemUpdater;

    private void Awake()
    {
        var currentHandSubsystem = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(currentHandSubsystem);
        foreach(var handSubsystem in currentHandSubsystem)
        {
            if (handSubsystem.running)
                handSubsystem.Stop();
        }

        var descriptors = new List<XRHandSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(descriptors);
        for(int i = 0; i < descriptors.Count; ++i)
        {
            var descriptor = descriptors[i];
            if (descriptor.id == MeidaPipeHandProvider.DescriptorId)
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

        m_SubsystemUpdater = new XRHandProviderUtility.SubsystemUpdater(handSubsystem);
        m_SubsystemUpdater.Start();
        handSubsystem.Start();

        Debug.Log("START INIT HAND SUBSYSTEM");
    }
}