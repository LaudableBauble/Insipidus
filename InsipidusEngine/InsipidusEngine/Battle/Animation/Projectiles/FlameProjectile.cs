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
    /// A flame projectile is a single point of flame launched in a direction.
    /// </summary>
    public class FlameProjectile : Projectile
    {
        #region Constructors
        /// <summary>
        /// Constructor for a flame projectile.
        /// </summary>
        /// <param name="position">The starting position of the projectile.</param>
        /// <param name="destination">The destination of this projectile.</param>
        public FlameProjectile(Vector2 position, Vector2 destination)
        {
            Initialize(position, destination);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load the projectile's content.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public override void LoadContent(ContentManager content)
        {
            //Call the base method.
            base.LoadContent(content);

            //Add a flame sprite to the collection.
            _Sprite.AddSprite(new Sprite(_Sprite, @"Battle\Misc\Flame[1]", _Position, 1, 1, 0, 0, 0, ""));
        }
        #endregion
    }
}
