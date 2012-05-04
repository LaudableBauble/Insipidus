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

namespace InsipidusEngine
{
    /// <summary>
    /// A battle move is simply an activated form of a move and is carried out by two parties in a battle; an attacker and a defender.
    /// </summary>
    public class BattleMove
    {
        #region Fields
        protected Move _Move;
        protected AttackState _AttackState;
        protected AttackOutcome _AttackOutcome;
        protected Character _User;
        protected Character _Target;
        protected bool _IsCancelable;
        #endregion

        #region Events
        public delegate void MoveEventHandler(BattleMove source);
        public event MoveEventHandler MoveCompleted;
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
                case AttackState.Cancelled: { break; }
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
                                    //Declare unstoppable.
                                    _IsCancelable = false;
                                    //Test animation.
                                    _User.Velocity = Calculator.LineDirection(_User.Position, _Target.Position) * _User.Speed * .05f;
                                    _Target.Velocity = Calculator.LineDirection(_Target.Position, _User.Position) * _Target.Speed * .05f;

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
                                    //Declare unstoppable.
                                    _IsCancelable = false;
                                    //Test animation.
                                    _User.Velocity = Calculator.LineDirection(_User.Position, _Target.Position) * _User.Speed * .05f;

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
        /// Use the move on a Pokémon.
        /// </summary>
        /// <param name="outcome">The outcome of the attack.</param>
        public void Use(AttackOutcome outcome)
        {
            //Stop here if the attacker or target isn't valid, or if the move is already underway.
            if (_User == null || _Target == null || _AttackState != AttackState.Idle) { return; }

            //Get the outcome of the move and then activate it.
            _AttackOutcome = outcome;
            _AttackState = AttackState.Underway;
        }
        /// <summary>
        /// Cancel the move for use in battle.
        /// </summary>
        public void Cancel()
        {
            //Cancel the move, if we can.
            if (_IsCancelable) { _AttackState = AttackState.Cancelled; }
        }
        /// <summary>
        /// The move has been completed.
        /// </summary>
        protected void MoveCompletedInvoke()
        {
            //The attack is now officially finished.
            _AttackState = AttackState.Concluded;

            //If someone has hooked up a delegate to the event, fire it.
            if (MoveCompleted != null) { MoveCompleted(this); }
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
