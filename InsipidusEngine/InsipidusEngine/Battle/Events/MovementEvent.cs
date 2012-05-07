﻿using System;
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
    /// A movement event is used when an attack requires some sort of movement from either participant, for example if one is going to move into contact with the other.
    /// Beware that this event will not resolve any collisions.
    /// </summary>
    public class MovementEvent : TimelineEvent
    {
        #region Fields
        protected Character _Character;
        protected Vector2 _Destination;
        protected MovementType _Type;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a movement event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="character">The character to move.</param>
        /// <param name="destination">The destination point of the movement.</param>
        /// <param name="type">The type movement.</param>
        public MovementEvent(Timeline timeline, float start, TimelineEvent dependentOn, Character character, Vector2 destination, MovementType type)
        {
            Initialize(timeline, start, dependentOn, character, destination, type);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the movement event.
        /// </summary>
        /// <param name="timeline">The timeline this event is part of.</param>
        /// <param name="start">The start of the event.</param>
        /// <param name="dependentOn">An optional event to be dependent upon, ie. wait for.</param>
        /// <param name="character">The character to move.</param>
        /// <param name="destination">The destination point of the movement.</param>
        /// <param name="type">The type movement.</param>
        protected virtual void Initialize(Timeline timeline, float start, TimelineEvent dependentOn, Character character, Vector2 destination, MovementType type)
        {
            //Call the base method.
            base.Initialize(timeline, start, dependentOn);

            //Initialize the variables.
            _Character = character;
            _Destination = destination;
            _Type = type;
        }
        /// <summary>
        /// Perform this event.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the beginning of this event.</param>
        /// <returns>Whether the event was performed this cycle or not.</returns>
        protected override bool PerformEvent(float elapsedTime)
        {
            //Call the base method and see whether to perform the event or not.
            if (!base.PerformEvent(elapsedTime)) { return false; }

            //The speed with which to move.
            float speed = 0;

            //Move differently depending on the type of the movement.
            switch (_Type)
            {
                case MovementType.Run:
                    {
                        //Check if the event has run its course.
                        if (Vector2.Distance(_Character.Position, _Destination) < 10) { EventConcludedInvoke(); }

                        //Move towards the target as fast as possible.
                        speed = User.Speed;
                        break;
                    }
                default: { break; }
            }

            //Move towards the target.
            User.Velocity = Calculator.LineDirection(_Character.Position, _Destination) * speed;

            //The event has been performed.
            return true;
        }
        #endregion
    }
}
