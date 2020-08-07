using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class CoroutineManager : Singleton<CoroutineManager>
    {
        public void WaitUntil(Func<bool> predicate, UnityAction onFinish) =>
            StartCoroutine(WaitUntilCoroutine(predicate, onFinish));


        public void WaitForSeconds(float time, UnityAction onFinish) =>
            StartCoroutine(WaitForSecondsCoroutine(time, onFinish));

        public static IEnumerator WaitUntilCoroutine(Func<bool> predicate, UnityAction onFinish)
        {
            yield return new WaitUntil(predicate);
            onFinish?.Invoke();
        }

        public static IEnumerator WaitForSecondsCoroutine(float time, UnityAction onFinish)
        {
            yield return new WaitForSeconds(time);
            onFinish.Invoke();
        }
    }
}