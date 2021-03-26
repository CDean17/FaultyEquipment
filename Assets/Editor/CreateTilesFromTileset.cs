using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;
public class CreateTilesFromTileset : EditorWindow
{
    int padding = 5;
    int buttonHeight = 25;
    Texture2D texture2D;
    GameObject baseTilemapObject;
    Tile tile;
    List<Tile> tiles;
    public TextAsset jsonFile;

    Grid grid;

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
    [MenuItem("Tools/PyxelEdit/Import Tilemap Map Data")]
    static void Init()
    {
        CreateTilesFromTileset window = ScriptableObject.CreateInstance<CreateTilesFromTileset>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 300);
        window.ShowPopup();
    }

    void OnGUI()
    {
        // Create generic base tile if none provided
        if (tile == null)
        {
            tile = new Tile();
        }

        if (grid != null)
        {
            baseTilemapObject = grid.gameObject.transform.GetChild(0).gameObject;
        }


        EditorGUI.DropShadowLabel(new Rect(0, 0, position.width, 20),
        "Create Tiles From Texture.");

        texture2D = (Texture2D)EditorGUI.ObjectField(
            new Rect(0 + padding, 30 + padding, position.width - padding * 2, 25),
            "Tilemap as Texture2D: ",
            texture2D,
            typeof(Texture2D),
            true
        );

        jsonFile = (TextAsset)EditorGUI.ObjectField(
            new Rect(0 + padding, 60 + padding, position.width - padding * 2, 25),
            "Tilemap JSON Data: ",
            jsonFile,
            typeof(TextAsset),
            true
        );

        grid = (Grid)EditorGUI.ObjectField(
            new Rect(0 + padding, 80 + padding, position.width - padding * 2, 25),
            "Grid Target: ",
            grid,
            typeof(Grid),
            true
        );

        bool isAccepted = GUI.Button(new Rect(0 + padding, (120 + padding), position.width - padding * 2, buttonHeight), "Go!");

        bool isCanceled = GUI.Button(new Rect(0 + padding, (120 + buttonHeight + padding + padding), position.width - padding * 2, buttonHeight), "Cancel");

        if (isAccepted)
        {
            Debug.Log("Building Tiles");
            if (texture2D != null)
            {
                tiles = BuildTiles();
                AddTilesToTilemap(grid);
                Debug.Log("Loaded sprites as tiles!");
                this.Close();
            }
        }

        if (isCanceled)
        {
            Debug.Log("Canceled");
            this.Close();
        }
    }

    List<Tile> BuildTiles()
    {
        string path = AssetDatabase.GetAssetPath(texture2D);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Tile> tiles = new List<Tile>();
        string fileExt = Path.GetExtension(path);
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);
        string fileNameWithExt = fileNameWithoutExt + fileExt;
        string tilesDirPath = path.Replace(fileNameWithExt, fileNameWithoutExt + "_Tiles/");

        // Make tiles dir if it doesnt exist
        if (!Directory.Exists(tilesDirPath))
        {
            Directory.CreateDirectory(tilesDirPath);
        }

        List<Sprite> sprites = new List<Sprite>();

        // Find sprites in assets dir
        foreach (Object o in assets)
        {
            if (o.GetType() == typeof(Sprite))
            {

                sprites.Add((Sprite)o);

            }
        }

        // Sort by name
        sprites.Sort((x, y) =>
        {
            int xInt = int.Parse(x.name.Split('_')[1]);
            int yInt = int.Parse(y.name.Split('_')[1]);
            return xInt.CompareTo(yInt);
        });

        // Sprite nullSprite = sprites[0];
        // Padding list to allow correct indices
        // sprites.Insert(0, nullSprite);

        foreach (Sprite s in sprites)
        {
            Tile t = Tile.CreateInstance<Tile>();
            t.sprite = s;
            string tilePath = tilesDirPath + s.name + ".asset";
            AssetDatabase.CreateAsset(t, tilePath);
            tiles.Add(t);
        }

        // Sort by name
        tiles.Sort((x, y) =>
        {
            int xInt = int.Parse(x.name.Split('_')[1]);
            int yInt = int.Parse(y.name.Split('_')[1]);
            return xInt.CompareTo(yInt);
        });

        return tiles;
    }

    Tilemap CloneTilemap(Grid grid, string name)
    {
        Tilemap tilemap;
        GameObject newTilemapContainer = Instantiate(baseTilemapObject, grid.gameObject.transform);
        tilemap = newTilemapContainer.GetComponent<Tilemap>();
        newTilemapContainer.name = name;
        return tilemap;
    }

    public void AddTilesToTilemap(Grid grid)
    {
        if (grid == null)
        {
            Debug.Log("No grid reference is set!");
            return;
        }
        string jsonString = jsonFile.ToString();
        JTilemapData t = JsonUtility.FromJson<JTilemapData>(jsonString);
        if (t.layers.Count > 0)
        {

            foreach (JTilemapData.Layer layer in t.layers)
            {
                GameObject obj = GameObject.Find(GetGameObjectPath(grid.gameObject) + "/" + layer.name);

                Tilemap tilemap;
                if (obj == null)
                {
                    tilemap = CloneTilemap(grid, layer.name);
                }
                else
                {
                    tilemap = obj.GetComponent<Tilemap>();
                }

                // Clears all tiles
                tilemap.ClearAllTiles();

                foreach (JTilemapData.Tile tile in layer.tiles)
                {
                    if (tile.tile != -1)
                    {
                        // TODO: figure out why the first row is borked
                        int tileIdx = tile.tile - 1;
                        if (tile.tile <= 8)
                        {
                            tileIdx = tile.tile;
                        }
                        if (tileIdx >= tiles.Count || tileIdx < 0)
                        {
                            Debug.Log("Bad tile index: " + tileIdx);
                            Debug.Log("Tile name: " + tile.x + ", " + tile.y);
                        }
                        else
                        {
                            Vector3Int coords = NormalizeCoords(tile, t);
                            Tile tileObj = tiles[tileIdx];
                            tilemap.SetTile(
                                coords, tileObj
                                );
                        }

                    }
                }

            }
        }
    }

    Vector3Int NormalizeCoords(JTilemapData.Tile tile, JTilemapData j)
    {
        int x = tile.x;
        // int y = -tile.y;
        int y = j.tileshigh - tile.y;
        return new Vector3Int(x, y, 0);
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}