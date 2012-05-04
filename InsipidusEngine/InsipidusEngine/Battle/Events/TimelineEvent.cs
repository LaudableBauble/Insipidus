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
    /// A timeline event is an animation rule that wants to be applied at a certain time in a timeline. Inherit from this class to build specific battle events.
    /// </summary>
    public abstract class TimelineEvent
    {
        #region Fields
        protected Timeline _Timeline;
        protected float _StartTime;
        protected float _EndTime;
        protected AnimationRule _Event;
        protected TimelineEventState _State;
        protected TimelineEventType _Type;
        protected TimelineEvent _DependentOn;
        #endregion

        #region Events
        public delegate void TimelineEventHandler(TimelineEvent eventRule);
        public event TimelineEventHandler OnStarted;
        public event TimelineEventHandler OnEnded;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the timeline event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="end">The end of the event.</param>
        /// <param name="rule">The event that will happen.</param>
        /// <param name="type">The type of event, ie. duration or action-specific.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        protected virtual void Initialize(Timeline timeline, float start, float end, AnimationRule rule, TimelineEventType type, TimelineEvent dependentOn)
        {
            //Initialize the variables.
            _Timeline = timeline;
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
        protected virtual void Update(Timeline timeline)
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

        /// <summary>
        /// Get the elapsed time in relation to this event.
        /// </summary>
        /// <param name="elapsedTime">The timeline's elapsed time.</param>
        /// <returns>The elapsed time in relation to this event.</returns>
        protected virtual float GetElapsedTime(float elapsedTime)
        {
            //If the event is dependent upon another event, calculate the elapsed time in relation to that event.
            if (_DependentOn != null)
            {
                //If that event is not concluded yet, we have to wait.
                if (_DependentOn.State != TimelineEventState.Concluded) { return -1; }

                //Calculate the elapsed time.
                return elapsedTime - _DependentOn.EndTime;
            }

            //Return the elapsed time since the beginning of this event.
            return elapsedTime - _StartTime;
        }
        /// <summary>
        /// Activate this event if the one we have been dependent upon has ended.
        /// </summary>
        /// <param name="eventRule">The event that has ended.</param>
        protected virtual void OnDependentEnded(TimelineEvent eventRule)
        {
            //Set the state to active.
            _State = TimelineEventState.Active;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The start time of this event.
        /// </summary>
        public float StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        /// <summary>
        /// The end time of this event.
        /// </summary>
        public float EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
        /// <summary>
        /// The rule that defines this event.
        /// </summary>
        public AnimationRule Event
        {
            get { return _Event; }
            set { _Event = value; }
        }
        /// <summary>
        /// What state the event is currently in, ie. idle, active or concluded.
        /// </summary>
        public TimelineEventState State
        {
            get { return _State; }
            set { _State = value; }
        }
        /// <summary>
        /// The duration of the event.
        /// </summary>
        public float Duration
        {
            get { return _EndTime - _StartTime; }
        }
        /// <summary>
        /// The event this is dependent on, ie. has to wait for. Null if this event is independent.
        /// </summary>
        public TimelineEvent DependentOn
        {
            get { return _DependentOn; }
            set { _DependentOn = value; }
        }
        #endregion
    }
}
