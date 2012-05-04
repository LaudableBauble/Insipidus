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

using InsipidusEngine.Battle.Events;
using InsipidusEngine.Imagery;

namespace InsipidusEngine
{
    /// <summary>
    /// A timeline is a finite set of time during which a number of different animation events may happen.
    /// </summary>
    public class Timeline
    {
        #region Fields
        private BattleAnimation _Battle;
        private float _ElapsedTime;
        private List<TimelineEvent> _Events;
        private bool _IsRunning;
        #endregion

        #region Events
        public delegate void TimelineEventHandler(TimelineEvent eventRule);
        public event TimelineEventHandler EventOccurred;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a timeline.
        /// </summary>
        /// <param name="battle">The battle animation to execute.</param>
        public Timeline(BattleAnimation battle)
        {
            Initialize(battle);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the timeline.
        /// </summary>
        /// <param name="battle">The battle animation to execute.</param>
        private void Initialize(BattleAnimation battle)
        {
            //Initialize the fields.
            _Battle = battle;
            _ElapsedTime = 0;
            _Events = new List<TimelineEvent>();
            _IsRunning = false;
        }
        /// <summary>
        /// Update the timeline.
        /// </summary>
        /// <param name="gametime">The current game time.</param>
        public void Update(GameTime gametime)
        {
            //If the timeline has not yet been started, stop here.
            if (!_IsRunning) { return; }

            //Get the time since the last update.
            _ElapsedTime += (float)gametime.ElapsedGameTime.TotalSeconds;

            //Get the events that has yet to be activated but should be, and invoke them.
            _Events.FindAll(item => item.State != TimelineEventState.Active && item.StartTime >= _ElapsedTime).ForEach(item => EventInvoke(item));
        }

        /// <summary>
        /// Starts the timeline.
        /// </summary>
        public void Start()
        {
            _IsRunning = true;
        }

        /// <summary>
        /// Stops the timeline.
        /// </summary>
        public void Stop()
        {
            _IsRunning = false;
            _ElapsedTime = 0;
        }

        /// <summary>
        /// Invoke a timeline event.
        /// </summary>
        /// <param name="eventRule">The event to activate.</param>
        protected void EventInvoke(TimelineEvent eventRule)
        {
            //The event has now occurred.
            eventRule.State = TimelineEventState.Active;

            //If the event says stop the game, listen to it.
            if (eventRule.Event == AnimationRule.EndAnimation) { Stop(); }

            //If someone has hooked up a delegate to the event, fire it.
            if (EventOccurred != null) { EventOccurred(eventRule); }
        }

        /// <summary>
        /// Add an event to the timeline.
        /// </summary>
        /// <param name="eventRule">The event to add.</param>
        public void AddEvent(TimelineEvent eventRule)
        {
            _Events.Add(eventRule);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The elapsed time.
        /// </summary>
        public float ElapsedTime
        {
            get { return _ElapsedTime; }
            set { _ElapsedTime = value; }
        }
        #endregion
    }
}
