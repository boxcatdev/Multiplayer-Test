using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BuilderInputH))]
public class BuilderController : MonoBehaviour
{
    private BuilderInputH _input;

    [Header("Movement Settings")]
    [SerializeField] private float _playerSpeed = 3f;
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _accelerationLerp = 8f;
    [SerializeField] private float _decelerationLerp = 15f;
    [Space]
    [SerializeField] private bool _canMove = true;
    //[SerializeField] private bool _smoothAnimation = true;
    public bool canMove => _canMove;

    [Header("Camera")]
    /*[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]*/
    public bool LockCameraPosition = false;
    [Space]
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private float _cameraPitch = 50f;
    //[SerializeField] private Vector3 _cameraEulerAngles;
    private float _cameraYaw = 0f;

    private CharacterController _controller;
    public const float PI_MULT = 57.29f;
    private float _targetSpeed = 0;
    private Vector3 _decelerationDirection;

    //animation
    private Animator _animator;
    private int _animIDSpeed;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _input = GetComponent<BuilderInputH>();
    }
    private void Start()
    {
        AssignAnimationIDs();

        //Cursor.lockState = CursorLockMode.Locked;
        LockCameraPosition = true;
    }
    private void FixedUpdate()
    {
        MoveUpdate();
        SimpleKeepGrounded();
        //SimpleCameraRotation();
    }
    private void LateUpdate()
    {
        CameraRotation(false);
    }

    #region Setup
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
    }
    #endregion

    #region Movement
    private void SimpleKeepGrounded()
    {
        // keep at y = 0
        if (transform.position.y != 0f) transform.position = new Vector3(transform.position.x, -0.01f, transform.position.z);

    }
    private void SimpleCameraRotation()
    {
        if (_cameraPivot == null) return;

        //_cameraPivot.rotation = Quaternion.Euler(_cameraEulerAngles);
    }
    private void CameraRotation(bool fixedTimeStep)
    {
        float threshold = 0.01f;
        if (_input.look.sqrMagnitude > threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = _input.isGamepad ? (fixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) : 1.0f;

            _cameraYaw += _input.look.x * deltaTimeMultiplier;
        }

        _cameraPivot.rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0);

        #region Old
        /*// if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);*/
        #endregion
    }
    private void MoveUpdate()
    {
        if (_canMove == false) return;

        Vector3 adjMove = new Vector3(_input.move.x, 0, _input.move.y).normalized;
        if (!adjMove.Equals(Vector3.zero)) _decelerationDirection = adjMove;

        if (!adjMove.Equals(Vector3.zero))
        {
            // move in direction of input
            float currentTargetSpeed = _input.sprint == true ? _sprintSpeed : _playerSpeed;
            if (_targetSpeed <= currentTargetSpeed - 0.01f)
                _targetSpeed = Mathf.Lerp(_targetSpeed, currentTargetSpeed, _accelerationLerp * Time.fixedDeltaTime);
            else
                _targetSpeed = currentTargetSpeed;

            _controller.Move(adjMove * _targetSpeed * Time.fixedDeltaTime);

            // rotate in direction of movement
            float targetRotation = Mathf.Atan2(adjMove.x, adjMove.z) * PI_MULT;
            targetRotation = Mathf.Round(targetRotation);
            Quaternion targetQ = Quaternion.Euler(0.0f, targetRotation, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQ, 20f * 0.01f);

            // animation
            if (_animator == null) return;
            _animator.SetFloat(_animIDSpeed, _targetSpeed); // _smoothAnimation == true ? _targetSpeed : 

            // grounded check
            //_isGrounded = _controller.isGrounded;
        }
        else
        {
            if (_targetSpeed >= 0.01f)
                _targetSpeed = Mathf.Lerp(_targetSpeed, 0f, _decelerationLerp * Time.fixedDeltaTime);
            else
                _targetSpeed = 0f;

            _controller.Move(_decelerationDirection * _targetSpeed * Time.fixedDeltaTime);

            // animation
            if (_animator == null) return;
            _animator.SetFloat(_animIDSpeed, _targetSpeed); // _smoothAnimation == true ? _targetSpeed :

            //float animSpeed = _animator.GetFloat(_animIDSpeed);
            //float targetSpeed = Mathf.Lerp(animSpeed, 0, 2f * Time.fixedDeltaTime);
            //_animator.SetFloat(_animIDSpeed, 0);
        }
    }

    private void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }
    #endregion
}
