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

using InsipidusEngine.Imagery;
using InsipidusEngine.Infrastructure;

namespace InsipidusEngine.Core
{
    /// <summary>
    /// A player is an entity that can be controlled by a user.
    /// </summary>
    public class Player : Entity
    {
        #region Fields
        // If the player can be controlled by the player.
        private bool _CanBeControlled;
        // The maximum speed the player can travel willingly in any direction.
        private double _MaxSpeed;
        // The currently active sprite.
        private Sprite _CurrentSprite;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a player.
        /// </summary>
        /// <param name="scene">The scene this player is part of.</param>
        public Player(Scene scene) : base(scene) { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the player.
        /// </summary>
        /// <param name="scene">The scene this player is part of.</param>
        protected override void Initialize(Scene scene)
        {
            // Call the base method.
            base.Initialize(scene);

            // Initialize the variables.
            _CanBeControlled = true;
            _MaxSpeed = 2;
        }
        /// <summary>
        /// Load content.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content)
        {
            // Clear all sprites.
            _Sprites = new SpriteManager();

            // Front.
            Sprite front = _Sprites.Add(new Sprite(_Sprites, "Front"));
            front.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Front[1]");
            front.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Front[2]");
            front.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Front[3]");

            // Back.
            Sprite back = _Sprites.Add(new Sprite(_Sprites, "Back"));
            back.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Back[0]");
            back.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Back[1]");
            back.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Back[2]");
            back.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Back[3]");

            // Right.
            Sprite right = _Sprites.Add(new Sprite(_Sprites, "Right"));
            right.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Right[0]");
            right.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Right[1]");
            right.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Right[2]");

            // Left.
            Sprite left = _Sprites.Add(new Sprite(_Sprites, "Left"));
            left.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Left[0]");
            left.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Left[1]");
            left.AddFrame(@"Characters\ZombieGuy\ZombieGuy1_Left[2]");

            // Only make one sprite visible.
            back.Visibility = Visibility.Invisible;
            right.Visibility = Visibility.Invisible;
            left.Visibility = Visibility.Invisible;

            // Current sprite is facing up.
            _CurrentSprite = front;

            // Load all sprites' content.
            _Sprites.LoadContent(content);

            // Set the shape of the body.
            _Body.Shape.Width = 15;
            _Body.Shape.Height = 15;
            _Body.Shape.Depth = _Sprites[0][_Sprites[0].CurrentFrameIndex].Height / 3;

            // Update the sprites' position offset.
            front.PositionOffset = new Vector2(0, -front[front.CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
            back.PositionOffset = new Vector2(0, -back[back.CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
            right.PositionOffset = new Vector2(0, -right[right.CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
            left.PositionOffset = new Vector2(0, -left[left.CurrentFrameIndex].Origin.Y + (_Body.Shape.Height / 2));
        }
        /// <summary>
        /// Handle input.
        /// </summary>
        /// <param name="input">The current state of input.</param>
        public override void HandleInput(InputState input)
        {
            // Call the base method.
            base.HandleInput(input);

            // If the player can't be controlled, end here.
            if (!_CanBeControlled) { return; }

            // If an arrow key is pressed.
            if (input.IsKeyDown(Keys.Left) && (_Body.Velocity.X > -_MaxSpeed))
            {
                // Left.
                _Body.AddForce(new Vector3(-_Body.AccelerationValue, 0, 0));
            }
            if (input.IsKeyDown(Keys.Right) && (_Body.Velocity.X < _MaxSpeed))
            {
                // Right.
                _Body.AddForce(new Vector3(_Body.AccelerationValue, 0, 0));
            }
            if (input.IsKeyDown(Keys.Up) && (_Body.Velocity.Y > -_MaxSpeed))
            {
                // Up.
                _Body.AddForce(new Vector3(0, -_Body.AccelerationValue, 0));
            }
            if (input.IsKeyDown(Keys.Down) && (_Body.Velocity.Y < _MaxSpeed))
            {
                // Down.
                _Body.AddForce(new Vector3(0, _Body.AccelerationValue, 0));
            }
            // If the space key is pressed.
            if (input.IsKeyDown(Keys.Space) && Math.Abs(_Body.Velocity.Z) < 1)
            {
                // Up in the air.
                _Body.AddForce(new Vector3(0, 0, _Body.AccelerationValue));
            }
        }
        /// <summary>
        /// Update the player.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            // Call the base method.
            base.Update(gameTime);

            // Determine which sprite should be drawn.
            ChangeSprite();
        }

        /// <summary>
        /// Change the sprite depending on velocity direction. Makes the player look towards where he is heading.
        /// </summary>
        private void ChangeSprite()
        {
            // If the player is not moving, stop here.
            if (Vector2.Distance(new Vector2(_Body.Velocity.X, _Body.Velocity.Y), Vector2.Zero) < 1E-2)
            {
                // If the player stands still there should be no animation.
                _CurrentSprite.EnableAnimation = false;
                return;
            }

            // Determine which sprite should be drawn.
            _CurrentSprite = GetCurrentSprite(GetDirection());

            // Make the sprite visible and enable its animation.
            _CurrentSprite.Visibility = Visibility.Visible;
            _CurrentSprite.EnableAnimation = true;

            // For all other sprites, make them invisible and disable their animation.
            foreach (Sprite s in _Sprites.Sprites)
            {
                if (_CurrentSprite != s)
                {
                    s.Visibility = Visibility.Invisible;
                    s.EnableAnimation = false;
                }
            }
        }
        /// <summary>
        /// Get the current sprite based on player velocity direction.
        /// </summary>
        /// <param name="dir">The direction of the player.</param>
        /// <returns>The current sprite.</returns>
        private Sprite GetCurrentSprite(float dir)
        {
            // If facing down.
            if (dir >= 60 && dir <= 120)
            {
                return _Sprites[0];
            }
            // If facing up.
            else if (dir >= -120 && dir <= -30)
            {
                return _Sprites[1];
            }
            // If facing right.
            else if (dir >= -30 && dir <= 30)
            {
                return _Sprites[2];
            }
            // If facing left.
            else if ((dir >= 150 && dir <= 180) || (dir >= -180 && dir <= -120)) { return _Sprites[3]; }

            // No sprite matched.
            return _CurrentSprite;
        }
        /// <summary>
        /// Get the direction of the player in degrees. If standing still, it will always point left.
        /// </summary>
        /// <returns>The direction of the player.</returns>
        private float GetDirection()
        {
            return Calculator.GetAngle(new Vector2(_Body.Velocity.X, _Body.Velocity.Y), Vector2.Zero) * (180 / (float)Math.PI);
        }
        #endregion
    }
}