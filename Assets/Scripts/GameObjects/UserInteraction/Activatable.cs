using UnityEngine;

namespace GameObjects.UserInteraction
{
    public abstract class Activatable : MonoBehaviour
    {
        protected bool Activated;
        
        public void Activate()
        {
            Activated = true;
            OnActivation();
        }

        public void Deactivate()
        {
            Activated = false;
            OnDeactivation();
        }

        public void Toggle()
        {
            if(!Activated)
                Activate();
            else
                Deactivate();
        }

        protected abstract void OnActivation();
        
        protected abstract void OnDeactivation();
    }
}