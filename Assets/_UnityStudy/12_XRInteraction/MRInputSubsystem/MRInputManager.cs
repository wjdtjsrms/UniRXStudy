namespace Anipen.Subsystem.MRInput
{
    using JSGCode.Util;

    public class MRInputManager : SingletonMonoBehaviour<MRInputManager>
    {
        public PinchSubsystem PinchSubsystem { get; private set; }

        private void Awake()
        {
            PinchSubsystem = new PinchSubsystem(new EditorPinchProvider());
            PinchSubsystem.Start();
        }

        private void OnDestroy()
        {
            PinchSubsystem.Stop();
        }
    }
}