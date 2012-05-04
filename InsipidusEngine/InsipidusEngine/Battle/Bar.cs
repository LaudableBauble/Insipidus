using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InsipidusEngine.Battle
{
    public delegate void BarEventHandler(object source);

    public class Bar
    {
        #region Fields
        private Texture2D _ColorTexture;
        private Color[] _FillColor;
        private float _MaxValue;
        private float _MinValue;
        private float _CurrentValue;
        private float _GoalValue;
        private float _ChangeValue;
        private float _MaxWidth;
        private float _MaxHeight;
        private SpriteBatch _SpriteBatch;
        private Rectangle _Rectangle;

        //Declaring the event.
        public event BarEventHandler ValueChange;
        #endregion

        #region Constructors
        public Bar()
        {
            _MaxValue = 100;
            _MinValue = 0;
            _CurrentValue = _MaxValue;
            _GoalValue = _CurrentValue;
            _ChangeValue = 1;
            _MaxWidth = 100;
            _MaxHeight = 10;
            _Rectangle = new Rectangle(500, 200, (int)_MaxWidth, (int)_MaxHeight);

            Initialize(Color.Green);
        }

        public Bar(float maxValue, float minValue, float currentValue, float changeValue, float maxWidth, float maxHeight, Vector2 position, Color color)
        {
            _MaxValue = maxValue;
            _MinValue = minValue;
            _CurrentValue = currentValue;
            _GoalValue = _CurrentValue;
            _ChangeValue = changeValue;
            _MaxWidth = maxWidth;
            _MaxHeight = maxHeight;
            _Rectangle = new Rectangle((int)position.X, (int)position.Y, (int)_MaxWidth, (int)_MaxHeight);

            Initialize(color);
        }
        #endregion

        #region Methods
        public void Initialize(Color color)
        {
            //Initialize the Events.
            ValueChange += new BarEventHandler(OnValueChange);
            //Calculate the new size of the bar.
            Width = GetTransformedValue(_MaxWidth, _MaxValue, _MinValue, _CurrentValue);

            _FillColor = new Color[(int)(_MaxWidth * _MaxHeight)];
            FillArray(color);
        }
        public void LoadContent(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            //Save the SpriteBatch.
            _SpriteBatch = spriteBatch;

            //Create the Bar Texture.
            _ColorTexture = new Texture2D(graphicsDevice, (int)_MaxWidth, (int)_MaxHeight, false, SurfaceFormat.Color);
            //Change the color of the texture.
            _ColorTexture.SetData(_FillColor);
        }
        public virtual void Update()
        {
            //Check if it is greater than the goal value.
            if (_CurrentValue > _GoalValue)
            {
                //Subtract either the ChangeValue from it or what's left.
                _CurrentValue -= Math.Min(_ChangeValue, (_CurrentValue - _GoalValue));
                //Call the Event.
                ValueChange(this);
            }
            //Check if it is less than the goal value.
            else if (_CurrentValue < _GoalValue)
            {
                //Add either the ChangeValue to it or what's left.
                _CurrentValue += Math.Min(_ChangeValue, (_GoalValue - _CurrentValue));
                //Call the Event.
                ValueChange(this);
            }
        }
        public void Draw()
        {
            //Draw the Bar.
            _SpriteBatch.Draw(_ColorTexture, _Rectangle, Color.White);
        }
        void FillArray(Color color)
        {
            //Fill the array.
            for (int a = 0; a < (_MaxWidth * _MaxHeight); a++) { _FillColor[a] = color; }
        }
        void ChangeColor(Color color)
        {
            //Fill the array.
            FillArray(color);

            //Check if the Texture has been initialized.
            if (_ColorTexture != null)
            {
                //Initialize it..
                Texture2D textureClone = new Texture2D(_ColorTexture.GraphicsDevice, (int)_MaxWidth, (int)_MaxHeight, false, SurfaceFormat.Color);
                textureClone.SetData(_FillColor);
                _ColorTexture = textureClone;
            }
        }
        float GetTransformedValue(float toBeTransformedValue, float maxValue, float minValue, float currentValue)
        {
            //Check if the value is greater than zero.
            if (currentValue > _MinValue)
            {
                //Calculate the new value.
                return (toBeTransformedValue * (currentValue / (maxValue - minValue)));
            }
            else { return 0; }
        }
        public Vector2 ConvertVector(Point position)
        {
            //Convert the position to accomodate for top-left placement.
            return (new Vector2((position.X - (Width / 2)), (position.Y - (Height / 2))));
        }
        public Point ConvertVector(Vector2 position)
        {
            //Convert the position to accomodate for top-left placement.
            return (new Point((int)(position.X - (Width / 2)), (int)(position.Y - (Height / 2))));
        }
        public virtual void OnValueChange(object obj)
        {
            //Calculate the new size of the bar.
            Width = GetTransformedValue(_MaxWidth, _MaxValue, _MinValue, _CurrentValue);
        }
        /// <summary>
        /// Change the goal value.
        /// </summary>
        /// <param name="value">The new value to aim for.</param>
        private void ChangeGoalValueInvoke(float value)
        {
            //Switch to the new goal value, but make sure to stay within bounds.
            _GoalValue = MathHelper.Clamp(value, 0, _MaxValue);
        }
        #endregion

        #region Properties
        public Texture2D ColorTexture
        {
            get { return _ColorTexture; }
            set { _ColorTexture = value; }
        }
        public Color FillColor
        {
            get { return _FillColor[0]; }
            set { ChangeColor(value); }
        }
        public float MaxValue
        {
            get { return _MaxValue; }
            set { _MaxValue = value; }
        }
        public float MinValue
        {
            get { return _MinValue; }
            set { _MinValue = value; }
        }
        public float GoalValue
        {
            get { return _GoalValue; }
            set { ChangeGoalValueInvoke(value); }
        }
        public float ChangeValue
        {
            get { return _ChangeValue; }
            set { _ChangeValue = value; }
        }
        public float CurrentValue
        {
            get { return _CurrentValue; }
            set { _CurrentValue = value; ValueChange(this); }
        }
        public float MaxWidth
        {
            get { return _MaxWidth; }
            set { _MaxWidth = value; }
        }
        public float MaxHeight
        {
            get { return _MaxHeight; }
            set { _MaxHeight = value; }
        }
        public SpriteBatch SpriteBatch
        {
            get { return _SpriteBatch; }
            set { _SpriteBatch = value; }
        }
        public Rectangle Rectangle
        {
            get { return _Rectangle; }
            set { _Rectangle = value; }
        }
        public float Width
        {
            get { return (float)_Rectangle.Width; }
            set { _Rectangle.Width = (int)value; }
        }
        public float Height
        {
            get { return (float)_Rectangle.Height; }
            set { _Rectangle.Height = (int)value; }
        }
        public Vector2 Position
        {
            get { return new Vector2(_Rectangle.X, _Rectangle.Y); }
            set { _Rectangle.Location = new Point((int)value.X, (int)value.Y); }
        }
        #endregion
    }
}
