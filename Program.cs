using System.Diagnostics;

List<List<int>> map = new() { new() };
using (StreamReader reader = new(args[0]))
{
    while (!reader.EndOfStream)
    {
        int c = reader.Read();
        if (c == '\n')
            map.Add(new());
        else if(c>='0'&&c<='9')
            map.Last().Add(c - '0');
    }
}

ROWS = map.Count;
COLS = map.First().Count;

solve(false);//part1
solve(true);//part2


void solve(bool part2)
{
    Stopwatch sw = Stopwatch.StartNew();
    Dictionary<node, int> bestTravels = new();
    PriorityQueue<node, int> pQ = new();
    pQ.Enqueue(new(0, 0, DIR.NONE, 0), 0);
    while (pQ.Count > 0)
    {
        pQ.TryDequeue(out var weAt,out int travel_time);
        if (bestTravels.ContainsKey(weAt))
            continue; //we found better before -- new path dequeued
        bestTravels[weAt] = travel_time;
        foreach (var dir in cardinalDIRs)
        {
            if (dir == reverse(weAt.dir))
                continue;
            if (dir==weAt.dir && weAt.dirCount >= (part2 ? 10 : 3))
                continue;
            int travelHowMuch = (part2 && dir != weAt.dir) ? 4 : 1;
            int row = weAt.row + rowModifier(dir,travelHowMuch);
            int col = weAt.col + colModifier(dir,travelHowMuch);
            if (row >= 0 && row < ROWS && col >= 0 && col < COLS)
            {
                int sameDir = (dir == weAt.dir) ? weAt.dirCount + travelHowMuch : travelHowMuch;
                int costOfTravel = travel_time;
                for(int i=1;i<=travelHowMuch;i++)
                {
                    int rowAddition = weAt.row + rowModifier(dir, i);
                    int colAddition = weAt.col + colModifier(dir, i);
                    costOfTravel += map[rowAddition][colAddition];
                }
                pQ.Enqueue(new(row, col, dir, sameDir), costOfTravel);//QUEUE new path
            }
        }
    }
    int min = int.MaxValue;
    foreach (var bestTravel in bestTravels)
        if (bestTravel.Key.row == ROWS - 1 && bestTravel.Key.col == COLS - 1)
            if (bestTravel.Value < min)
                min = bestTravel.Value;
    Console.WriteLine($"part {(part2?2:1)}: {min}\t\t~{sw.ElapsedMilliseconds}ms");
}

int rowModifier(DIR dir, int val)
{
    if (dir == DIR.DOWN)
        return val;
    if (dir == DIR.UP)
        return -val;
    return 0;
}

int colModifier(DIR dir, int val)
{
    if (dir == DIR.RIGHT)
        return val;
    if (dir == DIR.LEFT)
        return -val;
    return 0;
}

DIR reverse(DIR dir)
{
    return dir switch
    {
        DIR.UP => DIR.DOWN,
        DIR.DOWN => DIR.UP,
        DIR.LEFT => DIR.RIGHT,
        DIR.RIGHT => DIR.LEFT,
        _ => DIR.NONE
    };
}

enum DIR : short
{
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3,
    NONE = 5
}

readonly record struct node
{
    public readonly int row;
    public readonly int col;
    public readonly DIR dir;
    public readonly int dirCount;
    public node(int row, int col, DIR dir, int dirCount) =>
        (this.row, this.col, this.dir, this.dirCount) = (row, col, dir, dirCount);

}

partial class Program
{
    private static readonly DIR[] cardinalDIRs = new DIR[] { DIR.UP, DIR.DOWN, DIR.LEFT, DIR.RIGHT };
    private static int COLS;
    private static int ROWS;
}
