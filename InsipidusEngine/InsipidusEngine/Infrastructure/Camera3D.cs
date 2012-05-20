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
        private float _Speed;
        private float _Yaw;
        private float _Pitch;
        private float _Roll;
        private Matrix _Rotation;
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
            if (input.IsKeyDown(Keys.J)) { _Yaw += .02f; }
            if (input.IsKeyDown(Keys.L)) { _Yaw += -.02f; }
            if (input.IsKeyDown(Keys.I)) { _Pitch += -.02f; }
            if (input.IsKeyDown(Keys.K)) { _Pitch += .02f; }
            if (input.IsKeyDown(Keys.U)) { _Roll += -.02f; }
            if (input.IsKeyDown(Keys.O)) { _Roll += .02f; }

            //Movement.
            if (input.IsKeyDown(Keys.W)) { MoveCamera(_Rotation.Forward); }
            if (input.IsKeyDown(Keys.S)) { MoveCamera(-_Rotation.Forward); }
            if (input.IsKeyDown(Keys.A)) { MoveCamera(-_Rotation.Right); }
            if (input.IsKeyDown(Keys.D)) { MoveCamera(_Rotation.Right); }
            if (input.IsKeyDown(Keys.E)) { MoveCamera(_Rotation.Up); }
            if (input.IsKeyDown(Keys.Q)) { MoveCamera(-_Rotation.Up); }
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
            _Position = new Vector3(0, 0, 50);
            _Target = Vector3.Zero;

            //Set the yaw, pitch and roll as well as the speed.
            _Yaw = 0;
            _Pitch = 0;
            _Roll = 0;
            _Speed = .3f;

            //Set the matrices.
            _Rotation = Matrix.Identity;
            _View = Matrix.CreateLookAt(new Vector3(130, 30, -50), new Vector3(0, 0, -40), new Vector3(0, 1, 0));
            _Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _Device.Viewport.AspectRatio, 0.3f, 1000.0f);
        }
        /// <summary>
        /// Move the camera.
        /// </summary>
        /// <param name="direction">The direction of the movement.</param>
        private void MoveCamera(Vector3 direction)
        {
            _Position += _Speed * direction;
        }
        /// <summary>
        /// Update the camera's view matrix.
        /// </summary>
        private void UpdateView()
        {
            //Normalize the rotation.
            _Rotation.Forward.Normalize();
            _Rotation.Up.Normalize();
            _Rotation.Right.Normalize();

            //Update the rotation matrix.
            _Rotation *= Matrix.CreateFromAxisAngle(_Rotation.Right, _Pitch);
            _Rotation *= Matrix.CreateFromAxisAngle(_Rotation.Up, _Yaw);
            _Rotation *= Matrix.CreateFromAxisAngle(_Rotation.Forward, _Roll);

            //Reset the yaw, pitch and roll aswell as the target vector.
            _Yaw = 0.0f;
            _Pitch = 0.0f;
            _Roll = 0.0f;
            _Target = _Position + _Rotation.Forward;

            //Update the view matrix.
            _View = Matrix.CreateLookAt(_Position, _Target, _Rotation.Up);
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
        /// The camera's rotation.
        /// </summary>
        public Matrix Rotation
        {
            get { return _Rotation; }
            set { _Rotation = value; }
        }
        /// <summary>
        /// The speed of the camera.
        /// </summary>
        public float Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
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