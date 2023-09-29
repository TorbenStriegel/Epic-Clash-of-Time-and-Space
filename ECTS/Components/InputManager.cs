using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ECTS.Components { 
    /// <summary>
    /// the InputManager sets flags -> saves the input of the player in variables
    /// which button is pressed? Where is the mouse? Which area is marked?
    /// </summary>
    public sealed class InputManager
    {
        private GameLoop GameLoop { get; }

        private MouseState mLastMouseState;
        private MouseState mCurrentMouseState;
        internal bool mLeftMouseButtonJustReleased;
        internal bool mRightMouseButtonJustReleased;

        private float mXStart;
        private float mYStart;

        private float mOldScrollWheelValue;
        private float mTempScrollWheelValue;

        internal Vector2 mCurrentMouseGamePosition; // Mouse Position in Game-Coordinates

        internal InputManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            mYStart = 0;
            mXStart = 0;
            mOldScrollWheelValue = 0;
            mTempScrollWheelValue = 0;
        }


        /// <summary>
        /// updates keys and mouse clicks
        /// </summary>
        internal void Update()
        {
            // CLICK SOUND
            if (mCurrentMouseState.LeftButton == ButtonState.Pressed && mLastMouseState.LeftButton != ButtonState.Pressed ||
                mCurrentMouseState.RightButton == ButtonState.Pressed && mLastMouseState.RightButton != ButtonState.Pressed)
            {
                GameLoop.RenderManager.SoundManager.PlaySound("button");
            }


            var zoom = GameLoop.ObjectManager.DataManager.mZoomValue;
            var tempZoomValue = (float)Mouse.GetState().ScrollWheelValue / 700 + 1 - mTempScrollWheelValue; // Zoom function
            if ((int)(zoom * 1000) != (int)(tempZoomValue * 1000))
            {
                if (tempZoomValue > 0.6 && tempZoomValue < 3.0)
                {
                    GameLoop.RenderManager.CameraManager.SetCameraPosition(0, 0);
                    GameLoop.ObjectManager.DataManager.mZoomValue = tempZoomValue;
                    GameLoop.RenderManager.CameraManager.SetZoom(tempZoomValue);
                    mOldScrollWheelValue = tempZoomValue;
                }
                else
                {
                    mTempScrollWheelValue += tempZoomValue - mOldScrollWheelValue;
                }
            }

            const int scrollSpeed = 10;
            if (Mouse.GetState().X < 5 && GameLoop.RenderManager.CameraManager.mPosition.X < 0)
            {
                GameLoop.RenderManager.CameraManager.SetCameraPosition(scrollSpeed, 0);
            }

            var rightBorder = GameLoop.DisplayWidth - GameLoop.ObjectManager.DataManager.mWorldRectangle.Width;
            if (Mouse.GetState().X > GameLoop.DisplayWidth-5 && GameLoop.RenderManager.CameraManager.mPosition.X > rightBorder)
            {
                GameLoop.RenderManager.CameraManager.SetCameraPosition(-scrollSpeed, 0);
            }
            if (Mouse.GetState().Y < 5 && GameLoop.RenderManager.CameraManager.mPosition.Y < 0)
            {
                GameLoop.RenderManager.CameraManager.SetCameraPosition(0, scrollSpeed);
            }

            var lowerBorder = GameLoop.DisplayHeight - GameLoop.ObjectManager.DataManager.mWorldRectangle.Height -
                              GameLoop.DisplayHeight * 0.15;
            if (Mouse.GetState().Y > GameLoop.DisplayHeight - 5 && GameLoop.RenderManager.CameraManager.mPosition.Y > lowerBorder)
            {
                GameLoop.RenderManager.CameraManager.SetCameraPosition(0, -scrollSpeed);
            }
            
            // H A M B U R G E R        H U D
            UpdateHamburgerMenu();

            // S A F E      M O U S E    C L I C K      K O O R D S
            SafeMouseClickKoords();
            var xStart = mXStart;
            var yStart = mYStart;
            // button is still pressed -> change size of rectangle
            if (mCurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                var xEnd = mCurrentMouseGamePosition.X;
                var yEnd = mCurrentMouseGamePosition.Y;
                // get minimum of x und y coordinate
                var tmp = Math.Min(xStart, xEnd);
                xEnd = Math.Max(xStart, xEnd);
                xStart = (int)tmp;

                tmp = Math.Min(yStart, yEnd);
                yEnd = Math.Max(yStart, yEnd);
                yStart = (int)tmp;

                // marked rectangle size
                GameLoop.RenderManager.ScreenManager.mDestinationRectangle =
                    new Rectangle((int) xStart, (int) yStart, (int) (xEnd - xStart), (int) (yEnd - yStart));
            }
            else
            {
                GameLoop.RenderManager.ScreenManager.mDestinationRectangle = Rectangle.Empty;
            }
        }

        /// <summary>
        /// if mouse hovers over triangle in upper right corner -> dropdown menu opens
        /// </summary>
        private void UpdateHamburgerMenu()
        {
            // is mouse on burger icon -> open hud
            if (IsMouseAboveRectangle((int)(GameLoop.DisplayWidth * 0.96), GameLoop.DisplayWidth, 0, (int)(GameLoop.DisplayWidth * 0.04)) && !GameLoop.RenderManager.ScreenManager.Tutorial.mIsTutorialOpen)
            {
                GameLoop.RenderManager.ScreenManager.mOpenHud = true;
                GameLoop.RenderManager.ScreenManager.mStayOpen = true;
            }

            // is mouse on hud -> leave hud open
            if (IsMouseAboveRectangle((int)(GameLoop.DisplayWidth * 0.71), GameLoop.DisplayWidth, 0,
                    (int)(GameLoop.DisplayHeight * 0.6)) && GameLoop.RenderManager.ScreenManager.mOpenHud)
            {
                GameLoop.RenderManager.ScreenManager.mStayOpen = true;
            }
            else
            {
                // close hud
                GameLoop.RenderManager.ScreenManager.mStayOpen = false;
                GameLoop.RenderManager.ScreenManager.mOpenHud = false;
            }
        }

        private void SafeMouseClickKoords()
        {
            mLeftMouseButtonJustReleased = false;
            mRightMouseButtonJustReleased = false;

            // safe current mouse state
            mLastMouseState = mCurrentMouseState;
            mCurrentMouseState = Mouse.GetState();

            mCurrentMouseGamePosition = Vector2.Transform(mCurrentMouseState.Position.ToVector2(), Matrix.Invert(GameLoop.RenderManager.CameraManager.Transform)); // Mouse.Position to GameWorld.Position


            // release after left click
            if (mCurrentMouseState.LeftButton == ButtonState.Released &&
                mLastMouseState.LeftButton == ButtonState.Pressed)
            {
                mLeftMouseButtonJustReleased = true;

                // mark objects in rectangle
                GameLoop.ObjectManager.MarkObjectInRectangle(GameLoop.RenderManager.ScreenManager
                    .mDestinationRectangle);
            }

            // release after right click
            if (mCurrentMouseState.RightButton == ButtonState.Released &&
                mLastMouseState.RightButton == ButtonState.Pressed)
            {
                mRightMouseButtonJustReleased = true;
            }
            if (mLastMouseState.LeftButton == ButtonState.Released)
            {
                mXStart = mCurrentMouseGamePosition.X;
                mYStart = mCurrentMouseGamePosition.Y;
            }
        }

        /// <summary>
        /// Checks if Mouse is above the given rectangle
        /// </summary>
        /// <param name="xLeft">left top corner of rectangle</param>
        /// <param name="xRight">right top corner</param>
        /// <param name="yTop">left bottom corner</param>
        /// <param name="yBottom">right bottom corner</param>
        /// <returns>true if mouse is above rectangle, else false</returns>
        private static bool IsMouseAboveRectangle(int xLeft, int xRight, int yTop, int yBottom)
        {
            return Mouse.GetState().X > xLeft && Mouse.GetState().X < xRight &&
                   Mouse.GetState().Y > yTop && Mouse.GetState().Y < yBottom;
        }
        public void ResetScrollWheel()
        {
            mTempScrollWheelValue = (float)Mouse.GetState().ScrollWheelValue / 700;
        }
    }
}
