using System.Collections.Generic;
using UnityEngine;

public interface IDependency
{
    public bool InstanceExists();
    public GameObject GetInstance();
    public List<GameObject> GetInstances();
}