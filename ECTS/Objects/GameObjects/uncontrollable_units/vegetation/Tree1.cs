using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.vegetation
{
    [DataContract]
    public sealed class Tree1: GameObject
    {
        /// <summary>
        /// Medium sized tree which can be harvested by worker.
        /// </summary>
        /// 
        public Tree1(GameLoop gameLoop) : base(gameLoop)
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
            mObjectType = ObjectType.Tree;
            mTextureName = "Objects/trees";
            mSpeed = 0;
            mFace = new Rectangle(98, 192, 59, 72);
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
                mFace.X = 63; mFace.Y = 0; mFace.Width = 18; mFace.Height = 16;
                mPosition.X += 20; mPosition.Y += 29; mPosition.Width = 18; mPosition.Height = 16;
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
