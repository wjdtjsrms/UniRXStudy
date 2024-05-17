using UnityEngine;
using UnityEngine.XR.Hands;

public class MediaPipeHandSubsystem : XRHandSubsystem
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        var handsSubsystemCinfo = new XRHandSubsystemDescriptor.Cinfo
        {
            id = "MeidaPipe_Hands",
            providerType = typeof(MeidaPipeHandProvider),
            subsystemTypeOverride = typeof(MediaPipeHandSubsystem)
        };
        XRHandSubsystemDescriptor.Register(handsSubsystemCinfo);
    }
}
