#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using InsipidusEngine.Imagery;
using InsipidusEngine.Battle;
using InsipidusEngine.Infrastructure;
#endregion

namespace InsipidusEngine.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields
        ContentManager content;
        Camera2D _Camera;
        Creature pokemon1;
        Creature pokemon2;
        HealthBar healthbar1;
        HealthBar healthbar2;
        Bar energybar1;
        Bar energybar2;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null) { content = new ContentManager(ScreenManager.Game.Services, "Content"); }

            #region Initialize
            //Create the camera.
            _Camera = new Camera2D(new Rectangle(0, 0, ScreenManager.Game.Window.ClientBounds.Width, ScreenManager.Game.Window.ClientBounds.Height),
                new Rectangle(0, 0, 2000, 2000));
            _Camera.Position = new Vector2(1000, 1000);

            pokemon1 = Factory.Instance.Pansear;
            pokemon1.Position = new Vector2(700, 1000);
            pokemon2 = Factory.Instance.Snivy;
            pokemon2.Position = new Vector2(1300, 1000);

            //Make the Pokemons target each other.
            pokemon1.Target = pokemon2;
            pokemon2.Target = pokemon1;

            //Initialize the healthbars.
            healthbar1 = new HealthBar(pokemon1.HP, pokemon1.CurrentHP, 1, new Vector2(5, 5), 100, 10);
            healthbar2 = new HealthBar(pokemon2.HP, pokemon2.CurrentHP, 1, new Vector2(ScreenManager.Game.Window.ClientBounds.Width - 105, 5), 100, 10);
            energybar1 = new Bar(pokemon1.MaxEnergy, 0, pokemon1.CurrentEnergy, 1, 100, 5, new Vector2(5, 20), Color.CornflowerBlue);
            energybar2 = new Bar(pokemon2.MaxEnergy, 0, pokemon2.CurrentEnergy, 1, 100, 5, new Vector2(ScreenManager.Game.Window.ClientBounds.Width - 105, 20), Color.CornflowerBlue);
            #endregion

            #region LoadContent
            //Load the battle coordinator's content.
            BattleCoordinator.Instance.LoadContent(content);

            //Bulbasaur's sprites.
            Sprite front = pokemon1.Sprite.Add(new Sprite(pokemon1.Sprite, "Front"));
            front.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Front[1]");
            front.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Front[2]");
            front.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Front[1]");
            front.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Front[3]");
            Sprite back = pokemon1.Sprite.Add(new Sprite(pokemon1.Sprite, "Back"));
            back.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Back[1]");
            back.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Back[2]");
            back.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Back[1]");
            back.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Back[3]");
            Sprite left = pokemon1.Sprite.Add(new Sprite(pokemon1.Sprite, "Left"));
            left.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Left[1]");
            left.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Left[2]");
            left.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Left[1]");
            left.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Left[3]");
            Sprite right = pokemon1.Sprite.Add(new Sprite(pokemon1.Sprite, "Right"));
            right.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Right[1]");
            right.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Right[2]");
            right.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Right[1]");
            right.AddFrame(@"Characters\Bulbasaur\Bulbasaur_Right[3]");

            //Charizard's sprites.
            front = pokemon2.Sprite.Add(new Sprite(pokemon2.Sprite, "Front"));
            front.AddFrame(@"Characters\Charizard\Charizard_Front[1]");
            front.AddFrame(@"Characters\Charizard\Charizard_Front[2]");
            front.AddFrame(@"Characters\Charizard\Charizard_Front[1]");
            front.AddFrame(@"Characters\Charizard\Charizard_Front[3]");
            back = pokemon2.Sprite.Add(new Sprite(pokemon2.Sprite, "Back"));
            back.AddFrame(@"Characters\Charizard\Charizard_Back[1]");
            back.AddFrame(@"Characters\Charizard\Charizard_Back[2]");
            back.AddFrame(@"Characters\Charizard\Charizard_Back[1]");
            back.AddFrame(@"Characters\Charizard\Charizard_Back[3]");
            left = pokemon2.Sprite.Add(new Sprite(pokemon2.Sprite, "Left"));
            left.AddFrame(@"Characters\Charizard\Charizard_Left[1]");
            left.AddFrame(@"Characters\Charizard\Charizard_Left[2]");
            left.AddFrame(@"Characters\Charizard\Charizard_Left[1]");
            left.AddFrame(@"Characters\Charizard\Charizard_Left[3]");
            right = pokemon2.Sprite.Add(new Sprite(pokemon2.Sprite, "Right"));
            right.AddFrame(@"Characters\Charizard\Charizard_Right[1]");
            right.AddFrame(@"Characters\Charizard\Charizard_Right[2]");
            right.AddFrame(@"Characters\Charizard\Charizard_Right[1]");
            right.AddFrame(@"Characters\Charizard\Charizard_Right[3]");

            //Make the sprites' invisible.
            pokemon1.Sprite.Visibility = Visibility.Invisible;
            pokemon2.Sprite.Visibility = Visibility.Invisible;

            //Set all sprite's time per frame.
            pokemon1.Sprite.Sprites.ForEach(item => item.TimePerFrame = .5f);
            pokemon2.Sprite.Sprites.ForEach(item => item.TimePerFrame = .5f);

            //Load the pokemons' content.
            pokemon1.LoadContent(content);
            pokemon2.LoadContent(content);

            //Load the bars' content.
            healthbar1.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
            healthbar2.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
            energybar1.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
            energybar2.LoadContent(ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
            #endregion

            // Once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }
        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                //Update the camera.
                _Camera.Update(gameTime);

                //Update the battle coordinator.
                BattleCoordinator.Instance.Update(gameTime);

                //Update the Pokemon.
                pokemon1.Update(gameTime);
                pokemon2.Update(gameTime);

                //Update the bars.
                healthbar1.GoalValue = pokemon1.CurrentHP;
                healthbar2.GoalValue = pokemon2.CurrentHP;
                energybar1.CurrentValue = pokemon1.CurrentEnergy;
                energybar2.CurrentValue = pokemon2.CurrentEnergy;
                energybar1.GoalValue = pokemon1.CurrentEnergy;
                energybar2.GoalValue = pokemon2.CurrentEnergy;
                healthbar1.Update();
                healthbar2.Update();
                energybar1.Update();
                energybar2.Update();
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null) { throw new ArgumentNullException("input"); }

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
            else
            {
                if (input.IsKeyDown(Keys.D1)) { pokemon1.CurrentHP--; }
                else if (input.IsKeyDown(Keys.D2)) { pokemon1.CurrentHP++; }
                else if (input.IsKeyDown(Keys.D3)) { pokemon2.CurrentHP--; }
                else if (input.IsKeyDown(Keys.D4)) { pokemon2.CurrentHP++; }
                else if (input.IsKeyDown(Keys.A)) { _Camera.Move(new Vector2(-1, 0)); }
                else if (input.IsKeyDown(Keys.S)) { _Camera.Move(new Vector2(0, 1)); }
                else if (input.IsKeyDown(Keys.D)) { _Camera.Move(new Vector2(1, 0)); }
                else if (input.IsKeyDown(Keys.W)) { _Camera.Move(new Vector2(0, -1)); }
                else if (input.IsKeyDown(Keys.Left)) { pokemon1.Velocity += new Vector2(-1, 0); }
                else if (input.IsKeyDown(Keys.Down)) { pokemon1.Velocity += new Vector2(0, 1); }
                else if (input.IsKeyDown(Keys.Right)) { pokemon1.Velocity += new Vector2(1, 0); }
                else if (input.IsKeyDown(Keys.Up)) { pokemon1.Velocity += new Vector2(0, -1); }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.GhostWhite, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, _Camera.Transform);

            //Draw the battle coordinator.
            BattleCoordinator.Instance.Draw(spriteBatch);

            //Draw the pokemon.
            pokemon1.Draw(spriteBatch);
            pokemon2.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();

            //Draw the healthbars.
            healthbar1.Draw();
            healthbar2.Draw();
            energybar1.Draw();
            energybar2.Draw();

            //Draw some text.
            spriteBatch.DrawString(ScreenManager.Font, "State: " + pokemon1.BattleState, new Vector2(5, 45), Color.Black);
            spriteBatch.DrawString(ScreenManager.Font, "State: " + pokemon2.BattleState, new Vector2(ScreenManager.Game.Window.ClientBounds.Width - 200, 45), Color.Black);

            //Draw some text.
            foreach (BattleMove m in BattleCoordinator.Instance.Moves.FindAll(x => x.State != TimelineState.Idle))
            {
                if (m.User == pokemon1)
                {
                    spriteBatch.DrawString(ScreenManager.Font, "--- Attack ---\nName: " + m.Name, new Vector2(5, 65), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(ScreenManager.Font, "--- Attack ---\nName: " + m.Name, new Vector2(ScreenManager.Game.Window.ClientBounds.Width - 200, 65), Color.Black);
                }
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) { ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha); }
        }


        #endregion
    }
}
