List<List<int>> map = new();
using (StreamReader reader = new(args[0]))
    while(!reader.EndOfStream)
    {
        map.Add(new());
        foreach (char c in reader.ReadLine())
            map.Last().Add(c - '0');
    }

new List<bool> { false, true }.ForEach(b => Console.WriteLine($"part{(b?2:1)}: {solve(b)}"));
int solve(bool part2)
{
    Dictionary<(int row, int col, (int row, int col) dir, int dirCount), int> dists = new();
    PriorityQueue<(int row, int col, (int row, int col) dir, int dirCount), int> pQ = new();
    pQ.Enqueue((0,0,(0,0),0),0);
    (int row,int col)[] cardinalDIRs = new[]{(0,1),(1,0),(-1,0),(0,-1)};
    while (pQ.TryDequeue(out var weAt, out int travel_time))
    {
        if (dists.ContainsKey(weAt)) continue;//found better or eq before
        dists[weAt] = travel_time;
        foreach (var dir in cardinalDIRs)
        {
            if (dir == (-weAt.dir.row,-weAt.dir.col)) continue;//don't reverse
            if (dir==weAt.dir && weAt.dirCount >= (part2 ? 10 : 3)) continue;//dirrection limit
            int travelHowMuch = (part2 && dir != weAt.dir) ? 4 : 1; //p2 new dirrection - jump 4
            int row = weAt.row + travelHowMuch * dir.row;
            int col = weAt.col + travelHowMuch * dir.col;
            if (row >= 0 && row < map.Count && col >= 0 && col < map[row].Count)
            {
                int dirCount = travelHowMuch + (dir == weAt.dir ? weAt.dirCount : 0);
                int costOfTravel = travel_time;
                for(int i=1;i<=travelHowMuch;i++)
                    costOfTravel += map[weAt.row + i * dir.row][weAt.col + i * dir.col];
                pQ.Enqueue(new(row, col, dir, dirCount), costOfTravel);//QUEUE new path
            }
        }
    }
    return dists.Where(d => d.Key.row==map.Count-1 && d.Key.col==map[d.Key.row].Count-1).Min(d => d.Value);
}