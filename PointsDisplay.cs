using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceImmigrants
{
	public class PointsDisplay
	{
		public PointsDisplay(Game1 currentGame)
		{
			SpriteFont pointsDisplayFont = currentGame.Content.Load<SpriteFont>("PointsDisplay");

			currentGame.DrawQueue.Add(this, (double step) => {
				Game1.SpriteBatch.DrawString(
					pointsDisplayFont,
					$"Points: {currentGame.Points}",
					new Vector2(10, 60),
					Color.White
				);
			});

			Event.GameEnded.Invoked += (ended) => {
				currentGame.DrawQueue.Remove(this);
			};
		}
	}
}