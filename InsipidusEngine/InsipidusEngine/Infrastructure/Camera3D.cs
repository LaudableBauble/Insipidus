using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace InsipidusEngine.Infrastructure
{
    /// <summary>
    /// This is the camera used in the game.
    /// </summary>
    public class Camera3D
    {
        #region Fields
        private GraphicsDevice _Device;
        private Vector3 _Position;
        private Vector3 _Target;
        private float _MoveSpeed;
        private float _RotationSpeed;
        private float _XRotation;
        private float _YRotation;
        private Matrix _View;
        private Matrix _Projection;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the camera from which eyes the player will look upon the game world.
        /// </summary>
        /// <param name="device">The current graphics device.</param>
        public Camera3D(GraphicsDevice device)
        {
            Initialize(device);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the camera.
        /// </summary>
        /// <param name="device">The current graphics device.</param>
        private void Initialize(GraphicsDevice device)
        {
            //Initialize stuff.
            _Device = device;
            ResetCamera();
        }
        /// <summary>
        /// Handle input.
        /// </summary>
        /// <param name="input">The current input state.</param>
        public void HandleInput(InputState input)
        {
            //Rotation.
            if (input.IsKeyDown(Keys.Left)) { _XRotation += _RotationSpeed; }
            if (input.IsKeyDown(Keys.Right)) { _XRotation += -_RotationSpeed; }
            if (input.IsKeyDown(Keys.Down)) { _YRotation += -_RotationSpeed; }
            if (input.IsKeyDown(Keys.Up)) { _YRotation += _RotationSpeed; }

            //Movement.
            if (input.IsKeyDown(Keys.W)) { MoveCamera(new Vector3(0, 0, -1)); }
            if (input.IsKeyDown(Keys.S)) { MoveCamera(new Vector3(0,0,1)); }
            if (input.IsKeyDown(Keys.A)) { MoveCamera(new Vector3(-1, 0, 0)); }
            if (input.IsKeyDown(Keys.D)) { MoveCamera(new Vector3(1, 0, 0)); }
            if (input.IsKeyDown(Keys.E)) { MoveCamera(new Vector3(0, 1, 0)); }
            if (input.IsKeyDown(Keys.Q)) { MoveCamera(new Vector3(0, -1, 0)); }
        }
        /// <summary>
        /// Update the camera.
        /// </summary>
        /// <param name="gameTime">The GameTime instance.</param>
        public virtual void Update(GameTime gameTime)
        {
            //Update the view matrix accordingly.
            UpdateView();
        }

        /// <summary>
        /// Reset the camera.
        /// </summary>
        public void ResetCamera()
        {
            //Set the position and the look-at position.
            _Position = new Vector3(130, 30, -50);
            _Target = Vector3.Zero;

            //Set the rotation as well as the speed.
            _XRotation = MathHelper.PiOver2;
            _YRotation = -MathHelper.Pi / 10.0f;
            _MoveSpeed = 1.5f;
            _RotationSpeed = .02f;

            //Set the matrices.
            _View = Matrix.CreateLookAt(_Position, _Target, new Vector3(0, 1, 0));
            _Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _Device.Viewport.AspectRatio, .3f, 1000);
        }
        /// <summary>
        /// Move the camera.
        /// </summary>
        /// <param name="direction">The direction of the movement.</param>
        private void MoveCamera(Vector3 direction)
        {
            _Position += _MoveSpeed * Vector3.Transform(direction, Matrix.CreateRotationX(_YRotation) * Matrix.CreateRotationY(_XRotation));
        }
        /// <summary>
        /// Update the camera's view matrix.
        /// </summary>
        private void UpdateView()
        {
            //Calculate the target vector.
            Matrix rotation = Matrix.CreateRotationX(_YRotation) * Matrix.CreateRotationY(_XRotation);
            Vector3 target = _Position + Vector3.Transform(new Vector3(0, 0, -1), rotation);
            Vector3 upVector = Vector3.Transform(new Vector3(0, 1, 0), rotation);

            //Update the view matrix.
            _View = Matrix.CreateLookAt(_Position, target, upVector);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The camera viewport's position.
        /// </summary>
        public Vector3 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        /// <summary>
        /// The speed of the camera.
        /// </summary>
        public float Speed
        {
            get { return _MoveSpeed; }
            set { _MoveSpeed = value; }
        }
        /// <summary>
        /// The camera's view matrix.
        /// </summary>
        public Matrix View
        {
            get { return _View; }
            set { _View = value; }
        }
        /// <summary>
        /// The camera's projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return _Projection; }
            set { _Projection = value; }
        }
        #endregion
    }
}