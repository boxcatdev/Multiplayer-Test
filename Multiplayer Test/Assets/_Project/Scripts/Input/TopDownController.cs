using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]
public class TopDownController : MonoBehaviour
{
    private InputHandler _input;

    [Header("Movement Settings")]
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _accelerationLerp = 5f;
    [SerializeField] private float _decelerationLerp = 5f;
    [Space]
    [SerializeField] private bool _canMove = true;
    public bool canMove => _canMove;

    [Header("Camera")]
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Vector3 _cameraEulerAngles;

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
        _input = GetComponent<InputHandler>();
    }
    private void Start()
    {
        AssignAnimationIDs();
    }
    private void FixedUpdate()
    {
        MoveUpdate();
        SimpleKeepGrounded();
        SimpleCameraRotation();
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

        //_isGrounded = _controller.isGrounded;

        /*Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, 1f);

        if(hits.Length > 0)
        {
            Debug.Log(hits.Length);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        }*/
    }
    private void SimpleCameraRotation()
    {
        if(_cameraPivot == null) return;

        _cameraPivot.rotation = Quaternion.Euler(_cameraEulerAngles);
    }
    private void MoveUpdate()
    {
        if (_canMove == false) return;

        Vector3 adjMove = new Vector3(_input.move.x, 0, _input.move.y).normalized;
        if (!adjMove.Equals(Vector3.zero)) _decelerationDirection = adjMove;

        if (!adjMove.Equals(Vector3.zero))
        {
            // move in direction of input
            if (_targetSpeed <= _playerSpeed - 0.01f)
                _targetSpeed = Mathf.Lerp(_targetSpeed, _playerSpeed, _accelerationLerp * Time.fixedDeltaTime);
            else
                _targetSpeed = _playerSpeed;

            _controller.Move(adjMove * _targetSpeed * Time.fixedDeltaTime);

            // rotate in direction of movement
            float targetRotation = Mathf.Atan2(adjMove.x, adjMove.z) * PI_MULT;
            targetRotation = Mathf.Round(targetRotation);
            Quaternion targetQ = Quaternion.Euler(0.0f, targetRotation, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetQ, 20f * 0.01f);

            // animation
            if (_animator == null) return;
            _animator.SetFloat(_animIDSpeed, _targetSpeed);

            // grounded check
            //_isGrounded = _controller.isGrounded;
        }
        else
        {
            if(_targetSpeed >= 0.01f)
                _targetSpeed = Mathf.Lerp(_targetSpeed, 0f, _decelerationLerp * Time.fixedDeltaTime);
            else
                _targetSpeed = 0f;

            _controller.Move(_decelerationDirection * _targetSpeed * Time.fixedDeltaTime);

            if (_animator == null) return;
            _animator.SetFloat(_animIDSpeed, _targetSpeed);

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
