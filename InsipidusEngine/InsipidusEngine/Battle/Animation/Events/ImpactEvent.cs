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

namespace InsipidusEngine.Battle.Animation.Events
{
    /// <summary>
    /// This event is used when someone should suffer from an impact.
    /// </summary>
    public class ImpactEvent : TimelineEvent
    {
        #region Fields
        private BattleMove _Move;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a recoil event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="move">The move to calculate impact from.</param>
        public ImpactEvent(Timeline timeline, float start, TimelineEvent dependentOn, BattleMove move)
        {
            Initialize(timeline, start, dependentOn, move);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="move">The move to calculate impact from.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn, BattleMove move)
        {
            //Call the base method.
            base.Initialize(timeline, start, dependentOn);

            //Initialize the variables.
            _Move = move;
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

            //Calculate the impact of the attack.
            Vector2 impact = _Move.User.Velocity * _Move.Force;

            //Modify the target's state of battle.
            _Move.Target.RecieveImpact(impact);
            EventConcludedInvoke();

            //The event has been performed.
            return true;
        }
        #endregion
    }
}
