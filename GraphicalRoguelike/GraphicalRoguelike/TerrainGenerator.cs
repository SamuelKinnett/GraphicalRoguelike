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
            int[,] heightMap = new int[worldWidth, worldHeight]; //this array stores the height of each chunk. 0 = bedrock layer, 1 - 8 = ocean, 9 - 31 = land
            int[,] temporaryMap = new int[worldWidth, worldHeight]; //this array is used in the generation process to create the layer before it is applied to the heightmap

            int neighbourCount = 0; //this is used to store the number of 'alive' tiles surrounding a cell;

            Random rand = new Random();

            //fileManager.CreateWorldFolder(worldName);
            double landChance = 0.70;

            //fill the heightmap with bedrock (lowest layer)
            Array.Clear(heightMap, 0, heightMap.Length);

            for (int currentHeightLevel = 1; currentHeightLevel < 32; currentHeightLevel++) //for each height level
            {
                //clear the arrays of their previous contents
                Array.Clear(temporaryMap, 0, temporaryMap.Length);

                //firstly, fill most of the available area with random values (0 = empty space, 1 = filled).
                for (int y = 2; y < worldHeight - 2; y++)
                {
                    for (int x = 2; x < worldWidth - 2; x++)
                    {
                        if (rand.NextDouble() < landChance && heightMap[x ,y] == currentHeightLevel - 1)
                        {
                            temporaryMap[x, y] = 1;
                        }
                    }
                }

                //then, simulate a 4-5 rule cellular automata 7 times
                //for the first 4 times, also fill in areas with 0 neighbours

                for (int generation = 0; generation < 5; generation++)
                {
                    for (int y = 0; y < worldHeight; y++)
                    {
                        for (int x = 0; x < worldWidth; x++)
                        {

                            neighbourCount = GetNeighbourCount(x, y, worldWidth, worldHeight, temporaryMap);

                            if (neighbourCount >= 5 || neighbourCount == 0)
                            {
                                temporaryMap[x, y] = 1;
                            }
                            else
                            {
                                temporaryMap[x, y] = 0;
                            }

                        }
                    }
                }

                //for the last three times, only use the 4-5 rule

                for (int generation = 0; generation < 4; generation++)
                {
                    for (int y = 0; y < worldHeight; y++)
                    {
                        for (int x = 0; x < worldWidth; x++)
                        {

                            neighbourCount = GetNeighbourCount(x, y, worldWidth, worldHeight, temporaryMap);

                            if (neighbourCount >= 5)
                            {
                                temporaryMap[x, y] = 1;
                            }
                            else
                            {
                                temporaryMap[x, y] = 0;
                            }

                        }
                    }
                }

                landChance -= 0.01; //make the next level more sparse

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

        private int GetNeighbourCount(int x, int y, int worldWidth, int worldHeight, int[,] temporaryMap)
        {
            int neighbourCount = 0;

            if (y != 0 && y != (worldHeight - 1) && x != 0 && x != (worldWidth - 1)) //normal comparisons of non-border tiles
            {
                if (temporaryMap[x, y] == 1)
                {
                    neighbourCount++;
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
            else if (y == 0)
            {
                if (x == 0) //top left corner
                {
                    if (temporaryMap[x, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x + 1, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x + 1, y + 1] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x, y + 1] == 1)
                    {
                        neighbourCount++;
                    }
                }
                else if (x == worldWidth - 1) //top right corner
                {
                    if (temporaryMap[x, y] == 1)
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
                else //anywhere along the top border
                {
                    if (temporaryMap[x, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x + 1, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x + 1, y + 1] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x, y + 1] == 1)
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
                }
            }
            else if (x == 0)
            {
                if (y == worldHeight - 1) //bottom left corner
                {
                    if (temporaryMap[x, y] == 1)
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
                }
                else //anywhere along the left border.
                {
                    if (temporaryMap[x, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x, y - 1] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x, y + 1] == 1)
                    {
                        neighbourCount++;
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
                }
            }
            else if (y == worldHeight - 1)
            {
                if (x == worldWidth - 1) //bottom right corner
                {
                    if (temporaryMap[x, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x - 1, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x - 1, y - 1] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x, y - 1] == 1)
                    {
                        neighbourCount++;
                    }
                }
                else //anywhere along the bottom border
                {
                    if (temporaryMap[x, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x, y - 1] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x - 1, y] == 1)
                    {
                        neighbourCount++;
                    }
                    if (temporaryMap[x - 1, y - 1] == 1)
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
                }
            }
            else if (x == worldWidth - 1) //anywhere along the right border
            {
                if (temporaryMap[x, y] == 1)
                {
                    neighbourCount++;
                }
                if (temporaryMap[x, y + 1] == 1)
                {
                    neighbourCount++;
                }
                if (temporaryMap[x, y - 1] == 1)
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
                if (temporaryMap[x - 1, y - 1] == 1)
                {
                    neighbourCount++;
                }
            }

            return neighbourCount;
        }

    }
}
