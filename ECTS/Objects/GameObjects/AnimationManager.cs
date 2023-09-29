using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects
{
    /// <summary>
    /// The AnimationManager provides functions for gameObject Animation.
    /// </summary>
    public sealed class AnimationManager
    {
        private float mElapsedTime;                     // elapsed time;

        private const float BasicFrameInterval = 0.1f; // Duration between frames
        private float mCurrentFrameInterval;             // Duration between frames (is changed to pause after last frame)

        private int mFrame;                          // current frame of animations

        
        public AnimationManager()
        {
            mCurrentFrameInterval = BasicFrameInterval;

        }

        /// <summary>
        /// Basic Animation Function.
        /// Goes through all frames of the sprite HORIZONTALLY. Returns True if last frame of animation has been reached, False for all other frames.
        /// </summary>
        /// <returns>True if last Frame of animation has been reached, False for all other Frames.</returns>
        /// <param name="gameTime">Game Time.</param>
        /// <param name="actingObject">Object which is animated.</param>
        /// <param name="pause">Break between animations.</param>
        /// <param name="xStart">X-coordinate of first frame (0 by default)</param>
        /// <param name="frameWidth">Width of Frame (Sprite).</param>
        /// <param name="frameNumber">Numbers of Frames in the animation.</param>
        public bool Animate(GameTime gameTime, GameObject actingObject, int frameWidth, int frameNumber, float pause, int xStart = 0)
        {
            var ready = new bool();
            mElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //add time since last update

            if (mElapsedTime >= mCurrentFrameInterval)
            {
                if (mFrame >= frameNumber - 1)
                {
                    mFrame = 0;
                    mCurrentFrameInterval += pause - mCurrentFrameInterval; // pause between animations
                    ready = true;
                }
                else
                {
                    mFrame++;
                    mCurrentFrameInterval = BasicFrameInterval;
                }

                actingObject.mFace.X = xStart + mFrame * frameWidth;
                mElapsedTime = 0f;
            }

            return ready;
        }
    }
}
