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
        pQ.TryDequeue(out node weAt, out int travel_time);

        if (bestTravel.ContainsKey(weAt))
            return; //we found better before -- new path dequeued

        bestTravels[weAt] = travel_time;

        //QUEUE new paths forward:
        HashSet<DIR> newDirrections = new() { DIR.UP, DIR.DOWN, DIR.LEFT, DIR.RIGHT };
        newDirrections.Remove(reverse(weAt.D));//can't travel backwards
        if (weAt.SameDir >= 3) newDirrections.Remove(weAt.D);//can't travel this dirrection anymore
        foreach (DIR dir in newDirrections)
        {
            int row = weAt.Row + rowModifier(weAt.D);
            int col = weAt.Col + colModifier(weAt.D);
            if (row >= 0 && row < ROWS && col >= 0 && col < COLS)
            {
                int sameDir = dir == weAt.D ? weAt.SameDir + 1 : 1;
                pQ.Enqueue(new(row, col, dir, sameDir), travel_time + map[row][col]);
            }
        }
    }

    int min = int.MaxValue;
    foreach (var bestTravel in bestTravels)
        if (bestTravel.Key.Row == ROWS - 1 && bestTravel.Key.Col == COLS - 1)
            if (bestTravel.Value < min)
                min = bestTravel.Value;
    Console.WriteLine(min - map[0][0]);
}

short rowModifier(DIR dir)
{
    if (dir == DIR.DOWN)
        return 1;
    if (dir == DIR.UP)
        return -1;
    return 0;
}

short colModifier(DIR dir)
{
    if (dir == DIR.RIGHT)
        return 1;
    if (dir == DIR.LEFT)
        return -1;
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
    public DIR D;
    public int SameDir;
    public node(int row, int col, DIR d, int sameDir) =>
        (Row, Col, D, SameDir) = (row, col, d, sameDir);
}