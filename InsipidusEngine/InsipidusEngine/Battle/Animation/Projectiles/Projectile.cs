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

namespace InsipidusEngine.Battle.Projectiles
{
    /// <summary>
    /// A projectile is used by an attack as an animation event during battle and takes the form of a propelled force.
    /// Inherit from this class to build custom projectile events.
    /// </summary>
    public abstract class Projectile
    {
        #region Fields
        protected SpriteManager _Sprite;
        protected Vector2 _Position;
        protected Vector2 _Destination;
        protected Vector2 _Velocity;
        #endregion

        #region Events
        public delegate void ProjectileEventHandler(Projectile projectile, Character character);
        public event ProjectileEventHandler OnCollision;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the projectile.
        /// </summary>
        /// <param name="position">The starting position of the projectile.</param>
        /// <param name="destination">The projectile's destination.</param>
        protected virtual void Initialize(Vector2 position, Vector2 destination)
        {
            //Initialize some fields.
            _Sprite = new SpriteManager();
            _Position = position;
            _Destination = destination;
            _Velocity = new Vector2();
        }
        /// <summary>
        /// Load the projectile's content.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public virtual void LoadContent(ContentManager content)
        {
            //Load the sprites.
            _Sprite.LoadContent(content);
        }
        /// <summary>
        /// Update the projectile.
        /// </summary>
        /// <param name="gametime">The elapsed game time.</param>
        public virtual void Update(GameTime gametime)
        {
            //Determine the velocity.
            _Velocity = Calculator.LineDirection(_Position, _Destination) * 3;

            //Move the projectile and its sprite.
            _Position += _Velocity;
            _Sprite.Update(gametime, _Position, 0);

            //Check if the projectile has collided.
            if (Vector2.Distance(_Position, _Destination) < 10) { CollisionInvoke(null); }
        }
        /// <summary>
        /// Draw the projectile.
        /// </summary>
        /// <param name="spritebatch">The spritebatch to use.</param>
        public virtual void Draw(SpriteBatch spritebatch)
        {
            //Draw the sprites.
            _Sprite.Draw(spritebatch);
        }

        /// <summary>
        /// Invoke the collision event.
        /// </summary>
        /// <param name="character">The character this projectile collided with.</param>
        protected virtual void CollisionInvoke(Character character)
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (OnCollision != null) { OnCollision(this, character); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The projectile's sprites.
        /// </summary>
        public SpriteManager Sprite
        {
            get { return _Sprite; }
            set { _Sprite = value; }
        }
        /// <summary>
        /// The projectile's position.
        /// </summary>
        public Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        /// <summary>
        /// The projectile's destination.
        /// </summary>
        public Vector2 Destination
        {
            get { return _Destination; }
            set { _Destination = value; }
        }
        /// <summary>
        /// The projectile's velocity.
        /// </summary>
        public Vector2 Velocity
        {
            get { return _Velocity; }
            set { _Velocity = value; }
        }
        #endregion
    }
}
