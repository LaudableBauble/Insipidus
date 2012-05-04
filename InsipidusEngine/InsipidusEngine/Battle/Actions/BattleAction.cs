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
    /// A battle action is a physical and visual expression of a timeline event rule. Many rules can often be applied to a single type of action.
    /// </summary>
    public abstract class BattleAction
    {
        #region Fields
        protected Character _User;
        protected Character _Target;
        #endregion

        #region Events
        public delegate void BattleActionEventHandler(BattleAction action);
        public event BattleActionEventHandler OnCompleted;
        #endregion

        #region Methods
        /// <summary>
        /// Update the battle action.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the beginning of this battle action.</param>
        protected virtual void Update(float elapsedTime)
        {

        }

        protected virtual void CompletedInvoke()
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (OnCompleted != null) { OnCompleted(this); }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The user of this battle action, ie. the one who performs it.
        /// </summary>
        public Character User
        {
            get { return _User; }
            set { _User = value; }
        }
        /// <summary>
        /// The target of this battle action.
        /// </summary>
        public Character Target
        {
            get { return _Target; }
            set { _Target = value; }
        }
        #endregion
    }
}
