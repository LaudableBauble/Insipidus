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
    /// This event is used when a move's state of control over either the user or the target needs to be modified.
    /// </summary>
    public class ModifyControlEvent : TimelineEvent
    {
        #region Fields
        private BattleMove _Move;
        private Character _Character;
        private bool _HasControl;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="move">The move to modify.</param>
        /// <param name="character">The character whos state of control will be modified.</param>
        /// <param name="control">The new state of control.</param>
        public ModifyControlEvent(Timeline timeline, float start, TimelineEvent dependentOn, BattleMove move, Character character, bool control)
        {
            Initialize(timeline, start, dependentOn, move, character, control);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="move">The move to modify.</param>
        /// <param name="character">The character whos state of control will be modified.</param>
        /// <param name="control">The new state of control.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn, BattleMove move, Character character, bool control)
        {
            //Call the base method.
            base.Initialize(timeline, start, dependentOn);

            //Initialize the variables.
            _Move = move;
            _Character = character;
            _HasControl = control;
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

            //Modify the move's state of control.
            if (_Move.User == _Character) { _Move.HasUserControl = _HasControl; }
            else if (_Move.Target == _Character) { _Move.HasTargetControl = _HasControl; }

            //Conclude the event.
            EventConcludedInvoke();

            //The event has been performed.
            return true;
        }
        #endregion
    }
}
