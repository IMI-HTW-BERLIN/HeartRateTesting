using LevelObjects.UserInteraction;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerBehavior
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private float movementSpeed;
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private GameObject groundChecker;
        [SerializeField] private float groundDistance;
        [SerializeField] private LayerMask groundMask;

        [SerializeField] private Camera playerCamera;
        [Header("UI Input")] [SerializeField] private GameObject pressButtonUI;
        [SerializeField] private float offsetAmountToCamera;

        [Header("Post Processing")] [SerializeField]
        private LayerMask rayHitMask;

        private PlayerInputActions _inputActions;
        private CharacterController _controller;

        private float _cameraRotation;
        private float _bodyRotation;
        private Vector3 _movementInput;
        private Vector3 _velocity;

        private bool _isGrounded;
        private Pressable _currentPressable;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Player.Move.performed += OnMoveInput;
            _inputActions.Player.Camera.performed += OnCameraInput;
            _inputActions.Player.Jump.performed += OnJumpInput;
            _inputActions.Player.Interact.performed += OnInteract;

            _controller = GetComponent<CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable() => _inputActions.Enable();

        private void OnDisable() => _inputActions.Disable();

        private void Update()
        {
            _isGrounded = Physics.CheckSphere(groundChecker.transform.position, groundDistance, groundMask);
            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -1f;

            // Move player
            Vector3 movement = transform.right * _movementInput.x + transform.forward * _movementInput.y;
            _controller.Move(movement * (movementSpeed * Time.deltaTime));

            // Add y-movement
            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);

            // Rotate camera up and down
            playerCamera.transform.localRotation = Quaternion.Euler(_cameraRotation, 0, 0f);
            // Rotate body left and right
            gameObject.transform.localRotation = Quaternion.Euler(0, _bodyRotation, 0f);


            // Update view depth (Post Processing)
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            GameManager.Instance.ChangeDepthOfFieldEffect(
                Physics.Raycast(ray, out RaycastHit raycastHit, 1000, rayHitMask) ? raycastHit.distance : 1000);
            CheckForPressable();
        }

        public void RespawnAt(Vector3 position)
        {
            _controller.enabled = false;
            transform.position = position;
            _controller.enabled = true;
        }

        public void MovePlayer(Vector3 movement, bool resetVelocity = false)
        {
            if (resetVelocity)
                _velocity.y = 0f;
            _controller.Move(movement);
        }

        public void AddVelocity(Vector3 velocity, bool resetVelocity = false)
        {
            if (resetVelocity)
                _velocity = Vector3.zero;
            _velocity += velocity;
        }

        private void OnMoveInput(InputAction.CallbackContext obj) =>
            _movementInput = obj.ReadValue<Vector2>();

        private void OnCameraInput(InputAction.CallbackContext obj)
        {
            Vector2 mouseDelta = obj.ReadValue<Vector2>() * Time.deltaTime;
            _cameraRotation = Mathf.Clamp(_cameraRotation - mouseDelta.y * mouseSensitivity, -90f, 90f);
            _bodyRotation += mouseDelta.x * mouseSensitivity;
        }

        private void OnJumpInput(InputAction.CallbackContext obj)
        {
            if (_isGrounded)
                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void OnInteract(InputAction.CallbackContext obj) => _currentPressable?.Press();


        private void CheckForPressable()
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit))
                if (hit.collider.TryGetComponent(out Pressable pressable) &&
                    hit.distance <= pressable.UseDistanceForPlayer)
                {
                    _currentPressable = pressable;
                    ShowPressableButton(_currentPressable.transform.position);
                    return;
                }

            if (!_currentPressable)
                return;
            _currentPressable = null;
            HidePressableButton();
        }

        private void ShowPressableButton(Vector3 worldPosition)
        {
            pressButtonUI.transform.position =
                worldPosition - (worldPosition - playerCamera.transform.position) * offsetAmountToCamera;
            pressButtonUI.SetActive(true);
        }

        private void HidePressableButton() => pressButtonUI.SetActive(false);
    }
}