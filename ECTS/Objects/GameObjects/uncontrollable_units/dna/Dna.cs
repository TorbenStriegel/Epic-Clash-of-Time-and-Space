using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.dna
{
    [DataContract]
    internal sealed class Dna : GameObject
    {
        
        /// <summary>
        /// DNA dropped by monsters after death.
        /// </summary>
        public Dna(GameLoop gameLoop) : base(gameLoop)

        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            
            mObjectType = ObjectType.Dna;

            mTextureName = "Objects/dna";
            mFace.X = 0; mFace.Y = 0; mFace.Width = 33; mFace.Height = 33;

            mIsActing = true;
            IsInteracting = true;
            mIsMarkable = false;
            mIsMarked = false;

            mSpeed = 0;

            mRange = 40;

            mObjectLifetime = 45f;
        }

        // DeSerialization: Call internal initializing methods:
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            //**** Set all attributes that are initialized in this class here:
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
            // If objectAge > objectLifetime DNA is removed from game
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
