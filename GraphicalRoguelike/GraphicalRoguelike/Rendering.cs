using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GraphicalRoguelike
{
    /// <summary>
    /// This class handles algorithms for rendering various things to the screen, e.g. map rendering, to
    /// ensure the Draw() method remains clear.
    /// </summary>
    class Rendering
    {
        //quick renderer for testing purposes
        public void RenderWorld(SpriteBatch spriteBatch, SpriteSheet spriteSheet, int worldWidth, int worldHeight, int[,] worldMap)
        {
            Color color;

            for (int y = 0; y < worldHeight; y++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    switch (worldMap[x, y])
                    {
                        case 0:
                            color = new Color(0, 0, 140);
                            break;
                        case 1:
                            color = new Color(0, 0, 180);
                            break;
                        case 2:
                            color = new Color(0, 0, 220);
                            break;
                        default:
                            color = new Color(0, (worldMap[x, y]) * 10, 0);
                            break;
                    }

                    spriteSheet.Draw(spriteBatch, new Vector2((x * 8) + 20, (y * 8) + 20), 38, color, (float)0.5);
                }
            }
        }

    }
}
