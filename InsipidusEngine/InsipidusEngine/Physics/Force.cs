using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace InsipidusEngine.Physics
{
    /// <summary>
    /// A force instance is interpreted by the physics engine and is the only way unconnected bodies can communicate with each other in the game, ie. through collisions.
    /// </summary>
    public struct Force
    {
        /// <summary>
        /// Constructor for a force instance.
        /// </summary>
        /// <param name="body">The target body.</param>
        /// <param name="force">The velocity of the force.</param>
        public Force(Body body, Vector3 force)
        {
            // Set the variables.
            Body = body;
            Velocity = force;
        }

        /// <summary>
        /// The target body of the force.
        /// </summary>
        public Body Body
        {
            get;
            set;
        }
        /// <summary>
        /// The force's velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get;
            set;
        }
    }

}
