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
    /// A damage event is used when an attack is going to deal out some damage.
    /// </summary>
    public class DamageEvent : TimelineEvent
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
        public DamageEvent(Timeline timeline, float start, float end, AnimationRule rule, TimelineEventType type, TimelineEvent dependentOn)
        {
            Initialize(timeline, start, end, rule, type, dependentOn);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Perform this event.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the beginning of this event.</param>
        protected override void PerformEvent(float elapsedTime)
        {
            //Call the base method.
            base.PerformEvent(elapsedTime);

            //Damage the target.
            Target.ReceiveAttack(_Timeline.Move);
            EventConcludedInvoke();
        }
        #endregion
    }
}
