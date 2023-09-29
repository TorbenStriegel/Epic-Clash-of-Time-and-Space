using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.controllable_units
{
    [DataContract]
    public sealed class Engineer: GameObject
    {
        private AnimationManager AnimationManager { get; set; }
        /// <summary>
        /// Engineers can repair the spaceship.
        /// </summary>
        public Engineer(GameLoop gameLoop) : base(gameLoop)
        {
            mGameLoop = gameLoop;
            Initialize();                               // initialization (except for game loop) has to be done inside of the Initialize function, which is used by the serializer too.
        }

        private void Initialize()
        {
            AnimationManager = new AnimationManager();
            mTextureName = "Player/engineer_levels";
            mBackgroundTextureName = "Player/PlayerSkeleton";
            mFace.X = 32; mFace.Y = 0 + Level*128; mFace.Width = 32; mFace.Height = 32;
            mObjectType = ObjectType.Player;

            mIsColliding = false;
            mIsActing = true;
            IsInteracting = true;
            mIsMarked = false;
            mIsMarkable = true;

            mSpeed = 2;

            mRange = 70;
            mAttackStrength = 10;
            mDefenseStrength = 10;

        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            mGameLoop = Global.mGameLoop;
            Initialize();
        }
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            ChangeFace("Front");
        }

        public override void ChangeFace(string face)
        {
            if (face.Equals("Front"))
            {
                mFace.Y = 0 + Level * 128;
            }

            if (face.Equals("Left"))
            {
                mFace.Y = 32 + Level * 128;
            }

            if (face.Equals("Right"))
            {
                mFace.Y = 64 + Level * 128;
            }

            if (face.Equals("Back"))
            {
                mFace.Y = 96 + Level * 128;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0)
            {
                if (IsMarked)
                {
                    mGameLoop.ObjectManager.DataManager.MarkedPlayerObjects.Remove(this);
                }
                
                ObjectAge = ObjectLifetime;
                mGameLoop.ObjectManager.DataManager.mDiedUnits += 1;
                mGameLoop.Settings.mDiedUnits += 1;
            }

            if (mObjectState == ObjectState.Idle)
            {
                mFace.X = 32;
                mFace.Y = 0 + Level * 128;
                mTextureName = "Player/engineer_levels";

            }
            if (mObjectState == ObjectState.Moving)
            {
                mTextureName = "Player/engineer_levels";
                AnimationManager.Animate(gameTime, this, 32, 3, 0f);

            } 
            else if (mObjectState == ObjectState.Repairing)
            {
                mTextureName = "Player/engineer_repair_animation";
                AnimationManager.Animate(gameTime, this, 32, 6, 0f);
            }

            if (mOldTextureName != mTextureName)
            {
                mSpriteTexture = mGameLoop.GetTexture(mTextureName);
                mOldTextureName = mTextureName;
                mFace.X = 32;
                mFace.Y = 0 + Level * 128;
            }
        }
        public override void Draw()
        {
            if (mSpriteTexture == null || mBackgroundTexture == null)
            {
                mGameLoop.ObjectManager.LoadContent();
            }

            mGameLoop.SpriteBatch.Draw(mBackgroundTexture, mPosition, mFace, Global.mColor);        // draw skeleton in background
            mSpriteColor = Global.mColor;
            mSpriteColor.A = (byte)(mHealth / 100 * 255);
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, mSpriteColor);

        }

    }

}
