
using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.controllable_units
{
    [DataContract]
    public sealed class FighterDistance : GameObject
    {
        private AnimationManager AnimationManager { get; set; }
       
        /// <summary>
        /// Long distance fighters. Shoots Photon-Grenades at Monsters over a long distance.
        /// </summary>
        public FighterDistance(GameLoop gameLoop) : base(gameLoop)
        {
            mGameLoop = gameLoop;
            Initialize();                                       // initialization (except for game loop) has to be done inside of the Initialize function, which is used by the serializer too.
        }
        private void Initialize()
        {
            AnimationManager = new AnimationManager();
            mTextureName = "Player/fighterdistance_levels";
            mBackgroundTextureName = "Player/PlayerSkeleton";
            mFace.X = 32; mFace.Y = 0 + Level * 128; mFace.Width = 32; mFace.Height = 32;

            mObjectType = ObjectType.Player;

            mIsColliding = false;
            mIsActing = true;
            IsInteracting = true;
            mIsMarked = false;
            mIsMarkable = true;

            mSpeed = 2;

            mRange = 800;

            mAttackStrength = 50;           // Attack strength is given to grenade.
            mAttackFrequency = 2;
            mDefenseStrength = 20;
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
                mGunMuzzle.X = Position.X + 5;
                mGunMuzzle.Y = Position.Y + 16;
            }

            if (face.Equals("Left"))
            {
                mFace.Y = 32 + Level * 128;
                mGunMuzzle.X = Position.X + 5;
                mGunMuzzle.Y = Position.Y + 16;
            }

            if (face.Equals("Right"))
            {
                mFace.Y = 64 + Level * 128;
                mGunMuzzle.X = Position.X + 27;
                mGunMuzzle.Y = Position.Y + 16;
            }

            if (face.Equals("Back"))
            {
                mFace.Y = 96 + Level * 128;
                mGunMuzzle.X = Position.X + 27;
                mGunMuzzle.Y = Position.Y + 16;
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

            // STOP Fighting / Destroying if CurrentTargetObject dead.
            if (mCurrentTargetObject != null && State != ObjectState.Moving && mCurrentTargetObject.Health <= 0)
            {
                State = ObjectState.Idle;
            }

            if (mObjectState == ObjectState.Idle)
            {
                mTextureName = "Player/fighterdistance_levels";
                mSpriteTexture = mGameLoop.GetTexture(mTextureName);
            }
            if (mObjectState == ObjectState.Moving)
            {
                mTextureName = "Player/fighterdistance_levels";
                AnimationManager.Animate(gameTime, this, 32, 3, 0f);
            }
            if (mObjectState == ObjectState.Fighting)
            {
                mTextureName = "Player/fighterdistance_levels";

                // if shooting animation has ended create new grenade.
                if (AnimationManager.Animate(gameTime, this, 32, 1, 1f))
                {
                    mGameLoop.ObjectManager.ActionManager.Fight.GrenadeShot(this, mCurrentTargetObject);
                }
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
