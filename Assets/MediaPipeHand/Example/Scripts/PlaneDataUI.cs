using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Anipen.Subsystem.MeidaPipeHand.Example
{
    [RequireComponent(typeof(ARPlane))]
    public class PlaneDataUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text alignmentText;
        [SerializeField] private TMP_Text classificationText;

        private ARPlane plane;

        private void OnEnable()
        {
            plane = GetComponent<ARPlane>();
            plane.boundaryChanged += OnBoundaryChanged;
        }

        private void OnDisable()
        {
            plane.boundaryChanged -= OnBoundaryChanged;
        }

        private void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            classificationText.text = plane.classification.ToString();
            alignmentText.text = plane.alignment.ToString();

            transform.position = plane.center;
        }
    }
}