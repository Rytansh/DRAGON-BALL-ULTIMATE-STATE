using UnityEngine;

public interface IBootstrapProcess
{
    int Order {get;}
    void Initialise(WorldContext context);
}
