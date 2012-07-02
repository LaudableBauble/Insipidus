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

namespace InsipidusEngine.Helpers
{
    /// <summary>
    /// A destination is basically a class that keeps track of a destination point.
    /// </summary>
    public class Destination
    {
        #region Fields
        private Vector2 _Position;
        private Creature _Character;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a destination.
        /// </summary>
        /// <param name="destination">The static destination vector.</param>
        public Destination(Vector2 destination)
        {
            _Position = destination;
            _Character = null;
        }
        /// <summary>
        /// Constructor for a destination.
        /// </summary>
        /// <param name="character">The dynamic destination character.</param>
        public Destination(Creature character)
        {
            _Position = Vector2.Zero;
            _Character = character;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The destination point.
        /// </summary>
        public Vector2 Position
        {
            get { return (_Character == null) ? _Position : _Character.Position; }
            set { _Position = value; }
        }
        #endregion
    }
}
