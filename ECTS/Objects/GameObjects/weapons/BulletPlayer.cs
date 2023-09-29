
using System;
using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.weapons
{
    [DataContract]
    internal sealed class BulletPlayer: GameObject
    {
        public Vector2 mFlightDirection;
        /// <summary>
        /// Bullet shot by FighterClose
        /// </summary>
        public BulletPlayer(GameLoop gameLoop) : base(gameLoop)
 
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Bullet;

            mTextureName = "Objects/BulletBlue";
            mFace.X = 0; mFace.Y = 0; mFace.Width = 15; mFace.Height = 15;

            mIsActing = true;
            IsInteracting = true;
            mIsMarkable = false;
            mIsMarked = false;
            
            mSpeed = 7;

            mRange = -5;

            mObjectLifetime = 0.7f;
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
            // Move Bullet depending on FlightDirection and mSpeed
            mPosition.X += (int) Math.Round(mFlightDirection.X * mSpeed);
            mPosition.Y += (int) Math.Round(mFlightDirection.Y * mSpeed);

            // If objectAge > objectLifetime bullet is removed from quad-tree (and deleted by Garbage Collector).
            mObjectAge += (float)gameTime.ElapsedGameTime.TotalSeconds; //add time since last update

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
