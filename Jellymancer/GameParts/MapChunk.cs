using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellymancer.GameParts
{
    /// <summary>
    /// Basic part of the world map
    /// </summary>
    class MapChunk
    {
        private MapTile floor, wall, nothing;

        private Dictionary<string, Texture2D> monsterSprites = new Dictionary<string, Texture2D>();

        public Random random;

        /// <summary>
        /// Create new map chunk
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="content"></param>
        public MapChunk(int width, int height, ContentManager content)
        {
            // I don't feel content should be in this....
            map = new MapTile[width, height];

            // Archeotypes
            floor = new MapTile(content.Load<Texture2D>("Game/Tiles/Floor"), true); 
            wall = new MapTile(content.Load<Texture2D>("Game/Tiles/Wall"), false); 
            nothing = new MapTile(content.Load<Texture2D>("Game/Tiles/Empty"), false); 

            // Fill Inside with Floor
            for (var ix = 1; ix < width - 1; ++ix)
            {
                for (var iy = 1; iy < height - 1; ++iy)
                {
                    map[ix, iy] = floor;
                }
            }

            // Fill Outside with Wall
            for (var ix = 0; ix < width; ++ix)
            {
                map[ix, 0] = wall;
                map[ix, height - 1] = wall;
            }
            for (var iy = 0; iy < height; ++iy)
            {
                map[0, iy] = wall;
                map[width - 1, iy] = wall;
            }

            random = new Random();

            Width = width;
            Height = height;

            monsterSprites["meat1"] = content.Load<Texture2D>("Game/Sprites/Meat1");
            monsterSprites["meat2"] = content.Load<Texture2D>("Game/Sprites/Meat2");
            monsterSprites["meat3"] = content.Load<Texture2D>("Game/Sprites/Meat3");
            monsterSprites["rat"] = content.Load<Texture2D>("Game/Sprites/Rat");
            monsterSprites["adventurer5"] = content.Load<Texture2D>("Game/Sprites/Adventurer5"); 
            monsterSprites["adventurer3"] = content.Load<Texture2D>("Game/Sprites/Adventurer3"); 
            monsterSprites["adventurer4"] = content.Load<Texture2D>("Game/Sprites/Adventurer4");
            monsterSprites["Jelly1"] = content.Load<Texture2D>("Game/Sprites/Jelly1");
            monsterSprites["Jelly2"] = content.Load<Texture2D>("Game/Sprites/Jelly2");
            monsterSprites["Jelly3"] = content.Load<Texture2D>("Game/Sprites/Jelly3");
            monsterSprites["Jelly4"] = content.Load<Texture2D>("Game/Sprites/Jelly4");

            GenerateDungeon();
        }

        public MapTile[,] map;

        /// <summary>
        /// Width of map
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Height of map
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Players/Monsters on the map
        /// </summary>
        protected List<Actor> actors = new List<Actor>();

        /// <summary>
        /// Actors on the map that other objects can use
        /// </summary>
        public ReadOnlyCollection<Actor> Actors
        {
            get
            {
                return actors.AsReadOnly();
            }
        }

        /// <summary>
        /// Add actor to the list of actors, and change its 'MapChunk' to the current map
        /// </summary>
        /// <param name="actor"></param>
        public void AddActor(Actor actor)
        {
            // Add actor
            actors.Add(actor);
            // Add extra bits of actor
            foreach (var i in actor.characterParts)
            {
                AddActor(i);
            }
            // Set the actor to this map
            actor.currentMap = this;
        }

        /// <summary>
        /// Kill all actors marked dead
        /// </summary>
        public void KillDeadActors()
        {
            actors.RemoveAll(i => i.dead);
        }

        public Karcero.Engine.Models.Map<Karcero.Engine.Models.Cell> generatedMap;

        /// <summary>
        /// Turn this into a fun lovin' dungeon
        /// </summary>
        public void GenerateDungeon()
        {
            var generator = new Karcero.Engine.DungeonGenerator<Karcero.Engine.Models.Cell>();

            generatedMap = generator.GenerateA()
                     .DungeonOfSize(Width, Height)
                     .ABitRandom()
                     .SomewhatSparse()
                     .WithMediumChanceToRemoveDeadEnds()
                     .WithMediumSizeRooms()
                     .WithLargeNumberOfRooms()
                     .Now();

            for (var ix = 0; ix < (generatedMap.Width - 1); ++ix)
            {
                for (var iy = 0; iy < (generatedMap.Height - 1); ++iy)
                {
                    // Row, Column . Urgh.
                    var c = generatedMap.GetCell(iy, ix);
                    switch (c.Terrain)
                    {
                        case Karcero.Engine.Models.TerrainType.Rock:
                            map[ix, iy] = wall;
                            break;
                        case Karcero.Engine.Models.TerrainType.Floor:
                        case Karcero.Engine.Models.TerrainType.Door:
                            map[ix, iy] = floor;
                            break;
                    }
                }
            }

            // Make corrodors 2 or 3 wide usually
            for (var ix = 2; ix < (generatedMap.Width - 3); ++ix)
            {
                for (var iy = 2; iy < (generatedMap.Height - 3); ++iy)
                {
                    if (map[ix, iy].walkable && (!map[ix - 1, iy].walkable && !map[ix + 1, iy].walkable && map[ix, iy - 1].walkable && map[ix, iy + 1].walkable))
                    {
                        map[ix - 1, iy] = floor;
                        map[ix + 1, iy] = floor;
                    }

                    if (map[ix, iy].walkable && (!map[ix, iy - 1].walkable && !map[ix, iy + 1].walkable && map[ix - 1, iy].walkable && map[ix + 1, iy].walkable))
                    {
                        map[ix, iy - 1] = floor;
                        map[ix, iy + 1] = floor;
                    }
                }
            }

            // And make surrounded tiles empty
            for (var ix = 1; ix < (generatedMap.Width - 2); ++ix)
            {
                for (var iy = 1; iy < (generatedMap.Height - 2); ++iy)
                {
                    if (!map[ix, iy].walkable &&
                        !map[ix - 1, iy].walkable &&
                        !map[ix + 1, iy].walkable &&
                        !map[ix, iy - 1].walkable &&
                        !map[ix, iy + 1].walkable &&
                        !map[ix + 1, iy + 1].walkable &&
                        !map[ix - 1, iy + 1].walkable &&
                        !map[ix + 1, iy - 1].walkable &&
                        !map[ix + 1, iy - 1].walkable &&
                        !map[ix - 1, iy - 1].walkable)
                    {
                        map[ix, iy] = nothing;
                    }
                }
            }

            // Fill Outside with Wall
            for (var ix = 0; ix < Width; ++ix)
            {
                map[ix, 0] = wall;
                map[ix, Height - 1] = wall;
            }
            for (var iy = 0; iy < Height; ++iy)
            {
                map[0, iy] = wall;
                map[Width - 1, iy] = wall;
            }

            var startRoom = generatedMap.Rooms.OrderBy(i => i.Row).First();

            startX = (startRoom.Column + startRoom.Right) / 2;
            startY = (startRoom.Row + startRoom.Bottom) / 2;

            // Add Food
            for (var i = 0; i < (Width * Height) / 50; ++i)
            {
                var ix = 0;
                var iy = 0;
                while (!map[ix, iy].walkable)
                {
                    ix = random.Next(0, Width);
                    iy = random.Next(0, Height);
                }
                var key = $"meat{random.Next(1, 4)}";
                AddActor(new Food(monsterSprites[key], ix, iy));
            }

            // Add Critters
            for (var i = 0; i < (Width * Height) / 50; ++i)
            {
                var ix = 0;
                var iy = 0;
                while (!map[ix, iy].walkable)
                {
                    ix = random.Next(0, Width);
                    iy = random.Next(0, Height);
                }
                AddActor(new Critter(monsterSprites["rat"], ix, iy, random));
            }

            // Add Monsters
            for (var i = 0; i < (Width * Height) / 200; ++i)
            {
                var ix = 0;
                var iy = 0;
                while (!map[ix, iy].walkable)
                {
                    ix = random.Next(0, Width);
                    iy = random.Next(0, Height);
                }
                switch (random.Next(0, 3))
                {
                    case 0:
                        AddActor(new BasicEnemy(monsterSprites["adventurer3"], ix, iy));
                        break;
                    case 1:
                        AddActor(new BasicEnemy(monsterSprites["adventurer4"], ix, iy));
                        break;
                    case 2:
                        AddActor(new BasicEnemy(monsterSprites["adventurer5"], ix, iy));
                        break;
                }
            }

            // Add some jellys
            for (var i = 0; i < 10; ++i)
            {
                var ix = 0;
                var iy = 0;
                while (!map[ix, iy].walkable)
                {
                    ix = random.Next(0, Width);
                    iy = random.Next(0, Height);
                }
                Texture2D sprite = monsterSprites[$"Jelly{random.Next(1, 5)}"];
                AddActor(new JellyEnemy(sprite, sprite, ix, iy, random.Next(2, 5), random, this));
            }

            UpdatePathGrid();
        }

        public int startX, startY;

        public byte[,] pathGrid;
        public void UpdatePathGrid()
        {
            int width = DeenGames.Utils.AStarPathFinder.PathFinderHelper.RoundToNearestPowerOfTwo(Width);
            int height = DeenGames.Utils.AStarPathFinder.PathFinderHelper.RoundToNearestPowerOfTwo(Height);

            pathGrid = new byte[width, height];

            for (var ix = 1; ix < (generatedMap.Width - 2); ++ix)
            {
                for (var iy = 1; iy < (generatedMap.Height - 2); ++iy)
                {
                    pathGrid[ix, iy] = (byte)(map[ix, iy].walkable ?
                        DeenGames.Utils.AStarPathFinder.PathFinderHelper.EMPTY_TILE :
                        DeenGames.Utils.AStarPathFinder.PathFinderHelper.BLOCKED_TILE);
                }
            }
        }
    }
}

