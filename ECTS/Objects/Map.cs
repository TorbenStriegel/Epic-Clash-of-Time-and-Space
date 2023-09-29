using System;
using ECTS.Data;
using ECTS.Objects.GameObjects.controllable_units;
using ECTS.Objects.GameObjects.uncontrollable_units.food;
using ECTS.Objects.GameObjects.uncontrollable_units.metal;
using ECTS.Objects.GameObjects.uncontrollable_units.stone;
using ECTS.Objects.GameObjects.uncontrollable_units.vegetation;
using ECTS.Objects.GameObjects.uncontrollable_units.walls;
using Microsoft.Xna.Framework;


namespace ECTS.Objects
{
    /// <summary>
    /// Creates the standard Map
    /// </summary>
    internal sealed class Map
    {
        private readonly GameLoop mGameLoop;
        private readonly DataManager mDataManager;
        internal Rectangle mBaseCornerRectangle;
        
        internal Map(GameLoop gameLoop, DataManager dataManager)
        {
            mGameLoop = gameLoop;
            mDataManager = dataManager;

            mBaseCornerRectangle = new Rectangle { X = 1968, Y = 1872, Width = 912, Height = 1104 };
        }

        internal void TechDemo()
        {
            var r = new Random();
            for (var i = 0; i < 400; i++)
            {
                mDataManager.mUnits.Add(new FighterClose(mGameLoop)
                    { Position = new Rectangle(r.Next(1500, 4500), r.Next(1000, 4000), 32, 32), Range = 400});
            }

            for (var i = 0; i < 600; i++)
            {
                mDataManager.mUnits.Add(new MonsterPig(mGameLoop)
                    { Position = new Rectangle(r.Next(1500, 4500), r.Next(1000, 4000), 48, 52), Range = 500});
            }
            for (var i = 0; i < 20; i++)
            {
                mDataManager.mUnits.Add(new FighterDistance(mGameLoop)
                    { Position = new Rectangle(r.Next(1500, 4500), r.Next(1000, 3000), 32, 32), Range = 500 });
                mDataManager.mUnits.Add(new MonsterFrog(mGameLoop)
                    { Position = new Rectangle(r.Next(1500, 4500), r.Next(1000, 3000), 48, 27), Range = 500 });
            }

            // This wall is needed so that Monsters can attack distance fighters (it is a workaround of course.)
            mDataManager.mUnits.Add(new WallHorizontal(mGameLoop)
                { Position = new Rectangle(0, 0, 48, 48), Health = 100});

            // Main forest regions
            for (var i = 0; i < 100; i++)
            {
                // Forest 1: 60 <= x <= 1000, 72 <= y <= 700
                
                // Forest 2: 2600 <= x <= 4700, 3700 <= 4720
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2600, 3700), r.Next(3700, 4720), 59, 72) });
               
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(3701, 4700), r.Next(3700, 4720), 59, 72) });
                
                // Forest 3: 3000 <= x <= 4000, 700 <= y <= 1300
                
                // Forest 4: 80 <= x <= 400, 3000 <= y <= 4700
               
                // Forest 5: 1050 <= x <= 1700, 2000 <= y <= 2800
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(1050, 1700), r.Next(2000, 2800), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(1050, 1700), r.Next(2000, 2800), 15, 16) });
            }

            // smoothing edges of forests
            for (var i = 0; i < 20; i++)
            {
                // Forest 1
                
                // Forest 2
                // upper edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2850, 4550), r.Next(3600, 3700), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(2850, 4550), r.Next(3600, 3700), 15, 16) });
                // left edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2350, 2600), r.Next(3900, 4500), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(2350, 2600), r.Next(3900, 4500), 15, 16) });

                // Forest 3
                // upper edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(3150, 3850), r.Next(550, 700), 59, 72) });
                // lower edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(3150, 3850), r.Next(1300, 1450), 59, 72) });
                // right edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(4000, 4150), r.Next(800, 1200), 59, 72) });
                // left edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2850, 3000), r.Next(800, 1200), 59, 72) });

                // Forest 4
               

                // Forest 5
                // upper edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(1800, 2000), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(1800, 2000), 15, 16) });
                // lower edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(2800, 3000), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(2800, 3000), 15, 16) });
                // left edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(900, 1050), r.Next(2150, 2650), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(900, 1050), r.Next(2150, 2650), 15, 16) });
            }
        }



        /// <summary>
        /// Spawns "infinite" (very high health) resources (iron, metal) at the borders of the map.
        /// </summary>
        internal void SpawnInfRes()
        {
            var r = new Random();
            
            const int outer = 140; // Outer-Border
            const int inner = 200; // Inner-Border

            const int directions = 4;  // Number of borders

            const int bulk = 4; // number of resources at "same" position;
            const int spread = 75; // Spread of bulk = spread * 2

            var size = mDataManager.mWorldRectangle.Height;  // Size of (quadratic) Map
            const int dist = 2500;
            for (var i = 0; i < size / dist; i++)  // "size / dist" --> Once for every dist pixel-width/height of map (but at random position)
            {
                // Stone1
                for (var j = 0; j < directions; j++)
                {
                    var x = r.Next(outer, inner);
                    var y = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Stone1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }

                    x = r.Next(size - inner, size - outer);
                    y = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Stone1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }

                    y = r.Next(outer, inner);
                    x = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Stone1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }

                    y = r.Next(size - inner, size - outer);
                    x = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Stone1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }
                }

                // Metal1
                for (var j = 0; j < directions; j++)
                {
                    var x = r.Next(outer, inner);
                    var y = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Metal1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }

                    x = r.Next(size - inner, size - outer);
                    y = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Metal1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }

                    y = r.Next(outer, inner);
                    x = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Metal1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }

                    y = r.Next(size - inner, size - outer);
                    x = r.Next(outer, size - outer);
                    for (var k = 0; k < bulk; k++)
                    {
                        mDataManager.mUnits.Add(new Metal1(mGameLoop)
                        {
                            Position = new Rectangle(r.Next(x - spread, x + spread), r.Next(y - spread, y + spread), 46, 62),
                            Health = 9999999
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Spawns Trees and Food (2 kind for each) with normal health randomly on the map except in the center.
        /// </summary>
        /// <param name="factor">Defines how many objects of each resource is spawned: factor * 2</param>
        /// <param name="boolSpriteTexture"></param>
        internal void SpawnRenewableRes(int factor, bool boolSpriteTexture = false)
        {
            var r = new Random();

            var size = mDataManager.mWorldRectangle.Height;  // Size of (quadratic) Map
            const int outer = 200; // Outer-Border
            var inner = size / 2 - Math.Max(mBaseCornerRectangle.Height, mBaseCornerRectangle.Width) / 2 - 100; // Inner-Border
            
            for (var i = 0; i < factor; i++)
            {
                var x = r.Next(outer, size - outer);
                var y = r.Next(outer, size - outer);
                if (x > inner && x < size - inner)
                {
                    while (y > inner && y < size - inner)
                    {
                        y = r.Next(outer, size - outer);
                    }
                }
                var xOffset = r.Next(x - 80, x + 80);
                var yOffset = r.Next(y - 120, y + 120);

                if (boolSpriteTexture)  // hack workaround for error where mSpriteTexture = 0 in some cases but not in another calling mGameLoop.GetTexture would result in another error.
                {
                    mDataManager.mUnits.Add(new Tree1(mGameLoop)
                    {
                        Position = new Rectangle(x, y, 59, 72),
                        mSpriteTexture = mGameLoop.GetTexture("Objects/trees")
                    });
                    mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                    {
                        Position = new Rectangle(xOffset, yOffset, 15, 16),
                        mSpriteTexture = mGameLoop.GetTexture("Objects/trees")
                    });
                }
                else
                {
                    mDataManager.mUnits.Add(new Tree1(mGameLoop) {Position = new Rectangle(x, y, 59, 72)});
                    mDataManager.mUnits.Add(new BabyTree1(mGameLoop){Position = new Rectangle(x + 65, y + 80, 15, 16)});
                }


                x = r.Next(outer, size - outer);
                y = r.Next(outer, size - outer);
                if (x > inner && x < size - inner)
                {
                    while (y > inner && y < size - inner)
                    {
                        y = r.Next(outer, size - outer);
                    }
                }
                xOffset = r.Next(x - 60, x + 60);
                yOffset = r.Next(y - 60, y + 60);

                if (boolSpriteTexture)
                {
                    mDataManager.mUnits.Add(new Mushroom1(mGameLoop)
                    {
                        Position = new Rectangle(x, y, 25, 27),
                        mSpriteTexture = mGameLoop.GetTexture("Player/Food_Icon")
                    });
                    mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                    {
                        Position = new Rectangle(xOffset, yOffset, 32, 32),
                        mSpriteTexture = mGameLoop.GetTexture("Objects/Food_Icon_2")
                    });
                }
                else
                {
                    mDataManager.mUnits.Add(new Mushroom1(mGameLoop) {Position = new Rectangle(x, y, 25, 27)});
                    mDataManager.mUnits.Add(new Mushroom2(mGameLoop) {Position = new Rectangle(x + 30, y + 30, 32, 32)});
                }
            }
        }

        /// <summary>
        /// Does many unspeakable things.
        /// </summary>
        internal void DoUnsortedThings()
        {
            mDataManager.mUnits.Add(mDataManager.Spaceship);
            mDataManager.mUnits.Add(mDataManager.GateN);
            mDataManager.mUnits.Add(mDataManager.GateO);
            mDataManager.mUnits.Add(mDataManager.GateS);
            mDataManager.mUnits.Add(mDataManager.GateW);

            var r = new Random();

            mDataManager.mUnits.Add(new FighterClose(mGameLoop) { Position = new Rectangle(2100, 2050, 32, 32) });
            mDataManager.mUnits.Add(new Engineer(mGameLoop) { Position = new Rectangle(2200, 2050, 32, 32) });
            mDataManager.mUnits.Add(new Worker(mGameLoop) { Position = new Rectangle(2250, 2050, 32, 32) });
            
            
            // Walls
            for (var i = 0; i < 17; i++)
            {
                if (i == 8)  // Gate Position
                {
                    i++;
                    continue;
                    
                }
                mDataManager.mUnits.Add(new WallHorizontal(mGameLoop)
                {
                    Position = new Rectangle(2016 + i * 48, 1872, 48, 48),
                    mMonsterAttackWallPosition = new Vector2(2016 + i * 48 + 24, 1872 - 24)
                });
                mDataManager.mUnits.Add(new WallHorizontal(mGameLoop)
                {
                    Position = new Rectangle(2016 + i * 48, 2928, 48, 48),
                    mMonsterAttackWallPosition = new Vector2(2016 + i * 48 + 24, 3000)
                });

            }

            for (var j = 0; j < 21; j++)
            {
                if (j == 10)  // Gate Position
                {
                    j++;
                    continue;
                }
                mDataManager.mUnits.Add(new WallVertical(mGameLoop)
                {
                    Position = new Rectangle(1968, 1920 + j * 48, 48, 48),
                    mMonsterAttackWallPosition = new Vector2(1968 - 24, 1920 + j * 48 + 24)
                });
                mDataManager.mUnits.Add(new WallVertical(mGameLoop)
                {
                    Position = new Rectangle(2832, 1920 + j * 48, 48, 48), Health = 0,
                    mMonsterAttackWallPosition = new Vector2(2904, 1920 + j * 48 + 24)
                });
            }

            mDataManager.mUnits.Add(new WallRightDown(mGameLoop)
            {
                Position = new Rectangle(
                mBaseCornerRectangle.X + mBaseCornerRectangle.Width - 48,
                mBaseCornerRectangle.Y, 48, 48),
                mMonsterAttackWallPosition = new Vector2(mBaseCornerRectangle.X + mBaseCornerRectangle.Width + 24, mBaseCornerRectangle.Y + 24)
            });
            mDataManager.mUnits.Add(new WallDownLeft(mGameLoop)
            {
                Position = new Rectangle(
                mBaseCornerRectangle.X + mBaseCornerRectangle.Width - 48,
                mBaseCornerRectangle.Y + mBaseCornerRectangle.Height - 48, 48, 48),
                mMonsterAttackWallPosition = new Vector2(mBaseCornerRectangle.X + mBaseCornerRectangle.Width + 24, mBaseCornerRectangle.Y + mBaseCornerRectangle.Height - 24)
            });
            mDataManager.mUnits.Add(new WallLeftUp(mGameLoop)
            {
                Position = new Rectangle(
                mBaseCornerRectangle.X, mBaseCornerRectangle.Y + mBaseCornerRectangle.Height - 48, 48, 48),
                mMonsterAttackWallPosition = new Vector2(mBaseCornerRectangle.X - 24, mBaseCornerRectangle.Y + mBaseCornerRectangle.Height - 24)
            });
            mDataManager.mUnits.Add(new WallUpRight(mGameLoop)
            {
                Position = new Rectangle(
                mBaseCornerRectangle.X, mBaseCornerRectangle.Y, 48, 48),
                mMonsterAttackWallPosition = new Vector2(mBaseCornerRectangle.X - 24, mBaseCornerRectangle.Y + 24)
            });

            // Trees to set boundaries of map
            for (var i = 0; i < 80; i++)
            {
                mDataManager.mUnits.Add(new Tree2(mGameLoop)
                { Position = new Rectangle(0 + i * 60, 0, 59, 72), IsColliding = true, IsInteracting = false });
                mDataManager.mUnits.Add(new Tree2(mGameLoop)
                { Position = new Rectangle(0 + i * 60, 4728, 59, 72), IsColliding = true, IsInteracting = false });
            }
            for (var i = 0; i < 73; i++)
            {
                mDataManager.mUnits.Add(new Tree2(mGameLoop)
                { Position = new Rectangle(0, 64 + i * 64, 59, 72), IsColliding = true, IsInteracting = false });
                mDataManager.mUnits.Add(new Tree2(mGameLoop)
                { Position = new Rectangle(4741, 64 + i * 64, 59, 72), IsColliding = true, IsInteracting = false });
            }




            // Main forest regions
            for (var i = 0; i < 200; i++)
            {
                // Forest 1: 60 <= x <= 1000, 72 <= y <= 700
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(60, 1000), r.Next(72, 700), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(60, 1000), r.Next(72, 700), 15, 16) });
                // Forest 2: 2600 <= x <= 4700, 3700 <= 4720
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2600, 3700), r.Next(3700, 4720), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(2600, 3700), r.Next(3700, 4720), 15, 16) });
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(3701, 4700), r.Next(3700, 4720), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(3701, 4700), r.Next(3700, 4720), 15, 16) });
                // Forest 3: 3000 <= x <= 4000, 700 <= y <= 1300
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(3000, 4000), r.Next(700, 1300), 59, 72) });
                // Forest 4: 80 <= x <= 400, 3000 <= y <= 4700
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(80, 400), r.Next(3000, 4700), 59, 72) });
                // Forest 5: 1050 <= x <= 1700, 2000 <= y <= 2800
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(1050, 1700), r.Next(2000, 2800), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(1050, 1700), r.Next(2000, 2800), 15, 16) });
            }

            // smoothing edges of forests
            for (var i = 0; i < 30; i++)
            {
                // Forest 1
                // right edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(1000, 1100), r.Next(150, 600), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(1000, 1100), r.Next(150, 600), 15, 16) });
                // lower edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(130, 850), r.Next(700, 850), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(130, 850), r.Next(700, 850), 15, 16) });

                // Forest 2
                // upper edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2850, 4550), r.Next(3600, 3700), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(2850, 4550), r.Next(3600, 3700), 15, 16) });
                // left edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2350, 2600), r.Next(3900, 4500), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(2350, 2600), r.Next(3900, 4500), 15, 16) });

                // Forest 3
                // upper edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(3150, 3850), r.Next(550, 700), 59, 72) });
                // lower edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(3150, 3850), r.Next(1300, 1450), 59, 72) });
                // right edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(4000, 4150), r.Next(800, 1200), 59, 72) });
                // left edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(2850, 3000), r.Next(800, 1200), 59, 72) });

                // Forest 4
                // upper edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(80, 250), r.Next(2650, 3000), 59, 72) });
                // right edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(400, 500), r.Next(3300, 4450), 59, 72) });

                // Forest 5
                // upper edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(1800, 2000), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(1800, 2000), 15, 16) });
                // lower edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(2800, 3000), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1600), r.Next(2800, 3000), 15, 16) });
                // left edge
                mDataManager.mUnits.Add(new Tree1(mGameLoop)
                { Position = new Rectangle(r.Next(900, 1050), r.Next(2150, 2650), 59, 72) });
                mDataManager.mUnits.Add(new BabyTree1(mGameLoop)
                { Position = new Rectangle(r.Next(900, 1050), r.Next(2150, 2650), 15, 16) });
            }

            // Grass at food, metall and stone spreads
            for (var i = 0; i < 500; i++)
            {
                // food spread 1
                mDataManager.mEnvironment.Add(new Grass1(mGameLoop)
                { Position = new Rectangle(r.Next(1150, 2250), r.Next(100, 950), 15, 15) });

                // food spread 2
                mDataManager.mEnvironment.Add(new Grass1(mGameLoop)
                { Position = new Rectangle(r.Next(3700, 4600), r.Next(1000, 1800), 15, 15) });

                // food spread 3
                mDataManager.mEnvironment.Add(new Grass1(mGameLoop)
                { Position = new Rectangle(r.Next(2000, 3000), r.Next(3200, 4500), 15, 15) });

                // food spread 4
                mDataManager.mEnvironment.Add(new Grass1(mGameLoop)
                { Position = new Rectangle(r.Next(80, 1100), r.Next(2500, 3500), 15, 15) });

                // stone spread 2, 3 / metall spread 2
                mDataManager.mEnvironment.Add(new Grass1(mGameLoop)
                { Position = new Rectangle(r.Next(500, 1950), r.Next(3500, 4700), 15, 15) });

                // stone spread 4 / metall spread 3
                mDataManager.mEnvironment.Add(new Grass1(mGameLoop)
                { Position = new Rectangle(r.Next(3800, 4740), r.Next(1850, 3050), 15, 15) });

                // stone spread 5 / metall spread 4
                mDataManager.mEnvironment.Add(new Grass1(mGameLoop)
                { Position = new Rectangle(r.Next(3800, 4740), r.Next(80, 850), 15, 15) });
            }

            // Food: in forests and spreads with grass over the map
            for (var i = 0; i < 50; i++)
            {
                // Forest 1:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(60, 1200), r.Next(72, 900), 32, 32) });

                // Forest 2:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(2450, 3700), r.Next(3700, 4720), 32, 32) });
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(3701, 4700), r.Next(3700, 4720), 32, 32) });

                // Forest 3:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(2800, 4200), r.Next(600, 1400), 32, 32) });

                // Forest 4:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(80, 550), r.Next(2800, 4700), 32, 32) });

                // Forest 5:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(900, 1650), r.Next(1800, 3000), 32, 32) });

                // Food spread 1:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(1200, 2200), r.Next(150, 900), 32, 32) });

                // Food spread 2:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(3700, 4600), r.Next(1000, 1800), 32, 32) });

                // Food spread 3:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(2000, 3000), r.Next(3200, 4500), 32, 32) });

                // Food spread 4:
                mDataManager.mUnits.Add(new Mushroom2(mGameLoop)
                { Position = new Rectangle(r.Next(80, 1100), r.Next(2500, 3500), 32, 32) });
            }

            // Metall and stone
            for (var i = 0; i < 50; i++)
            {
                // Stone spread 1: 2400 <= x <= 2600, 150 <= y <= 350
                mDataManager.mUnits.Add(new Stone1(mGameLoop)
                { Position = new Rectangle(r.Next(2450, 2650), r.Next(150, 450), 46, 62) });
                // Stone spread 2: 1550 <= x <= 1750, 3900 <= y <= 4100
                mDataManager.mUnits.Add(new Stone1(mGameLoop)
                { Position = new Rectangle(r.Next(1500, 1800), r.Next(3850, 4150), 46, 62) });
                // Stone spread 3: 1000 <= 1200, 4400 <= y <= 4600
                mDataManager.mUnits.Add(new Stone1(mGameLoop)
                { Position = new Rectangle(r.Next(950, 1250), r.Next(4350, 4650), 46, 62) });
                // Stone spread 4: 4150 <= x <= 4650, 2200 <= y <=  2700
                mDataManager.mUnits.Add(new Stone1(mGameLoop)
                { Position = new Rectangle(r.Next(4150, 4650), r.Next(2200, 2700), 46, 62) });
                // Stone spread 5: 4250 <= x <= 4720, 80 <= y <= 580
                mDataManager.mUnits.Add(new Stone1(mGameLoop)
                { Position = new Rectangle(r.Next(4250, 4720), r.Next(80, 580), 46, 62) });

                // Metall spread 1: 500 <= x <= 700, 1100 <= y <= 1300
                mDataManager.mUnits.Add(new Metal1(mGameLoop)
                { Position = new Rectangle(r.Next(450, 750), r.Next(1050, 1350), 53, 61) });
                // Metall spread 2: 1100 <= x <= 1400, 4200 <= y <= 4500
                mDataManager.mUnits.Add(new Metal1(mGameLoop)
                { Position = new Rectangle(r.Next(1100, 1400), r.Next(4200, 4500), 53, 61) });
                // Metall spread 3: 4150 <= x <= 4650, 2200 <= y <= 2700
                mDataManager.mUnits.Add(new Metal1(mGameLoop)
                { Position = new Rectangle(r.Next(4150, 4650), r.Next(2200, 2700), 53, 61) });
                // Metall spread 4: 4250 <= x <= 4720, 80 <= y <= 580
                mDataManager.mUnits.Add(new Metal1(mGameLoop)
                { Position = new Rectangle(r.Next(4250, 4720), r.Next(80, 580), 53, 61) });
            }


        }
    }
}
