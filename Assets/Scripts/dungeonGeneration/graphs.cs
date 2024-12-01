using System;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs {
    public class Vertex : IEquatable<Vertex> {          // IEquatable used so I can overwrite comparison methods
        public Vector3 Position { get; private set; }   // Position shouldnt be editable by external objects

        public Vertex() {

        }

        public Vertex(Vector3 position) {
            Position = position;
        }

        public override bool Equals(object obj) {
            if (obj is Vertex v) {
                return Position == v.Position;
            }

            return false;
        }

        public bool Equals(Vertex other) {
            return Position == other.Position;          // Equate using position vectors
        }

        public override int GetHashCode() {
            return Position.GetHashCode();              // Hash code should be calculated from position rather than from the whole object
        }
    }

    public class Vertex<T> : Vertex {
        public T Item { get; private set; }             // Items need to be able to be stored within vertexes so that a room can be
                                                        // associated with them during the generation process
        public Vertex(T item) {                         // This means we can have a vertex represent a room and calculate a start
            Item = item;                                // and end of the dungeon
        }                                               // It will also be easier to attach a room onto each vertex

        public Vertex(Vector3 position, T item) : base(position) {
            Item = item;
        }
    }

    public class Edge : IEquatable<Edge> {
        public Vertex U { get; set; }                   // An edge of a shape is basically just 2 vertexes joined together
        public Vertex V { get; set; }                   // no need to actually calculate a line between the vertexes

        public Edge() {

        }

        public Edge(Vertex u, Vertex v) {
            U = u;
            V = v;
        }

        // Compare edges by comparing the vertexes which make them
        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U || left.U == right.V)
                && (left.V == right.U || left.V == right.V);
        }

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

        // Needed to be able to put these into a HashSet
        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }
    }
}