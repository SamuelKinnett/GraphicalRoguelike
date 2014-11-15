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
        public int[,] GenerateWorld(int worldWidth, int worldHeight, string worldName)
        {
            int[,] heightMap = new int[worldWidth, worldHeight]; //this array stores the height of each chunk. 0 = bedrock layer, 1 - 3 = ocean, 4 - 11 = hills/land, 12 - 15 = mountains
            int[,] temporaryMap = new int[worldWidth, worldHeight]; //this array is used in the generation process to create the layer before it is applied to the heightmap
            int[,] cellularMap = new int[worldWidth, worldHeight]; //this array is used in the cellular automata stage of generation;

            int neighbourCount = 0; //this is used to store the number of 'alive' tiles surrounding a cell;

            Random rand = new Random();

            //fileManager.CreateWorldFolder(worldName);
            double landChance = 0.8;

            //fill the heightmap with bedrock (lowest layer)
            for (int y = 0; y < worldHeight; y++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    heightMap[x, y] = 0;
                }
            }

            for (int currentHeightLevel = 1; currentHeightLevel < 15; currentHeightLevel++) //for each height level
            {
                //clear the arrays of their previous contents
                Array.Clear(temporaryMap, 0, temporaryMap.Length);

                //firstly, fill the available area with random values (0 = empty space, 1 = filled).
                for (int y = 0; y < worldHeight; y++)
                {
                    for (int x = 0; x < worldWidth; x++)
                    {
                        if (rand.NextDouble() < landChance && heightMap[x ,y] == currentHeightLevel - 1)
                        {
                            temporaryMap[x, y] = 1;
                        }
                    }
                }

                //then, simulate a 4-5 rule cellular automata 5 times.

                for (int generation = 0; generation < 4; generation++)
                {
                    for (int y = 0; y < worldHeight; y++)
                    {
                        for (int x = 0; x < worldWidth; x++)
                        {

                            neighbourCount = 0;

                            if (y != 0 && y != (worldHeight - 1) && x != 0 && x != (worldWidth - 1)) //stops comparisons being performed on the borders
                            {
                                if (temporaryMap[x, y] == 1)
                                {
                                    neighbourCount ++;
                                }
                                if (temporaryMap[x + 1, y + 1] == 1)
                                {
                                    neighbourCount++;
                                }
                                if (temporaryMap[x + 1, y] == 1)
                                {
                                    neighbourCount++;
                                }
                                if (temporaryMap[x + 1, y - 1] == 1)
                                {
                                    neighbourCount++;
                                }
                                if (temporaryMap[x, y - 1] == 1)
                                {
                                    neighbourCount++;
                                }
                                if (temporaryMap[x - 1, y - 1] == 1)
                                {
                                    neighbourCount++;
                                }
                                if (temporaryMap[x - 1, y] == 1)
                                {
                                    neighbourCount++;
                                }
                                if (temporaryMap[x - 1, y + 1] == 1)
                                {
                                    neighbourCount++;
                                }
                                if (temporaryMap[x, y + 1] == 1)
                                {
                                    neighbourCount++;
                                }
                            }

                            if (neighbourCount >= 5)
                            {
                                cellularMap[x, y] = 1;
                            }

                        }
                    }

                    Array.Copy(cellularMap, temporaryMap, cellularMap.Length);
                    Array.Clear(cellularMap, 0, cellularMap.Length);

                }

                landChance -= 0.05; //make the next level more sparse

                //apply the layer to the heightmap

                for (int y = 0; y < worldHeight; y++)
                {
                    for (int x = 0; x < worldWidth; x++)
                    {
                        if (temporaryMap[x, y] == 1)
                        {
                            heightMap[x, y] = currentHeightLevel;
                        }
                    }
                }

            }

            //Next, call GenerateChunk for each heightmap square.


            return heightMap; //for testing purposes.
        }

        private int[,] GenerateChunk(int chunkSize, int[] heights)
        {
            int[,] chunk = new int[chunkSize, chunkSize];

            return chunk;
        }

    }
}
