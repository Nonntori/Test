using UnityEngine;
using _Project.Script.Conveyor;

public class BottleCollector : MonoBehaviour
{
    [SerializeField] private BoxManager _boxManager;
    [SerializeField] private ConveyorSystem _conveyorSystem;
    [SerializeField] private float _collectionInterval = 1.0f; // Интервал проверки и сбора бутылок
    [SerializeField] private float _detectionRadius = 5.0f; // Радиус обнаружения бутылок
    
    private float _lastCollectionTime;
    
    private void Start()
    {
        _lastCollectionTime = Time.time;
    }
    
    private void Update()
    {
        // Проверяем бутылки на конвейере периодически
        if (Time.time - _lastCollectionTime > _collectionInterval)
        {
            TryCollectBottles();
            _lastCollectionTime = Time.time;
        }
    }
    
    private void TryCollectBottles()
    {
        // Находим все бутылки, которые достигли конца конвейера
        Bottle[] bottles = FindObjectsOfType<Bottle>();
        
        foreach (Bottle bottle in bottles)
        {
            // Проверяем, что бутылка достигла конца конвейера и еще не собрана
            if (bottle.HasReachedEnd && !bottle.IsCollected)
            {
                // Находим подходящую коробку для бутылки
                Box targetBox = _boxManager.GetBoxForBottle(bottle.BottleColors);
                
                if (targetBox != null)
                {
                    // Помещаем бутылку в коробку
                    bottle.CollectInBox(targetBox);
                }
            }
        }
    }
    
    // Для ручного сбора бутылки при клике на нее
    public void CollectBottle(Bottle bottle)
    {
        if (!bottle.IsCollected)
        {
            Box targetBox = _boxManager.GetBoxForBottle(bottle.BottleColors);
            
            if (targetBox != null)
            {
                bottle.CollectInBox(targetBox);
            }
        }
    }
}
