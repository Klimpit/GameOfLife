using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;

    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;

    [SerializeField] private Pattern pattern;

    [SerializeField] private Slider speedSlider;

    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    [SerializeField] private float updateInterval = 0.05f;

    private bool stopTheGame = true;
    private IEnumerator simulation;

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
    }

    private void Start()
    {
        simulation = Simulate();
        SetPattern(pattern);
        ReadTiles();
    }

    private void Update()
    {
        ControlUpdateInterval();
        StopTheGame(Input.GetKeyDown(KeyCode.Space));
    }

    private void SetPattern(Pattern pattern)
    {
        if (pattern != null)
        {
            Clear();

            Vector2Int center = pattern.GetCenter();

            for (int i = 0; i < pattern.cells.Length; i++)
            {
                Vector3Int cell = (Vector3Int)(pattern.cells[i] - center);  // ����� i-�� ������ �� ������� � ��������� �������� �� ��������� �� center
                currentState.SetTile(cell, aliveTile); // ������������� �� �� Tilemap
                aliveCells.Add(cell); // ��������� � ��������� ��� ������
            }
        }
    }

    private void ReadTiles()
    {
        if(pattern == null)
        {
            BoundsInt bounds = currentState.cellBounds; // �������� ������� ��������� ������

            for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
            {
                for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    if (currentState.HasTile(tilePosition)) // ��������� �� ������� ����� ������
                    {
                        aliveCells.Add(new Vector3Int(x, y));
                    }
                }
            }
        }
    }

    private void Clear() // ������� ������ � ������ Tilemap'�, ����� ������ ������
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
    }

    private IEnumerator Simulate() // ������� ��������� ��� ���������� ��������� ���������� ������
    {
        while (enabled)
        {
            UpdateState();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void UpdateState()
    {
        foreach(Vector3Int cell in aliveCells) // ���������� �������� ������ �� �������� � ��������� activeCells
        {
            for(int x = -1; x<=1; x++)
            {
                for(int y =-1; y<=1; y++)
                {
                    cellsToCheck.Add(cell + new Vector3Int(x, y, 0));
                }
            }
        }

        foreach(Vector3Int cell in cellsToCheck) // �������� ���� ������ ��������� Game of life
        {
            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if(!alive && neighbors == 3)
            {
                nextState.SetTile(cell, aliveTile);
                aliveCells.Add(cell);
            }
            else if(alive && (neighbors < 2 || neighbors > 3))
            {
                nextState.SetTile(cell, deadTile);
                aliveCells.Remove(cell);
            }
            else
            {
                nextState.SetTile(cell, currentState.GetTile(cell));
            }
        }

        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
    }

    private int CountNeighbors(Vector3Int cell) // ������� ����� ������ � ������� 3�3
    {
        int count = 0;

        for (int x = -1; x<= 1; x++)
        {
            for(int y = -1; y<= 1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int(x, y, 0);

                if (x == 0 && y == 0)
                    continue;
                else if (IsAlive(neighbor))
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool IsAlive(Vector3Int cell) // �������� ������ �� �����
    {
        return currentState.GetTile(cell) == aliveTile;
    }

    private void StopTheGame(bool key)
    {
        if (stopTheGame && key)
        {
            StartCoroutine(simulation);
            stopTheGame = false;
        }
        else if (!stopTheGame && key)
        {
            stopTheGame = true;
            StopCoroutine(simulation);
        }
    }

    private void ControlUpdateInterval()
    {
        updateInterval = Mathf.Pow(speedSlider.value, -1);
    }
}
