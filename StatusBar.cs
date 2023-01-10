using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Security.Policy;

namespace SpaceImmigrants
{
    public class StatusBar
    {
        public static Vector2 HeartSize = new(40, 40);
        public static Vector2 HeartPosition = new(20, 20);
        public static Vector2 HeartPadding = new(30, 0);
        public int Health = 3;

        private Texture2D _heartSprite;

        public StatusBar(Game1 currentGame)
        {
            this._heartSprite = currentGame.Content.Load<Texture2D>("heart");

            currentGame.DrawQueue.Add(this, (double step) => {
                for (int i = 0; i < this.Health; i++)
                {
                    Vector2 heartPosition = HeartPosition + HeartPadding * i;
                    Rectangle destination = new Rectangle(
                        (int)heartPosition.X,
                        (int)heartPosition.Y,
                        (int)HeartSize.X,
                        (int)HeartSize.Y
                    );

                    Game1.SpriteBatch.Draw(
                        texture: this._heartSprite,
                        destinationRectangle: destination,
                        color: Color.White
                    );
                }
            });
        }
    }
}