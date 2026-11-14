using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;          // optional
    [SerializeField] private GameObject selectedMark; // optional highlight

    public SelectableData Data { get; private set; }

    public void Setup(SelectableData data, Action<SelectableData> onClick)
    {
        Data = data;
        icon.sprite = data.icon;
        icon.preserveAspect = true;
        if (label != null) label.text = data.displayName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(data));
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectedMark != null) selectedMark.SetActive(selected);
    }
}