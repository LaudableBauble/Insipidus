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
        /**
	 * Constructor for an entity.
	 * 
	 * @param scene
	 *            The scene this entity is part of.
	 */

        public Entity(Scene scene)
        {
            Initialize(scene);
        }
        #endregion

        #region Methods
        /**
	 * Initialize the entity.
	 * 
	 * @param scene
	 *            The scene this entity is part of.
	 */
        protected void Initialize(Scene scene)
        {
            // Initialize the variables.
            _Name = "";
            _Scene = scene;
            _Sprites = new SpriteManager();
            _Body = new Body(_Scene != null ? _Scene.PhysicsSimulator : null);
            _Body.setEntity(this);
            _Body.addBody();
        }

        /**
         * Load content and add a sprite. This uses default body shape values.
         * 
         * @param spritePath
         *            The path of the main sprite.
         */
        public void LoadContent(String spritePath)
        {
            LoadContent(spritePath, -1);
        }

        /**
         * Load content and add a sprite.
         * 
         * @param spritePath
         *            The path of the main sprite.
         * @param height
         *            The height of the shape as seen on picture. This is used for collision data. -1 results in the use of the full height of the sprite and a depth of 1.
         */
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
		_Body.getShape().setWidth(_Sprites[0][_Sprites[0].CurrentFrameIndex].Width);
		_Body.getShape().setHeight((height == -1) ? _Sprites[0][_Sprites[0].CurrentFrameIndex].Height : height);
		_Body.getShape().setDepth((height == -1) ? 1 : _Sprites[0][_Sprites[0].CurrentFrameIndex].Height - height);

		// Update the sprite's position offset.
		_Sprites.[0].setPositionOffset(new Vector2(0, -_Sprites[0][_Sprites[0].CurrentFrameIndex].Origin.Y + (_Body.getShape().getHeight() / 2)));
	}

        /**
         * Handle input.
         * 
         * @param input
         *            The input manager.
         */
        public void HandleInput(InputState input)
        {
            // Check if the left mouse button is down.
            if (input.IsNewLeftMousePress())
            {
                // Transform the mouse coordinates into world space.

                // Check if the Body has been clicked on.
                if ((input.mouseEventPosition().x <= (_Body.getLayeredPosition().x + (_Body.getShape().getWidth() / 2)))
                        && (input.mouseEventPosition().x >= (_Body.getLayeredPosition().x - (_Body.getShape().getWidth() / 2)))
                        && (input.mouseEventPosition().y <= (_Body.getLayeredPosition().y + (_Body.getShape().getHeight() / 2)))
                        && (input.mouseEventPosition().y >= (_Body.getLayeredPosition().y - (_Body.getShape().getHeight() / 2))))
                {
                    // Turn the debug isClicked variable on.
                    _Body._IsClicked = !_Body._IsClicked;
                }
            }
        }

        /**
         * Update the entity.
         * 
         * @param gameTime
         *            The game timer.
         */
        public void Update(GameTime gameTime)
        {
            // Update the sprites.
            _Sprites.Update(gameTime, Helper.getScreenPosition(new Vector3(_Body.getLayeredPosition(), _Body.getShape().getBottomDepth())));
        }

        /**
         * Draw the entity.
         * 
         * @param graphics
         *            The graphics component.
         */
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the sprite.
            _Sprites.Draw(spriteBatch);
        }
        #endregion

        #region Properties
        /**
	 * Get the entity's body.
	 * 
	 * @return The body of the entity.
	 */
        public Body Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        /**
         * Get the manager of all this entity's sprites.
         * 
         * @return The sprite manager.
         */
        public SpriteManager Sprites
        {
            get { return _Sprites; }
            set { _Sprites = value; }
        }

        /**
         * Set the entity's depth. This also modifies its height as well as updates the main sprite.
         * 
         * @param depth
         *            The new depth.
         */
        public float Depth
        {
            set
            {
                // Set the body's height and depth.
                _Body.getShape().setHeight((value < 1) ? _Sprites[0][_Sprites[0].CurrentFrameIndex].Height : _Sprites[0][_Sprites[0].CurrentFrameIndex].Height - value);
                _Body.getShape().setDepth((value < 1) ? 1 : value);

                // Update the sprite's position offset.
                _Sprites[0].PositionOffset = new Vector2(0, -_Sprites[0][_Sprites[0].CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
            }
        }
        /**
         * Set the entity's height. This also modifies its depth as well as updates the main sprite.
         * 
         * @param height
         *            The new height.
         */
        public float Height
        {
            set
            {
                // Set the body's height and depth.
                _Body.getShape().setHeight((value == -1) ? _Sprites[0][_Sprites[0].CurrentFrameIndex].Height : value);
                _Body.getShape().setDepth((value == -1) ? 1 : _Sprites[0][_Sprites[0].CurrentFrameIndex].Height - value);

                // Update the sprite's position offset.
                _Sprites[0].PositionOffset = new Vector2(0, -_Sprites[0][_Sprites[0].CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
            }
        }
        /**
         * Get the entity's name.
         * 
         * @return The name of the entity.
         */
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /**
         * Get the entity's position, ie. all three dimensions.
         * 
         * @return The position of the entity.
         */
        public Vector3 Position
        {
            get { return _Body.Position; }
            set { _Body.Position = value; }
        }
        /**
         * Get the scene the entity is part of.
         * 
         * @return The scene of the entity.
         */
        public Scene Scene
        {
            get { return _Scene; }
            set { _Scene = value; }
        }
        #endregion
    }
}
