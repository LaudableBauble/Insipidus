using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsipidusEngine
{
    abstract class CharacterList
    {
        #region Fields
        private List<Character> _ListOfPokemon;
        #endregion

        #region Poperties
        public List<Character> ListOfPokemon
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
            _ListOfPokemon = new List<Character>();
        }
        public CharacterList(int capacity)
        {
            _ListOfPokemon = new List<Character>();
            _ListOfPokemon.Capacity = capacity;
        }
        #endregion

        #region Methods
        public void Add(Character pokemon)
        {
            //Add the Pokemon to the list.
            _ListOfPokemon.Add(pokemon);
        }
        public void Remove(Character pokemon)
        {
            //Remove the Pokemon from the list.
            _ListOfPokemon.Remove(pokemon);
        }
        #endregion
    }
}