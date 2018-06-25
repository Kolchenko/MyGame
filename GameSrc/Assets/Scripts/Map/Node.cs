using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> neighbours;
    public int x;
    public int y;

    public Node()
    {
        neighbours = new List<Node>();
    }

    public Node(Node node)
    {
        if (node != null)
        {
            neighbours = new List<Node>(node.neighbours);
            x = node.x;
            y = node.y;
        }
    }

    public float DistanceTo(Node node)
    {
        return Vector2.Distance(
            new Vector2(x, y),
            new Vector2(node.x, node.y)
            );
    }

    public int DistanceBetweenNode(Node node)
    {
        int x0 = x - Mathf.FloorToInt(y / 2);
        int y0 = y;
        int x1 = node.x - Mathf.FloorToInt(node.y / 2);
        int y1 = node.y;
        int dx = x1 - x0;
        int dy = y1 - y0;

        return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy), Mathf.Abs(dx + dy));
    }
}
