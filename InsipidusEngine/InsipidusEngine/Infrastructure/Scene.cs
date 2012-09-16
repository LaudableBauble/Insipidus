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
using InsipidusEngine.Imagery;

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
        protected RenderTarget2D _DepthMap;
        protected RenderTarget2D _ShadowMap;
        protected RenderTarget2D _CombinedMap;
        protected Effect _DepthEffect;
        protected Effect _LightEffect;
        protected Effect _CombinedEffect;

        protected Light _LightData;
        protected Entity _Light;
        protected Matrix _View;

        protected RenderMap _MapToDraw;
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
            _MapToDraw = RenderMap.Combined;
            _Light = new Entity(this);
            _LightData = new Light(Vector3.Zero, Color.White, 1, 50);
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

            //Setup the light.
            _Light.Body.IsImmaterial = true;
            _Light.Name = "Light";
            AddEntity(_Light);
            _Light.LoadContent(content, @"Misc\LightBulb[1]");

            //Create acceptable presentation parameters for the graphics device.
            PresentationParameters pp = _GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            //Create the render targets.
            _ColorMap = new RenderTarget2D(_GraphicsDevice, width, height, false, format, pp.DepthStencilFormat);
            _NormalMap = new RenderTarget2D(_GraphicsDevice, width, height, false, format, pp.DepthStencilFormat);
            _DepthMap = new RenderTarget2D(_GraphicsDevice, width, height, false, format, pp.DepthStencilFormat);
            _ShadowMap = new RenderTarget2D(_GraphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            _CombinedMap = new RenderTarget2D(_GraphicsDevice, width, height, false, format, pp.DepthStencilFormat);

            //Load the shaders.
            _DepthEffect = content.Load<Effect>(@"Shaders\DepthBuffer");
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

            //Change the height of the light.
            if (input.IsNewMouseScrollUp() || input.IsKeyDown(Keys.Q)) { _Light.Position += new Vector3(0, 0, 5f); }
            else if (input.IsNewMouseScrollDown() || input.IsKeyDown(Keys.E)) { _Light.Position -= new Vector3(0, 0, 5f); }

            //Change which render map to be displayed.
            if (input.IsNewKeyPress(Keys.R)) { _MapToDraw = Enum.IsDefined(typeof(RenderMap), _MapToDraw - 1) ? _MapToDraw - 1 : RenderMap.Combined; }
            else if (input.IsNewKeyPress(Keys.T)) { _MapToDraw = Enum.IsDefined(typeof(RenderMap), _MapToDraw + 1) ? _MapToDraw + 1 : RenderMap.Color; }
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

            //Update the position of the light.
            _Light.Position = new Vector3(Helper.ConvertScreenToWorld(Helper.GetMousePosition(), _View), _Light.Position.Z);

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
            //Update the view matrix.
            _View = view;

            //Clear the screen.
            _GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 0, 0);

            //Initialize the maps and draw to them in order.
            RenderColorMap(spriteBatch);
            RenderNormalMap(spriteBatch);
            RenderDepthMap(spriteBatch);
            RenderShadowMap(spriteBatch);
            RenderCombinedMap(spriteBatch);

            //Decide which rendering to display.
            _GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            switch (_MapToDraw)
            {
                case RenderMap.Color: { spriteBatch.Draw(_ColorMap, Vector2.Zero, Color.White); break; }
                case RenderMap.Normal: { spriteBatch.Draw(_NormalMap, Vector2.Zero, Color.White); break; }
                case RenderMap.Depth: { spriteBatch.Draw(_DepthMap, Vector2.Zero, Color.White); break; }
                case RenderMap.Shadow: { spriteBatch.Draw(_ShadowMap, Vector2.Zero, Color.White); break; }
                case RenderMap.Combined: { spriteBatch.Draw(_CombinedMap, Vector2.Zero, Color.White); break; }
            }
            spriteBatch.End();

            //Draw the debug render targets.
            //DrawDebugRenderTargets(spriteBatch);
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
            _GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 0, 0);

            //Initialize the depth shader.
            _DepthEffect.CurrentTechnique = _DepthEffect.Techniques["DepthBuffer"];
            _DepthEffect.Parameters["DrawToDepthMap"].SetValue(false);
            _DepthEffect.CurrentTechnique.Passes[0].Apply();

            //Begin drawing the scene.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, _DepthEffect, _View);

            //Enable the depth buffer. NOTE: The order is important due to that the sprite batch disables the depth buffer in the begin() call.
            DepthStencilState depth = new DepthStencilState() { DepthBufferEnable = true, DepthBufferWriteEnable = true, DepthBufferFunction = CompareFunction.GreaterEqual };
            _GraphicsDevice.DepthStencilState = depth;

            //Draw all entities to the screen.
            _Entities.ForEach(item => item.Draw(spriteBatch, DrawState.Color, _DepthEffect));

            //DEBUG!
            _Light.Draw(spriteBatch, DrawState.Color, null);

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
            _GraphicsDevice.SetRenderTarget(null);
            _GraphicsDevice.SetRenderTarget(_NormalMap);
            _GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 0, 0);

            //Initialize the depth shader.
            _DepthEffect.CurrentTechnique = _DepthEffect.Techniques["DepthBuffer"];
            _DepthEffect.Parameters["DrawToDepthMap"].SetValue(false);
            _DepthEffect.CurrentTechnique.Passes[0].Apply();

            //Begin drawing the scene.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, _DepthEffect, _View);

            //Enable the depth buffer. NOTE: The order is important due to that the sprite batch disables the depth buffer in the begin() call.
            DepthStencilState depth = new DepthStencilState() { DepthBufferEnable = true, DepthBufferWriteEnable = true, DepthBufferFunction = CompareFunction.GreaterEqual };
            _GraphicsDevice.DepthStencilState = depth;

            //Draw all entities to the screen.
            _Entities.ForEach(item => item.Draw(spriteBatch, DrawState.Normal, _DepthEffect));

            //End the drawing.
            spriteBatch.End();
        }
        /// <summary>
        /// Render all entities' depth maps to the appropriate render target.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        private void RenderDepthMap(SpriteBatch spriteBatch)
        {
            //Set the correct render target and clear it.
            _GraphicsDevice.SetRenderTarget(null);
            _GraphicsDevice.SetRenderTarget(_DepthMap);
            _GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 0, 0);

            //Initialize the depth shader.
            _DepthEffect.CurrentTechnique = _DepthEffect.Techniques["DepthBuffer"];
            _DepthEffect.Parameters["DrawToDepthMap"].SetValue(true);
            _DepthEffect.CurrentTechnique.Passes[0].Apply();

            //Begin drawing the scene.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, _DepthEffect, _View);

            //Enable the depth buffer. NOTE: The order is important due to that the sprite batch disables the depth buffer in the begin() call.
            DepthStencilState depth = new DepthStencilState() { DepthBufferEnable = true, DepthBufferWriteEnable = true, DepthBufferFunction = CompareFunction.GreaterEqual };
            _GraphicsDevice.DepthStencilState = depth;

            //Draw all entities to the screen.
            _Entities.ForEach(item => item.Draw(spriteBatch, DrawState.Depth, _DepthEffect));

            //End the drawing.
            spriteBatch.End();
        }
        /// <summary>
        /// Render a shadow map from a color map and normal map.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        private void RenderShadowMap(SpriteBatch spriteBatch)
        {
            //Set the correct render target and clear it.
            _GraphicsDevice.SetRenderTarget(null);
            _GraphicsDevice.SetRenderTarget(_ShadowMap);
            _GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 0, 0);

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
            _LightEffect.Parameters["LightStrength"].SetValue(_LightData.Strength);
            _LightEffect.Parameters["LightPosition"].SetValue(_Light.Position);
            _LightEffect.Parameters["LightColor"].SetValue(_LightData.Color.ToVector4());
            _LightEffect.Parameters["LightRadius"].SetValue(_LightData.Radius);
            _LightEffect.Parameters["LightDecay"].SetValue(200);
            _LightEffect.Parameters["SpecularStrength"].SetValue(1f);
            _LightEffect.Parameters["ScreenWidth"].SetValue(_GraphicsDevice.Viewport.Width);
            _LightEffect.Parameters["ScreenHeight"].SetValue(_GraphicsDevice.Viewport.Height);
            _LightEffect.Parameters["CameraPosition"].SetValue(new Vector3(_GraphicsDevice.Viewport.Width / 2, _GraphicsDevice.Viewport.Height / 2, 1));
            _LightEffect.Parameters["AmbientColor"].SetValue(new Color(.1f, .1f, .1f, 1).ToVector4());
            _LightEffect.Parameters["NormalMap"].SetValue(_NormalMap);
            _LightEffect.Parameters["ColorMap"].SetValue(_ColorMap);
            _LightEffect.Parameters["DepthMap"].SetValue(_DepthMap);

            //Set the technique.
            _LightEffect.CurrentTechnique = _LightEffect.Techniques["DeferredLighting"];

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
            //Clear the render target.
            _GraphicsDevice.SetRenderTarget(null);
            _GraphicsDevice.SetRenderTarget(_CombinedMap);
            _GraphicsDevice.Clear(Color.Black);

            //Finally draw the combined maps onto the screen.
            _CombinedEffect.CurrentTechnique = _CombinedEffect.Techniques["DeferredCombined"];
            _CombinedEffect.Parameters["Ambient"].SetValue(1f);
            _CombinedEffect.Parameters["LightAmbient"].SetValue(4);
            _CombinedEffect.Parameters["AmbientColor"].SetValue(new Color(.1f, .1f, .1f, 1).ToVector4());
            _CombinedEffect.Parameters["ColorMap"].SetValue(_ColorMap);
            _CombinedEffect.Parameters["ShadingMap"].SetValue(_ShadowMap);
            _CombinedEffect.Parameters["NormalMap"].SetValue(_NormalMap);

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
            Rectangle size = new Rectangle(0, 0, _ColorMap.Width / 4, _ColorMap.Height / 4);
            var position = new Vector2(0, _GraphicsDevice.Viewport.Height - size.Height);
            spriteBatch.Draw(_ColorMap, new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height), Color.White);
            spriteBatch.Draw(_NormalMap, new Rectangle((int)position.X + size.Width, (int)position.Y, size.Width, size.Height), Color.White);
            spriteBatch.Draw(_DepthMap, new Rectangle((int)position.X + size.Width * 2, (int)position.Y, size.Width, size.Height), Color.White);
            spriteBatch.Draw(_ShadowMap, new Rectangle((int)position.X + size.Width * 3, (int)position.Y, size.Width, size.Height), Color.White);
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
