using System.Collections.Frozen;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day23;

/// <summary>
/// Solution code for Day 23: LAN Party
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 23: LAN Party"; }
    }

    /*
    public string InputFile
    {
        get
        {
            var dir = NamespacePath.GetFolderPathFromType(GetType());
            return Path.Combine(dir, "input.test.txt");
        }
    }
    //*/

    class AdjacencyMatrixGraph(int size)
    {
        private readonly int[,] _matrix = new int[size, size];
        private readonly Dictionary<string, int> _nameToId = new(size);

        private int GetOrCreateVertexId(string name)
        {
            if (!_nameToId.TryGetValue(name, out var id))
            {
                id = _nameToId.Count;
                _nameToId[name] = id;
            }
            return id;
        }

        public int Size => _matrix.GetLength(0);

        public IEnumerable<string> Vertices => _nameToId.Keys;

        public void AddEdge(string from, string to)
        {
            int fromId = GetOrCreateVertexId(from);
            int toId = GetOrCreateVertexId(to);
            _matrix[fromId, toId] = 1;
            _matrix[toId, fromId] = 1;
        }

        public bool HasEdge(string from, string to)
        {
            if (!_nameToId.ContainsKey(from) || !_nameToId.ContainsKey(to))
            {
                return false;
            }
            int fromId = _nameToId[from];
            int toId = _nameToId[to];
            return _matrix[fromId, toId] == 1;
        }

        public IEnumerable<string> GetAdjacentVertices(string vertex)
        {
            int vertexId = _nameToId[vertex];
            for (int i = 0; i < Size; i++)
            {
                if (_matrix[vertexId, i] == 1)
                {
                    yield return _nameToId.First(x => x.Value == i).Key;
                }
            }
        }

        public bool IsClique(IReadOnlyList<string> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = i + 1; j < vertices.Count; j++)
                {
                    if (!HasEdge(vertices[i], vertices[j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public HashSet<string> MaxClique()
        {
            var vertices = Vertices.ToArray();
            var maxClique = new HashSet<string>();
            for (int i = 0; i < vertices.Length; i++)
            {
                var clique = new HashSet<string> { vertices[i] };
                for (int j = 0; j < vertices.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (IsClique(clique.Union([vertices[j]]).ToArray()))
                    {
                        clique.Add(vertices[j]);
                    }
                }
                if (clique.Count > maxClique.Count)
                {
                    maxClique = clique;
                }
            }
            return maxClique;
        }
    }

    static AdjacencyMatrixGraph ParseNetworkMap(string input)
    {
        var computerPairs = input.Split('\n').Select(line => line.Split("-"));
        var uniqueComputers = computerPairs.SelectMany(pair => pair).Distinct();
        var graph = new AdjacencyMatrixGraph(uniqueComputers.Count());
        foreach (var pair in computerPairs)
        {
            graph.AddEdge(pair[0], pair[1]);
        }
        return graph;
    }

    class FrozenSetContentEqualityComparer<T> : IEqualityComparer<FrozenSet<T>>
    {
        public bool Equals(FrozenSet<T>? x, FrozenSet<T>? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.SetEquals(y);
        }

        public int GetHashCode(FrozenSet<T> obj)
        {
            return obj.Aggregate(0, (acc, v) => v != null ? acc ^ v.GetHashCode() : acc);
        }
    }

    public object PartOne(string input)
    {
        var graph = ParseNetworkMap(input);
        FrozenSetContentEqualityComparer<string> comparer = new();
        var groups = new HashSet<FrozenSet<string>>(comparer);
        foreach (var vertex in graph.Vertices)
        {
            var connections = graph.GetAdjacentVertices(vertex).ToArray();
            for (int i = 0; i < connections.Length; i++)
            {
                for (int j = i + 1; j < connections.Length; j++)
                {
                    if (graph.HasEdge(connections[i], connections[j]))
                    {
                        var group = new HashSet<string>
                        {
                            vertex,
                            connections[i],
                            connections[j],
                        }.ToFrozenSet();
                        groups.Add(group);
                    }
                }
            }
        }
        var filteredGroups = groups.Where(group => group.Any(v => v.StartsWith('t')));
        return filteredGroups.Count();
    }

    public object PartTwo(string input)
    {
        var graph = ParseNetworkMap(input);
        var maxClique = graph.MaxClique();
        return string.Join(',', maxClique.Order());
    }
}
