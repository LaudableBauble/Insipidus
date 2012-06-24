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

using InsipidusEngine.Core;

namespace InsipidusEngine.Physics
{
    /// <summary>
    /// A body extends a shape to also include movement by altering its velocity, something the physics simulator does with its use of forces.
    /// </summary>
    public class Body
    {
        #region Fields
        private PhysicsSimulator _Physics;
        private Entity _Entity;
        private Shape _Shape;
        private float _FrictionCoefficient;
        private float _Mass;
        private float _AccelerationValue;
        private Vector3 _Velocity;
        private float _MaxVelocity;
        private bool _IsStatic;
        private bool _IsImmaterial;
        private HashSet<Body> _Collisions;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a body. Uses default values.
        /// </summary>
        public Body() : this(null) { }

        /// <summary>
        /// Constructor for a body. Uses default values.
        /// </summary>
        /// <param name="physics">The physics simulator that this body will be a part of.</param>
        public Body(PhysicsSimulator physics) : this(1, 1, 1, 10, .25f, physics) { }

        /// <summary>
        /// Constructor for a body.
        /// </summary>
        /// <param name="width">The width of the shape.</param>
        /// <param name="height">The height of the shape.</param>
        /// <param name="depth">The depth of the shape.</param>
        /// <param name="mass">The mass of the body.</param>
        /// <param name="friction">The friction of the body.</param>
        /// <param name="physics">The physics simulator that this body will be a part of.</param>
        public Body(float width, float height, float depth, float mass, float friction, PhysicsSimulator physics)
        {
            Initialize(width, height, depth, mass, friction, physics);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the body.
        /// </summary>
        /// <param name="width">The width of the shape.</param>
        /// <param name="height">The height of the shape.</param>
        /// <param name="depth">The depth of the shape.</param>
        /// <param name="mass">The mass of the body.</param>
        /// <param name="friction">The friction of the body.</param>
        /// <param name="physics">The physics simulator that this body will be a part of.</param>
        protected void Initialize(float width, float height, float depth, float mass, float friction, PhysicsSimulator physics)
        {
            // Initialize a couple of variables.
            _Shape = new Shape(width, height, depth);
            _IsStatic = false;
            _IsImmaterial = false;
            _MaxVelocity = 8;
            _Mass = mass;
            _Velocity = new Vector3(0, 0, 0);
            _FrictionCoefficient = friction;
            _AccelerationValue = 1;
            _Physics = physics;
            _Collisions = new HashSet<Body>();
        }
        /// <summary>
        /// Update the body.
        /// </summary>
        public void Update()
        {
            // Check if the body isn't static.
            if (!_IsStatic)
            {
                // Add the velocity to the position.
                _Shape.Position = Vector3.Add(_Shape.Position, _Velocity);
            }
            else
            {
                // Null the velocity.
                _Velocity = new Vector3(0, 0, 0);
            }
        }

        /// <summary>
        /// Add this body to its physics simulator, if it isn't already.
        /// </summary>
        public void AddBody()
        {
            // If the physics simulator is null, stop and print an error.
            if (_Physics == null)
            {
                Console.WriteLine(this + ": Error reaching PhysicsSimulator.");
                return;
            }

            // Try to add the body.
            try
            {
                // Check if the body already is added.
                if (!_Physics.Contains(this))
                {
                    _Physics.AddBody(this);
                }
            }
            // Catch the Exceptions that may arise.
            catch (Exception e)
            {
                Console.WriteLine(this + ": Error adding body. (" + e + ")");
            }
        }
        /// <summary>
        /// Add a force to the physics simulator.
        /// </summary>
        /// <param name="force">The force to add.</param>
        public void AddForce(Vector3 force)
        {
            // Check if the PhysicsSimultor isn't null.
            if (_Physics != null)
            {
                // Try to add the force.
                try { _Physics.AddForce(new Force(this, force)); }
                catch (Exception e) { Console.WriteLine(this + ": Error adding force. (" + e + ")"); }
            }
            // Print out the error.
            else { Console.WriteLine(this + ": Error with null physics simulator."); }
        }
        /// <summary>
        /// Add a body with which this body has collided with.
        /// </summary>
        /// <param name="body">The body with which a collisions has occurred.</param>
        public void AddCollision(Body body)
        {
            _Collisions.Add(body);
        }
        /// <summary>
        /// Clear all saved collisions. Usually done by the physics simulator each update.
        /// </summary>
        public void ClearCollisions()
        {
            _Collisions.Clear();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The physics simulator of this body.
        /// </summary>
        public PhysicsSimulator PhysicsSimulator
        {
            get { return _Physics; }
            set { _Physics = value; }
        }
        /// <summary>
        /// The shape of the body.
        /// </summary>
        public Shape Shape
        {
            get { return _Shape; }
            set { _Shape = value; }
        }
        /// <summary>
        /// The body's layered position, ie. only the x and y-coordinates.
        /// </summary>
        public Vector2 LayeredPosition
        {
            get { return _Shape.LayeredPosition; }
        }
        /// <summary>
        /// The position of the body.
        /// </summary>
        public Vector3 Position
        {
            get { return _Shape.Position; }
            set { _Shape.Position = value; }
        }
        /// <summary>
        /// The velocity of the body.
        /// </summary>
        public Vector3 Velocity
        {
            get { return _Velocity; }
            set { _Velocity = value; }
        }
        /// <summary>
        /// The maximum velocity of the body.
        /// </summary>
        public float MaxVelocity
        {
            get { return _MaxVelocity; }
            set { _Mass = value; }
        }
        /// <summary>
        /// The mass of the body.
        /// </summary>
        public float Mass
        {
            get { return _Mass; }
            set { _Mass = value; }
        }
        /// <summary>
        /// The friction coefficient of the body.
        /// </summary>
        public float FrictionCoefficient
        {
            get { return _FrictionCoefficient; }
            set { _FrictionCoefficient = value; }
        }
        /// <summary>
        /// The acceleration value of the body.
        /// </summary>
        public float AccelerationValue
        {
            get { return _AccelerationValue; }
            set { _AccelerationValue = value; }
        }
        /// <summary>
        /// Whether the body is static.
        /// </summary>
        public bool IsStatic
        {
            get { return _IsStatic; }
            set { _IsStatic = value; }
        }
        /// <summary>
        /// Whether the body is immaterial.
        /// </summary>
        public bool IsImmaterial
        {
            get { return _IsImmaterial; }
            set { _IsImmaterial = value; }
        }
        /// <summary>
        /// All of the body's collisions this update cycle.
        /// </summary>
        public HashSet<Body> Collisions
        {
            get { return _Collisions; }
        }
        /// <summary>
        /// The entity of this body.
        /// </summary>
        public Entity Entity
        {
            get { return _Entity; }
            set { _Entity = value; }
        }
        #endregion
    }
}
