using System;
using System.Collections.Generic;
using System.Linq;

namespace BranchAndBound
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ustawienie współrzędnuch punktów
            Dictionary<string, Tuple<double, double>> points = new Dictionary<string, Tuple<double, double>>();
            points.Add("A", new Tuple<double, double>(0, 0));
            points.Add("B", new Tuple<double, double>(4, 7));
            points.Add("C", new Tuple<double, double>(8, 13));
            points.Add("D", new Tuple<double, double>(1, 8));
            points.Add("E", new Tuple<double, double>(6, 4));
            points.Add("F", new Tuple<double, double>(2, 10));
            points.Add("G", new Tuple<double, double>(3, 3));

            // Obliczanie macierzy odległości
            double[,] distances = new double[points.Count, points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    double x1 = points.ElementAt(i).Value.Item1;
                    double y1 = points.ElementAt(i).Value.Item2;
                    double x2 = points.ElementAt(j).Value.Item1;
                    double y2 = points.ElementAt(j).Value.Item2;
                    double dist = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
                    distances[i, j] = dist;
                }
            }

            // Tworzenie stanu początkowego
            State startState = new State(0, new List<int>(), distances);
            for (int i = 1; i < points.Count; i++)
            {
                startState.Unvisited.Add(i);
            }

            // Tworzenie kolejki z priorytetem
            PriorityQueue<State> queue = new PriorityQueue<State>();
            queue.Enqueue(startState, startState.LowerBound);

            // Uruchomienie algorytmu "Branch and Bound
            State bestState = null;
            while (queue.Count > 0)
            {
                State currentState = queue.Dequeue();

                if (currentState.Unvisited.Count == 0)
                {
                    if (bestState == null || currentState.Cost < bestState.Cost)
                    {
                        bestState = currentState;
                    }
                }
                else if (bestState == null || currentState.LowerBound < bestState.Cost)
                {
                    foreach (int city in currentState.Unvisited)
                    {
                        List<int> newUnvisited = new List<int>(currentState.Unvisited);
                        newUnvisited.Remove(city);

                        State newState = new State(city, newUnvisited, distances);
                        queue.Enqueue(newState, newState.LowerBound);
                    }
                }
            }

            // Wypisywanie wynikow
            Console.Write("Маршрут: ");
            foreach (int city in bestState.Path)
            {
                Console.Write(points.ElementAt(city).Key + " => ");
            }
            Console.WriteLine("A");
            Console.WriteLine("Длина маршрута: " + bestState.Cost);
        }
    }

    // Klasa do reprezentowania stanu zadania
    class State
    {
        public int LastCity { get; set; }
        public List<int> Unvisited { get; set; }
        public List<int> Path { get; set; }
        public double Cost { get; set; }
        public double[,] Distances { get; set; }

        public State(int lastCity, List<int> unvisited, double[,] distances)
        {
            LastCity = lastCity;
            Unvisited = unvisited;
            Path = new List<int>();
            Cost = 0;
            Distances = distances;
        }

        // Obliczenie dolnej granicy dla danego stanu
        public double LowerBound
        {
            get
            {
                double bound = Cost;
                // Obliczenie pozostałych odległości od ostatniego miasta do najbliższych nieodwiedzanych miast
                while (Unvisited.Count > 0)
                {
                    double minDist = double.MaxValue;
                    foreach (int city in Unvisited)
                    {
                        double dist = Distances[LastCity, city];
                        if (dist < minDist)
                        {
                            minDist = dist;
                        }
                    }
                    bound += minDist;
                    LastCity = Array.IndexOf(Distances, minDist);
                    Unvisited.Remove(LastCity);
                }
                // Dodanie odległości od ostatniego miasta do miasta początkowego
                bound += Distances[LastCity, 0];
                return bound;
            }
        }
    }

    // Klasa do implementacji kolejki z priorytetem
    class PriorityQueue<T>
    {
        private SortedDictionary<double, Queue<T>> _queues = new SortedDictionary<double, Queue<T>>();

        public int Count { get; private set; }

        public void Enqueue(T item, double priority)
        {
            Queue<T> queue;
            if (!_queues.TryGetValue(priority, out queue))
            {
                queue = new Queue<T>();
                _queues.Add(priority, queue);
            }
            queue.Enqueue(item);
            Count++;
        }

        public T Dequeue()
        {
            var pair = _queues.First();
            var queue = pair.Value;
            var item = queue.Dequeue();
            if (queue.Count == 0)
            {
                _queues.Remove(pair.Key);
            }
            Count--;
            return item;
        }
    }
}