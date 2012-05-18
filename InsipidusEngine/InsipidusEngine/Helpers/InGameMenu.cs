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
using Microsoft.Xna.Framework.Graphics;
using InsipidusEngine.Infrastructure;
#endregion

namespace InsipidusEngine
{

    /// <summary>
    /// Enum describes the menu state.
    /// </summary>
    public enum MenuState
    {
        TransitionOn,
        Active,
        Deactive,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// Base class for In Game Menu that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class InGameMenu
    {
        #region Fields

        List<List<SelectEntry>> menuEntries = new List<List<SelectEntry>>();
        int selectedEntry;
        int selectedColumn;
        string menuTitle;
        SpriteBatch spriteBatch;
        SpriteFont font;

        #endregion

        #region Properties


        /// <summary>
        /// Gets and sets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        public List<List<SelectEntry>> MenuEntries
        {
            get { return menuEntries; }
            set { menuEntries = value; }
        }

        /// <summary>
        /// Gets the current menu state.
        /// </summary>
        public MenuState MenuState 
        {
            get { return menuState; }
            set { menuState = value; }
        }

        MenuState menuState = MenuState.TransitionOn;

        /// <summary>
        /// Checks whether this menu is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (menuState == MenuState.TransitionOn ||
                        menuState == MenuState.Active);
            }
        }

        /// <summary>
        /// Gets and sets the SpriteBatch.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        /// <summary>
        /// Gets and sets the SpriteFont.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// Gets and sets the selected column value.
        /// </summary>
        public int SelectedColumn
        {
            get { return selectedColumn; }
            set { selectedColumn = value; }
        }

        /// <summary>
        /// Gets and sets the menu title value.
        /// </summary>
        public string MenuTitle
        {
            get { return menuTitle; }
            set { menuTitle = value; }
        }

        /// <summary>
        /// Gets and sets the selectedEntry value.
        /// </summary>
        public int SelectedEntry
        {
            get { return selectedEntry; }
            set { selectedEntry = value; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public InGameMenu()
        {
            this.menuTitle = "Menu";
            this.selectedEntry = 0;
            this.selectedColumn = 0;
        }

        /// <summary>
        /// LoadContent.
        /// </summary>
        public virtual void LoadContent(SpriteBatch spriteBatch, SpriteFont font)
        {
            this.spriteBatch = spriteBatch;
            this.font = font;
        }

        #endregion

        #region Handle Input
        
        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public virtual void HandleInput(InputState input)
        {
            //Check if the menu is active.
            if (IsActive)
            {
                // Move to the previous menu entry?
                if (input.MenuUp)
                {
                    selectedEntry--;

                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }

                // Move to the next menu entry?
                if (input.MenuDown)
                {
                    selectedEntry++;

                    if (selectedEntry >= menuEntries.Count)
                        selectedEntry = 0;
                }

                // Move to the left menu entry?
                if (input.MenuLeft)
                {
                    selectedColumn--;

                    if (selectedColumn < 0) { selectedColumn = (menuEntries.Count - 1); }
                }

                // Move to the right menu entry?
                if (input.MenuRight)
                {
                    selectedColumn++;

                    if (selectedColumn > (menuEntries.Count - 1)) { selectedColumn = 0; }
                }

                // Accept or cancel the menu?
                if (input.MenuSelect)
                {
                    OnSelectEntry(selectedEntry);
                }
                else if (input.MenuCancel)
                {
                    OnCancel();
                }
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedColumn][selectedEntry].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected virtual void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public virtual void Update()
        {
            // Update each nested MenuEntry object.
            for (int column = 0; column < menuEntries.Count; column++)
            {
                for (int entry = 0; entry < menuEntries.Count; entry++)
                {
                    bool isSelected = IsActive && (entry == selectedEntry);

                    menuEntries[column][entry].Update(isSelected);
                }
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public virtual void Draw()
        {
            //Check if the menu is active.
            if (IsActive)
            {
                // Draw each menu entry in turn.
                for (int column = 0; column < menuEntries.Count; column++)
                {
                    for (int entry = 0; entry < menuEntries.Count; entry++)
                    {
                        SelectEntry menuEntry = menuEntries[column][entry];

                        bool isSelected = IsActive && (entry == selectedEntry);

                        menuEntry.Draw(spriteBatch, font);
                    }
                }
            }
        }

        #endregion
    }
}
