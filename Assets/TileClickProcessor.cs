using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class TileClickProcessor : MonoBehaviour
{

    private new Camera camera;

    private Tile tileClicked;

    [SerializeField] private MinesweeperGrid grid;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Text bombCountText;
    private int countFlag;

    private bool isRevealing = false;
    private bool isGameStarted = false;
    private int tileRevealedCount = 0;

    private void Awake() {
        camera = GetComponent<Camera>();
    }

    public void Initialize() {
        tileRevealedCount = 0;
        isGameStarted = true;
        isRevealing = false;
        tileClicked = null;
        countFlag = 0;
        UpdateBombCountText();
    }

    private void Update() {
        if (isRevealing || !isGameStarted) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            tileClicked = FindTileUnderCursor();

            if (tileClicked && !tileClicked.IsFlag) {
                tileClicked.HandleClickDown();
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            var tile = FindTileUnderCursor();

            if (tile && !tile.IsRevealed) {
                tile.ToggleFlag();
                if (tile.IsFlag) {
                    ++countFlag;
                } else {
                    --countFlag;
                }

                UpdateBombCountText();
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (tileClicked && !tileClicked.IsFlag) {
                tileClicked.HandleClickUp();
                if (!tileClicked.IsRevealed && tileClicked == FindTileUnderCursor()) {
                    if (!grid.IsGenerated) {
                        grid.Generate(tileClicked.X, tileClicked.Y);
                    }
                    StartCoroutine(StartReveal(tileClicked));
                }
                tileClicked = null;
            }
        }
    }

    private void UpdateBombCountText() {
        bombCountText.text = $"{countFlag}/{grid.BombCount}";
    }

    Tile FindTileUnderCursor() {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            var tile = hit.transform.GetComponent<Tile>();
            return tile;
        }

        return null;
    }

    IEnumerator StartReveal(Tile startTile) {
        isRevealing = true;
        var tiles = new List<Tile>();
        var nextTiles = new List<Tile>();
        tiles.Add(startTile);

        var visited = new HashSet<Tile>();
        visited.Add(startTile);

        void visit(Tile tile) {
            if (tile && !tile.IsRevealed && !visited.Contains(tile)) {
                visited.Add(tile);
                nextTiles.Add(tile);
            }
        }

        while(tiles.Count > 0) {
            foreach (var tile in tiles) {
                if (!tile.IsRevealed) {
                    tile.Reveal();
                    if (tile.IsBomb) {
                        RevealAllBombs();
                        gameManager.EndGame(false);
                        isGameStarted = false;
                        isRevealing = false;
                        yield break;
                    } else {
                        ++tileRevealedCount;

                        if (tileRevealedCount == grid.TileCount - grid.BombCount) {
                            gameManager.EndGame(true);
                            isRevealing = false;
                            isGameStarted = false;
                            yield break;
                        }
                    }
                }

                if (tile.Content == 0) {    
                    visit(grid.GetTile(tile.X + 1, tile.Y + 1));
                    visit(grid.GetTile(tile.X + 1, tile.Y    ));
                    visit(grid.GetTile(tile.X + 1, tile.Y - 1));
                    visit(grid.GetTile(tile.X    , tile.Y + 1));
                    visit(grid.GetTile(tile.X    , tile.Y - 1));
                    visit(grid.GetTile(tile.X - 1, tile.Y + 1));
                    visit(grid.GetTile(tile.X - 1, tile.Y    ));
                    visit(grid.GetTile(tile.X - 1, tile.Y - 1));
                }
            }

            tiles = nextTiles;
            nextTiles = new List<Tile>();

            yield return new WaitForSeconds(0.05f);
        }

        isRevealing = false;
    }

    private void RevealAllBombs() {
        for(int y = 0; y < grid.Size; ++y) {
            for(int x = 0; x < grid.Size; ++x) {
                var tile = grid.GetTile(x, y);
                if (tile.IsBomb) {
                    tile.Reveal();
                }
            }
        }
    }



}
