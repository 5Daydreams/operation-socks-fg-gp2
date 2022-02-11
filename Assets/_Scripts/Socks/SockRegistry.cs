using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SockRegistry")]
public class SockRegistry : ScriptableObject
{
    [SerializeField] internal int sockNumberToWin;
    [SerializeField] private int sockInventoryCapacity;
    
    [SerializeField] private List<SockData> socksData;
    
    private int sockNumberInLevel;
    private List<InteractableSock> currentSockInventory;

    public int SockInventoryCapacity => sockInventoryCapacity;

    public void DropOff()
    {
        foreach (var socks in socksData)
        {
           socks.DropOff();
        }
    }

    public int GetCurrentCount()
    {
        int currentCount = 0;
        for (int i = 0; i < socksData.Count; i++)
        {
            currentCount += socksData[i].CollectedSocks.Count;
        }
        return currentCount;
    }
    
    public int GetSocksInLevelCount()
    {
        int currentCount = 0;
        for (int i = 0; i < socksData.Count; i++)
        {
            currentCount += socksData[i].SocksInLevel.Count;
        }
        return currentCount;
    }
    
    public int GetDroppedOffCount()
    {
        int currentCount = 0;
        for (int i = 0; i < socksData.Count; i++)
        {
            currentCount += socksData[i].DroppedOffSocks.Count;
        }
        return currentCount;
    }

    public void ResetInventory()
    {
        foreach (var socks in socksData)
        {
            socks.ResetSocks();
            socks.ResetLevel();
            //socks.ResetDropOff();
        }
    }
    
    public void ResetFullInventory()
    {
        foreach (var socks in socksData)
        {
            socks.ResetSocks();
            socks.ResetLevel();
            socks.ResetDropOff();
        }
    }

    public bool CheckIfWon()
    {
        return sockNumberToWin <= GetDroppedOffCount();
    }
    
}
