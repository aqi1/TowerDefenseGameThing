using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [SerializeField] private int size = 20;
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private GameObject towerLocationPrefab;
    [SerializeField] private GameObject towerLocationButtonPrefab;
    [SerializeField] private Transform mapCanvasTransform;

    private int xsize;
    private int[,] grid; // 0-empty, 1-path, 2-tower
    private static List<GameObject> path;

    void Awake()
    {
        xsize = size + 5;
        path = new List<GameObject>();
        grid = new int[xsize + 1, size+1];
        CreateGrid();
        CreateTowerLocations();
    }

    private void CreateGrid()
    {
        int nextMove;
        int prevMove = -1;

        int x = 0;
        // int z = UnityEngine.Random.Range(size / 2, size);
        int z = size / 2;

        FillGridWithPath(x, z);
        while (x < xsize)
        {
            // 0-up, 1-up, 2-down, 3-down, 4-right
            // weighted z-axis changes to be more probable
            nextMove = UnityEngine.Random.Range(0, 5);

            // retry random if stepping backwards
            while(((nextMove == 0 || nextMove == 1) && (prevMove == 2 || prevMove == 3)) || ((nextMove == 2 || nextMove == 3) && (prevMove == 0 || prevMove == 1)))
                nextMove = UnityEngine.Random.Range(0, 5);

            prevMove = nextMove;

            switch (nextMove)
            {
                case 0:
                case 1:
                    if (z < size - 1 && x > 0)
                        z++;
                    else
                    {
                        x++;
                        prevMove = 4;
                    }
                    break;
                case 2:
                case 3:
                    if (z > 1 && x > 0)
                        z--;
                    else
                    {
                        x++;
                        prevMove = 4;
                    }
                    break;
                case 4:
                    x++;
                    break;
                default:
                    Debug.Log("CreateGrid() has failed");
                    break;
            }

            Debug.Log("FillGridWithPath(" + x + ", " + z + ")");
            FillGridWithPath(x, z);
        }
    }

    private void FillGridWithPath(int x, int z)
    {
        grid[x, z] = 1;
        path.Add(Instantiate(waypointPrefab, new Vector3(x, FindTerrainHeight(x, z), z), Quaternion.identity));
        path[path.Count - 1].name = (path.Count - 1).ToString();
    }

    private void CreateTowerLocations()
    {
        for (int x = 0; x < xsize+1; x++)
        {
            for (int z = 0; z < size+1; z++)
            {
                if (grid[x, z] == 0 && CanPlaceTower(x, z))
                {
                    grid[x, z] = 2;
                    Instantiate(towerLocationButtonPrefab, new Vector3(x, FindTerrainHeight(x,z), z), Quaternion.Euler(90,0,0), mapCanvasTransform);
                }
            }
        }
    }

    private bool CanPlaceTower(int x, int z)
    {
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                int a = x + i;
                int b = z + j;
                if (a >= 0 && a < xsize && b >= 0 && b < size && grid[a, b] == 1)
                    return true;
            }
        }

        return false;
    }

    public static List<GameObject> GetPath()
    {
        return path;
    }

    float FindTerrainHeight(int x, int z)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(x, 5, z), Vector3.down, out hit))
        {
            // Debug.Log("hit point y: " + hit.point.y);
            return hit.point.y + 0.2f;
        }
        else
            return 0;
    }
}
