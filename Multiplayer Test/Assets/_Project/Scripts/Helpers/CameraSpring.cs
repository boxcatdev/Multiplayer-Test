using PatchworkGames;
using UnityEngine;

public class CameraSpring : MonoBehaviour
{
    [Header("Spring")]
    //[SerializeField] private Transform _camPivot;
    [SerializeField] private float _targetDistance = 10f;

    private void Update()
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        //if (_camPivot == null) return;

        //Vector3 origin = _camPivot.position;
        //Vector3 direction = transform.position - origin;

        Vector3 origin = transform.parent.position;
        Vector3 direction = -transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit))
        {
            if (hit.distance < _targetDistance)
            {
                transform.localPosition = new Vector3(0, 0, -hit.distance);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, -_targetDistance);
            }
        }
        else
        {
            transform.localPosition = new Vector3(0, 0, -_targetDistance);
        }
    }
}
