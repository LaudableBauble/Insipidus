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
    /// This is the Sprite who when linked to an Object displays it's Image.
    /// </summary>
    public class SpriteOld : IDisposable
    {
        //The Sprite Properties.
        #region Sprite Fields

        private ContentManager _ContentManager;
        private SpriteBatch _SpriteBatch;

        //The Sprite Orientation.
        public enum Orientation
        {
            None,
            Left,
            Right
        }

        //The Sprite Visibility.
        public enum Visibility
        {
            Invisible,
            Visible
        }

        //The Sprite Texture.
        private Texture2D[,] _SpriteTexture = new Texture2D[10, 100];

        //The Sprite's 2D Vector position.
        private Vector2[,] _SpritePosition = new Vector2[10, 100];
        //The name of the Sprites.
        private string[, ,] _SpriteName = new string[10, 10, 100];
        //The number of Sprites.
        private int[] _SpriteCount = new int[10];

        //The Frame of the selected Sprite.
        private int[,] _SpriteFrame = new int[10, 100];
        //The number of Sprite Frames.
        private int[,] _SpriteFrameCount = new int[10, 100];
        //The Time each frame/subimage is drawn.
        private float[,] _TimePerFrame = new float[10, 100];
        //The Sprite Frame's Starting Index number which determines which Sprite Frames to loop through.
        private int[,] _SpriteFrameStartIndex = new int[10, 100];
        //The Sprite Frame's Ending Index number which determines which Sprite Frames to loop through.
        private int[,] _SpriteFrameEndIndex = new int[10, 100];
        //The Sprite Animation equals true if the game should loop through the Sprite Frames.
        private bool[,] _SpriteAnimation = new bool[10, 100];
        //The Direction the Sprite Animation proceeds.
        private bool[,] _SpriteAnimationDirection = new bool[10, 100];
        //The Time since the last Update;
        private float[,] _TotalElapsedTime = new float[10, 100];

        //The Rotation.
        private float[,] _SpriteRotation = new float[10, 100];
        //The Scale.
        private float[,] _SpriteScale = new float[10, 100];
        //The Depth of the Sprite.
        private int[,] _SpriteDepth = new int[10, 100];
        //The Origin.
        private Vector2[, ,] _SpriteOrigin = new Vector2[10, 10, 100];
        //The Size.
        private float[, ,] _SpriteWidth = new float[10, 10, 100];
        private float[, ,] _SpriteHeight = new float[10, 10, 100];
        //The Sprite Offset.
        private float[,] _SpriteOffset = new float[10, 100];
        //The Sprite Rotation Offset.
        private float[,] _SpriteRotationOffset = new float[10, 100];
        //The Sprite Visibility.
        private Visibility[,] _SpriteVisibility = new Visibility[10, 100];
        //The Sprite Orientation.
        private Orientation[,] _SpriteOrientation = new Orientation[10, 100];

        #endregion

        //The Sprite Methods.
        #region Methods

        //The Main Methods.
        #region Main Methods
        /// <summary>
        /// Initialize stuff.
        /// </summary>
        /// <param name="timePerFrame"></param>
        public void InitializeSprite() { }

        /// <summary>
        /// Update the Sprite.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="contentManager"></param>
        /// <param name="spriteIndex"></param>
        /// <param name="position"></param>
        public void UpdateSprite(GameTime gameTime, int bodyIndex, int spriteIndex, Vector2 position, float rotation)
        {
            //Update the Frames.
            if (_SpriteAnimation[bodyIndex, spriteIndex] == true) { UpdateFrame(gameTime, bodyIndex, spriteIndex); }
            //Update the Main Sprite's Position.
            _SpritePosition[bodyIndex, spriteIndex] = CalculateOrbitPosition(position,
                AddAngles(rotation, _SpriteRotationOffset[bodyIndex, spriteIndex]),
                _SpriteOffset[bodyIndex, spriteIndex]);
            //Update the Main Sprite's Rotation.
            _SpriteRotation[bodyIndex, spriteIndex] = rotation;
        }

        /// <summary>
        /// Draw the Sprites to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawSprite(int bodyStartIndex, int bodyEndIndex)
        {
            //Loop through the depth values.
            for (int depth = 0; depth <= 10; depth++)
            {
                //Loop through all the bodies.
                for (int bodyIndex = bodyStartIndex; bodyIndex < bodyEndIndex; bodyIndex++)
                {
                    //Loop through all the sprites.
                    for (int spriteIndex = 0; spriteIndex < _SpriteCount[bodyIndex]; spriteIndex++)
                    {
                        //Check if the sprite's depth equals the looped depth value.
                        if (_SpriteDepth[bodyIndex, spriteIndex] == depth)
                        {
                            //If the Sprite is visible.
                            if (_SpriteVisibility[bodyIndex, spriteIndex] == SpriteOld.Visibility.Visible)
                            {
                                //Draw the sprite.
                                DrawSpriteFunction(bodyIndex, spriteIndex);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load the texture for the sprite using the Content Pipeline.
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="spriteName"></param>
        public void LoadSpriteContent(ContentManager contentManager, SpriteBatch spriteBatch)
        {
            _ContentManager = contentManager;
            _SpriteBatch = spriteBatch;
        }

        #endregion

        //Methods dealing with Adding Images.
        #region Add Image Methods
        /// <summary>
        /// Add a Sprite.
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="spriteName"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddSprite(string spriteName, int bodyIndex, Vector2 position, float timePerFrame, float scale, float width, float height, int depth,
            float rotation, float offset)
        {
            //Save the Sprite.
            _SpriteName[bodyIndex, _SpriteCount[bodyIndex], 0] = spriteName;
            //Load the Sprite.
            _SpriteTexture[bodyIndex, _SpriteCount[bodyIndex]] = _ContentManager.Load<Texture2D>(spriteName);
            //Set the Position.
            _SpritePosition[bodyIndex, _SpriteCount[bodyIndex]] = position;
            //Set the Offset.
            _SpriteOffset[bodyIndex, _SpriteCount[bodyIndex]] = offset;
            //Set the Rotation Offset.
            _SpriteRotationOffset[bodyIndex, _SpriteCount[bodyIndex]] = 0;
            //Set the Time Per Frame.
            _TimePerFrame[bodyIndex, _SpriteCount[bodyIndex]] = timePerFrame;
            //The Sprite's Scale.
            _SpriteScale[bodyIndex, _SpriteCount[bodyIndex]] = scale;
            //The Sprite's Depth.
            _SpriteDepth[bodyIndex, _SpriteCount[bodyIndex]] = depth;
            //Set the Sprite's Size.
            _SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], 0] = width;
            _SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], 0] = height;
            //Set the Origin.
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].X = (_SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], 0] / 2);
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].Y = (_SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], 0] / 2);
            //Set the Rotation.
            _SpriteRotation[bodyIndex, _SpriteCount[bodyIndex]] = rotation;
            //Set the Visibility.
            _SpriteVisibility[bodyIndex, _SpriteCount[bodyIndex]] = SpriteOld.Visibility.Visible;
            //Set the Orientation.
            _SpriteOrientation[bodyIndex, _SpriteCount[bodyIndex]] = Orientation.Right;
            //Add a count to the SpriteCount.
            _SpriteCount[bodyIndex]++;
        }

        public void AddSprite(string spriteName, int bodyIndex, Vector2 position, float orbitRotation,
            float timePerFrame, float scale, float width, float height, int depth, float rotation,
            float offset)
        {
            //Save the Sprite.
            _SpriteName[bodyIndex, _SpriteCount[bodyIndex], 0] = spriteName;
            //Load the Sprite.
            _SpriteTexture[bodyIndex, _SpriteCount[bodyIndex]] = _ContentManager.Load<Texture2D>(spriteName);
            //Set the Position.
            _SpritePosition[bodyIndex, _SpriteCount[bodyIndex]] = CalculateOrbitPosition(position, orbitRotation, offset);
            //Set the Offset.
            _SpriteOffset[bodyIndex, _SpriteCount[bodyIndex]] = offset;
            //Set the Rotation Offset.
            _SpriteRotationOffset[bodyIndex, _SpriteCount[bodyIndex]] = SubtractAngles(rotation, orbitRotation);
            //Set the Time Per Frame.
            _TimePerFrame[bodyIndex, _SpriteCount[bodyIndex]] = timePerFrame;
            //The Sprite's Scale.
            _SpriteScale[bodyIndex, _SpriteCount[bodyIndex]] = scale;
            //The Sprite's Depth.
            _SpriteDepth[bodyIndex, _SpriteCount[bodyIndex]] = depth;
            //Set the Sprite's Size.
            _SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], 0] = width;
            _SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], 0] = height;
            //Set the Origin.
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].X = (_SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], 0] / 2);
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].Y = (_SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], 0] / 2);
            //Set the Rotation.
            _SpriteRotation[bodyIndex, _SpriteCount[bodyIndex]] = rotation;
            //Set the Visibility.
            _SpriteVisibility[bodyIndex, _SpriteCount[bodyIndex]] = SpriteOld.Visibility.Visible;
            //Set the Orientation.
            _SpriteOrientation[bodyIndex, _SpriteCount[bodyIndex]] = Orientation.Right;
            //Add a count to the SpriteCount.
            _SpriteCount[bodyIndex]++;
        }

        public void AddSprite(string spriteName, int bodyIndex, Vector2 position, float timePerFrame,
            float scale, float width, float height, int depth, float rotation, float offset,
            Vector2 origin)
        {
            //Save the Sprite.
            _SpriteName[bodyIndex, _SpriteCount[bodyIndex], 0] = spriteName;
            //Load the Sprite.
            _SpriteTexture[bodyIndex, _SpriteCount[bodyIndex]] = _ContentManager.Load<Texture2D>(spriteName);
            //Set the Position.
            _SpritePosition[bodyIndex, _SpriteCount[bodyIndex]] = position;
            //Set the Offset.
            _SpriteOffset[bodyIndex, _SpriteCount[bodyIndex]] = offset;
            //Set the Rotation Offset.
            _SpriteRotationOffset[bodyIndex, _SpriteCount[bodyIndex]] = 0;
            //Set the Time Per Frame.
            _TimePerFrame[bodyIndex, _SpriteCount[bodyIndex]] = timePerFrame;
            //The Sprite's Scale.
            _SpriteScale[bodyIndex, _SpriteCount[bodyIndex]] = scale;
            //The Sprite's Depth.
            _SpriteDepth[bodyIndex, _SpriteCount[bodyIndex]] = depth;
            //Set the Sprite's Size.
            _SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], 0] = width;
            _SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], 0] = height;
            //Set the Origin.
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].X = origin.X;
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].Y = origin.Y;
            //Set the Rotation.
            _SpriteRotation[bodyIndex, _SpriteCount[bodyIndex]] = rotation;
            //Set the Visibility.
            _SpriteVisibility[bodyIndex, _SpriteCount[bodyIndex]] = SpriteOld.Visibility.Visible;
            //Set the Orientation.
            _SpriteOrientation[bodyIndex, _SpriteCount[bodyIndex]] = Orientation.Right;
            //Add a count to the SpriteCount.
            _SpriteCount[bodyIndex]++;
        }

        public void AddSprite(string spriteName, Texture2D spriteTexture, int bodyIndex, Vector2 position, float timePerFrame,
            float scale, float width, float height, int depth, float rotation, float offset)
        {
            //Save the Sprite.
            _SpriteName[bodyIndex, _SpriteCount[bodyIndex], 0] = spriteName;
            //Load the Sprite.
            _SpriteTexture[bodyIndex, _SpriteCount[bodyIndex]] = spriteTexture;
            //Set the Position.
            _SpritePosition[bodyIndex, _SpriteCount[bodyIndex]] = position;
            //Set the Offset.
            _SpriteOffset[bodyIndex, _SpriteCount[bodyIndex]] = offset;
            //Set the Rotation Offset.
            _SpriteRotationOffset[bodyIndex, _SpriteCount[bodyIndex]] = 0;
            //Set the Time Per Frame.
            _TimePerFrame[bodyIndex, _SpriteCount[bodyIndex]] = timePerFrame;
            //The Sprite's Scale.
            _SpriteScale[bodyIndex, _SpriteCount[bodyIndex]] = scale;
            //The Sprite's Depth.
            _SpriteDepth[bodyIndex, _SpriteCount[bodyIndex]] = depth;
            //Set the Sprite's Size.
            _SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], 0] = width;
            _SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], 0] = height;
            //Set the Origin.
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].X = (_SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], 0] / 2);
            _SpriteOrigin[bodyIndex, _SpriteCount[bodyIndex], 0].Y = (_SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], 0] / 2);
            //Set the Rotation.
            _SpriteRotation[bodyIndex, _SpriteCount[bodyIndex]] = rotation;
            //Set the Visibility.
            _SpriteVisibility[bodyIndex, _SpriteCount[bodyIndex]] = Visibility.Visible;
            //Set the Orientation.
            _SpriteOrientation[bodyIndex, _SpriteCount[bodyIndex]] = Orientation.Right;
            //Add a count to the SpriteCount.
            _SpriteCount[bodyIndex]++;
        }

        /// <summary>
        /// Add a Frame to the Sprite.
        /// </summary>
        /// <param name="spriteIndex"></param>
        /// <param name="frameName"></param>
        public void AddFrame(int bodyIndex, int spriteIndex, string frameName, float width, float height)
        {
            //Add a count to the SpriteFrameCount.
            _SpriteFrameCount[bodyIndex, spriteIndex]++;
            //Save the SpriteFrame's Name to the SpriteName array at the correct location.
            _SpriteName[bodyIndex, spriteIndex, _SpriteFrameCount[bodyIndex, spriteIndex]] = frameName;
            //Save the Frame Width and Height.
            _SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], _SpriteFrameCount[bodyIndex, spriteIndex]] = width;
            _SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], _SpriteFrameCount[bodyIndex, spriteIndex]] = height;
            //Save the Origin.
            _SpriteOrigin[bodyIndex, spriteIndex, _SpriteFrameCount[bodyIndex, spriteIndex]].X = (_SpriteWidth[bodyIndex, _SpriteCount[bodyIndex], _SpriteFrameCount[bodyIndex, spriteIndex]] / 2);
            _SpriteOrigin[bodyIndex, spriteIndex, _SpriteFrameCount[bodyIndex, spriteIndex]].Y = (_SpriteHeight[bodyIndex, _SpriteCount[bodyIndex], _SpriteFrameCount[bodyIndex, spriteIndex]] / 2);
        }
        #endregion

        //Methods finding/sorting the images.
        #region Image Sort Methods
        /// <summary>
        /// Find the SpriteFrame's Index in the Array.
        /// </summary>
        /// <param name="frameName"></param>
        /// <param name="spriteIndex"></param>
        /// <returns></returns>
        public int FindFrameIndex(string frameName, int bodyIndex, int spriteIndex)
        {
            //Loop through the array using a for loop.
            for (int i = 0; i < _SpriteFrameCount[bodyIndex, spriteIndex]; i++)
            {
                //If this is the right index, end the loop by changing the flag and return the index value.
                if (_SpriteName[bodyIndex, spriteIndex, i] == frameName)
                {
                    return i;
                }
            }

            //Return -1 in case of a failure.
            return -1;
        }

        /// <summary>
        /// Find the Sprite's Index in the Array.
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="spriteIndex"></param>
        /// <returns></returns>
        public int FindSpriteIndex(string spriteName, int bodyIndex, int spriteIndex)
        {
            //Loop through the array using a for loop.
            for (int i = 0; i < _SpriteCount[bodyIndex]; i++)
            {
                //If this is the right index, end the loop by changing the flag and return the index value.
                if (_SpriteName[bodyIndex, i, spriteIndex] == spriteName)
                {
                    return i;
                }
            }

            //Return -1 in case of a failure.
            return -1;
        }

        /// <summary>
        /// Sort the SpriteName array.
        /// </summary>
        /// <param name="spriteIndex"></param>
        public void SortSpriteFrameList(int bodyIndex, int spriteIndex)
        {
            //Sort the array by using a for loop.
            for (int i = 0; i < _SpriteFrameCount[bodyIndex, spriteIndex]; i++)
            {
                //Check if it's empty.
                if (_SpriteName[bodyIndex, spriteIndex, i] == "")
                {
                    //Set the empty variable to the next's on the list.
                    _SpriteName[bodyIndex, spriteIndex, i] = _SpriteName[bodyIndex, spriteIndex, i + 1];

                    //Make the next empty and do it all again.
                    _SpriteName[bodyIndex, spriteIndex, i + 1] = "";
                }
            }
        }

        /// <summary>
        /// Sort the SpriteName array.
        /// </summary>
        /// <param name="spriteIndex"></param>
        public void SortSpriteList(int bodyIndex)
        {
            //Sort the array by using a for loop.
            for (int i = 0; i < _SpriteCount[bodyIndex]; i++)
            {
                //Check if it's empty.
                if (_SpriteName[bodyIndex, i, 0] == null)
                {
                    //Set the empty variable to the next's on the list.
                    _SpriteName[bodyIndex, i, 0] = _SpriteName[bodyIndex, i + 1, 0];
                    //Load the Sprite.
                    _SpriteTexture[bodyIndex, i] = _SpriteTexture[bodyIndex, i + 1];
                    //Set the Position.
                    _SpritePosition[bodyIndex, i] = _SpritePosition[bodyIndex, i + 1];
                    //Set the Offset.
                    _SpriteOffset[bodyIndex, i] = _SpriteOffset[bodyIndex, i + 1];
                    //Set the Rotation Offset.
                    _SpriteRotationOffset[bodyIndex, i] = _SpriteRotationOffset[bodyIndex, i + 1];
                    //Set the Time Per Frame.
                    _TimePerFrame[bodyIndex, i] = _TimePerFrame[bodyIndex, i + 1];
                    //The Sprite's Scale.
                    _SpriteScale[bodyIndex, i] = _SpriteScale[bodyIndex, i + 1];
                    //The Sprite's Depth.
                    _SpriteDepth[bodyIndex, i] = _SpriteDepth[bodyIndex, i + 1];
                    //Set the Rotation.
                    _SpriteRotation[bodyIndex, i] = _SpriteRotation[bodyIndex, i + 1];
                    //Set the Visibility.
                    _SpriteVisibility[bodyIndex, i] = _SpriteVisibility[bodyIndex, i + 1];
                    //Set the Orientation.
                    _SpriteOrientation[bodyIndex, i] = _SpriteOrientation[bodyIndex, i + 1];

                    //Loop through all frames in the sprite.
                    for (int frame = 1; frame < _SpriteFrameCount[bodyIndex, i + 1]; frame++)
                    {
                        //Set the empty variable to the next's on the list.
                        _SpriteName[bodyIndex, i, frame] = _SpriteName[bodyIndex, i + 1, frame];
                        //Set the Sprite's Size.
                        _SpriteWidth[bodyIndex, i, frame] = _SpriteWidth[bodyIndex, i + 1, frame];
                        _SpriteHeight[bodyIndex, i, frame] = _SpriteHeight[bodyIndex, i + 1, frame];
                        //Set the Origin.
                        _SpriteOrigin[bodyIndex, i, frame] = _SpriteOrigin[bodyIndex, i + 1, frame];
                    }

                    //Make the next empty and do it all again.
                    _SpriteName[bodyIndex, i + 1, 0] = null;
                    //Load the Sprite.
                    _SpriteTexture[bodyIndex, i + 1] = null;
                    //Set the Position.
                    _SpritePosition[bodyIndex, i + 1] = Vector2.Zero;
                    //Set the Offset.
                    _SpriteOffset[bodyIndex, i + 1] = 0;
                    //Set the Rotation Offset.
                    _SpriteRotationOffset[bodyIndex, i + 1] = 0;
                    //Set the Time Per Frame.
                    _TimePerFrame[bodyIndex, i + 1] = 0;
                    //The Sprite's Scale.
                    _SpriteScale[bodyIndex, i + 1] = 0;
                    //The Sprite's Depth.
                    _SpriteDepth[bodyIndex, i + 1] = 0;
                    //Set the Rotation.
                    _SpriteRotation[bodyIndex, i + 1] = 0;
                    //Set the Visibility.
                    _SpriteVisibility[bodyIndex, i + 1] = Visibility.Visible;
                    //Set the Orientation.
                    _SpriteOrientation[bodyIndex, i + 1] = Orientation.Right;

                    //Loop through all frames in the sprite.
                    for (int frame = 1; frame < _SpriteFrameCount[bodyIndex, i]; frame++)
                    {
                        //Set the empty variable to the next's on the list.
                        _SpriteName[bodyIndex, i + 1, frame] = null;
                        //Set the Sprite's Size.
                        _SpriteWidth[bodyIndex, i + 1, frame] = 0;
                        _SpriteHeight[bodyIndex, i + 1, frame] = 0;
                        //Set the Origin.
                        _SpriteOrigin[bodyIndex, i + 1, frame] = Vector2.Zero;
                    }
                }
            }
        }
        #endregion

        //Methods dealing with deleting Images.
        #region Deleting Images
        /// <summary>
        /// Delete a Frame from the Sprite.
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="frameName"></param>
        public void DeleteFrame(string spriteName, string frameName, int bodyIndex)
        {
            //Find the Sprite Index and save it to an integer for further use.
            int spriteIndex = FindSpriteIndex(spriteName, bodyIndex, 0);
            //Find the SpriteFrame's Index and save it to an integer for further use.
            int spriteFrameIndex = FindFrameIndex(frameName, bodyIndex, spriteIndex);

            //Delete the SpriteFrame in the SpriteName array at the correct location by setting the value to 999.
            _SpriteName[bodyIndex, spriteIndex, spriteFrameIndex] = "";

            //Sort the SpriteName array.
            SortSpriteFrameList(bodyIndex, spriteIndex);
        }

        public void DeleteSprite(int bodyIndex, int spriteIndex)
        {
            //Delete the name.
            _SpriteName[bodyIndex, spriteIndex, 0] = null;
            //Delete the Sprite.
            _SpriteTexture[bodyIndex, spriteIndex] = null;
            //Delete the Position.
            _SpritePosition[bodyIndex, spriteIndex] = Vector2.Zero;
            //Delete the Offset.
            _SpriteOffset[bodyIndex, spriteIndex] = 0;
            //Delete the Rotation Offset.
            _SpriteRotationOffset[bodyIndex, spriteIndex] = 0;
            //Delete the Time Per Frame.
            _TimePerFrame[bodyIndex, spriteIndex] = 0;
            //The Sprite's Scale.
            _SpriteScale[bodyIndex, spriteIndex] = 0;
            //The Sprite's Depth.
            _SpriteDepth[bodyIndex, spriteIndex] = 0;
            //Delete the Rotation.
            _SpriteRotation[bodyIndex, spriteIndex] = 0;
            //Delete the Visibility.
            _SpriteVisibility[bodyIndex, spriteIndex] = Visibility.Visible;
            //Delete the Orientation.
            _SpriteOrientation[bodyIndex, spriteIndex] = Orientation.Right;
            //Subtract a count to the SpriteCount.
            _SpriteCount[bodyIndex]--;

            //Find each frame and delete it.
            for (int frame = 0; frame < _SpriteFrameCount[bodyIndex, spriteIndex]; frame++)
            {
                _SpriteName[bodyIndex, spriteIndex, frame] = null;
                _SpriteWidth[bodyIndex, spriteIndex, frame] = 0;
                _SpriteHeight[bodyIndex, spriteIndex, frame] = 0;
                _SpriteOrigin[bodyIndex, spriteIndex, frame] = Vector2.Zero;

                _SpriteFrameCount[bodyIndex, spriteIndex]--;
            }

            //Sort the SpriteName array.
            SortSpriteList(bodyIndex);
        }
        #endregion

        //Methods updating the images.
        #region Update Images
        /// <summary>
        /// Update the Sprite so that the Frame changes.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteIndex"></param>
        /// <param name="contentManager"></param>
        public void UpdateFrame(GameTime gameTime, int bodyIndex, int spriteIndex)
        {
            //Get the Time since the last Update.
            _TotalElapsedTime[bodyIndex, spriteIndex] += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //If it's time to change SpriteFrame.
            if (_TotalElapsedTime[bodyIndex, spriteIndex] > _TimePerFrame[bodyIndex, spriteIndex])
            {
                if (_SpriteAnimationDirection[bodyIndex, spriteIndex] == true)
                {
                    //If the drawn SpriteFrames are less than the total number of Frames.
                    if (_SpriteFrame[bodyIndex, spriteIndex] < _SpriteFrameEndIndex[bodyIndex, spriteIndex])
                    {
                        //Change to the next frame.
                        _SpriteFrame[bodyIndex, spriteIndex]++;
                    }
                    else
                    {
                        //Set the SpriteFrame to the starting index again, making the animation start over.
                        _SpriteFrame[bodyIndex, spriteIndex] = _SpriteFrameStartIndex[bodyIndex, spriteIndex];
                    }
                }
                else
                {
                    //If the drawn SpriteFrames are less than the total number of Frames.
                    if (_SpriteFrame[bodyIndex, spriteIndex] > _SpriteFrameStartIndex[bodyIndex, spriteIndex])
                    {
                        //Change to the next frame.
                        _SpriteFrame[bodyIndex, spriteIndex]--;
                    }
                    else
                    {
                        //Set the SpriteFrame to the starting index again, making the animation start over.
                        _SpriteFrame[bodyIndex, spriteIndex] = _SpriteFrameEndIndex[bodyIndex, spriteIndex];
                    }
                }

                //Substract the TimePerFrame, to be certain the next Frame is drawn in time.
                _TotalElapsedTime[bodyIndex, spriteIndex] -= _TimePerFrame[bodyIndex, spriteIndex];

                //Load the correct SpriteFrame.
                LoadSprite(bodyIndex, spriteIndex);
            }
        }
        #endregion

        public void LoadSprite(int bodyIndex, int spriteIndex)
        {
            //Load the Sprite.
            _SpriteTexture[bodyIndex, spriteIndex] = _ContentManager.Load<Texture2D>
                    (_SpriteName[bodyIndex, spriteIndex, _SpriteFrame[bodyIndex, spriteIndex]]);
        }

        public void DrawSpriteFunction(int bodyIndex, int spriteIndex)
        {
            if (_SpriteOrientation[bodyIndex, spriteIndex] == Orientation.Right)
            {
                //Draw the sprite.
                _SpriteBatch.Draw(_SpriteTexture[bodyIndex, spriteIndex], _SpritePosition[bodyIndex, spriteIndex],
                    null, Color.White, _SpriteRotation[bodyIndex, spriteIndex],
                    _SpriteOrigin[bodyIndex, spriteIndex, _SpriteFrame[bodyIndex, spriteIndex]],
                    _SpriteScale[bodyIndex, spriteIndex], SpriteEffects.None, 0);
            }
            else if (_SpriteOrientation[bodyIndex, spriteIndex] == Orientation.Left)
            {
                //Draw the sprite flipped on the horizontal axis.
                _SpriteBatch.Draw(_SpriteTexture[bodyIndex, spriteIndex], _SpritePosition[bodyIndex, spriteIndex],
                    null, Color.White, _SpriteRotation[bodyIndex, spriteIndex],
                    _SpriteOrigin[bodyIndex, spriteIndex, _SpriteFrame[bodyIndex, spriteIndex]],
                    _SpriteScale[bodyIndex, spriteIndex], SpriteEffects.FlipHorizontally, 0);
            }
        }

        public float AddAngles(float radian1, float radian2)
        {
            float addResult = radian1 + radian2;

            if (addResult > (Math.PI * 2)) { return (addResult - ((float)Math.PI * 2)); }
            else { return addResult; }
        }

        public float SubtractAngles(float radian1, float radian2)
        {
            float subtractResult = radian1 - radian2;

            if (subtractResult < 0) { return (subtractResult + ((float)Math.PI * 2)); }
            else { return subtractResult; }
        }

        public Vector2 CalculateOrbitPosition(Vector2 position, float rotation, float offset)
        {
            Vector2 vector = Vector2.Add(Vector2.Multiply(Calculator.RadiansToVector(rotation), offset), position);

            return vector;
        }

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion

        #endregion

        //The Sprite Properties.
        #region Properies
        //The ContentManager.
        public ContentManager ContentManager
        {
            get { return (_ContentManager); }
            set { _ContentManager = value; }
        }

        //The SpriteBatch.
        public SpriteBatch SpriteBatch
        {
            get { return (_SpriteBatch); }
            set { _SpriteBatch = value; }
        }

        //The SpriteTexture Texture2D.
        public Texture2D[,] SpriteTexture
        {
            get { return (_SpriteTexture); }
            set { _SpriteTexture = value; }
        }

        //The SpritePosition Vector2.
        public Vector2[,] SpritePosition
        {
            get { return (_SpritePosition); }
            set { _SpritePosition = value; }
        }

        //The SpriteName string.
        public string[, ,] SpriteName
        {
            get { return (_SpriteName); }
            set { _SpriteName = value; }
        }

        //The SpriteCount int.
        public int[] SpriteCount
        {
            get { return (_SpriteCount); }
            set { _SpriteCount = value; }
        }

        //The SpriteFrame int.
        public int[,] SpriteFrame
        {
            get { return (_SpriteFrame); }
            set { _SpriteFrame = value; }
        }

        //The SpriteFrameCount int.
        public int[,] SpriteFrameCount
        {
            get { return (_SpriteFrameCount); }
            set { _SpriteFrameCount = value; }
        }

        //The TimePerFrame float.
        public float[,] TimePerFrame
        {
            get { return (_TimePerFrame); }
            set { _TimePerFrame = value; }
        }

        //The SpriteFrameStartIndex int.
        public int[,] SpriteFrameStartIndex
        {
            get { return (_SpriteFrameStartIndex); }
            set { _SpriteFrameStartIndex = value; }
        }

        //The SpriteFrameEndIndex int.
        public int[,] SpriteFrameEndIndex
        {
            get { return (_SpriteFrameEndIndex); }
            set { _SpriteFrameEndIndex = value; }
        }

        //The SpriteAnimation bool.
        public bool[,] SpriteAnimation
        {
            get { return (_SpriteAnimation); }
            set { _SpriteAnimation = value; }
        }

        //The SpriteAnimationDirection bool.
        public bool[,] SpriteAnimationDirection
        {
            get { return (_SpriteAnimationDirection); }
            set { _SpriteAnimationDirection = value; }
        }

        //The TotalElapsedTime float.
        public float[,] TotalElapsedTime
        {
            get { return (_TotalElapsedTime); }
            set { _TotalElapsedTime = value; }
        }

        //The SpriteRotation float.
        public float[,] SpriteRotation
        {
            get { return (_SpriteRotation); }
            set { _SpriteRotation = value; }
        }

        //The SpriteScale float.
        public float[,] SpriteScale
        {
            get { return (_SpriteScale); }
            set { _SpriteScale = value; }
        }

        //The SpriteDepth int.
        public int[,] SpriteDepth
        {
            get { return (_SpriteDepth); }
            set { _SpriteDepth = value; }
        }

        //The SpriteOrigin Vector2.
        public Vector2[, ,] SpriteOrigin
        {
            get { return (_SpriteOrigin); }
            set { _SpriteOrigin = value; }
        }

        //The SpriteWidth float.
        public float[, ,] SpriteWidth
        {
            get { return (_SpriteWidth); }
            set { _SpriteWidth = value; }
        }

        //The SpriteHeight float.
        public float[, ,] SpriteHeight
        {
            get { return (_SpriteHeight); }
            set { _SpriteHeight = value; }
        }

        //The SpriteOffset float.
        public float[,] SpriteOffset
        {
            get { return (_SpriteOffset); }
            set { _SpriteOffset = value; }
        }

        //The SpriteRotationOffset float.
        public float[,] SpriteRotationOffset
        {
            get { return (_SpriteRotationOffset); }
            set { _SpriteRotationOffset = value; }
        }

        //The SpriteVisibility Visibility.
        public Visibility[,] SpriteVisibility
        {
            get { return (_SpriteVisibility); }
            set { _SpriteVisibility = value; }
        }

        //The SpriteOrientation Orientation.
        public Orientation[,] SpriteOrientation
        {
            get { return (_SpriteOrientation); }
            set { _SpriteOrientation = value; }
        }

        #endregion
    }
}