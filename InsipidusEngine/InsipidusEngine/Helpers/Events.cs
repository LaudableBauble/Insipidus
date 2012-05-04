using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsipidusEngine
{
    /// <summary>
    /// An event used when the bounds of something has changed.
    /// </summary>
    public class BoundsChangedEventArgs : EventArgs
    {
        #region Fields
        private float _Width;
        private float _Height;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the event used when the bounds of something has changed.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        public BoundsChangedEventArgs(float width, float height)
        {
            //Pass along the data.
            _Width = width;
            _Height = height;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The new width.
        /// </summary>
        public float Width
        {
            get { return _Width; }
        }
        /// <summary>
        /// The new height.
        /// </summary>
        public float Height
        {
            get { return _Height; }
        }
        #endregion
    }
}
