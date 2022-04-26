using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ArtificialLife
{
    public partial class Form1 : Form,IFiled
    {
        private Brush[] _foodColor = new Brush[100];
        private Brush[] _corpseColor = new Brush[100];
        public Form1()
        {
            InitializeComponent();
            _backBuffer = new Bitmap(Creature.FIELD_WIDTH, Creature.FIELD_HEIGHT);
            _creatureFactory = new CreatureFactory(this);

            for (int i = 0; i < 100; i++)
            {
                var creature1 = _creatureFactory.CreateCreature();
                AddCreature(creature1);

            }

            for (int x = 200; x < 300; x++)
            {
                for (int y = 100; y < 300; y+=50)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        _foodData[x, y +i] = 10;
                    }
                    
                }
            }

            for (int i = 0; i <= 10; i++)
            {
                _foodColor[i] = new SolidBrush(Color.FromArgb(i*20, Color.Aquamarine));
                _corpseColor[i] = new SolidBrush(Color.FromArgb(i*20, Color.Crimson));
            }



        }

        private List<Creature> _creatures = new List<Creature>();
        private readonly CreatureFactory _creatureFactory;
        private Random _random = new Random();
        private int[,] _foodData = new int[1000, 1000];
        private int[,] _corpseData = new int[1000, 1000];
        protected override void OnPaint(PaintEventArgs e)
        {
            using (Graphics g=Graphics.FromImage(_backBuffer))
            {
                Stopwatch sw = Stopwatch.StartNew();

                g.Clear(Color.DarkGray);
                
                _creatures.AddRange(_spawnBuffer);
                _spawnBuffer.Clear();
                int foodCounter = 0;
                int corpseCounter = 0;
                for (int x = 0; x < Creature.FIELD_WIDTH; x++)
                {
                    for (int y = 0; y < Creature.FIELD_HEIGHT; y++)
                    {
                        if (_foodData[x, y] > 0)
                        {
                            g.FillRectangle(_foodColor[_foodData[x, y]], x, y, 1, 1);
                            foodCounter++;
                        }
                        if (_corpseData[x, y] > 0)
                        {
                            g.FillRectangle(_corpseColor[_corpseData[x, y]], x, y, 1, 1);
                            corpseCounter++;
                        }
                    }
                }
                foreach (Creature creature in _creatures)
                {
                    if (creature.IsDead)
                    {
                        _corpseData[creature.X, creature.Y] = 3;
                    }
                }
                _creatures.RemoveAll(x => x.IsDead);
                foreach (Creature creature in _creatures)
                {
                    g.FillEllipse(new SolidBrush(creature.Color), creature.X - 2, creature.Y - 2, 5, 5);
                }

                foreach (Creature creature in _creatures)
                {
                    creature.Update();
                }
                
                if (_creatures.Count < 100)
                {

                }

                

                g.DrawString(_creatures.Count.ToString(), SystemFonts.CaptionFont, Brushes.Red, 10, 10);
                g.DrawString(foodCounter.ToString(), SystemFonts.CaptionFont, Brushes.Red, 10, 20);
                g.DrawString(corpseCounter.ToString(), SystemFonts.CaptionFont, Brushes.Red, 10, 30);

                sw.Stop();
                var delay = (int)(33 - sw.ElapsedMilliseconds);
                if (delay > 0)
                    Thread.Sleep(delay);
            }

            //for (int i = 0; i < 10; i++)
            //{
            //    _foodData[_random.Next(0, Creature.FIELD_WIDTH), _random.Next(0, Creature.FIELD_HEIGHT)] = 10;
            //}

            e.Graphics.DrawImage(_backBuffer,0,0,Width,Height);
            Invalidate();

        }

        private List<Creature> _spawnBuffer = new List<Creature>();
        private readonly Bitmap _backBuffer;

        public void AddCreature(Creature creature)
        {
            _spawnBuffer.Add(creature);



        }

        public bool IsFood(int xi, int yi,bool corpse)
        {
            if (corpse)
            {
                for (int x = -3; x < 3; x++)
                {
                    for (int y = -3; y < 3; y++)
                    {
                        var xq = xi + x;
                        var yq = yi + y;
                        if (xq < 0) xq += Creature.FIELD_WIDTH;
                        if (yq < 0) yq += Creature.FIELD_HEIGHT;
                        if (xq > Creature.FIELD_WIDTH) xq -= Creature.FIELD_WIDTH;
                        if (yq > Creature.FIELD_HEIGHT) yq -= Creature.FIELD_HEIGHT;

                        if (_corpseData[xq, yq] > 0)
                        {
                            _corpseData[xq, yq]--;
                            return true;
                        }
                    }
                }
            }
            else
            {
                for (int x = -3; x < 3; x++)
                {
                    for (int y = -3; y < 3; y++)
                    {
                        var xq = xi + x;
                        var yq = yi + y;
                        if (xq < 0) xq += Creature.FIELD_WIDTH;
                        if (yq < 0) yq += Creature.FIELD_HEIGHT;
                        if (xq > Creature.FIELD_WIDTH) xq -= Creature.FIELD_WIDTH;
                        if (yq > Creature.FIELD_HEIGHT) yq -= Creature.FIELD_HEIGHT;

                        if (_foodData[xq, yq] > 0)
                        {
                            _foodData[xq, yq]--;
                            return true;
                        }
                    }
                }
            }
            
            

            return false;
        }
    }
}
