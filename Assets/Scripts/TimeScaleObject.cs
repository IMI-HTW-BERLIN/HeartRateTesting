using Managers;
using UnityEngine;

public abstract class TimeScaleObject : MonoBehaviour
{
    protected abstract void TimeScaledUpdate(float timeScaledDeltaTime);

    protected abstract void TimeScaledFixedUpdate(float timeScaledFixedDeltaTime);

    protected virtual void Update()
    {
        TimeScaledUpdate(Time.deltaTime * GameManager.Instance.TimeScale);
    }

    protected virtual void FixedUpdate()
    {
        TimeScaledFixedUpdate(Time.deltaTime * GameManager.Instance.TimeScale);
    }
}