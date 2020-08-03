using UnityEngine;

namespace LevelObjects.UserInteraction
{
    public abstract class Pressable : MonoBehaviour
    {
        [SerializeField] protected float pressCooldown;

        /// <summary>
        /// The maximum distance between Player and Pressable that allows the player to press this Pressable.
        /// Will be used for visually showing the player that he can press this object.
        /// </summary>
        [Tooltip("The distance to the player that is required to press this object")] [SerializeField]
        protected float useDistanceForPlayer;

        protected float LastPress;

        public float UseDistanceForPlayer => useDistanceForPlayer;

        protected abstract void OnPress();

        protected abstract void OnShowInReach();

        protected abstract void OnHideInReach();

        public void Press()
        {
            if (LastPress + pressCooldown > Time.time)
                return;

            LastPress = Time.time;
            OnPress();
        }

        public void ShowInReach() => OnShowInReach();

        public void HideInReach() => OnHideInReach();
    }
}