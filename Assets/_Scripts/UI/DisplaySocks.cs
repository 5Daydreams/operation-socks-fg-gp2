using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySocks : MonoBehaviour
{
    [SerializeField] private SockRegistry sockRegistry;

    [SerializeField] private TMPro.TMP_Text winCountLabel;
    [SerializeField] private TMPro.TMP_Text simpleLabel;

    private void Update()
    {
        winCountLabel.text = $"{sockRegistry.GetDroppedOffCount().ToString()} / {sockRegistry.sockNumberToWin.ToString()}";
        simpleLabel.text = $"x{sockRegistry.GetCurrentCount().ToString()}";
    }

#if UNITY_EDITOR
    private int offset = 33;
    private Vector2 size = new Vector2(180, 25);
    private void OnGUI()
    {
        GUI.color = Color.white;
        GUI.Box(new Rect(10,  100 + offset, size.x, size.y), $"Left in level: {sockRegistry.GetSocksInLevelCount().ToString()}");
    }
#endif
}
