using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 遗传算法
/// </summary>
public class GeneticAlgorithm
{
    public int Generation; // 当前为第n代
    public bool Running; // 当前状态

    private int mGeneLength = 2; // 基因长度
    private int mChromosomeLength = 70; // 染色体长度
    private int mPopulationSize = 140; // 人口数
    private double mMutationRate = 0.001f; // 突变系数
    private double mBestScore; // 最高分
    private double mTotalScores; // 总计分
    
    private List<Unit> mUnits; // 当前种群
    private List<Unit> mLastUnits; // 上一代种群
    public int mCurBestUnitIndex; // 当前最好的个体序号

    private MazeController mMazeController = MazeController.Instance;

    public GeneticAlgorithm()
    {
        Running = false;
        mUnits = new List<Unit>();
        mLastUnits = new List<Unit>();
    }

    /// <summary>
    /// 获取当前种群适应度最高的个体
    /// </summary>
    /// <returns></returns>
    public Unit GetCurBestUnit()
    {
        if (mUnits.Count == 0) return null;
        return mUnits[mCurBestUnitIndex];
    }

    public void Run()
    {
        CreateInitialPopulation();
        Running = true;
    }

    /// <summary>
    /// 生成新一代
    /// </summary>
    public void Epoch()
    {
        if (!Running) return;
        CheckScores();

        if (!Running)
        {
            mLastUnits.Clear();
            mLastUnits.AddRange(mUnits);
            return;
        }

        List<Unit> babies = new List<Unit>();
        while (babies.Count < mPopulationSize)
        {
            Unit mom = RouletteWheelSelection();
            Unit dad = RouletteWheelSelection();
            Unit baby1 = new Unit();
            Unit baby2 = new Unit();
            Crossover(mom.Genome, dad.Genome, baby1.Genome, baby2.Genome);
            Mutate(baby1.Genome);
            Mutate(baby2.Genome);
            babies.Add(baby1);
            babies.Add(baby2);
        }

        mLastUnits.Clear();
        mLastUnits.AddRange(mUnits);
        // 用新的一代覆盖当前基因组
        mUnits = babies;

        // 增加生成计数器
        Generation++;
    }

    /// <summary>
    /// 创建初始人口
    /// </summary>
    private void CreateInitialPopulation()
    {
        mUnits.Clear();

        for (int i = 0; i < mPopulationSize; i++)
        {
            Unit baby = new Unit(mChromosomeLength);
            mUnits.Add(baby);
        }
    }

    /// <summary>
    /// 评估个体适应度
    /// </summary>
    private void CheckScores()
    {
        mCurBestUnitIndex = 0;
        mBestScore = 0;
        mTotalScores = 0;

        for (int i = 0; i < mPopulationSize; i++)
        {
            List<int> directions = Decode(mUnits[i].Genome);

            mUnits[i].Score = mMazeController.GetRouteScore(directions);

            mTotalScores += mUnits[i].Score;

            if (mUnits[i].Score > mBestScore)
            {
                mBestScore = mUnits[i].Score;
                mCurBestUnitIndex = i;

                // 判断这条染色体是否找到出口
                if (mUnits[i].Score == 1)
                {
                    Debug.Log("找到出口！");
                    Running = false; // 停止运行
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 轮盘选择法
    /// </summary>
    /// <returns></returns>
    private Unit RouletteWheelSelection()
    {
        double slice = UnityEngine.Random.value * mTotalScores;
        double total = 0;
        int selectedIndex = 0;

        for (int i = 0; i < mPopulationSize; i++)
        {
            total += mUnits[i].Score;

            if (total > slice)
            {
                selectedIndex = i;
                break;
            }
        }

        return mUnits[selectedIndex];
    }

    /// <summary>
    /// 染色体交叉
    /// </summary>
    /// <param name="mom"></param>
    /// <param name="dad"></param>
    /// <param name="baby1"></param>
    /// <param name="baby2"></param>
    public void Crossover(List<int> mom, List<int> dad, List<int> baby1, List<int> baby2)
    {
        System.Random rnd = new System.Random();

        int crossoverPoint = rnd.Next(0, mChromosomeLength - 1);

        for (int i = 0; i < crossoverPoint; i++)
        {
            baby1.Add(mom[i]);
            baby2.Add(dad[i]);
        }

        for (int i = crossoverPoint; i < mom.Count; i++)
        {
            baby1.Add(dad[i]);
            baby2.Add(mom[i]);
        }
    }

    /// <summary>
    /// 染色体变异
    /// </summary>
    /// <param name="chrom"></param>
    public void Mutate(List<int> chrom)
    {
        for (int i = 0; i < chrom.Count; i++)
        {
            if (UnityEngine.Random.value < mMutationRate)
            {
                // 变异
                chrom[i] = chrom[i] == 0 ? 1 : 0;
            }
        }
    }

    /// <summary>
    /// 染色体解码（基因数据转换为方向）
    /// 0=北, 1=南, 2=东, 3=西
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    public List<int> Decode(List<int> chromosome)
    {
        List<int> directions = new List<int>();

        // 基因分组
        for (int geneIndex = 0; geneIndex < chromosome.Count; geneIndex += mGeneLength)
        {
            List<int> gene = new List<int>();

            for (int bitIndex = 0; bitIndex < mGeneLength; bitIndex++)
            {
                gene.Add(chromosome[geneIndex + bitIndex]);
            }

            directions.Add(GenesToInt(gene));
        }

        return directions;
    }

    /// <summary>
    /// 将基因组转换为方向
    /// </summary>
    /// <param name="genes"></param>
    /// <returns></returns>
    private int GenesToInt(List<int> genes)
    {
        int value = 0;
        int multiplier = 1;

        foreach (int gene in genes)
        {
            value += gene * multiplier;
            multiplier *= 2;
        }

        return value;
    }
}