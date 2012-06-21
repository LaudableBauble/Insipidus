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

using InsipidusEngine.Tools;

namespace InsipidusEngine.Infrastructure
{
    /// <summary>
    /// A scene is a map of the game world and can be populated by entities.
    /// </summary>
    public class Scene
    {
        #region Fields
        protected String _Name;
        protected PhysicsSimulator _Physics;
        protected RobustList<Entity> _Entities;
        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor for a scene.
        /// </summary>
        public Scene()
        {
            Initialize();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the scene.
        /// </summary>
        protected void Initialize()
        {
            // Initialize the variables.
            _Name = "Scene";
            _Entities = new RobustList<Entity>();
            _Physics = new PhysicsSimulator();
        }

        /// <summary>
        /// Load all content.
        /// </summary>
        public void LoadContent()
        {
            // Load all entities' content.
        }

        /// <summary>
        /// Handle input.
        /// </summary>
        /// <param name="input">The current input state.</param>
        public void HandleInput(InputState input)
        {
            // Let all entities respond to input.
            _Entities.ForEach(item => item.HandleInput(input));
        }

        /// <summary>
        /// Update the scene.
        /// </summary>
        /// <param name="gametime">The game time.</param>
        public void Update(GameTime gametime)
        {
            // Update the physics simulator.
            _Physics.update();

            // Update all entities.
            _Entities.ForEach(item => item.Update(gametime));
        }

        /// <summary>
        /// Draw the scene.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch responsible for drawing the scene.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Enable depth sorting by composite.
            Composite old = spriteBatch.getComposite();
            spriteBatch.setComposite(_Composite);

            // Draw all entities.
            foreach (Entity entity in _Entities)
            {
                // Prepare the graphics device for depth-sorting.
                ((DepthComposite)spriteBatch.getComposite()).setEntity(entity);
                entity.draw(spriteBatch);
            }

            // Notify the depth composite that the frame has ended, at least for the scene.
            _Composite.endFrame();
            spriteBatch.setComposite(old);
        }

        /// <summary>
        /// Add an entity to the scene.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public Entity AddEntity(Entity entity)
        {
            _Entities.Add(entity);
            entity.setScene(this);
            _Physics.addBody(entity.getBody());
            Collections.sort(_Entities, new EntityDepthComparator());
            return entity;
        }

        /// <summary>
        /// Remove an entity from the scene.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void RemoveEntity(Entity entity)
        {
            _Entities.Remove(entity);
            _Physics.removeBody(entity.getBody());
        }
        #endregion

        #region Properties
        /// <summary>
        /// The name of the scene.
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// The physics simulator active in the scene.
        /// </summary>
        public PhysicsSimulator PhysicsSimulator
        {
            get { return _Physics; }
            set { _Physics = value; }
        }
        /// <summary>
        /// The list of entities in the scene.
        /// </summary>
        public RobustList<Entity> Entities
        {
            get { return _Entities; }
            set { _Entities = value; }
        }
        #endregion
    }
}
