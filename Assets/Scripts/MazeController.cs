using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 迷宫管理
/// </summary>
public class MazeController : SingletonMonoBehaviour<MazeController>
{
    [SerializeField] private Transform mRoot;
    [SerializeField] private GameObject mStartPrefab;
    [SerializeField] private GameObject mEndPrefab;
    [SerializeField] private GameObject mWallPrefab;
    [SerializeField] private GameObject mRoadPrefab;
    [SerializeField] private GameObject mPathPrefab;

    public GeneticAlgorithm GA; // 遗传算法
    private int[,] mMapStructure; // 地图结构
    private Vector2 mStartPos; // 结束坐标
    private Vector2 mEndPos; // 终点坐标
    private List<int> mSuitablePath; // 合适的路径
    private List<GameObject> mPathBlocks; // 路径块

    private void Start()
    {
        mMapStructure = new int[,]
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
            {8, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
            {1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1},
            {1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1},
            {1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1},
            {1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 5},
            {1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };
        CreateMap();
        InitGA();

        mStartPos = new Vector2(14f, 7f);
        mEndPos = new Vector2(0f, 2f);
        mSuitablePath = new List<int>();
        mPathBlocks = new List<GameObject>();
    }

    /// <summary>
    /// 创建地图
    /// </summary>
    private void CreateMap()
    {
        Debug.Log("length(x)=" + mMapStructure.GetLength(1));
        Debug.Log("length(y)=" + mMapStructure.GetLength(0));

        for (int y = 0; y < mMapStructure.GetLength(0); y++)
        {
            for (int x = 0; x < mMapStructure.GetLength(1); x++)
            {
                GameObject prefab = GetTagPrefab(mMapStructure[y, x]);
                if (prefab != null)
                {
                    GameObject obj = Instantiate(prefab);
                    obj.transform.SetParent(mRoot);
                    obj.transform.position = new Vector2(x, -y);
                }
            }
        }
    }

    /// <summary>
    /// 通过标签获取预制体
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private GameObject GetTagPrefab(int tag)
    {
        GameObject prefab;
        switch (tag)
        {
            case 0:
                prefab = mRoadPrefab;
                break;
            case 1:
                prefab = mWallPrefab;
                break;
            case 5:
                prefab = mStartPrefab;
                break;
            case 8:
                prefab = mEndPrefab;
                break;
            default:
                prefab = null;
                break;
        }

        return prefab;
    }

    /// <summary>
    /// 渲染这一代的染色体路径
    /// </summary>
    private void RenderSuitableChromosomePath()
    {
        ClearPathBlocks();
        Unit niceUnit = GA.GetCurBestUnit();
        if (niceUnit == null) return;

        List<int> niceDirections = GA.Decode(niceUnit.Genome);
        Vector2 position = mStartPos;

        foreach (int direction in niceDirections)
        {
            position = Move(position, direction);
            GameObject pathBlock = Instantiate(mPathPrefab);
            pathBlock.transform.SetParent(mRoot);
            pathBlock.transform.position = new Vector2(position.x, -position.y);
            mPathBlocks.Add(pathBlock);
        }

        Debug.Log($"路径块个数：{mPathBlocks.Count}");
    }

    /// <summary>
    /// 清除路径块
    /// </summary>
    private void ClearPathBlocks()
    {
        if (mPathBlocks == null) return;
        foreach (GameObject block in mPathBlocks)
        {
            Destroy(block);
        }

        mPathBlocks.Clear();
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector2 Move(Vector2 position, int direction)
    {
        switch (direction)
        {
            case 0: // 北
                if (position.y + 1 >= mMapStructure.GetLength(0) || mMapStructure[(int) (position.y + 1), (int) position.x] == 1)
                {
                    break;
                }
                else
                {
                    position.y += 1;
                }

                break;
            case 1: // 南
                if (position.y - 1 < 0 || mMapStructure[(int) (position.y - 1), (int) position.x] == 1)
                {
                    break;
                }
                else
                {
                    position.y -= 1;
                }

                break;
            case 2: // 东
                if (position.x + 1 >= mMapStructure.GetLength(1) || mMapStructure[(int) position.y, (int) (position.x + 1)] == 1)
                {
                    break;
                }
                else
                {
                    position.x += 1;
                }

                break;
            case 3: // 西
                if (position.x - 1 < 0 || mMapStructure[(int) position.y, (int) (position.x - 1)] == 1)
                {
                    break;
                }
                else
                {
                    position.x -= 1;
                }

                break;
        }

        return position;
    }

    public void Refresh()
    {
        if (GA == null)
        {
            InitGA();
        }
        if (GA.Running)
        {
            GA.Epoch();
        }

        RenderSuitableChromosomePath();
    }

    public void InitGA()
    {
        ClearPathBlocks();
        GA = new GeneticAlgorithm();
        GA.Run();
    }

    /// <summary>
    /// 获取路线得分
    /// </summary>
    /// <param name="directions"></param>
    /// <returns></returns>
    public double GetRouteScore(List<int> directions)
    {
        Vector2 position = mStartPos;

        foreach (int direction in directions)
        {
            position = Move(position, direction);
        }

        // 通过坐标差值计算分数
        Vector2 deltaPosition = new Vector2(Math.Abs(position.x - mEndPos.x), Math.Abs(position.y - mEndPos.y));
        double result = 1 / (double) (deltaPosition.x + deltaPosition.y + 1);

        return result;
    }
}