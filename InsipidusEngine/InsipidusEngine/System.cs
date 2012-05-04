using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace InsipidusEngine
{
    public class System
    {
        #region Fields

        public ScreenManager ScreenManager;

        public ContentManager contentManager;
        public SpriteBatch spriteBatch;

        Text.TextBox TextBox;

        #endregion

        #region Methods

        public void Initialize(ScreenManager screen)
        {
            ScreenManager = screen;
            ScreenManager.Game.Window.Title = "Scrolling Text Engine";

            TextBox = new Text.TextBox(
                "What's he building in there? /n " +
"What the hell is he building In there? /n " +
"He has /s subscriptions /s to /s those Magazines... He never /f Waves /f when he goes by... /n " +
"He's hiding something /f from The /f rest of us... /n " +
"He's all To himself... I think I know Why... He took down the Tire swing from the Peppertree /n " +
"He has no children of his Own you see... He has no dog, And he has no friends... " +
"And His lawn is dying... /n and What about all those packages He sends. /n What's he building in there? /n " +
"With that hook light On the stairs. /n What's he building In there... /n I'll tell you one thing " +
"He's not building a playhouse for The children. /n What's he building In there? /n " +
"Now what's that sound from under the door? /n He's pounding nails into a Hardwood floor... /n " +
"I Swear to god I heard someone Moaning low... and I keep Seeing the blue light of a TV show... /n " +
"He has a router, And a table saw... and you Won't believe what Mr. Sticha saw. /n There's poison " +
"underneath the sink Of course... But there's also Enough formaldehyde to choke A horse... /n " +
"What's he building In there. What the hell is he Building in there? /n I heard he Has an ex-wife " + 
            "in some place Called Mayors Income, Tennessee. And he used to have a consulting business " +
            "in Indonesia... /n but what is he building in there? /n What the hell is building in there? /n " +
"He has no friends, But he gets a lot of mail. /n I'll bet he spent a little Time in jail... /n " +
"I heard he was up on the Roof last night Signaling with a flashlight. /n And what's that tune he's " +
"Always whistling... /n What's he building in there? /n What's he building in there? /n " +
"We have a right to know...", ScreenManager.SpeechFont);
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
        {
            ScreenManager.Game.IsMouseVisible = true;
            TextBox.LoadContent(content, spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            TextBox.Update();
        }

        public void HandleInput(InputState input)
        {
            TextBox.HandleInput(input);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TextBox.Draw();
        }

        #endregion
    }
}
