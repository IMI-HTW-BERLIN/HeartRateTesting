using Managers;
using UnityEngine;

namespace Animation
{
    public abstract class TimeScaleObject : MonoBehaviour
    {
        private bool _slowMotionManagerNotNull;

        private void Awake()
        {
            _slowMotionManagerNotNull = SlowMotionManager.Instance != null;
        }

        protected abstract void TimeScaledUpdate(float timeScaledDeltaTime);

        protected virtual void TimeScaledFixedUpdate(float timeScaledFixedDeltaTime)
        {
        }

        protected virtual void Update()
        {
            TimeScaledUpdate(Time.deltaTime *
                             (_slowMotionManagerNotNull ? SlowMotionManager.Instance.TimeScale : Time.timeScale));
        }

        protected virtual void FixedUpdate()
        {
            TimeScaledFixedUpdate(Time.deltaTime *
                                  (_slowMotionManagerNotNull ? SlowMotionManager.Instance.TimeScale : Time.timeScale));
        }
    }
}