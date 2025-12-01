using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private LayerMask flagLayer;

    public System.Action<FlagTarget> OnFlagHit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, flagLayer))
        {
            FlagTarget target = hit.collider.GetComponent<FlagTarget>();
            if (target != null)
            {
                OnFlagHit?.Invoke(target);
            }
        }
    }
}
