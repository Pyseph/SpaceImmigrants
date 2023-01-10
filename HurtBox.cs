using Microsoft.Xna.Framework;

namespace SpaceImmigrants
{
    public class HurtBox
    {
        public Vector2 Position;
        public Vector2 SpriteSize;

        public HurtBox(Enemy obj)
        {
            this.Position = obj.Position;
            this.SpriteSize = obj.SpriteSize;

            obj.PositionChanged += (sender, e) => {
                this.Position = obj.Position;
            };
        }
    }
}