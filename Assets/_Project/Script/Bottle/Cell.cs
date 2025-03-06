using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Cell : MonoBehaviour
{
    private bool _isAvailable = true;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Box box))
        {
            Available();
            box.transform.SetParent(transform);
        }
    }

    public void Available()
    {
        _isAvailable = false;
    }
    
    public bool IsAvailable { get; private set; } = true;
    private Box _occupyingBox = null;
    public Vector3 worldPosition;
    public Vector2 gridPosition;
    public Box boxObject;
    public Cell(GameObject cell, Vector2Int vector2Int, Vector3 cellPosition)
    {
        throw new System.NotImplementedException();
    }

    public void Occupy(Box box)
    {
        IsAvailable = false;
        _occupyingBox = box;
    }
    
    public void Release()
    {
        IsAvailable = true;
        _occupyingBox = null;
    }
    
    public Box GetOccupyingBox()
    {
        return _occupyingBox;
    }
}
