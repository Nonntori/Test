using UnityEngine;


public class CellData
{
    public GameObject cellObject;
    public Box boxObject;
    public Vector2Int gridPosition;
    public Vector3 worldPosition;

    public bool IsOccupied => boxObject != null;

    public CellData(GameObject cell, Vector2Int pos, Vector3 worldPos)
    {
        cellObject = cell;
        gridPosition = pos;
        worldPosition = worldPos;
        boxObject = null;
    }
}
