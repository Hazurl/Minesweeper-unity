using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperGrid : MonoBehaviour
{
    [SerializeField] private new Camera camera;

    [SerializeField] private Tile tilePrefab;

    private List<Tile> tiles;
    public bool IsGenerated { get; private set; }

    public int TileCount => Size * Size;

    public int BombCount { get; private set; }

    public int Size { get; private set; }

    public void Initialize(int size, float percentageBomb) {
        Size = size;
        BombCount = Mathf.FloorToInt(TileCount * percentageBomb);

        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }

        tiles = new List<Tile>();
        IsGenerated = false;

        for(int y = 0; y < Size; ++y) {
            for(int x = 0; x < Size; ++x) {
                var tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform) as Tile;
                tile.X = x;
                tile.Y = y;
                tile.Content = 0;

                tiles.Add(tile);
            }
        }

        camera.transform.position = new Vector3(Size / 2, Size / 2, camera.transform.position.z);
        camera.orthographicSize = size / 2 + 1;
    }

    public Tile GetTile(int x, int y) {
        if (x < 0 || x >= Size || y < 0 || y >= Size) {
            return null;
        }

        return tiles[x + y*Size];
    }

    public void Generate(int exceptX, int exceptY) {
        IsGenerated = true;

        int bombCount = 0;

        while(bombCount < BombCount) {
            var index = Random.Range(0, tiles.Count);
            var x = index % Size;
            var y = index  / Size;

            var d = Mathf.Max(Mathf.Abs(x - exceptX), Mathf.Abs(y - exceptY));
            
            if (d <= 1) {
                continue;
            }

            if (tiles[index].IsBomb) {
                continue;
            }

            tiles[index].Content = -1;
            ++bombCount;
        }

        for(int y = 0; y < Size; ++y) {
            for(int x = 0; x < Size; ++x) {
                if (!tiles[x + y*Size].IsBomb) {
                    tiles[x + y*Size].Content = CountBombAround(x, y);
                }
            }
        }
    }

    private int CountBombAround(int x, int y) {
        int count = 0;

        for(int dy = -1; dy <= 1; ++dy) {
            if (y + dy < 0 || y + dy >= Size) continue;

            for(int dx = -1; dx <= 1; ++dx) {
                if (x + dx < 0 || x + dx >= Size) continue;
                if (dx == 0 && dy == 0) continue;

                int index = x + dx + (y + dy)*Size;

                if (tiles[index].IsBomb) {
                    ++count;
                }
            }
        }

        return count;
    }

}
