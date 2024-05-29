using Anipen.Subsystem.MRInput;
using UnityEngine;

public class MRInputTest : MonoBehaviour, ITapHandler
{
    private void Start()
    {
        MRInputManager.Instance.RegistTabEvent(this);
    }

    private void OnDestroy()
    {
        MRInputManager.Instance?.UnregistTabEvent(this);
    }

    public void OnTap(InputData data)
    {
        Debug.Log(data);
    }
}