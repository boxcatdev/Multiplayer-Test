using UnityEngine;

public class BillboardComponent : MonoBehaviour
{
    [Header("Set Rotation")]
    [SerializeField] private Vector3 _eulerRotation;
    [SerializeField] private bool _useSetRotation = true;
    [SerializeField] private bool _useCamDirection = false; 

    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (_useSetRotation)
        {
            transform.rotation = Quaternion.Euler(_eulerRotation);
        }

        if (_useCamDirection)
        {
            Vector3 direction = transform.position - _mainCam.transform.position;
            transform.forward = -direction;
        }
    }
}
