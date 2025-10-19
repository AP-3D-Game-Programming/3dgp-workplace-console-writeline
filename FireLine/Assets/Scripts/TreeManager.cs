using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    private Terrain terrain;
    public GameObject player;
    public GameObject TreePrefab;
    private List<GameObject> realTrees;
    private TerrainData terrainData;
    private Dictionary<Vector3, TreeInstance> dicTree;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terrain = Terrain.activeTerrain;
        realTrees = new List<GameObject>();
        terrainData = terrain.terrainData;
        dicTree = new Dictionary<Vector3, TreeInstance>();


        foreach (var tree in terrainData.treeInstances)
        {
            AddTree(tree);
        }

        InvokeRepeating("BurnRandomTree", 1f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (dicTree.Count != 0)
        {
            SpawnNearestTrees();
        }
    }
    private void SpawnNearestTrees()
    {
        foreach (var key in dicTree.Keys.ToList())
        {
            if (Vector3.Distance(player.transform.position, key) <= 10f)
            {
                if (FindRealTree(key) != null)
                    continue;
                SpawnTree(key);

            }
        }
       // Despawn distant trees
        var toRemove = new List<GameObject>();
        foreach (var realTree in realTrees)
        {
            if (!realTree.transform.Find("Fire Indicator").gameObject.active &&
                Vector3.Distance(player.transform.position, realTree.transform.position) > 10f)
            {
                // Save position before destroying
                Vector3 treePos = realTree.transform.position;

                // Recreate the TreeInstance properly
                var treeInstance = new TreeInstance
                {
                    position = getNormalizedTreePos(realTree),
                    prototypeIndex = 0,  // <-- set correctly depending on your terrain setup
                    widthScale = 1,
                    heightScale = 1,
                    color = Color.white,
                    lightmapColor = Color.white,
                    rotation = 0
                };

                AddTree(treeInstance);
                toRemove.Add(realTree);
            }
        }

        // Remove after iteration to avoid modifying list during loop
        foreach (var realTree in toRemove)
        {
            realTrees.Remove(realTree);
            Destroy(realTree);
        }

        UpdateTreesTerrain();
    }
    private GameObject SpawnTree(Vector3 treePos)
    {
        var tmp = Instantiate(TreePrefab, treePos, GetQuaternionFromFloat(dicTree[treePos].rotation));
        realTrees.Add(tmp);
        dicTree.Remove(treePos);
        UpdateTreesTerrain();
        return tmp;
    }
    private Quaternion GetQuaternionFromFloat(float rotation)
    {
        return Quaternion.Euler(0, rotation * Mathf.Rad2Deg ,0);
    }
    private GameObject FindRealTree(Vector3 treePos)
    {
        foreach (var realTree in realTrees)
            if (Vector3.Distance(realTree.transform.position, treePos) < 0.1f)
                return realTree;
        return null;
    }
    private void BurnRandomTree()
    {
        var i = Random.Range(0, dicTree.Count);
        var key = dicTree.ElementAt(i).Key;

        var tree = FindRealTree(key);

        if (tree == null)
            tree = SpawnTree(key);

        Transform fireIndicator = tree.transform.Find("Fire Indicator");
        fireIndicator.gameObject.SetActive(true);
    }

    private void UpdateTreesTerrain()
    {
        terrain.terrainData.treeInstances = dicTree.Values.ToArray();
    }
    private void AddTree(TreeInstance tree)
    {
        var pos = Vector3.Scale(tree.position, terrainData.size) + terrain.transform.position;

        if (!dicTree.ContainsKey(pos))
            dicTree.Add(pos, tree);
    }
    private Vector3 getNormalizedTreePos(GameObject realTree)
    {
        return Vector3.Scale(
            realTree.transform.position - terrain.transform.position,
            new Vector3(1f / terrainData.size.x, 1f / terrainData.size.y, 1f / terrainData.size.z)
        );
    }
}
