using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using InsipidusEngine.Imagery;

namespace InsipidusEngine.Battle
{
    class BattleHUD
    {
        #region Fields
        private Battle _Battle;
        private SpriteFont _Font;
        private Text.TextBox _TextBox;
        private SpriteOld _Sprite;
        private HealthBar _HealthBarOpponent;
        private HealthBar _HealthBarPlayer;
        private ExperienceBar _ExperienceBarPlayer;
        private BattleMenu _BattleMenu;
        #endregion

        #region Properties
        public Battle Battle
        {
            get { return _Battle; }
            set { _Battle = value; }
        }
        public SpriteFont Font
        {
            get { return _Font; }
            set { _Font = value; }
        }
        public Text.TextBox TextBox
        {
            get { return _TextBox; }
            set { _TextBox = value; }
        }
        public SpriteOld Sprite
        {
            get { return _Sprite; }
            set { _Sprite = value; }
        }
        public HealthBar HealthBarOpponent
        {
            get { return _HealthBarOpponent; }
            set { _HealthBarOpponent = value; }
        }
        public HealthBar HealthBarPlayer
        {
            get { return _HealthBarPlayer; }
            set { _HealthBarPlayer = value; }
        }
        public ExperienceBar ExperienceBarPlayer
        {
            get { return _ExperienceBarPlayer; }
            set { _ExperienceBarPlayer = value; }
        }
        public BattleMenu BattleMenu
        {
            get { return _BattleMenu; }
            set { _BattleMenu = value; }
        }
        #endregion

        #region Constructors
        public BattleHUD(Battle battle, SpriteFont font)
        {
            _Battle = battle;
            _Font = font; ;
            _TextBox = new Text.TextBox("A wild BULBASAUR has appeared!", _Font, new Rectangle(200, 179, 242, 46),
                new Rectangle(200, 179, 212, 26), 5, 10, TextBoxCloseMode.Manual);
            //Initialize the TextBox.
            _TextBox.Initialize();
            _TextBox.AddText("/c Go! CHARMANDER!");

            //Create the Sprite.
            //_Sprite = new SpriteOld();

            //Create the Healthbars.
            _HealthBarOpponent = new HealthBar(100, 100, 0.5f, new Vector2(153, 79), 48, 2);
            _HealthBarPlayer = new HealthBar(100, 100, 0.5f, new Vector2(280, 136), 47, 2);
            //Create the Experience Bar.
            _ExperienceBarPlayer = new ExperienceBar(100, 0, 0.5f, new Vector2(272, 153), 65, 3);

            //Create the BattleMenu.
            _BattleMenu = new BattleMenu("Battle", new Vector2(270, 179));
        }
        #endregion

        #region Methods
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager,
            SpriteBatch spriteBatch)
        {
            //Initialize the Events.
            _TextBox.TextManager.TextPrinted += new Text.TextManagerEventHandler(OnTextPrinted);

            //Load the Sprite.
            _Sprite.LoadSpriteContent(contentManager, spriteBatch);
            //Add the Background Sprite.
            _Sprite.AddSprite("Battle/Background/GrassBattleBackground[1]", 0, new Vector2(200, 100),
                1, 1, 241, 114, 0, 0, 0);
            //Add the Enemy Pokemon.
            _Sprite.AddSprite("Pokemon/Bulbasaur/BulbasaurFront[1]", 0, new Vector2(250, 100),
                1, 1, 34, 33, 1, 0, 0);
            //Add the Player Pokemon.
            _Sprite.AddSprite("Pokemon/Charmander/CharmanderBack[1]", 0, new Vector2(120, 139),
                1, 1, 46, 35, 1, 0, 0);
            //Add the Opponent's Healthbar.
            _Sprite.AddSprite("Battle/Menu/HealthbarOpponent[1]", 0, new Vector2(140, 75),
                1, 1, 100, 29, 2, 0, 0);
            //Add the Player's Healthbar.
            _Sprite.AddSprite("Battle/Menu/HealthbarPlayer[1]", 0, new Vector2(260, 137),
                1, 1, 103, 37, 2, 0, 0);
            //Add the Text Box Sprite.
            _Sprite.AddSprite("Battle/Menu/BattleTextBubble[1]", 0, new Vector2(200, 179),
                1, 1, 242, 46, 3, 0, 0);

            //Load the TextManager.
            _TextBox.LoadContent(contentManager, spriteBatch);
            //Make the TextBox Sprite invisible.
            _TextBox.Sprite.SpriteVisibility[0, 0] = Visibility.Invisible;

            //Load the Healthbars.
            _HealthBarOpponent.LoadContent(graphicsDevice, spriteBatch);
            _HealthBarPlayer.LoadContent(graphicsDevice, spriteBatch);
            //Load the Experience Bar.
            _ExperienceBarPlayer.LoadContent(graphicsDevice, spriteBatch);

            //Load the BattleMenu.
            _BattleMenu.LoadContent(contentManager, spriteBatch, _Font);
        }
        public void HandleInput(InputState input)
        {
            _TextBox.HandleInput(input);
            _BattleMenu.HandleInput(input);
        }
        public void Update(BattleStatus battleState)
        {
            //Check what Battle State the Battle is in.
            switch (battleState)
            {
                case (BattleStatus.Intro):
                    {
                        //Update the TextManager.
                        _TextBox.Update();

                        //Update the Healthbars.
                        _HealthBarOpponent.Update();
                        _HealthBarPlayer.Update();
                        //Update the Experience Bar.
                        _ExperienceBarPlayer.Update();

                        break;
                    }
                case (BattleStatus.BattleMenu):
                    {
                        //Update the TextManager.
                        _TextBox.Update();

                        //Update the Healthbars.
                        _HealthBarOpponent.Update();
                        _HealthBarPlayer.Update();
                        //Update the Experience Bar.
                        _ExperienceBarPlayer.Update();

                        //Update the BattleMenu.
                        _BattleMenu.Update();

                        break;
                    }
            }
        }
        public void Draw()
        {
            //Draw the Sprites.
            _Sprite.DrawSprite(0, _Sprite.SpriteCount[0]);

            //Draw the TextManager, aka SpeechBubble plus text.
            _TextBox.Draw();

            //Draw the Healtbars.
            _HealthBarOpponent.Draw();
            _HealthBarPlayer.Draw();
            //Draw the Experience Bar.
            _ExperienceBarPlayer.Draw();

            //Draw the BattleMenu.
            _BattleMenu.Draw();
        }

        public void OnTextPrinted(object obj)
        {
            //Check what Battle State the Battle is in.
            switch (_Battle.BattleState)
            {
                case (BattleStatus.Intro):
                    {
                        //End the Intro and start the BattleMenu phase.
                        _Battle.BattleState = BattleStatus.BattleMenu;
                        //Activate the BattleMenu.
                        _BattleMenu.MenuState = MenuState.Active;
                        //Change the TextBox Width.
                        _TextBox.TextManager.Width = (_TextBox.Box.Width -
                            (int)((_BattleMenu.Sprite.SpriteWidth[0, 0, 0] + 5) + (2 * _TextBox.VerticalBorder())));
                        //Add the What to do text.
                        _TextBox.AddText("/c What should CHARMANDER do?");
                        //Make the Message Arrow invisible.
                        _TextBox.Sprite.SpriteVisibility[0, 1] = Visibility.Invisible;

                        break;
                    }
            }
        }
        #endregion
    }
}