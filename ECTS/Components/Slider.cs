using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ECTS.Components
{
    internal sealed class Slider : Component
    {
        private readonly Texture2D mSlider;
        private readonly Texture2D mSliderBackground;
        private Vector2 PositionSlider { get; set; }
        private bool mClicked;
        private MouseState mOldMouseState;

        public readonly int mMinValue;
        private readonly int mMaxValue;
        public readonly int mHeight;
        private const int SliderHeight = 80;
        private const int SliderWidth = 50;
        private readonly int mDifference;

        public Slider(int height, float value)
        {
            mHeight = height;
            mSlider = Global.mGameLoop.Content.Load<Texture2D>("Components/SettingsMenu/Slider");
            mSliderBackground = Global.mGameLoop.Content.Load<Texture2D>("Components/SettingsMenu/SliderBackground");
            mMinValue = 450;
            mMaxValue = (int)((Global.mGameLoop.DisplayWidth - mSlider.Width)*0.78); // Depending on the resolution.
            mDifference = mMaxValue - mMinValue+45;
            PositionSlider = new Vector2(mMinValue+ mDifference*value, height); // Sets the slider to the stored position. 
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mSliderBackground,
                new Rectangle(mMinValue - 10, mHeight+SliderHeight / 2-9, mMaxValue + 120 - mMinValue, 20),
                Color.White);
            spriteBatch.Draw(mSlider,
                new Rectangle((int) PositionSlider.X, (int) PositionSlider.Y, SliderWidth, SliderHeight),
                Color.White);
        }

        public override void Update()
        {
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            var rectangle = new Rectangle((int)PositionSlider.X, (int)PositionSlider.Y, SliderWidth, SliderHeight);
            if (mouseState.LeftButton == ButtonState.Pressed && rectangle.Contains(mousePoint) && !mClicked && mOldMouseState.LeftButton != ButtonState.Pressed) // Check whether the slider was clicked.
            {
                mClicked = true;
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Pressed && mClicked) // Responsible for moving the slider
                {
                    var newXPosition = PositionSlider.X + mouseState.X - mOldMouseState.X;
                    if (newXPosition > mMinValue && newXPosition < mMaxValue + SliderWidth)
                    {
                        PositionSlider = new Vector2(newXPosition, PositionSlider.Y);
                    }
                }
                else
                {
                    mClicked = false;
                }
            }
            mOldMouseState = mouseState;
        }
        public float GetValueOfTheSlider() // Calculates the percentage value of the slider (between 0.0 and 1.0)
        {
            var currentPosition = PositionSlider.X - 450;
            return currentPosition/mDifference;
        }
    }
}
