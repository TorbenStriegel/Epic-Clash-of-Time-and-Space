
using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.weapons
{
    [DataContract]
    internal sealed class GrenadePlayerExplosion: GameObject
    {
        private AnimationManager AnimationManager { get; set; }
        /// <summary>
        /// Explosion for Grenades (and other purposes?)
        /// </summary>
        public GrenadePlayerExplosion(GameLoop gameLoop) : base(gameLoop)

        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Explosion;

            mTextureName = "Objects/GrenadeExplosion";
            mFace.X = 0; mFace.Y = 0; mFace.Width = 190; mFace.Height = 190;

            mIsActing = true;
            IsInteracting = true;
            mIsMarkable = false;
            mIsMarked = false;

            mRange = -40;

            mObjectLifetime = 0.5f;

            AnimationManager = new AnimationManager();
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
            if (face.Equals("Dead"))
            {
                mFace.X = 0; mFace.Y = 0; mFace.Width = 0; mFace.Height = 0;
            }
        }
        public override void Update(GameTime gameTime)
        {

            if (AnimationManager.Animate(gameTime, this, 190, 6, 0f))
            {
                ObjectAge = ObjectLifetime;
            }

        }
        public override void Draw()
        {
            if (mSpriteTexture == null)
            {
                mGameLoop.ObjectManager.LoadContent();
            }
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, Global.mColor);
        }
    }
}
