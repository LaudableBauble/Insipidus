using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsipidusEngine
{
    /// <summary>
    /// The different types of mouse buttons.
    /// </summary>
    public enum MouseButton
    {
        Left, Right, Middle
    }
    /// <summary>
    /// Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
        TransitionOn, Active, TransitionOff, Hidden,
    }
    public enum BattleType
    {
        OneVsOne, OneVsTwo, TwoVsTwo
    }
    public enum BattleStatus
    {
        Intro, BattleMenu
    }
    public enum PokemonIndividual
    {
        Bulbasaur,
        Charmander,
        Squirtle
    }
    /// <summary>
    /// The type of a Pokémon.
    /// </summary>
    public enum PokemonType
    {
        Normal,
        Grass,
        Fire,
        Water,
        Psychic,
        Ghost,
        Dragon,
        Ice,
        Dark,
        Steel,
        Poison,
        Ground,
        Rock,
        Electric,
        Fighting
    }
    /// <summary>
    /// The gender of something, ie. human or Pokémon.
    /// </summary>
    public enum Gender
    {
        Male, Female, None
    }
    /// <summary>
    /// The state of attack.
    /// </summary>
    public enum AttackState
    {
        Idle, Cancelled, Underway, Conclusion, Concluded
    }
    /// <summary>
    /// The outcome of an attack.
    /// </summary>
    public enum AttackOutcome
    {
        None, Miss, Dodge, Block, Clash, Hit
    }
    /// <summary>
    /// The state of battle for a participant.
    /// </summary>
    public enum BattleState
    {
        None, Idle, Waiting, Active
    }
    public enum TextBoxEvent
    {
        None, ClearTextBox, DelayMultiplierChange, EndTextBox, NewLine, ScrollRowsDown
    }
    public enum TextBoxCloseMode
    {
        Manual, Automatic
    }
    /// <summary>
    /// A rule to be applied during a battle animation.
    /// </summary>
    public enum AnimationRule
    {
        MoveToTarget, MoveToUser, DamageTarget, ConsumeEnergy
    }

    /// <summary>
    /// The type of timeline event.
    /// </summary>
    public enum TimelineEventType
    {
        Duration, Direct
    }
    /// <summary>
    /// The state of a timeline and its events.
    /// </summary>
    public enum TimelineState
    {
        Idle, Active, Concluded
    }
    /// <summary>
    /// The different types of movement available to a character.
    /// </summary>
    public enum MovementType
    {
        Walk, Run, Jump, Fly, Teleport
    }

    /// <summary>
    /// The orientation of a sprite; namely none, left or right.
    /// </summary>
    public enum Orientation
    {
        None, Left, Right
    }
    /// <summary>
    /// The visibility of the sprite.
    /// </summary>
    public enum Visibility
    {
        Invisible, Visible
    }
    /// <summary>
    /// The direction of something.
    /// </summary>
    public enum Direction
    {
        Up, Down, Right, Left
    }
    /// <summary>
    /// The speed of something, chopped up to distinctive levels.
    /// </summary>
    public enum SpeedType
    {
        Still, Slow, Normal, Fast
    }
    /// <summary>
    /// The outcome of a battle event.
    /// </summary>
    public enum EventOutcome
    {
        None, Fail, Success
    }
}
