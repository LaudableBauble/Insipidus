using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InsipidusEngine.Battle
{
    class HealthBar : Bar
    {
        #region Fields

        #endregion

        #region Constructors
        public HealthBar(float maxHealth, float currentHealth, float changeValue, Vector2 position, float width, float height)
        {
            MaxValue = maxHealth;
            MinValue = 0;
            CurrentValue = currentHealth;
            GoalValue = CurrentValue;
            ChangeValue = changeValue;
            MaxWidth = width;
            MaxHeight = height;
            Rectangle = new Rectangle((int)position.X, (int)position.Y, (int)MaxWidth, (int)MaxHeight);

            Initialize(DetermineColor());
        }
        #endregion

        #region Methods
        public override void Update()
        {
            //Update the bar.
            base.Update();
        }
        private Color DetermineColor()
        {
            //Create the ratio variable.
            float ratio = (CurrentValue / MaxValue);

            //Check the value ratio.
            if (ratio > 0.5) { return (new Color(100, 160, 104)); }
            else if ((ratio < 0.5) && (ratio > 0.25)) { return (new Color(204, 156, 104)); }
            else if (ratio < 0.25) { return (new Color(200, 92, 84)); }
            else { return (FillColor); }
        }
        public override void OnValueChange(object obj)
        {
            //Determine the new Color.
            FillColor = DetermineColor();

            base.OnValueChange(obj);
        }
        #endregion

        #region Properties

        #endregion
    }
}