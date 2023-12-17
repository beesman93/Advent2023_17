using System.Drawing;
using System.Runtime.InteropServices;

List<string> lines = new();
using (StreamReader reader = new(args[0]))
{
    while (!reader.EndOfStream)
        lines.Add(reader.ReadLine());
}

int ROWS = lines.Count;
int COLS = lines[0].Length;

int[][]map = new int[ROWS][];
int[][] costFromOrigin = new int[ROWS][];
for (int row = 0; row < ROWS; row++)
{
    map[row] = new int[COLS];
    costFromOrigin[row] = new int[COLS];
    for (int col = 0; col < COLS; col++)
    {
        map[row][col] = lines[row][col] - '0';
        costFromOrigin[row][col] = int.MaxValue;
    }
}

solve(false);
solve(true);
void solve(bool part2)
{
    //bestTravel for each node from each dirrection with dirCount
    //pQ of where we could travel next
    Dictionary<node, int> bestTravels = new();
    PriorityQueue<node, int> pQ = new();
    pQ.Enqueue(new(0, 0, DIR.NONE, 0), 0); //(row,col,dirrection, dir_count), travel_time

    while (pQ.Count > 0)
    {
        popNext(ref pQ, ref bestTravels);
    }
    void popNext(ref PriorityQueue<node, int> pQ, ref Dictionary<node, int> bestTravel)
    {
        pQ.TryDequeue(out node weAt,out int travel_time);

        if (bestTravel.ContainsKey(weAt))
            return; //we found better before -- new path dequeued

        bestTravels[weAt] = travel_time;

        //QUEUE new paths forward:
        HashSet<DIR> newDirrections = new() { DIR.UP, DIR.DOWN, DIR.LEFT, DIR.RIGHT };
        if(weAt.Dir!=DIR.NONE)newDirrections.Remove(reverse(weAt.Dir));//can't travel backwards
        //if (part2 && weAt.Dir != DIR.NONE && weAt.SameDir < 4) newDirrections = new() { weAt.Dir };//we have to travel same dirrection in part2
        if (weAt.SameDir >= (part2?10:3)) newDirrections.Remove(weAt.Dir);//can't travel this dirrection anymore
        foreach (DIR dir in newDirrections)
        {
            int travelHowMuch = 1;
            if (part2 && dir != weAt.Dir)
                travelHowMuch = 4;
            int row = weAt.Row + rowModifier(dir,travelHowMuch);
            int col = weAt.Col + colModifier(dir,travelHowMuch);
            if (row >= 0 && row < ROWS && col >= 0 && col < COLS)
            {
                int sameDir = dir == weAt.Dir ? weAt.SameDir + travelHowMuch : travelHowMuch;
                int costOfTravel = travel_time;
                for(int i=1;i<=travelHowMuch;i++)
                {
                    int rowAddition = weAt.Row + rowModifier(dir, i);
                    int colAddition = weAt.Col + colModifier(dir, i);
                    costOfTravel += map[rowAddition][colAddition];
                }
                pQ.Enqueue(new(row, col, dir, sameDir), costOfTravel);
            }
        }
    }

    int min = int.MaxValue;
    foreach (var bestTravel in bestTravels)
        if (bestTravel.Key.Row == ROWS - 1 && bestTravel.Key.Col == COLS - 1)
            if (bestTravel.Value < min)
                min = bestTravel.Value;
    Console.WriteLine(min);
}

int rowModifier(DIR dir,int val)
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

struct node
{
    public int Row;
    public int Col;
    public DIR Dir;
    public int SameDir;
    public node(int row, int col, DIR d, int sameDir) =>
        (Row, Col, Dir, SameDir) = (row, col, d, sameDir);
}