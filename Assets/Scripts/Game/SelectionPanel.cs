using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPanel : MonoBehaviour
{
    [SerializeField] private SelectionButton buttonPrefab;
    [SerializeField] private Transform container; // Content with a Layout Group
    [SerializeField] private List<SelectableData> items;

    [Header("Cerrar al elegir")]
    [SerializeField] private bool closeOnSelect = true;
    [SerializeField] private SlidePanel slidePanel; // si esta vacio, busca el del mismo objeto/padre

    public event Action<SelectableData> Selected;

    private readonly List<SelectionButton> spawned = new();

    private void Awake()
    {
        if (slidePanel == null) slidePanel = GetComponentInParent<SlidePanel>();
    }

    private void Start()
    {
        foreach (var item in items)
        {
            var btn = Instantiate(buttonPrefab, container);
            btn.Setup(item, OnItemClicked);
            spawned.Add(btn);
        }
    }

    private void OnItemClicked(SelectableData data)
    {
        foreach (var b in spawned) b.SetSelected(b.Data == data);
        Selected?.Invoke(data);

        if (closeOnSelect && slidePanel != null) slidePanel.Close();
    }
}