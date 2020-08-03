using Managers;
using UnityEngine;

namespace Animation
{
    public abstract class TimeScaleObject : MonoBehaviour
    {
        protected abstract void TimeScaledUpdate(float timeScaledDeltaTime);

        protected virtual void TimeScaledFixedUpdate(float timeScaledFixedDeltaTime)
        {
        }

        protected virtual void Update()
        {
            TimeScaledUpdate(Time.deltaTime * GameManager.Instance.TimeScale);
        }

        protected virtual void FixedUpdate()
        {
            TimeScaledFixedUpdate(Time.deltaTime * GameManager.Instance.TimeScale);
        }
    }
}