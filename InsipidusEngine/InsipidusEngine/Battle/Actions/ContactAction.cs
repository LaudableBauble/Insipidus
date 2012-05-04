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

using InsipidusEngine.Battle;
using InsipidusEngine.Imagery;

namespace InsipidusEngine.Battle.Actions
{
    /// <summary>
    /// A contact action is used when an attack requires physical contact between the participants.
    /// </summary>
    public abstract class ContactAction : BattleAction
    {
        #region Constructors
        /// <summary>
        /// Constructor for a contact action.
        /// </summary>
        /// <param name="user">The user of the event.</param>
        /// <param name="target">The target for the event.</param>
        public ContactAction(Character user, Character target)
        {
            //Store the arguments.
            _User = user;
            _Target = target;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the battle action.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the beginning of this battle action.</param>
        protected override void Update(float elapsedTime)
        {
            //Move the user towards the target.
            _User.Velocity = Calculator.LineDirection(_User.Position, _Target.Position) * _User.Speed * .05f;
        }
        #endregion
    }
}
