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
    /// A timeline is a finite set of time during which a number of different animation events may occur.
    /// </summary>
    public class Timeline
    {
        #region Fields
        private BattleMove _Move;
        private float _ElapsedTime;
        private List<TimelineEvent> _Events;
        private TimelineState _State;
        #endregion

        #region Events
        public delegate void TimelineEventHandler();
        public event TimelineEventHandler OnConcluded;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a timeline.
        /// </summary>
        /// <param name="move">The battle move to execute.</param>
        public Timeline(BattleMove move)
        {
            Initialize(move);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the timeline.
        /// </summary>
        /// <param name="move">The battle move to execute.</param>
        private void Initialize(BattleMove move)
        {
            //Initialize the fields.
            _Move = move;
            _ElapsedTime = 0;
            _Events = new List<TimelineEvent>();
            _State = TimelineState.Idle;
        }
        /// <summary>
        /// Update the timeline.
        /// </summary>
        /// <param name="gametime">The current game time.</param>
        public void Update(GameTime gametime)
        {
            //If the timeline is not active, stop here.
            if (_State != TimelineState.Active) { return; }

            //Get the time since the last update.
            _ElapsedTime += (float)gametime.ElapsedGameTime.TotalSeconds;

            //Update all events.
            _Events.ForEach(item => item.Update(this));
        }

        /// <summary>
        /// Starts the timeline.
        /// </summary>
        public void Start()
        {
            _State = TimelineState.Active;
        }

        /// <summary>
        /// Stops the timeline.
        /// </summary>
        public void Stop()
        {
            //Stop the timeline.
            _State = TimelineState.Concluded;

            //If someone has hooked up a delegate to the event, fire it.
            if (OnConcluded != null) { OnConcluded(); }
        }

        /// <summary>
        /// Resets the timeline.
        /// </summary>
        public void Reset()
        {
            //Stops the timeline, resets the elapsed time and all events.
            Stop();
            _ElapsedTime = 0;
            _Events.ForEach(item => item.State = TimelineState.Idle);
            _State = TimelineState.Idle;
        }

        /// <summary>
        /// An event has been concluded, see if the timeline has ended with it.
        /// </summary>
        /// <param name="e">The event that has been concluded.</param>
        private void OnEventConcluded(TimelineEvent e)
        {
            //If this was the last active event, conclude the timeline.
            if (!_Events.Exists(item => item.State != TimelineState.Concluded)) { Stop(); }
        }
        /// <summary>
        /// Add an event to the timeline.
        /// </summary>
        /// <param name="eventRule">The event to add.</param>
        public void AddEvent(TimelineEvent eventRule)
        {
            //Add the event and subscribe to its events.
            _Events.Add(eventRule);
            eventRule.OnConcluded += OnEventConcluded;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The battle move this timeline tries to express.
        /// </summary>
        public BattleMove Move
        {
            get { return _Move; }
            set { _Move = value; }
        }
        /// <summary>
        /// The user of the move.
        /// </summary>
        public Character User
        {
            get { return _Move.User; }
            set { _Move.User = value; }
        }
        /// <summary>
        /// The target of the move.
        /// </summary>
        public Character Target
        {
            get { return _Move.Target; }
            set { _Move.Target = value; }
        }
        /// <summary>
        /// The elapsed time.
        /// </summary>
        public float ElapsedTime
        {
            get { return _ElapsedTime; }
            set { _ElapsedTime = value; }
        }
        /// <summary>
        /// The state of this timeline.
        /// </summary>
        public TimelineState State
        {
            get { return _State; }
        }
        #endregion
    }
}
