using UnityEngine;

public class MoverController : MonoBehaviour
{
    [SerializeField] private BottleCollectionGrid _bottleCollection;
    [SerializeField] private Cell cell;

    public BottleCollectionGrid BottleCollection => _bottleCollection;
    
    public Transform GetTargetToMove()
    {
        Cell[] areas = _bottleCollection.GatheringAreas;

        foreach (Cell area in areas)
        {
            if (area.IsAvailable == false)
            {
                area.Available();
                return area.transform;
            }
        }
        
        return null; 
    }
}
