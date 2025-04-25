using System.Windows;
using System.Windows.Media;


namespace DrawingVisualApp
{
    public enum ParticleType
    {
        Food = 0,
        Poison = 1
    }

    public class Particle
    {
        public Vector2D pos;
        public ParticleType type;

        public Particle(Vector2D pos, ParticleType type)
        {
            this.type = type;
            this.pos = pos;
        }

        public Point GetPoint() => new Point(pos.X, pos.Y);


        public void Draw(DrawingContext dc)
        {
            switch (type)
            {
                case ParticleType.Food: 
                    dc.DrawEllipse(Brushes.Green, null, GetPoint(), 2, 2);
                    break;
                case ParticleType.Poison:
                    dc.DrawEllipse(Brushes.Red, null, GetPoint(), 2, 2);
                    break;
            }
        }
    }
}
