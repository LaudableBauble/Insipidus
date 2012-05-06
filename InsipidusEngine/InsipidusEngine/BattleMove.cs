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
using InsipidusEngine.Battle.Events;
using InsipidusEngine.Imagery;

namespace InsipidusEngine
{
    /// <summary>
    /// A battle move is simply an activated form of a move and is carried out by two parties in a battle; an attacker and a defender.
    /// </summary>
    public class BattleMove
    {
        #region Fields
        private Move _Move;
        private AttackState _AttackState;
        private AttackOutcome _AttackOutcome;
        private Timeline _Timeline;
        private Character _User;
        private Character _Target;
        private bool _IsCancelable;
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
        public BattleMove(Move move, Character user, Character target)
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
        public void Initialize(Move move, Character user, Character target)
        {
            //Initialize the class.
            _Move = move;
            _AttackState = AttackState.Idle;
            _AttackOutcome = AttackOutcome.None;
            _Timeline = new Timeline(this);
            _User = user;
            _Target = target;
            _IsCancelable = true;

            //Add events to the timeline.
            TimelineEvent moveTo = new ContactEvent(_Timeline, 0, 0, AnimationRule.MoveToTarget, TimelineEventType.Direct, null);
            TimelineEvent damage = new DamageEvent(_Timeline, 0, 0, AnimationRule.DamageTarget, TimelineEventType.Direct, moveTo);
            TimelineEvent energy = new EnergyEvent(_Timeline, 0, 0, AnimationRule.ConsumeEnergy, TimelineEventType.Direct, damage);
            _Timeline.AddEvent(moveTo);
            _Timeline.AddEvent(damage);
            _Timeline.AddEvent(energy);

            //Start the timeline.
            _Timeline.Start();

            //Subscribe to the timeline.
            _Timeline.OnConcluded += ConcludedInvoke;
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
        /// Use the move on a character by activating it.
        /// </summary>
        /// <param name="outcome">The outcome of the attack.</param>
        public void Activate(AttackOutcome outcome)
        {
            //If the move has not been set-up properly or if its already underway, we cannot activate it again.
            if (_User == null || _Target == null || _AttackState != AttackState.Idle) { throw new Exception("The move has either already been activated or it has not been set-up properly."); }

            //Get the outcome of the move and then activate it.
            _AttackOutcome = outcome;
            _AttackState = AttackState.Underway;
        }
        /// <summary>
        /// Cancel the move and end it prematurely.
        /// </summary>
        public void Cancel()
        {
            //Cancel the move, if we can.
            if (_IsCancelable) { _AttackState = AttackState.Cancelled; }
        }
        /// <summary>
        /// The move has been concluded.
        /// </summary>
        protected void ConcludedInvoke()
        {
            //The attack is now officially finished.
            _AttackState = AttackState.Concluded;

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
        public Character User
        {
            get { return _User; }
            set { _User = value; }
        }
        public Character Target
        {
            get { return _Target; }
            set { _Target = value; }
        }
        public AttackState State
        {
            get { return _AttackState; }
        }
        public AttackOutcome Outcome
        {
            get { return _AttackOutcome; }
        }
        #endregion
    }
}
