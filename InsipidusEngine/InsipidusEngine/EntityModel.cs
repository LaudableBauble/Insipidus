using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InsipidusEngine
{
    /// <summary>
    /// An entity model is a polygon-shape that when overlayed with a texture can be seen in the game.
    /// </summary>
    public class EntityModel
    {
        #region Fields
        private VertexPositionNormalTexture[] _Vertices;
        private Vector3 _Position;
        private Vector3 _Size;
        private float _Rotation;
        private Texture2D _Texture;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for an entity model.
        /// </summary>
        public EntityModel()
        {
            Initialize();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Initialize the entity model.
        /// </summary>
        protected void Initialize()
        {
            //Initialize some variables.
            _Vertices = new VertexPositionNormalTexture[0];
            _Position = Vector3.Zero;
            _Size = Vector3.One;
            _Rotation = 0;
        }
        /// <summary>
        /// Writes our list of vertices to the vertex buffer, then draws triangles to the device.
        /// </summary>
        /// <param name="device">The graphics device to draw to.</param>
        public void Draw(GraphicsDevice device)
        {
            //Make sure we have vertices to draw.
            if (_Vertices.Count() == 0) { return; }

            // Create the shape buffer and dispose of it to prevent out of memory.
            using (VertexBuffer buffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, _Vertices.Count(), BufferUsage.WriteOnly))
            {
                // Load the buffer.
                buffer.SetData(_Vertices);

                // Send the vertex buffer to the device.
                device.SetVertexBuffer(buffer);
            }

            // Draw the primitives from the vertex buffer to the device as triangles.
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, _Vertices.Count() / 3);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The position of the model.
        /// </summary>
        public Vector3 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        /// <summary>
        /// The rotation of the model.
        /// </summary>
        public float Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }
        /// <summary>
        /// The size of the model.
        /// </summary>
        public Vector3 Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        /// <summary>
        /// The texture of the model.
        /// </summary>
        public Texture2D Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }
        /// <summary>
        /// The vertices that compose this model.
        /// </summary>
        public VertexPositionNormalTexture[] Vertices
        {
            get { return _Vertices; }
            set { _Vertices = value; }
        }
        /// <summary>
        /// The model's world matrix.
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Size) * Matrix.CreateTranslation(Position); }
        }
        #endregion
    }
}