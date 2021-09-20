using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour, IMoveHandler
{
    public int index;
    [SerializeField] private TMP_Text text;
    [SerializeField] private string[] options;
    [SerializeField] private UnityEvent<int> onChangeSelection;
    
    private List<string> listOptions = new List<string>();

    private void Awake()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (options.Length > 0)
        {
            listOptions.Clear();
            foreach (var option in options)
                listOptions.Add(option);
            text.text = listOptions[index];
        }
    }

    public void NextIndex()
    {
        SetIndex(index + 1);
    }
    
    public void PrecedentIndex()
    {
        SetIndex(index - 1);
    }
    
    public void SetIndex(int newIndex)
    {
        index = newIndex;
        if (listOptions.Count <= index)
            index = 0;
        else if (0 > index)
            index = listOptions.Count - 1;

        text.text = listOptions[index];
        onChangeSelection.Invoke(index);
    }
    
    public void OnMove(AxisEventData eventData)
    {
        if(listOptions.Count <= 0)
            return;
        
        switch (eventData.moveDir)
        {
            case MoveDirection.Right:
            {
                NextIndex();
                break;
            }
            case MoveDirection.Left:
            {
                PrecedentIndex();
                break;
            }
        }
    }
}
