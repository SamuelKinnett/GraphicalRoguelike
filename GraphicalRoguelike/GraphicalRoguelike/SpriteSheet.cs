﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GraphicalRoguelike
{
    //this class handles all of the terrain that can be rendered.
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

        public void Draw(SpriteBatch spriteBatch, Vector2 location, int textureNumber)
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
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }


    }
}
