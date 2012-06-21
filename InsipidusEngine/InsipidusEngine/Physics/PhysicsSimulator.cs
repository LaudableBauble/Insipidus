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
            // Check if the array isn't empty.
            try
            {
                // Clear all bodies' record of collision.
                foreach (Body b in _Bodies)
                {
                    b.clearCollisions();
                }

                // Loop through all bodies.
                foreach (Body b1 in _Bodies)
                {
                    // Ground collision?
                    bool ground = false;

                    // Loop through all bodies and check for collision.
                    foreach (Body b2 in _Bodies)
                    {
                        // Check so it's not the same body, or if both bodies are set to static.
                        if (b1 == b2 || (b1.getIsStatic() && b2.getIsStatic()))
                        {
                            continue;
                        }

                        // Check if the bodies are within range. If so, continue to the narrow phase part.
                        if (BroadPhase(b1, b2))
                        {
                            // Get the layered MTV by doing a narrow phase collision check.
                            Vector2 mtv = NarrowPhase(b1.getShape(), b2.getShape());

                            // Check for ground collision and alter bodies if necessary.
                            if (CheckGroundCollision(b1, b2, mtv))
                            {
                                // Add the collision to the body.
                                b1.addCollision(b2);
                                b2.addCollision(b1);

                                if (b1.getIsStatic() || b1.getIsImmaterial() || b2.getIsImmaterial())
                                {
                                    continue;
                                }

                                // Move body1 above body2 and null the movement on the z-axis (otherwise the body gets stuck).
                                b1.getShape().setBottomDepth(b2.getShape().getTopDepth(b1.getLayeredPosition()) + _Gravity / 2);
                                b1.getVelocity().setZ(0);
                                if (b2.getShape().getDepthDistribution() != DepthDistribution.Uniform)
                                {
                                    b1.setVelocity(Vector3.Zero);
                                }
                                ground = true;
                            }
                            else
                            {
                                // Ensure that the would-be collision occurred in allowed height space.
                                mtv = GetLayeredCollision(b1, b2, mtv);

                                // Check if the bodies intersect.
                                if (mtv != null)
                                {
                                    if (!b1.getIsImmaterial() && !b2.getIsImmaterial())
                                    {
                                        // Move the bodies so that they don't intersect each other anymore.
                                        ClearIntersection(b1, b2, mtv);
                                    }

                                    // Add the collision to the body.
                                    b1.addCollision(b2);
                                    b2.addCollision(b1);
                                }
                            }
                        }
                    }

                    // If the entity is dynamic and not standing on the ground, apply gravity.
                    if (!ground && !b1.getIsStatic() && !b1.getIsImmaterial())
                    {
                        b1.addGravity(_Gravity);
                    }

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
            catch (Exception e)
            {
                Console.WriteLine(this + ": Update Physics Error. (" + e + ")");
            }
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
                    body.setPhysicsSimulator(this);
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
                return Vector2.Distance(b1.getLayeredPosition(), b2.getLayeredPosition()) < (Math.Max(b1.getShape().getWidth(), b1.getShape().getHeight()) + Math.Max(b2
                        .getShape().getWidth(), b2.getShape().getHeight()));
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
        /// <returns>The MTV of the intersection or null if the collision was negative.</returns>
        public Vector2 NarrowPhase(Shape s1, Shape s2)
        {
            // The minimum amount of overlap. Start real high.
            float overlap = float.MaxValue;
            // The smallest axis found.
            Vector2 smallest = null;

            try
            {
                // Get the axes of both bodies.
                Vector2[][] axes = new Vector2[][] { s1.getAxes(), s2.getAxes() };

                // Iterate over the axes of both bodies.
                foreach (Vector2[] v in axes)
                {
                    // Iterate over both bodies' axes.
                    foreach (Vector2 a in v)
                    {
                        // Project both bodies onto the axis.
                        Vector2 p1 = s1.project(a);
                        Vector2 p2 = s2.project(a);

                        // Get the overlap.
                        double o = Vector2.getOverlap(p1, p2);

                        // Do the projections overlap?
                        if (o == -1)
                        {
                            // We can guarantee that the shapes do not overlap.
                            return null;
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

            // We now know that every axis had an overlap on it, which means we can
            // guarantee an intersection between the bodies.
            return Vector2.Multiply(smallest, overlap);
        }
        /// <summary>
        /// Check for collisions between two bodies at a certain range of z-coordinates (height).
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <param name="mtv">The MTV of the layered collision.</param>
        /// <returns>The MTV of the intersection or null if the collision was negative.</returns>
        private Vector2 GetLayeredCollision(Body b1, Body b2, Vector2 mtv)
        {
            // If there is no layered collision between the bodies, stop here.
            if (mtv == null) { return null; }

            // Get the dynamic and static body.
            Body a = b1.getIsStatic() ? b2 : b1;
            Body b = (a == b1) ? b2 : b1;

            // Get the min and max heights for both bodies.
            Vector2 h1 = new Vector2(a.getShape().getBottomDepth(), a.getShape().getTopDepth(a.getLayeredPosition()));
            Vector2 h2 = new Vector2(b.getShape().getBottomDepth(), b.getShape().getTopDepth(a.getLayeredPosition()));

            // Get min and max heights for possible collisions between the bodies.
            Vector2 heights = Vector2.getMiddleValues(h1, h2);

            // If there were no matching heights found, no collision possible.
            if (heights == null || (h2.Y - h1.X) < 3) { return null; }

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
        private bool CheckGroundCollision(Body b1, Body b2, Vector2 mtv)
        {
            // The first body has to be dynamic or if there is no layered collision between the bodies, stop here.
            if (mtv == null) { return false; }

            // Get the dynamic and static body.
            Body a = b1.getIsStatic() ? b2 : b1;
            Body b = (a == b1) ? b2 : b1;

            // Both bodies' depth positions.
            Vector2 h1 = new Vector2(a.getShape().getBottomDepth(), a.getShape().getTopDepth());
            Vector2 h2 = new Vector2(b.getShape().getBottomDepth(), b.getShape().getTopDepth(a.getLayeredPosition()));

            // The difference in height.
            double diff = h1.X - h2.Y;

            // If the distance between the bodies is either greater than the threshold or less than the velocity needed to collide, no collision.
            if (diff > Math.Max(-a.getVelocity().z + _Gravity, 0) || (h2.Y - h1.X) > 2) { return false; }

            // There must be a ground collision after all.
            return true;
        }

        /// <summary>
        /// Pull two bodies that are intersecting apart by using the MTV.
        /// </summary>
        /// <param name="b1">The first body to check.</param>
        /// <param name="b2">The second body to check.</param>
        /// <param name="mtv">The MTV of the collision.</param>
        private void ClearIntersection(Body b1, Body b2, Vector2 mtv)
        {
            // If the MTV is null, stop here.
            if (mtv == null) { return; }

            // Add the MTV to the first body and subtract it from the second. Only move dynamic bodies!
            if (!b1.getIsStatic())
            {
                b1.getShape().setLayeredPosition(Vector2.Add(b1.getShape().getLayeredPosition(), mtv));
                b1.setVelocity(Vector3.Zero);
            }
            if (!b2.getIsStatic())
            {
                b2.getShape().setLayeredPosition(Vector2.Subtract(b2.getShape().getLayeredPosition(), mtv));
                b2.setVelocity(Vector3.Zero);
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
            Vector2 energyB1 = Vector3.Absolute(Vector3.Multiply(b1.getVelocity(), b1.getMass())).toVector2();
            Vector2 energyB2 = Vector3.Absolute(Vector3.Multiply(b2.getVelocity(), b2.getMass())).toVector2();
            Vector2 energyBT = Vector2.Add(energyB1, energyB2);

            // Get the intersection rectangle.
            Rectangle intersection = b1.getShape().getIntersection(b2.getShape());
            // Calculate the Collision point.
            Vector2 collision = Helper.ToCentroid(intersection);

            // The mass ratio between the objects.
            float massRatio = b2.getMass() / b1.getMass();

            // The average kinetic energy. Multiply with something to lower the collision force and also with the mass ratio.
            Vector2 averageEnergy = Vector2.Multiply(Vector2.Multiply(Vector2.Divide(energyBT, 2), _EnergyDecrease), massRatio);

            // Multiply the Average kinetic Energy with the collision vector direction relative to the body's position.
            Vector2 impact = Vector2.Multiply(averageEnergy, Vector2.Direction(Vector2.Angle(collision, b1.getLayeredPosition())));

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
            Vector2 energyB1 = Vector3.Divide(Vector3.Multiply(Vector3.absolute(Vector3.Multiply(b1.getVelocity(), b1.getVelocity())), b1.getMass()), 2).toVector2();
            Vector2 energyB2 = Vector3.Divide(Vector3.Multiply(Vector3.absolute(Vector3.Multiply(b2.getVelocity(), b2.getVelocity())), b2.getMass()), 2).toVector2();

            // Get the intersection rectangle.
            Rectangle intersection = b1.getShape().getIntersection(b2.getShape());
            // Calculate the Collision point.
            Vector2 collision = Helper.toCentroid(intersection);

            // The average kinetic energy. Multiply with something to lower the
            // collision force.
            Vector2 averageEnergy = Vector2.Multiply(Vector2.Divide(Vector2.Add(energyB1, energyB2), 2), _EnergyDecrease);

            // Multiply the Average Kinetic Energy with the collision vector
            // direction relative to the body's position.
            Vector2 impact = Vector2.Multiply(averageEnergy, Vector2.getDirection(Vector2.getAngle(collision, b1.getLayeredPosition())));

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
                        f.Body.setVelocity(Vector3.Add(f.Body.Velocity, f.Velocity));
                    }
                }
                // Catch the exception.
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
            // return (new Force(b, Vector.multiply(Vector.getDirection(Vector.getAngle(b.position, b.velocity)), (b.frictionCoefficient * (b.mass * gravity)))));
            return new Force(b, Vector2.Multiply(Vector2.Inverse(Vector2.getDirection(b.getVelocity().toVector2(), Vector3.Length(Vector3.absolute(b.getVelocity())))),
                    (b.getFrictionCoefficient() * (b.getMass() * _Gravity))));
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
                Vector2 friction = Vector3.Add(f.Body.Velocity, f.Velocity).toVector2();
                // ////////////////////////////////////////////////////////
                // System.out.println(this + ": Old Velocity: (" + f.body.velocity.toString() + ")");
                // Clam the friction above or beneath zero and subtract it from the velocity.
                f.Body.Velocity(new Vector3(Vector2.Clamp(f.Body.Velocity.toVector2(), friction, 0), f.Body.Velocity.z));
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