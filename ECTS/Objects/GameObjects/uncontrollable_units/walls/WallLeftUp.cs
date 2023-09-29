using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.walls
{
    [DataContract]
    internal sealed class WallLeftUp: GameObject
    {
        private Rectangle mFaceBackground;          // used to display the broken wall as background.
        /// <summary>
        /// Wall for lower left corner of a rectangle (actually goes down, then right if followed from the top).
        /// Sprite (appearance) depends on health.
        /// </summary>
        public WallLeftUp(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Wall;

            mTextureName = "Objects/walls";
            mFace.X = 0; mFace.Y = 0; mFace.Width = 48; mFace.Height = 48;
            mBackgroundTextureName = "Objects/walls";
            mFaceBackground.X = 0; mFaceBackground.Y = 112; mFaceBackground.Width = mFace.Width; mFaceBackground.Height = mFace.Height;

            mIsColliding = true;
            mIsActing = false;
            mIsMarked = false;
            IsInteracting = true;
            mIsMarkable = false;

            mSpeed = 0;

            mRange = 0;

            mDefenseStrength = 1000;
        }

        // DeSerialization: Call internal initializing methods:
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            //**** Set all attributes that are initialized in this class here:
            // For GameLoop use "mGameLoop = Global.mGameLoop;"
            mGameLoop = Global.mGameLoop;

            //**** Call all functions that initialize attributes here:
            Initialize();

        }

        public override void ChangeFace(string face)
        {
            if (face.Equals("Complete"))
            {
                mFace.X = 0; mFace.Y = 0; mFace.Width = 48; mFace.Height = 48;
            }
            if (face.Equals("Broken"))
            {
                mFace.X = 0; mFace.Y = 56; mFace.Width = 48; mFace.Height = 48;
            }
            if (face.Equals("Dead"))
            {
                mFace.X = 0; mFace.Y = 112; mFace.Width = 48; mFace.Height = 48;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //
        }
        public override void Draw()
        {
            mGameLoop.SpriteBatch.Draw(mBackgroundTexture, mPosition, mFaceBackground, Global.mColor);        // draw broken wall in background
            mSpriteColor = Global.mColor;
            mSpriteColor.A = (byte)(127 + 0.5 * (mHealth / 100) * 255);                                                 // make wall transparent depending on health
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, mSpriteColor);
        }
    }
}
