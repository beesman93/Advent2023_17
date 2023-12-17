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

int ROWS = map.Count;
int COLS = map.First().Count;

solve(false);//part1
solve(true);//part2

void solve(bool part2)
{
    Stopwatch sw = Stopwatch.StartNew();
    Dictionary<(int row, int col, (int row, int col) dir, int dirCount), int> bestTravels = new();
    PriorityQueue<(int row, int col, (int row, int col) dir, int dirCount), int> pQ = new();
    pQ.Enqueue(new(0, 0, (0,0), 0), 0);
    (int row,int col)[] cardinalDIRs = new[] {(0,1),(1,0),(-1,0),(0,-1)};
    while (pQ.TryDequeue(out var weAt, out int travel_time))
    {
        if (bestTravels.ContainsKey(weAt))
            continue; //we found better before -- new (worse) path dequeued, go next
        bestTravels[weAt] = travel_time;
        foreach (var dir in cardinalDIRs)
        {
            if (dir == reverse(weAt.dir))
                continue;//don't reverse
            if (dir==weAt.dir && weAt.dirCount >= (part2 ? 10 : 3))
                continue;//don'T continue same dirrection after 3(p1) / 10(p2)
            int travelHowMuch = (part2 && dir != weAt.dir) ? 4 : 1; //step by step p1/p2 same dir, jump 4 p2 new dirrection
            int row = weAt.row + travelHowMuch * dir.row;
            int col = weAt.col + travelHowMuch * dir.col;
            if (row >= 0 && row < ROWS && col >= 0 && col < COLS)
            {
                int sameDir = (dir == weAt.dir) ? weAt.dirCount + travelHowMuch : travelHowMuch;
                int costOfTravel = travel_time;
                for(int i=1;i<=travelHowMuch;i++)
                {
                    int rowAddition = weAt.row + i * dir.row;
                    int colAddition = weAt.col + i * dir.col;
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
    sw.Stop();
    Debug.Assert(part2 ? min == 1157 : min == 936);
    Console.WriteLine($"part {(part2?2:1)}: {min}\t\t~{sw.ElapsedMilliseconds}ms");
}
(int row, int col) reverse((int row,int col) dir)
{
    return (-dir.row,-dir.col);
}