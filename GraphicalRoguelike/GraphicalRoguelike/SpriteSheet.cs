using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GraphicalRoguelike
{
    /// <summary>
    /// This class adds support for spritesheets. It takes in the number of rows and columns and then
    /// assigns each sprite in the sheet a number starting from 0, going left to right and top to bottom.
    /// </summary>
    public class SpriteSheet
    {
        //instance variables
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public SpriteSheet(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, int textureNumber, Color color, float scale)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            //this counts through the texture number to work out the co-ordinates of the texture.
            //it turns a number e.g. 4 into an X and Y value and uses these to draw the tile.
            int temp = textureNumber;
            int posX = 0;
            int posY = 0;
            while (temp - Columns > 0)
            {
                posY++;
                temp -= Columns;
            }
            posX = textureNumber % Columns;
            
            //Convert the tile positions into co-ordinates
            posX = posX * width;
            posY = posY * height;

            //Create one rectangle to extract the tile and one to render it.
            Rectangle sourceRectangle = new Rectangle(posX, posY, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            //Draw the tile to the screen
            spriteBatch.Begin();
            if (scale == 1)
            {
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, color);
            }
            else
            {
                spriteBatch.Draw(Texture, location, sourceRectangle, color, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
                spriteBatch.Draw(Texture, location, sourceRectangle, color, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }
    }
}
