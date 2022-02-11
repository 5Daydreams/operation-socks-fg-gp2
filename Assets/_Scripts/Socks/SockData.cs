using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SockData")]
public class SockData : ScriptableObject
{
    private List<InteractableSock> collectedSocks = new List<InteractableSock>();
    private List<InteractableSock> socksInLevel = new List<InteractableSock>();
    private List<InteractableSock> droppedOffSocks = new List<InteractableSock>();

    public List<InteractableSock> CollectedSocks => collectedSocks;
    public List<InteractableSock> SocksInLevel => socksInLevel;
    public List<InteractableSock> DroppedOffSocks => droppedOffSocks;

    private void OnEnable()
    {
        socksInLevel.Clear();
    }

    private void OnDisable()
    {
        socksInLevel.Clear();
    }

    public void AddSock(InteractableSock sock)
    {
        CollectedSocks.Add(sock);
        socksInLevel.Remove(sock);
    }

    public void DropOff()
    {
        DroppedOffSocks.AddRange(collectedSocks);
        CollectedSocks.Clear();
    }
    
    public void RegisterLevel(InteractableSock sock)
    {
        socksInLevel.Add(sock);
    }

    public void ResetSocks()
    {
        foreach (var sock in CollectedSocks)
        {
            try
            {
                sock.Activate();
            }
            catch
            {
                // ignored
            }
        }
        CollectedSocks.Clear();
    }
    
    public void ResetLevel()
    {
        SocksInLevel.Clear();
    }

    public void ResetDropOff()
    {
        foreach (var sock in DroppedOffSocks)
        {
            try
            {
                sock.Activate();
            }
            catch
            {
                // ignored
            }
        }
        DroppedOffSocks.Clear();
    }
    
}
