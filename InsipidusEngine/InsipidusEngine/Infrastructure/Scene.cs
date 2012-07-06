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
using InsipidusEngine.Physics;
using InsipidusEngine.Tools;

namespace InsipidusEngine.Helpers
{
    /// <summary>
    /// A scene is a map of the game world and can be populated by entities.
    /// </summary>
    public class Scene
    {
        #region Fields
        protected String _Name;
        protected GraphicsDevice _GraphicsDevice;
        protected PhysicsSimulator _Physics;
        protected RobustList<Entity> _Entities;

        protected RenderTarget2D _ColorMap;
        protected RenderTarget2D _NormalMap;
        protected RenderTarget2D _ShadowMap;
        protected Effect _DepthEffect;
        protected Effect _LightEffect;
        protected Effect _CombinedEffect;

        protected Vector3 _LightPosition;
        protected Matrix _View;
        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor for a scene.
        /// </summary>
        public Scene()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the scene.
        /// </summary>
        protected virtual void Initialize()
        {
            // Initialize the variables.
            _Name = "Scene";
            _Entities = new RobustList<Entity>();
            _Physics = new PhysicsSimulator();
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="graphics">The graphics device in use.</param>
        /// <param name="content">The content manager to use.</param>
        public virtual void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            //Store the graphics device somewhere close.
            _GraphicsDevice = graphics;

            //Initialize the light's position.
            _LightPosition = Vector3.Zero;

            //Create acceptable presentation parameters for the graphics device.
            PresentationParameters pp = _GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            //Create the render targets.
            _ColorMap = new RenderTarget2D(_GraphicsDevice, width, height);
            _NormalMap = new RenderTarget2D(_GraphicsDevice, width, height);
            _ShadowMap = new RenderTarget2D(_GraphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            //Load the shaders.
            _DepthEffect = content.Load<Effect>(@"Shaders\Depth");
            _LightEffect = content.Load<Effect>(@"Shaders\Lighting");
            _CombinedEffect = content.Load<Effect>(@"Shaders\DeferredCombine");

            // Load all entities' content.
        }
        /// <summary>
        /// Handle input.
        /// </summary>
        /// <param name="input">The current input state.</param>
        public virtual void HandleInput(InputState input)
        {
            // Let all entities respond to input.
            _Entities.ForEach(item => item.HandleInput(input));

            //If to change the height of the light.
            if (input.IsNewMouseScrollUp()) { _LightPosition.Z += 5f; }
            else if (input.IsNewMouseScrollDown()) { _LightPosition.Z -= 5f; }
        }
        /// <summary>
        /// Update the scene.
        /// </summary>
        /// <param name="gametime">The game time.</param>
        public virtual void Update(GameTime gametime)
        {
            // Update the physics simulator.
            _Physics.Update();

            //Update the list of entities.
            _Entities.Update();

            // Update all entities.
            _Entities.ForEach(item => item.Update(gametime));
        }
        /// <summary>
        /// Draw the scene.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch responsible for drawing the scene.</param>
        /// <param name="view">The view matrix through which the game world is seen.</param>
        public virtual void Draw(SpriteBatch spriteBatch, Matrix view)
        {
            //Update the light's position.
            _LightPosition = new Vector3(Helper.GetMousePosition(), _LightPosition.Z);

            //Update the view matrix.
            _View = view;

            //Clear the screen.
            _GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 0, 0);

            //Initialize the color map and draw to it.
            RenderColorMap(spriteBatch);

            //Initialize the normal map and draw to it.
            _GraphicsDevice.SetRenderTarget(null);
            RenderNormalMap(spriteBatch);

            //Initialize the shadow map and draw to it.
            _GraphicsDevice.SetRenderTarget(null);
            RenderShadowMap(spriteBatch);

            //Initialize the deferred and combined map.
            _GraphicsDevice.SetRenderTarget(null);
            RenderCombinedMap(spriteBatch);

            //Draw the debug render targets.
            DrawDebugRenderTargets(spriteBatch);
        }

        /// <summary>
        /// Add an entity to the scene.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public Entity AddEntity(Entity entity)
        {
            _Entities.Add(entity);
            entity.Scene = this;
            _Physics.AddBody(entity.Body);
            return entity;
        }
        /// <summary>
        /// Remove an entity from the scene.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void RemoveEntity(Entity entity)
        {
            _Entities.Remove(entity);
            _Physics.RemoveBody(entity.Body);
        }
        /// <summary>
        /// Render all entities' color maps to the appropriate render target.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        private void RenderColorMap(SpriteBatch spriteBatch)
        {
            //Set the correct render target and clear it.
            _GraphicsDevice.SetRenderTarget(_ColorMap);
            _GraphicsDevice.Clear(Color.Transparent);

            //Initialize the depth shader.
            _DepthEffect.CurrentTechnique = _DepthEffect.Techniques["DepthBuffer"];
            _DepthEffect.CurrentTechnique.Passes[0].Apply();

            //Begin drawing the scene.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, _DepthEffect, _View);

            //Enable the depth buffer. NOTE: The order is important due to that the sprite batch disables the depth buffer in the begin() call.
            DepthStencilState depth = new DepthStencilState() { DepthBufferEnable = true, DepthBufferWriteEnable = true, DepthBufferFunction = CompareFunction.GreaterEqual };
            _GraphicsDevice.DepthStencilState = depth;

            //Draw all entities to the screen.
            _Entities.ForEach(item => item.Draw(spriteBatch, DrawState.Color, _DepthEffect));

            //End the drawing.
            spriteBatch.End();
        }
        /// <summary>
        /// Render all entities' normal maps to the appropriate render target.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        private void RenderNormalMap(SpriteBatch spriteBatch)
        {
            //Set the correct render target and clear it.
            _GraphicsDevice.SetRenderTarget(_NormalMap);
            _GraphicsDevice.Clear(Color.Transparent);

            //Initialize the depth shader.
            _DepthEffect.CurrentTechnique = _DepthEffect.Techniques["DepthBuffer"];
            _DepthEffect.CurrentTechnique.Passes[0].Apply();

            //Draw all entities to the screen.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, _DepthEffect, _View);
            _Entities.ForEach(item => item.Draw(spriteBatch, DrawState.Normal, _DepthEffect));
            spriteBatch.End();
        }
        /// <summary>
        /// Render a shadow map from a color map and normal map.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        private void RenderShadowMap(SpriteBatch spriteBatch)
        {
            //Set the correct render target and clear it.
            _GraphicsDevice.SetRenderTarget(_ShadowMap);
            _GraphicsDevice.Clear(Color.Transparent);

            //Map out the texture by assigning vertices.
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = new VertexPositionColorTexture(new Vector3(-1, 1, 0), Color.White, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(1, 1, 0), Color.White, new Vector2(1, 0));
            vertices[2] = new VertexPositionColorTexture(new Vector3(-1, -1, 0), Color.White, new Vector2(0, 1));
            vertices[3] = new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, new Vector2(1, 1));
            VertexBuffer buffer = new VertexBuffer(_GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.None);
            buffer.SetData(vertices);

            //Set the vertex buffer.
            _GraphicsDevice.SetVertexBuffer(buffer);

            //Set the light data.
            _LightEffect.Parameters["_LightStrength"].SetValue(1f);
            _LightEffect.Parameters["_LightPosition"].SetValue(_LightPosition);
            _LightEffect.Parameters["_LightColor"].SetValue(new Vector4(1.0f, 1f, 1.0f, 1.0f));
            _LightEffect.Parameters["_LightDecay"].SetValue(200);
            _LightEffect.Parameters["_SpecularStrength"].SetValue(1f);

            //Set the technique.
            _LightEffect.CurrentTechnique = _LightEffect.Techniques["DeferredLighting"];

            _LightEffect.Parameters["_ScreenWidth"].SetValue(_GraphicsDevice.Viewport.Width);
            _LightEffect.Parameters["_ScreenHeight"].SetValue(_GraphicsDevice.Viewport.Height);
            _LightEffect.Parameters["_AmbientColor"].SetValue(new Color(.1f, .1f, .1f, 1).ToVector4());
            _LightEffect.Parameters["_NormalMap"].SetValue(_NormalMap);
            _LightEffect.Parameters["_ColorMap"].SetValue(_ColorMap);

            //Perform a shader pass.
            _LightEffect.CurrentTechnique.Passes[0].Apply();

            //Add Belding (Black background) to the render target and draw the light map over the black background.
            _GraphicsDevice.BlendState = Helper.BlendBlack;
            _GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
        /// <summary>
        /// Render the final color map that will be displayed to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        private void RenderCombinedMap(SpriteBatch spriteBatch)
        {
            //Set the correct render target and clear it.
            _GraphicsDevice.Clear(Color.Black);

            //Finally draw the combined maps onto the screen.
            _CombinedEffect.CurrentTechnique = _CombinedEffect.Techniques["DeferredCombined"];
            _CombinedEffect.Parameters["_Ambient"].SetValue(1f);
            _CombinedEffect.Parameters["_LightAmbient"].SetValue(4);
            _CombinedEffect.Parameters["_AmbientColor"].SetValue(new Color(.1f, .1f, .1f, 1).ToVector4());
            _CombinedEffect.Parameters["_ColorMap"].SetValue(_ColorMap);
            _CombinedEffect.Parameters["_ShadingMap"].SetValue(_ShadowMap);
            _CombinedEffect.Parameters["_NormalMap"].SetValue(_NormalMap);

            //Perform the shader pass.
            _CombinedEffect.CurrentTechnique.Passes[0].Apply();

            //Draw the color map to the screen with the shadow map overlapping it.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, _CombinedEffect);
            spriteBatch.Draw(_ColorMap, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
        /// <summary>
        /// [DEBUG] Draws the debug render targets onto the bottom of the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        private void DrawDebugRenderTargets(SpriteBatch spriteBatch)
        {
            // Draw some debug textures.
            spriteBatch.Begin();
            Rectangle size = new Rectangle(0, 0, _ColorMap.Width / 3, _ColorMap.Height / 3);
            var position = new Vector2(0, _GraphicsDevice.Viewport.Height - size.Height);
            spriteBatch.Draw(_ColorMap, new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height), Color.White);
            spriteBatch.Draw(_NormalMap, new Rectangle((int)position.X + size.Width, (int)position.Y, size.Width, size.Height), Color.White);
            spriteBatch.Draw(_ShadowMap, new Rectangle((int)position.X + size.Width * 2, (int)position.Y, size.Width, size.Height), Color.White);
            spriteBatch.End();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name of the scene.
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// The physics simulator active in the scene.
        /// </summary>
        public PhysicsSimulator PhysicsSimulator
        {
            get { return _Physics; }
            set { _Physics = value; }
        }
        /// <summary>
        /// The list of entities in the scene.
        /// </summary>
        public RobustList<Entity> Entities
        {
            get { return _Entities; }
            set { _Entities = value; }
        }
        #endregion
    }
}
