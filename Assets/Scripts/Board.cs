using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
public enum DirectionMov { none, vertical, horizantal }
public enum GameStatus { none, gaming, finished, over }

public class Board : MonoBehaviour
{
    [Header("Features")]
    public int width;
    public int height;
    public Color colorFirst;
    Color currentColor;
    public float gemSpeed;
    public DirectionMov directionMov;
    public GameStatus gameStatus;

    [Header("Objects")]
    public GameObject bgTilePrefab;
    public Transform[] gems;
    public Transform[,] allGems;
    public Transform[] newGems;
    private MatchFinder matchFind;

    private void Awake()
    {
        matchFind = FindObjectOfType<MatchFinder>();
    }

    private void Start()
    {
        allGems = new Transform[width, height];

        // Setup();
    }
    private void Update()
    {
        if (gameStatus == GameStatus.finished)
        {
            matchFind.FindAllMatches();
            gameStatus = GameStatus.none;
        }
    }

    public void Setup()
    {
        for (int x = 0; x < width; x++)     // Creating lines
        {
            for (int y = 0; y < height; y++)  // Creating columns
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = EZ_PoolManager.Spawn(bgTilePrefab.transform, pos, Quaternion.identity).gameObject;

                SpriteRenderer imageComponent = bgTile.GetComponent<SpriteRenderer>();
                currentColor = imageComponent.color;

                if (x % 2 == 0 && y % 2 != 0 || x % 2 != 0 && y % 2 == 0)
                    currentColor = colorFirst;

                imageComponent.color = currentColor;

                int gemToUse = Random.Range(0, gems.Length);
                ControlGem(new Vector2Int(x, y), gems[gemToUse], gemToUse);
            }
        }
    }

    private void ControlGem(Vector2Int pos, Transform gemToSpawn, int prefabID)
    {
        int posX = pos.x - 1;
        int posY = pos.y - 1;
        GemType currentGemType = gemToSpawn.GetComponent<Gem>().gemType;
        GemType formerGemType;
        int sameGemsX = 0;
        int sameGemsY = 0;

        for (int i = 0; i < 2; i++)
        {
            if (posX >= 0)
            {
                formerGemType = allGems[posX, pos.y].GetComponent<Gem>().gemType;

                if (formerGemType == currentGemType)
                    sameGemsX++;

                else
                {
                    sameGemsX = 0;
                    break;
                }

                posX--;

            }
        }
        for (int i = 0; i < 2; i++)
        {
            if (posY >= 0)
            {
                formerGemType = allGems[pos.x, posY].GetComponent<Gem>().gemType;

                if (formerGemType == currentGemType)
                    sameGemsY++;

                else
                {
                    sameGemsY = 0;
                    break;
                }

                posY--;
            }
        }

        if (sameGemsX > 1 || sameGemsY > 1)
        {

            int gemToUse = Random.Range(0, gems.Length);
            ControlGem(pos, gems[gemToUse], gemToUse);
        }

        else
        {
            SpawnGem(pos, gemToSpawn, prefabID);
        }
    }

    public void SpawnGem(Vector2Int pos, Transform gemToSpawn, int prefabID)
    {
        Transform gem = EZ_PoolManager.Spawn(gemToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);

        allGems[pos.x, pos.y] = gem;
        gem.GetComponent<Gem>().SetupGem(pos, prefabID);
    }
}