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
        private double _FrictionCoefficient;
        private double _Mass;
        private double _AccelerationValue;
        private Vector3 _Velocity;
        private double _MaxVelocity;
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
        public Body(float width, float height, float depth, double mass, double friction, PhysicsSimulator physics)
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
        protected void Initialize(float width, float height, float depth, double mass, double friction, PhysicsSimulator physics)
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
		_Collisions = new HashSet<>();
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
        /**
         * Add a force to the physics simulator.
         * 
         * @param force
         *            The force to add.
         */
        public void AddForce(Vector3 force)
        {
            // Check if the PhysicsSimultor isn't null.
            if (_Physics != null)
            {
                // Try to add the force.
                try
                {
                    _Physics.addForce(new Force(this, force));
                }
                // Catch the Exceptions that may arise.
                catch (Exception e)
                {
                    Console.WriteLine(this + ": Error adding force. (" + e + ")");
                }
            }
            // Print out the error.
            else
            {
                Console.WriteLine(this + ": Error with null physics simulator.");
            }
        }

        /**
         * Add a body with which this body has collided with.
         * 
         * @param body
         *            The other body in the collision.
         */
        public void addCollision(Body body)
        {
            _Collisions.Add(body);
        }

        /**
         * Get all of the body's collisions this update cycle.
         * 
         * @return All collisions this update cycle.
         */
        public HashSet<Body> getCollisions()
        {
            return _Collisions;
        }

        /**
         * Clear all saved collisions. Usually done by the physics simulator each update.
         */
        public void clearCollisions()
        {
            _Collisions.Clear();
        }
        #endregion

        #region Properties
        /**
         * Get the body's physics simulator.
         * 
         * @return The physics simulator.
         */
        public PhysicsSimulator getPhysicsSimulator()
        {
            return _Physics;
        }

        /**
         * Set the body's physics simulator.
         * 
         * @param physics
         *            The new physics simulator.
         */
        public void setPhysicsSimulator(PhysicsSimulator physics)
        {
            _Physics = physics;
        }

        /**
         * Get the body's shape.
         * 
         * @return The shape of the body.
         */
        public Shape getShape()
        {
            return _Shape;
        }

        /**
         * Get the body's layered position, ie. only the x and y-coordinates.
         * 
         * @return The position of the body.
         */
        public Vector2 getLayeredPosition()
        {
            return _Shape.LayeredPosition;
        }

        /**
         * Get the body's position, ie. all three dimensions.
         * 
         * @return The position of the body.
         */
        public Vector3 getPosition()
        {
            return _Shape.Position;
        }

        /**
         * Get the body's velocity.
         * 
         * @return The velocity of the body.
         */
        public Vector3 getVelocity()
        {
            return _Velocity;
        }

        /**
         * Get the body's maximum velocity.
         * 
         * @return The maximum velocity of the body.
         */
        public double getMaxVelocity()
        {
            return _MaxVelocity;
        }

        /**
         * Get the body's mass.
         * 
         * @return The mass of the body.
         */
        public double getMass()
        {
            return _Mass;
        }
        /**
         * Get the body's friction coefficient.
         * 
         * @return The friction coefficient of the body.
         */
        public double getFrictionCoefficient()
        {
            return _FrictionCoefficient;
        }

        /**
         * Get the body's acceleration value.
         * 
         * @return The acceleration value of the body.
         */
        public double getAccelerationValue()
        {
            return _AccelerationValue;
        }

        /**
         * Add gravity (velocity on the z-axis) to the body's velocity.
         * 
         * @param gravity
         *            The gravity to apply.
         */
        public void addGravity(float gravity)
        {
            _Velocity.Z = _Velocity.Z - gravity;
        }

        /**
         * Get the body's staticness.
         * 
         * @return Whether the body is static or not.
         */
        public bool getIsStatic()
        {
            return _IsStatic;
        }

        /**
         * Get whether the body is immaterial.
         * 
         * @return Whether the body is immaterial or not.
         */
        public bool getIsImmaterial()
        {
            return _IsImmaterial;
        }

        /**
         * Get the entity of this body.
         * 
         * @return The entity.
         */
        public Entity getEntity()
        {
            return _Entity;
        }
        #endregion
    }
}
