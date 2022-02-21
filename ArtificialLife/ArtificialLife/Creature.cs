using System;
using System.Collections.Generic;
using System.Drawing;

namespace ArtificialLife
{
    public class Creature
    {
        private readonly CreatureFactory _creatureFactory;
        private readonly IFiled _filed;
        public int X { get; set; }
        public int Y { get; set; }
        public const int MAX_GENE_TYPES = 6;
        public const int GENE_LENGTH = 60;
        public const int FIELD_WIDTH = 600;
        public const int FIELD_HEIGHT = 400;
        private static Random _rnd = new Random();
        public Color Color { get; set; } = Color.Black;
        public Creature(CreatureFactory creatureFactory, IFiled filed)
        {
            _creatureFactory = creatureFactory;
            _filed = filed;
            X = _rnd.Next(0, FIELD_WIDTH);
            Y = _rnd.Next(0, FIELD_HEIGHT);
            for (int i = 0; i < GENE_LENGTH; i++)
            {
                
                Genes.Add(_rnd.Next(0,10)==1?EGene.Breed:EGene.Eat);
            }
        }
        public List<EGene> Genes { get; set; } = new List<EGene>();

        public int Hunger { get; set; } = 50;
        public int Age { get; set; } = 0;
        public bool IsDead { get; set; }
        private int _activeGene = 0;
        public void Update()
        {
            if (X > FIELD_WIDTH) X = 0;
            if (X < 0) X = FIELD_WIDTH;

            if (Y > FIELD_HEIGHT) Y = 0;
            if (Y < 0) Y = FIELD_HEIGHT;
            var gene = Genes[_activeGene];
            switch (gene)
            {
                case EGene.Up:
                    Y--;
                    break;
                case EGene.Down:
                    Y++;
                    break;
                case EGene.Left:
                    X--;
                    break;
                case EGene.Right:
                    X++;
                    break;
                //case EGene.ColorRed:
                //    var r = Color.R;
                //    if (r < 255) r += 15;
                //    Color = Color.FromArgb(r, Color.G, Color.B);
                //    break;
                //case EGene.ColorGreen:
                //    var g = Color.G;
                //    if (g < 255) g += 15;
                //    Color = Color.FromArgb(Color.R,g, Color.B);
                //    break;
                //case EGene.ColorBlue:
                //    var b = Color.B;
                //    if (b < 255) b+=15;
                //    Color = Color.FromArgb(Color.R, Color.G,b);
                //    break;
                case EGene.Eat:
                    
                    if (_filed.IsFood(X, Y,false))
                    {
                        var g = Color.G;
                        if (g < 255) g += 15;
                        Color = Color.FromArgb(Color.R, g, Color.B);
                        Hunger -=GENE_LENGTH*2;
                        if (Hunger < 0) Hunger = 0;
                    }
                    break;
                case EGene.EatCorpse:
                    
                    if (_filed.IsFood(X, Y,true))
                    {
                        var r = Color.R;
                        if (r < 255) r += 15;
                        Color = Color.FromArgb(r, Color.G, Color.B);
                        Hunger -=GENE_LENGTH;
                        if (Hunger < 0) Hunger = 0;
                    }
                    break;
                case EGene.Breed:
                    if (Hunger < 5)
                    {
                        _filed.AddCreature(_creatureFactory.CreateChild(this));
                        Hunger += 50;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _activeGene++;
            if (_activeGene >= Genes.Count)
            {
                _activeGene = 0;
            }

            //Age++;
            //if (Age >= 30 * 2)
            //{
            //    IsDead = true;
            //}







            if (Hunger == 300)
            {
                IsDead = true;
            }
            Hunger++;

            if (X > FIELD_WIDTH) X = 0;
            if (X < 0) X = FIELD_WIDTH;

            if (Y > FIELD_HEIGHT) Y = 0;
            if (Y < 0) Y = FIELD_HEIGHT;

        }
    }
}