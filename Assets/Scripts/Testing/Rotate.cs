using System;
using UnityEngine;

namespace Testing
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private Vector3 rotation;
        private void Update()
        {
            transform.Rotate(rotation * Time.deltaTime);
        }
    }
}