﻿using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float movementSensitivity;
        [SerializeField] private float mouseSensitivity;
        [SerializeField] private float gravityValue;
        [SerializeField] private float jumpHeight;
        
        private PlayerInputActions _inputActions;

        private float _xRotation;

        private Vector3 _movementInput;

        private float _horizontalMovement;

        private Rigidbody _rb;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Player.Move.performed += OnMoveInput;
            _inputActions.Player.Camera.performed += OnCameraInput;
            _inputActions.Player.Jump.performed += OnJumpInput;
            _inputActions.Enable();

            _rb = gameObject.GetComponent<Rigidbody>();

            //Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            TurnCamera();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void OnJumpInput(InputAction.CallbackContext obj)
        {
            _rb.AddForce(new Vector3(0, jumpHeight * -gravityValue, 0));
        }

        private void OnCameraInput(InputAction.CallbackContext obj)
        {
            Vector2 mouseDelta = obj.ReadValue<Vector2>();
            // Rotate body on Y-Axis
            gameObject.transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
            // Rotate camera on X-Axis
            float newRotation = _xRotation - mouseDelta.y * mouseSensitivity;
            _xRotation = Mathf.Clamp(newRotation, -90f, 90f);
        }

        private void OnMoveInput(InputAction.CallbackContext obj) => _movementInput = obj.ReadValue<Vector2>();

        private void MovePlayer()
        {
            Transform t = transform;
            Vector3 movement = (t.right * _movementInput.x + t.forward * _movementInput.y) * movementSensitivity;
            _rb.AddForce(movement);
        }

        private void TurnCamera() => playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}