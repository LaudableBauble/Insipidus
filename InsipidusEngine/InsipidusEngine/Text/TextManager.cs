using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InsipidusEngine.Text
{
    //The TextManager Event Handler.
    public delegate void TextManagerEventHandler(object source);

    class TextManager
    {
        //The Fields.
        #region Fields
        //The TextBox Rectangle.
        private Rectangle _TextBox;
        //The main Text.
        private string _MainText;
        //The List of text. Each row in the List represents a new typed row.
        private List<string> _TextList;
        //The Cloned Text that the TextManager works with.
        private string _TextClone;
        //The Current Word.
        private string _CurrentWord;
        //The Index of the Current letter.
        private int _Index;
        //The Current Row.
        private int _CurrentRow;
        //The Update Delay.
        private float _UpdateDelay;
        //The Current Update Delay.
        private float _CurrentUpdateDelay;
        //The Update Delay Multiplier.
        private float _UpdateDelayMultiplier;
        //If the TextManager should wait for user Input.
        private bool _WaitForInput;
        //The Size of the used font.
        private float _FontSize;
        //The space between the rows.
        private float _RowSeperation;
        //If the TextManager should Exit.
        private bool _Exit;
        //The TextBoxEvent.
        private TextBoxEvent _TextBoxEvent;
        //The TextBoxCloseMode.
        private TextBoxCloseMode _TextBoxCloseMode;

        //Declaring the event.
        public event TextManagerEventHandler TextPrinted;
        #endregion

        //The Properties.
        #region Properties
        public Rectangle TextBox
        {
            get { return _TextBox; }
            set { _TextBox = value; }
        }
        public int Width
        {
            get { return _TextBox.Width; }
            set { _TextBox.Width = value; }
        }
        public int Height
        {
            get { return _TextBox.Height; }
            set { _TextBox.Height = value; }
        }
        public string Text
        {
            get { return _MainText; }
            set { _MainText = value; }
        }
        public List<string> TextList
        {
            get { return _TextList; }
            set { _TextList = value; }
        }
        public string CurrentWord
        {
            get { return _CurrentWord; }
            set { _CurrentWord = value; }
        }
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        public int CurrentRow
        {
            get { return _CurrentRow; }
            set { _CurrentRow = value; }
        }
        public float UpdateDelay
        {
            get { return _UpdateDelay; }
            set { _UpdateDelay = value; }
        }
        public float UpdateDelayMultiplier
        {
            get { return _UpdateDelayMultiplier; }
            set { _UpdateDelayMultiplier = value; }
        }
        public bool WaitForInput
        {
            get { return _WaitForInput; }
            set { _WaitForInput = value; }
        }
        public float FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }
        public float RowSeperation
        {
            get { return _RowSeperation; }
            set { _RowSeperation = value; }
        }
        public bool Exit
        {
            get { return _Exit; }
            set { _Exit = value; }
        }
        public TextBoxEvent TextBoxEvent
        {
            get { return _TextBoxEvent; }
            set { _TextBoxEvent = value; }
        }
        public TextBoxCloseMode TextBoxCloseMode
        {
            get { return _TextBoxCloseMode; }
            set { _TextBoxCloseMode = value; }
        }
        #endregion

        //The Constructors.
        #region Constructors
        public TextManager(string text, Rectangle box)
        {
            //The Textbox.
            _TextBox = box;
            //The Main text.
            _MainText = text;
            //The Cloned Text.
            _TextClone = _MainText;
            //The Current Word.
            _CurrentWord = _TextClone.Substring(0, _TextClone.IndexOf(" "));
            //The Index.
            _Index = 0;
            //The Current Row.
            _CurrentRow = 0;
            //The Update Delay.
            _UpdateDelay = 3;
            //The WaitForInput bool.
            _WaitForInput = false;
            //The way the TextBox should close.
            _TextBoxCloseMode = TextBoxCloseMode.Automatic;
            //Initialize the TextList.
            _TextList = new List<string>();
            //Fill the TextList.
            _TextList.Add("");
            //The Font Size.
            _FontSize = 8;
            //The Row Seperation Add.
            _RowSeperation = 5;
        }
        public TextManager(string text, Rectangle box, float fontSize, float rowSeperationAdd, float textDelay,
            TextBoxCloseMode closeMode)
        {
            //The Textbox.
            _TextBox = box;
            //The Main text.
            _MainText = text;
            //The Cloned Text.
            _TextClone = _MainText;
            //The Current Word.
            _CurrentWord = _TextClone.Substring(0, _TextClone.IndexOf(" "));
            //The Index.
            _Index = 0;
            //The Current Row.
            _CurrentRow = 0;
            //The Update Delay.
            _UpdateDelay = 3;
            //The WaitForInput bool.
            _WaitForInput = false;
            //The way the TextBox should close.
            _TextBoxCloseMode = closeMode;
            //Initialize the TextList.
            _TextList = new List<string>();
            //Fill the TextList.
            _TextList.Add("");
            //The Font Size.
            _FontSize = fontSize;
            //The Row Seperation Add.
            _RowSeperation = rowSeperationAdd;
        }
        #endregion

        //The Methods.
        #region Methods
        //Initialize.
        public void Initialize()
        {
            //The UpdateDelayMultiplier.
            _UpdateDelayMultiplier = 1;
            //The Current UpdateDelay.
            _CurrentUpdateDelay = (_UpdateDelay * _UpdateDelayMultiplier);
        }
        //Load all Content.
        public void LoadContent(ContentManager contentManager, SpriteBatch spriteBatch)
        {

        }
        //Handle all Input.
        public void HandleInput(InputState input)
        {
            //Check if it is waiting for user Input.
            if (_WaitForInput)
            {
                //Check for Input.
                if (input.MainSelect)
                {
                    //Check for what event should be happening.
                    switch (_TextBoxEvent)
                    {
                        case (TextBoxEvent.ClearTextBox):
                            {
                                //Clear the Text Box.
                                ClearTextBox();
                                //Exit.
                                break;
                            }
                        case (TextBoxEvent.ScrollRowsDown):
                            {
                                //Scroll the rows one step.
                                ScrollRows();
                                //Exit.
                                break;
                            }
                        case (TextBoxEvent.EndTextBox):
                            {
                                //Exit.
                                _Exit = true;
                                //Call the TextPrinted Event.
                                TextPrinted(this);

                                //Exit.
                                break;
                            }
                    }
                    //Disable the WaitForInput.
                    _WaitForInput = false;
                    //Reset the TextBoxEvent.
                    _TextBoxEvent = TextBoxEvent.None;
                }
            }
        }
        //Update.
        public void Update()
        {
            //If the TextManager shouldn't exit.
            if (!_Exit)
            {
                //Check if it isn't waiting for user input.
                if (!_WaitForInput)
                {
                    //Check if it is time to add a letter.
                    if (_CurrentUpdateDelay <= 0)
                    {
                        //Add a letter.
                        AddLetter();
                        //Add to the Index counter.
                        _Index++;

                        //Update the Text.
                        UpdateText();
                        //Reset the Text Update Delay.
                        _CurrentUpdateDelay = (_UpdateDelay * _UpdateDelayMultiplier);
                    }
                    //Subtract from the Update Delay.
                    else { _CurrentUpdateDelay--; }
                }
            }
            //Exit the TextManager.
            else { }
        }
        //Draw the Text.
        public void Draw()
        {

        }

        //Update the Text.
        public void UpdateText()
        {
            //Check if the letter is completely printed.
            if (_TextList[_CurrentRow].EndsWith(_CurrentWord))
            {
                //Check if there are more words.
                if ((_TextClone.IndexOf(_CurrentWord) + _CurrentWord.Length) < _TextClone.Length)
                {
                    //Find the next word.
                    NextWord();

                    //Check what type of action is ordered.
                    switch (_TextBoxEvent)
                    {
                        //If no action is ordered.
                        case (TextBoxEvent.None):
                            {
                                //Check if the new word doesn't fit.
                                if (!WillWordFit(_CurrentWord, _CurrentRow))
                                {
                                    //Add a row or scrool the rows.
                                    AddRowOrScrollTextBox();
                                }
                                else
                                {
                                    //Add the white-space.
                                    AddLetter();
                                    //Increment the Index counter.
                                    _Index++;
                                }
                                //End.
                                break;
                            }
                        //If a new line action is ordered.
                        case (TextBoxEvent.NewLine):
                            {
                                //Add a row or Reset the whole Text Box.
                                AddRowOrScrollTextBox();
                                //End.
                                break;
                            }
                        //If a clear box action is ordered.
                        case (TextBoxEvent.ClearTextBox):
                            {
                                //Wait for user input.
                                _WaitForInput = true;
                                //End.
                                break;
                            }
                        //If a delay multiplier change action is ordered.
                        case (TextBoxEvent.DelayMultiplierChange):
                            {
                                //Goto the default action.
                                goto case (TextBoxEvent.None);
                            }
                    }
                }
                //If there's no more words in the Text.
                else
                {
                    //Wait for user Input.
                    _WaitForInput = true;
                    //End the Text Box.
                    _TextBoxEvent = TextBoxEvent.EndTextBox;
                }
            }
        }
        //Check if the word will fit on the specified row.
        bool WillWordFit(string word, int row)
        {
            //Check if the word will fit.
            if ((_FontSize * word.Length) <= (_TextBox.Width - (_FontSize * _TextList[row].Length)))
            { return true; }
            else { return false; }
        }
        //Get a character at a given position from the current word.
        string GetCharacter(string text, int index)
        {
            //Return the character at the given position.
            return (_CurrentWord[_Index].ToString());
        }
        //Add another row to the text.
        void AddRow()
        {
            //Add a row.
            _CurrentRow++;
            _TextList.Add("");
        }
        //Add another letter to the "rolling" Text.
        void AddLetter()
        {
            //Add the letter to the TextList.
            _TextList[_CurrentRow] += GetCharacter(_CurrentWord, _Index);
        }
        //Decide if if it's time to add another row or begin scrolling them.
        void AddRowOrScrollTextBox()
        {
            //Create the char array, containing spaces.
            char[] space = { ' ' };
            //Trim the Current word so that the leading white-space is removed.
            _CurrentWord = _CurrentWord.TrimStart(space);

            //Check if there is room for another row.
            if (_CurrentRow < (((int)Math.Round((_TextBox.Height / _RowSeperation), 0)) - 1))
            {
                //Add a row.
                AddRow();
                //Reset the TextBoxEvent.
                _TextBoxEvent = TextBoxEvent.None;
            }
            else
            {
                //Wait for user Input.
                _WaitForInput = true;
                //Reset the Text Box.
                _TextBoxEvent = TextBoxEvent.ScrollRowsDown;
            }
        }
        //Add some text to the Main Text.
        public void AddText(string text)
        {
            //Add the text.
            _MainText = (_MainText + " " + text);
            //Clone the text.
            _TextClone = (_TextClone + " " + text);

            //If the cloned text is as big as the current word. (+1 = the space between the added texts)
            if ((_TextClone.Length - text.Length) <= (_CurrentWord.Length + 1))
            {
                //Update the Text.
                UpdateText();
                //Don't Exit.
                _Exit = false;
                //Clear the TextBox.
                ClearTextBox();
            }
        }
        //Add together two strings.
        string AddString(string text1, string text2)
        {
            //Return the added strings.
            return (text1 + text2);
        }
        //Find The Next Word in the Text.
        public void NextWord()
        {
            //Find the next word.
            FindNextWord();
            //Check for action code in the word.
            CheckCodeAction();
            //Reset the Index counter.
            _Index = 0;
        }
        //Find the Next Word in he Text.
        void FindNextWord()
        {
            //Remove the word from the cloned text.
            _TextClone = _TextClone.Remove(0, _CurrentWord.Length);

            //Find the index for the next white-space.
            int a = _TextClone.IndexOf(" ", 1);

            //Check if a is negative, if there are no more words in the text.
            if (a < 0) { a = _TextClone.Length; }
            //Else, increment a to cope with the transition from index counting to length counting.
            else { a++; }

            //Single out the next word and save it.
            _CurrentWord = (_TextClone.Substring(0, a));
        }
        //Clear the TextList.
        public void ClearTextBox()
        {
            //Clear the TextList.
            _TextList.Clear();
            //Add a blank to the TextList.
            _TextList.Add("");
            //Reset the row count.
            _CurrentRow = 0;
        }
        //Scroll Rows.
        void ScrollRows()
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
        //Check for codes in the Text and apply specific actions.
        void CheckCodeAction()
        {
            //Check for action code in the text.
            switch (_CurrentWord.Trim())
            {
                //New Line.
                case ("/n"):
                    {
                        //Add a new line.
                        _TextBoxEvent = TextBoxEvent.NewLine;
                        //Find the next word.
                        FindNextWord();

                        break;
                    }
                //Clear the Box.
                case ("/c"):
                    {
                        //Update the Delay Multiplier.
                        _TextBoxEvent = TextBoxEvent.ClearTextBox;
                        //Find the next word.
                        FindNextWord();

                        break;
                    }
                //Faster Text.
                case ("/f"):
                    {
                        //Update the Delay Multiplier.
                        _TextBoxEvent = TextBoxEvent.DelayMultiplierChange;
                        //Decrease the delay.
                        _UpdateDelayMultiplier -= .25f;
                        //Keep it within limits.
                        _UpdateDelayMultiplier = (float)Math.Max(_UpdateDelayMultiplier, .25);
                        //Find the next word.
                        FindNextWord();

                        break;
                    }
                //Slower Text.
                case ("/s"):
                    {
                        //Update the Delay Multiplier.
                        _TextBoxEvent = TextBoxEvent.DelayMultiplierChange;
                        //Decrease the delay.
                        _UpdateDelayMultiplier += .25f;
                        //Keep it within limits.
                        _UpdateDelayMultiplier = (float)Math.Min(_UpdateDelayMultiplier, 10);
                        //Find the next word.
                        FindNextWord();

                        break;
                    }
            }
        }
        #endregion
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PokemonGame.Text
{
    //The TextManager Event Handler.
    public delegate void TextManagerEventHandler(object source);

    class TextManager
    {
        #region Fields
        private TextBox _TextBox;
        private string _FullText;
        private string _TextClone;
        private string _CurrentWord;
        private int _Index;
        private int _CurrentRow;
        private float _UpdateDelay;
        private float _UpdateDelayMultiplier;
        private bool _WaitForInput;
        private TextBoxEvent _TextBoxEvent;
        private TextBoxAction _TextBoxAction;
        private TextBoxCloseMode _TextBoxCloseMode;
        private float _ArrowUpdateDelay;

        //Declaring the event.
        public event TextManagerEventHandler TextPrinted;
        #endregion

        #region Properties
        public TextBox TextBox
        {
            get { return _TextBox; }
            set { _TextBox = value; }
        }
        public string Text
        {
            get { return _FullText; }
            set { _FullText = value; }
        }
        public string CurrentWord
        {
            get { return _CurrentWord; }
            set { _CurrentWord = value; }
        }
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        public int CurrentRow
        {
            get { return _CurrentRow; }
            set { _CurrentRow = value; }
        }
        public float UpdateDelay
        {
            get { return _UpdateDelay; }
            set { _UpdateDelay = value; }
        }
        public float UpdateDelayMultiplier
        {
            get { return _UpdateDelayMultiplier; }
            set { _UpdateDelayMultiplier = value; }
        }
        public bool WaitForInput
        {
            get { return _WaitForInput; }
            set { _WaitForInput = value; }
        }
        public TextBoxEvent TextBoxEvent
        {
            get { return _TextBoxEvent; }
            set { _TextBoxEvent = value; }
        }
        public TextBoxAction TextBoxAction
        {
            get { return _TextBoxAction; }
            set { _TextBoxAction = value; }
        }
        public TextBoxCloseMode TextBoxCloseMode
        {
            get { return _TextBoxCloseMode; }
            set { _TextBoxCloseMode = value; }
        }
        public float ArrorUpdateDelay
        {
            get { return _ArrowUpdateDelay; }
            set { _ArrowUpdateDelay = value; }
        }
        #endregion

        #region Constructors
        public TextManager(SpriteFont font, string text)
        {
            _TextBox = new TextBox(font, 234f, 42f, 15f, 6f, new Vector2(200, 150), 10f, 3f);
            _FullText = text;
            _TextClone = _FullText;
            _CurrentWord = _TextClone.Substring(0, _TextClone.IndexOf(" "));
            _Index = 0;
            _CurrentRow = 0;
            _UpdateDelayMultiplier = 1;
            _UpdateDelay = (_TextBox.TextDelay * _UpdateDelayMultiplier);
            _WaitForInput = false;
            _ArrowUpdateDelay = _TextBox.ArrowUpdateDelay;
            _TextBoxCloseMode = TextBoxCloseMode.Automatic;
        }

        public TextManager(SpriteFont font, string text, float boxWidth, float boxHeight, float borderWidth,
            float borderHeight,Vector2 spritePosition, float letterSeperationAdd, float textDelay,
            TextBoxCloseMode closeMode)
        {
            _TextBox = new TextBox(font, boxWidth, boxHeight, borderWidth, borderHeight, spritePosition,
                letterSeperationAdd, textDelay);
            _FullText = text;
            _TextClone = _FullText;
            _CurrentWord = _TextClone.Substring(0, _TextClone.IndexOf(" "));
            _Index = 0;
            _CurrentRow = 0;
            _UpdateDelayMultiplier = 1;
            _UpdateDelay = (_TextBox.TextDelay * _UpdateDelayMultiplier);
            _WaitForInput = false;
            _ArrowUpdateDelay = _TextBox.ArrowUpdateDelay;
            _TextBoxCloseMode = closeMode;
        }
        #endregion

        #region Methods
        public void LoadContent(ContentManager contentManager, SpriteBatch spriteBatch)
        {
            //Load the Text Box Sprite.
            _TextBox.LoadContent(contentManager, spriteBatch);
        }
        public void HandleInput(InputState input)
        {
            //Check if it is waiting for user Input.
            if (_WaitForInput)
            {
                //Check for Input.
                if (input.MainSelect)
                {
                    //Check for what event should be happening.
                    switch (_TextBoxEvent)
                    {
                        case (TextBoxEvent.ClearTextBox):
                            {
                                //Clear the Text Box.
                                _TextBox.Clear();
                                //Reset the row count.
                                _CurrentRow = 0;
                                //Disable the WaitForInput.
                                _WaitForInput = false;
                                //Make the Message Arrow invisible.
                                _TextBox.Sprite.SpriteVisibility[0, 1] = Sprite.Visibility.Invisible;
                                //Exit.
                                break;
                            }
                        case (TextBoxEvent.ScrollRowsDown):
                            {
                                //Scroll the rows one step.
                                _TextBox.ScrollRows();
                                //Disable the WaitForInput.
                                _WaitForInput = false;
                                //Make the Message Arrow invisible.
                                _TextBox.Sprite.SpriteVisibility[0, 1] = Sprite.Visibility.Invisible;
                                //Exit.
                                break;
                            }
                        case (TextBoxEvent.EndTextBox):
                            {
                                //Disable the WaitForInput.
                                _WaitForInput = false;
                                //Call the TextPrinted Event.
                                TextPrinted(this);

                                //Check if the TextBoxCloseMode is automatic.
                                if (_TextBoxCloseMode == TextBoxCloseMode.Automatic)
                                {
                                    //Dispose the TextBox;
                                    _TextBox.Dispose();
                                    _TextBox = null;
                                }

                                //Exit.
                                break;
                            }
                    }
                }
            }
        }
        public void Update()
        {
            //Check if the TextBox exists.
            if (_TextBox != null)
            {
                //Check if it isn't waiting for user input.
                if (!_WaitForInput)
                {
                    //Check if it is time to add a letter.
                    if (_UpdateDelay <= 0)
                    {
                        //Add a letter.
                        AddLetter();
                        //Add to the Index counter.
                        _Index++;

                        //Update the Text.
                        UpdateText();
                        
                        //Reset the Text Update Delay.
                        _UpdateDelay = (_TextBox.TextDelay * _UpdateDelayMultiplier);
                    }
                    else
                    {
                        //Subtract from the Update Delay.
                        _UpdateDelay--;
                    }
                }
                else
                {
                    //Check if it is time to move the arrow.
                    if (_ArrowUpdateDelay <= 0)
                    {
                        //Animate the Arrow.
                        _TextBox.Sprite.SpritePosition[0, 1] = new Vector2(_TextBox.Sprite.SpritePosition[0, 1].X,
                            (_TextBox.Sprite.SpritePosition[0, 1].Y + (_TextBox.ArrowPositionMultiplier * 2)));
                        //Change the ArrowMultiplier.
                        _TextBox.ArrowPositionMultiplier = (_TextBox.ArrowPositionMultiplier * -1);
                        //Reset the ArrowUpdateDelay.
                        _ArrowUpdateDelay = _TextBox.ArrowUpdateDelay;
                    }
                    else
                    {
                        //Subtract from the ArrowUpdateDelay.
                        _ArrowUpdateDelay--;
                    }
                }
            }
        }
        public void Draw()
        {
            //Check if the TextBox exists.
            if (_TextBox != null) { _TextBox.Draw(); }
        }
        bool WillWordFit(string word, int row)
        {
            //The Length of the word.
            //float a = (word.Length * _TextBox.FontSize);
            //The max room on this row.
            //float b = (_TextBox.Width - (_TextBox.BorderWidth * 2));
            //The room taken on this row.
            //float c = (_TextBox.TextList[_CurrentRow].Length * _TextBox.FontSize);
            //The room left on this row.
            //float d = (b - c);

            //Check if the word will fit.
            if ((_TextBox.Font.MeasureString(word).X) <= 
                ((_TextBox.Width - (_TextBox.BorderLeft +  _TextBox.BorderRight)) -
                (_TextBox.Font.MeasureString(_TextBox.TextList[_CurrentRow]).X)))
            { return true; }
            else { return false; }
        }
        string GetCharacter(string text, int index)
        {
            //Return the caracter at the given position.
            return (_CurrentWord[_Index].ToString());
        }
        string AddString(string text1, string text2)
        {
            //Return the added strings.
            return (text1 + text2);
        }
        void FindNextWord()
        {
            //Remove the word from the cloned text.
            _TextClone = _TextClone.Remove(0, _CurrentWord.Length);

            //Find the index for the next white-space.
            int a = _TextClone.IndexOf(" ", 1);

            //Check if a is negative, if there are no more words in the text.
            if (a < 0) { a = _TextClone.Length; }
            else { a++; }

            //Single out the next word and return it.
            _CurrentWord = _TextClone.Substring(0, a);
        }
        void AddRow()
        {
            //Add a row.
            _CurrentRow++;
            _TextBox.AddRow();
        }
        void AddLetter()
        {
            //Add the letter.
            _TextBox.TextList[_CurrentRow] = AddString(_TextBox.TextList[_CurrentRow],
                GetCharacter(_CurrentWord, _Index));
        }
        void AddRowOrScrollTextBox()
        {
            //Check if there is room for another row.
            if (_CurrentRow < (_TextBox.RowCount - 1))
            {
                //Add a row.
                AddRow();
                //Create the char array.
                char[] space = { ' ' };
                //Trim the Current word so that the leading white-space is removed.
                _CurrentWord = _CurrentWord.TrimStart(space);
            }
            else
            {
                //Wait for user Input.
                _WaitForInput = true;
                //Create the char array.
                char[] space = { ' ' };
                //Trim the Current word so that the leading white-space is removed.
                _CurrentWord = _CurrentWord.TrimStart(space);
                //Reset the Text Box.
                _TextBoxEvent = TextBoxEvent.ScrollRowsDown;
                //Make the Message Arrow visible.
                _TextBox.Sprite.SpriteVisibility[0, 1] = Sprite.Visibility.Visible;
            }
        }
        void CheckCodeAction()
        {
            //Check for action code in the text.
            switch (_CurrentWord.Trim())
            {
                //New Line.
                case ("/n"):
                    {
                        //Add a new line.
                        _TextBoxAction = TextBoxAction.NewLine;
                        //Find the next word.
                        FindNextWord();
                        
                        break;
                    }
                //Clear the Box.
                case ("/c"):
                    {
                        //Update the Delay Multiplier.
                        _TextBoxAction = TextBoxAction.ClearBox;
                        //Find the next word.
                        FindNextWord();

                        break;
                    }
                //Faster Text.
                case ("/f"):
                    {
                        //Update the Delay Multiplier.
                        _TextBoxAction = TextBoxAction.DelayMultiplierChange;
                        //Decrease the delay.
                        _UpdateDelayMultiplier -= .25f;
                        //Keep it within limits.
                        _UpdateDelayMultiplier = (float)Math.Max(_UpdateDelayMultiplier, .25);
                        //Find the next word.
                        FindNextWord();

                        break;
                    }
                //Slower Text.
                case ("/s"):
                    {
                        //Update the Delay Multiplier.
                        _TextBoxAction = TextBoxAction.DelayMultiplierChange;
                        //Decrease the delay.
                        _UpdateDelayMultiplier += .25f;
                        //Keep it within limits.
                        _UpdateDelayMultiplier = (float)Math.Min(_UpdateDelayMultiplier, 10);
                        //Find the next word.
                        FindNextWord();

                        break;
                    }
            }
        }
        public void AddText(string text)
        {
            //Add the text.
            _FullText = (_FullText + " " + text);
            //Clone the text.
            _TextClone = (_TextClone + " " + text);
        }
        public void NextWord()
        {
            //Find the next word.
            FindNextWord();

            //Check for action code in the word.
            CheckCodeAction();

            //Reset the Index counter.
            _Index = 0;
        }
        public void UpdateText()
        {
            //Check if the letter is completely printed.
            if (_TextBox.TextList[_CurrentRow].EndsWith(_CurrentWord))
            {
                //Check if there are more words.
                if ((_TextClone.IndexOf(_CurrentWord) + _CurrentWord.Length) < _TextClone.Length)
                {
                    //Find the next word.
                    NextWord();

                    //Check what type of action is ordered.
                    switch (_TextBoxAction)
                    {
                        //If no action is ordered.
                        case (TextBoxAction.None):
                            {
                                //Check if the new word doesn't fit.
                                if (!WillWordFit(_CurrentWord, _CurrentRow))
                                {
                                    //Add a row or Reset the whole Text Box.
                                    AddRowOrScrollTextBox();
                                }
                                else
                                {
                                    //Add the white-space.
                                    AddLetter();
                                    //Add to the Index counter.
                                    _Index++;
                                }
                                //End.
                                break;
                            }
                        //If a new line action is ordered.
                        case (TextBoxAction.NewLine):
                            {
                                //Add a row or Reset the whole Text Box.
                                AddRowOrScrollTextBox();

                                //End.
                                break;
                            }
                        //If a clear box action is ordered.
                        case (TextBoxAction.ClearBox):
                            {
                                //Wait for user input.
                                _WaitForInput = true;
                                //Activate the Clear Box Event.
                                _TextBoxEvent = TextBoxEvent.ClearTextBox;

                                //End.
                                break;
                            }
                        //If a delay multiplier change action is ordered.
                        case (TextBoxAction.DelayMultiplierChange):
                            {
                                //Goto the default action.
                                goto case (TextBoxAction.None);
                            }
                    }
                }
                else
                {
                    //Wait for user Input.
                    _WaitForInput = true;

                    //End the Text Box.
                    _TextBoxEvent = TextBoxEvent.EndTextBox;
                }
                //Reset the TextBoxAction.
                _TextBoxAction = TextBoxAction.None;
            }
        }
        #endregion
    }
}*/