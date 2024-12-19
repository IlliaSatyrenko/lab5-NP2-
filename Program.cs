class BeeAlgorithm
{
    public int N;
    public int K_F;
    public int K_D;

    private Random random = new Random();
    public long[,] graph;

    public BeeAlgorithm(int fur, int dil, int n, long[,] gr)
    {
        K_F = fur;
        K_D = dil;
        N = n;

        graph = new long[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                graph[i, j] = gr[i, j];
            }
        }
    }

    public static long[,] InitializeGraph(int n)
    {
        Random random = new Random();
        long[,] tempGraph = new long[n, n];

        for (int i = 0; i < n; i++)
        {
            int edges = 0;
            for (int j = i + 1; j < n; j++)
            {
                if (edges < 10 && random.NextDouble() < 0.3)
                {
                    tempGraph[i, j] = random.Next(5, 151);
                    tempGraph[j, i] = tempGraph[i, j];
                    edges++;
                }
                else
                {
                    tempGraph[i, j] = long.MaxValue;
                }
            }
            if (edges == 0)
            {
                long neighbor = random.Next(0, n);
                while (neighbor == i)
                {
                    neighbor = random.Next(0, n);
                }

                tempGraph[i, neighbor] = random.Next(5, 151);
                tempGraph[neighbor, i] = tempGraph[i, neighbor];
            }
        }

        return tempGraph;
    }

    public (List<long> Path, long Distance) BeeOptimization(int start, int end)
    {
        List<long> bestPath = new List<long>();
        long bestDistance = long.MaxValue;

        int noImprovementCounter = 0;
        int maxNoImprovement = 100;
        int iter = 1;

        while (noImprovementCounter < maxNoImprovement)
        {
            List<(List<long>, long)> scoutRoutes = GenerateRandomRoutes(start, end, K_D);
            List<(List<long>, long)> forageRoutes = ExploitRoutes(scoutRoutes, K_F);

            bool improved = false;

            foreach (var route in forageRoutes)
            {
                if (route.Item2 < bestDistance)
                {
                    bestDistance = route.Item2;
                    bestPath = route.Item1;
                    improved = true;
                }
            }
            iter++;

            if (improved)
            {
                noImprovementCounter = 0;
            }
            else
            {
                noImprovementCounter++;
            }

            if (noImprovementCounter >= maxNoImprovement)
            {
                Console.WriteLine($"Алгоритм завершено достроково на {iter + 1}-й ітерації через відсутність покращень.");
            }
        }

        return (bestPath, bestDistance);
    }

    private List<(List<long>, long)> GenerateRandomRoutes(int start, int end, int count)
    {
        var routes = new List<(List<long>, long)>();

        for (int i = 0; i < count; i++)
        {
            List<long> route = new List<long> { start };
            int current = start;

            while (current != end)
            {
                var neighbors = Enumerable.Range(0, N).Where(v => graph[current, v] != long.MaxValue && !route.Contains(v)).ToList();
                if (neighbors.Count == 0) break;
                current = neighbors[random.Next(neighbors.Count)];
                route.Add(current);
            }

            if (current == end)
            {
                long distance = CalculateDistance(route);
                routes.Add((route, distance));
            }
        }

        return routes;
    }

    private List<(List<long>, long)> ExploitRoutes(List<(List<long>, long)> scoutRoutes, int count)
    {
        var forageRoutes = new List<(List<long>, long)>();

        foreach (var (route, distance) in scoutRoutes)
        {
            List<long> bestLocalRoute = new List<long>(route);
            long bestLocalDistance = distance;

            for (int i = 0; i < count; i++)
            {
                var newRoute = MutateRoute(route);
                long newDistance = CalculateDistance(newRoute);

                if (newDistance < bestLocalDistance)
                {
                    bestLocalRoute = newRoute;
                    bestLocalDistance = newDistance;
                }
            }

            forageRoutes.Add((bestLocalRoute, bestLocalDistance));
        }

        return forageRoutes;
    }

    private List<long> MutateRoute(List<long> route)
    {
        var mutatedRoute = new List<long>(route);
        int index = random.Next(1, route.Count - 1);
        var neighbors = Enumerable.Range(0, N).Where(v => graph[route[index - 1], v] != long.MaxValue && !route.Contains(v)).ToList();

        if (neighbors.Count > 0)
        {
            long bestNeighbor = neighbors.OrderBy(v => graph[route[index - 1], v]).First();

            if (graph[bestNeighbor, route[index + 1]] != long.MaxValue)
            {
                mutatedRoute[index] = bestNeighbor;

                long oldDistance = CalculateDistance(route);
                long newDistance = CalculateDistance(mutatedRoute);

                if (newDistance < oldDistance && newDistance != long.MaxValue)
                {
                    return mutatedRoute;
                }
                else
                {
                    mutatedRoute[index] = route[index];
                }
            }
        }
        return mutatedRoute;
    }

    private long CalculateDistance(List<long> route)
    {
        long distance = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            if (graph[route[i], route[i + 1]] == long.MaxValue)
            {
                return long.MaxValue;
            }
            distance += graph[route[i], route[i + 1]];
            if (distance < 0)
            {
                return long.MaxValue;
            }
        }
        return distance;
    }
}

class Item
{
    public int ind;
    public int dist;

    public Item(int i, int d)
    {
        ind = i;
        dist = d;
    }
}

class Program
{    
    static void Main()
    {
        int n = 300;
        int fur = 45;
        int dil = 10;

        long[,] graph = BeeAlgorithm.InitializeGraph(n);

        //List<Item> bestVals = new List<Item>();

        for (int i = 5; i <= 5; i += 2)
        {
            BeeAlgorithm ba = new BeeAlgorithm(fur, dil, n, graph);
            var bestPath = ba.BeeOptimization(0, n - 1);

            //bestVals.Add(new Item(i, bestPath.Distance));

            Console.WriteLine("Найкоротший шлях:");
            Console.WriteLine(string.Join(" -> ", bestPath.Path));
            Console.WriteLine($"Довжина шляху: {bestPath.Distance}");
            Console.WriteLine("");
        }

        //Console.WriteLine("Best:  " + bestVals.OrderBy(val => val.dist).ToList().First().ind + "   and dist: "+ bestVals.OrderBy(val => val.dist).ToList().First().dist);
    }
}