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
    /// This event is used when the move's state of cancelability needs to be modified.
    /// </summary>
    public class ModifyCancelableEvent : TimelineEvent
    {
        #region Fields
        private BattleMove _Move;
        private bool _IsCancelable;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="move">The move who will be modified.</param>
        /// <param name="isCancelable">Whether the move can be cancelled or not.</param>
        public ModifyCancelableEvent(Timeline timeline, float start, TimelineEvent dependentOn, BattleMove move, bool isCancelable)
        {
            Initialize(timeline, start, dependentOn, move, isCancelable);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the modify event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="move">The move who will be modified.</param>
        /// <param name="isCancelable">Whether the move can be cancelled or not.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn, BattleMove move, bool isCancelable)
        {
            //Call the base method.
            base.Initialize(timeline, start, dependentOn);

            //Initialize the variables.
            _Move = move;
            _IsCancelable = isCancelable;
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

            //Modify the moves's state of cancelability.
            _Move.IsCancelable = _IsCancelable;
            EventConcludedInvoke();

            //The event has been performed.
            return true;
        }
        #endregion
    }
}
