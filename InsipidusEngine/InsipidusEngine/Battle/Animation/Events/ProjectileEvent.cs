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
using InsipidusEngine.Helpers;
using InsipidusEngine.Battle.Projectiles;

namespace InsipidusEngine.Battle.Animation.Events
{
    /// <summary>
    /// A projectile event is used when an attack requires some sort of projectile to be launched.
    /// </summary>
    public class ProjectileEvent : TimelineEvent
    {
        #region Fields
        protected Vector2 _Position;
        protected Destination _Destination;
        protected Character _Target;
        protected Projectile _Projectile;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a projectile event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="position">The starting position of the projectile.</param>
        /// <param name="destination">The destination point of the projectile.</param>
        public ProjectileEvent(Timeline timeline, float start, TimelineEvent dependentOn, Vector2 position, Destination destination)
        {
            Initialize(timeline, start, dependentOn, position, destination);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the projectile event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="position">The starting position of the projectile.</param>
        /// <param name="destination">The destination point of the projectile.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn, Vector2 position, Destination destination)
        {
            //Call the base method.
            base.Initialize(timeline, start, dependentOn);

            //Initialize the variables.
            _Position = position;
            _Destination = destination;
            _Projectile = new FlameProjectile(_Position, _Destination);

            //Subscribe to the projectile's collision event.
            _Projectile.OnCollision += OnProjectileCollided;
        }
        /// <summary>
        /// Load the projectile event's content.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public override void LoadContent(ContentManager content)
        {
            //Load the projectile's content.
            _Projectile.LoadContent(content);
        }
        /// <summary>
        /// Draw the timeline event.
        /// </summary>
        /// <param name="spritebatch">The sprite batch to use.</param>
        public override void Draw(SpriteBatch spritebatch)
        {
            //Call the base method.
            base.Draw(spritebatch);

            //If the event has been concluded, stop here.
            if (_State == TimelineState.Concluded) { return; }

            //Draw the projectile.
            _Projectile.Draw(spritebatch);
        }

        /// <summary>
        /// Perform this event.
        /// </summary>
        /// <param name="gametime">The elapsed game time.</param>
        /// <param name="elapsedTime">The elapsed time since the beginning of this event.</param>
        /// <returns>Whether the event was performed this cycle or not.</returns>
        protected override bool PerformEvent(GameTime gametime, float elapsedTime)
        {
            //Call the base method and see whether to perform the event or not.
            if (!base.PerformEvent(gametime, elapsedTime)) { return false; }

            //Update the projectile.
            _Projectile.Update(gametime);

            //The event has been performed.
            return true;
        }
        /// <summary>
        /// End this event when the projectile has collided with something.
        /// </summary>
        /// <param name="projectile">The projectile in question.</param>
        /// <param name="character">The character which the projectile collided with.</param>
        protected void OnProjectileCollided(Projectile projectile, Character character)
        {
            //Conclude this event.
            EventConcludedInvoke();
        }
        #endregion
    }
}
