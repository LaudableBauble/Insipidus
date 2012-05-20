using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsipidusEngine
{
    /// <summary>
    /// An effect describes a certain state of a character, ie. whether the character is poisoned or has a weakened defense.
    /// Apart from this the areas of use for an effect can be many, but its most popular use is in abilities and moves.
    /// </summary>
    public abstract class EffectStatus
    {
        #region Fields
        private int _TimeLeftActive;
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion
    }
}
