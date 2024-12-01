using System;
using System.Collections.Generic;
using UnityEngine;
using BlueRaja;

public class pathfinder {

    public class Node {                                                 // A node is just a point on the grid
        public Vector2Int Position { get; private set; }
        public Node Previous { get; set; }                              // For pathfinding, the previous node needs to be stored in the node
        public float Cost { get; set; }                                 // Cost is a variable which defines which paths to a node should be favoured

        public Node(Vector2Int position) {                              // Constructor method
            Position = position;
        }
    }

    public struct PathCost {                                            // Struct used to save memory
        public bool traversable;
        public float cost;
    }

    static readonly Vector2Int[] neighbors = {                          // This array makes it easier to define which vectors are the neighbour of a specific vector when pathfinding
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };

    grid<Node> nodes;
    SimplePriorityQueue<Node, float> queue;
    HashSet<Node> closed;
    Stack<Vector2Int> stack;

    public pathfinder(Vector2Int size) {
        nodes = new grid<Node>(size, Vector2Int.zero);

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();
        stack = new Stack<Vector2Int>();

        for (int x = 0; x < size.x; x++) {                              // Iterate through the columns of the grid
            for (int y = 0; y < size.y; y++) {                          // Iterate through the rows of the grid
                    nodes[x, y] = new Node(new Vector2Int(x, y));       // Create a node in each spot in the grid
            }
        }
    }

    void ResetNodes() {
        var size = nodes.Size;

        // Iterate  through every node in the grid
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                var node = nodes[x, y];
                node.Previous = null;                                   // No nodes should have any routes to it yet
                node.Cost = float.PositiveInfinity;                     // The default state of node costs should always be positive infinity
            }
        }
    }

    // Using a delegate for the cost function here as I think it will be easier if I was able to switch between pathfinding algorithms
    // For example, I can easily switch between a* and djikstra algorithm and see which provides better results
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, Func<Node, Node, PathCost> costFunction) {
        ResetNodes();
        queue.Clear();
        closed.Clear();

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();

        nodes[start].Cost = 0;                                          // The first node should always have no cost (it needs to be first)
        queue.Enqueue(nodes[start], 0);                                 // Enter the start node with the lowest priority (so it comes out first)

        while (queue.Count > 0) {
            Node node = queue.Dequeue();
            closed.Add(node);                                           // Signifies that the node has already been visited

            if (node.Position == end) {                                 // A path has been found
                return ReconstructPath(node);                           // Technically, the end node could be checked too, but would take up more time
            }                                                           // and finding the absolute fastest path isnt a priority

            foreach (var offset in neighbors) {                         // Check all neighbours of a node
                if (!nodes.InBounds(node.Position + offset)) continue;  // Check if the neighbour being checked is in the grid
                var neighbor = nodes[node.Position + offset];
                if (closed.Contains(neighbor)) continue;                // Check if the neighbour has already been visited

                var pathCost = costFunction(node, neighbor);            // Generate a cost for the path between the current node and its neighbours
                if (!pathCost.traversable) continue;                    // If the path cost function isnt traversable, ignore the next part (skip the current iteration)

                float newCost = node.Cost + pathCost.cost;

                if (newCost < neighbor.Cost) {                          // If the new cost is less than the neighbour's stored cost, update it with the new 
                    neighbor.Previous = node;                           // lower cost and make the current node the path to the neighbour node
                    neighbor.Cost = newCost;

                    // If the node exists in the priority queue, update the priority of it, if not, add it to the queue
                    if (queue.TryGetPriority(node, out float existingPriority)) {
                        queue.UpdatePriority(node, newCost);
                    } else {
                        queue.Enqueue(neighbor, neighbor.Cost);
                    }
                }
            }
        }

        return null;
    }

    List<Vector2Int> ReconstructPath(Node node) {
        List<Vector2Int> result = new List<Vector2Int>();

        while (node != null) {                                          // Go backwards through the path, and add each vector to a stack
            stack.Push(node.Position);                                  
            node = node.Previous;                                       // -- The only node that should be passed through this is the end node
        }                                                               // -- The only node which has null as it's previous node should be the root

        while (stack.Count > 0) {
            result.Add(stack.Pop());                                    // Turn the stack into a list of vectors (FIFO)
        }

        return result;
    }
}
