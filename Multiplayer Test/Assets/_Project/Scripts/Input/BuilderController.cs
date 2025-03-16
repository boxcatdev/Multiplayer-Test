using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BuilderIH))]
public class BuilderController : MonoBehaviour
{
    private BuilderIH _input;

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
    public bool LockCameraPosition = false;
    [SerializeField] private Transform _cameraPivot;
    [Space]
    [SerializeField] private float _cameraPitch = 50f;
    [SerializeField] private float _rotationSpeed = 0.25f;
    //[SerializeField] private Vector3 _cameraEulerAngles;
    private float _cameraYaw = 0f;
    private bool _toggleCamRotate = true;

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
        _input = GetComponent<BuilderIH>();
    }
    private void OnEnable()
    {
        _input.OnSecondaryPress += SecondaryInput;
    }
    private void OnDisable()
    {
        _input.OnSecondaryPress -= SecondaryInput;
    }
    private void Start()
    {
        AssignAnimationIDs();

        Cursor.lockState = CursorLockMode.Locked;
        //LockCameraPosition = true;
    }
    private void FixedUpdate()
    {
        //MoveUpdate();
        OldMove();
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

            _cameraYaw += (_toggleCamRotate ? _input.look.x : 0f) * _rotationSpeed * deltaTimeMultiplier;
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

    private float _targetRotation;
    private float _rotationVelocity;
    private float RotationSmoothTime = 0.12f;
    private void OldMove()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? _sprintSpeed : _playerSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = 1f;
        //float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _targetSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.fixedDeltaTime * _accelerationLerp);

            // round speed to 3 decimal places
            _targetSpeed = Mathf.Round(_targetSpeed * 1000f) / 1000f;
        }
        else
        {
            _targetSpeed = targetSpeed;
        }

        //_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        //if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _cameraPivot.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_targetSpeed * Time.fixedDeltaTime) +
                         new Vector3(0.0f, 0f, 0.0f) * Time.fixedDeltaTime);

        // animation
        if (_animator == null) return;
        _animator.SetFloat(_animIDSpeed, _targetSpeed);

        // update animator if using character
        /*if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }*/
    }

    private void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }
    #endregion

    #region Cursor
    private void SecondaryInput(bool down)
    {
        if (down == false) return;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            _toggleCamRotate = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            _toggleCamRotate = true;
        }

        /*if (down == true)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }*/
    }
    #endregion
}
