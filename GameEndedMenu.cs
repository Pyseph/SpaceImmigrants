using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Security.Policy;

namespace SpaceImmigrants
{

    class GameEndedMenu
    {
        private SpriteFont _fontSprite;

        public GameEndedMenu(Game1 currentGame)
        {
            this._fontSprite = currentGame.Content.Load<SpriteFont>("GameEndedFont");

            Event.GameEnded.Invoked += (ended) => {
                currentGame.DrawQueue.Add(this, (double step) => {
                    // draw a Game Over message on the center of the screen
                    string message = $"Game Over\nFinal Points: {currentGame.Points}";
                    Vector2 messageSize = this._fontSprite.MeasureString(message);
                    Game1.SpriteBatch.DrawString(
                        this._fontSprite,
                        message,
                        new Vector2(
                            Game1.ViewportSize.X / 2 - (messageSize.X / 2),
                            Game1.ViewportSize.Y / 2 - (messageSize.Y / 2)
                        ),
                        Color.White
                    );
                });
            };
        }
    }
}