using Microsoft.Xna.Framework;

namespace SpaceImmigrants
{
    public class HurtBox
    {
        public Vector2 Position;
        public Vector2 SpriteSize;
        public Rectangle Rect;
        public Enemy Parent;

        public HurtBox(Enemy obj)
        {
            this.Position = obj.Position;
            this.SpriteSize = obj.SpriteSize;
            this.Parent = obj;

            obj.PositionChanged += (sender, e) => {
                this.Position = obj.Position;
                this.Rect = new Rectangle(
                    (int)this.Position.X,
                    (int)this.Position.Y,
                    (int)this.SpriteSize.X,
                    (int)this.SpriteSize.Y
                );
            };
        }
    }
}