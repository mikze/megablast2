public static class Bomb
{
    public static Task Plant(int x, int y)
    {
        return new Task(async () =>
        {
            await Task.Delay(2000);
            foreach (var e in Game.Entities.Where(a => a is Wall).Select(x => x as Wall).Where(y => y is object && y.Destructible))
                e?.CheckCollistion(new Fire());

        });
    }
}

public class Fire : IEntity
{
    public bool Collision { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; } = 32;
    public int Height { get; set; } = 32;
    public bool CheckCollistion(IEntity entity) => false;
}