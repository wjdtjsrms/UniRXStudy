using Anipen.Subsystem.MRInput;
using UnityEngine;

public class MRInputTest : MonoBehaviour, ITapHandler
{
    public void OnTap(MRInputData data)
    {
        Debug.Log(data);
    }

    private void Start()
    {
        MRInputManager.Instance.RegistTabEvent(this);
    }

    private void OnDestroy()
    {
        MRInputManager.Instance?.UnregistTabEvent(this);
    }
}
