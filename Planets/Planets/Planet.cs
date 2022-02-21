namespace Planets;

public class Planet
{
    public Planet(PointF position, PointF speedVector)
    {
        Position = position;
        SpeedVector = speedVector;
    }

    public PointF Position { get; set; }
    public PointF SpeedVector { get; set; }
    public float Mass { get; set; } = 1;
    public float Radius { get; set; } = 2;

    public bool Fixed { get; set; }
    public bool Delete { get; set; }
    public void Update(float simulationSpeed)
    {
        if (Fixed)
        {
            return;
        }
        Position = new PointF(Position.X + SpeedVector.X* simulationSpeed,
            Position.Y + SpeedVector.Y* simulationSpeed);
    }
}