using GameObjects.UserInteraction;
using UnityEngine;

namespace GameObjects
{
    public class Door : Activatable
    {
        [SerializeField] private float animationTime;


        protected override void OnActivation()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDeactivation()
        {
            throw new System.NotImplementedException();
        }
    }
}