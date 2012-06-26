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
    /// This event is used when someone's energy needs to be modified.
    /// </summary>
    public class ModifyEnergyEvent : TimelineEvent
    {
        #region Fields
        private Creature _Character;
        private float _Amount;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="character">The character whos energy will be modified.</param>
        /// <param name="amount">The amount to add to the energy.</param>
        public ModifyEnergyEvent(Timeline timeline, float start, TimelineEvent dependentOn, Creature character, float amount)
        {
            Initialize(timeline, start, dependentOn, character, amount);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="character">The character whos energy will be modified.</param>
        /// <param name="amount">The amount to add to the energy.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn, Creature character, float amount)
        {
            //Call the base method.
            base.Initialize(timeline, start, dependentOn);

            //Initialize the variables.
            _Character = character;
            _Amount = amount;
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

            //Modify the target's energy.
            _Character.CurrentEnergy += _Amount;
            EventConcludedInvoke();

            //The event has been performed.
            return true;
        }
        #endregion
    }
}
