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

using InsipidusEngine.Helpers;

namespace InsipidusEngine.Physics
{
    /// <summary>
    /// A shape is a geometrical form used primarily for collision detection. When used by a body it can interact with the world around it,
    /// otherwise it is nothing more than a ghost in the eyes of the engine.
    /// Currently only supports rectangular boxes and triangles with rectangular bases.
    /// </summary>
    public class Shape
    {
        #region Fields
        private float _Width;
        private float _Height;
        private float _Depth;
        private Vector3 _Position;
        private float _Rotation;
        private Vector2 _Origin;
        private DepthDistribution _DepthDistribution;
        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor for a shape.
        /// </summary>
        public Shape() : this(1, 1, 1) { }
        /// <summary>
        /// Constructor for a shape.
        /// </summary>
        /// <param name="width">The width of the shape.</param>
        /// <param name="height">The height of the shape.</param>
        /// <param name="depth">The depth of the shape.</param>
        public Shape(float width, float height, float depth) : this(Vector3.Zero, width, height, depth) { }
        /// <summary>
        /// Constructor for a shape.
        /// </summary>
        /// <param name="position">The position of the shape.</param>
        /// <param name="width">The width of the shape.</param>
        /// <param name="height">The height of the shape.</param>
        /// <param name="depth">The depth of the shape.</param>
        public Shape(Vector3 position, float width, float height, float depth)
        {
            Initialize(position, width, height, depth);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the shape.
        /// </summary>
        /// <param name="position">The position of the shape.</param>
        /// <param name="width">The width of the shape.</param>
        /// <param name="height">The height of the shape.</param>
        /// <param name="depth">The depth of the shape.</param>
        protected void Initialize(Vector3 position, float width, float height, float depth)
        {
            // Initialize some variables.
            _Position = position;
            _Rotation = 0;
            _Width = width;
            _Height = height;
            _Depth = depth;
            _DepthDistribution = DepthDistribution.Uniform;

            // Update the origin.
            _Origin = Center;
        }

        /// <summary>
        /// Check if this shape is intersecting another.
        /// </summary>
        /// <param name="shape">The other shape to check collisions with.</param>
        /// <returns>Whether the two shapes intersect each other.</returns>
        public bool Intersects(Shape shape)
        {
            // Create the two base rectangles.
            Rectangle rect1 = new Rectangle((int)(shape.Position.X - shape.Width / 2), (int)(shape.Position.Y - shape.Height / 2),
                (int)shape.Width, (int)shape.Height);
            Rectangle rect2 = new Rectangle((int)_Position.X, (int)_Position.Y, (int)_Width, (int)_Height);

            // If they intersect, continue.
            if (rect1.Intersects(rect2)) { return true; }

            // No intersection at this point.
            return false;
        }
        /// <summary>
        /// Calculates an intersection rectangle from between the given shape and this instance. If no intersection exists null is returned.
        /// </summary>
        /// <param name="shape">The shape to get an intersection from.</param>
        /// <returns>The intersection rectangle.</returns>
        public Rectangle GetIntersection(Shape shape)
        {
            // Create the two base rectangles.
            Rectangle rect1 = new Rectangle((int)shape.Position.X, (int)shape.Position.Y, (int)shape.Width, (int)shape.Height);
            Rectangle rect2 = new Rectangle((int)_Position.X, (int)_Position.Y, (int)_Width, (int)_Height);

            // Return the intersection rectangle.
            //return rect1.Intersection(rect2);
            return rect1;
        }
        /// <summary>
        /// Compare two shapes by their depth values. Allows margin overlap, ie. same end-point position.
        /// </summary>
        /// <param name="s1">The first shape.</param>
        /// <param name="s2">The second shape.</param>
        /// <returns>1 if the first shape is located 'higher', -1 if the second shape is 'located' higher and 0 if the shapes overlap.</returns>
        public static int IsOverlapping(Shape s1, Shape s2)
        {
            // The entities' depth data.
            Vector2 v1 = new Vector2(s1.BottomDepth, Math.Min(s1.GetTopDepth(s1.LayeredPosition), s1.GetTopDepth(s2.LayeredPosition)));
            Vector2 v2 = new Vector2(s2.BottomDepth, Math.Min(s2.GetTopDepth(s1.LayeredPosition), s2.GetTopDepth(s2.LayeredPosition)));

            // Compare the shapes to each other.
            if (!Calculator.IsOverlapping(v1, v2, false))
            {
                if (s1.Position.Z < s2.Position.Z) { return -1; }
                else if (s1.Position.Z > s2.Position.Z) { return 1; }
            }

            // The shapes' overlap, return 0.
            return 0;
        }
        /// <summary>
        /// Get the axes of this layered shape, ie. the normals of each edge, given a certain depth. Uses clockwise ordering.
        /// </summary>
        /// <param name="z">The depth of the shape.</param>
        /// <returns>An array of axes.</returns>
        public Vector2[] GetAxes(float z)
        {
            return GetLayeredShape(z).GetAxes();
        }
        /// <summary>
        /// Get the axes of this shape, ie. the normals of each edge. Uses clockwise ordering.
        /// </summary>
        /// <returns>An array of axes.</returns>
        public Vector2[] GetAxes()
        {
            // Note that because of parallel edges in a rectangle only two edges have to be returned.
            return new Vector2[] { Vector2.Normalize(Calculator.Perpendicular(TopRight - TopLeft)), Vector2.Normalize(Calculator.Perpendicular(BottomRight - TopRight)) };
        }
        /// <summary>
        /// Get the vertices of this shape. Uses clockwise ordering.
        /// </summary>
        /// <returns>An array of vertices.</returns>
        public Vector2[] GetVertices()
        {
            return new Vector2[] { TopLeft, TopRight, BottomRight, BottomLeft };
        }
        /// <summary>
        /// Project the shape onto the axis and return the end points of the resulting line.
        /// NOTE: The axis must be normalized to get accurate projections.
        /// </summary>
        /// <param name="axis">The axis to project upon.</param>
        /// <returns>A line in one-dimensional space.</returns>
        public Vector2 Project(Vector2 axis)
        {
            // Set up the end points of the projection.
            float min = Vector2.Dot(axis, TopLeft);
            float max = min;

            // Iterate through every vertex in the shape.
            foreach (Vector2 v in GetVertices())
            {
                // Get the dot product.
                float p = Vector2.Dot(axis, v);

                // See if any new end points have emerged.
                if (p < min) { min = p; }
                else if (p > max) { max = p; }
            }

            // Return the projection.
            return new Vector2(min, max);
        }
        /// <summary>
        /// Get a layer from this shape given a z-coordinate.
        /// </summary>
        /// <param name="z">The z-coordinate to get a layered shape from.</param>
        /// <returns>The layered shape.</returns>
        public Shape GetLayeredShape(float z)
        {
            // The depth.
            float depth = z - BottomDepth;

            // If the depth provided does not stay within the shape's bounds, stop here.
            if (depth < 0 || depth > TopDepth) { throw new ArgumentException(); }

            // Check the depth distribution.
            switch (_DepthDistribution)
            {
                case DepthDistribution.Top:
                    {
                        // Get the ratio between height and depth.
                        float ratio = _Height / _Depth;

                        // Get the amount of height to remove and calculate the new position.
                        float height = depth * ratio;
                        float y = _Position.Y - (_Height / 2) + height + ((_Height - height) / 2);

                        // Return the layered shape.
                        return new Shape(new Vector3(_Position.X, y, z), _Width, _Height - (float)height, 1f);
                    }
                case DepthDistribution.Bottom:
                    {
                        // Get the ratio between height and depth.
                        float ratio = _Height / _Depth;

                        // Get the amount of height to remove and calculate the new position.
                        float height = depth * ratio;
                        float y = _Position.Y + (_Height / 2) - height - ((_Height - height) / 2);

                        // Return the layered shape.
                        return new Shape(new Vector3(_Position.X, y, z), _Width, _Height - (float)height, 1f);
                    }
                case DepthDistribution.Right:
                    {
                        // Get the ratio between width and depth.
                        float ratio = _Width / _Depth;

                        // Get the amount of width to remove and calculate the new position.
                        float width = depth * ratio;
                        float x = _Position.X - (_Width / 2) + width + ((_Width - width) / 2);

                        // Return the layered shape.
                        return new Shape(new Vector3(x, _Position.Y, z), _Width - (float)width, _Height, 1f);
                    }
                case DepthDistribution.Left:
                    {
                        // Get the ratio between width and depth.
                        float ratio = _Width / _Depth;

                        // Get the amount of width to remove and calculate the new position.
                        float width = depth * ratio;
                        float x = _Position.X + (_Width / 2) - width - ((_Width - width) / 2);

                        // Return the layered shape.
                        return new Shape(new Vector3(x, _Position.Y, z), _Width - (float)width, _Height, 1f);
                    }
                default:
                    {
                        // Uniform depth distribution.
                        return new Shape(new Vector3(_Position.X, _Position.Y, z), _Width, _Height, 1f);
                    }
            }
        }
        /// <summary>
        /// Get the position (z - depth / 2 + actual depth) of the shape's top-edge, not acknowledging rotation, at a given layered position.
        /// </summary>
        /// <param name="layeredPosition">The layered position to find the depth for.</param>
        /// <returns>The position (depth) of the shape's top-edge. Either the top depth or bottom depth if the shape does not occupy the space at the given position, depending on direction.</returns>
        public float GetTopDepth(Vector2 layeredPosition)
        {
            // The depth to add.
            float depth;
            // The amount of the top step. Used to enable characters to reach the top of the slope without colliding with adjacent bodies.
            float amount = 5;

            // Check the depth distribution. Shorten the sloped shape to fix the issue with characters not reaching the top height of the slope due to collisions with adjacent bodies.
            switch (_DepthDistribution)
            {
                case DepthDistribution.Top:
                    {
                        depth = ((_Position.Y + (_Height / 2)) - layeredPosition.Y) * (_Depth / (_Height - amount));
                        break;
                    }
                case DepthDistribution.Bottom:
                    {
                        depth = (layeredPosition.Y - (_Position.Y - (_Height / 2))) * (_Depth / (_Height - amount));
                        break;
                    }
                case DepthDistribution.Right:
                    {
                        depth = (layeredPosition.X - (_Position.X - (_Width / 2))) * (_Depth / (_Width - amount));
                        break;
                    }
                case DepthDistribution.Left:
                    {
                        depth = ((_Position.X + (_Width / 2)) - layeredPosition.X) * (_Depth / (_Width - amount));
                        break;
                    }
                default:
                    {
                        // Calculate the depth.
                        depth = _Depth;
                        break;
                    }
            }

            //Return the top depth.
            return (_Position.Z - (_Depth / 2) + Math.Min(Math.Max(depth, 0), _Depth));
        }
        /// <summary>
        /// Get a depth sorting value for this shape at the given local x and y-coordinates, ie. as seen from its image.
        /// </summary>
        /// <param name="x">The local x-coordinate.</param>
        /// <param name="y">The local y-coordinate.</param>
        /// <returns>The depth sorting value for this shape.</returns>
        public float getDepthSort(float x, float y)
        {
            // Remember that the top-left corner is (0, 0) in an image, but not in the shape.

            // The coordinates in world space.
            float dy = _Position.Y;
            float dz = _Position.Z;

            // The depth at the current position, local to the shape.
            float depth = GetTopDepth(new Vector2(_Position.X - _Width / 2 + x, _Position.Y - _Height / 2 + y)) - (_Position.Z - _Depth / 2);

            // If the coordinates is within bounds on the x-axis.
            if (x >= 0 && x <= _Width)
            {
                // If the coordinates match the front 'face' of the shape.
                if (y >= _Depth + _Height - depth && y <= _Depth + _Height)
                {
                    dy = _Position.Y + _Height / 2;
                    dz = _Position.Z - _Depth / 2 + _Depth + _Height - y;
                }
                // If the coordinates match the top 'face' of the shape.
                else if (y >= _Depth - depth && y <= _Depth - depth + _Height)
                {
                    dy = _Position.Y - _Height / 2 + y - (_Depth - depth);
                    dz = BottomDepth + depth;
                }
            }

            // Return the depth sorting value.
            return dy + dz;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The width of the shape.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { _Width = value; _Origin = Center; }
        }
        /// <summary>
        /// The height of the shape.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { _Height = value; _Origin = Center; }
        }
        /// <summary>
        /// The depth of the shape.
        /// </summary>
        public float Depth
        {
            get { return _Depth; }
            set { _Depth = value; }
        }
        /// <summary>
        /// The position of the shape.
        /// </summary>
        public Vector3 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        /// <summary>
        /// The shape's layered 2D-position, ie. only the x and y-coordinate.
        /// </summary>
        public Vector2 LayeredPosition
        {
            get { return new Vector2(_Position.X, _Position.Y); }
            set { _Position = new Vector3(value.X, value.Y, _Position.Z); }
        }
        /// <summary>
        /// The bottom position of the shape, ie. the origin of the shape's bottom layer.
        /// </summary>
        public Vector3 BottomPosition
        {
            get { return new Vector3(_Position.X, _Position.Y, _Position.Z - Depth + 2); }
            set { _Position = new Vector3(value.X, value.Y, value.Z + Depth / 2); }
        }
        /// <summary>
        /// The minimum position of the shape. Only works if the shape is not rotated. Used to map out a depth texture correctly.
        /// </summary>
        public Vector3 MinPosition
        {
            get { return new Vector3(_Position.X - _Width / 2, _Position.Y - _Height / 2, _Position.Z - _Depth / 2); }
        }
        /// <summary>
        /// The center of the shape.
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(_Width / 2, _Height / 2); }
        }
        /// <summary>
        /// The position of the top-left corner of the shape, acknowledging rotation.
        /// </summary>
        public Vector2 TopLeft
        {
            get
            {
                Vector2 topLeft = Calculator.ToTopLeft(this);
                return Calculator.RotateVector(topLeft, Vector2.Add(topLeft, _Origin), _Rotation);
            }
        }
        /// <summary>
        /// The position of the top-right corner of the shape, acknowledging rotation.
        /// </summary>
        public Vector2 TopRight
        {
            get
            {
                Vector2 topRight = Calculator.ToTopRight(this);
                return Calculator.RotateVector(topRight, Vector2.Add(topRight, new Vector2(-_Origin.X, _Origin.Y)), _Rotation);
            }
        }
        /// <summary>
        /// The position of the bottom-left corner of the shape, acknowledging rotation.
        /// </summary>
        public Vector2 BottomLeft
        {
            get
            {
                Vector2 bottomLeft = Calculator.ToBottomLeft(this);
                return Calculator.RotateVector(bottomLeft, Vector2.Add(bottomLeft, new Vector2(_Origin.X, -_Origin.Y)), _Rotation);
            }
        }
        /// <summary>
        /// The position of the bottom-right corner of the shape, acknowledging rotation.
        /// </summary>
        public Vector2 BottomRight
        {
            get
            {
                Vector2 bottomRight = Calculator.ToBottomRight(this);
                return Calculator.RotateVector(bottomRight, Vector2.Add(bottomRight, new Vector2(-_Origin.X, -_Origin.Y)), _Rotation);
            }
        }
        /// <summary>
        /// The position (z + depth / 2) of the shape's top-edge, not acknowledging rotation.
        /// </summary>
        public float TopDepth
        {
            get { return _Position.Z + (_Depth / 2); }
        }
        /// <summary>
        /// The position (z - depth / 2) of the shape's bottom-edge, not acknowledging rotation. Assumes the shape is rectangular.
        /// </summary>
        public float BottomDepth
        {
            get { return _Position.Z - _Depth / 2; }
            set { _Position.Z = value + _Depth / 2; }
        }
        /// <summary>
        /// The shape's depth distribution.
        /// </summary>
        public DepthDistribution DepthDistribution
        {
            get { return _DepthDistribution; }
            set { _DepthDistribution = value; }
        }
        #endregion
    }
}
