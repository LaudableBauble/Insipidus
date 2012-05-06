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
    /// A move is an ability that can be performed by a character; either to damage or to affect another character in some way. Moves are primarily used in battle.
    /// In order to use a move in battle, encapsulate it within a <code>BattleMove</code> instance and thus activate.
    /// </summary>
    public class Move
    {
        #region Fields
        private string _Name;
        private int _Id;
        private string _Description;
        private List<PokemonType> _Types;
        private int _PowerPhysical;
        private int _PowerSpecial;
        private int _Accuracy;
        private int _EnergyConsume;
        private Status _Status;
        private float _Force;
        #endregion

        #region Methods
        public void Initialize()
        {
            //Initialize the class.
            _Types = new List<PokemonType>();
        }
        public void LoadContent(ContentManager content)
        {

        }
        public void Update(GameTime gametime)
        {

        }
        public void Draw(SpriteBatch spritebatch)
        {

        }
        #endregion

        #region Properties
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public int PowerPhysical
        {
            get { return _PowerPhysical; }
            set { _PowerPhysical = value; }
        }
        public int PowerSpecial
        {
            get { return _PowerSpecial; }
            set { _PowerSpecial = value; }
        }
        public int Accuracy
        {
            get { return _Accuracy; }
            set { _Accuracy = value; }
        }
        public int EnergyConsume
        {
            get { return _EnergyConsume; }
            set { _EnergyConsume = value; }
        }
        public Status Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        public List<PokemonType> Types
        {
            get { return _Types; }
            set { _Types = value; }
        }
        public float Force
        {
            get { return _Force; }
            set { _Force = value; }
        }
        #endregion
    }
}
