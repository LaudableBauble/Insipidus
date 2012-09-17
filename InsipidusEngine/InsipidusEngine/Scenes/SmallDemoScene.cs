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

using InsipidusEngine.Core;
using InsipidusEngine.Helpers;

namespace InsipidusEngine.Scenes
{
    /// <summary>
    /// This is a small demo scene.
    /// </summary>
    public class SmallDemoScene : Scene
    {
        // Entities.
        private Entity _DarkBlock1;
        private Entity _DarkBlock2;
        private Entity _DarkBlock3;
        private Entity _DarkBlock4;
        private Entity _Floor;

        #region Constructors
        /// <summary>
        /// Constructor for a scene.
        /// </summary>
        public SmallDemoScene() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the scene.
        /// </summary>
        protected override void Initialize()
        {
            // Call the base method.
            base.Initialize();

            // Name the scene.
            _Name = "SmallDemoScene";

            // Create a dark block (west of pathway arch).
            _DarkBlock1 = new Entity(this);
            _DarkBlock1.Body.Position = new Vector3(852, 1000, 0);
            _DarkBlock1.Body.IsStatic = true;

            // Create a dark block (east of pathway arch).
            _DarkBlock2 = new Entity(this);
            _DarkBlock2.Body.Position = new Vector3(1149, 1000, 0);
            _DarkBlock2.Body.IsStatic = true;

            // Create a dark block (south of pathway platform 1).
            _DarkBlock3 = new Entity(this);
            _DarkBlock3.Body.Position = new Vector3(900, 1183, 0);
            _DarkBlock3.Body.IsStatic = true;

            // Create a dark block (south of pathway platform 2).
            _DarkBlock4 = new Entity(this);
            _DarkBlock4.Body.Position = new Vector3(1120, 1183, 0);
            _DarkBlock4.Body.IsStatic = true;

            // Create the floor.
            _Floor = new Entity(this);
            _Floor.Body.Position = new Vector3(1000, 1050, 0);
            _Floor.Body.IsStatic = true;

            // Add all entities to the scene.
            AddEntity(_Floor);
            AddEntity(_DarkBlock1);
            AddEntity(_DarkBlock2);
            AddEntity(_DarkBlock3);
            AddEntity(_DarkBlock4);

            // Give a name to all entities.
            _DarkBlock1.Name = "DarkBlock1";
            _DarkBlock2.Name = "DarkBlock2";
            _DarkBlock3.Name = "DarkBlock3";
            _DarkBlock4.Name = "DarkBlock4";
            _Floor.Name = "Floor";
        }
        /// <summary>
        /// Load all content.
        /// </summary>
        /// <param name="graphics">The graphics device in use.</param>
        /// <param name="content">The content manager to use.</param>
        public override void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            //Call the base method.
            base.LoadContent(graphics, content);

            // Load all entities' content.
            _DarkBlock1.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock2.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock3.LoadContent(content, @"Entities\DarkTiledBlock[1]", 70);
            _DarkBlock4.LoadContent(content, @"Entities\DarkTiledBlock[1]", 70);
            _Floor.LoadContent(content, @"Entities\WoodTiledFloor[1]");

            //Reload their textures.
            _Entities.Update();
            _Entities.ForEach(entity => entity.Sprites.Sprites.ForEach(sprite => sprite.LoadFrame()));

            // Set their depths.
            _DarkBlock1.Body.Shape.BottomDepth = 1;
            _DarkBlock2.Body.Shape.BottomDepth = 1;
            _DarkBlock3.Body.Shape.BottomDepth = 1;
            _DarkBlock4.Body.Shape.BottomDepth = 1;
            _Floor.Body.Shape.BottomDepth = 0;
        }
        #endregion
    }
}
