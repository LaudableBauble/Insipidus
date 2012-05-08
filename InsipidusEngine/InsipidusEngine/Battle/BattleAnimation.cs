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

using InsipidusEngine.Battle.Animation;
using InsipidusEngine.Imagery;

namespace InsipidusEngine.Battle
{
    /// <summary>
    /// A battle animation is what makes battles visible to the naked eye. It is basically a set of rules dictating what a move will look like
    /// and a method to play through it.
    /// </summary>
    public class BattleAnimation
    {
        #region Fields
        protected Character _User;
        protected Character _Target;
        protected Timeline _Timeline;
        #endregion

        #region Events
        public delegate void BattleAnimationEventHandler(Timeline source);
        public event BattleAnimationEventHandler AnimationCompleted;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a battle animation.
        /// </summary>
        /// <param name="move">The move to use in battle.</param>
        public BattleAnimation(BattleMove move)
        {
            //Initialize a few things.
            Initialize(move);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the battle animation.
        /// </summary>
        /// <param name="move">The move to use in battle.</param>
        protected void Initialize(BattleMove move)
        {
            //Initialize the class.
            _Timeline = new Timeline(move);
        }
        public void LoadContent(ContentManager content)
        {

        }
        /// <summary>
        /// Update the battle animation.
        /// </summary>
        /// <param name="gametime">The current game time.</param>
        public void Update(GameTime gametime)
        {
            //Update the timeline.
            _Timeline.Update(gametime);
        }

        /// <summary>
        /// Starts the battle animation.
        /// </summary>
        public void StartAnimation()
        {
            _Timeline.Start();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The user of the battle animation.
        /// </summary>
        public Character User
        {
            get { return _User; }
            set { _User = value; }
        }
        /// <summary>
        /// The target of the battle animation.
        /// </summary>
        public Character Target
        {
            get { return _Target; }
            set { _Target = value; }
        }
        #endregion
    }
}
