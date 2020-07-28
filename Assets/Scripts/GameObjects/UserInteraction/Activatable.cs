using UnityEngine;

namespace GameObjects.UserInteraction
{
    public abstract class Activatable : TimeScaleObject
    {
        protected bool Activated;

        [ExecuteAlways]
        public void Activate()
        {
            OnActivation();
            Activated = true;
        }

        [ExecuteAlways]
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