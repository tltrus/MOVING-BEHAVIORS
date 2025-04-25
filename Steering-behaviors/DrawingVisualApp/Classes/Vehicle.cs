using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;


namespace DrawingVisualApp
{
    class Vehicle
    {
        Vector2D p0, p1, p2, p3;
        Vector2D velocity = new Vector2D();
        Vector2D acceleration = new Vector2D();

        Brush brush = Brushes.White;
        int maxspeed = 3;
        double maxforce = 0.6;
        double angle;
        int length = 10;
        double health = 1;


        public Vehicle()
        {
            var x = MainWindow.rnd.Next(MainWindow.width);
            var y = MainWindow.rnd.Next(MainWindow.height);

            p0 = new Vector2D(x, y);
            p1 = new Vector2D(x + length, y);
            p2 = new Vector2D(x, y - 3);
            p3 = new Vector2D(x, y + 3);

            //var R = (byte)MainWindow.rnd.Next(255);
            //var G = (byte)MainWindow.rnd.Next(255);
            //var B = (byte)MainWindow.rnd.Next(255);
            //brush = new SolidColorBrush(Color.FromRgb(R, G, B));
        }

        public Vehicle(Vector2D pos)
        {
            p0 = new Vector2D(pos.X, pos.Y);
            p1 = new Vector2D(pos.X + length, pos.Y);
            p2 = new Vector2D(pos.X, pos.Y - 3);
            p3 = new Vector2D(pos.X, pos.Y + 3);
        }

        public void Update()
        {
            health -= 0.001;

            velocity.Add(acceleration);
            velocity.Limit(maxspeed);
            AddVelocity(velocity);

            acceleration.Mult(0);
        }

        public void Behaviors()
        {
            // Определение цели движения
            Vector2D closest = Eat(MainWindow.particles);

            if (closest is null)
            {
                //
            }
            else
            {
                var angle = Vector2D.Sub(p0, closest);
                Rotate(angle);

                var target = Seek(closest);
                target.Mult(maxforce);

                ApplyForce(target);
            }
        }

        private void ApplyForce(Vector2D force) => acceleration.Add(force);

        private void AddVelocity(Vector2D velocity)
        {
            p0.Add(velocity);
            p1.Add(velocity);
            p2.Add(velocity);
            p3.Add(velocity);
        }

        private void Rotate(Vector2D target)
        {
            Vector2D direction = Vector2D.Sub(p0, p1);

            angle = target.angleBetween(direction);

            // Направление вращения
            var dir = Math.Sign(direction.Cross(target));

            angle = angle * 180 / Math.PI;

            // Костыль
            double to = 0;
            if (dir > 0)
                to = angle * 0.4;
            else
                to = angle * -0.4;

            //Матрицы трансформации
            double[,] points = new double[4, 3] { { p0.X, p0.Y, 1 }, { p1.X, p1.Y, 1 }, { p2.X, p2.Y, 1 }, { p3.X, p3.Y, 1 } };

            Matrix2d mTransNeg = new Matrix2d();
            mTransNeg.Translate(-p0.X, -p0.Y);

            Matrix2d mRot = new Matrix2d();
            mRot.Rotate(to);

            Matrix2d mTrans = new Matrix2d();
            mTrans.Translate(p0.X, p0.Y);

            Matrix2d mRes = mTransNeg * mRot * mTrans; // Перемножение матриц
            var matrixTrans = mRes.ToArray();

            var result_points = Matrix2d.Mult(points, matrixTrans);

            p0.X = result_points[0, 0];
            p0.Y = result_points[0, 1];
            p1.X = result_points[1, 0];
            p1.Y = result_points[1, 1];
            p2.X = result_points[2, 0];
            p2.Y = result_points[2, 1];
            p3.X = result_points[3, 0];
            p3.Y = result_points[3, 1];
        }

        public void GoAway() => maxforce *= -1;

        public Vector2D Eat(List<Particle> list)
        {
            double record = double.MaxValue;
            Vector2D closest = null;

            foreach (var l in list.ToList())
            {
                var d = p0.Dist(l.pos);

                if (d < maxspeed)
                {
                    list.RemoveAt(list.IndexOf(l));
                    
                    switch(l.type)
                    {
                        case ParticleType.Food:
                            health += 0.2;
                            break;
                        case ParticleType.Poison:
                            health -= 1;
                            break;
                    }

                    continue;
                }
                else
                {
                    if (d < record)
                    {
                        record = d;
                        closest = l.pos.CopyToVector();
                    }
                }
            }

            // This is the moment of eating!
            if (closest is null)
                return null;
            else
            {
                return closest.CopyToVector();
            }
        }

        // STEER = DESIRED MINUS VELOCITY
        public Vector2D Seek(Vector2D target)
        {
            var desired = Vector2D.Sub(target, p0); // A vector pointing from the location to the target

            // Scale to maximum speed
            desired.SetMag(maxspeed);

            // Steering = Desired minus velocity
            var steer = Vector2D.Sub(desired, velocity);
            steer.Limit(maxforce); // Limit to maximum steering force

            return steer.CopyToVector();
        }

        public bool IsDead() => health < 0;

        public Vector2D GetPos() => p1.CopyToVector();

        public Vector2D Clone()
        {
            if (MainWindow.rnd.NextDouble() < 0.0005)
                return new Vector2D(MainWindow.rnd.Next(MainWindow.width), MainWindow.rnd.Next(MainWindow.height));
            else 
                return null;
        }

        public void Draw(DrawingContext dc)
        {
            Point P0 = new Point();
            Point P1 = new Point();
            P0.X = p0.X;
            P0.Y = p0.Y;
            P1.X = p1.X;
            P1.Y = p1.Y;

            dc.DrawLine(new Pen(brush, 3), P0, P1);

            Point P2 = new Point();
            Point P3 = new Point();
            P2.X = p2.X;
            P2.Y = p2.Y;
            P3.X = p3.X;
            P3.Y = p3.Y;

            dc.DrawLine(new Pen(brush, 3), P2, P3);
        }
    }
}
