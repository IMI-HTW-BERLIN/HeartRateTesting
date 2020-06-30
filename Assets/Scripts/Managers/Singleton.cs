﻿using UnityEngine;

namespace Managers
{
    public class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this as T;
            
            DontDestroyOnLoad(gameObject);
        }
    }
}
