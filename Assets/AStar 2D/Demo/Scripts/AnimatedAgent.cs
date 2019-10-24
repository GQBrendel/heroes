using UnityEngine;
using System.Collections;

namespace AStar_2D.Demo
{ 
    /// <summary>
    /// Example class that shows how the agent class can be inherited from and expanded.
    /// </summary>
    public class AnimatedAgent : Agent
    {
        // Private
        private Animator anim = null;
        private float timer = 0;
        private bool canBob = true;
        public bool moved = false;

      

        // Methods
        /// <summary>
        /// Called by unity.
        /// Note that the base method is called. This is essential to initialize the base class.
        /// </summary>
        public override void Start()
        {
            // Make sure we call start on the agent class
            base.Start();

            // Find the animator controller
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// Called by Unity.
        /// Note that the base method is called. This is essential to update the base class.
        /// </summary>
        public override void Update()
        {
            // Make sure we update our agents movement
            base.Update();

            // Update the sprite animation so the character is facing the correct direction
            updateAnimation();
            
        }

        /// <summary>
        /// Called when the agent is unable to reach a target destination.
        /// </summary>
        public override void onDestinationUnreachable()
        {
            Debug.LogWarning(string.Format("Agent [{0}]: I can't reach that target", gameObject.name));
        }

        public override void onDestinationReached()
        {
            Debug.Log("Terminei de mover");
            moved = true;
            if(gameObject.tag.Contains("Hero")) {
              //  TileManager.Instance.SendMessage("endAction");
            }
        }

        private void updateAnimation()
        {
            anim.SetBool("IsMoving", IsMoving);
         }
        
    }
}
