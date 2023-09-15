using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public enum GemType { blue, green, red, yellow }

public class Gem : MonoBehaviour
{
    [Header("Identity")]
    public Vector2Int posIndex;
    public GemType gemType;
    public int gemPrefab;

    [Header("Features")]
    private bool moving;
    private bool mousePressed = false;
    public bool isMatched;
    private float swipeAngle = 0;
    private Vector2 firstTouchPosition, finalTouchPosition;
    private Vector2Int distance;

    [Header("Objects")]
    [HideInInspector] public Board board;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > .01f)
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);

        else
        {
            if (moving)
            {
                transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
                moving = false;
                AllGemsUpdate();
                board.gameStatus = GameStatus.finished;
            }
        }

        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            moving = true;
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngel();
        }
    }

    public void SetupGem(Vector2Int pos, int prefabID)
    {
        posIndex = pos;
        gemPrefab = prefabID;
    }

    private void OnMouseDown()
    {
        if (board.gameStatus == GameStatus.none)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
            board.gameStatus = GameStatus.gaming;
        }
    }


    private void CalculateAngel()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
            MovePieces();
    }

    private void MovePieces()
    {
        Vector2 position2D = finalTouchPosition - firstTouchPosition;
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(Mathf.Abs(position2D.x)), Mathf.RoundToInt(Mathf.Abs(position2D.y)));
        distance = roundedPosition;

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)    // Swipe right
        {
            board.directionMov = DirectionMov.horizantal;
            board.newGems = new Transform[distance.x + board.width];
            Transform gem;

            for (int i = 0; i < board.newGems.Length; i++)
            {
                if (i < distance.x)
                {
                    Transform copiedGem = board.allGems[board.width - distance.x + i, posIndex.y];
                    gem = EZ_PoolManager.Spawn(board.gems[copiedGem.GetComponent<Gem>().gemPrefab], new Vector3(i - distance.x, posIndex.y, 0f), Quaternion.identity);

                    gem.GetComponent<Gem>().gemPrefab = copiedGem.GetComponent<Gem>().gemPrefab;
                }
                else
                {
                    gem = board.allGems[i - distance.x, posIndex.y];
                }
                gem.GetComponent<Gem>().posIndex = new Vector2Int(i, posIndex.y);
                board.newGems[i] = gem;
            }
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)    // Swipe up
        {
            board.directionMov = DirectionMov.vertical;
            board.newGems = new Transform[distance.y + board.height];
            Transform gem;

            for (int i = 0; i < board.newGems.Length; i++)
            {
                if (i < distance.y)
                {
                    Transform copiedGem = board.allGems[posIndex.x, board.height - distance.y + i];
                    gem = EZ_PoolManager.Spawn(board.gems[copiedGem.GetComponent<Gem>().gemPrefab], new Vector3(posIndex.x, i - distance.y, 0f), Quaternion.identity);

                    gem.GetComponent<Gem>().gemPrefab = copiedGem.GetComponent<Gem>().gemPrefab;
                }
                else
                {
                    gem = board.allGems[posIndex.x, i - distance.y];
                }
                gem.GetComponent<Gem>().posIndex = new Vector2Int(posIndex.x, i);
                board.newGems[i] = gem;
            }
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)    // Swipe Down
        {
            board.directionMov = DirectionMov.vertical;
            board.newGems = new Transform[distance.y + board.height];
            Transform gem;
            int x = 0, y = 0;

            for (int i = 0; i < board.newGems.Length; i++)
            {
                if (i < board.height - distance.y)
                {
                    gem = board.allGems[posIndex.x, i + distance.y];
                    gem.GetComponent<Gem>().posIndex = new Vector2Int(posIndex.x, i);
                }

                else if (i >= distance.x && i < board.height)
                {
                    Transform copiedGem = board.allGems[posIndex.y, i - distance.y];
                    gem = EZ_PoolManager.Spawn(board.gems[copiedGem.GetComponent<Gem>().gemPrefab], new Vector3(posIndex.x, board.height + y, 0f), Quaternion.identity);
                    gem.GetComponent<Gem>().gemPrefab = copiedGem.GetComponent<Gem>().gemPrefab;
                    gem.GetComponent<Gem>().posIndex = new Vector2Int(posIndex.x, i);
                    y++;
                }
                else
                {
                    gem = board.allGems[posIndex.x, x];
                    gem.GetComponent<Gem>().posIndex = new Vector2Int(posIndex.x, x - distance.y);
                    x++;
                }

                board.newGems[i] = gem;
            }
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)    // Swipe left
        {
            board.directionMov = DirectionMov.horizantal;
            board.newGems = new Transform[distance.x + board.width];
            Transform gem;
            int x = 0, y = 0;

            for (int i = 0; i < board.newGems.Length; i++)
            {
                if (i < board.width - distance.x)
                {
                    gem = board.allGems[i + distance.x, posIndex.y];
                    gem.GetComponent<Gem>().posIndex = new Vector2Int(i, posIndex.y);
                }

                else if (i >= distance.x && i < board.width)
                {
                    Transform copiedGem = board.allGems[i - distance.x, posIndex.y];
                    gem = EZ_PoolManager.Spawn(board.gems[copiedGem.GetComponent<Gem>().gemPrefab], new Vector3(board.width + y, posIndex.y, 0f), Quaternion.identity);
                    gem.GetComponent<Gem>().gemPrefab = copiedGem.GetComponent<Gem>().gemPrefab;
                    gem.GetComponent<Gem>().posIndex = new Vector2Int(i, posIndex.y);
                    y++;
                }
                else
                {
                    gem = board.allGems[x, posIndex.y];
                    gem.GetComponent<Gem>().posIndex = new Vector2Int(x - distance.x, posIndex.y);
                    x++;
                }

                board.newGems[i] = gem;
            }
        }

    }

    private void AllGemsUpdate()
    {
        if (board.directionMov == DirectionMov.horizantal)
        {
            for (int i = 0; i < board.newGems.Length; i++)
            {
                if (i >= board.width)
                {
                    EZ_PoolManager.Despawn(board.newGems[i]);
                }
                else
                {
                    board.allGems[i, posIndex.y] = board.newGems[i];
                }
            }
        }
        else if (board.directionMov == DirectionMov.vertical)
        {
            for (int i = 0; i < board.newGems.Length; i++)
            {
                if (i >= board.height)
                {
                    EZ_PoolManager.Despawn(board.newGems[i]);
                }
                else
                {
                    board.allGems[posIndex.x, i] = board.newGems[i];
                }
            }
        }
    }
}