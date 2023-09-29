using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.vegetation
{
    [DataContract]
    internal sealed class BabyTree1: GameObject
    {
        /// <summary>
        /// Baby tree which can be harvested by worker.
        /// </summary>
        public BabyTree1(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();



        }
        private void Initialize()
        {
            
            mIsColliding = false;
            mIsActing = false;
            IsInteracting = true;
            mHealth = 30;                   // less health, less resources
            mObjectType = ObjectType.Tree;
            mTextureName = "Objects/trees";
            mSpeed = 0;
            mFace = new Rectangle(16, 32, 15, 32);
            mIsMarked = false;
            mIsMarkable = false;

            mDefenseStrength = 10f;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            mGameLoop = Global.mGameLoop;
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
