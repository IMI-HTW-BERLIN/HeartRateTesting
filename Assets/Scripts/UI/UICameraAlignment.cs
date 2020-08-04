using UnityEngine;

namespace UI
{
    public class UICameraAlignment : MonoBehaviour
    {
        private Camera _camera;
        private void Awake() => _camera = Camera.main;

        private void Update()
        {
            transform.LookAt(_camera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}