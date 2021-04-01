using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Essentials.Algorithms.SugiyamaGraphs
{
    public class SugiyamaGraph<TNode,TConnection> where TConnection : IConnection<TNode>, IComparable where TNode : INode, IComparable
    {
        private static bool DebugOutput = false;
        
        public TNode[] Nodes { get; }
        public TConnection[] Connections { get; }
        private float EntrySize { get; }
        
        private readonly List<Layer> Layers = new List<Layer>();
        
        public SugiyamaGraph(TNode[] nodes, TConnection[] connections, float entrySize)
        {
            Nodes = nodes;
            Connections = connections;
            EntrySize = entrySize;
        }

        public bool CalculateLayout()
        {
            Layers.Clear();
            if (!CreateAcyclicDirectedGraph()) return false;
            if (!InsertIntermediateConnectionEntries()) return false;
            if (!MinimizeCrossings()) return false;
            if (!ArrangePositions()) return false;
            if (!CenterNodes()) return false;
            return true;
        }

        /// <summary>
        /// See step 1.2 of Sugiyama Algorithm
        /// Put nodes into layered hierarchy
        /// </summary>
        /// <returns><code>true</code> if successful; otherwise <code>false</code></returns>
        private bool CreateAcyclicDirectedGraph()
        {
            Print("1.1: Create Acyclic Directed Graph");
            var queue = Connections.Select(c => c.To).Distinct().ToList();
            var rootNodes = Nodes.Except(queue).ToArray();
            var done = rootNodes.ToList();
            var firstLayer = GetLayer(0);
            if (rootNodes.Length > 0)
            {
                foreach (var node in rootNodes)
                {
                    firstLayer.AddEntry(node);
                }
                Print("Found "+rootNodes.Length+" root nodes.");
            }
            else
            {
                PrintWarning("Sugiyama Graph Warning: Supplied Connections are cyclic");
                return false;
            }

            var antiDeadlock = 0;
            const int maxCycles = 1000;
            var currentLayer = firstLayer;
            var queuedConnections = new List<TConnection>();
            
            while (queue.Count > 0 && antiDeadlock<maxCycles)
            {
                antiDeadlock++;
                // get current nodes
                var currentNodes = currentLayer.Entries.OfType<NodeEntry>();
                // get all connections going out of those nodes
                var rightConnections = Connections.Where(c => currentNodes.Any(n => n.Node.CompareTo(c.From) == 0))
                    .Concat(queuedConnections).Distinct().ToArray();
                // get all nodes attached to these connections
                var connectedNodes = rightConnections.Select(c => c.To).ToArray();
                // get all connections to those nodes
                var connectedConnections = Connections.Where(c => connectedNodes.Any(n=>c.To.CompareTo(n) == 0)).ToArray();
                // filter out all connections that origin from pending nodes
                var invalidConnections = connectedConnections.Where(c => queue.Any(n => c.From.CompareTo(n) == 0)).ToArray();
                queuedConnections.Clear();
                queuedConnections.AddRange(invalidConnections);
                // filter out all nodes that have input connections from nodes in future layers
                var invalidNodes = connectedNodes.Where(n=>invalidConnections.Any(c => c.To.CompareTo(n)==0));
                var validNodes = connectedNodes.Except(invalidNodes).ToArray();
                if (validNodes.Length == 0)
                {
                    PrintWarning("Loop aborted; Could not find any valid nodes. \n" +
                                 "Remaining:\n" + string.Join("\n", queue.Select(n => n.ToString())) + "\n" +
                                 "Connections:\n" + string.Join("\n",
                                     Connections.Where(c =>
                                         queue.Any(n => n.CompareTo(c.From) == 0 || n.CompareTo(c.To) == 0))));
                    return false;
                }
                var layer = GetLayer(currentLayer.Position + 1);
                foreach (var node in validNodes)
                {
                    layer.AddEntry(node);
                    queue.Remove(node);
                    done.Add(node);
                }

                foreach (var connection in rightConnections.Except(invalidConnections))
                {
                    var previous = currentLayer.GetEntry(connection.From);
                    var next = layer.GetEntry(connection.To);
                    if (previous == null || next == null) continue;
                    previous.Next.Add(next);
                    next.Previous.Add(previous);
                }
                
                currentLayer = layer;
            }

            return true;
        }

        /// <summary>
        /// See step 1.3 of Sugiyama Algorithm
        /// Insert dummy entries for connections that span multiple layers so that every node only connects to
        /// entries within the previous and the next layer
        /// </summary>
        /// <returns><code>true</code> if successful; otherwise <code>false</code></returns>
        private bool InsertIntermediateConnectionEntries()
        {
            foreach (var layer in Layers)
            {
                var nodes = layer.Entries.OfType<NodeEntry>();
                var connectionsToLayer = Connections.Where(c => nodes.Any(n => c.To.CompareTo(n.Node) == 0));
                foreach (var connection in connectionsToLayer)
                {
                    var startLayer = Layers.First(l => l.Contains(connection.From));
                    var layerDistance = layer.Position - startLayer.Position;
                    if (layerDistance <= 1)
                    {
                        continue;
                    }
                    var startEntry = startLayer.GetEntry(connection.From);
                    LayerEntry previousNode = startEntry;
                    for (var i = 1; i < layerDistance; i++)
                    {
                        var intermediateLayer = GetLayer(startLayer.Position + i);
                        var intermediateEntry = intermediateLayer.AddEntry(connection);
                        previousNode.Next.Add(intermediateEntry);
                        intermediateEntry.Previous.Add(previousNode);
                        previousNode = intermediateEntry;
                    }
                    var endEntry = layer.GetEntry(connection.To);
                    previousNode.Next.Add(endEntry);
                    endEntry.Previous.Add(previousNode);
                }
            }

            var maxEntryCount = Layers.Max(l => l.EntryCount);
            foreach (var layer in Layers)
            {
                for (var i = layer.EntryCount; i < maxEntryCount; i++)
                {
                    layer.AddDummy();
                }
            }

            return true;
        }

        /// <summary>
        /// See step 2 of Sugiyama Algorithm
        /// Minimize intersections of connections by reordering entries within the layers
        /// </summary>
        /// <returns></returns>
        private bool MinimizeCrossings()
        {
            var layers = Layers;
            foreach (var layer in layers)
            {
                if (layer.Position <= 0)
                {
                    ApplyEntryOrder(layer.IndexedEntries);
                    continue;
                }
                
                if (!MinimizeLeftCrossings(layer)) return false;
            }

            foreach (var layer in layers.Select(l=>l).Reverse())
            {
                if (!MinimizeRightCrossings(layer)) return false;
            }

            return true;
        }

        /// <summary>
        /// See step 3 of Sugiyama Algorithm
        /// Assign positions for all nodes
        /// </summary>
        /// <returns></returns>
        private bool ArrangePositions()
        {
            for (var i = 0; i < Layers.Count; i++)
            {
                if (!ArrangePositions(GetLayer(i))) return false;
            }

            return true;
        }

        /// <summary>
        /// This is an ugly simplification of step 4 from the Sugiyama Algorithm
        /// Definitely improve when time is available
        /// TODO do that properly
        /// </summary>
        /// <returns></returns>
        private bool CenterNodes()
        {
            foreach (var layer in Layers)
            {
                layer.RemoveDummies();
            }
            var height = Layers.SelectMany(l => l.Entries.Select(e => e.Position + EntrySize / 2)).DefaultIfEmpty(1)
                .Max();
            foreach (var layer in Layers)
            {
                var entries = layer.Entries;
                var layerMin = entries.Select(e => e.Position - EntrySize / 2).DefaultIfEmpty(0).Min();
                var layerMax = entries.Select(e => e.Position + EntrySize / 2).DefaultIfEmpty(1).Max();
                var bottomMargin = (height - (layerMax-layerMin)) / 2;
                var offset = bottomMargin - layerMin;
                foreach (var entry in entries)
                {
                    entry.Position += offset;
                }
            }

            return true;
        }

        private bool MinimizeLeftCrossings(Layer layer)
        {
            var entries = layer.Entries;
            if (entries.Length == 0) return true;
            var indexedEntries = layer.IndexedEntries;

            foreach (var entry in entries)
            {
                indexedEntries[entry] = entry.Previous.Select(p => p.Position).DefaultIfEmpty(0).Average(p => p);
            }

            if (!MinimizeCrossings(layer, entries, indexedEntries, CountLeftIntersections)) return false;
            return true;
        }

        private bool MinimizeRightCrossings(Layer layer)
        {
            var entries = layer.Entries;
            if (entries.Length == 0) return true;
            var indexedEntries = entries.ToDictionary(e=>e,e=>e.Position);

            if (!MinimizeCrossings(layer, entries, indexedEntries, CountRightIntersections)) return false;
            return true;
        }

        private bool MinimizeCrossings(Layer current, LayerEntry[] entries, Dictionary<LayerEntry,float> indexedEntries, Func<Layer,Dictionary<LayerEntry,float>,int> intersectionCounter)
        {
            
            var intersectionCount = intersectionCounter(current, indexedEntries);
            Print(current.Position + ": Counted " + intersectionCount + " Intersections");

            if (intersectionCount == 0)
            {
                ApplyEntryOrder(indexedEntries);
                return true;
            }

            for (var iteration = 0; iteration < 3 && intersectionCount > 0; iteration++)
            {
                for (var i = 0; i < entries.Length - 1 && intersectionCount > 0; i++)
                {
                    var entry = entries[i];
                    var entryPosition = indexedEntries[entry];
                    for (var j = i + 1; j < entries.Length && intersectionCount > 0; j++)
                    {
                        var otherEntry = entries[j];
                        var otherEntryPosition = indexedEntries[otherEntry];
                        indexedEntries[entry] = otherEntryPosition;
                        indexedEntries[otherEntry] = entryPosition;
                        var newIntersectionCount = intersectionCounter(current, indexedEntries);
                        if (newIntersectionCount >= intersectionCount)
                        {
                            indexedEntries[entry] = entryPosition;
                            indexedEntries[otherEntry] = otherEntryPosition;
                            continue;
                        }

                        intersectionCount = newIntersectionCount;

                        break;
                    }
                }
            }

            ApplyEntryOrder(indexedEntries);

            return true;
        }

        private void ApplyEntryOrder(Dictionary<LayerEntry, float> indexedEntries)
        {
            var position = 0f;
            var entries = indexedEntries.OrderBy(e => e.Value).Select(e => e.Key).ToArray();
            foreach (var entry in entries)
            {
                entry.Position = position + EntrySize / 2;
                position += EntrySize;
            }
        }

        private int CountLeftIntersections(Layer current, Dictionary<LayerEntry,float> indexedEntries)
        {
            var edges = indexedEntries.SelectMany(e => e.Key.Previous.Select(p => new Edge(
                    new Vector2(current.Position-1, p.Position), new Vector2(current.Position, e.Value)
                ))).ToArray();

            return CountIntersections(edges);
        }

        private int CountRightIntersections(Layer current, Dictionary<LayerEntry,float> indexedEntries)
        {
            
            var edges = indexedEntries.SelectMany(e => e.Key.Next.Select(n => new Edge(
                new Vector2(current.Position+1, n.Position), new Vector2(current.Position, e.Value)
            ))).ToArray();

            return CountIntersections(edges);
        }

        private int CountIntersections(Edge[] edges)
        {
            var result = 0;
            for (var i = 0; i < edges.Length; i++)
            {
                var edge = edges[i];
                for (var j = i + 1; j < edges.Length; j++)
                {
                    var otherEdge = edges[j];
                    var intersection = MathUtil.DoIntersect(edge.a, edge.b, otherEdge.a, otherEdge.b, allowEdgeCases:false);
                    if (!intersection) continue;
                    result++;
                }
            }

            return result;
        }

        private bool ArrangePositions(Layer current)
        {
            var position = 0f;
            var entries = current.Entries.OrderBy(e => e.Position).ToArray();
            // simply stack entries in this iteration
            foreach (var entry in entries)
            {
                entry.Position = position+EntrySize / 2;
                position += EntrySize;
            }

            return true;
        }

        private TConnection[] GetLeftConnections(IEnumerable<TConnection> connections, LayerEntry layerEntry)
        {
            return layerEntry is NodeEntry nodeEntry
                ? connections.Where(c => c.To.CompareTo(nodeEntry) == 0).ToArray()
                : (layerEntry is ConnectionEntry connectionEntry ? new[] {connectionEntry.Connection} : null);
        }

        private LayerEntry[] GetConnectedEntries(IEnumerable<LayerEntry> entries, IEnumerable<TConnection> connections)
        {
            return entries.Where(e => e is NodeEntry nodeEntry
                ? connections.Any(c => c.From.CompareTo(nodeEntry) == 0)
                : e is ConnectionEntry connectionEntry &&
                  connections.Any(c => c.CompareTo(connectionEntry.Connection) == 0)).ToArray();
        }
        
        public Dictionary<TNode, Vector2> GetNodePositions()
        {
            var maxLayerHeight = Layers.SelectMany(l => l.Entries.Select(e => e.Position + EntrySize / 2))
                .DefaultIfEmpty(0).Max();
            if (maxLayerHeight <= 0) maxLayerHeight = 1;
            var maxLayerIndex = (float) Layers.Where(l=>l.EntryCount>0).Select(l => l.Position).DefaultIfEmpty(0).Max()+1;
            return Layers.SelectMany(l => l.Entries.OfType<NodeEntry>().Select(e =>
                    new KeyValuePair<TNode, Vector2>(e.Node,
                        new Vector2((l.Position + 0.5f) / maxLayerIndex, e.Position / maxLayerHeight))))
                .ToDictionary(e => e.Key, e => e.Value);
        }

        private Layer GetLayer(int position)
        {
            var existing = Layers.FirstOrDefault(l => l.Position == position);
            if (existing != null) return existing;
            if (position < 0)
            {
                foreach (var layer in Layers)
                {
                    layer.Position += 1;
                }

                position = 0;
            }
            var result = new Layer(){Position = position};
            Layers.Add(result);
            return result;
        }

        private class Layer
        {
            /// <summary>
            /// Horizontal position in graph
            /// </summary>
            public int Position { get; set; }
            public LayerEntry[] Entries => _entries.ToArray();
            public int EntryCount => _entries.Count;

            public Dictionary<LayerEntry, float> IndexedEntries
            {
                get
                {
                    var result = new Dictionary<LayerEntry,float>();
                    var sortedEntries = _entries.OrderBy(e => e.Position).ToArray();
                    for(var i = 0; i < sortedEntries.Length; i++) result.Add(sortedEntries[i], i);
                    return result;
                }
            }

            private readonly List<LayerEntry> _entries = new List<LayerEntry>();

            public NodeEntry AddEntry(TNode node)
            {
                var result = new NodeEntry(node);
                _entries.Add(result);
                return result;
            }

            public ConnectionEntry AddEntry(TConnection connection)
            {
                var result = new ConnectionEntry(connection);
                _entries.Add(result);
                return result;
            }

            public DummyEntry AddDummy()
            {
                var result = new DummyEntry();
                _entries.Add(result);
                return result;
            }

            public void RemoveDummies()
            {
                foreach (var entry in _entries.OfType<DummyEntry>().ToArray())
                {
                    _entries.Remove(entry);
                }
            }

            public NodeEntry GetEntry(TNode node)
            {
                return _entries.OfType<NodeEntry>().FirstOrDefault(e => e.Node.CompareTo(node) == 0);
            }

            public ConnectionEntry GetEntry(TConnection connection)
            {
                return _entries.OfType<ConnectionEntry>().FirstOrDefault(e => e.Connection.CompareTo(connection) == 0);
            }

            public void RemoveEntry(TNode node)
            {
                var entry = _entries.OfType<NodeEntry>().FirstOrDefault(e => e.Node.CompareTo(node)==0);
                if (entry == null) return;
                _entries.Remove(entry);
            }

            public bool Contains(TNode node)
            {
                return Entries.OfType<NodeEntry>().Any(e => e.Node.CompareTo(node) == 0);
            }
        }

        private abstract class LayerEntry
        {
            /// <summary>
            /// Vertical position inside layer
            /// </summary>
            public float Position { get; set; }
            
            public readonly List<LayerEntry> Next = new List<LayerEntry>();
            public readonly List<LayerEntry> Previous = new List<LayerEntry>();
        }

        private class NodeEntry : LayerEntry
        {
            public TNode Node { get; }

            public NodeEntry(TNode node)
            {
                Node = node;
            }

            public override string ToString()
            {
                return Node.ToString();
            }
        }

        private class ConnectionEntry : LayerEntry
        {
            public TConnection Connection { get; }
            
            public ConnectionEntry(TConnection connection)
            {
                Connection = connection;
            }

            public override string ToString()
            {
                return Connection.ToString();
            }
        }

        private class DummyEntry : LayerEntry
        {

            public override string ToString()
            {
                return "dummy";
            }
        }

        private struct Edge
        {
            public Vector2 a;
            public Vector2 b;

            public Edge(Vector2 a, Vector2 b)
            {
                this.a = a;
                this.b = b;
            }
        }

        private static void Print(object obj)
        {
            if (!DebugOutput) return;
            Debug.Log("<b>Sugiyama Graph</b> "+obj);
        }

        private static void PrintWarning(object obj)
        {
            if (!DebugOutput) return;
            Debug.LogWarning("<b>Sugiyama Graph</b> "+obj);
        }

        private static void PrintError(object obj)
        {
            if (!DebugOutput) return;
            Debug.LogError("<b>Sugiyama Graph</b> "+obj);
        }
    }
}