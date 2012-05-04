using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InsipidusEngine.Battle
{
    class ExperienceBar: Bar
    {
        #region Fields
        
        #endregion

        #region Properties

        #endregion

        #region Constructors
        public ExperienceBar(float maxExp, float currentExp, float changeValue, Vector2 position,
            float width, float height)
        {
            MaxValue = maxExp;
            MinValue = 0;
            CurrentValue = currentExp;
            GoalValue = MaxValue;
            ChangeValue = changeValue;
            MaxWidth = width;
            MaxHeight = height;
            Rectangle = new Rectangle((int)position.X, (int)position.Y, (int)MaxWidth, (int)MaxHeight);
            Position = position;

            Initialize(new Color(64, 200, 248));
        }
        #endregion

        #region Methods
        public override void Update()
        {
            //Update the bar.
            base.Update();
        }
        public override void OnValueChange(object obj)
        {
            base.OnValueChange(obj);
        }
        #endregion
    }
}