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

using InsipidusEngine.Physics;
using InsipidusEngine.Infrastructure;
using InsipidusEngine.Imagery;

namespace InsipidusEngine.Core
{
    /// <summary>
    /// An entity, sporting a body and a sprite, is the most basic building blocks of the physical game world.
    /// </summary>
    public class Entity
    {
        #region Fields
        protected String _Name;
        protected Scene _Scene;
        protected SpriteManager _Sprites;
        protected Body _Body;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for an entity.
        /// </summary>
        /// <param name="scene">The scene this entity is part of.</param>
        public Entity(Scene scene)
        {
            Initialize(scene);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the scene.
        /// </summary>
        /// <param name="scene">The scene this entity is part of.</param>
        protected void Initialize(Scene scene)
        {
            // Initialize the variables.
            _Name = "";
            _Scene = scene;
            _Sprites = new SpriteManager();
            _Body = new Body(_Scene != null ? _Scene.PhysicsSimulator : null);
            _Body.Entity = this;
            _Body.AddBody();
        }
        /// <summary>
        /// Load content and add a sprite. This uses default body shape values.
        /// </summary>
        /// <param name="spritePath">The path to the main sprite.</param>
        public void LoadContent(String spritePath)
        {
            LoadContent(spritePath, -1);
        }
        /// <summary>
        /// Load content and add a sprite.
        /// </summary>
        /// <param name="spritePath">The path to the main sprite.</param>
        /// <param name="height">The height of the shape as seen on picture. This is used for collision data.
        /// -1 results in the use of the full height of the sprite and a depth of 1.</param>
        public void LoadContent(String spritePath, float height)
        {
            // Clear all sprites.
            _Sprites = new SpriteManager();

            // Add a sprite.
            _Sprites.AddSprite(new Sprite(_Sprites, "Entity"));
            _Sprites[0].AddFrame(new Frame(spritePath));

            // Load all sprites' content.
            _Sprites.LoadContent();

            // Set the shape of the body.
            _Body.Shape.Width = _Sprites[0][_Sprites[0].CurrentFrameIndex].Width;
            _Body.Shape.Height = (height == -1) ? _Sprites[0][_Sprites[0].CurrentFrameIndex].Height : height;
            _Body.Shape.Depth = (height == -1) ? 1 : _Sprites[0][_Sprites[0].CurrentFrameIndex].Height - height;

            // Update the sprite's position offset.
            _Sprites[0].PositionOffset = new Vector2(0, -_Sprites[0][_Sprites[0].CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
        }
        /// <summary>
        /// Update the entity.
        /// </summary>
        /// <param name="gameTime">The game's time.</param>
        public void Update(GameTime gameTime)
        {
            // Update the sprites.
            _Sprites.Update(gameTime, Helper.getScreenPosition(new Vector3(_Body.LayeredPosition, _Body.Shape.BottomDepth)));
        }
        /// <summary>
        /// Draw the entity.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the sprite.
            _Sprites.Draw(spriteBatch);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The entity's body.
        /// </summary>
        public Body Body
        {
            get { return _Body; }
            set { _Body = value; }
        }
        /// <summary>
        /// The entity's sprite manager.
        /// </summary>
        public SpriteManager Sprites
        {
            get { return _Sprites; }
            set { _Sprites = value; }
        }
        /// <summary>
        /// Set the entity's depth. This also modifies its height as well as updates the main sprite.
        /// </summary>
        public float Depth
        {
            set
            {
                // Set the body's height and depth.
                _Body.Shape.Height = (value < 1) ? _Sprites[0][_Sprites[0].CurrentFrameIndex].Height : _Sprites[0][_Sprites[0].CurrentFrameIndex].Height - value;
                _Body.Shape.Depth = (value < 1) ? 1 : value;

                // Update the sprite's position offset.
                _Sprites[0].PositionOffset = new Vector2(0, -_Sprites[0][_Sprites[0].CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
            }
        }
        /// <summary>
        /// Set the entity's height. This also modifies its depth as well as updates the main sprite.
        /// </summary>
        public float Height
        {
            set
            {
                // Set the body's height and depth.
                _Body.Shape.Height = (value == -1) ? _Sprites[0][_Sprites[0].CurrentFrameIndex].Height : value;
                _Body.Shape.Depth = (value == -1) ? 1 : _Sprites[0][_Sprites[0].CurrentFrameIndex].Height - value;

                // Update the sprite's position offset.
                _Sprites[0].PositionOffset = new Vector2(0, -_Sprites[0][_Sprites[0].CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
            }
        }
        /// <summary>
        /// The name of the entity.
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// The position of the entity.
        /// </summary>
        public Vector3 Position
        {
            get { return _Body.Position; }
            set { _Body.Position = value; }
        }
        /// <summary>
        /// The scene this entity is part of.
        /// </summary>
        public Scene Scene
        {
            get { return _Scene; }
            set { _Scene = value; }
        }
        #endregion
    }
}
