using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapGenerator : MonoBehaviour
{
    public int width, height;
    public GameObject territoryPrefab;
    public Sprite townSprite, waterSprite;
    public float spacing = 1f;

    private Territory[,] grid;
    private List<Territory> landTiles = new List<Territory>();
    public SocialEngine socialEngine;
    public SettlementEngine settlementEngine;

    void Start()
    {
        GenerateTerrain();
        SmoothWater();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].type == TerritoryType.Water)
                {
                    // Either disable them or destroy them if you don't need them
                    Destroy(grid[x, y].gameObject);
                }
            }
        }

        // 1. MUST fill the land list after smoothing is done
        landTiles.Clear();
        foreach (var t in grid) if (t.type == TerritoryType.Town) landTiles.Add(t);

        // 2. Start the hierarchy (Top-Down)
        GenerateImperialHierarchy();

        Object.FindAnyObjectByType<CharacterCreatorUI>().ShowCreator();

        Object.FindAnyObjectByType<MapManager>().UpdateMapVisuals();
    }

    public void FinalizeWorldGeneration(Territory playerCapital)
    {
        List<Territory> kingdomList = new List<Territory>();
        // This finds EVERY Territory component in the map and picks only the Kingdoms
        Territory[] allTerritories = GetComponentsInChildren<Territory>();
        foreach (Territory t in allTerritories)
        {
            if (t.type == TerritoryType.Kingdom)
            {
                kingdomList.Add(t);
            }
        }

        Debug.Log($"SocialEngine check: {socialEngine != null}. Kingdom Count: {kingdomList.Count}. Tile Count: {landTiles.Count}. Now populating world...");

        socialEngine.PopulateWorld(kingdomList, playerCapital);

        Debug.Log($"SocialEngine check: Done populating world. Now assigning capitals.");

        settlementEngine.AssignCapitals(kingdomList);

        Debug.Log($"SocialEngine check: Done assigning capitals. Now assigning sizes and populations.");

        settlementEngine.AssignTerritorySizeAndPopulation(kingdomList);

        settlementEngine.RunInitialEconomySimulation();

        foreach (Territory kingdom in kingdomList)
        {
            kingdom.ownerKingdom.SetUpKingdom(kingdom);
        }

        Debug.Log($"Map generation complete. Updating map visuals.");

        Object.FindAnyObjectByType<MapManager>().UpdateMapVisuals();
    }

    void GenerateImperialHierarchy()
    {
        // Ensure we have land to divide
        if (landTiles.Count == 0) return;

        int numKingdoms = landTiles.Count / 200;
        List<Territory> kingdomCapitals = GetRandomLandTiles(numKingdoms);
        var kingdomBundles = MultiFloodFill(kingdomCapitals, landTiles);

        foreach (var bundle in kingdomBundles)
        {
            // CREATE KINGDOM
            GameObject kObj = new GameObject("Kingdom_Container");
            kObj.transform.SetParent(this.transform);
            Territory kingdom = kObj.AddComponent<Territory>();
            kingdom.type = TerritoryType.Kingdom;
            kingdom.territoryColour = new Color(Random.value, Random.value, Random.value);

            // DIVIDE INTO PROVINCES
            DivideIntoSubTerritories(kingdom, bundle.Value, TerritoryType.Province);
        }
    }

    void DivideIntoSubTerritories(Territory parent, List<Territory> availableTiles, TerritoryType subType)
    {
        if (availableTiles.Count == 0) return;

        int divisions = System.Math.Max(2, (int)(availableTiles.Count * UnityEngine.Random.Range(0.015f, 0.026f)));

        List<Territory> seeds = GetRandomTilesFromList(availableTiles, divisions);
        var bundles = MultiFloodFill(seeds, availableTiles);

        foreach (var bundle in bundles)
        {
            GameObject subObj = new GameObject(subType.ToString() + "_Container");
            subObj.transform.SetParent(parent.transform);

            Territory sub = subObj.AddComponent<Territory>();
            sub.type = subType;
            sub.parentTerritory = parent;
            sub.territoryName = subType.ToString() + " of " + parent.territoryName;
            float h, s, v;
            Color.RGBToHSV(parent.territoryColour, out h, out s, out v);
            s = Mathf.Clamp(s + Random.Range(-0.25f, 0.25f), 0.3f, 1f);
            v = Mathf.Clamp(v + Random.Range(-0.25f, 0.25f), 0.4f, 1f);
            sub.territoryColour = Color.HSVToRGB(h, s, v);

            if (subType == TerritoryType.Province)
            {
                // KEEP GOING DOWN
                DivideIntoSubTerritories(sub, bundle.Value, TerritoryType.County);
            }
            else if (subType == TerritoryType.County)
            {
                // REACHED THE BOTTOM: Assign the actual Town tiles
                foreach (Territory townTile in bundle.Value)
                {
                    townTile.parentTerritory = sub;
                    townTile.transform.SetParent(sub.transform);

                    // Final check to make sure name and color are set on the tile
                    townTile.territoryName = "Town of " + parent.territoryName;
                    sub.subTerritories.Add(townTile);
                    Color.RGBToHSV(sub.territoryColour, out h, out s, out v);
                    s = Mathf.Clamp(s + Random.Range(-0.25f, 0.25f), 0.3f, 1f);
                    v = Mathf.Clamp(v + Random.Range(-0.25f, 0.25f), 0.4f, 1f);
                    townTile.territoryColour = Color.HSVToRGB(h, s, v);
                }
            }
            parent.subTerritories.Add(sub);
        }
    }

    // This ensures every tile is assigned to the NEAREST seed
    Dictionary<Territory, List<Territory>> MultiFloodFill(List<Territory> seeds, List<Territory> totalPool)
    {
        var assignments = new Dictionary<Territory, List<Territory>>();
        var claimLookup = new Dictionary<Territory, Territory>();
        Queue<Territory> queue = new Queue<Territory>();
        HashSet<Territory> poolSet = new HashSet<Territory>(totalPool);

        foreach (var seed in seeds)
        {
            // FIX 1: Add the seed to its own list immediately
            assignments[seed] = new List<Territory> { seed };
            claimLookup[seed] = seed;
            queue.Enqueue(seed);
        }

        while (queue.Count > 0)
        {
            Territory current = queue.Dequeue();

            foreach (Territory neighbor in GetNeighbors(current))
            {
                // Only claim if it's LAND and NOT already claimed
                if (poolSet.Contains(neighbor) && !claimLookup.ContainsKey(neighbor))
                {
                    Territory ownerSeed = claimLookup[current];
                    claimLookup[neighbor] = ownerSeed;
                    assignments[ownerSeed].Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // FINAL SAFETY: If any land tile was orphaned (no path to a capital)
        // assign it to the nearest existing kingdom.
        foreach (Territory t in totalPool)
        {
            if (!claimLookup.ContainsKey(t))
            {
                Territory closestSeed = null;
                float minDst = float.MaxValue;
                foreach (var seed in seeds)
                {
                    float dst = Vector3.Distance(t.transform.position, seed.transform.position);
                    if (dst < minDst) { minDst = dst; closestSeed = seed; }
                }
                assignments[closestSeed].Add(t);
            }
        }

        return assignments;
    }

    // --- UTILS ---
    List<Territory> GetNeighbors(Territory t)
    {
        List<Territory> neighbors = new List<Territory>();
        Vector2Int pos = GetGridPos(t);
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = pos.x + dx[i];
            int ny = pos.y + dy[i];
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                neighbors.Add(grid[nx, ny]);
        }
        return neighbors;
    }

    Vector2Int GetGridPos(Territory t) => new Vector2Int(Mathf.RoundToInt(t.transform.position.x / spacing), Mathf.RoundToInt(t.transform.position.y / spacing));

    List<Territory> GetRandomLandTiles(int count)
    {
        List<Territory> shuffled = new List<Territory>(landTiles);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rnd = Random.Range(i, shuffled.Count);
            var temp = shuffled[rnd]; shuffled[rnd] = shuffled[i]; shuffled[i] = temp;
        }
        return shuffled.GetRange(0, Mathf.Min(count, shuffled.Count));
    }

    List<Territory> GetRandomTilesFromList(List<Territory> list, int count)
    {
        List<Territory> copy = new List<Territory>(list);
        count = Mathf.Min(count, copy.Count);
        List<Territory> selected = new List<Territory>();
        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(0, copy.Count);
            selected.Add(copy[r]);
            copy.RemoveAt(r);
        }
        return selected;
    }

    void GenerateTerrain()
    {
        grid = new Territory[width, height];
        float offsetX = Random.Range(0f, 9999f);
        float offsetY = Random.Range(0f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject go = Instantiate(territoryPrefab, new Vector3(x * spacing, y * spacing, 0), Quaternion.identity, transform);
                Territory t = go.GetComponent<Territory>();
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

                // Multi-layered noise for better shapes
                float freq = 0.1f;
                float noise1 = Mathf.PerlinNoise((x + offsetX) * freq, (y + offsetY) * freq);
                float noise2 = Mathf.PerlinNoise((x + offsetX) * freq * 2, (y + offsetY) * freq * 2) * 0.5f;
                float finalNoise = (noise1 + noise2) / 1.5f;

                // No falloff here—noise goes right to the edge
                if (finalNoise > 0.35f)
                {
                    t.type = TerritoryType.Town;
                }
                else
                {
                    t.type = TerritoryType.Water;
                }
                sr.sprite = (t.type == TerritoryType.Town) ? townSprite : waterSprite;

                grid[x, y] = t;
            }
        }
    }

    // 2. Shape the landmasses (Terrain only)
    void SmoothWater()
    {
        for (int i = 0; i < 3; i++)
        {
            TerritoryType[,] nextGen = new TerritoryType[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Don't skip! Just check neighbors carefully
                    int waterNeighbors = CountWaterNeighbors(x, y);

                    if (waterNeighbors >= 4)
                    {
                        nextGen[x, y] = TerritoryType.Water;
                    }
                    else if (waterNeighbors <= 3)
                    {
                        nextGen[x, y] = TerritoryType.Town;
                    }
                    else nextGen[x, y] = grid[x, y].type;
                }
            }

            // Apply types back to grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y].type = nextGen[x, y];
                }
            }
        }
    }

    int CountWaterNeighbors(int x, int y)
    {
        int count = 0;
        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) { count++; continue; }
                if (nx == x && ny == y) continue;
                if (grid[nx, ny].type == TerritoryType.Water) count++;
            }
        }
        return count;
    }
}