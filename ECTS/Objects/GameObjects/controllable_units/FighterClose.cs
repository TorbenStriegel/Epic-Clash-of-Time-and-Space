
using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.controllable_units
{
    [DataContract]
    public sealed class FighterClose : GameObject
    {
        
        private AnimationManager AnimationManager { get; set; }
       

        /// <summary>
        /// Short distance fighters. Fight monsters with short range of action.
        /// </summary>
        public FighterClose (GameLoop gameLoop) : base(gameLoop)
        {
            mGameLoop = gameLoop;
            Initialize();                           // initialization (except for game loop) has to be done inside of the Initialize function, which is used by the serializer too.
        }
        private void Initialize()
        {
            AnimationManager = new AnimationManager();
            mTextureName = "Player/fighterclose_levels";
            mBackgroundTextureName = "Player/PlayerSkeleton";
            mFace.X = 32; mFace.Y = 0 + mLevel * 128; mFace.Width = 32; mFace.Height = 32;

            mObjectType = ObjectType.Player;

            mIsColliding = false;
            mIsActing = true;
            IsInteracting = true;
            mIsMarked = false;
            mIsMarkable = true;

            mSpeed = 3;

            mRange = 300;

            mAttackStrength = 35;           // Attack strength is given to bullet.
            mAttackFrequency = 3;
            mDefenseStrength = 30;
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
                mTextureName = "Player/fighterclose_levels";
                mSpriteTexture = mGameLoop.GetTexture(mTextureName);

            } else if (mObjectState == ObjectState.Moving)
            {
                mTextureName = "Player/fighterclose_levels";
                AnimationManager.Animate(gameTime, this, 32, 3,0f);
            } else if (mObjectState == ObjectState.Fighting)
            {
                mTextureName = "Player/fighterclose_levels";
                // if shooting animation has ended create new bullet.
                if (AnimationManager.Animate(gameTime, this, 32, 3, 1/AttackFrequency))
                {
                    mGameLoop.RenderManager.SoundManager.PlaySound("lasershot");
                    mGameLoop.ObjectManager.ActionManager.Fight.BulletShot(this, mCurrentTargetObject);
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
