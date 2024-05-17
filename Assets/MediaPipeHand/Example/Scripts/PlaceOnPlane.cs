using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    private Transform m_CameraTransform;

    [SerializeField]
    private GameObject m_HeadPoseReticle;
    private GameObject m_SpawnedHeadPoseReticle;
    private RaycastHit m_HitInfo;

    private void Start()
    {
        m_SpawnedHeadPoseReticle = Instantiate(m_HeadPoseReticle, Vector3.zero, Quaternion.identity);
    }

    private void Update()
    {
        if (Physics.Raycast(new Ray(m_CameraTransform.position, m_CameraTransform.forward), out m_HitInfo))
        {
            if (m_HitInfo.transform.TryGetComponent(out ARPlane plane))
            {
                m_SpawnedHeadPoseReticle.transform.SetPositionAndRotation(m_HitInfo.point, Quaternion.FromToRotation(m_SpawnedHeadPoseReticle.transform.up, m_HitInfo.normal));
            }
        }
    }
}
