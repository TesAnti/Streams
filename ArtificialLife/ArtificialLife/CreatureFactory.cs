using System;
using System.Collections.Generic;

namespace ArtificialLife
{
    public class CreatureFactory
    {
        private readonly IFiled _filed;
        private Random _rnd = new Random();

        public CreatureFactory(IFiled filed)
        {
            _filed = filed;
        }

        public Creature CreateCreature()
        {
            return new Creature(this, _filed);
        }
        public Creature CreateChild(Creature parent)
        {
            var res= new Creature(this, _filed);
            var parentGenes = parent.Genes;
            
            res.Genes = new List<EGene>();
            res.Genes.AddRange(parentGenes);
            res.X = parent.X+_rnd.Next(-3,4);
            res.Y = parent.Y + _rnd.Next(-3, 4);

            var i = _rnd.Next(0,res.Genes.Count);
            var newGene = (EGene) _rnd.Next(0, Creature.MAX_GENE_TYPES+1);

            res.Genes[i] = newGene;

            return res;
        }
    }
}