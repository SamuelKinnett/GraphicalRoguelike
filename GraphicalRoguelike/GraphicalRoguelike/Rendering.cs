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
        public void RenderWorld(SpriteBatch spriteBatch, Texture2D mapSquare, int worldWidth, int worldHeight, int[,] worldMap)
        {
            Color color;

            for (int y = 0; y < worldHeight; y++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    int temp = worldMap[x, y];
                    color = Color.Black;

                    if (temp < 9)
                    {
                        color = new Color(temp * 2, temp * 2, temp * 20 + 110);
                    }
                    else
                    {
                        color = new Color(0, temp * 8, 0);
                    }

                    //spriteSheet.Draw(spriteBatch, new Vector2((x * 4) + 20, (y * 4) + 20), 38, color, (float)0.25);
                    spriteBatch.Begin();
                    spriteBatch.Draw(mapSquare, new Rectangle((x * 4) + 20, (y * 4) + 20, 4, 4), color);
                    spriteBatch.End();
                }
            }
        }

        public void RenderMenu(SpriteBatch spriteBatch, SpriteSheet menuShip)
        {

        }

    }
}
