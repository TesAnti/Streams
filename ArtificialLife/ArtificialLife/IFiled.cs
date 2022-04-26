namespace ArtificialLife
{
    public interface IFiled
    {
        void AddCreature(Creature creature);
        bool IsFood(int x, int y,bool corpse);
    }
}