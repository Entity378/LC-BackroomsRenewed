using VELDDev.BackroomsRenewed.Generation.Algorithms;

namespace VELDDev.BackroomsRenewed.Generation;

public class BackroomsGenerator : MonoBehaviour
{
    public struct ExitInfo
    {
        public Vector2Int position;
        public WallFlags direction;
    }

    public int width = 20;
    public int height = 20;
    public int exitCount = 1;
    [Range(0f, 1f)] public float wallRemovalFraction = 0.1f;
    public MazeAlgorithm algorithm = LocalConfig.Singleton.GenerationAlgorithm.Value;

    [HideInInspector] public Cell[,] cells = new Cell[0, 0];
    [HideInInspector] public List<ExitInfo> exitPositions = new();
    private IMazeAlgorithm currentAlgorithm;
    
    public enum MazeAlgorithm
    {
        Blob,
        Kruskal,
        Prim,
        Wilson,
        RandomPathMerge,
        FractalTessellation
    }
    
    public IEnumerator Generate()
    {
        yield return InitializeMaze();
        
        currentAlgorithm = algorithm switch
        {
            MazeAlgorithm.Kruskal => new KruskalAlgorithm(),
            MazeAlgorithm.Prim => new PrimAlgorithm(),
            MazeAlgorithm.Wilson => new WilsonAlgorithm(),
            MazeAlgorithm.Blob => new BlobAlgorithm(),
            //MazeAlgorithm.RandomPathMerge => new RandomPathMergeAlgorithm(),
            //MazeAlgorithm.FractalTessellation => new FractalTessellationAlgorithm(),
            _ => throw new System.NotImplementedException()
        };
        
        yield return currentAlgorithm.Generate(cells, width, height);
        RemoveRandomWalls(wallRemovalFraction);
        PlaceExits();
    }

    private void RemoveRandomWalls(float fraction)
    {
        if (fraction <= 0f) return;

        var walls = new List<(Vector2Int cell, WallFlags dir, Vector2Int neighbor, WallFlags opposite)>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < width - 1 && (cells[x, y].walls & WallFlags.East) != 0)
                    walls.Add((new(x, y), WallFlags.East, new(x + 1, y), WallFlags.West));
                if (y < height - 1 && (cells[x, y].walls & WallFlags.North) != 0)
                    walls.Add((new(x, y), WallFlags.North, new(x, y + 1), WallFlags.South));
            }
        }

        int toRemove = Mathf.FloorToInt(walls.Count * fraction);
        for (int i = 0; i < toRemove; i++)
        {
            int pick = Random.Range(i, walls.Count);
            (walls[i], walls[pick]) = (walls[pick], walls[i]);
            var w = walls[i];
            cells[w.cell.x, w.cell.y].walls &= ~w.dir;
            cells[w.neighbor.x, w.neighbor.y].walls &= ~w.opposite;
        }
    }
    
    private IEnumerator InitializeMaze()
    {
        cells = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new Cell { position = new(x, y) };
                yield return null;
            }
        }
    }
    
    private void PlaceExits()
    {
        exitPositions.Clear();
        var usedPositions = new HashSet<Vector2Int>();
        int maxAttempts = exitCount * 20;

        for (int attempts = 0; exitPositions.Count < exitCount && attempts < maxAttempts; attempts++)
        {
            int side = Random.Range(0, 4);
            Vector2Int pos;
            WallFlags dir;

            switch (side)
            {
                case 0:
                    pos = new Vector2Int(Random.Range(0, width), height - 1);
                    dir = WallFlags.North;
                    break;
                case 1:
                    pos = new Vector2Int(width - 1, Random.Range(0, height));
                    dir = WallFlags.East;
                    break;
                case 2:
                    pos = new Vector2Int(Random.Range(0, width), 0);
                    dir = WallFlags.South;
                    break;
                default:
                    pos = new Vector2Int(0, Random.Range(0, height));
                    dir = WallFlags.West;
                    break;
            }

            if (usedPositions.Contains(pos)) continue;

            usedPositions.Add(pos);
            cells[pos.x, pos.y].walls &= ~dir;
            exitPositions.Add(new ExitInfo { position = pos, direction = dir });
        }
    }
}