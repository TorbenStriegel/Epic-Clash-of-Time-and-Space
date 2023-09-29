using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static ECTS.Components.Global;

namespace ECTS.Components
{
    internal sealed class Button : Component
    {
        private MouseState mCurrentMouse;
        private readonly SpriteFont mFont;
        private bool mIsHovering;
        private readonly bool mHoverCosts;
        private readonly int mObjectType;
        private MouseState mPreviousMouse;
        private readonly Texture2D mTexture;
        public event EventHandler Click;
        public readonly int mButtonWidth;
        public readonly int mButtonHeight;
        public bool mDeactivated;
        private readonly bool mOnlyText;
        private Color PenColour { get; }
        public Vector2 Position { get; }
        private Rectangle SectionRectangle { get; } = Rectangle.Empty;
        private Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, mButtonWidth, mButtonHeight);
        public string Text { get; set; }

        public Button(Texture2D texture, int buttonSize, int heightOfTheButton, bool right) // bool right = true -> Button on the right side, with false left.
        {
            mTexture = texture;
            mButtonWidth = (int)(mTexture.Width * (buttonSize / 100.0) * (mGameLoop.DisplayWidth / 1200.0)); // Adjusts the size of the image without changing the proportions.
            mButtonHeight = (int)(mTexture.Height * (buttonSize / 100.0) * (mGameLoop.DisplayWidth / 1200.0));
            if (right)
            {
                Position = new Vector2(mGameLoop.DisplayWidth - mButtonWidth,
                    heightOfTheButton);
            }
            else
            {
                Position = new Vector2(0, heightOfTheButton);
            }
            PenColour = Color.Black;
        }

        public Button(Vector2 position, Texture2D texture, int buttonSize, bool onlyText = false)
        {
            mOnlyText = onlyText;
            Position = position;
            mTexture = texture;
            mButtonWidth = (int)(mTexture.Width * (buttonSize / 100.0) * (mGameLoop.DisplayWidth / 1200.0));
            mButtonHeight = (int)(mTexture.Height * (buttonSize / 100.0) * (mGameLoop.DisplayWidth / 1200.0));
            PenColour = Color.Black;
        }

        public Button(Rectangle buttonSizeRectangle, Texture2D buttonTexture, Rectangle textureSectorRectangle, int objectType, bool objectCosts = false)
        {
            Position = new Vector2(buttonSizeRectangle.X, buttonSizeRectangle.Y);
            mButtonHeight = buttonSizeRectangle.Height;
            mButtonWidth = buttonSizeRectangle.Width;
            SectionRectangle = textureSectorRectangle;
            mObjectType = objectType;
            mHoverCosts = objectCosts;
            mTexture = buttonTexture;
            PenColour = Color.Black;
        }

        public Button(Vector2 position, int buttonSize, float buttonLength)
        {
            Position = position;
            mFont = mGameLoop.Content.Load<SpriteFont>("Font/Arial30");
            mTexture = mGameLoop.Content.Load<Texture2D>("Button/Empty");
            mButtonWidth = (int)(mTexture.Width * (buttonSize / 100.0) * (mGameLoop.DisplayWidth / 1200.0) * buttonLength);
            mButtonHeight = (int)(mTexture.Height * (buttonSize / 100.0) * (mGameLoop.DisplayWidth / 1200.0));

            PenColour = Color.Black;
        }
        public override void Draw(SpriteBatch spriteBatch) // Draws the button
        {
            var color = !mDeactivated ? Color.White : Color.Gray;
            // draw hovering				
            if (mIsHovering && !mOnlyText) // Grays out the button when you hover the mouse over it.
            {
                color = Color.Gray;

                if (mHoverCosts)  // hover costs of object
                {
                    mGameLoop.RenderManager.ScreenManager.HudMenu.DrawCosts(new Rectangle((int)Position.X, (int)Position.Y, mButtonWidth, mButtonHeight), mObjectType);
                }
            }
            // draw button
            if (SectionRectangle.IsEmpty)
            {
                spriteBatch.Draw(mTexture, Rectangle, color);
            }
            else
            {
                spriteBatch.Draw(mTexture, Rectangle, SectionRectangle, color);
            }

            // Draw text			
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }

            var x = Rectangle.X + Rectangle.Width / 2 - mFont.MeasureString(Text).X / 2;
            var y = Rectangle.Y + Rectangle.Height / 2 - mFont.MeasureString(Text).Y / 2;
            spriteBatch.DrawString(mFont, Text, new Vector2(x, y), PenColour);
        }

        public override void Update() // Check if the button has been clicked or if the mouse is over the button.
        {
            mPreviousMouse = mCurrentMouse;
            mCurrentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(mCurrentMouse.X, mCurrentMouse.Y, 1, 1);
            mIsHovering = false;

            if (!mouseRectangle.Intersects(Rectangle))
            {
                return;
            }
            mIsHovering = true;

            if (mCurrentMouse.LeftButton == ButtonState.Released && mPreviousMouse.LeftButton == ButtonState.Pressed && !mDeactivated)
            {
                Click?.Invoke(this, new EventArgs());
            }
        }
    }
}
