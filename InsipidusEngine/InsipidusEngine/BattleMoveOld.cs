/*
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

using PokemonGame.Battle;
using PokemonGame.Imagery;

namespace PokemonGame
{
    /// <summary>
    /// A battle move is simply the activated form of a move and can thus be part of a battle.
    /// </summary>
    public class PerformedMove
    {
        #region Fields
        private Move _Move;
        private AttackState _AttackState;
        private AttackOutcome _AttackOutcome;
        private Pokemon _User;
        private Pokemon _Target;
        private bool _IsCancelable;

        public delegate void MoveEventHandler(PerformedMove source);
        public event MoveEventHandler MoveCompleted;
        public event MoveEventHandler MoveUnderway;
        public event MoveEventHandler MoveCancelled;
        #endregion

        #region Constructors
        /// <summary>
        /// Create the activated form for a given move.
        /// </summary>
        /// <param name="move">The move to use in battle.</param>
        /// <param name="user">The user of the move.</param>
        /// <param name="target">The target for the move.</param>
        public PerformedMove(Move move, Pokemon user, Pokemon target)
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
        public void Initialize(Move move, Pokemon user, Pokemon target)
        {
            //Initialize the class.
            _Move = move;
            _AttackState = AttackState.Idle;
            _AttackOutcome = AttackOutcome.None;
            _User = user;
            _Target = target;
            _IsCancelable = true;
        }
        public void LoadContent(ContentManager content)
        {

        }
        /// <summary>
        /// Update the active move.
        /// </summary>
        /// <param name="gametime">The current game time.</param>
        public void Update(GameTime gametime)
        {
            //Perform the move.
            switch (_AttackState)
            {
                case AttackState.Idle: { break; }
                case AttackState.Cancelled:
                    {
                        //Fire the appropriate event.
                        MoveCancelledInvoke();

                        break;
                    }
                case AttackState.Underway:
                    {
                        //The outcome of the move.
                        switch (_AttackOutcome)
                        {
                            case AttackOutcome.Miss:
                                {
                                    //The attack is ready to be completed.
                                    _AttackState = AttackState.Conclusion;
                                    break;
                                }
                            case AttackOutcome.Clash:
                                {
                                    //Test animation.
                                    _User.Velocity = Calculator.LineDirection(_User.Position, _Target.Position) * _User.Speed * .1f;

                                    //If the attacker has reached the target.
                                    if (Vector2.Distance(_User.Position, _Target.Position) < 10)
                                    {
                                        //The attack is ready to be completed.
                                        _AttackState = AttackState.Conclusion;
                                    }
                                    break;
                                }
                            case AttackOutcome.Hit:
                                {
                                    //Test animation.
                                    _User.Velocity = Calculator.LineDirection(_User.Position, _Target.Position) * _User.Speed * .1f;

                                    //If the attacker has reached the target.
                                    if (Vector2.Distance(_User.Position, _Target.Position) < 10)
                                    {
                                        //The attack is ready to be completed.
                                        _AttackState = AttackState.Conclusion;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case AttackState.Conclusion:
                    {
                        //The outcome of the move.
                        switch (_AttackOutcome)
                        {
                            case AttackOutcome.Miss:
                                {
                                    break;
                                }
                            case AttackOutcome.Clash:
                                {
                                    break;
                                }
                            case AttackOutcome.Hit:
                                {
                                    //Damage the target.
                                    _Target.ReceiveAttack(this);
                                    break;
                                }
                        }

                        //The move consumes energy!
                        _User.CurrentEnergy -= _Move.EnergyConsume;

                        //Call the event.
                        MoveCompletedInvoke();
                        break;
                    }
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {

        }

        /// <summary>
        /// Cancel the move for use in battle.
        /// </summary>
        public void Cancel()
        {
            //Cancel the move.
            _AttackState = _AttackState != AttackState.Idle ? AttackState.Cancelled : _AttackState;
        }
        /// <summary>
        /// Use the move on a Pokémon.
        /// </summary>
        /// <param name="outcome">The outcome of the attack.</param>
        public void Use(AttackOutcome outcome)
        {
            //Stop here if the attacker or target isn't valid, or if the move is already underway.
            if (_User == null || _Target == null || _AttackState != AttackState.Idle) { return; }

            //Get the outcome of the move and then activate it.
            _AttackOutcome = outcome;
            MoveUnderwayInvoke();
        }
        /// <summary>
        /// The move has been completed.
        /// </summary>
        private void MoveCompletedInvoke()
        {
            //If the move was not underway or finished, break here.
            if (_AttackState == AttackState.Idle || _AttackState == AttackState.Cancelled) { return; }

            //The attack is now officially finished.
            _AttackState = AttackState.Concluded;

            //If someone has hooked up a delegate to the event, fire it.
            if (MoveCompleted != null) { MoveCompleted(this); }
        }
        /// <summary>
        /// The move has been accepted and is now underway.
        /// </summary>
        private void MoveUnderwayInvoke()
        {
            //If we are involved in something, break here.
            if (_AttackState != AttackState.Idle) { return; }

            //The attack is now underway.
            _AttackState = AttackState.Underway;

            //If someone has hooked up a delegate to the event, fire it.
            if (MoveUnderway != null) { MoveUnderway(this); }
        }
        /// <summary>
        /// The move has been cancelled.
        /// </summary>
        private void MoveCancelledInvoke()
        {
            //If the move was not underway, break here.
            if (_AttackState != AttackState.Underway) { return; }

            //The attack is now officially finished.
            _AttackState = AttackState.Concluded;

            //If someone has hooked up a delegate to the event, fire it.
            if (MoveCancelled != null) { MoveCancelled(this); }
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
        public Pokemon User
        {
            get { return _User; }
            set { _User = value; }
        }
        public Pokemon Target
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
*/