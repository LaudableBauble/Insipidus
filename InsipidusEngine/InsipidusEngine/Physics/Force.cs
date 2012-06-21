using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsipidusEngine.Physics
{
    /**
     * A force instance is interpreted by the physics engine and is the only way unconnected bodies can communicate with each other in the game, ie. through collisions.
     * 
     * NOTE: CHANGE THIS INTO A STRUCT!!!
     */
    public class Force
    {
        // The target body, force direction and magnitude.
        private Body _Body;
        private Vector3 _Force;

        /**
         * Constructor for a force instance.
         */
        public Force()
        {
            this(new Body(), Vector3.empty());
        }

        /**
         * Constructor for a force instance.
         * 
         * @param body
         *            The target body.
         * @param force
         *            The velocity of the force.
         */
        public Force(Body body, Vector3 force)
        {
            // Set the variables.
            _Body = body;
            _Force = force;
        }

        /**
         * Constructor for a force instance.
         * 
         * @param body
         *            The target body.
         * @param force
         *            The velocity of the force.
         */
        public Force(Body body, Vector2 force)
        {
            // Set the variables.
            _Body = body;
            _Force = new Vector3(force);
        }

        /**
         * Get a blank force instance.
         * 
         * @return Blank force instance.
         */
        public static Force blank()
        {
            return new Force();
        }

        /**
         * Get the force's target body.
         */
        public Body getBody()
        {
            return _Body;
        }

        /**
         * Get the force's velocity.
         */
        public Vector3 getForce()
        {
            return _Force;
        }
    }

}
