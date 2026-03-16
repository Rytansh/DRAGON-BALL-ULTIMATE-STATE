using Unity.Entities;
using System.Collections.Generic;
using Unity.Collections;
using DBUS.Battle.Components.Events;

public struct BehaviourExecutionComparer : IComparer<BehaviourExecutionRequest>
{
    public int Compare(BehaviourExecutionRequest a, BehaviourExecutionRequest b)
    {
        int priorityCompare = b.Priority.CompareTo(a.Priority);
        if (priorityCompare != 0)
            return priorityCompare;
        return a.RegistrationIndex.CompareTo(b.RegistrationIndex);
    }
}
