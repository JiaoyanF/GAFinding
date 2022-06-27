using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelRoot : MonoBehaviour
{
    [SerializeField] private Button mBtnReset;
    [SerializeField] private Button mBtnNextGene;
    [SerializeField] private Button mBtnTenGene;
    [SerializeField] private Button mBtnEndGene;
    [SerializeField] private Text mTextGene;

    private MazeController mMazeController => MazeController.Instance;

    private void Start()
    {
        mBtnReset.onClick.AddListener(OnClickReset);
        mBtnNextGene.onClick.AddListener(OnClickNextGene);
        mBtnTenGene.onClick.AddListener(OnClickTenGene);
        mBtnEndGene.onClick.AddListener(OnClickEndGene);

        mTextGene.text = "";
    }

    private void OnClickReset()
    {
        mMazeController.InitGA();
        mTextGene.text = "";
    }

    private void OnClickNextGene()
    {
        mMazeController.Refresh();
        mTextGene.text = $"第{mMazeController.GA.Generation}代";
    }

    private void OnClickTenGene()
    {
        for (int i = 0; i < 10; i++)
        {
            mMazeController.Refresh();
            mTextGene.text = $"第{mMazeController.GA.Generation}代";
        }
    }

    private void OnClickEndGene()
    {
        while (mMazeController.GA.Running)
        {
            mMazeController.Refresh();
            mTextGene.text = $"第{mMazeController.GA.Generation}代";
        }
    }
}