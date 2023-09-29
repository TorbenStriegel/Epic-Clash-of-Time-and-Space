using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;


namespace ECTS.Objects.GameObjects.uncontrollable_units.spaceship
{
    [DataContract]
    public sealed class Spaceship1: GameObject
    {
        /// <summary>
        /// Spaceship. 
        /// </summary>
        public Spaceship1(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Spaceship;

            mTextureName = "Objects/Spaceship1";
            mFace.X = 0; mFace.Y = 0; mFace.Width = 300; mFace.Height = 454;

            mIsActing = false;
            mIsMarked = false;
            mIsMarkable = true;
            IsInteracting = true;
            IsColliding = false;

            mSpeed = 0;
            mRange = 0;

            mDefenseStrength = 500;
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

            }
            if (Health >= 100)
            {
                mFace.X = 0; mFace.Y = 0; mFace.Width = 300; mFace.Height = 454;
            }
            else if (Health >= 70 && Health < 100)
            {
                mFace.X = 300; mFace.Y = 0; mFace.Width = 300; mFace.Height = 454;
            }
            else if (Health >= 30 && Health < 70)
            {
                mFace.X = 600; mFace.Y = 0; mFace.Width = 300; mFace.Height = 454;
            }
            else if (Health >= 10 && Health < 30)
            {
                mFace.X = 900; mFace.Y = 0; mFace.Width = 300; mFace.Height = 454;
            }
            else if (Health <= 10)
            {
                mFace.X = 1200; mFace.Y = 0; mFace.Width = 300; mFace.Height = 454;
            }
        }

        public override void Update(GameTime gameTime)
        {
            ChangeFace("");
            mGameLoop.ObjectManager.DataManager.mSpaceshipRepairing = Health;
        }

        public override void Draw()
        {
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, Global.mColor);
        }
    }
}
