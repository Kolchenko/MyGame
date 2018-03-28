using UnityEngine;

public class TileMap : MonoBehaviour
{
    public TileType[] tileTypes;
    //public Transform hexPrefab;

    public int tileWidth = 11;
    public int tileHeight = 11;

    float hexWidth = 1.732f;
    float hexHeight = 2.0f;
    public float gap = 0.0f;

    Vector3 startPos;

    void Start()
    {
        AddGap();
        CalcStartPos();
        CreateTile();
    }

    void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    void CalcStartPos()
    {
        float offset = 0;
        if (tileHeight / 2 % 2 != 0)
            offset = hexWidth / 2;

        float x = -hexWidth * (tileWidth / 2) - offset;
        float z = hexHeight * 0.75f * (tileHeight / 2);

        startPos = new Vector3(x, 0, z);
    }

    Vector3 CalcWorldPos(Vector2 tilePos)
    {
        float offset = 0;
        if (tilePos.y % 2 != 0)
            offset = hexWidth / 2;

        float x = startPos.x + tilePos.x * hexWidth + offset;
        float z = startPos.z - tilePos.y * hexHeight * 0.75f;

        return new Vector3(x, 0, z);
    }

    void CreateTile()
    {
        for (int y = 0; y < tileHeight; y++)
        {
            for (int x = 0; x < tileWidth; x++)
            {
                //todo: set random tile type
                TileType tt = tileTypes[0];
                Transform hex = Instantiate(tt.tileVisualPrefab.transform) as Transform;
                Vector2 tilePos = new Vector2(x, y);
                hex.position = CalcWorldPos(tilePos);
                hex.parent = this.transform;
                hex.name = "Hexagon" + x + "|" + y;
            }
        }
    }
}