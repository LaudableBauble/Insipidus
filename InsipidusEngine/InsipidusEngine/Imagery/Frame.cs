using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace InsipidusEngine.Imagery
{
    /// <summary>
    /// The frame of a sprite, ie. the closest you can get to an individual image. Does not store textures directly but paths to them.
    /// This is v1.1 and extended to include normal and depth maps.
    /// </summary>
    public class Frame
    {
        #region Fields
        private string _ColorPath;
        private string _NormalPath;
        private string _DepthPath;
        private float _Width;
        private float _Height;
        private Vector2 _Origin;
        private Texture2D _Texture;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="path">The path of the color map.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        public Frame(string path, float width, float height)
        {
            Initialize(path, null, width, height, Vector2.Zero);
        }
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="path">The path of the color map.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        /// <param name="origin">The origin of the frame.</param>
        public Frame(string path, float width, float height, Vector2 origin)
        {
            Initialize(path, null, width, height, origin);
        }
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="texture">The texture of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        public Frame(Texture2D texture, float width, float height)
        {
            Initialize("", texture, width, height, Vector2.Zero);
        }
        /// <summary>
        /// Constructor for a frame.
        /// </summary>
        /// <param name="texture">The texture of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        /// <param name="origin">The origin of the frame.</param>
        public Frame(Texture2D texture, float width, float height, Vector2 origin)
        {
            Initialize("", texture, width, height, origin);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Intialize the frame.
        /// </summary>
        /// <param name="path">The path of the frame.</param>
        /// <param name="texture">The texture of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        /// <param name="origin">The origin of the frame texture.</param>
        protected void Initialize(string path, Texture2D texture, float width, float height, Vector2 origin)
        {
            //Intialize a few variables.
            _ColorPath = path;
            _NormalPath = "";
            _DepthPath = "";
            _Texture = texture;
            _Height = height;
            _Width = width;
            _Origin = origin;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The path of the color map.
        /// </summary>
        public string ColorPath
        {
            get { return _ColorPath; }
            set { _ColorPath = value; }
        }
        /// <summary>
        /// The path of the normal map.
        /// </summary>
        public string NormalPath
        {
            get { return _NormalPath; }
            set { _NormalPath = value; }
        }
        /// <summary>
        /// The path of the depth map.
        /// </summary>
        public string DepthPath
        {
            get { return _DepthPath; }
            set { _DepthPath = value; }
        }
        /// <summary>
        /// The texture of the frame. Use only with dynamically generated textures that cannot be loaded of memory.
        /// </summary>
        public Texture2D Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }
        /// <summary>
        /// The origin of the frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return _Origin; }
            set { _Origin = value; }
        }
        /// <summary>
        /// The frame height.
        /// </summary>
        public float Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        /// <summary>
        /// The frame width.
        /// </summary>
        public float Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        #endregion
    }
}