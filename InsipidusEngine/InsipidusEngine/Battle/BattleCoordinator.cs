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

namespace InsipidusEngine.Battle
{
    /// <summary>
    /// The battle coordinator handles all battles in the game and subsequently controls the flow of each battle.
    /// </summary>
    public class BattleCoordinator
    {
        #region Fields
        private static BattleCoordinator _Instance;
        private ContentManager _Content;
        private List<BattleMove> _Moves;
        private float _ElapsedTime;
        private float _TimePerTurn;
        #endregion

        #region Constructors
        /// <summary>
        /// Make sure that the constructor is private.
        /// </summary>
        private BattleCoordinator()
        {
            //Initialize the coordinator.
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the battle coordinator.
        /// </summary>
        public void Initialize()
        {
            //Initialize the class.
            _Moves = new List<BattleMove>();
            _ElapsedTime = 0;
            _TimePerTurn = .25f;
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="content">The content manager.</param>
        public void LoadContent(ContentManager content)
        {
            //Save the content manager for future references.
            _Content = content;

            //Load all content.
            (new List<BattleMove>(_Moves)).ForEach(move => move.LoadContent(content));
        }
        /// <summary>
        /// Update the battle coordinator.
        /// </summary>
        /// <param name="gametime">The time that has passed.</param>
        public void Update(GameTime gametime)
        {
            //Get the time since the last update.
            _ElapsedTime += (float)gametime.ElapsedGameTime.TotalSeconds;

            //Copy the list of moves so that we can iterate on the list unobstructed.
            List<BattleMove> moves = new List<BattleMove>(_Moves);

            //Whether it is time to end the turn.
            if (_ElapsedTime >= _TimePerTurn)
            {
                //Go through all queued moves and use them.
                moves.FindAll(move => move.State == TimelineState.Idle).ForEach(move => UseMove(move));

                //Begin the cycle anew.
                _ElapsedTime -= _TimePerTurn;
            }

            //Update all active moves.
            moves.ForEach(move => move.Update(gametime));
        }
        /// <summary>
        /// Draw the battle coordinator.
        /// </summary>
        /// <param name="spritebatch">The spritebatch to use.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            //Copy the list of moves so that we can iterate on the list unobstructed.
            List<BattleMove> moves = new List<BattleMove>(_Moves);
            //Draw the sprite and moves.
            moves.ForEach(move => move.Draw(spritebatch));
        }

        /// <summary>
        /// Queue a move for use in battle.
        /// </summary>
        /// <param name="move">The move to queue.</param>
        public void QueueMove(BattleMove move)
        {
            //Add the move to the list.
            if (move != null && move.User != null && move.Target != null) { _Moves.Add(move); }

            //Subscribe to the move's events.
            move.Concluded += OnMoveFinished;
        }
        /// <summary>
        /// Use a move.
        /// </summary>
        /// <param name="move">The move to use.</param>
        private void UseMove(BattleMove move)
        {
            //Load the move's content.
            move.LoadContent(_Content);

            //Use the move.
            move.Activate();
        }
        /// <summary>
        /// Try to cancel a character's moves.
        /// </summary>
        /// <param name="move">The move to cancel.</param>
        public void CancelMoves(Creature character)
        {
            //Try to cancel all moves.
            FindUserMoves(character).ForEach(item => item.Cancel());
        }
        /// <summary>
        /// Get all of sthe currently active moves carried out by a specified user.
        /// </summary>
        /// <param name="user">The user of the moves.</param>
        /// <returns>The moves.</returns>
        public List<BattleMove> FindUserMoves(Creature user)
        {
            return (new List<BattleMove>(_Moves)).FindAll(item => item.User == user);
        }
        /// <summary>
        /// Find out whether any active move currently has control over the specified character.
        /// </summary>
        /// <param name="character">The character to base the search on.</param>
        /// <returns>Whether any move has control over the character.</returns>
        public bool HasMoveControl(Creature character)
        {
            //The result.
            bool result = false;

            //Find out.
            foreach (BattleMove move in new List<BattleMove>(_Moves))
            {
                if ((move.User == character && move.HasUserControl) || (move.Target == character && move.HasTargetControl)) { result = true; break; }
            }

            //Return the result.
            return result;
        }
        /// <summary>
        /// Update a character's state by looking at all active moves' state of control.
        /// </summary>
        /// <param name="character">The character whos state to update.</param>
        public void UpdateControlState(Creature character)
        {
            //If any active move has control over the character, set its state to active.
            if (HasMoveControl(character)) { character.BattleState = BattleState.Active; }
            else { character.BattleState = BattleState.Idle; }
        }
        /// <summary>
        /// When a move has been cancelled or completed.
        /// </summary>
        /// <param name="move">The move that is finished.</param>
        private void OnMoveFinished(BattleMove move)
        {
            //The move is now obsolete, remove it.
            _Moves.Remove(move);

            //Unsubscribe from the move.
            move.Concluded -= OnMoveFinished;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The static battle coordinator instance.
        /// </summary>
        public static BattleCoordinator Instance
        {
            get
            {
                if (_Instance == null) { _Instance = new BattleCoordinator(); }
                return _Instance;
            }
        }
        /// <summary>
        /// Get this turn's queue of moves.
        /// </summary>
        public List<BattleMove> Moves
        {
            get { return _Moves; }
        }
        #endregion
    }
}
