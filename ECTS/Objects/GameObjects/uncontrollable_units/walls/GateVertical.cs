using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Components;
using ECTS.Pathfinder;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.walls
{
    [DataContract]
    public sealed class GateVertical: GameObject
    {
        
        public GateVertical(GameLoop gameLoop) : base(gameLoop)
        {
            mGameLoop = gameLoop;
            Initialize();
        }

        private void Initialize()
        {
            mObjectType = ObjectType.Gate;

            mTextureName = "Objects/wall_gate_wide_2";
            mFace.X = 108; mFace.Y = 4; mFace.Width = 48; mFace.Height = 96;

            mIsColliding = false;
            mIsActing = false;
            mIsMarked = false;
            IsInteracting = true;
            mIsMarkable = false;

            mSpeed = 0;

            mRange = 0;

            mDefenseStrength = 1000;
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
           if (face.Equals("SwitchGate"))
           {
                if (IsColliding)
                {
                    mFace.X = 108; mFace.Y = 4; mFace.Width = 48; mFace.Height = 96;
                    IsColliding = false;
                }
                else if (mGameLoop.ObjectManager.DataManager.mUnits.GetEntriesInRectangle(Position).Count < 2)
                {
                    mFace.X = 165; mFace.Y = 4; mFace.Width = 48; mFace.Height = 96;
                    IsColliding = true;
                    var tempActiveFlocks = new HashSet<Flock>(mGameLoop.ObjectManager.DataManager.ActiveFlocks);
                    foreach (var flock in tempActiveFlocks)
                    {
                        flock.ChangePath(mGameLoop);
                    }
                }
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
