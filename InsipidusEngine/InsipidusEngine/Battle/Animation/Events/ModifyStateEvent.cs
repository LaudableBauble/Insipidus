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
    /// This event is used when a move's battle state needs to be modified.
    /// </summary>
    public class ModifyStateEvent : TimelineEvent
    {
        #region Fields
        private Character _Character;
        private BattleState _ChangeState;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="character">The character who health will be modified.</param>
        /// <param name="state">The state to change into.</param>
        public ModifyStateEvent(Timeline timeline, float start, TimelineEvent dependentOn, Character character, BattleState state)
        {
            Initialize(timeline, start, dependentOn, character, state);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="character">The character who health will be modified.</param>
        /// <param name="state">The state to change into.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn, Character character, BattleState state)
        {
            //Call the base method.
            base.Initialize(timeline, start, dependentOn);

            //Initialize the variables.
            _Character = character;
            _ChangeState = state;
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

            //Modify the target's state of battle.
            _Character.BattleState = _ChangeState;
            EventConcludedInvoke();

            //The event has been performed.
            return true;
        }
        #endregion
    }
}
