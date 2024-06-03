using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "IndieLINY/MiniGame/CocktailData", fileName = "new cocktail data")]
public class CocktailData : ScriptableObject
{
    public enum CocktailType
    {
        FeelGood,
        Warmth,
        Courage,
        Stability,
        Consolation,
        Flutter
    }
    
    [field: SerializeField, Foldout("칵테일 정보"), PropertyName("칵테일 타입")] 
    private CocktailType _cocktailEmotionType;
    public CocktailType CocktailEmotionType => _cocktailEmotionType;
    
    [field: SerializeField, Foldout("칵테일 정보"), PropertyName("칵테일 이름")] 
    private string _cocktailName;
    public string CocktailName => _cocktailName;
    
    [field: SerializeField, Foldout("칵테일 정보"), PropertyName("칵테일 설명")] 
    private string _cockTailDescription;
    public string CockTailDescription => _cockTailDescription;
    
    [field: SerializeField, Foldout("칵테일 정보"), PropertyName("칵테일 글씨 색")] 
    private Color _resultTextColor = Color.white;
    public Color ResultTextColor => _resultTextColor;
    
    [field: SerializeField, Foldout("칵테일 정보"), PropertyName("칵테일 스프라이트")] 
    private Sprite _cocktailSprite;
    public Sprite CocktailSprite => _cocktailSprite;

    [field: SerializeField, Foldout("칵테일 정보"), PropertyName("칵테일 가격")]
    private int _cocktailPrice;
    public int CocktailPrice => _cocktailPrice;
    
    [SerializeField] private GameObject _prefab;

    public GameObject ClonePrefab()
    {
        return GameObject.Instantiate(_prefab);
    }
}
