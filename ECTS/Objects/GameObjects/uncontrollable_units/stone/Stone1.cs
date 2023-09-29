using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.stone
{
    [DataContract]
    internal sealed class Stone1: GameObject
    {
        /// <summary>
        /// Stone object which can be harvested by worker. 
        /// </summary>
        public Stone1(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Stone;

            mTextureName = "Objects/stone";
            mFace.X = 97; mFace.Y = 132; mFace.Width = 46; mFace.Height = 62;

            mIsActing = false;
            mIsMarked = false;
            IsInteracting = true;
            mIsMarkable = false;

            mSpeed = 0;
            mRange = 0;
            mDefenseStrength = 10f;
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
