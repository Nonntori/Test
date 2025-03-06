using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Project.Script.Conveyor
{
    /// <summary>
    /// Основной класс, управляющий конвейерной системой
    /// </summary>
    public class ConveyorSystem : MonoBehaviour
    {
        [Header("Настройки объектов")]
        [SerializeField] private GameObject _itemPrefab; // Префаб объекта для создания
        [SerializeField] private Transform _spawnPoint; // Точка создания объектов
        [SerializeField] private Transform _endPoint; // Конечная точка конвейера
    
        [Header("Настройки конвейера")]
        [SerializeField] private float _conveyorSpeed = 2.0f; // Скорость движения объектов
        [SerializeField] private int _maxItemsToCreate = 10; // Максимальное кол-во объектов для создания
        [SerializeField] private int _maxItemsOnBelt = 5; // Максимальное кол-во объектов на ленте
        [SerializeField] private float _spawnInterval = 2.0f; // Интервал между созданием объектов
        [SerializeField] private float _endSpacing = 1.0f; // Расстояние между объектами в конце конвейера
        
        [Header("Интеграция с системой сбора")]
        [SerializeField] private BottleCollector _bottleCollector; // Ссылка на сборщик бутылок
    
        private readonly List<Bottle> _itemsOnBelt = new List<Bottle>(); // Список объектов на ленте
    
        private int _itemsCreated = 0; // Счетчик созданных объектов
        private bool _isSpawning = false; // Индикатор процесса создания

        private void Start()
        {
            // Запуск корутины для создания объектов
            StartCoroutine(SpawnItems());
        }

        private IEnumerator SpawnItems()
        {
            _isSpawning = true;
        
            while (_itemsCreated < _maxItemsToCreate && _itemsOnBelt.Count < _maxItemsOnBelt)
            {
                // Создаем новый объект
                GameObject newItem = Instantiate(_itemPrefab, _spawnPoint.position, Quaternion.identity);
                newItem.name = "Item_" + _itemsCreated;
                newItem.transform.parent = _spawnPoint;
                
                Bottle itemComponent = newItem.GetComponent<Bottle>(); 
                
                // Инициализируем объект
                itemComponent.Initialize(this, _conveyorSpeed, _endPoint.position);
            
                // Добавляем объект в список на ленте
                _itemsOnBelt.Add(itemComponent);
                _itemsCreated++;
            
                // Ожидаем перед созданием следующего объекта
                yield return new WaitForSeconds(_spawnInterval);
            
                // Проверяем, не превышено ли максимальное количество объектов на ленте
                if (_itemsOnBelt.Count >= _maxItemsOnBelt)
                {
                    yield return new WaitUntil(() => _itemsOnBelt.Count < _maxItemsOnBelt);
                }
            }
        
            _isSpawning = false;
        }

        /// <summary>
        /// Метод для удаления объекта с конвейера
        /// </summary>
        public void RemoveItemFromBelt(Bottle item)
        {
            if (_itemsOnBelt.Contains(item))
            {
                _itemsOnBelt.Remove(item);
            
                // Если создание объектов остановилось из-за лимита на ленте, возобновляем
                if (!_isSpawning && _itemsCreated < _maxItemsToCreate && _itemsOnBelt.Count < _maxItemsOnBelt)
                {
                    StartCoroutine(SpawnItems());
                }
            
                // Перестраиваем объекты в конце конвейера
                RearrangeEndItems();
            }
        }

        /// <summary>
        /// Перераспределение объектов в конце конвейера для сохранения правильного расстояния
        /// </summary>
        private void RearrangeEndItems()
        {
            // Получаем и сортируем объекты в конце конвейера по Z-координате (от дальнего к ближнему)
            var endItems = _itemsOnBelt
                .Where(item => item.HasReachedEnd && !item.IsCollected)
                .OrderBy(item => item.transform.position.z)
                .ToList();
    
            // Расставляем объекты с нужным расстоянием
            for (int i = 0; i < endItems.Count; i++)
            {
                endItems[i].SetEndPosition(CalculateEndPosition(i));
            }
        }

        /// <summary>
        /// Получить позицию для объекта в конце конвейера
        /// </summary>
        public Vector3 GetEndPositionForItem(Bottle item)
        {
            // Индекс позиции = количество объектов, которые уже достигли конца (но не собраны)
            int index = _itemsOnBelt.Count(existingItem => 
                existingItem != item && existingItem.HasReachedEnd && !existingItem.IsCollected);
    
            return CalculateEndPosition(index);
        }

        /// <summary>
        /// Рассчитать позицию для объекта в конце конвейера по его индексу
        /// </summary>
        private Vector3 CalculateEndPosition(int index)
        {
            return new Vector3(
                _endPoint.position.x,
                _endPoint.position.y,
                _endPoint.position.z + (index * _endSpacing)
            );
        }
    }
}