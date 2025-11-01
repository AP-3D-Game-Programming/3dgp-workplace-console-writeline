using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class TreesManager : MonoBehaviour
{
    public GameObject player;
    public GameObject[] treePrefabs;
    public GameObject[] BurnedtreePrefabs;
    private TerrainData terrainData;
    private Terrain terrain;
    private List<TreeModel> trees;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;
        trees = new List<TreeModel>();

        foreach (var treeInstance in terrainData.treeInstances)
        {
            trees.Add(new TreeModel()
            {
                TreeInstance = treeInstance,
                RealWorldPos = GetRealWorldTreePos(treeInstance),
                RealWorldRotation = GetQuaternionFromFloat(treeInstance.rotation)
            });
        }

        InvokeRepeating("HandleTreeSpawning", 0.5f, 0.5f);
        InvokeRepeating("BurnRandomTree", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void SpawnNearestTrees()
    {
        foreach (var tree in trees)
        {
            if (Vector3.Distance(player.transform.position, tree.RealWorldPos) <= 10f)
            {
                if (tree.RealTree == null)
                {
                    SpawnRealTree(tree);
                }

            }
        }
    }
    private void DestroyTreesOutOfBound()
    {
        foreach (var tree in trees)
        {
            if (tree.RealTree == null)
                continue;

            var isObjectOnFire = tree.RealTree.transform.Find("Fire Indicator")?.gameObject.active ?? false;
            var isObjectBurned = tree.RealTree.transform.GetComponent<TreeManager>()?.Burned ?? false;

            if (Vector3.Distance(player.transform.position, tree.RealWorldPos) > 10f && !isObjectOnFire && !isObjectBurned)
            {
                Destroy(tree.RealTree);
                tree.RealTree = null;
            }
        }
    }

    private void UpdateTerrainTrees()
    {
        TreeInstance[] toRenderTrees = trees.Where(t => t.RealTree == null).Select(t => t.TreeInstance).ToArray();
        terrainData.treeInstances = toRenderTrees;
    }
    private void HandleTreeSpawning()
    {
        SpawnNearestTrees();
        DestroyTreesOutOfBound();
        UpdateTerrainTrees();
    }

    /// <summary>
    /// Spawns a the RealTree if doesn't exist
    /// </summary>
    private void SpawnRealTree(TreeModel tree)
    {
        if (tree.RealTree != null)
            return;

        tree.RealTree = Instantiate(treePrefabs[tree.TreeInstance.prototypeIndex], tree.RealWorldPos, tree.RealWorldRotation);
        tree.setRealTreeCorrectScale();
        if (tree.TreeInstance.prototypeIndex < 2)
            tree.RealTree.GetComponent<TreeManager>().OnBurnedStateChange += ReplaceWithBurnedTree;
    }

    void ReplaceWithBurnedTree(TreeManager tree)
    {
        if (tree.Burned)
        {
            var treeModel = trees.Find(t => t.RealTree?.GetComponent<TreeManager>() == tree);
            Destroy(treeModel.RealTree);
            treeModel.RealTree = null;


            treeModel.RealTree = Instantiate(BurnedtreePrefabs[treeModel.TreeInstance.prototypeIndex], treeModel.RealWorldPos, treeModel.RealWorldRotation);
            treeModel.RealTree.GetComponent<TreeManager>().Burned = true;
            treeModel.setRealTreeCorrectScale();
            Debug.Log("DEAD TREE SHOULD SPAWN!!!");
        }
    }
    private void BurnRandomTree()
    {
        var i = -1;
        do i = Random.Range(0, trees.Count);
        while (trees[i].TreeInstance.prototypeIndex == 2);

        SpawnRealTree(trees[i]);

        Transform fireIndicator = trees[i].RealTree.transform.Find("Fire Indicator");
        fireIndicator.gameObject.SetActive(true);
    }

    // The following method was generated with assistance from ChatGPT (OpenAI)
    // https://chat.openai.com
    /// <summary>
    /// Returns a more accurate world-space position for a TreeInstance on the given terrain.
    /// Uses TerrainData.GetInterpolatedHeight and TransformPoint so it's accurate with scaled/rotated terrains.
    /// </summary>
    private Vector3 GetRealWorldTreePos(TreeInstance tree)
    {
        Vector3 size = terrainData.size;

        // Local-space X,Z inside the terrain (in terrain-local units)
        float localX = tree.position.x * size.x;
        float localZ = tree.position.z * size.z;

        // Get interpolated height from the heightmap (returned in terrain-local Y)
        // Pass normalized coords (0..1) to GetInterpolatedHeight
        float localY = terrainData.GetInterpolatedHeight(tree.position.x, tree.position.z);

        // Compose terrain-local position (relative to terrain pivot)
        Vector3 localPos = new Vector3(localX, localY, localZ);

        // Convert to world space (handles terrain.transform position, rotation, and scale)
        Vector3 worldPos = terrain.transform.TransformPoint(localPos);

        return worldPos;
    }

    private Quaternion GetQuaternionFromFloat(float rotation)
    {
        return Quaternion.Euler(0, rotation * Mathf.Rad2Deg ,0);
    }
}
