#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace InsipidusEngine.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry worldEntry = new MenuEntry("World");
            MenuEntry novelBattleEntry = new MenuEntry("Novel Battle");
            MenuEntry classicBattleEntry = new MenuEntry("Classic Battle");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            worldEntry.Selected += OnWorldMenuEntrySelected;
            novelBattleEntry.Selected += NovelBattleMenuEntrySelected;
            classicBattleEntry.Selected += ClassicBattleMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(worldEntry);
            MenuEntries.Add(novelBattleEntry);
            MenuEntries.Add(classicBattleEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }
        #endregion

        #region Handle Input
        /// <summary>
        /// Event handler for when the World menu entry is selected.
        /// </summary>
        void OnWorldMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new WorldScreen());
        }
        /// <summary>
        /// Event handler for when the Novel Battle menu entry is selected.
        /// </summary>
        void NovelBattleMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen());
        }
        /// <summary>
        /// Event handler for when the Classic Battle menu entry is selected.
        /// </summary>
        void ClassicBattleMenuEntrySelected(object sender, EventArgs e)
        {
            Battle.BattleInfo info = new Battle.BattleInfo(BattleType.OneVsOne, new Party(), new Party());
            LoadingScreen.Load(ScreenManager, true, new BattleScreen(info));
        }
        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }
        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        #endregion
    }
}
