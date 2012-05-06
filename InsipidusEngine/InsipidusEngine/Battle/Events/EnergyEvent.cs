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

namespace InsipidusEngine.Battle.Events
{
    /// <summary>
    /// An energy event is used when an attack is altering the amount of energy of any participant.
    /// </summary>
    public class EnergyEvent : TimelineEvent
    {
        #region Constructors
        /// <summary>
        /// Constructor for a timeline event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="end">The end of the event.</param>
        /// <param name="rule">The event rule that will be applied.</param>
        /// <param name="type">The type of event, ie. duration or action-specific.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        public EnergyEvent(Timeline timeline, float start, float end, AnimationRule rule, TimelineEventType type, TimelineEvent dependentOn)
        {
            Initialize(timeline, start, end, rule, type, dependentOn);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Perform this event.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the beginning of this event.</param>
        /// <returns>Whether the event was performed this cycle or not.</returns>
        protected override bool PerformEvent(float elapsedTime)
        {
            //Call the base method and see whether to perform the event or not.
            if (!base.PerformEvent(elapsedTime)) { return false; }

            //The move consumes energy for the user.
            User.CurrentEnergy -= _Timeline.Move.EnergyConsume;
            EventConcludedInvoke();

            //The event has been performed.
            return true;
        }
        #endregion
    }
}
