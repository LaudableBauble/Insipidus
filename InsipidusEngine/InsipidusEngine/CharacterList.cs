using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsipidusEngine
{
    abstract class CharacterList
    {
        #region Fields
        private List<Creature> _ListOfPokemon;
        #endregion

        #region Poperties
        public List<Creature> ListOfPokemon
        {
            get { return _ListOfPokemon; }
            set { _ListOfPokemon = value; }
        }
        public int Capacity
        {
            get { return _ListOfPokemon.Capacity; }
            set { _ListOfPokemon.Capacity = value; }
        }
        public int Count
        {
            get { return _ListOfPokemon.Count; }
        }
        #endregion

        #region Constructors
        public CharacterList()
        {
            _ListOfPokemon = new List<Creature>();
        }
        public CharacterList(int capacity)
        {
            _ListOfPokemon = new List<Creature>();
            _ListOfPokemon.Capacity = capacity;
        }
        #endregion

        #region Methods
        public void Add(Creature pokemon)
        {
            //Add the Pokemon to the list.
            _ListOfPokemon.Add(pokemon);
        }
        public void Remove(Creature pokemon)
        {
            //Remove the Pokemon from the list.
            _ListOfPokemon.Remove(pokemon);
        }
        #endregion
    }
}