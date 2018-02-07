using UnityEngine;

public class LevelPreview : LevelLoaderBase
{
    protected override void GenerateLevel()
    {
        Transform oldContainer = levelContainer;
        levelContainer = new GameObject("Preview Container").transform;
        base.GenerateLevel();
        Destroy(oldContainer.gameObject);
    }
}
