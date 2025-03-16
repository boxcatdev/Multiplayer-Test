using PatchworkGames;
using UnityEngine;

public class CameraSpring : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float _targetDistance = 10f;
    [Space]
    [SerializeField] private LayerMask _ignoreLayer;

    private void Update()
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        Vector3 origin = transform.parent.position;
        Vector3 direction = -transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, ~_ignoreLayer))
        {
            if (hit.distance < _targetDistance)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -hit.distance);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -_targetDistance);
            }
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -_targetDistance);
        }
    }
}
