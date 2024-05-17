using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Anipen.Subsystem.MeidaPipeHand.Example
{
    public class PlaceOnPlane : MonoBehaviour
    {
        [SerializeField]
        private Transform cameraTransform;

        [SerializeField]
        private GameObject headPoseReticle;
        private GameObject spawnedHeadPoseReticle;
        private RaycastHit hitInfo;

        private void Start()
        {
            spawnedHeadPoseReticle = Instantiate(headPoseReticle, Vector3.zero, Quaternion.identity);
        }

        private void Update()
        {
            if (Physics.Raycast(new Ray(cameraTransform.position, cameraTransform.forward), out hitInfo))
            {
                if (hitInfo.transform.TryGetComponent(out ARPlane plane))
                {
                    spawnedHeadPoseReticle.transform.SetPositionAndRotation(hitInfo.point, Quaternion.FromToRotation(spawnedHeadPoseReticle.transform.up, hitInfo.normal));
                }
            }
        }
    }

}