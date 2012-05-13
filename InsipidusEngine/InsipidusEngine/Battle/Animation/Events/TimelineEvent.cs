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
    /// A timeline event is an animation rule that wants to be applied at a certain time in a timeline. Inherit from this class to build specific battle events.
    /// </summary>
    public abstract class TimelineEvent
    {
        #region Fields
        protected Timeline _Timeline;
        protected float _StartTime;
        protected float _EndTime;
        protected TimelineState _State;
        protected EventOutcome _Outcome;
        protected TimelineEvent _DependentOn;
        #endregion

        #region Events
        public delegate void TimelineEventHandler(TimelineEvent eventRule);
        public event TimelineEventHandler OnStarted;
        public event TimelineEventHandler OnConcluded;
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the timeline event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn)
        {
            //Initialize the variables.
            _Timeline = timeline;
            _StartTime = start;
            _EndTime = start;
            _DependentOn = dependentOn;
            _Outcome = EventOutcome.None;
            _State = TimelineState.Idle;

            //If this event is dependent upon someone else, subscribe to it.
            if (_DependentOn != null) { _DependentOn.OnConcluded += OnDependentEnded; }
        }
        /// <summary>
        /// Load the timeline event's content.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public virtual void LoadContent(ContentManager content)
        {

        }
        /// <summary>
        /// Update the timeline event.
        /// </summary>
        /// <param name="gametime">The elapsed game time.</param>
        /// <param name="timeline">The timeline that this event belongs to.</param>
        public virtual void Update(GameTime gametime, Timeline timeline)
        {
            //If the event has been concluded, stop here.
            if (_State == TimelineState.Concluded) { return; }

            //Perform the event.
            PerformEvent(gametime, GetElapsedTime(timeline.ElapsedTime));
        }
        /// <summary>
        /// Draw the timeline event.
        /// </summary>
        /// <param name="spritebatch">The sprite batch to use.</param>
        public virtual void Draw(SpriteBatch spritebatch)
        {

        }

        /// <summary>
        /// Perform this event.
        /// </summary>
        /// <param name="gametime">The elapsed game time.</param>
        /// <param name="elapsedTime">The elapsed time since the beginning of this event.</param>
        /// <returns>Whether the event was performed this cycle or not.</returns>
        protected virtual bool PerformEvent(GameTime gametime, float elapsedTime)
        {
            //If the elapsed time is less than 0, quit.
            if (_State == TimelineState.Concluded || elapsedTime < 0) { return false; }

            //Activate the event.
            _State = TimelineState.Active;

            //The event was performed.
            return true;
        }
        /// <summary>
        /// Get the elapsed time in relation to this event. A negative number indicates that this event has yet to see the light of day.
        /// </summary>
        /// <param name="elapsedTime">The timeline's elapsed time.</param>
        /// <returns>The elapsed time in relation to this event.</returns>
        protected virtual float GetElapsedTime(float elapsedTime)
        {
            //If the event is dependent upon another event, calculate the elapsed time in relation to that event.
            if (_DependentOn != null)
            {
                //If that event is not concluded yet, we have to wait.
                if (_DependentOn.State != TimelineState.Concluded) { return -1; }

                //Calculate the elapsed time.
                return elapsedTime - _DependentOn.EndTime;
            }

            //Return the elapsed time since the beginning of this event.
            return elapsedTime - _StartTime;
        }
        /// <summary>
        /// This timeline event has now ended.
        /// </summary>
        protected void EventConcludedInvoke()
        {
            //The event has now occurred.
            _State = TimelineState.Concluded;

            //If someone has hooked up a delegate to the event, fire it.
            if (OnConcluded != null) { OnConcluded(this); }
        }
        /// <summary>
        /// Activate this event if the one we have been dependent upon has ended.
        /// </summary>
        /// <param name="eventRule">The event that has ended.</param>
        protected virtual void OnDependentEnded(TimelineEvent eventRule)
        {
            //Set the state to active.
            _State = TimelineState.Active;
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
        /// What state the event is currently in, ie. idle, active or concluded.
        /// </summary>
        public TimelineState State
        {
            get { return _State; }
            set { _State = value; }
        }
        /// <summary>
        /// The outcome of the event.
        /// </summary>
        public EventOutcome Outcome
        {
            get { return _Outcome; }
            set { _Outcome = value; }
        }
        /// <summary>
        /// The event this is dependent on, ie. has to wait for. Null if this event is independent.
        /// </summary>
        public TimelineEvent DependentOn
        {
            get { return _DependentOn; }
            set { _DependentOn = value; }
        }
        /// <summary>
        /// The user of the battle animation.
        /// </summary>
        public Character User
        {
            get { return _Timeline.User; }
        }
        /// <summary>
        /// The target of the battle animation.
        /// </summary>
        public Character Target
        {
            get { return _Timeline.Target; }
        }
        #endregion
    }
}
