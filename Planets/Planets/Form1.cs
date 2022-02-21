using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Planets
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            sw.Start();
            _trackLayer = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(_trackLayer))
            {
                g.Clear(Color.Black);
            }
        }

        private Bitmap _trackLayer;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private Point _mouseDownPoint;
        private bool _isDown = false;
        private DateTime _downTime;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isDown = true;
            _mouseDownPoint = new Point(e.X, e.Y);
            _downTime = DateTime.Now;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDown = false;
            var speedVector = new PointF((e.X - _mouseDownPoint.X) / 10f, (e.Y - _mouseDownPoint.Y) / 10f);
            var mass = (float)(DateTime.Now - _downTime).TotalSeconds;
            _planets.Add(new Planet(new PointF(e.X, e.Y), speedVector) { Mass = mass, Radius = mass * 2 });
        }

        private Point _lastMouse = new Point();

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _lastMouse = new Point(e.X, e.Y);
        }

        private List<Planet> _planets = new List<Planet>()
        {

            new Planet(new PointF(300, 200), new PointF(2, 0)) { Mass = 10 ,Radius = 10},
            new Planet(new PointF(400, 300), new PointF(0, 0)) { Mass = 10 ,Radius = 10,Fixed = true},
            
            
        };
      
        private float simulationSpeed = 1f;
        Stopwatch sw = new Stopwatch();


        protected override void OnPaint(PaintEventArgs e)
        {
            sw.Restart();
            var g = e.Graphics;
            g.Clear(Color.Black);

            //f=(m1+m2)/(r*r)
            //a=f/m1
            //a=m2/(r*r)

            

            foreach (Planet planet in _planets)
            {
                if (planet.Delete) continue;


                foreach (Planet planet2 in _planets)
                {
                    if (planet == planet2) continue;
                    if (planet2.Delete) continue;
                    var m2 = planet2.Mass;
                    var forceVector = new PointF(planet2.Position.X - planet.Position.X,
                        planet2.Position.Y - planet.Position.Y);
                    var r = Math.Sqrt(forceVector.X * forceVector.X + forceVector.Y * forceVector.Y);
                    var a = new PointF((float)(forceVector.X * m2 / (r * r)),
                        (float)(forceVector.Y * m2 / (r * r)));

                    planet.SpeedVector = new PointF(planet.SpeedVector.X + a.X * simulationSpeed,
                        planet.SpeedVector.Y + a.Y * simulationSpeed);

                   
                }
            }

            _planets.RemoveAll(x => x.Delete);
           

            using (Graphics gTrack = Graphics.FromImage(_trackLayer))
            {
                foreach (Planet planet in _planets)
                {
                    var start = planet.Position;
                    planet.Update(simulationSpeed);
                    var end = planet.Position;
                    gTrack.DrawLine(Pens.Green, start, end);
                }
            }

            g.DrawImage(_trackLayer, 0, 0);
            foreach (Planet planet in _planets)
            {
                g.FillEllipse(new SolidBrush(Color.Red), planet.Position.X - planet.Radius,
                    planet.Position.Y - planet.Radius, planet.Radius*2 + 1, planet.Radius*2 + 1);
            }

            if (_isDown)
            {
                Pen p = new Pen(Color.BlueViolet, 2);
                p.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(p, _mouseDownPoint, _lastMouse);
                g.DrawString((DateTime.Now - _downTime).TotalSeconds.ToString(), SystemFonts.CaptionFont,
                    Brushes.Violet, _lastMouse.X + 10, _lastMouse.Y);
            }

            while (sw.ElapsedMilliseconds < 20)
            {
                Thread.Sleep(1);
            }

            g.DrawString("FPS:" + (1000 / sw.ElapsedMilliseconds).ToString(), SystemFonts.CaptionFont, Brushes.Green,
                10, 10);
            Invalidate();
        }

        
    }
}