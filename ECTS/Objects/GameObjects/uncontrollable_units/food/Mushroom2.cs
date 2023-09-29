using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.food
{
    [DataContract]
    internal sealed class Mushroom2: GameObject
    {
        /// <summary>
        /// 3 small mushrooms representing food. Can be harvested by Workers, Engineers and Fighters.
        /// </summary>
        public Mushroom2(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Food;

            mTextureName = "Objects/Food_Icon_2";
            mFace.X = 2; mFace.Y = 0; mFace.Width = 30; mFace.Height = 40;

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
