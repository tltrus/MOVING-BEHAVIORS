using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace DrawingVisualApp
{
    /// <summary>
    /// Based on : "Coding Challenge #69: Evolutionary Steering Behaviors"
    /// https://thecodingtrain.com/CodingChallenges/069.1-steering-evolution.html
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        public static Random rnd = new Random();
        public static int width, height;

        DrawingVisual visual;
        DrawingContext dc;
        List<Vehicle> cars = new List<Vehicle>();
        Vector2D mouse = new Vector2D();
        public static List<Particle> particles = new List<Particle>();



        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            for (int i = 0; i < 10; ++i)
                cars.Add(new Vehicle());

            for (int i = 0; i < 10; ++i)
            {
                var pos = new Vector2D(rnd.Next(width), rnd.Next(height));
                particles.Add(new Particle(pos, ParticleType.Food));
            }

            for (int i = 0; i < 4; ++i)
            {
                var pos = new Vector2D(rnd.Next(width), rnd.Next(height));
                particles.Add(new Particle(pos, ParticleType.Poison));
            }

            timer.Start();
        }

        private void timerTick(object sender, EventArgs e) => Drawing();

        private void Drawing()
        {
            if (rnd.NextDouble() < 0.1)
            {
                var pos = new Vector2D(rnd.Next(width), rnd.Next(height));
                particles.Add(new Particle(pos, ParticleType.Food));
            }

            if (rnd.NextDouble() < 0.01)
            {
                var pos = new Vector2D(rnd.Next(width), rnd.Next(height));
                particles.Add(new Particle(pos, ParticleType.Poison));
            }


            g.RemoveVisual(visual);

            using (dc = visual.RenderOpen())
            {
                foreach (var p in particles)
                {
                    p.Draw(dc);
                }

                foreach (var c in cars.ToList())
                {
                    if (c.IsDead())
                    {
                        var pos = c.GetPos();
                        cars.RemoveAt(cars.IndexOf(c));
                        particles.Add(new Particle(pos, ParticleType.Food));
                        continue;
                    }
                    c.Behaviors();
                    c.Update();
                    c.Draw(dc);

                    var newVehiclePos = c.Clone();
                    if (newVehiclePos is null)
                    {
                        // trick
                    }
                    else
                    {
                        cars.Add(new Vehicle(newVehiclePos));
                    }
                }

                DrawText(dc, "red is poison", Brushes.Red, new Point(20, 20));
                DrawText(dc, "green is food", Brushes.Green, new Point(20, 40));

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void g_MouseUp(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < cars.Count; i++)
            {
                cars[i].GoAway();
            }
        }

        private void g_MouseMove(object sender, MouseEventArgs e)
        {
            mouse.X = e.GetPosition(g).X;
            mouse.Y = e.GetPosition(g).Y;
        }

        private void DrawText(DrawingContext dc, string text, Brush brush, Point pos)
        {
            // Draw labeling
            FormattedText formattedText = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"),
                                                            FlowDirection.LeftToRight, new Typeface("Verdana"), 11, brush,
                                                            VisualTreeHelper.GetDpi(visual).PixelsPerDip);
            dc.DrawText(formattedText, pos);
        }
    }
}
