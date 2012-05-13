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

using InsipidusEngine;
using InsipidusEngine.Imagery;
using InsipidusEngine.Battle;

namespace InsipidusEngine
{
    /// <summary>
    /// A character is a creature and is primarily used for battling with other characters.
    /// </summary>
    public class Character
    {
        #region Fields
        private string _Name;
        private int _Id;
        private Gender _Gender;
        private List<PokemonType> _Types;
        private float _CurrentHP;
        private float _HP;
        private float _AttackPhysical;
        private float _DefensePhysical;
        private float _SpecialAttack;
        private float _SpecialDefense;
        private float _Speed;
        private int _Level;
        private List<Move> _Moves;
        private float _CurrentEnergy;
        private float _MaxEnergy;
        private SpriteManager _Sprite;
        private Character _Target;
        private Vector2 _Position;
        private Vector2 _Velocity;
        private float _ElapsedMovementTime;
        private float _TimeBetweenMoving;
        private float _ElapsedEnergyTime;
        private float _EnergyRecoverySpeed;
        private BattleState _BattleState;
        private Direction _FacingDirection;
        private SpeedType _MovementSpeed;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a character.
        /// </summary>
        public Character()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the character.
        /// </summary>
        private void Initialize()
        {
            //Initialize the class.
            _Gender = Gender.Male;
            _Types = new List<PokemonType>();
            _Moves = new List<Move>();
            _Sprite = new SpriteManager();
            _MaxEnergy = 100;
            _CurrentEnergy = _MaxEnergy;
            _Position = Vector2.Zero;
            _Velocity = Vector2.Zero;
            _TimeBetweenMoving = .01f;
            _ElapsedMovementTime = 0;
            _ElapsedEnergyTime = 0;
            _EnergyRecoverySpeed = .1f;
            _BattleState = BattleState.Idle;
            _FacingDirection = Direction.Down;
            _MovementSpeed = InsipidusEngine.SpeedType.Still;
        }
        /// <summary>
        /// Load the character's content.
        /// </summary>
        /// <param name="content">The content manager to use.</param>
        public void LoadContent(ContentManager content)
        {
            //Load all content.
            _Sprite.LoadContent(content);
            _Moves.ForEach(item => item.LoadContent(content));
        }
        /// <summary>
        /// Update the character.
        /// </summary>
        /// <param name="gametime">The time that has passed.</param>
        public void Update(GameTime gametime)
        {
            //Get the time since the last update.
            _ElapsedEnergyTime += (float)gametime.ElapsedGameTime.TotalSeconds;
            _ElapsedMovementTime += (float)gametime.ElapsedGameTime.TotalSeconds;

            //Whether it is time to recover some energy.
            if (_ElapsedEnergyTime >= _EnergyRecoverySpeed)
            {
                //Recover some energy.
                _CurrentEnergy = MathHelper.Clamp(_CurrentEnergy + 1, 0, 100);

                //Begin the cycle anew.
                _ElapsedEnergyTime -= _EnergyRecoverySpeed;
            }

            //If the pokémon is currently trading blows, let him be guided by the battle animation.
            if (_BattleState != BattleState.Active)
            {
                //Can we attack? If yes, do so.
                if (_CurrentEnergy >= _MaxEnergy) { LaunchAttack(_Moves[Calculator.RandomNumber(0, _Moves.Count - 1)], _Target); }
            }

            //Update the character's movement.
            UpdateMovement();
            //Determine the sprite to display.
            DetermineSprite();

            //Update the sprite and moves.
            _Sprite.Update(gametime, _Position, 0);
        }
        /// <summary>
        /// Draw the character.
        /// </summary>
        /// <param name="spritebatch">The sprite batch to use.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            //Draw the sprite and moves.
            _Sprite.Draw(spritebatch);
            _Moves.ForEach(item => item.Draw(spritebatch));
        }

        /// <summary>
        /// Launch an attack on a character with a move.
        /// </summary>
        /// <param name="move">The move to attack with.</param>
        /// <param name="target">The target to attack.</param>
        public void LaunchAttack(Move move, Character target)
        {
            //Stop here if the move or target isn't valid, or if a move already has been set in motion.
            if (move == null || target == null || _BattleState != BattleState.Idle) { return; }

            //Create the activated form of the move.
            BattleMove active = new BattleMove(move, this, target);

            //Prepare for the attack.
            _BattleState = BattleState.Waiting;

            //Attack.
            BattleCoordinator.Instance.QueueMove(active);
        }
        /// <summary>
        /// Recive an attack from another character.
        /// </summary>
        /// <param name="damage">The damage the attack dishes out.</param>
        public void RecieveAttack(float damage)
        {
            //Decrease the health.
            _CurrentHP -= damage;

            //Try to cancel the current attack, if there exist one.
            BattleCoordinator.Instance.FindMove(this).Cancel();
        }
        /// <summary>
        /// Determine which sprite to show.
        /// </summary>
        private void DetermineSprite()
        {
            //Create the sprite placeholder.
            Sprite sprite = _Sprite.GetSprite(0);

            //Determine the sprite to display.
            switch (_FacingDirection)
            {
                case Direction.Down: { sprite = _Sprite.GetSprite("Front"); break; }
                case Direction.Up: { sprite = _Sprite.GetSprite("Back"); break; }
                case Direction.Right: { sprite = _Sprite.GetSprite("Right"); break; }
                case Direction.Left: { sprite = _Sprite.GetSprite("Left"); break; }
            }

            //Determine the animation speed of the current sprite.
            switch (_MovementSpeed)
            {
                case SpeedType.Fast: { sprite.EnableAnimation = true; sprite.TimePerFrame = .05f; break; }
                case SpeedType.Normal: { sprite.EnableAnimation = true; sprite.TimePerFrame = .1f; break; }
                case SpeedType.Slow: { sprite.EnableAnimation = true; sprite.TimePerFrame = .3f; break; }
                case SpeedType.Still: { sprite.EnableAnimation = false; break; }
            }

            //If the chosen sprite already is visible, end here.
            if (sprite.Visibility == Visibility.Visible) { return; }

            //First, make all sprites invisible. Then make the chosen sprite visible.
            _Sprite.Visibility = Visibility.Invisible;
            sprite.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Update the character's movement.
        /// </summary>
        private void UpdateMovement()
        {
            //Whether it is time to move.
            if (_ElapsedMovementTime >= _TimeBetweenMoving)
            {
                //If idle, try to keep our distance.
                if (_BattleState == BattleState.Idle) { KeepDistance(); }

                //Update the velocity and clamp it if needed. Also update the position.
                UpdateVelocity();
                _Position += _Velocity;

                //Begin the cycle anew.
                _ElapsedMovementTime -= _TimeBetweenMoving;
            }

            //Determine which direction to face based upon velocity.
            float dir = Calculator.VectorToRadians(_Velocity / _Velocity.Length()) / Calculator.RadiansToDegreesRatio;

            //If the direction is not valid, ie. NaN, keep the previous facing direction.
            if (!float.IsNaN(dir))
            {
                //Determine the facing direction.
                if (dir >= -30 && dir <= 30) { _FacingDirection = Direction.Up; }
                else if ((dir >= 150 && dir <= 180) || (dir >= -180 && dir <= -120)) { _FacingDirection = Direction.Down; }
                else if (dir >= 60 && dir <= 120) { _FacingDirection = Direction.Right; }
                else if (dir >= -120 && dir <= -30) { _FacingDirection = Direction.Left; }
            }

            //The current speed.
            float speed = _Velocity.LengthSquared();

            //Determine the movement speed.
            if (speed > 4) { _MovementSpeed = SpeedType.Fast; }
            else if (speed > 1.5f) { _MovementSpeed = SpeedType.Normal; }
            else if (speed > .5f) { _MovementSpeed = SpeedType.Slow; }
            else { _MovementSpeed = SpeedType.Still; }
        }
        /// <summary>
        /// Update the velocity and clamp it if needed.
        /// </summary>
        private void UpdateVelocity()
        {
            //Decrease the velocity.
            _Velocity -= (_Velocity.LengthSquared() > 1e-2) ? Vector2.Normalize(_Velocity) * .1f : _Velocity;
            //Clamp the velocity.
            _Velocity = Vector2.Clamp(_Velocity, -new Vector2(2, 2), new Vector2(2, 2));
        }
        /// <summary>
        /// Try to keep some distance from the opponent.
        /// </summary>
        private void KeepDistance()
        {
            //Move away if too close.
            if (Vector2.Distance(_Target.Position, _Position) < 50 && _BattleState == BattleState.Idle)
            {
                //The direction.
                Vector2 direction = Calculator.LineDirection(_Target.Position, _Position);
                //The velocity.
                _Velocity += (direction != Vector2.Zero) ? direction * _Speed * .05f : Vector2.One * _Speed * .05f;
            }
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public Gender Gender
        {
            get { return _Gender; }
            set { _Gender = value; }
        }
        public List<PokemonType> Types
        {
            get { return _Types; }
            set { _Types = value; }
        }
        public float CurrentHP
        {
            get { return _CurrentHP; }
            set { _CurrentHP = value; }
        }
        public float HP
        {
            get { return _HP; }
            set { _HP = value; }
        }
        public float AttackPhysical
        {
            get { return _AttackPhysical; }
            set { _AttackPhysical = value; }
        }
        public float DefensePhysical
        {
            get { return _DefensePhysical; }
            set { _DefensePhysical = value; }
        }
        public float SpecialAttack
        {
            get { return _SpecialAttack; }
            set { _SpecialAttack = value; }
        }
        public float SpecialDefense
        {
            get { return _SpecialDefense; }
            set { _SpecialDefense = value; }
        }
        public float Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }
        public int Level
        {
            get { return _Level; }
            set { _Level = value; }
        }
        public float CurrentEnergy
        {
            get { return _CurrentEnergy; }
            set { _CurrentEnergy = value; }
        }
        public float MaxEnergy
        {
            get { return _MaxEnergy; }
            set { _MaxEnergy = value; }
        }
        public SpriteManager Sprite
        {
            get { return _Sprite; }
            set { _Sprite = value; }
        }
        public List<Move> Moves
        {
            get { return _Moves; }
            set { _Moves = value; }
        }
        public Character Target
        {
            get { return _Target; }
            set { _Target = value; }
        }
        public Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        public Vector2 Velocity
        {
            get { return _Velocity; }
            set { _Velocity = value; }
        }
        /// <summary>
        /// The state of battle for this character.
        /// </summary>
        public BattleState BattleState
        {
            get { return _BattleState; }
            set { _BattleState = value; }
        }
        #endregion
    }
}
