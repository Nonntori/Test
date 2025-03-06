using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Box : MonoBehaviour
{
    [Header("Box Settings")]
    [SerializeField] private int capacity = 10;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Colors boxColor = Colors.White;
    
    [Header("Visual Settings")]
    [SerializeField] private bool useColorAnimation = true;
    [SerializeField] private float colorChangeTime = 0.3f;
    [SerializeField] private Transform contentContainer; // Контейнер для содержимого коробки
    
    [field: Header("Events")]
    private event Action onBoxFull;
    private event Action<int> onItemAdded;
    private event Action onMoveComplete;

    private int currentCount = 0;
    private Renderer _renderer;
    private Dictionary<Colors, Color> _colorMap;
    private Tween _currentMoveTween;
    
    public bool IsFull => currentCount >= capacity;
    public Colors BoxColor => boxColor;
    public float FillPercentage => (float)currentCount / capacity;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            _renderer = GetComponentInChildren<Renderer>();
        }

        InitializeColorMap();
    }

    private void Start()
    {
        // Применяем начальный цвет, если он не был установлен извне
        if (_renderer != null && _renderer.material.color == Color.white)
        {
            SetBoxColor(boxColor);
        }
    }

    private void InitializeColorMap()
    {
        _colorMap = new Dictionary<Colors, Color>
        {
            { Colors.Red, Color.red },
            { Colors.Green, Color.green },
            { Colors.Blue, Color.blue },
            { Colors.Yellow, Color.yellow },
            { Colors.White, Color.white },
            { Colors.Black, Color.black },
            { Colors.None, Color.gray }
        };
    }
    
    // Устанавливает цвет коробки
    public void SetBoxColor(Colors color)
    {
        boxColor = color;
        
        if (_renderer == null) return;
        
        if (_colorMap.TryGetValue(color, out Color unityColor))
        {
            if (useColorAnimation)
            {
                _renderer.material.DOColor(unityColor, colorChangeTime);
            }
            else
            {
                _renderer.material.color = unityColor;
            }
        }
    }
    
    // Перемещает коробку к целевой позиции
    public void Move(Vector3 targetPosition)
    {
        // Отменяем предыдущее движение, если оно есть
        if (_currentMoveTween != null && _currentMoveTween.IsActive())
        {
            _currentMoveTween.Kill();
        }
        
        _currentMoveTween = transform.DOMove(targetPosition, moveSpeed)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                onMoveComplete?.Invoke();
            });
    }
    
    // Перемещает коробку к целевой позиции
    public void Move(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            Move(targetTransform.position);
        }
    }
    
    // Добавляет объект в коробку
    public bool AddObject()
    {
        if (IsFull) return false;
        
        currentCount++;
        onItemAdded?.Invoke(currentCount);
        
        // Визуализация добавления объекта
        UpdateVisualRepresentation();
        
        if (IsFull)
        {
            onBoxFull?.Invoke();
        }
        
        return true;
    }
    
    // Обновляет визуальное представление содержимого коробки
    private void UpdateVisualRepresentation()
    {
        // Здесь можно добавить логику визуализации содержимого
        // Например, отображение стопки объектов внутри коробки
        if (contentContainer != null)
        {
            // Примерная реализация - показ высоты наполнения
            float fillHeight = FillPercentage * contentContainer.localScale.y;
            contentContainer.localScale = new Vector3(
                contentContainer.localScale.x,
                fillHeight,
                contentContainer.localScale.z
            );
        }
    }

}