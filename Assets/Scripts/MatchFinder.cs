using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    private CanvasController canvas;
    public List<Transform> currentMatches = new List<Transform>();
    bool isMatched;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        canvas = FindObjectOfType<CanvasController>();
    }

    public void FindAllMatches()
    {
        isMatched = false;

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Transform currentGem = board.allGems[x, y];
                if (currentGem != null)
                {
                    if (x > 0 && x < board.width - 1)
                    {
                        Transform leftGem = board.allGems[x - 1, y];
                        Transform rightGem = board.allGems[x + 1, y];

                        if (leftGem != null && rightGem != null)
                        {
                            if (leftGem.GetComponent<Gem>().gemType == currentGem.GetComponent<Gem>().gemType && rightGem.GetComponent<Gem>().gemType == currentGem.GetComponent<Gem>().gemType)
                            {
                                leftGem.GetComponent<Gem>().isMatched = true;
                                rightGem.GetComponent<Gem>().isMatched = true;
                                currentGem.GetComponent<Gem>().isMatched = true;

                                leftGem.tag = "isMatched";
                                rightGem.tag = "isMatched";
                                currentGem.tag = "isMatched";

                                canvas.UpdateTask(currentGem.GetComponent<Gem>().gemPrefab);

                                isMatched = true;
                            }
                        }
                    }
                    if (y > 0 && y < board.height - 1)
                    {
                        Transform aboveGem = board.allGems[x, y + 1];
                        Transform belowGem = board.allGems[x, y - 1];

                        if (aboveGem != null && belowGem != null)
                        {
                            if (aboveGem.GetComponent<Gem>().gemType == currentGem.GetComponent<Gem>().gemType && belowGem.GetComponent<Gem>().gemType == currentGem.GetComponent<Gem>().gemType)
                            {
                                aboveGem.GetComponent<Gem>().isMatched = true;
                                belowGem.GetComponent<Gem>().isMatched = true;
                                currentGem.GetComponent<Gem>().isMatched = true;

                                aboveGem.tag = "isMatched";
                                belowGem.tag = "isMatched";
                                currentGem.tag = "isMatched";

                                canvas.UpdateTask(currentGem.GetComponent<Gem>().gemPrefab);

                                isMatched = true;
                            }
                        }
                    }
                }
            }
        }

        if (isMatched)
        {
            StartCoroutine(DestroyGems(.5f));
        }
        else
        {
            board.gameStatus = GameStatus.none;
        }
    }

    IEnumerator DestroyGems(float second)
    {
        yield return new WaitForSeconds(second);


        GameObject[] matches = GameObject.FindGameObjectsWithTag("isMatched");

        foreach (var item in matches)
        {
            item.tag = "Untagged";
            board.allGems[item.GetComponent<Gem>().posIndex.x, item.GetComponent<Gem>().posIndex.y] = null;
            EZ_PoolManager.Despawn(item.transform);
        }

        StartCoroutine(DecreaseRowCo(.2f));

    }

    private IEnumerator DecreaseRowCo(float second)
    {
        yield return new WaitForSeconds(second);
        isMatched = false;

        int nullCounter = 0;

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (board.allGems[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    board.allGems[x, y].GetComponent<Gem>().posIndex.y -= nullCounter;
                    board.allGems[x, y - nullCounter] = board.allGems[x, y];
                    board.allGems[x, y] = null;
                }

            }

            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(.5f);

        FindAllMatches();
    }
    private void RefillBoard()
    {
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (board.allGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, board.gems.Length);

                    board.SpawnGem(new Vector2Int(x, y), board.gems[gemToUse], gemToUse);
                }
            }
        }

        CheckMisplacedGems();
    }

    private void CheckMisplacedGems()
    {
        List<Transform> foundGems = new List<Transform>();

        Gem[] gemComponents = FindObjectsOfType<Gem>();

        foreach (Gem gem in gemComponents)
        {
            foundGems.Add(gem.transform);
        }


        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (foundGems.Contains(board.allGems[x, y]))
                {
                    foundGems.Remove(board.allGems[x, y]);
                }
            }
        }

        foreach (Transform g in foundGems)
        {
            Destroy(g.gameObject);
        }
    }

}