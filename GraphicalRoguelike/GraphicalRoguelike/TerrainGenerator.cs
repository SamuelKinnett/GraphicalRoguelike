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
            double landChance = 0.78;

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
                        if (rand.NextDouble() < landChance && heightMap[x, y] == currentHeightLevel - 1)
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
                                //testing: only fill if less than the random number
                                if (rand.NextDouble() < landChance && heightMap[x, y] == currentHeightLevel - 1)
                                {
                                    temporaryMap[x, y] = 1;
                                }
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

        /// <summary>
        /// Like GenerateWorld, only with islands specified by the user.
        /// </summary>
        public int[,] GenerateWorld(int worldWidth, int worldHeight, string worldName, int numberOfIslands, double gradient)
        {
            int[,] heightMap = new int[worldWidth, worldHeight]; //this array stores the height of each chunk. 0 = bedrock layer, 1 - 8 = ocean, 9 - 31 = land
            int[,] temporaryMap = new int[worldWidth, worldHeight]; //this array is used in the generation process to create the layer before it is applied to the heightmap
            double[,] probabilityMap = new double[worldWidth, worldHeight]; //this array stores the probability of a tile being solid
            bool[,] computedTiles = new bool[worldWidth, worldHeight]; //this array stores which tiles have had their probability calculated

            int neighbourCount = 0; //this is used to store the number of 'alive' tiles surrounding a cell

            Random rand = new Random();

            //fileManager.CreateWorldFolder(worldName);
            double landChance = 0.78;

            //fill the heightmap with bedrock (lowest layer)
            Array.Clear(heightMap, 0, heightMap.Length);

            //fill the computedTiles with false
            Array.Clear(computedTiles, 0, computedTiles.Length);

            //fill the probabilityMap with 0;
            Array.Clear(probabilityMap, 0, probabilityMap.Length);

            //randomly choose the 'island' locations
            for (int islandCount = 0; islandCount < numberOfIslands; islandCount++)
            {
                int tempX = rand.Next(8, worldWidth - 8);
                int tempY = rand.Next(8, worldHeight - 8);

                probabilityMap[tempX, tempY] = 0.9;
                computedTiles[tempX, tempY] = true;
            }

            //compute probability map
            bool comparisonCompleted = false;

            while (comparisonCompleted == false)
            {
                comparisonCompleted = true;

                for (int x = 2; x < worldWidth - 2; x++)
                {
                    for (int y = 2; y < worldHeight - 2; y++)
                    {
                        if (computedTiles[x, y] == false)
                        {
                            if (computedTiles[x + 1, y] || computedTiles[x - 1, y] || computedTiles[x, y + 1] || computedTiles[x, y - 1] )
                            {
                                comparisonCompleted = false;
                                probabilityMap[x, y] = GetNeighbourMean(x, y, worldWidth, worldHeight, probabilityMap, computedTiles) - gradient;
                                computedTiles[x, y] = true;
                            }
                        }
                    }
                }
            }

            for (int currentHeightLevel = 1; currentHeightLevel < 32; currentHeightLevel++) //for each height level
            {
                //clear the arrays of their previous contents
                Array.Clear(temporaryMap, 0, temporaryMap.Length);

                //firstly, fill most of the available area with random values (0 = empty space, 1 = filled).
                for (int y = 2; y < worldHeight - 2; y++)
                {
                    for (int x = 2; x < worldWidth - 2; x++)
                    {
                        if (rand.NextDouble() < probabilityMap[x, y] - (currentHeightLevel * 0.01) && heightMap[x, y] == currentHeightLevel - 1)
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
                                //testing: only fill if less than the random number
                                if (rand.NextDouble() < probabilityMap[x, y] && heightMap[x, y] == currentHeightLevel - 1)
                                {
                                    temporaryMap[x, y] = 1;
                                }
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

        /// <summary>
        /// This method returns the number of 'alive' tiles surrounding the tile specified by the x and y parameters.
        /// </summary>
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

        /// <summary>
        /// This method returns the mean average of all values surrounding a tile
        /// </summary>
        /// <param name="x">The X co-ordinate of the tile</param>
        /// <param name="y">The Y co-ordinate of the tile</param>
        /// <param name="worldWidth">The width of the world</param>
        /// <param name="worldHeight">The height of the world</param>
        /// <param name="temporaryMap">The map to perform the comparisons on</param>
        /// <returns></returns>
        private double GetNeighbourMean(int x, int y, int worldWidth, int worldHeight, double[,] temporaryMap, bool[,] computedMap)
        {
            double neighbourTotal = 0;
            double average = 0;
            int numberOfNeighbours = 0;

            if (x != 0 && y != 0 && x != worldWidth - 1 && y != worldHeight - 1) //normal comparison.
            {
                if (computedMap[x + 1, y] == true)
                {
                    neighbourTotal += temporaryMap[x + 1, y];
                    numberOfNeighbours++;
                }
                if (computedMap[x, y + 1] == true)
                {
                    neighbourTotal += temporaryMap[x, y + 1];
                    numberOfNeighbours++;
                }
                if (computedMap[x - 1, y] == true)
                {
                    neighbourTotal += temporaryMap[x - 1, y];
                    numberOfNeighbours++;
                }
                if (computedMap[x, y - 1] == true)
                {
                    neighbourTotal += temporaryMap[x, y - 1];
                    numberOfNeighbours++;
                }
            }
            else if (x == 0)
            {
                if (y == 0) //top left corner
                {
                    if (computedMap[x + 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x + 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y + 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y + 1];
                        numberOfNeighbours++;
                    }
                }
                else if (y == worldHeight - 1) //bottom left corner
                {
                    if (computedMap[x + 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x + 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y - 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y - 1];
                        numberOfNeighbours++;
                    }
                }
                else //anything along left side
                {
                    if (computedMap[x + 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x + 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y + 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y + 1];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y - 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y - 1];
                        numberOfNeighbours++;
                    }
                }
            }
            else if (x == worldWidth - 1)
            {
                if (y == 0) //top right corner
                {
                    if (computedMap[x - 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x - 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y + 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y + 1];
                        numberOfNeighbours++;
                    }
                }
                else if (y == worldHeight - 1) //bottom right corner
                {
                    if (computedMap[x - 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x - 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y - 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y - 1];
                        numberOfNeighbours++;
                    }
                }
                else //anywhere along right side
                {
                    if (computedMap[x - 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x - 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y + 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y + 1];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y - 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y - 1];
                        numberOfNeighbours++;
                    }
                }
            }
            else
            {
                if (y == 0) //anywhere along top side
                {
                    if (computedMap[x - 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x - 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y + 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y + 1];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x + 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x + 1, y];
                        numberOfNeighbours++;
                    }
                }
                else if (y == worldHeight - 1) //anywhere along bottom side
                {
                    if (computedMap[x + 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x + 1, y];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x, y - 1] == true)
                    {
                        neighbourTotal += temporaryMap[x, y - 1];
                        numberOfNeighbours++;
                    }
                    if (computedMap[x - 1, y] == true)
                    {
                        neighbourTotal += temporaryMap[x - 1, y];
                        numberOfNeighbours++;
                    }
                }
            }

            average = neighbourTotal / numberOfNeighbours;
            return average;
        }

    }
}
