using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid<T> {
    T[] data;

    public Vector2Int Size { get; private set; }
    public Vector2Int Offset { get; set; }

    // Create a grid with specific size (stored as a vector for the x - wdith and y - heigh components)
    public grid(Vector2Int size, Vector2Int offset) {
        Size = size;
        Offset = offset;                                                    // Offset may not be needed, but added just in case

        data = new T[size.x * size.y];                                      // Create an array with a length which is the area of the grid
    }

    public int GetIndex(Vector2Int pos) {
        return pos.x + (Size.x * pos.y);                                    // Find a certain item in the grid
    }

    public bool InBounds(Vector2Int pos) {                                  // Check if item is inside the area of the grid
        return new RectInt(Vector2Int.zero, Size).Contains(pos + Offset);   // Create a 2D representation of the grid to check if it is inside 
    }

    // This makes the index of items in the grid accessible with square brackets, turning the (x, y) format into a vector before searching the array
    public T this[int x, int y] {
        get {
            return this[new Vector2Int(x, y)];
        }
        set {
            this[new Vector2Int(x, y)] = value;                             // Use the method below to get the index of an item in the grid
        }
    }

    public T this[Vector2Int pos] {                                         // Get the index of an item in the grid using a vectior argument
        get {
            pos += Offset;
            return data[GetIndex(pos)];
        }
        set {
            pos += Offset;
            data[GetIndex(pos)] = value;
        }
    }
}