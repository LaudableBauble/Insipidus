#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using InsipidusEngine.Imagery;
using InsipidusEngine.Infrastructure;
using InsipidusEngine.Scenes;
#endregion

namespace InsipidusEngine.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class RPGScreen : GameScreen
    {
        #region Fields
        private GraphicsDevice device;
        private ContentManager content;
        private Scene _Scene;
        private Camera2D _Camera;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a RPG screen.
        /// </summary>
        public RPGScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null) { content = new ContentManager(ScreenManager.Game.Services, "Content"); }

            #region Initialize
            //Store the device.
            device = ScreenManager.GraphicsDevice;

            //Create the scene.
            _Scene = new LargeDemoScene();

            //Create the camera.
            _Camera = new Camera2D(new Rectangle(0, 0, ScreenManager.Game.Window.ClientBounds.Width, ScreenManager.Game.Window.ClientBounds.Height),
                new Rectangle(0, 0, 2000, 2000));
            _Camera.Position = new Vector2(1000, 1000);
            #endregion

            #region LoadContent
            //Load the scene's content.
            _Scene.LoadContent(content);
            #endregion

            // Once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }
        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null) { throw new ArgumentNullException("input"); }

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
            else
            {
                //Let the scene handle input.
                _Scene.HandleInput(input);

                //Move the camera.
                if (input.IsKeyDown(Keys.A)) { _Camera.Move(new Vector2(-1, 0)); }
                else if (input.IsKeyDown(Keys.S)) { _Camera.Move(new Vector2(0, 1)); }
                else if (input.IsKeyDown(Keys.D)) { _Camera.Move(new Vector2(1, 0)); }
                else if (input.IsKeyDown(Keys.W)) { _Camera.Move(new Vector2(0, -1)); }
            }
        }
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                //Update the scene.
                _Scene.Update(gameTime);
                //Update the camera.
                _Camera.Update(gameTime);
            }
        }
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            device.RasterizerState = RasterizerState.CullNone;
            //device.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            device.DepthStencilState = new DepthStencilState();

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

            //The sprite batch to use.
            SpriteBatch spriteBatch = new SpriteBatch(device);

            //Begin drawing.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, _Camera.Transform);

            //Draw the scene.
            _Scene.Draw(spriteBatch);

            //End the drawing.
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) { ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha); }
        }
        #endregion
    }
}