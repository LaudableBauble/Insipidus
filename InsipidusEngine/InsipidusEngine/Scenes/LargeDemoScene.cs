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
    /// This is a large demo scene.
    /// </summary>
    public class LargeDemoScene : Scene
    {
        // Character.
        private Player _Player;

        // Entities.
        private Entity _Shelf1;
        private Entity _Shelf2;
        private Entity _Shelf3;
        private Entity _Shelf4;
        private Entity _Shelf5;
        private Entity _Block1;
        private Entity _Block2;
        private Entity _Block3;
        private Entity _Block4;
        private Entity _Block5;
        private Entity _Block6;
        private Entity _DarkBlock1;
        private Entity _DarkBlock2;
        private Entity _DarkBlock3;
        private Entity _DarkBlock4;
        private Entity _DarkBlock5;
        private Entity _DarkBlock6;
        private Entity _DarkBlock7;
        private Entity _DarkBlock8;
        private Entity _DarkBlock9;
        private Entity _DarkBlock10;
        private Entity _DarkBlock11;
        private Entity _DarkBlock12;
        private Entity _DarkBlock13;
        private Entity _Stairs1;
        private Entity _Stairs2;
        private Entity _Stairs3;
        private Entity _Stairs4;
        private Entity _Stairs5;
        private Entity _Stairs6;
        private Entity _Stairs7;
        private Entity _Stairs8;
        private Entity _Pathway1;
        private Entity _Pathway2;
        private Entity _Pathway3;
        private Entity _Pathway4;
        private Entity _PathwayArch1;
        private Entity _PathwayPlatform1;
        private Entity _PathwayPlatform2;
        private Entity _PathwayPlatform3;
        private Entity _Pillar1;
        private Entity _Pillar2;
        private Entity _MarbleArch;
        private Entity _MarbleWall1;
        private Entity _MarbleWall2;
        private Entity _Floor;

        #region Constructors
        /// <summary>
        /// Constructor for a scene.
        /// </summary>
        public LargeDemoScene() : base() { }
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
            _Name = "LargeDemoScene";

            // Create a character.
            _Player = new Player(this);
            _Player.Body.Position = new Vector3(1040, 1010, 50);

            // Create the shelf (south of pathway arch).
            _Shelf1 = new Entity(this);
            _Shelf1.Body.Position = new Vector3(940, 1025, 0);
            _Shelf1.Body.IsStatic = true;

            // Create the shelf (east of pathway platform 3).
            _Shelf2 = new Entity(this);
            _Shelf2.Body.Position = new Vector3(1270, 1080, 0);
            _Shelf2.Body.IsStatic = true;

            // Create the shelf (south of block 4).
            _Shelf3 = new Entity(this);
            _Shelf3.Body.Position = new Vector3(900, 945, 0);
            _Shelf3.Body.IsStatic = true;

            // Create the shelf (south of dark block 11).
            _Shelf4 = new Entity(this);
            _Shelf4.Body.Position = new Vector3(1300, 940, 0);
            _Shelf4.Body.IsStatic = true;

            // Create the shelf (south of dark block 7).
            _Shelf5 = new Entity(this);
            _Shelf5.Body.Position = new Vector3(670, 850, 0);
            _Shelf5.Body.IsStatic = true;

            // Create a block (north of staircase 2).
            _Block1 = new Entity(this);
            _Block1.Body.Position = new Vector3(710.5f, 942, 0);
            _Block1.Body.IsStatic = true;

            // Create a block (north of dark block 2).
            _Block2 = new Entity(this);
            _Block2.Body.Position = new Vector3(1180, 940, 0);
            _Block2.Body.IsStatic = true;

            // Create a block (north of dark block 1).
            _Block3 = new Entity(this);
            _Block3.Body.Position = new Vector3(852.5f, 909, 0);
            _Block3.Body.IsStatic = true;

            // Create a block (north of block 1).
            _Block4 = new Entity(this);
            _Block4.Body.Position = new Vector3(710.5f, 875.5f, 0);
            _Block4.Body.IsStatic = true;

            // Create a block (north of dark block 5, main stairs).
            _Block5 = new Entity(this);
            _Block5.Body.Position = new Vector3(1000, 794.5f, 0);
            _Block5.Body.IsStatic = true;

            // Create a block (east of block 2).
            _Block6 = new Entity(this);
            _Block6.Body.Position = new Vector3(1324, 940, 0);
            _Block6.Body.IsStatic = true;

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

            // Create a dark block (north of pathway arch, main stairs).
            _DarkBlock5 = new Entity(this);
            _DarkBlock5.Body.Position = new Vector3(1000, 872, 0);
            _DarkBlock5.Body.IsStatic = true;

            // Create a dark block (north of block 4, west stairs).
            _DarkBlock6 = new Entity(this);
            _DarkBlock6.Body.Position = new Vector3(710.5f, 771, 0);
            _DarkBlock6.Body.IsStatic = true;

            // Create a dark block (north of block 3, east of dark block 6).
            _DarkBlock7 = new Entity(this);
            _DarkBlock7.Body.Position = new Vector3(847.5f, 818, 0);
            _DarkBlock7.Body.IsStatic = true;

            // Create a dark block (north of block 5, main stairs).
            _DarkBlock8 = new Entity(this);
            _DarkBlock8.Body.Position = new Vector3(895, 720, 0);
            _DarkBlock8.Body.IsStatic = true;

            // Create a dark block (east of dark block 2).
            _DarkBlock9 = new Entity(this);
            _DarkBlock9.Body.Position = new Vector3(1283, 1000, 0);
            _DarkBlock9.Body.IsStatic = true;

            // Create a dark block (north of block 2).
            _DarkBlock10 = new Entity(this);
            _DarkBlock10.Body.Position = new Vector3(1180, 830.5f, 0);
            _DarkBlock10.Body.IsStatic = true;

            // Create a dark block (east of dark block 10).
            _DarkBlock11 = new Entity(this);
            _DarkBlock11.Body.Position = new Vector3(1317, 863.5f, 0);
            _DarkBlock11.Body.IsStatic = true;

            // Create a dark block (south of pathway platform 2).
            _DarkBlock12 = new Entity(this);
            _DarkBlock12.Body.Position = new Vector3(1257, 1183, 0);
            _DarkBlock12.Body.IsStatic = true;

            // Create a dark block (north of block 5, east of dark block 8, main stairs).
            _DarkBlock13 = new Entity(this);
            _DarkBlock13.Body.Position = new Vector3(1128, 720, 0);
            _DarkBlock13.Body.IsStatic = true;

            // Create a staircase (west of dark block 1).
            _Stairs1 = new Entity(this);
            _Stairs1.Body.Position = new Vector3(759.5f, 982.5f, 0);
            _Stairs1.Body.IsStatic = true;
            _Stairs1.Body.Shape.DepthDistribution = DepthDistribution.Right;

            // Create a staircase (west of dark block 2).
            _Stairs2 = new Entity(this);
            _Stairs2.Body.Position = new Vector3(1056.5f, 1033, 0);
            _Stairs2.Body.IsStatic = true;
            _Stairs2.Body.Shape.DepthDistribution = DepthDistribution.Right;

            // Create a staircase (north of dark block 5, main stairs).
            _Stairs3 = new Entity(this);
            _Stairs3.Body.Position = new Vector3(1000, 931.5f, 0);
            _Stairs3.Body.IsStatic = true;
            _Stairs3.Body.Shape.DepthDistribution = DepthDistribution.Top;

            // Create a staircase (east of block 1).
            _Stairs4 = new Entity(this);
            _Stairs4.Body.Position = new Vector3(807.5f, 942, 0);
            _Stairs4.Body.IsStatic = true;
            _Stairs4.Body.Shape.DepthDistribution = DepthDistribution.Left;

            // Create a staircase (east of dark block 7).
            _Stairs5 = new Entity(this);
            _Stairs5.Body.Position = new Vector3(755, 868.5f, 0);
            _Stairs5.Body.IsStatic = true;
            _Stairs5.Body.Shape.DepthDistribution = DepthDistribution.Right;

            // Create a staircase (north of block 5, main stairs).
            _Stairs6 = new Entity(this);
            _Stairs6.Body.Position = new Vector3(1015, 861, 0);
            _Stairs6.Body.IsStatic = true;
            _Stairs6.Body.Shape.DepthDistribution = DepthDistribution.Top;

            // Create a staircase (south of dark block 8, main stairs).
            _Stairs7 = new Entity(this);
            _Stairs7.Body.Position = new Vector3(1015, 778, 0);
            _Stairs7.Body.IsStatic = true;
            _Stairs7.Body.Shape.DepthDistribution = DepthDistribution.Top;

            // Create a staircase (west of dark block 11).
            _Stairs8 = new Entity(this);
            _Stairs8.Body.Position = new Vector3(1224.5f, 914, 0);
            _Stairs8.Body.IsStatic = true;
            _Stairs8.Body.Shape.DepthDistribution = DepthDistribution.Right;

            // Create a pathway.
            _Pathway1 = new Entity(this);
            _Pathway2 = new Entity(this);
            _Pathway3 = new Entity(this);
            _Pathway4 = new Entity(this);
            _PathwayArch1 = new Entity(this);
            _Pathway1.Body.Position = new Vector3(936.5f, 1000, 0);
            _Pathway2.Body.Position = new Vector3(968.5f, 1000, 0);
            _PathwayArch1.Body.Position = new Vector3(1000, 1000, 0);
            _Pathway3.Body.Position = new Vector3(1032.5f, 1000, 0);
            _Pathway4.Body.Position = new Vector3(1064.5f, 1000, 0);
            _Pathway1.Body.IsStatic = true;
            _Pathway2.Body.IsStatic = true;
            _Pathway3.Body.IsStatic = true;
            _Pathway4.Body.IsStatic = true;
            _PathwayArch1.Body.IsStatic = true;

            // Create a pathway platform (south of dark block 1).
            _PathwayPlatform1 = new Entity(this);
            _PathwayPlatform1.Body.Position = new Vector3(852, 1107.5f, 0);
            _PathwayPlatform1.Body.IsStatic = true;

            // Create a pathway platform (south of dark block 2).
            _PathwayPlatform2 = new Entity(this);
            _PathwayPlatform2.Body.Position = new Vector3(1149, 1107.5f, 0);
            _PathwayPlatform2.Body.IsStatic = true;

            // Create a pathway platform (south of dark block 9).
            _PathwayPlatform3 = new Entity(this);
            _PathwayPlatform3.Body.Position = new Vector3(1305, 1107.5f, 0);
            _PathwayPlatform3.Body.IsStatic = true;

            // Create a pillar (west of marble arch).
            _Pillar1 = new Entity(this);
            _Pillar1.Body.Position = new Vector3(991, 690, 0);
            _Pillar1.Body.IsStatic = true;

            // Create a pillar (east of marble arch).
            _Pillar2 = new Entity(this);
            _Pillar2.Body.Position = new Vector3(1039, 690, 0);
            _Pillar2.Body.IsStatic = true;

            // Create a marble arch (north of dark block 8).
            _MarbleArch = new Entity(this);
            _MarbleArch.Body.Position = new Vector3(1015, 690, 0);
            _MarbleArch.Body.IsStatic = true;

            // Create a marble wall (west of pillar 1).
            _MarbleWall1 = new Entity(this);
            _MarbleWall1.Body.Position = new Vector3(823, 680, 0);
            _MarbleWall1.Body.IsStatic = true;

            // Create a marble wall (east of pillar 2).
            _MarbleWall2 = new Entity(this);
            _MarbleWall2.Body.Position = new Vector3(1207, 680, 0);
            _MarbleWall2.Body.IsStatic = true;

            // Create the floor.
            _Floor = new Entity(this);
            _Floor.Body.Position = new Vector3(1000, 1050, 0);
            _Floor.Body.IsStatic = true;

            // Add all entities to the scene.
            //AddEntity(_Player);
            AddEntity(_Floor);
            AddEntity(_Shelf1);
            AddEntity(_Shelf2);
            AddEntity(_Shelf3);
            AddEntity(_Shelf4);
            AddEntity(_Shelf5);
            AddEntity(_Block1);
            AddEntity(_Block2);
            AddEntity(_Block3);
            AddEntity(_Block4);
            AddEntity(_Block5);
            AddEntity(_Block6);
            AddEntity(_DarkBlock1);
            AddEntity(_DarkBlock2);
            AddEntity(_DarkBlock3);
            AddEntity(_DarkBlock4);
            AddEntity(_DarkBlock5);
            AddEntity(_DarkBlock6);
            AddEntity(_DarkBlock7);
            AddEntity(_DarkBlock8);
            AddEntity(_DarkBlock9);
            AddEntity(_DarkBlock10);
            AddEntity(_DarkBlock11);
            AddEntity(_DarkBlock12);
            AddEntity(_DarkBlock13);
            AddEntity(_Stairs1);
            AddEntity(_Stairs2);
            AddEntity(_Stairs3);
            AddEntity(_Stairs4);
            AddEntity(_Stairs5);
            AddEntity(_Stairs6);
            AddEntity(_Stairs7);
            AddEntity(_Stairs8);
            AddEntity(_Pathway1);
            AddEntity(_Pathway2);
            AddEntity(_Pathway3);
            AddEntity(_Pathway4);
            AddEntity(_PathwayArch1);
            AddEntity(_PathwayPlatform1);
            AddEntity(_PathwayPlatform2);
            AddEntity(_PathwayPlatform3);
            AddEntity(_Pillar1);
            AddEntity(_Pillar2);
            AddEntity(_MarbleArch);
            AddEntity(_MarbleWall1);
            AddEntity(_MarbleWall2);
            //AddEntity(_Floor);
            AddEntity(_Player);

            // Give a name to all entities.
            _Player.Name = "Character";
            _Shelf1.Name = "Shelf1";
            _Shelf2.Name = "Shelf2";
            _Shelf3.Name = "Shelf3";
            _Shelf4.Name = "Shelf4";
            _Shelf5.Name = "Shelf5";
            _Block1.Name = "Block1";
            _Block2.Name = "Block2";
            _Block3.Name = "Block3";
            _Block4.Name = "Block4";
            _Block5.Name = "Block5";
            _Block6.Name = "Block6";
            _DarkBlock1.Name = "DarkBlock1";
            _DarkBlock2.Name = "DarkBlock2";
            _DarkBlock3.Name = "DarkBlock3";
            _DarkBlock4.Name = "DarkBlock4";
            _DarkBlock5.Name = "DarkBlock5";
            _DarkBlock6.Name = "DarkBlock6";
            _DarkBlock7.Name = "DarkBlock7";
            _DarkBlock8.Name = "DarkBlock8";
            _DarkBlock9.Name = "DarkBlock9";
            _DarkBlock10.Name = "DarkBlock10";
            _DarkBlock11.Name = "DarkBlock11";
            _DarkBlock12.Name = "DarkBlock12";
            _DarkBlock13.Name = "DarkBlock13";
            _Stairs1.Name = "Stairs1";
            _Stairs2.Name = "Stairs2";
            _Stairs3.Name = "Stairs3";
            _Stairs4.Name = "Stairs4";
            _Stairs5.Name = "Stairs5";
            _Stairs6.Name = "Stairs6";
            _Stairs7.Name = "Stairs7";
            _Stairs8.Name = "Stairs8";
            _Pathway1.Name = "Pathway1";
            _Pathway2.Name = "Pathway2";
            _Pathway3.Name = "Pathway3";
            _Pathway4.Name = "Pathway4";
            _PathwayPlatform1.Name = "PathwayPlatform1";
            _PathwayPlatform2.Name = "PathwayPlatform2";
            _PathwayPlatform3.Name = "PathwayPlatform3";
            _Pillar1.Name = "Pillar1";
            _Pillar1.Name = "Pillar2";
            _MarbleArch.Name = "MarbleArch";
            _MarbleWall1.Name = "MarbleWall1";
            _MarbleWall1.Name = "MarbleWall2";
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
            _Player.LoadContent(content);
            _Shelf1.LoadContent(content, @"Entities\Bookshelf[1]", 12);
            _Shelf2.LoadContent(content, @"Entities\Bookshelf[1]", 12);
            _Shelf3.LoadContent(content, @"Entities\Bookshelf[1]", 12);
            _Shelf4.LoadContent(content, @"Entities\Bookshelf[1]", 12);
            _Shelf5.LoadContent(content, @"Entities\Bookshelf[1]", 12);
            _Block1.LoadContent(content, @"Entities\ElevatedBlock[3]", 48);
            _Block2.LoadContent(content, @"Entities\ElevatedBlock[2]", 85);
            _Block3.LoadContent(content, @"Entities\ElevatedBlock[3]", 48);
            _Block4.LoadContent(content, @"Entities\ElevatedBlock[2]", 85);
            _Block5.LoadContent(content, @"Entities\ElevatedBlock[4]", 85);
            _Block6.LoadContent(content, @"Entities\ElevatedBlock[2]", 85);
            _DarkBlock1.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock2.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock3.LoadContent(content, @"Entities\DarkTiledBlock[1]", 70);
            _DarkBlock4.LoadContent(content, @"Entities\DarkTiledBlock[1]", 70);
            _DarkBlock5.LoadContent(content, @"Entities\DarkTiledBlock[3]", 70);
            _DarkBlock6.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock7.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock8.LoadContent(content, @"Entities\DarkTiledBlock[3]", 70);
            _DarkBlock9.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock10.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock11.LoadContent(content, @"Entities\DarkTiledBlock[2]", 134);
            _DarkBlock12.LoadContent(content, @"Entities\DarkTiledBlock[1]", 70);
            _DarkBlock13.LoadContent(content, @"Entities\DarkTiledBlock[3]", 70);
            _Stairs1.LoadContent(content, @"Entities\StoneStairsRight[3]", 33);
            _Stairs2.LoadContent(content, @"Entities\StoneStairsRight[3]", 33);
            _Stairs3.LoadContent(content, @"Entities\StoneStairsTop[2]", 46);
            _Stairs4.LoadContent(content, @"Entities\StoneStairsLeft[1]", 33);
            _Stairs5.LoadContent(content, @"Entities\StoneStairsRight[3]", 33);
            _Stairs6.LoadContent(content, @"Entities\StoneStairsTop[2]", 46);
            _Stairs7.LoadContent(content, @"Entities\StoneStairsTop[2]", 46);
            _Stairs8.LoadContent(content, @"Entities\StoneStairsRight[3]", 33);
            _Pathway1.LoadContent(content, @"Entities\StonePathwayBlock[1]", 33);
            _Pathway2.LoadContent(content, @"Entities\StonePathwayBlock[1]", 33);
            _Pathway3.LoadContent(content, @"Entities\StonePathwayBlock[1]", 33);
            _Pathway4.LoadContent(content, @"Entities\StonePathwayBlock[1]", 33);
            _PathwayArch1.LoadContent(content, @"Entities\StonePathwayArch[1]", 33);
            _PathwayPlatform1.LoadContent(content, @"Entities\StonePathwayBlock[2]", 81);
            _PathwayPlatform2.LoadContent(content, @"Entities\StonePathwayBlock[4]", 81);
            _PathwayPlatform3.LoadContent(content, @"Entities\StonePathwayBlock[3]", 81);
            _Pillar1.LoadContent(content, @"Entities\MarblePillar[1]", 13);
            _Pillar2.LoadContent(content, @"Entities\MarblePillar[1]", 13);
            _MarbleArch.LoadContent(content, @"Entities\MarbleArch[1]", 5);
            _MarbleWall1.LoadContent(content, @"Entities\MarbleWall[1]", 22);
            _MarbleWall2.LoadContent(content, @"Entities\MarbleWall[1]", 22);
            _Floor.LoadContent(content, @"Entities\WoodTiledFloor[1]");

            //Set their normal maps.
            _Shelf1.Sprites[0].Frames[0].NormalPath = @"Entities\Bookshelf[1](Normal)";
            _Shelf2.Sprites[0].Frames[0].NormalPath = @"Entities\Bookshelf[1](Normal)";
            _Shelf3.Sprites[0].Frames[0].NormalPath = @"Entities\Bookshelf[1](Normal)";
            _Shelf4.Sprites[0].Frames[0].NormalPath = @"Entities\Bookshelf[1](Normal)";
            _Shelf5.Sprites[0].Frames[0].NormalPath = @"Entities\Bookshelf[1](Normal)";
            _Block2.Sprites[0].Frames[0].NormalPath = @"Entities\ElevatedBlock[2](Normal)";
            _Block4.Sprites[0].Frames[0].NormalPath = @"Entities\ElevatedBlock[2](Normal)";
            _Block6.Sprites[0].Frames[0].NormalPath = @"Entities\ElevatedBlock[2](Normal)";
            _Block1.Sprites[0].Frames[0].NormalPath = @"Entities\ElevatedBlock[3](Normal)";
            _Block3.Sprites[0].Frames[0].NormalPath = @"Entities\ElevatedBlock[3](Normal)";
            _Block5.Sprites[0].Frames[0].NormalPath = @"Entities\ElevatedBlock[4](Normal)";
            _DarkBlock3.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[1](Normal)";
            _DarkBlock4.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[1](Normal)";
            _DarkBlock12.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[1](Normal)";
            _DarkBlock1.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[2](Normal)";
            _DarkBlock2.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[2](Normal)";
            _DarkBlock6.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[2](Normal)";
            _DarkBlock7.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[2](Normal)";
            _DarkBlock9.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[2](Normal)";
            _DarkBlock10.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[2](Normal)";
            _DarkBlock11.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[2](Normal)";
            _DarkBlock5.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[3](Normal)";
            _DarkBlock8.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[3](Normal)";
            _DarkBlock13.Sprites[0].Frames[0].NormalPath = @"Entities\DarkTiledBlock[3](Normal)";
            _Stairs1.Sprites[0].Frames[0].NormalPath = @"Entities\StoneStairsRight[3](Normal)";
            _Stairs2.Sprites[0].Frames[0].NormalPath = @"Entities\StoneStairsRight[3](Normal)";
            _Stairs5.Sprites[0].Frames[0].NormalPath = @"Entities\StoneStairsRight[3](Normal)";
            _Stairs8.Sprites[0].Frames[0].NormalPath = @"Entities\StoneStairsRight[3](Normal)";
            _Pillar1.Sprites[0].Frames[0].NormalPath = @"Entities\MarblePillar[1](Normal)";
            _Pillar2.Sprites[0].Frames[0].NormalPath = @"Entities\MarblePillar[1](Normal)";
            _MarbleWall1.Sprites[0].Frames[0].NormalPath = @"Entities\MarbleWall[1](Normal)";
            _MarbleWall2.Sprites[0].Frames[0].NormalPath = @"Entities\MarbleWall[1](Normal)";
            _Floor.Sprites[0].Frames[0].NormalPath = @"Entities\WoodTiledFloor[1](Normal)";

            //Set their depth maps.
            _DarkBlock3.Sprites[0].Frames[0].DepthPath = @"Entities\DarkTiledBlock[1](Depth)";
            _DarkBlock4.Sprites[0].Frames[0].DepthPath = @"Entities\DarkTiledBlock[1](Depth)";
            _DarkBlock12.Sprites[0].Frames[0].DepthPath = @"Entities\DarkTiledBlock[1](Depth)";
            _Player.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Shelf1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Shelf2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Shelf3.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Shelf4.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Shelf5.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Block1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Block2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Block3.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Block4.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Block5.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Block6.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock5.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock6.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock7.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock8.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock9.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock10.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock11.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _DarkBlock13.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs3.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs4.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs5.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs6.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs7.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Stairs8.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Pathway1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Pathway2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Pathway3.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Pathway4.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _PathwayArch1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _PathwayPlatform1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _PathwayPlatform2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _PathwayPlatform3.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Pillar1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Pillar2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _MarbleArch.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _MarbleWall1.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _MarbleWall2.Sprites[0].Frames[0].DepthPath = @"Entities\DepthDefault";
            _Floor.Sprites[0].Frames[0].DepthPath = @"Entities\WoodTiledFloor[1](Depth)";

            //Reload their textures.
            _Entities.Update();
            _Entities.ForEach(entity => entity.Sprites.Sprites.ForEach(sprite => sprite.LoadFrame()));

            // Set their depths.
            _Shelf1.Body.Shape.BottomDepth = 1;
            _Shelf2.Body.Shape.BottomDepth = 1;
            _Shelf3.Body.Shape.BottomDepth = 48;
            _Shelf4.Body.Shape.BottomDepth = 97;
            _Shelf5.Body.Shape.BottomDepth = 97;
            _Block1.Body.Shape.BottomDepth = 1;
            _Block2.Body.Shape.BottomDepth = 49;
            _Block3.Body.Shape.BottomDepth = 1;
            _Block4.Body.Shape.BottomDepth = 48;
            _Block5.Body.Shape.BottomDepth = 48;
            _Block6.Body.Shape.BottomDepth = 49;
            _DarkBlock1.Body.Shape.BottomDepth = 1;
            _DarkBlock2.Body.Shape.BottomDepth = 1;
            _DarkBlock3.Body.Shape.BottomDepth = 1;
            _DarkBlock4.Body.Shape.BottomDepth = 1;
            _DarkBlock5.Body.Shape.BottomDepth = 1;
            _DarkBlock6.Body.Shape.BottomDepth = 97;
            _DarkBlock7.Body.Shape.BottomDepth = 97;
            _DarkBlock8.Body.Shape.BottomDepth = 97;
            _DarkBlock9.Body.Shape.BottomDepth = 1;
            _DarkBlock10.Body.Shape.BottomDepth = 97;
            _DarkBlock11.Body.Shape.BottomDepth = 97;
            _DarkBlock12.Body.Shape.BottomDepth = 1;
            _DarkBlock13.Body.Shape.BottomDepth = 97;
            _Stairs1.Body.Shape.BottomDepth = 1;
            _Stairs2.Body.Shape.BottomDepth = 1;
            _Stairs3.Body.Shape.Depth = 48;
            _Stairs3.Body.Shape.BottomDepth = 1;
            _Stairs4.Body.Shape.BottomDepth = 48;
            _Stairs5.Body.Shape.BottomDepth = 97;
            _Stairs6.Body.Shape.Depth = 48;
            _Stairs6.Body.Shape.BottomDepth = 49;
            _Stairs7.Body.Shape.Depth = 48;
            _Stairs7.Body.Shape.BottomDepth = 97;
            _Stairs8.Body.Shape.BottomDepth = 97;
            _Pathway1.Body.Shape.BottomDepth = 1;
            _Pathway2.Body.Shape.BottomDepth = 1;
            _Pathway3.Body.Shape.BottomDepth = 1;
            _Pathway4.Body.Shape.BottomDepth = 1;
            _PathwayArch1.Body.Shape.BottomDepth = 22;
            _PathwayPlatform1.Body.Shape.BottomDepth = 37;
            _PathwayPlatform2.Body.Shape.BottomDepth = 37;
            _PathwayPlatform3.Body.Shape.BottomDepth = 1;
            _Pillar1.Body.Shape.BottomDepth = 144;
            _Pillar2.Body.Shape.BottomDepth = 144;
            _MarbleArch.Body.Shape.BottomDepth = 184;
            _MarbleWall1.Body.Shape.BottomDepth = 144;
            _MarbleWall2.Body.Shape.BottomDepth = 144;
            _Floor.Body.Shape.BottomDepth = 0;
        }
        #endregion
    }
}
