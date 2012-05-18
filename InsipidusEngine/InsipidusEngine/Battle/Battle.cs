using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using InsipidusEngine.Infrastructure;

namespace InsipidusEngine.Battle
{
    //The Battle Event Handler.
    public delegate void BattleEventHandler(object source);

    class Battle
    {
        #region Fields
        private BattleInfo _BattleInfo;
        private BattleIntro _BattleIntro;
        private BattleStatus _BattleState;
        private BattleHUD _BattleHUD;

        //Declaring the event.
        public event BattleEventHandler BattleStateChange;
        #endregion

        #region Properties
        public BattleInfo BattleInfo
        {
            get { return _BattleInfo; }
            set { _BattleInfo = value; }
        }
        public BattleIntro BattleIntro
        {
            get { return _BattleIntro; }
            set { _BattleIntro = value; }
        }
        public BattleStatus BattleState
        {
            get { return _BattleState; }
            set { _BattleState = value; }
        }
        public BattleHUD BattleHUD
        {
            get { return _BattleHUD; }
            set { _BattleHUD = value; }
        }
        #endregion

        #region Constructors
        public Battle(BattleInfo info, SpriteFont font)
        {
            _BattleInfo = info;
            _BattleState = BattleStatus.Intro;
            _BattleHUD = new BattleHUD(this, font);

            //Initialize the Events.
            BattleStateChange += new BattleEventHandler(OnBattleStateChange);
            BattleStateChange(this);
        }
        #endregion

        #region Methods
        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager,
            SpriteBatch spriteBatch)
        {
            //Load the Battle HUD.
            _BattleHUD.LoadContent(graphicsDevice, contentManager, spriteBatch);
        }

        public void HandleInput(InputState input)
        {
            _BattleHUD.HandleInput(input);
        }

        public void Update()
        {
            //Check what Battle State the Battle is in.
            switch (_BattleState)
            {
                case (BattleStatus.Intro):
                    {
                        //Update the BattleHUD.
                        _BattleHUD.Update(_BattleState);

                        break;
                    }
                case (BattleStatus.BattleMenu):
                    {
                        //Update the BattleHUD.
                        _BattleHUD.Update(_BattleState);

                        break;
                    }
            }
        }

        public void Draw()
        {
            //Draw the BattleHUD.
            _BattleHUD.Draw();
        }

        public void OnBattleStateChange(object obj)
        {
            //Check what Battle State the Battle is in.
            switch (_BattleState)
            {
                case (BattleStatus.Intro):
                    {
                        //Deactivate the BattleMenu.
                        _BattleHUD.BattleMenu.MenuState = MenuState.Deactive;

                        break;
                    }
                case (BattleStatus.BattleMenu):
                    {
                        //Activate the BattleMenu.
                        _BattleHUD.BattleMenu.MenuState = MenuState.Active;

                        break;
                    }
            }
        }

        #endregion
    }
}
