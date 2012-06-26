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
    /// Collision data is created during a collision and describes the state of the impact.
    /// </summary>
    public struct CollisionData
    {
        /// <summary>
        /// Constructor for collision data.
        /// </summary>
        /// <param name="sender">The shape responsible for the collision.</param>
        /// <param name="recipient">The recipient shape of the collision.</param>
        public CollisionData(Shape sender, Shape recipient) : this(sender, recipient, Vector2.Zero, 0) { }
        /// <summary>
        /// Constructor for collision data.
        /// </summary>
        /// <param name="sender">The shape responsible for the collision.</param>
        /// <param name="recipient">The recipient shape of the collision.</param>
        /// <param name="axis">The axis of the collision.</param>
        /// <param name="overlap">The overlap of the collision on the specified axis.</param>
        public CollisionData(Shape sender, Shape recipient, Vector2 axis, float overlap)
            : this()
        {
            // Set the variables.
            Sender = sender;
            Recipient = recipient;
            Axis = axis;
            Overlap = overlap;
            HasCollision = false;
        }

        /// <summary>
        /// The shape responsible of the collision.
        /// </summary>
        public Shape Sender
        {
            get;
            set;
        }
        /// <summary>
        /// The shape victimized by the collision.
        /// </summary>
        public Shape Recipient
        {
            get;
            set;
        }
        /// <summary>
        /// The axis of the collision.
        /// </summary>
        public Vector2 Axis
        {
            get;
            set;
        }
        /// <summary>
        /// The collision's overlap.
        /// </summary>
        public float Overlap
        {
            get;
            set;
        }
        /// <summary>
        /// Whether a collision has occurred.
        /// </summary>
        public bool HasCollision
        {
            get;
            set;
        }
    }

}
