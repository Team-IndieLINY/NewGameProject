using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class CocktailTypeTable
{
    private static CocktailTypeTable _instance;
    private Dictionary<CocktailData.CocktailType, List<RecipeData>> _cocktailTypeTable;

    public static CocktailTypeTable Instance()
    {
        if (_instance == null)
        {
            _instance = new CocktailTypeTable();
        }

        return _instance;
    }

    private CocktailTypeTable()
    {
        _cocktailTypeTable = new Dictionary<CocktailData.CocktailType, List<RecipeData>>();
        

        var table = Resources.Load<RecipeTable>("MainRecipeTable");

        foreach (var recipeData in table.Datas)
        {

            if (_cocktailTypeTable.ContainsKey(recipeData.Cocktail.CocktailEmotionType))
            {
                _cocktailTypeTable[recipeData.Cocktail.CocktailEmotionType].Add(recipeData);
            }
            else
            {
                _cocktailTypeTable.Add(recipeData.Cocktail.CocktailEmotionType, new List<RecipeData> { recipeData });
            }
        }
    }

    [CanBeNull]
    public List<RecipeData> GetCocktailData(CocktailData.CocktailType cocktailType)
    {
        return _cocktailTypeTable.GetValueOrDefault(cocktailType, null);
    }
}
