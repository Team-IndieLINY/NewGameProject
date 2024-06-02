using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class EvaluateTest : MonoBehaviour
{
    [SerializeField] private HologramUI _hologramUI;
    [SerializeField] private DrinkData _testData;
    [SerializeField] private DrinkData _testData2;
    [SerializeField] private DrinkData _testData3;
    // Start is called before the first frame update

    [ButtonMethod]
    public void Test1()
    {
        _hologramUI.EvaluateMaterialBlock(_testData, HologramUI.EvaluateType.Bad);
    }
    
    [ButtonMethod]
    public void Test2()
    {
        _hologramUI.EvaluateMaterialBlock(_testData2, HologramUI.EvaluateType.Good);
    }
    
    [ButtonMethod]
    public void Test3()
    {
        _hologramUI.EvaluateMaterialBlock(_testData3, HologramUI.EvaluateType.Perfect);
    }
}
