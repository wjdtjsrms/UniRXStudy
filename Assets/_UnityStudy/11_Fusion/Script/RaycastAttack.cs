using Fusion;
using UnityEngine;

public class RaycastAttack : NetworkBehaviour
{
    public float damage = 10;
    public PlayerMovement playerMovement;

    private void Update()
    {
        if (HasStateAuthority == false)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = playerMovement.playerCamera.ScreenPointToRay(Input.mousePosition);
            ray.origin += playerMovement.playerCamera.transform.forward;

            Debug.DrawRay(ray.origin, ray.direction, Color.red, 1f);

            if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction, out var hit))
            {
                if (hit.transform.TryGetComponent<Health>(out var health))
                {
                    health.DealDamageRPC(damage);
                }
            }
        }
    }
}
