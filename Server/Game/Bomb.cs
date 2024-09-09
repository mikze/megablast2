
using System.Runtime.CompilerServices;

public static class Bomb
{
    static object Locker = new object(); 
    public static Task Plant(int x, int y)
    {
        return new Task(async () =>
        {
            Console.WriteLine($"PLANT BOMB");
            int size = 4;
            List<Fire> fires1 = new List<Fire>();
            List<Fire> fires2 = new List<Fire>();
            List<Fire> fires3 = new List<Fire>();
            List<Fire> fires4 = new List<Fire>();

            for (int i = 1; i <= size; i++)
                fires1.Add(new Fire() { PosX = x, PosY = y + (i * 32) });

            for (int i = 1; i <= size; i++)
                fires2.Add(new Fire() { PosX = x + (i * 32), PosY = y });

            for (int i = -1; i >= -size; i--)
                fires3.Add(new Fire() { PosX = x, PosY = y + (i * 32) });

            for (int i = -1; i >= -size; i--)
                fires4.Add(new Fire() { PosX = x + (i * 32), PosY = y });

            await Task.Delay(2000);
            Calc(fires1);
            Calc(fires2);
            Calc(fires3);
            Calc(fires4);
        });

        static void Calc(List<Fire> fires)
        {
            lock(Locker)
            {
                foreach (var f in fires)
                {
                    bool stop = false;

                    foreach (var e in Game.GetEntities().Where(a => a is Wall).Select(x => x as Wall).Where(y => y is object && y.Destructible))
                        if (e is object && e.Destructible && !e.Destroyed && e.CheckCollistion(f))
                        {
                            e.Destroyed = true;
                            Console.WriteLine($"Destroy block: {e.PosX} {e.PosY};  fire: {f.PosX}  {f.PosY}");
                            stop = true;
                        }

                    if (stop)
                        break;
                }
            }
        }
    }
}

public class Fire : IEntity
{
    public bool Collision { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public int Width { get; set; } = 1;
    public int Height { get; set; } = 1;
    public string Id { get; init; } = "";
    public bool Destroyed { get; set; }

    public bool CheckCollistion(IEntity entity) => false;
}