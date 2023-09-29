using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.food
{
    [DataContract]
    internal sealed class Mushroom1: GameObject
    {
        /// <summary>
        /// Large mushrooms representing food. Can be harvested by Workers, Engineers and Fighters.
        /// </summary>
        public Mushroom1(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Food;

            mTextureName = "Player/Food_Icon";
            mFace.X = 8; mFace.Y = 11; mFace.Width = 25; mFace.Height = 27;

            mIsActing = false;
            mIsMarked = false;
            mIsMarkable = false;
            IsInteracting = true;

            mSpeed = 0;
            mRange = 0;
            mDefenseStrength = 0.2f;
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
            if (!face.Equals("Dead"))
            {
                return;
            }

            mFace.X = 0; mFace.Y = 0; mFace.Width = 0; mFace.Height = 0;

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
