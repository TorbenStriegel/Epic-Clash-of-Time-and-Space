using System;
using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.weapons
{
    [DataContract]
    internal sealed class GrenadePlayer: GameObject
    {
        public Vector2 mFlightDirection;

        public Vector2 mStartingPoint;
        public float mTravelDistance;
        private float mDistanceTravelled;

        
        /// <summary>
        /// Bullet shot by FighterClose
        /// </summary>
        public GrenadePlayer(GameLoop gameLoop) : base(gameLoop)

        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Grenade;

            mTextureName = "Objects/Grenade";
            mFace.X = 0; mFace.Y = 0; mFace.Width = 15; mFace.Height = 15;

            mIsActing = false;
            IsInteracting = false;
            mIsMarkable = false;
            mIsMarked = false;

            mSpeed = 5;

            mRange = 100;

            mObjectLifetime = 2f;
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
            if (!face.Equals("Dead"))
            {
                return;
            }

            mFace.X = 0; mFace.Y = 0; mFace.Width = 0; mFace.Height = 0;
        }

        public override void Update(GameTime gameTime)
        {
            // Move Grenade depending on FlightDirection and mSpeed
            mPosition.X += (int)Math.Round(mFlightDirection.X * mSpeed);
            mPosition.Y += (int)Math.Round(mFlightDirection.Y * mSpeed);
            // When Target is reached explode
            mDistanceTravelled = Vector2.Distance(mStartingPoint, ObjectCenter);
            if (mDistanceTravelled > mTravelDistance)
            {

                mSpeed = 0;
                mObjectAge = mObjectLifetime;
                mGameLoop.RenderManager.SoundManager.PlaySound("explosion");
                mGameLoop.ObjectManager.mTempTree.Add(new GrenadePlayerExplosion(mGameLoop)
                {
                    Position = new Rectangle((int)ObjectCenter.X - 190/2, (int)ObjectCenter.Y -190/2, 190, 190),
                    IsActing = true, AttackStrength = AttackStrength,
                    AttackingUnit = this.AttackingUnit
                });

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

