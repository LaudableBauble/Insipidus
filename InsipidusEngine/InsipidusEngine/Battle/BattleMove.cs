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
using InsipidusEngine.Battle.Animation.Events;
using InsipidusEngine.Imagery;

namespace InsipidusEngine.Battle
{
    /// <summary>
    /// A battle move is simply an activated form of a move and is carried out by two parties in a battle; an attacker and a defender.
    /// </summary>
    public class BattleMove
    {
        #region Fields
        private Move _Move;
        private Timeline _Timeline;
        private Creature _User;
        private Creature _Target;
        private bool _IsCancelable;
        private bool _HasUserControl;
        private bool _HasTargetControl;
        #endregion

        #region Events
        public delegate void MoveEventHandler(BattleMove source);
        public event MoveEventHandler Concluded;
        #endregion

        #region Constructors
        /// <summary>
        /// Create the activated form for a given move.
        /// </summary>
        /// <param name="move">The move to use in battle.</param>
        /// <param name="user">The user of the move.</param>
        /// <param name="target">The target for the move.</param>
        public BattleMove(Move move, Creature user, Creature target)
        {
            //Initialize a few things.
            Initialize(move, user, target);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the active move.
        /// </summary>
        /// <param name="move">The move to use in battle.</param>
        /// <param name="user">The user of the move.</param>
        /// <param name="target">The target for the move.</param>
        protected void Initialize(Move move, Creature user, Creature target)
        {
            //Initialize the class.
            _Move = move;
            _User = user;
            _Target = target;
            _IsCancelable = true;
            _HasUserControl = true;
            _HasTargetControl = false;
            _Timeline = Factory.Instance.createBattleAnimation(this);

            //Subscribe to the timeline.
            _Timeline.OnConcluded += ConcludedInvoke;
        }
        /// <summary>
        /// Load the timeline's content.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public virtual void LoadContent(ContentManager content)
        {
            //Load the timeline's content.
            _Timeline.LoadContent(content);
        }
        /// <summary>
        /// Update the active move.
        /// </summary>
        /// <param name="gametime">The current game time.</param>
        public void Update(GameTime gametime)
        {
            //Update the timeline.
            _Timeline.Update(gametime);
        }
        /// <summary>
        /// Draw the timeline.
        /// </summary>
        /// <param name="spritebatch">The sprite batch to use.</param>
        public virtual void Draw(SpriteBatch spritebatch)
        {
            //Draw the timeline.
            _Timeline.Draw(spritebatch);
        }

        /// <summary>
        /// Use the move on a character by activating it.
        /// </summary>
        public void Activate()
        {
            //If the move has not been set-up properly or if its already underway, we cannot activate it again.
            if (_User == null || _Target == null || _Timeline.State != TimelineState.Idle) { throw new Exception("The move has either already been activated or it has not been set-up properly."); }

            //Start the aniamtion.
            _Timeline.Start();

            //Update the user's and target's state.
            BattleCoordinator.Instance.UpdateControlState(_User);
            BattleCoordinator.Instance.UpdateControlState(_Target);
        }
        /// <summary>
        /// Cancel the move and end it prematurely.
        /// </summary>
        public void Cancel()
        {
            //Cancel the move, if we can.
            if (!_IsCancelable) { return; }

            //Stop the animation.
            UpdateControlState(false, false);
            _Timeline.Stop();
        }
        /// <summary>
        /// Get this move's damage capabilities. Note that the calculation is partly based upon random values, which means that successive calls
        /// to this method will yield different results.
        /// </summary>
        /// <returns>The damage of this move.</returns>
        public float GetDamage()
        {
            //The STAB, weakness/resistance factor and a random value.
            float STAB = 1;
            float wrFactor = 1;
            float random = Calculator.RandomNumber(85, 100);

            //Calculate the damage of the move.
            float damagePhysical = (((((2 * _User.Level / 5) + 2) * _User.AttackPhysical * PowerPhysical / _Target.DefensePhysical) / 50) + 2) * STAB * wrFactor * random / 100;
            float damageSpecial = (((((2 * _User.Level / 5) + 2) * _User.SpecialAttack * PowerSpecial / _Target.SpecialDefense) / 50) + 2) * STAB * wrFactor * random / 100;

            //Calculate the cleanliness and force of the attack.
            float hitCleanliness = MathHelper.Clamp(_User.Speed / _Target.Speed, 0, 1) * Calculator.RandomNumber(Accuracy / 100, 1);

            //Damage the Pokémon and subtract from its health.
            return (damagePhysical + damageSpecial) * hitCleanliness;
        }
        /// <summary>
        /// Get whether the move will hit the target or not. It is the target's responsibility to check whether the attack will hit or not.
        /// Note that the calculation is partly based upon random values, which means that successive calls to this method will yield different results.
        /// </summary>
        /// <returns>The outcome of the move, true if the move is going to hit.</returns>
        public bool GetOutcome()
        {
            //Whether the move hit or clashed.
            return (MathHelper.Clamp(_User.Speed / _Target.Speed, 0, 1) * Calculator.RandomNumber(Accuracy / 100, 1)) >= .5f;
        }
        /// <summary>
        /// Update the move's state of control.
        /// </summary>
        /// <param name="hasUserControl">Whether the move has control over the user.</param>
        /// <param name="hasTargetControl">Whether the move has control over thet target.</param>
        private void UpdateControlState(bool hasUserControl, bool hasTargetControl)
        {
            //If the user control has changed.
            if (_HasUserControl != hasUserControl)
            {
                //Update the control state, both for the move and the user.
                _HasUserControl = hasUserControl;
                BattleCoordinator.Instance.UpdateControlState(_User);
            }

            //If the target control has changed.
            if (_HasTargetControl != hasTargetControl)
            {
                //Update the control state, both for the move and the target.
                _HasTargetControl = hasTargetControl;
                BattleCoordinator.Instance.UpdateControlState(_Target);
            }
        }
        /// <summary>
        /// The move's animation has been concluded, and so has this move.
        /// </summary>
        private void ConcludedInvoke()
        {
            //If someone has hooked up a delegate to the event, fire it.
            if (Concluded != null) { Concluded(this); }
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return _Move.Name; }
            set { _Move.Name = value; }
        }
        public string Description
        {
            get { return _Move.Description; }
            set { _Move.Description = value; }
        }
        public int PowerPhysical
        {
            get { return _Move.PowerPhysical; }
            set { _Move.PowerPhysical = value; }
        }
        public int PowerSpecial
        {
            get { return _Move.PowerSpecial; }
            set { _Move.PowerSpecial = value; }
        }
        public int Accuracy
        {
            get { return _Move.Accuracy; }
            set { _Move.Accuracy = value; }
        }
        public int EnergyConsume
        {
            get { return _Move.EnergyConsume; }
            set { _Move.EnergyConsume = value; }
        }
        public Status Status
        {
            get { return _Move.Status; }
            set { _Move.Status = value; }
        }
        public List<PokemonType> Types
        {
            get { return _Move.Types; }
            set { _Move.Types = value; }
        }
        public float Force
        {
            get { return _Move.Force; }
            set { _Move.Force = value; }
        }
        public Move Move
        {
            get { return _Move; }
            set { _Move = value; }
        }
        public Creature User
        {
            get { return _User; }
            set { _User = value; }
        }
        public Creature Target
        {
            get { return _Target; }
            set { _Target = value; }
        }
        public TimelineState State
        {
            get { return _Timeline.State; }
        }
        /// <summary>
        /// Whether the move can be cancelled or not.
        /// </summary>
        public bool IsCancelable
        {
            get { return _IsCancelable; }
            set { _IsCancelable = value; }
        }
        /// <summary>
        /// Whether the move has control over the user or not.
        /// </summary>
        public bool HasUserControl
        {
            get { return _HasUserControl; }
            set { UpdateControlState(value, _HasTargetControl); }
        }
        /// <summary>
        /// Whether the move has control over the target or not.
        /// </summary>
        public bool HasTargetControl
        {
            get { return _HasTargetControl; }
            set { UpdateControlState(_HasUserControl, value); }
        }
        #endregion
    }
}
