using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Components;
using ECTS.Objects.GameObjects.uncontrollable_units.dna;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.controllable_units
{
    [DataContract]
    internal sealed class MonsterFrog: GameObject
    {
        private AnimationManager AnimationManager { get; set; }
        private bool mDnaDropped;
        /// <summary>
        /// Monster of type frog.
        /// </summary>
        public MonsterFrog(GameLoop gameLoop) : base(gameLoop)
        {
            mGameLoop = gameLoop;
            Initialize();                       // initialization (except for game loop) has to be done inside of the Initialize function, which is used by the serializer too.
        }

        private void Initialize()
        {
            AnimationManager = new AnimationManager();
            mTextureName = "Player/Monster";
            mFace.X = 144; mFace.Y = 47; mFace.Width = 48; mFace.Height = 27;

            mObjectType = ObjectType.Enemy;
            mObjectTargetList = new List<GameObject>();

            mIsColliding = false;
            mIsActing = true;
            IsInteracting = true;
            mIsMarked = false;
            mIsMarkable = true;

            mSpeed = 2;

            mRange = 500;

            mAttackStrength = 80;           // Attack strength is given to grenade.
            mAttackFrequency = 1;
            mStrikingDistance = 600;
            mDefenseStrength = 20;

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
            if (face.Equals("Front"))
            {
                mFace.Y = 47;
                mGunMuzzle.X = Position.X + 24;
                mGunMuzzle.Y = Position.Y + 5;
            }

            if (face.Equals("Left"))
            {
                 mFace.Y = 119;
                 mGunMuzzle.X = Position.X + 5;
                 mGunMuzzle.Y = Position.Y + 5;
            }

            if (face.Equals("Right"))
            {
                mFace.Y = 191;
                mGunMuzzle.X = Position.X + 27;
                mGunMuzzle.Y = Position.Y + 5;
            }

            if (face.Equals("Back"))
            {
                mFace.Y = 263;
                mGunMuzzle.X = Position.X + 24;
                mGunMuzzle.Y = Position.Y + 5;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0)
            {
                if (IsMarked)
                {
                    mGameLoop.ObjectManager.DataManager.MarkedEnemiesObjects.Remove(this);
                }
                // DROP DNA
                if (!mDnaDropped)
                {
                    var r = new Random();
                    if (r.Next(0, 9) < 2)
                    {
                        mGameLoop.RenderManager.SoundManager.PlaySound("dna_appears");
                        mGameLoop.ObjectManager.mTempTree.Add(new Dna(mGameLoop)
                            { Position = new Rectangle(Position.X, Position.Y, 33, 33) });
                    }
                    mDnaDropped = true;
                }

                ObjectAge = ObjectLifetime;
                mGameLoop.ObjectManager.DataManager.mKilledEnemies += 1;
                mGameLoop.Settings.mKilledEnemies += 1;
            }

            if (UnderAttack && TimeSinceAttack < mMaxTimeSinceAttack)
            {
                TimeSinceAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
            } 
            else if (TimeSinceAttack >= mMaxTimeSinceAttack)
            {
                UnderAttack = false;
                TimeSinceAttack = 0;
            }
            
            // STOP Fighting / Destroying if CurrentTargetObject dead.
            if (mCurrentTargetObject != null && State != ObjectState.Moving && mCurrentTargetObject.Health <= 0)
            {
                State = ObjectState.Idle;
            }


            if (mObjectState == ObjectState.Idle)
            {
                mTextureName = "Player/Monster";
                mSpriteTexture = mGameLoop.GetTexture(mTextureName);

            } else if (mObjectState == ObjectState.Moving)
            {
                mTextureName = "Player/Monster";
                AnimationManager.Animate(gameTime, this, 48, 3, 0f, 144);
            }
            
            else if (mObjectState == ObjectState.Destroying)
            {
                mTextureName = "Player/Monster";
                if (AnimationManager.Animate(gameTime, this, 48, 1, 1 / AttackFrequency, 144))
                {
                    mGameLoop.ObjectManager.ActionManager.Fight.GrenadeShot(this, mCurrentTargetObject);
                }
            }
            else if (mObjectState == ObjectState.Fighting)
            {
                mTextureName = "Player/Monster";
                if (AnimationManager.Animate(gameTime, this, 48, 3, 1 / AttackFrequency, 144))
                {
                    mGameLoop.ObjectManager.ActionManager.Fight.GrenadeShot(this, mCurrentTargetObject);
                }
            }
        }

        public override void Draw()
        {
            mSpriteColor = Global.mColor;
            mSpriteColor.A = (byte)(127 + 0.5 * (mHealth / 100) * 255);
            LoadContent();
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, mSpriteColor);
        }

    }
}
