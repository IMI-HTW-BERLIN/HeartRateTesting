using UnityEngine;

namespace LevelObjects.UserInteraction
{
    public abstract class Activatable : MonoBehaviour
    {
        protected bool Activated;

        public void Activate()
        {
            OnActivation();
            Activated = true;
        }

        public void Deactivate()
        {
            OnDeactivation();
            Activated = false;
        }

        public void Toggle()
        {
            if (!Activated)
                Activate();
            else
                Deactivate();
        }

        protected abstract void OnActivation();

        protected abstract void OnDeactivation();
    }
}