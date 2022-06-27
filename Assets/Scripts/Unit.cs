using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子代
/// </summary>
public class Unit
{
    public List<int> Genome;// 基因组
    public double Score;

    public Unit()
    {
        Initialize();
    }

    public Unit(int numBits)
    {
        Initialize();

        // 二进制编码
        for (int i = 0; i < numBits; i++)
        {
            Genome.Add(GetRandomInt(0, 1));
        }
    }

    private void Initialize()
    {
        Score = 0;
        Genome = new List<int>();
    }

    /// <summary>
    /// 获取范围内随机整数（避免伪随机的情况）
    /// </summary>
    /// <param name="minNum">最小值（包含）</param>
    /// <param name="MaxNum">最大值（包含）</param>
    /// <returns></returns>
    public static int GetRandomInt(int minNum, int MaxNum)
    {
        return new System.Random(Guid.NewGuid().GetHashCode()).Next(minNum, MaxNum + 1);
    }
}