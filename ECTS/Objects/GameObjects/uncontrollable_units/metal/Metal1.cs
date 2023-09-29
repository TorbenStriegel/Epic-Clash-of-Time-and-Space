using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.metal
{
    [DataContract]
    internal sealed class Metal1: GameObject
    {
        /// <summary>
        /// Metal object which can be harvested by workers.
        /// </summary>
        public Metal1(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Metal;

            mTextureName = "Objects/metal";
            mFace.X = 11; mFace.Y = 127; mFace.Width = 53; mFace.Height = 61;

            mIsActing = false;
            mIsMarked = false;
            mIsMarkable = false;
            IsInteracting = true;

            mSpeed = 0;
            mRange = 0;
            mDefenseStrength = 10;
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
            //
        }
        public override void Draw()
        {
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, Global.mColor);
        }


    }
}
