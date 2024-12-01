using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class delaunayTriangulation {
    public class Triangle : IEquatable<Triangle> {                  // IEquatable used so I can compare triangle objects in a specific way
        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }
        
        public bool IsBad { get; set; }

        public Triangle() {

        }

        public Triangle(Vertex a, Vertex b, Vertex c) {             // Create a triangle object with 3 arguments
            A = a;
            B = b;
            C = c;
        }

        public bool containsVertex(Vector3 v) {                     // Check if a given vector is a vertex (approximately)
            return Vector3.Distance(v, A.Position) < 0.01f
                || Vector3.Distance(v, B.Position) < 0.01f
                || Vector3.Distance(v, C.Position) < 0.01f;
        }

        public bool circumCircleContains(Vector3 v) {
            Vector3 a = A.Position;                                 // Get the vector3 position of the vertexes
            Vector3 b = B.Position;
            Vector3 c = C.Position;

            float ab = a.sqrMagnitude;                              // sqrMagnitude is used instead of calculating the magnitude as it is faster (no usage of square root function)
            float cd = b.sqrMagnitude;                              // square of magnitude can be used to compare magnitudes as they shouldn't be negative anyways
            float ef = c.sqrMagnitude;

            // Generate x and y for circumcircle radius
            float circumX = (ab * (c.y - b.y) + cd * (a.y - c.y) + ef * (b.y - a.y)) / (a.x * (c.y - b.y) + b.x * (a.y - c.y) + c.x * (b.y - a.y));
            float circumY = (ab * (c.x - b.x) + cd * (a.x - c.x) + ef * (b.x - a.x)) / (a.y * (c.x - b.x) + b.y * (a.x - c.x) + c.y * (b.x - a.x));

            Vector3 circum = new Vector3(circumX / 2, circumY / 2); // Generate vector for circumcircle radius calculation
            float circumRadius = Vector3.SqrMagnitude(a - circum);  // Get radius for circumcircle
            float dist = Vector3.SqrMagnitude(v - circum);
            return dist <= circumRadius;                            // If vector distance from circumcircle centre <= circumcircle radius, it is inside the circumcircle
        }

        // Check if 2 triangles are the same by comparing all of the vertexes
        public static bool operator ==(Triangle left, Triangle right) {
            return (left.A == right.A || left.A == right.B || left.A == right.C)
                && (left.B == right.A || left.B == right.B || left.B == right.C)
                && (left.C == right.A || left.C == right.B || left.C == right.C);
        }

        public static bool operator !=(Triangle left, Triangle right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Triangle t) {
                return this == t;
            }

            return false;
        }

        public bool Equals(Triangle t) {
            return this == t;
        }

        // Needed for HashSet
        public override int GetHashCode() {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }
    }

    public class Edge {
        public Vertex U { get; set; }
        public Vertex V { get; set; }
        public bool IsBad { get; set; }

        public Edge() {

        }

        public Edge(Vertex u, Vertex v) {
            U = u;
            V = v;
        }

        // Check 2 Edges are the same if their U and V vectors are the same
        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U || left.U == right.V)
                && (left.V == right.U || left.V == right.V);
        }

        // If they are not the same, then they are inequal
        public static bool operator !=(Edge left, Edge right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Edge e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }

        // Use the approximate method on their U and V vectors to see if they are almost equal
        public static bool approximate(Edge left, Edge right) {
            return delaunayTriangulation.approximate(left.U, right.U) && delaunayTriangulation.approximate(left.V, right.V)
                || delaunayTriangulation.approximate(left.U, right.V) && delaunayTriangulation.approximate(left.V, right.U);
        }
    }

    // Approximates 2 float values to see if they are nearly the same
    // -- float.Epsilon is the smallest possible float value
    static bool approximate(float x, float y) {
        return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2 
            || Mathf.Abs(x - y) < float.MinValue;
    }

    // Approximate 2 vertexes, check if they are nearly the same
    static bool approximate(Vertex left, Vertex right) {
        return approximate(left.Position.x, right.Position.x) && approximate(left.Position.y, right.Position.y);
    }

    public List<Vertex> Vertices { get; private set; }
    public List<Edge> Edges { get; private set; }
    public List<Triangle> Triangles { get; private set; }

    // Initialise Edges and Triangles
    delaunayTriangulation() {
        Edges = new List<Edge>();
        Triangles = new List<Triangle>();
    }

    public static delaunayTriangulation Triangulate(List<Vertex> vertices) {
        delaunayTriangulation delaunay = new delaunayTriangulation();
        delaunay.Vertices = new List<Vertex>(vertices);
        delaunay.Triangulate();

        return delaunay;
    }

    void Triangulate() {
        float minX = Vertices[0].Position.x;
        float minY = Vertices[0].Position.y;
        float maxX = minX;
        float maxY = minY;
        
        // Find the largest and smallest vertexes in the x and y positions.
        foreach (var vertex in Vertices) {
            if (vertex.Position.x < minX) minX = vertex.Position.x;
            if (vertex.Position.x > maxX) maxX = vertex.Position.x;
            if (vertex.Position.y < minY) minY = vertex.Position.y;
            if (vertex.Position.y > maxY) maxY = vertex.Position.y;
        }

        // Difference between the smallest and largest values
        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy) * 2;

        Vertex p1 = new Vertex(new Vector2(minX - 1         , minY - 1          ));
        Vertex p2 = new Vertex(new Vector2(minX - 1         , maxY + deltaMax   ));
        Vertex p3 = new Vertex(new Vector2(maxX + deltaMax  , minY - 1          ));

        Triangles.Add(new (p1, p2, p3));

        // Triangulation using the triangles, creating edges which connect the points together
        foreach (var vertex in Vertices) {
            List<Edge> polygon = new List<Edge>();

            foreach (var t in Triangles) {
                if (t.circumCircleContains(vertex.Position)) {
                    t.IsBad = true;                                     // If the triangle's circumcircle contains the vertex, then it shouldn't be used in next iteration
                    polygon.Add(new Edge(t.A, t.B));
                    polygon.Add(new Edge(t.B, t.C));
                    polygon.Add(new Edge(t.C, t.A));
                }
            }

            Triangles.RemoveAll((Triangle t) => t.IsBad);

            for (int i = 0; i < polygon.Count; i++) {
                for (int j = i + 1; j < polygon.Count; j++) {           // Compare all edges
                    if (Edge.approximate(polygon[i], polygon[j])) {     // If the edges are approximately the same, delete both
                        polygon[i].IsBad = true;
                        polygon[j].IsBad = true;
                    }
                }
            }

            polygon.RemoveAll((Edge e) => e.IsBad);

            foreach (var edge in polygon) {
                Triangles.Add(new Triangle(edge.U, edge.V, vertex));    // Create new triangles with the new edges
            }
        }

        Triangles.RemoveAll((Triangle t) => t.containsVertex(p1.Position) || t.containsVertex(p2.Position) || t.containsVertex(p3.Position));

        HashSet<Edge> edgeSet = new HashSet<Edge>();

        // Add every edge of the remaining triangles to the edgeSet.
        foreach (var t in Triangles) {
            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);

            if (edgeSet.Add(ab)) {                                      // These edges will be from triangles which have none of the points
                Edges.Add(ab);                                          // inside of their circumcircles.
            }                                                           // This means that all of the points are joined by one line only.

            if (edgeSet.Add(bc)) {
                Edges.Add(bc);
            }

            if (edgeSet.Add(ca)) {
                Edges.Add(ca);
            }
        }
    }
}
