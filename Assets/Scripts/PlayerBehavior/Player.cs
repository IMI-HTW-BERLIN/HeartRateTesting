using LevelObjects.UserInteraction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerBehavior
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [Header("Movement")] [SerializeField] private float movementSensitivity;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float mouseSensitivity;
        [Header("Jumping")] [SerializeField] private float jumpHeight;
        [SerializeField] private Transform groundChecker;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundDistance;
        [SerializeField] private float jumpCooldown;

        [Header("Interaction")] [SerializeField]
        private float interactionDistance;

        private PlayerInputActions _inputActions;
        private Rigidbody _rb;

        private float _xRotation;
        private float _yRotation;
        private Vector3 _movementInput;
        private float _horizontalMovement;

        private bool _canJump;
        private float _lastJumpTime;

        private Pressable _currentPressable;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Player.Move.performed += OnMoveInput;
            _inputActions.Player.Camera.performed += OnCameraInput;
            _inputActions.Player.Jump.performed += OnJumpInput;
            _inputActions.Player.Interact.performed += OnInteract;

            _rb = gameObject.GetComponent<Rigidbody>();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable() => _inputActions.Enable();

        private void OnDisable() => _inputActions.Disable();

        private void Update()
        {
            CheckForPressable();
            TurnPlayer();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            if (_rb.velocity.magnitude > maxSpeed)
                _rb.velocity = _rb.velocity.normalized * maxSpeed;
        }

        private void OnJumpInput(InputAction.CallbackContext obj)
        {
            if (!Physics.Raycast(groundChecker.position, Vector3.down, groundDistance, groundLayer) ||
                Time.unscaledTime - _lastJumpTime < jumpCooldown)
                return;
            _lastJumpTime = Time.unscaledTime;
            _rb.AddForce(new Vector3(0, jumpHeight, 0));
        }

        private void OnCameraInput(InputAction.CallbackContext obj)
        {
            Vector2 mouseDelta = obj.ReadValue<Vector2>();
            _xRotation = Mathf.Clamp(_xRotation - mouseDelta.y * mouseSensitivity, -90f, 90f);
            _yRotation += mouseDelta.x * mouseSensitivity;
        }

        private void OnMoveInput(InputAction.CallbackContext obj) => _movementInput = obj.ReadValue<Vector2>();

        private void TurnPlayer()
        {
            playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0f);
            gameObject.transform.localRotation = Quaternion.Euler(0, _yRotation, 0f);
        }

        private void MovePlayer()
        {
            Transform t = transform;
            Vector3 movement = (t.right * _movementInput.x + t.forward * _movementInput.y) * movementSensitivity;
            _rb.AddForce(movement);
        }

        private void CheckForPressable()
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;
            if (hit.collider.TryGetComponent(out Pressable pressable) && hit.distance <= pressable.UseDistanceForPlayer)
            {
                _currentPressable = pressable;
                _currentPressable.ShowInReach();
            }
            else if (_currentPressable)
            {
                _currentPressable.HideInReach();
                _currentPressable = null;
            }
        }

        private void OnInteract(InputAction.CallbackContext obj) => _currentPressable?.Press();
    }
}