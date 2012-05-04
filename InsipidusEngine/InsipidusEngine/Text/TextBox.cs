using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using InsipidusEngine.Imagery;

namespace InsipidusEngine.Text
{
    class TextBox : IDisposable
    {
        //The Fields.
        #region Fields
        //The TextBox Sprite.
        private SpriteOld _Sprite;
        //The Font.
        private SpriteFont _Font;
        //The Font Size.
        private float _FontSize;
        //The TextManager.
        private TextManager _TextManager;
        //The Position of the TextBox.
        private Vector2 _SpritePosition;
        //The shape of the TextBox.
        private Rectangle _TextBox;
        //The shape of the TextArea.
        private Rectangle _TextArea;
        private float _TextDelay;
        private float _MessageArrowPositionMultiplier;
        private float _MessageArrowUpdateDelay;
        private float _CurrentArrowUpdateDelay;
        //The SpriteBatch.
        private SpriteBatch _SpriteBatch;
        #endregion

        //The Properties.
        #region Properties
        public SpriteOld Sprite
        {
            get { return _Sprite; }
            set { _Sprite = value; }
        }
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        public float FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }
        public TextManager TextManager
        {
            get { return _TextManager; }
            set { _TextManager = value; }
        }
        public Vector2 SpritePosition
        {
            get { return _SpritePosition; }
            set { _SpritePosition = value; }
        }
        public Rectangle Box
        {
            get { return _TextBox; }
            set { _TextBox = value; }
        }
        public Rectangle TextArea
        {
            get { return _TextArea; }
            set { _TextArea = value; }
        }
        public float TextDelay
        {
            get { return _TextDelay; }
            set { _TextDelay = value; }
        }
        public float ArrowPositionMultiplier
        {
            get { return _MessageArrowPositionMultiplier; }
            set { _MessageArrowPositionMultiplier = value; }
        }
        public float ArrowUpdateDelay
        {
            get { return _MessageArrowUpdateDelay; }
            set { _MessageArrowUpdateDelay = value; }
        }
        public SpriteBatch SpriteBatch
        {
            get { return _SpriteBatch; }
            set { _SpriteBatch = value; }
        }
        #endregion

        //The Constructors.
        #region Constructors
        public TextBox(string text, SpriteFont font)
        {
            //The Font.
            _Font = font;
            //The Shape of the TextBox.
            _TextBox = new Rectangle(200, 200, 200, 200);
            //The shape of the TextArea.
            _TextArea = new Rectangle(200, 200, 200, 200);
            //The position of the TextBox.
            _SpritePosition = FromTopLeftVector(_TextBox);
            //The Delay of the Text.
            _TextDelay = 3;

            //The Sprite.
            _Sprite = new SpriteOld();
            //The Font Size.
            _FontSize = ((_Font.MeasureString("tm").X + _Font.MeasureString("tm").Y) / 4);
            //The Arrow Position Multiplier.
            _MessageArrowPositionMultiplier = 1;
            //The Arrow Update Delay.
            _MessageArrowUpdateDelay = 6;
            //The TextManager.
            _TextManager = new TextManager(text, _TextArea, _FontSize, 10, _TextDelay, TextBoxCloseMode.Automatic);
        }
        public TextBox(string text, SpriteFont font, Rectangle textBox, Rectangle textArea, float rowSeperationAdd, float textDelay,
            TextBoxCloseMode closeMode)
        {
            //The Font.
            _Font = font;
            //The position of the TextBox.
            _SpritePosition = new Vector2(textBox.X, textBox.Y);
            //The Shape of the TextBox.
            _TextBox = ToTopLeftRectangle(textBox);
            //The shape of the TextArea.
            _TextArea = ToTopLeftRectangle(textArea);
            //The Delay of the Text.
            _TextDelay = textDelay;

            //The Sprite.
            _Sprite = new SpriteOld();
            //The Font Size.
            _FontSize = ((_Font.MeasureString("tm").X + _Font.MeasureString("tm").Y) / 4);
            //The Arrow Position Multiplier.
            _MessageArrowPositionMultiplier = 1;
            //The Arrow Update Delay.
            _MessageArrowUpdateDelay = 6;
            //The TextManager.
            _TextManager = new TextManager(text, _TextArea, _FontSize, rowSeperationAdd, textDelay, closeMode);
        }
        #endregion

        //The Methods.
        #region Methods
        //Initialize.
        public void Initialize()
        {
            //The TextManager.
            _TextManager.Initialize();
        }
        //Load the Content.
        public void LoadContent(ContentManager contentManager, SpriteBatch spriteBatch)
        {
            //Save the SpriteBatch for later use.
            _SpriteBatch = spriteBatch;

            //Load the Sprite.
            _Sprite.LoadSpriteContent(contentManager, spriteBatch);
            //Add the speech Bubble sprite.
            _Sprite.AddSprite("TextBox/SpeechBubble", 0, _SpritePosition, 1, 1, _TextBox.Width, _TextBox.Height, 0, 0, 0);
            //Add the Arrow sprite.
            _Sprite.AddSprite("TextBox/Arrow/MessageArrow[1]", 0,
                new Vector2((_SpritePosition.X + (_TextBox.Width / 2) - 13), (_SpritePosition.Y + (_TextBox.Height / 2) - 10)),
                1, 1, 10, 5, 1, 0, 0);
            //Make the arrow invisible.
            _Sprite.SpriteVisibility[0, 1] = SpriteOld.Visibility.Invisible;
        }
        //Handle all Input.
        public void HandleInput(InputState input)
        {
            //Handle The TextManager's Input.
            _TextManager.HandleInput(input);
        }
        //Update.
        public void Update()
        {
            //Update the TextManager.
            _TextManager.Update();

            //Check if it is time to move the arrow.
            if (_CurrentArrowUpdateDelay <= 0)
            {
                //Animate the Arrow.
                _Sprite.SpritePosition[0, 1] = new Vector2(_Sprite.SpritePosition[0, 1].X,
                    (_Sprite.SpritePosition[0, 1].Y + (_MessageArrowPositionMultiplier * 2)));
                //Change the ArrowMultiplier.
                _MessageArrowPositionMultiplier = (_MessageArrowPositionMultiplier * -1);
                //Reset the ArrowUpdateDelay.
                _CurrentArrowUpdateDelay = _MessageArrowUpdateDelay;
            }
            //Else, decrement the counter.
            else
            {
                //Subtract from the ArrowUpdateDelay.
                _CurrentArrowUpdateDelay--;
            }
        }
        //Draw the Text.
        public void Draw()
        {
            //Draw the Sprite.
            _Sprite.DrawSprite(0, 2);

            //Draw the text.
            for (int row = 0; row < _TextManager.TextList.Count; row++)
            {
                //Draw the Text for each row with the specified settings.
                /*_SpriteBatch.DrawString(_Font, _TextManager.TextList[row],
                    new Vector2((_SpritePosition.X + ((_TextBox.Width - _TextManager.Width) / 2)),
                        (_SpritePosition.Y - ((_TextBox.Height - _TextManager.Height) / 2)) +
                        (row * (_TextManager.FontSize + _TextManager.RowSeperation))), Color.Black);*/
                _SpriteBatch.DrawString(_Font, _TextManager.TextList[row],
                    new Vector2(_TextManager.TextBox.X, (_TextManager.TextBox.Y +
                        (row * (_TextManager.FontSize + _TextManager.RowSeperation)))), Color.Black);
            }
        }

        //The vertical border.
        public float VerticalBorder()
        {
            //Return the size of the vertical border.
            return ((_TextBox.Width - _TextManager.TextBox.Width) / 2);
        }
        //The horizontal border.
        public float HorizontalBorder()
        {
            //Return the size of the horizontal border.
            return ((_TextBox.Height - _TextManager.TextBox.Height) / 2);
        }
        //Add some Text.
        public void AddText(string text)
        {
            //Add some ttext to the TextManager.
            _TextManager.AddText(text);
        }
        //Convert the Position from top-left to middle.
        Vector2 FromTopLeftVector(Rectangle rect)
        {
            //Return the new Vector.
            return (new Vector2(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2)));
        }
        //Convert the Position from top-left to middle.
        Rectangle FromTopLeftRectangle(Rectangle rect)
        {
            //Return the new Vector.
            return (new Rectangle(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2), rect.Width, rect.Height));
        }
        //Convert the Position to top-left from middle.
        Vector2 ToTopLeftVector(Rectangle rect)
        {
            //Return the new Vector.
            return (new Vector2(rect.X - (rect.Width / 2), rect.Y - (rect.Height / 2)));
        }
        //Convert the Position to top-left from middle.
        Rectangle ToTopLeftRectangle(Rectangle rect)
        {
            //Return the new Vector.
            return (new Rectangle(rect.X - (rect.Width / 2), rect.Y - (rect.Height / 2), rect.Width, rect.Height));
        }
        //Dispose.
        public void Dispose()
        {
            //Dispose of the Sprite.
            _Sprite.Dispose();
            //Dispose of the Font.
            _Font = null;
            //Dispose of the TextManager.
            _TextManager.Exit = true;
            _TextManager = null;
        }
        #endregion
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PokemonGame.Text
{
    class TextBox : IDisposable
    {
        #region Fields
        private Sprite _Sprite;
        private SpriteFont _Font;
        private float _FontSize;
        private Vector2 _SpritePosition;
        private float _Width;
        private float _Height;
        private float _BorderLeft;
        private float _BorderRight;
        private float _BorderTop;
        private float _BorderDown;
        private int _RowCount;
        private List<string> _TextList;
        private List<Vector2> _TextPositionList;
        private float _TextDelay;
        private float _LetterSeperation;
        private float _MessageArrowPositionMultiplier;
        private float _MessageArrowUpdateDelay;
        private SpriteBatch _SpriteBatch;
        #endregion

        #region Properties
        public Sprite Sprite
        {
            get { return _Sprite; }
            set { _Sprite = value; }
        }
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        public float FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }
        public Vector2 SpritePosition
        {
            get { return _SpritePosition; }
            set { _SpritePosition = value; }
        }
        public float Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        public float Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        public float BorderLeft
        {
            get { return _BorderLeft; }
            set { _BorderLeft = value; }
        }
        public float BorderRight
        {
            get { return _BorderRight; }
            set { _BorderRight = value; }
        }
        public float BorderTop
        {
            get { return _BorderTop; }
            set { _BorderTop = value; }
        }
        public float BorderDown
        {
            get { return _BorderDown; }
            set { _BorderDown = value; }
        }
        public int RowCount
        {
            get { return _RowCount; }
            set { _RowCount = value; }
        }
        public List<string> TextList
        {
            get { return _TextList; }
            set { _TextList = value; }
        }
        public List<Vector2> TextPositionList
        {
            get { return _TextPositionList; }
            set { _TextPositionList = value; }
        }
        public float TextDelay
        {
            get { return _TextDelay; }
            set { _TextDelay = value; }
        }
        public float LetterSeperation
        {
            get { return _LetterSeperation; }
            set { _LetterSeperation = value; }
        }
        public float ArrowPositionMultiplier
        {
            get { return _MessageArrowPositionMultiplier; }
            set { _MessageArrowPositionMultiplier = value; }
        }
        public float ArrowUpdateDelay
        {
            get { return _MessageArrowUpdateDelay; }
            set { _MessageArrowUpdateDelay = value; }
        }
        public SpriteBatch SpriteBatch
        {
            get { return _SpriteBatch; }
            set { _SpriteBatch = value; }
        }
        #endregion

        #region Constructors
        public TextBox(SpriteFont font, float width, float height, float borderWidth, float borderHeight,
            Vector2 spritePosition, float letterSeperationAdd, float textDelay)
        {
            _Font = font;
            _FontSize = ((_Font.MeasureString("tm").X + _Font.MeasureString("tm").Y) / 4);
            _Width = width;
            _Height = height;
            _BorderLeft = borderWidth;
            _BorderTop = borderHeight;
            _SpritePosition = spritePosition;
            _LetterSeperation = (_FontSize + letterSeperationAdd);
            _TextDelay = textDelay;

            _RowCount = (int)Math.Round(((_Height - (_BorderTop * 2)) / _LetterSeperation), 0);
            _TextList = new List<string>();
            _TextPositionList = new List<Vector2>();
            _TextList.Capacity = _RowCount;
            _TextPositionList.Capacity = _RowCount;
            _TextList.Add("");
            _TextPositionList.Add(new Vector2(((_SpritePosition.X - (_Width / 2)) + _BorderLeft),
                ((_SpritePosition.Y - (_Height / 2)) + _BorderTop)));
            _MessageArrowPositionMultiplier = 1;
            _MessageArrowUpdateDelay = 6;

            //The Sprite.
            _Sprite = new Sprite();
        }
        #endregion

        #region Methods
        public void LoadContent(ContentManager contentManager, SpriteBatch spriteBatch)
        {
            //Save the SpriteBatch for later use.
            _SpriteBatch = spriteBatch;

            //Load the Sprite.
            _Sprite.LoadSpriteContent(contentManager, spriteBatch);
            //Add the speech Bubble sprite.
            _Sprite.AddSprite("TextBox/SpeechBubble", 0, _SpritePosition, 1, 1, _Width, _Height, 0, 0, 0);
            //Add the Arrow sprite.
            _Sprite.AddSprite("TextBox/Arrow/MessageArrow[1]", 0,
                new Vector2((_SpritePosition.X + (_Width / 2) - 13), (_SpritePosition.Y + (_Height / 2) - 10)),
                1, 1, 10, 5, 1, 0, 0);
            //Make the arrow invisible.
            _Sprite.SpriteVisibility[0, 1] = Sprite.Visibility.Invisible;
        }
        public void Draw()
        {
            //Draw the Sprite.
            _Sprite.DrawSprite(0, 2);

            //Draw the text.
            for (int row = 0; row < _TextList.Count; row++)
            {
                _SpriteBatch.DrawString(_Font, _TextList[row], _TextPositionList[row], Color.Black);
            }
        }
        public void Clear()
        {
            _TextList.Clear();
            _TextList.Add("");
        }
        public void ScrollRows()
        {
            //Loop through all the rows in the textList.
            for (int row = 1; row <= (_TextList.Count - 1); row++)
            {
                //Cycle the rows.
                _TextList[(row - 1)] = _TextList[row];
            }

            //Empty the last one.
            _TextList[(_TextList.Count - 1)] = "";
        }
        public void AddRow()
        {
            _TextList.Add("");
            _TextPositionList.Add(new Vector2((float)Math.Round((double)(((_SpritePosition.X - (_Width / 2)) +
                _BorderLeft)), 0),
                (float)Math.Round((double)(_TextPositionList[(_TextPositionList.Count - 1)].Y +
                _LetterSeperation), 0)));
        }
        public void Dispose()
        {
            _Sprite.Dispose();
            _Font = null;
        }
        #endregion
    }
}*/