using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicalRoguelike
{
    class TerrainGenerator
    {
        FileManager fileManager = new FileManager();

        /// <summary>
        /// This method repeatedly uses cellular automata with decreasing probability to
        /// generate land in order to build the terrain.
        /// </summary>
        public void GenerateWorld(int worldWidth, int worldHeight, string worldName)
        {
            fileManager.CreateWorldFolder(worldName);
            double landChance = 0.9;

        }

        private int[,] GenerateChunk(int chunkSize)
        {
            int[,] chunk = new int[chunkSize, chunkSize];

            return chunk;
        }

    }
}
