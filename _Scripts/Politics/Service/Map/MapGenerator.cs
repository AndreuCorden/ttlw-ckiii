using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Data;

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
    public GameObject mapInteraction;

    [System.Obsolete]
    void Start()
    {
        GenerateTerrain();
        SmoothWater();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].territoryType == TerritoryType.Water)
                {
                    // Either disable them or destroy them if you don't need them
                    Destroy(grid[x, y].gameObject);
                }
            }
        }

        // 1. MUST fill the land list after smoothing is done
        landTiles.Clear();
        foreach (var t in grid) if (t.territoryType == TerritoryType.Land) landTiles.Add(t);

        LinkNeighbors();

        // 2. Start the hierarchy (Top-Down)
        GenerateFeudalHierarchy();

        Object.FindAnyObjectByType<MapManager>().UpdateMapVisuals();

        Object.FindAnyObjectByType<CharacterCreatorUI>().ShowCreator();
    }

    [System.Obsolete]
    public void FinalizeWorldGeneration(Title playerTitle)
    {
        mapInteraction.SetActive(true);
        List<Title> kingdomList = new List<Title>();
        foreach (var t in Object.FindObjectsOfType<Title>())
        {
            if (t.rank == TitleRank.King)
            {
                kingdomList.Add(t);
            }
        }

        Debug.Log($"SocialEngine check: {socialEngine != null}. Kingdom Count: {kingdomList.Count}. Tile Count: {landTiles.Count}. Now populating world...");

        socialEngine.PopulateWorld(kingdomList, playerTitle);

        settlementEngine.MarkAsCapital(kingdomList);

        Debug.Log($"SocialEngine check: Done assigning capitals. Now assigning sizes and populations.");

        settlementEngine.AssignTerritorySizeAndPopulation();

        settlementEngine.RunInitialEconomySimulation();

        settlementEngine.RunInitialPopulationAddition(kingdomList,landTiles);

        Debug.Log($"Map generation complete. Updating map visuals.");

        Object.FindAnyObjectByType<MapManager>().UpdateMapVisuals();
        Object.FindAnyObjectByType<PlayerManager>().UpdateCharacterParameters();
        Object.FindAnyObjectByType<PlayerManager>().SetActiveParameterView();
        Object.FindAnyObjectByType<CharacterRegistry>().PopulateRegistry();
    }

    public Title CreateTitle(string prefix, TitleRank rank, Title liege)
    {
        GameObject obj = new GameObject($"Title_{prefix}");
        obj.transform.SetParent(liege != null ? liege.transform : this.transform);
        Title t = obj.AddComponent<Title>();
        t.rank = rank;
        t.seatOfPower = null;
        t.liege = liege;
        if (liege != null) liege.vassals.Add(t);
        return t;
    }

    public void GenerateFeudalHierarchy()
    {
        // 1. INITIAL GEOGRAPHIC CARVING
        var KingdomBundles = MultiFloodFill(GetRandomTilesFromList(landTiles, landTiles.Count / 100), landTiles);
        List<Title> kingdoms = new List<Title>();

        foreach (var kbundle in KingdomBundles)
        {
            Title kTitle = CreateTitle("Kingdom", TitleRank.King, null);
            kingdoms.Add(kTitle);

            var ProvinceBundle = MultiFloodFill(GetRandomTilesFromList(kbundle.Value, Mathf.Max(2, kbundle.Value.Count / 30)), kbundle.Value);

            foreach (var pbundle in ProvinceBundle)
            {
                Title pTitle = CreateTitle("Province", TitleRank.Duke, kTitle);

                var CountBundle = MultiFloodFill(GetRandomTilesFromList(pbundle.Value, Mathf.Max(2, pbundle.Value.Count / 10)), pbundle.Value);

                foreach (var cbundle in CountBundle)
                {
                    Title cTitle = CreateTitle("County", TitleRank.Count, pTitle);

                    // Fill everything with Barons first
                    foreach (Territory b in cbundle.Value)
                    {
                        Title bTitle = CreateTitle("Barony", TitleRank.Baron, cTitle);
                        b.county = cTitle;
                        b.duchy = pTitle;
                        b.kingdom = kTitle;
                        b.owner = bTitle;
                        bTitle.seatOfPower = b;
                        bTitle.directDomain.Add(b);
                    }

                    // 2. COUNTY SEIZURE (Takes 2 Baronies)
                    SeizeTiles(cTitle, 2);
                }

                // 3. PROVINCE SEIZURE (Takes 2 tiles, prefers Baronies over Counties)
                SeizeTiles(pTitle, 2);
            }

            // 4. KINGDOM SEIZURE (Takes 2 tiles, prefers Baronies -> Counties -> Dukes)
            SeizeTiles(kTitle, 2);
        }
        ApplyTopDownColours(kingdoms);
    }

    /// <summary>
    /// Logic to seize tiles from lower ranks and handle the destruction of previous titles.
    /// </summary>
    private void SeizeTiles(Title taker, int amount)
    {
        // 1. Find targets (Preferring tiles that aren't currently Seats of Power)
        List<Territory> targets = taker.FullRealmTiles
            .Where(t => t.owner != taker && t.owner != null)
            .OrderBy(t => t == t.owner.seatOfPower ? 1 : 0) // Prefer non-seats (0 comes before 1)
            .ThenBy(t => (int)t.owner.rank)                 // Prefer lower ranks
            .ThenBy(t => Random.value)                      // Randomize within those tiers
            .Take(amount)
            .ToList();

        for (int i = 0; i < targets.Count; i++)
        {
            Territory t = targets[i];
            Title victim = t.owner;

            // Seize the tile
            t.owner = taker;
            taker.directDomain.Add(t);
            victim.directDomain.Remove(t);
            if (i == 0) taker.seatOfPower = t;

            // 2. Handle the Victim's Survival Logic
            HandleTitleSurvival(victim, taker);
        }
    }

    private void HandleTitleSurvival(Title victim, Title taker)
    {
        // If they still own other land, they just move their seat if needed
        if (victim.directDomain.Count > 0)
        {
            if (victim.seatOfPower == null || victim.seatOfPower.owner != victim)
            {
                victim.seatOfPower = victim.directDomain[0];
            }
            return;
        }

        // --- NO LAND LEFT: TRY TO SEIZE FROM A VASSAL ---
        if (victim.vassals.Count > 0)
        {
            // Find a vassal to take a tile from (Preferring Barons)
            Title targetVassal = victim.vassals
                .OrderBy(v => (int)v.rank)
                .FirstOrDefault();

            if (targetVassal != null && targetVassal.directDomain.Count > 0)
            {
                // Take the vassal's tile
                Territory seizedFromVassal = targetVassal.directDomain[0];

                targetVassal.directDomain.Remove(seizedFromVassal);
                victim.directDomain.Add(seizedFromVassal);
                seizedFromVassal.owner = victim;
                victim.seatOfPower = seizedFromVassal;

                // Recursively check if that vassal survives now!
                HandleTitleSurvival(targetVassal, victim);
                return;
            }
        }

        // --- TOTAL ELIMINATION ---
        // If we got here, they have no land and no vassals to steal from.
        Debug.Log($"{victim.name} has been eliminated by {taker.name}");

        // Orphans move to the new Taker
        foreach (Title orphan in new List<Title>(victim.vassals))
        {
            orphan.liege = taker;
            if (!taker.vassals.Contains(orphan)) taker.vassals.Add(orphan);
        }

        if (victim.liege != null) victim.liege.vassals.Remove(victim);
        DestroyImmediate(victim.gameObject);
    }

    // public void GenerateFeudalHierarchy()
    // {
    //     // 1. CARVE KINGDOMS
    //     var kingdomBundles = MultiFloodFill(GetRandomTilesFromList(landTiles, landTiles.Count / 50), landTiles);
    //     List<Title> kingdoms = new List<Title>();

    //     foreach (var kbundle in kingdomBundles)
    //     {
    //         Title kTitle = CreateTitle("Kingdom", TitleRank.King, null);
    //         kingdoms.Add(kTitle);

    //         // 2. CARVE PROVINCES
    //         var provinces = MultiFloodFill(GetRandomTilesFromList(kbundle.Value, Mathf.Max(2, kbundle.Value.Count / 20)), kbundle.Value);

    //         foreach (var pbundle in provinces)
    //         {
    //             Title dTitle = CreateTitle("Province", TitleRank.Duke, kTitle);

    //             // 3. CARVE COUNTIES
    //             var counties = MultiFloodFill(GetRandomTilesFromList(pbundle.Value, Mathf.Max(2, pbundle.Value.Count / 5)), pbundle.Value);

    //             foreach (var cbundle in counties)
    //             {
    //                 Title cTitle = CreateTitle("County", TitleRank.Count, dTitle);

    //                 // IMPORTANT: Tag all tiles in the county so they know where they live geographically
    //                 foreach (Territory t in cbundle.Value)
    //                 {
    //                     t.county = cTitle;
    //                     t.duchy = dTitle;
    //                     t.kingdom = kTitle;
    //                 }
    //             }

    //             // 4. ASSIGN SEATS (Now that geography is locked)
    //             // Pick a Duke seat from the Province bundle
    //             Territory dSeat = pbundle.Value[Random.Range(0, pbundle.Value.Count)];
    //             dTitle.seatOfPower = dSeat;
    //             dTitle.directDomain.Add(dSeat);
    //         }

    //         // Pick a King seat from the Kingdom bundle
    //         Territory kSeat = kbundle.Value[Random.Range(0, kbundle.Value.Count)];
    //         kTitle.seatOfPower = kSeat;
    //         kTitle.directDomain.Add(kSeat);
    //     }

    //     // 5. FINAL PASS: Barons and Royal Enclaves
    //     // We iterate through all land tiles once everything is carved
    //     foreach (Territory t in landTiles)
    //     {
    //         // Skip tiles already claimed as Seats of Power
    //         if (t.kingdom.seatOfPower == t || t.duchy.seatOfPower == t || t.county.seatOfPower == t)
    //             continue;

    //         float roll = Random.value;
    //         if (roll < 0.05f)
    //         {
    //             t.kingdom.directDomain.Add(t);
    //         }
    //         else if (roll < 0.15f)
    //         {
    //             t.duchy.directDomain.Add(t);
    //         }
    //         else
    //         {
    //             // If nobody high-up took it, it's a Barony for the local Count
    //             Title baron = CreateTitle("Barony", TitleRank.Baron, t.county);
    //             baron.seatOfPower = t;
    //             baron.directDomain.Add(t);
    //         }
    //     }

    //     ApplyTopDownColours(kingdoms);
    // }

    public void ApplyTopDownColours(List<Title> kingdoms)
    {
        foreach (Title king in kingdoms)
        {
            king.colour = new Color(Random.value, Random.value, Random.value);
            SetColourRecursive(king, king.colour);
        }
    }

    void SetColourRecursive(Title current, Color baseColor)
    {
        foreach (Title vassal in current.vassals)
        {
            float h, s, v;
            Color.RGBToHSV(baseColor, out h, out s, out v);
            // Slightly shift the color for the vassal
            float sShift = Random.Range(-0.2f, 0.2f);
            float vShift = Random.Range(-0.2f, 0.2f);

            Color vassalColor = Color.HSVToRGB(h, Mathf.Clamp01(s + sShift), Mathf.Clamp01(v + vShift)); vassal.colour = vassalColor;
            SetColourRecursive(vassal, vassalColor);
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

    void LinkNeighbors()
    {
        foreach (Territory t in landTiles)
        {
            // If it's a grid, find tiles at x+1, x-1, y+1, y-1
            // If it's physics-based, use a small OverlapCircle
            Collider2D[] hits = Physics2D.OverlapCircleAll(t.transform.position, 1.1f);
            foreach (var hit in hits)
            {
                Territory neighbor = hit.GetComponent<Territory>();
                if (neighbor != null && neighbor != t)
                {
                    t.neighbors.Add(neighbor);
                }
            }
        }
    }

    Vector2Int GetGridPos(Territory t) => new Vector2Int(Mathf.RoundToInt(t.transform.position.x / spacing), Mathf.RoundToInt(t.transform.position.y / spacing));

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
                    t.territoryType = TerritoryType.Land;
                }
                else
                {
                    t.territoryType = TerritoryType.Water;
                }
                sr.sprite = (t.territoryType == TerritoryType.Land) ? townSprite : waterSprite;

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
                        nextGen[x, y] = TerritoryType.Land;
                    }
                    else nextGen[x, y] = grid[x, y].territoryType;
                }
            }

            // Apply types back to grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y].territoryType = nextGen[x, y];
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
                if (grid[nx, ny].territoryType == TerritoryType.Water) count++;
            }
        }
        return count;
    }
}