using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome Prefab Holder", menuName = "Biome Prefab Holder")]
public class BiomePrefabHolder : ScriptableObject
{
    public List<GameObject> biomePrefabs = new List<GameObject>();
    
    public GameObject GetRandomBiomePrefab()
    {
        return biomePrefabs[Random.Range(0, biomePrefabs.Count)];
    }

}
