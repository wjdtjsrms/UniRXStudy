using UnityEngine;
using UnityEngine.XR.Hands;

namespace Anipen.Subsystem.MeidaPipeHand
{
    public class MediaPipeHandSubsystem : XRHandSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterDescriptor()
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
}