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
using InsipidusEngine.Helpers;

namespace InsipidusEngine
{
    /// <summary>
    /// A factory produces preset instances of popular classes and simplifies creation of others.
    /// </summary>
    public class Factory
    {
        #region Fields
        private static Factory _Instance;
        #endregion

        #region Constructors
        /// <summary>
        /// Make sure that the constructor is private.
        /// </summary>
        private Factory() { }
        #endregion

        #region Methods
        #region Battle Animations
        /// <summary>
        /// Create a battle animation, given a certain move.
        /// </summary>
        /// <param name="move">The move to create an animation for.</param>
        /// <returns></returns>
        public Timeline createBattleAnimation(BattleMove move)
        {
            //TODO: Creating and handling a move's animation should be more robust and maintainable. This is a bit of a hack.

            //Create the animation timeline.
            Timeline animation = new Timeline(move);

            //Check the name of the move and create a fitting animation.
            switch (move.Name)
            {
                case "Ember":
                    {
                        //Add events to the timeline.
                        ModifyEnergyEvent energy = new ModifyEnergyEvent(animation, 0, null, move.User, -move.EnergyConsume);
                        ModifyControlEvent state = new ModifyControlEvent(animation, 0, energy, move, move.User, false);
                        ModifyCancelableEvent cancel = new ModifyCancelableEvent(animation, 0, energy, move, false);
                        ProjectileEvent projectile = new ProjectileEvent(animation, 0, null, move.User.Position, new Destination(move.Target));
                        ModifyHealthEvent damage = new ModifyHealthEvent(animation, 0, projectile, move.Target, -move.GetDamage());
                        ImpactEvent impact = new ImpactEvent(animation, 0, projectile, move);

                        //Add the events to the timeline.
                        animation.AddEvent(energy);
                        animation.AddEvent(state);
                        animation.AddEvent(cancel);
                        animation.AddEvent(projectile);
                        animation.AddEvent(damage);
                        animation.AddEvent(impact);
                        break;
                    }
                case "Scratch":
                    {
                        //Add events to the timeline.
                        ModifyControlEvent stateStart = new ModifyControlEvent(animation, 0, null, move, move.User, true);
                        MovementEvent moveTo = new MovementEvent(animation, 0, null, move.User, new Destination(move.Target), MovementType.Run);
                        ModifyCancelableEvent cancel = new ModifyCancelableEvent(animation, 0, moveTo, move, false);
                        ModifyHealthEvent damage = new ModifyHealthEvent(animation, 0, moveTo, move.Target, -move.GetDamage());
                        ModifyEnergyEvent energy = new ModifyEnergyEvent(animation, 0, damage, move.User, -move.EnergyConsume);
                        ModifyControlEvent stateEnd = new ModifyControlEvent(animation, 0, energy, move, move.User, false);
                        ImpactEvent impact = new ImpactEvent(animation, 0, moveTo, move);

                        //Add the events to the timeline.
                        animation.AddEvent(stateStart);
                        animation.AddEvent(moveTo);
                        animation.AddEvent(cancel);
                        animation.AddEvent(damage);
                        animation.AddEvent(energy);
                        animation.AddEvent(stateEnd);
                        animation.AddEvent(impact);
                        break;
                    }
                default: { goto case "Scratch"; }
            }

            //Return the animation.
            return animation;
        }
        #endregion

        #region Moves
        /// <summary>
        /// Create a move.
        /// </summary>
        /// <param name="name">The name of the move.</param>
        /// <param name="description">The description of the move.</param>
        /// <param name="type">The first type of the move.</param>
        /// <param name="powerPhysical">The physical power of the move.</param>
        /// <param name="powerSpecial">The special power of the move.</param>
        /// <param name="accuracy">The accuracy of the move.</param>
        /// <param name="energyConsume">The energy consumed by the move.</param>
        /// <param name="status">The status ailments that the moves afflicts victims with.</param>
        /// <param name="force">The physical force behind the move.</param>
        /// <returns>The move created with the given data.</returns>
        public Move CreateMove(string name, string description, PokemonType type, int powerPhysical, int powerSpecial, int accuracy, int energyConsume,
            Status status, float force)
        {
            //Create the list of types.
            List<PokemonType> types = new List<PokemonType>();
            types.Add(type);

            //Return the move.
            return CreateMove(name, description, types, powerPhysical, powerSpecial, accuracy, energyConsume, status, force);
        }
        /// <summary>
        /// Create a move.
        /// </summary>
        /// <param name="name">The name of the move.</param>
        /// <param name="description">The description of the move.</param>
        /// <param name="types">The types of the move.</param>
        /// <param name="powerPhysical">The physical power of the move.</param>
        /// <param name="powerSpecial">The special power of the move.</param>
        /// <param name="accuracy">The accuracy of the move.</param>
        /// <param name="energyConsume">The energy consumed by the move.</param>
        /// <param name="status">The status ailments that the moves afflicts victims with.</param>
        /// <param name="force">The physical force behind the move.</param>
        /// <returns>The move created with the given data.</returns>
        public Move CreateMove(string name, string description, List<PokemonType> types, int powerPhysical, int powerSpecial, int accuracy, int energyConsume,
            Status status, float force)
        {
            //Create the move.
            Move move = new Move();
            move.Initialize();

            //Set the stats accordingly.
            move.Name = name;
            move.Description = description;
            move.Types = types;
            move.PowerPhysical = powerPhysical;
            move.PowerSpecial = powerSpecial;
            move.Accuracy = accuracy;
            move.EnergyConsume = energyConsume;
            move.Status = status;
            move.Force = force;

            //Return the move.
            return move;
        }
        #endregion

        #region Pokémon
        /// <summary>
        /// Create a Pokémon.
        /// </summary>
        /// <param name="name">The name of the pokémon.</param>
        /// <param name="gender">The gender of the pokémon.</param>
        /// <param name="type">The type of the pokémon.</param>
        /// <param name="hp">The max HP of the pokémon.</param>
        /// <param name="attackPhysical">The physical attack power of the pokémon.</param>
        /// <param name="defensePhysical">The physical defense power of the pokémon.</param>
        /// <param name="specialAttack">The special attack power of the pokémon.</param>
        /// <param name="specialDefense">The special defense power of the pokémon.</param>
        /// <param name="powerPhysical">The physical power of the move.</param>
        /// <param name="powerSpecial">The special power of the move.</param>
        /// <param name="speed">The speed of the pokémon.</param>
        /// <param name="level">The level of the pokémon.</param>
        /// <param name="moves">The moves that this pokémon knows.</param>
        /// <param name="maxEnergy">The max energy of the pokémon.</param>
        /// <returns>The pokémon created with the given data.</returns>
        public Character CreatePokemon(string name, Gender gender, PokemonType type, float hp, float attackPhysical, float defensePhysical,
            float specialAttack, float specialDefense, float speed, int level, List<Move> moves, float maxEnergy)
        {
            //Create the list of types.
            List<PokemonType> types = new List<PokemonType>();
            types.Add(type);

            //Return the move.
            return CreatePokemon(name, gender, types, hp, attackPhysical, defensePhysical, specialAttack, specialDefense, speed, level, moves, maxEnergy);
        }
        /// <summary>
        /// Create a Pokémon.
        /// </summary>
        /// <param name="name">The name of the pokémon.</param>
        /// <param name="gender">The gender of the pokémon.</param>
        /// <param name="types">The types of the pokémon.</param>
        /// <param name="hp">The max HP of the pokémon.</param>
        /// <param name="attackPhysical">The physical attack power of the pokémon.</param>
        /// <param name="defensePhysical">The physical defense power of the pokémon.</param>
        /// <param name="specialAttack">The special attack power of the pokémon.</param>
        /// <param name="specialDefense">The special defense power of the pokémon.</param>
        /// <param name="powerPhysical">The physical power of the move.</param>
        /// <param name="powerSpecial">The special power of the move.</param>
        /// <param name="speed">The speed of the pokémon.</param>
        /// <param name="level">The level of the pokémon.</param>
        /// <param name="moves">The moves that this pokémon knows.</param>
        /// <param name="maxEnergy">The max energy of the pokémon.</param>
        /// <returns>The pokémon created with the given data.</returns>
        public Character CreatePokemon(string name, Gender gender, List<PokemonType> types, float hp, float attackPhysical, float defensePhysical,
            float specialAttack, float specialDefense, float speed, int level, List<Move> moves, float maxEnergy)
        {
            //Create the Pokémon.
            Character pokemon = new Character();

            //Set the stats accordingly.
            pokemon.Name = name;
            pokemon.Gender = gender;
            pokemon.Types = types;
            pokemon.CurrentHP = hp;
            pokemon.HP = hp;
            pokemon.AttackPhysical = attackPhysical;
            pokemon.DefensePhysical = defensePhysical;
            pokemon.SpecialAttack = specialAttack;
            pokemon.SpecialDefense = specialDefense;
            pokemon.Speed = speed;
            pokemon.Level = level;
            pokemon.Moves = moves;
            pokemon.CurrentEnergy = maxEnergy;
            pokemon.MaxEnergy = maxEnergy;

            //Return the move.
            return pokemon;
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// The factory instance.
        /// </summary>
        public static Factory Instance
        {
            get
            {
                if (_Instance == null) { _Instance = new Factory(); }
                return _Instance;
            }
        }

        #region Preset Moves
        /// <summary>
        /// Get a new Ember move.
        /// </summary>
        public Move Ember
        {
            get { return CreateMove("Ember", "", PokemonType.Fire, 0, 40, 100, 75, null, 5); }
        }
        /// <summary>
        /// Get a new Scratch move.
        /// </summary>
        public Move Scratch
        {
            get { return CreateMove("Scratch", "", PokemonType.Normal, 40, 0, 100, 50, null, 10); }
        }
        /// <summary>
        /// Get a new Dynamicpunch move.
        /// </summary>
        public Move Dynamicpunch
        {
            get { return CreateMove("Dynamicpunch", "", PokemonType.Fighting, 100, 0, 50, 95, null, 15); }
        }
        #endregion

        #region Preset Pokémon
        /// <summary>
        /// Get a new Snivy pokémon.
        /// </summary>
        public Character Snivy
        {
            get
            {
                return CreatePokemon("Snivy", Gender.Male, PokemonType.Grass, 45, 45, 55, 45, 55, 63, 5, new List<Move>() { Scratch, Ember, Dynamicpunch }, 100);
            }
        }
        /// <summary>
        /// Get a new Pansear pokémon.
        /// </summary>
        public Character Pansear
        {
            get
            {
                return CreatePokemon("Pansear", Gender.Male, PokemonType.Fire, 50, 53, 48, 53, 48, 64, 5, new List<Move>() { Scratch, Ember, Dynamicpunch }, 100);
            }
        }
        #endregion
        #endregion
    }
}
