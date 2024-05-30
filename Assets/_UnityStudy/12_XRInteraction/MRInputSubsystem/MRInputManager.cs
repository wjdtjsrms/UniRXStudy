namespace Anipen.Subsystem.MRInput
{
    using JSGCode.Util;

    public class MRInputManager : SingletonMonoBehaviour<MRInputManager>
    {
        public PinchSubsystem PinchSubsystem { get; private set; } = new();

        private void Awake()
        {
            PinchSubsystem.Start();
        }

        private void OnDestroy()
        {
            PinchSubsystem.Stop();
        }
    }
}