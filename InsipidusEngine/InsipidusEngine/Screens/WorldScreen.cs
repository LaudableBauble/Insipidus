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
using InsipidusEngine.Battle;
using InsipidusEngine.Infrastructure;
#endregion

namespace InsipidusEngine.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class WorldScreen : GameScreen
    {
        #region Fields
        GraphicsDevice device;
        ContentManager content;

        Camera3D _Camera;

        int terrainWidth;
        int terrainLength;
        float[,] heightData;

        VertexBuffer terrainVertexBuffer;
        IndexBuffer terrainIndexBuffer;

        Effect effect;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public WorldScreen()
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

            //Create the camera.
            _Camera = new Camera3D(device);
            #endregion

            #region LoadContent
            effect = content.Load<Effect>(@"Misc\Effect");

            LoadVertices();
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
                //Let the camera handle input.
                _Camera.HandleInput(input);
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
            DrawTerrain();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) { ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha); }
        }

        private void LoadVertices()
        {
            Texture2D heightMap = content.Load<Texture2D>(@"Misc\heightmap");
            LoadHeightData(heightMap);

            VertexPositionNormalColored[] terrainVertices = SetUpTerrainVertices();
            int[] terrainIndices = SetUpTerrainIndices();
            terrainVertices = CalculateNormals(terrainVertices, terrainIndices);
            CopyToTerrainBuffers(terrainVertices, terrainIndices);
        }
        private void LoadHeightData(Texture2D heightMap)
        {
            float minimumHeight = float.MaxValue;
            float maximumHeight = float.MinValue;

            terrainWidth = heightMap.Width;
            terrainLength = heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainLength];
            heightMap.GetData(heightMapColors);

            heightData = new float[terrainWidth, terrainLength];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainLength; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R;
                    if (heightData[x, y] < minimumHeight) { minimumHeight = heightData[x, y]; }
                    if (heightData[x, y] > maximumHeight) { maximumHeight = heightData[x, y]; }
                }
            }

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainLength; y++)
                {
                    heightData[x, y] = (heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 50.0f;
                }
            }
        }
        private void DrawTerrain()
        {
            effect.CurrentTechnique = effect.Techniques["Colored"];
            Matrix worldMatrix = Matrix.Identity;
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(_Camera.View);
            effect.Parameters["xProjection"].SetValue(_Camera.Projection);

            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xAmbient"].SetValue(0.1f);
            effect.Parameters["xLightDirection"].SetValue(new Vector3(.5f, -.5f, .5f));

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.SetVertexBuffer(terrainVertexBuffer);
                device.Indices = terrainIndexBuffer;

                int noVertices = terrainVertexBuffer.VertexCount;
                int noTriangles = terrainIndexBuffer.IndexCount / 3;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, noVertices, 0, noTriangles);
            }
        }
        private VertexPositionNormalColored[] SetUpTerrainVertices()
        {
            VertexPositionNormalColored[] terrainVertices = new VertexPositionNormalColored[terrainWidth * terrainLength];

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainLength; y++)
                {
                    terrainVertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);

                    if (heightData[x, y] < 6) { terrainVertices[x + y * terrainWidth].Color = Color.Blue; }
                    else if (heightData[x, y] < 8) { terrainVertices[x + y * terrainWidth].Color = Color.Aquamarine; }
                    else if (heightData[x, y] < 10) { terrainVertices[x + y * terrainWidth].Color = Color.SandyBrown; }
                    else if (heightData[x, y] < 15) { terrainVertices[x + y * terrainWidth].Color = Color.Green; }
                    else if (heightData[x, y] < 18) { terrainVertices[x + y * terrainWidth].Color = Color.ForestGreen; }
                    else if (heightData[x, y] < 25) { terrainVertices[x + y * terrainWidth].Color = Color.SaddleBrown; }
                    else if (heightData[x, y] < 30) { terrainVertices[x + y * terrainWidth].Color = Color.DimGray; }
                    else { terrainVertices[x + y * terrainWidth].Color = Color.White; }
                }
            }

            return terrainVertices;
        }
        private int[] SetUpTerrainIndices()
        {
            int[] indices = new int[(terrainWidth - 1) * (terrainLength - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainLength - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }

            return indices;
        }
        private VertexPositionNormalColored[] CalculateNormals(VertexPositionNormalColored[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }
        private void CopyToTerrainBuffers(VertexPositionNormalColored[] vertices, int[] indices)
        {
            terrainVertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalColored), vertices.Length, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(vertices);

            terrainIndexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(indices);
        }
        #endregion
    }
}