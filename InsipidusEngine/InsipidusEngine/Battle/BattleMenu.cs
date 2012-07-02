#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using InsipidusEngine.Imagery;
using InsipidusEngine.Helpers;
#endregion

namespace InsipidusEngine
{

    /// <summary>
    /// Class for a Battle Menu that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    class BattleMenu: InGameMenu
    {
        #region Fields
        private SpriteOld _Sprite;
        private Vector2 _Position;
        #endregion

        #region Properties
        public SpriteOld Sprite
        {
            get { return _Sprite; }
            set { _Sprite = value; }
        }
        public Vector2 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BattleMenu(string menuTitle, Vector2 position)
        {
            MenuTitle = menuTitle;
            SelectedEntry = 0;
            SelectedColumn = 0;
            _Position = position;

            //Create the Sprite.
            //_Sprite = new SpriteOld();

            //Create a clone of the Position.
            Vector2 p = _Position;

            // Create our menu entries.
            SelectEntry fightMenuEntry = new SelectEntry("Fight", new Vector2(p.X - 43, p.Y - 16));
            SelectEntry pokemonMenuEntry = new SelectEntry("Pokemon", new Vector2(p.X - 43, p.Y + 2));
            SelectEntry bagMenuEntry = new SelectEntry("Bag", new Vector2(p.X + 2, p.Y - 16));
            SelectEntry runMenuEntry = new SelectEntry("Run", new Vector2(p.X + 2, p.Y + 2));

            // Hook up menu event handlers.
            fightMenuEntry.Selected += FightMenuEntrySelected;
            pokemonMenuEntry.Selected += PokemonMenuEntrySelected;
            bagMenuEntry.Selected += BagMenuEntrySelected;
            runMenuEntry.Selected += RunMenuEntrySelected;

            //Create the MenuEntries List.
            MenuEntries = new List<List<SelectEntry>>();

            // Add entries lists to the menu.
            MenuEntries.Add(new List<SelectEntry>());
            MenuEntries.Add(new List<SelectEntry>());

            // Add entries to the menu.
            MenuEntries[0].Add(fightMenuEntry);
            MenuEntries[0].Add(pokemonMenuEntry);
            MenuEntries[1].Add(bagMenuEntry);
            MenuEntries[1].Add(runMenuEntry);
        }

        /// <summary>
        /// LoadContent.
        /// </summary>
        public void LoadContent(ContentManager contentManager, SpriteBatch spriteBatch, SpriteFont font)
        {
            base.LoadContent(spriteBatch, font);

            //Load the Sprite.
            _Sprite.LoadSpriteContent(contentManager, spriteBatch);
            //Add the Menu Sprite.
            _Sprite.AddSprite("Battle/Menu/BattleSelectMenu[1]", 0, _Position, 1, 1, 101, 45, 4, 0, 0);
            //Add the Selecter Sprite.
            _Sprite.AddSprite("Battle/Menu/BattleAttackSelecter[1]", 0, _Position, 1, 1, 44, 16, 5, 0, 0);
        }

        #endregion

        #region Handle Input
        
        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            //Move through the menu.
            base.HandleInput(input);

            //Update the Selecter's Position.
            _Sprite.SpritePosition[0, 1] =
                new Vector2(MenuEntries[SelectedColumn][SelectedEntry].Position.X - 1 +
                    (_Sprite.SpriteWidth[0, 1, 0] / 2), MenuEntries[SelectedColumn][SelectedEntry].Position.Y +
            (_Sprite.SpriteHeight[0, 1, 0] / 2) - 1);
        }

        /// <summary>
        /// Event handler for when the Fight menu entry is selected.
        /// </summary>
        void FightMenuEntrySelected(object sender, EventArgs e)
        {
            //Fight.
        }

        /// <summary>
        /// Event handler for when the Pokémon menu entry is selected.
        /// </summary>
        void PokemonMenuEntrySelected(object sender, EventArgs e)
        {
            //Pokémon.
        }

        /// <summary>
        /// Event handler for when the Bag menu entry is selected.
        /// </summary>
        void BagMenuEntrySelected(object sender, EventArgs e)
        {
            //Bag.
        }

        /// <summary>
        /// Event handler for when the Run menu entry is selected.
        /// </summary>
        void RunMenuEntrySelected(object sender, EventArgs e)
        {
            //Run.
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected override void OnCancel()
        {
            //Cancel.
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected override void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw()
        {
            //Check if the menu is active.
            if (IsActive)
            {
                //Draw the Menu Sprite.
                _Sprite.DrawSprite(0, 1);

                base.Draw();

                //Draw the Selecter Sprite.
                _Sprite.DrawSprite(1, _Sprite.SpriteCount[0]);
            }
        }


        #endregion
    }
}
