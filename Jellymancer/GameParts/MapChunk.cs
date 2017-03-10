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

            Width = width;
            Height = height;

            AddActor(new GameParts.Actor(content.Load<Texture2D>("Game/Sprites/Adventurer1"), 10, 10));
            AddActor(new GameParts.Actor(content.Load<Texture2D>("Game/Sprites/Adventurer2"), 15, 10));
            AddActor(new GameParts.Actor(content.Load<Texture2D>("Game/Sprites/Adventurer3"), 5, 10));
            AddActor(new GameParts.Actor(content.Load<Texture2D>("Game/Sprites/Adventurer4"), 5, 5));
            AddActor(new GameParts.Actor(content.Load<Texture2D>("Game/Sprites/Adventurer5"), 15, 5));

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

        /// <summary>
        /// Turn this into a fun lovin' dungeon
        /// </summary>
        public void GenerateDungeon()
        {
            // Fill with nothing
            for (var ix = 0; ix < this.Width; ++ix)
            {
                for (var iy = 0; iy < this.Height; ++iy)
                {
                    map[ix, iy] = nothing;
                }
            }

            var generator = new Karcero.Engine.DungeonGenerator<Karcero.Engine.Models.Cell>();

            var generatedMap = generator.GenerateA()
                     .MediumDungeon()
                     .ABitRandom()
                     .SomewhatSparse()
                     .WithMediumChanceToRemoveDeadEnds()
                     .WithMediumSizeRooms()
                     .WithLargeNumberOfRooms()
                     .Now();

            for (var ix = 0; ix < generatedMap.Width; ++ix)
            {
                for (var iy = 0; iy < generatedMap.Height ; ++iy)
                {
                    // Row, Column . Urgh.
                    var c = generatedMap.GetCell(iy, ix);
                    switch(c.Terrain)
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

        }

    }
}
