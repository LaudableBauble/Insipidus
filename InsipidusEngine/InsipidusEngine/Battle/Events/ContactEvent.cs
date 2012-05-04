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

using InsipidusEngine.Battle.Actions;
using InsipidusEngine.Imagery;

namespace InsipidusEngine.Battle.Events
{
    /// <summary>
    /// A contact event is used when an attack requires physical contact between the participants.
    /// </summary>
    public class ContactEvent : TimelineEvent
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
        public ContactEvent(Timeline timeline, float start, float end, AnimationRule rule, TimelineEventType type, TimelineEvent dependentOn)
        {
            Initialize(start, end, rule, type, dependentOn);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the timeline event.
        /// </summary>
        /// <param name="start">The start of the event.</param>
        /// <param name="end">The end of the event.</param>
        /// <param name="rule">The event that will happen.</param>
        /// <param name="type">The type of event, ie. duration or action-specific.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        private void Initialize(float start, float end, AnimationRule rule, TimelineEventType type, TimelineEvent dependentOn)
        {
            //Initialize the variables.
            _StartTime = start;
            _EndTime = end;
            _Event = rule;
            _Type = type;
            _DependentOn = dependentOn;
            _State = TimelineEventState.Idle;

            //If this event is dependent upon someone else, subscribe to it.
            if (_DependentOn != null) { _DependentOn.OnEnded += OnDependentEnded; }
        }
        /// <summary>
        /// Update the timeline event.
        /// </summary>
        /// <param name="timeline">The timeline that this event belongs to.</param>
        protected override void Update(Timeline timeline)
        {
            //If the event is not active, stop here.
            if (_State != TimelineEventState.Active) { return; }

            //The elapsed time in regard to this event's starting time.
            float elapsedTime = GetElapsedTime(timeline.ElapsedTime);

            //If the elapsed time is less than 0, quit.
            if (elapsedTime < 0) { return; }

            //Perform the event.
            switch (_Event)
            {
                case AnimationRule.DamageTarget: { break; }
                case AnimationRule.EndAnimation: { break; }
                case AnimationRule.MoveToTarget: { break; }
                case AnimationRule.MoveToUser: { break; }
                default: { break; }
            }
        }
        #endregion

        #region Properties
        #endregion
    }
}
