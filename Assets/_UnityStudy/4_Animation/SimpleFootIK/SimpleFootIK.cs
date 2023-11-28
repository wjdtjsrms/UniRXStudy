using UnityEngine;

public class SimpleFootIK : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [Range(0, 1f)][SerializeField] private float distanceGround;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Left Foot
            // Position �� Rotation weight ����
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat("IKLeftFootWeight"));

            ///<summary>
            /// GetIKPosition 
            ///   => IK�� �Ϸ��� ��ü�� ��ġ �� ( �Ʒ����� �ƹ�Ÿ���� LeftFoot�� �ش��ϴ� ��ü�� ��ġ �� )
            /// Vector3.up�� ���� ���� 
            ///   => origin�� ��ġ�� ���� �÷� �ٴڿ� ���� �ٴ��� �ν� ���ϴ� �� �����ϱ� ����
            ///      (LeftFoot�� �߸� ������ �ֱ� ������ �߹ٴڰ� ��� ���� �Ÿ��� �ְ�, Vector3.up�� �������� ������ �߸� �������� ó���� �Ǿ� �� �Ϻΰ� �ٴڿ� ����.)
            ///</summary>
            Ray leftRay = new(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            // distanceGround: LeftFoot���� �������� �Ÿ�
            // +1�� ���� ����: Vector3.up�� ���־��� ����
            if (Physics.Raycast(leftRay, out RaycastHit leftHit, distanceGround + 1f, layerMask))
            {
                // ���� �� �ִ� ���̶��
                if (leftHit.transform.CompareTag("WalkableGround"))
                {
                    Vector3 footPosition = leftHit.point;
                    footPosition.y += distanceGround;

                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, leftHit.normal));
                }
            }

            // Right Foot
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat("IKRightFootWeight"));

            Ray rightRay = new(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(rightRay, out RaycastHit rightHit, distanceGround + 1f, layerMask))
            {
                if (rightHit.transform.CompareTag("WalkableGround"))
                {
                    Vector3 footPosition = rightHit.point;
                    footPosition.y += distanceGround;

                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rightHit.normal));
                }

            }
        }
    }
}