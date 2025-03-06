using System.Collections;
using UnityEngine;

namespace _Project.Script.Conveyor
{
    /// <summary>
    /// Класс для отдельных объектов на конвейере
    /// </summary>
    public class Bottle : MonoBehaviour
    {
        [SerializeField] private Colors _bottleColors; // Цвет бутылки для соответствия цвету коробки
        
        private ConveyorSystem _conveyorSystem;
        private float _speed;
        private Vector3 _endPosition;
        private Vector3 _targetEndPosition;
        private bool _isMoving = true;
        
        public Colors BottleColors {
            get {
                return _bottleColors;
            }
        }

        public bool HasReachedEnd { get; private set; } = false;
        public bool IsCollected { get; private set; } = false;

        /// <summary>
        /// Инициализация объекта на конвейере
        /// </summary>
        public void Initialize(ConveyorSystem system, float moveSpeed, Vector3 conveyorEndPosition)
        {
            _conveyorSystem = system;
            _speed = moveSpeed;
            _endPosition = conveyorEndPosition;
            
            // Случайно назначаем цвет бутылке при создании, если не был указан ранее
            if (_bottleColors == 0)
            {
                _bottleColors = (Colors)Random.Range(1, System.Enum.GetValues(typeof(Colors)).Length);
            }
            
            // Устанавливаем цвет материала бутылки
            SetBottleColor();
        }
        
        private void SetBottleColor()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                UnityEngine.Color bottleColor = UnityEngine.Color.white; // Значение по умолчанию
                
                switch (_bottleColors)
                {
                    case Colors.Red:
                        bottleColor = UnityEngine.Color.red;
                        break;
                    case Colors.Green:
                        bottleColor = UnityEngine.Color.green;
                        break;
                    case Colors.Blue:
                        bottleColor = UnityEngine.Color.blue;
                        break;
                    case Colors.Yellow:
                        bottleColor = UnityEngine.Color.yellow;
                        break;
                }
                
                renderer.material.color = bottleColor;
            }
        }

        private void Update()
        {
            if (_isMoving && !IsCollected)
            {
                // Двигаем объект вперед
                transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
            
                // Проверяем, не достиг ли объект конца конвейера
                if (!HasReachedEnd && transform.position.z >= _endPosition.z)
                {
                    HasReachedEnd = true;
                
                    // Получаем позицию в конце с учетом других объектов
                    _targetEndPosition = _conveyorSystem.GetEndPositionForItem(this);
                
                    // Плавно перемещаем объект на его финальную позицию
                    StartCoroutine(MoveToEndPosition());
                }
            }
        }

        /// <summary>
        /// Плавное перемещение к конечной позиции
        /// </summary>
        private IEnumerator MoveToEndPosition()
        {
            _isMoving = false;
            float journeyLength = Vector3.Distance(transform.position, _targetEndPosition);
            float startTime = Time.time;
        
            Vector3 startPosition = transform.position;
        
            while (Vector3.Distance(transform.position, _targetEndPosition) > 0.01f)
            {
                float distCovered = (Time.time - startTime) * _speed;
                float fractionOfJourney = distCovered / journeyLength;
            
                transform.position = Vector3.Lerp(startPosition, _targetEndPosition, fractionOfJourney);
            
                yield return null;
            }
        
            transform.position = _targetEndPosition;
        }

        /// <summary>
        /// Установка новой конечной позиции (используется при перестроении)
        /// </summary>
        public void SetEndPosition(Vector3 newPosition)
        {
            _targetEndPosition = newPosition;
        
            // Если объект уже в конце, начинаем перемещение к новой позиции
            if (HasReachedEnd && !_isMoving && !IsCollected)
            {
                StartCoroutine(MoveToEndPosition());
            }
        }

        /// <summary>
        /// Метод для взаимодействия с объектом
        /// </summary>
        public void Interact()
        {
            // Здесь можно добавить код для взаимодействия с объектом
            Debug.Log("Взаимодействие с объектом: " + gameObject.name);
        
            // Удаляем объект с конвейера
            _conveyorSystem.RemoveItemFromBelt(this);
        
            // Удаляем объект из сцены (или можно заменить на другую логику)
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Метод для помещения бутылки в коробку
        /// </summary>
        public void CollectInBox(Box targetBox)
        {
            if (IsCollected) return;
            
            IsCollected = true;
            _isMoving = false;
            
            // Добавляем бутылку в коробку
            targetBox.AddObject();
            
            // Удаляем бутылку с конвейера
            _conveyorSystem.RemoveItemFromBelt(this);
            
            // Визуально перемещаем бутылку к коробке и уменьшаем до исчезновения
            StartCoroutine(MoveToBox(targetBox.transform));
        }
        
        private IEnumerator MoveToBox(Transform boxTransform)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = boxTransform.position + Vector3.up; // Немного выше коробки
            float duration = 0.5f;
            float startTime = Time.time;
            
            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
            
            // Уничтожаем объект бутылки после перемещения
            Destroy(gameObject);
        }
    }
}