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

using InsipidusEngine.Infrastructure;
using InsipidusEngine.Imagery;
using InsipidusEngine.Tools;

namespace InsipidusEngine.Physics
{

    /// <summary>
    /// A physics simulator tries to simulate the physics of bodies so that the game world behaves realistically.
    /// </summary>
    public class PhysicsSimulator
    {
        #region Fields
        public RobustList<Body> _Bodies;
        public RobustList<Force> _Forces;
        public float _Gravity;
        public float _EnergyDecrease;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a physics simulator.
        /// </summary>
        public PhysicsSimulator()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the physics simulator.
        /// </summary>
        private void Initialize()
        {
            // Initialize some variables.
            _Bodies = new RobustList<Body>();
            _Forces = new RobustList<Force>();
            _Gravity = 0.3f;
            _EnergyDecrease = 0.05f;
        }
        /// <summary>
        /// Update the physics simulator.
        /// </summary>
        public void Update()
        {
            //Update the list of bodies and forces.
            _Bodies.Update();
            _Forces.Update();

            // Check if the array isn't empty.
            try
            {
                // Clear all bodies' record of collision.
                foreach (Body b in _Bodies) { b.ClearCollisions(); }

                // Loop through all bodies.
                foreach (Body b1 in _Bodies)
                {
                    // Ground collision?
                    bool ground = false;

                    // Loop through all bodies and check for collision.
                    foreach (Body b2 in _Bodies)
                    {
                        // Check so it's not the same body, or if both bodies are set to static.
                        if (b1 == b2 || (b1.IsStatic && b2.IsStatic)) { continue; }

                        // Check if the bodies are within range. If so, continue to the narrow phase part.
                        if (BroadPhase(b1, b2))
                        {
                            // Get the layered MTV by doing a narrow phase collision check.
                            CollisionData mtv = NarrowPhase(b1.Shape, b2.Shape);

                            // Check for ground collision and alter bodies if necessary.
                            if (CheckGroundCollision(b1, b2, mtv))
                            {
                                // Add the collision to the body.
                                b1.AddCollision(b2);
                                b2.AddCollision(b1);

                                if (b1.IsStatic || b1.IsImmaterial || b2.IsImmaterial) { continue; }

                                // Move body1 above body2 and null the movement on the z-axis (otherwise the body gets stuck).
                                b1.Shape.BottomDepth = b2.Shape.GetTopDepth(b1.LayeredPosition) + _Gravity / 2;
                                b1.Velocity = new Vector3(b1.Velocity.X, b1.Velocity.Y, 0);
                                if (b2.Shape.DepthDistribution != DepthDistribution.Uniform) { b1.Velocity = Vector3.Zero; }
                                ground = true;
                            }
                            else
                            {
                                // Ensure that the would-be collision occurred in allowed height space.
                                mtv = GetLayeredCollision(b1, b2, mtv);

                                // Check if the bodies intersect.
                                if (mtv.HasCollision)
                                {
                                    // Move the bodies so that they don't intersect each other anymore.
                                    if (!b1.IsImmaterial && !b2.IsImmaterial) { ClearIntersection(b1, b2, mtv); }

                                    // Add the collision to the body.
                                    b1.AddCollision(b2);
                                    b2.AddCollision(b1);
                                }
                            }
                        }
                    }

                    // If the entity is dynamic and not standing on the ground, apply gravity.
                    if (!ground && !b1.IsStatic && !b1.IsImmaterial) { b1.Velocity = new Vector3(b1.Velocity.X, b1.Velocity.Y, b1.Velocity.Z - _Gravity); }

                    // Add the friction.
                    AddFrictionForce(GetBodyFriction(b1));
                    // Add all forces to the body.
                    AddForcesToBody(GetForces(b1));
                    // Update the body.
                    b1.Update();
                }

                // Clear the all forces.
                _Forces.Clear();
            }
            // Catch the exception.
            catch (Exception e) { Console.WriteLine(this + ": Update Physics Error. (" + e + ")"); }
        }

        /// <summary>
        /// Do a broad phase collision check. (Currently checks if the bodies might intersect on the XY-axis.
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <returns>Whether two bodies are close enough for a collision.</returns>
        private bool BroadPhase(Body b1, Body b2)
        {
            // Try this.
            try
            {
                // Check if the bodies are within range.
                return Vector2.Distance(b1.LayeredPosition, b2.LayeredPosition) < (Math.Max(b1.Shape.Width, b1.Shape.Height) + Math.Max(b2
                        .Shape.Width, b2.Shape.Height));
            }
            // Catch the exception and display relevant information.
            catch (Exception e)
            {
                Console.WriteLine(this + ": Broad Phase Error. (" + e + ") - Body1: " + b1 + " Body2: " + b2);
            }

            // Something went wrong.
            return false;
        }
        /// <summary>
        /// Do a narrow phase collision check between two shapes by using SAT (Separating Axis Theorem).
        /// If a collision has occurred, get the MTV (Minimum Translation Vector) of the two intersecting shapes.
        /// </summary>
        /// <param name="s1">The first shape to check.</param>
        /// <param name="s2">The second shape to check.</param>
        /// <returns>The MTV of the intersection.</returns>
        public CollisionData NarrowPhase(Shape s1, Shape s2)
        {
            // The minimum amount of overlap. Start real high.
            float overlap = float.MaxValue;
            //The collision data.
            CollisionData data = new CollisionData(s1, s2);
            // The smallest axis found.
            Vector2 smallest = Vector2.Zero;

            try
            {
                // Get the axes of both bodies.
                Vector2[][] axes = new Vector2[][] { s1.GetAxes(), s2.GetAxes() };

                // Iterate over the axes of both bodies.
                foreach (Vector2[] v in axes)
                {
                    // Iterate over both bodies' axes.
                    foreach (Vector2 a in v)
                    {
                        // Project both bodies onto the axis.
                        Vector2 p1 = s1.Project(a);
                        Vector2 p2 = s2.Project(a);

                        // Get the overlap.
                        float o = Calculator.GetOverlap(p1, p2);

                        // Do the projections overlap?
                        if (o == -1)
                        {
                            // We can guarantee that the shapes do not overlap.
                            return data;
                        }
                        else
                        {
                            // Check for minimum.
                            if (o < overlap)
                            {
                                // Store the minimum overlap and the axis it was projected upon. Make sure that the separation vector is pointing the right way.
                                overlap = o;
                                smallest = a;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(this + ": Narrow Phase Error. (" + e + ")");
            }

            // We now know that every axis had an overlap on it, which means we can guarantee an intersection between the bodies.
            data.HasCollision = true;
            data.Axis = smallest;
            data.Overlap = overlap;

            //Return the collision data.
            return data;
        }
        /// <summary>
        /// Check for collisions between two bodies at a certain range of z-coordinates (height).
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <param name="mtv">The MTV of the layered collision.</param>
        /// <returns>The MTV of the intersection.</returns>
        private CollisionData GetLayeredCollision(Body b1, Body b2, CollisionData mtv)
        {
            // If there is no layered collision between the bodies, stop here.
            if (!mtv.HasCollision) { return mtv; }

            // Get the dynamic and static body.
            Body a = b1.IsStatic ? b2 : b1;
            Body b = (a == b1) ? b2 : b1;

            // Get the min and max heights for both bodies.
            Vector2 h1 = new Vector2(a.Shape.BottomDepth, a.Shape.GetTopDepth(a.LayeredPosition));
            Vector2 h2 = new Vector2(b.Shape.BottomDepth, b.Shape.GetTopDepth(a.LayeredPosition));

            // Get min and max heights for possible collisions between the bodies.
            Vector2 heights = Calculator.GetMiddleValues(h1, h2);

            // If there were no matching heights found, no collision possible.
            if ((heights.X < 0 && heights.Y > 0) || (h2.Y - h1.X) < 3) { return mtv; }

            //A collision has been found.
            mtv.HasCollision = true;

            // Return the MTV.
            return mtv;
        }
        /// <summary>
        /// Check for collision between two bodies where one is coming from above the other.
        /// This is a preemptive step due to the use of body velocity to project future positions.
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <param name="mtv">The MTV of the collision.</param>
        /// <returns>Whether there was a ground collision or not, from the first body's perspective</returns>
        private bool CheckGroundCollision(Body b1, Body b2, CollisionData mtv)
        {
            // The first body has to be dynamic or if there is no layered collision between the bodies, stop here.
            if (!mtv.HasCollision) { return false; }

            // Get the dynamic and static body.
            Body a = b1.IsStatic ? b2 : b1;
            Body b = (a == b1) ? b2 : b1;

            // Both bodies' depth positions.
            Vector2 h1 = new Vector2(a.Shape.BottomDepth, a.Shape.TopDepth);
            Vector2 h2 = new Vector2(b.Shape.BottomDepth, b.Shape.GetTopDepth(a.LayeredPosition));

            // The difference in height.
            double diff = h1.X - h2.Y;

            // If the distance between the bodies is either greater than the threshold or less than the velocity needed to collide, no collision.
            if (diff > Math.Max(-a.Velocity.Z + _Gravity, 0) || (h2.Y - h1.X) > 2) { return false; }

            // There must be a ground collision after all.
            return true;
        }
        /// <summary>
        /// Pull two bodies that are intersecting apart by using the MTV.
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <param name="mtv">The MTV of the collision.</param>
        private void ClearIntersection(Body b1, Body b2, CollisionData mtv)
        {
            // If the MTV is null, stop here.
            if (!mtv.HasCollision) { return; }

            // Add the MTV to the first body and subtract it from the second. Only move dynamic bodies!
            if (!b1.IsStatic)
            {
                b1.Shape.LayeredPosition = b1.Shape.LayeredPosition + mtv.Axis * mtv.Overlap;
                b1.Velocity = Vector3.Zero;
            }
            if (!b2.IsStatic)
            {
                b2.Shape.LayeredPosition = b2.Shape.LayeredPosition - mtv.Axis * mtv.Overlap;
                b2.Velocity = Vector3.Zero;
            }
        }
        /// <summary>
        /// Calculate the impact force vector.
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <returns>The impact force vector.</returns>
        private Force ImpactForce(Body b1, Body b2)
        {
            // Calculate the Energy of the two bodies before impact.
            Vector3 energyB1 = Calculator.Absolute(b1.Velocity * b1.Mass);
            Vector3 energyB2 = Calculator.Absolute(b2.Velocity * b2.Mass);
            Vector3 energyBT = energyB1 + energyB2;

            // Get the intersection rectangle.
            Rectangle intersection = b1.Shape.GetIntersection(b2.Shape);
            // Calculate the Collision point.
            Vector2 collision = Calculator.ToCentroid(intersection);

            // The mass ratio between the objects.
            float massRatio = b2.Mass / b1.Mass;

            // The average kinetic energy. Multiply with something to lower the collision force and also with the mass ratio.
            Vector3 averageEnergy = (energyBT / 2) * _EnergyDecrease * massRatio;

            // Multiply the Average kinetic Energy with the collision vector direction relative to the body's position.
            Vector3 impact = averageEnergy * new Vector3(Calculator.Direction(Calculator.GetAngle(collision, b1.LayeredPosition)), 0);

            // Return the average vector.
            return new Force(b1, impact);
        }
        /// <summary>
        /// Calculate the impact force by using the kinetic energy.
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <returns>The impact force between the two bodies.</returns>
        private Force ImpactForceEnergy(Body b1, Body b2)
        {
            // Calculate the Kinetic Energy of the two bodies.
            Vector3 energyB1 = Calculator.Absolute(b1.Velocity * b1.Velocity) * b1.Mass / 2;
            Vector3 energyB2 = Calculator.Absolute(b2.Velocity * b2.Velocity) * b2.Mass / 2;

            // Get the intersection rectangle.
            Rectangle intersection = b1.Shape.GetIntersection(b2.Shape);
            // Calculate the Collision point.
            Vector2 collision = Calculator.ToCentroid(intersection);

            // The average kinetic energy. Multiply with something to lower the
            // collision force.
            Vector3 averageEnergy = ((energyB1 + energyB2) / 2) * _EnergyDecrease;

            // Multiply the Average Kinetic Energy with the collision vector
            // direction relative to the body's position.
            Vector3 impact = averageEnergy * new Vector3(Calculator.Direction(Calculator.GetAngle(collision, b1.LayeredPosition)), 0);

            // Return the average vector.
            return new Force(b1, impact);
        }
        /// <summary>
        /// Get the forces connected to a certain body.
        /// </summary>
        /// <param name="body">The body used to find connected forces.</param>
        /// <returns>The connected forces.</returns>
        public List<Force> GetForces(Body body)
        {
            // Create the return variable.
            List<Force> force = new List<Force>();

            // Try this.
            try
            {
                // Loop through the forces array.
                foreach (Force f in _Forces)
                {
                    // Try to match the bodies in the Force.
                    if (f.Body == body) { force.Add(f); }
                }
            }
            // Catch the exception.
            catch (Exception e) { Console.WriteLine(this + ": Force Exists Error. (" + e + ")"); }

            // Return all forces belonging to a body.
            return force;
        }
        /// <summary>
        /// Add a body to the physics simulator.
        /// </summary>
        /// <param name="body">The body to add.</param>
        public void AddBody(Body body)
        {
            // Try to add the body at the end of the array.
            try
            {
                // If the body isn't already in the folds of the physics simulator.
                if (!_Bodies.Contains(body))
                {
                    _Bodies.Add(body);
                    body.PhysicsSimulator = this;
                }
            }
            // Catch the exception and display relevant information.
            catch (Exception e)
            {
                Console.WriteLine(this + ": Error adding body. (" + e + ")");
            }
        }
        /// <summary>
        /// Remove a body from the physics simulator.
        /// </summary>
        /// <param name="body">The body to remove.</param>
        public void RemoveBody(Body body)
        {
            _Bodies.Remove(body);
        }
        /// <summary>
        /// Add a force to the physics simulator.
        /// </summary>
        /// <param name="force">The force to add.</param>
        public void AddForce(Force force)
        {
            // Try to add the Force at the end of the list.
            try
            {
                _Forces.Add(force);
            }
            // Catch the exception and display relevant information.
            catch (Exception e)
            {
                Console.WriteLine(this + ": Error adding force. (" + e + ")");
            }
        }
        /// <summary>
        /// Check if the physics simulator contains a body.
        /// </summary>
        /// <param name="body">The body to look for.</param>
        /// <returns>Whether the body do indeed exist in the physics simulator.</returns>
        public bool Contains(Body body)
        {
            // Create the return variable.
            bool exists = false;

            // Try this.
            try
            {
                // Loop through the body array.
                foreach (Body b in _Bodies)
                {
                    // Try to match the bodies.
                    if (body == b) { exists = true; }
                }
            }
            // Catch the exception.
            catch (Exception e) { Console.WriteLine(this + ": Body Exists Error. (" + e + ")"); }

            // Return the result.
            return exists;
        }
        /// <summary>
        /// Add forces to their respective bodies.
        /// </summary>
        /// <param name="bodyForces">The list of forces to add.</param>
        private void AddForcesToBody(List<Force> bodyForces)
        {
            // Check if the list isn't empty.
            if (bodyForces.Count != 0)
            {
                // Try this.
                try
                {
                    // Loop through the force list.
                    foreach (Force f in bodyForces)
                    {
                        // Add the Force to the body.
                        f.Body.Velocity = Vector3.Add(f.Body.Velocity, f.Velocity);
                    }
                }
                catch (Exception e) { Console.WriteLine(this + ": Adding Force to Body Error. (" + e + ")"); }
            }
        }
        /// <summary>
        /// Calculate the friction force and its direction for a body.
        /// </summary>
        /// <param name="b">The body to calculate the friction for.</param>
        /// <returns>The friction.</returns>
        private Force GetBodyFriction(Body b)
        {
            // First, multiply the friction coefficient with the gravity's force exertion on the body (mass * gravity value),
            // then with the direction of the velocity. Inverse the vector and finally return the result.
            // return (new Force(b, Vector.multiply(Vector.getDirection(Vector.getAngle(b.position, b.velocity)), b.frictionCoefficient * (b.mass * gravity))));
            return new Force(b, Vector3.Negate(Calculator.Direction(b.Velocity, Vector3.Distance(Calculator.Absolute(b.Velocity), Vector3.Zero))) *
                    (b.FrictionCoefficient * b.Mass * _Gravity));
        }
        /// <summary>
        /// Add a friction force to a body.
        /// </summary>
        /// <param name="f">The friction force to add.</param>
        private void AddFrictionForce(Force f)
        {
            // Try this.
            try
            {
                // ////////////////////////////////////////////////////////
                // System.out.println(this + ": The Friction: (" + f.getForce().toString() + ")");
                // Calculate the Friction.
                Vector3 friction = f.Body.Velocity + f.Velocity;
                // ////////////////////////////////////////////////////////
                // System.out.println(this + ": Old Velocity: (" + f.body.velocity.toString() + ")");
                // Clam the friction above or beneath zero and subtract it from the velocity.
                Vector3 v = Vector3.Clamp(f.Body.Velocity, Vector3.Zero, friction);
                v.Z = f.Body.Velocity.Z;
                f.Body.Velocity = v;
                // ////////////////////////////////////////////////////////
                // System.out.println(this + ": Velocity with applied Friction: (" + friction.toString() + ")");
                // System.out.println(this + ": ----------------------------------------------------------------------------");
            }
            // Catch the exception.
            catch (Exception e) { Console.WriteLine(this + ": Adding Friction Force to Body Error. (" + e + ")"); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The number of bodies.
        /// </summary>
        public int BodyCount
        {
            get
            {
                // Create the return variable.
                int result = 0;

                try { result = _Bodies.Count; }
                // Catch the exception and display relevant information.
                catch (Exception e) { Console.WriteLine(this + ": Body Count Error. (" + e + ")"); }

                // Return the result.
                return result;
            }
        }
        /// <summary>
        /// The number of forces in this iteration.
        /// </summary>
        public int ForceCount
        {
            get
            {
                // Create the return variable.
                int result = 0;

                try { result = _Forces.Count; }
                // Catch the exception and display relevant information.
                catch (Exception e) { Console.WriteLine(this + ": Force Count Error. (" + e + ")"); }

                // Return the result.
                return result;
            }
        }
        /// <summary>
        /// The list of all bodies.
        /// </summary>
        public List<Body> getBodies
        {
            get { return _Bodies.ToList(); }
        }
        /// <summary>
        /// The gravity of the physics simulator.
        /// </summary>
        public float Gravity
        {
            get { return _Gravity; }
            set { _Gravity = value; }
        }
        #endregion
    }
}