using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HologramUI : MonoBehaviour
{
    [SerializeField] private Transform _hologramButtonPosTransform;
    [SerializeField] private Transform _hologramPosTransform;
    [SerializeField] private Transform _hologramPanelTransform;

    [SerializeField] private Button _hologramButton;

    [SerializeField] private GameObject _hologramHomePanel;
    [SerializeField] private GameObject _hologramRecipePanel;
    [SerializeField] private GameObject _hologramLogPanel;
    [SerializeField] private GameObject _hologramSNSPanel;

    [SerializeField] private Color32[] _cocktailCategoryColors;


    [SerializeField] private GameObject _selectedButtonBackground;
    [SerializeField] private GameObject _cocktailSelectionBackground;
    
    [SerializeField] private GameObject _cocktailSelectionFrame;
    [SerializeField] private GameObject _cocktailFirstSelectionButton;

    [SerializeField] private List<Image> _cocktailButtonImages;
    [SerializeField] private List<GameObject> _materialBlocks;

    [SerializeField] private RectTransform _hologramSummaryRectTransform;

    [SerializeField] private GameObject _hologramSummaryPanel;
    [SerializeField] private RectTransform _hologramExpandPosition;
    [SerializeField] private CanvasGroup _recipeCanvasGroup;

    [SerializeField] private GameObject[] _recipePreviewMaterialBlocks = new GameObject[3];
    [SerializeField] private Sprite[] _materialEvaulationSprites;
    [SerializeField] private Sprite _materialTodoSprite;
    [SerializeField] private Sprite _materialProcessSprite;

    [SerializeField] private GameObject _textBlockPrefab;
    [SerializeField] private GameObject _logWindowContent;

    [field: SerializeField, Foldout("칵테일 정보 UI"), PropertyName("칵테일 이미지")]
    private Image _cocktailInfoImage;
    
    [field: SerializeField, Foldout("칵테일 정보 UI"), PropertyName("칵테일 이름")]
    private TextMeshProUGUI _cocktailInfoName;
    
    [field: SerializeField, Foldout("칵테일 정보 UI"), PropertyName("칵테일 설명")]
    private TextMeshProUGUI _cocktailInfoDescription;
    
    [field: SerializeField, Foldout("칵테일 정보 UI"), PropertyName("칵테일 가격")]
    private TextMeshProUGUI _cocktailInfoPrice;

    private Dictionary<string, Color32> _cocktailCategoryColorTable;
    private Dictionary<string, CocktailData.CocktailType> _cocktailTypeTable;
    private RecipeData[] _recipeDatas = new RecipeData[3];
    private RecipeData _currentRecipeData;

    public event Action<RecipeData> OnClickPreview; 
    
    private void Awake()
    {
        _hologramPanelTransform.position = _hologramButtonPosTransform.position;
        _hologramPanelTransform.localScale = _hologramButtonPosTransform.localScale;

        _cocktailCategoryColorTable = new Dictionary<string, Color32>()
        {
            { "FeelGoodButton", _cocktailCategoryColors[0] },
            { "WarmthButton", _cocktailCategoryColors[1] },
            { "CourageButton", _cocktailCategoryColors[2] },
            { "StabilityButton", _cocktailCategoryColors[3] },
            { "ConsolationButton", _cocktailCategoryColors[4] },
            { "FlutterButton", _cocktailCategoryColors[5] }
        };

        _cocktailTypeTable = new Dictionary<string, CocktailData.CocktailType>()
        {
            { "FeelGoodButton", CocktailData.CocktailType.FeelGood },
            { "WarmthButton", CocktailData.CocktailType.Warmth },
            { "CourageButton", CocktailData.CocktailType.Courage },
            { "StabilityButton", CocktailData.CocktailType.Stability },
            { "ConsolationButton", CocktailData.CocktailType.Consolation },
            { "FlutterButton", CocktailData.CocktailType.Flutter }
        };
        
        LoadCocktailDatas(CocktailData.CocktailType.FeelGood);
        LoadCocktailDetails(_recipeDatas[0]);
    }

    public void OnClickOpenButton()
    {
        _hologramButton.gameObject.SetActive(false);
        _hologramPanelTransform.DOMove(_hologramPosTransform.position, 0.5f);
        _hologramPanelTransform.DOScale(_hologramPosTransform.localScale, 0.5f);
    }

    public void OnClickCloseButton()
    {
        _hologramPanelTransform.DOMove(_hologramButtonPosTransform.position, 0.3f);
        _hologramPanelTransform.DOScale(_hologramButtonPosTransform.localScale, 0.3f);
        _hologramButton.gameObject.SetActive(true);
    }

    public void OnClickHomeButton()
    {
        _hologramHomePanel.SetActive(true);
        _hologramRecipePanel.SetActive(false);
        _hologramSNSPanel.SetActive(false);
    }

    public void OnClickRecipeIconButton()
    {
        _hologramHomePanel.SetActive(false);
        _hologramRecipePanel.SetActive(true);
    }

    public void OnClickSNSIconButton()
    {
        _hologramHomePanel.SetActive(false);
        _hologramSNSPanel.SetActive(true);
    }

    public void OnClickLogButton()
    {
        _hologramLogPanel.SetActive(true);
    }

    public void OnClickCloseLogButton()
    {
        _hologramLogPanel.SetActive(false);
    }

    public void OnClickPreviewRecipeButton()
    {
        if (_currentRecipeData == null)
        {
            return;
        }
        _hologramRecipePanel.SetActive(false);
        _hologramSummaryPanel.SetActive(true);
        _hologramPanelTransform.gameObject.GetComponent<RectTransform>()
            .DOSizeDelta(_hologramSummaryRectTransform.sizeDelta, 0.3f);
        _hologramPanelTransform.gameObject.GetComponent<RectTransform>()
            .DOAnchorPos(_hologramSummaryRectTransform.anchoredPosition, 0.3f);

        if (_currentRecipeData.MeansurementParameter1.DrinkData == null)
        {
            _recipePreviewMaterialBlocks[0].SetActive(false);
        }
        else
        {
            _recipePreviewMaterialBlocks[0].SetActive(true);
            _recipePreviewMaterialBlocks[0].transform.GetChild(0).GetComponent<Image>().sprite =
                _currentRecipeData.MeansurementParameter1.DrinkData.Sprite;
            _recipePreviewMaterialBlocks[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                _currentRecipeData.MeansurementParameter1.DrinkData.Name;
        }

        
        if (_currentRecipeData.MeansurementParameter2.DrinkData == null)
        {
            _recipePreviewMaterialBlocks[1].SetActive(false);
        }
        else
        {
            _recipePreviewMaterialBlocks[1].SetActive(true);
            _recipePreviewMaterialBlocks[1].transform.GetChild(0).GetComponent<Image>().sprite =
                _currentRecipeData.MeansurementParameter2.DrinkData.Sprite;
            _recipePreviewMaterialBlocks[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                _currentRecipeData.MeansurementParameter2.DrinkData.Name;
        }

        
        if (_currentRecipeData.MeansurementParameter3.DrinkData == null)
        {
            _recipePreviewMaterialBlocks[2].SetActive(false);
        }
        else
        {
            _recipePreviewMaterialBlocks[2].SetActive(true);
            _recipePreviewMaterialBlocks[2].transform.GetChild(0).GetComponent<Image>().sprite =
                _currentRecipeData.MeansurementParameter3.DrinkData.Sprite;
            _recipePreviewMaterialBlocks[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                _currentRecipeData.MeansurementParameter3.DrinkData.Name;
        }
        
        if (_currentRecipeData.MeansurementParameter4.DrinkData == null)
        {
            _recipePreviewMaterialBlocks[3].SetActive(false);
        }
        else
        {
            _recipePreviewMaterialBlocks[3].SetActive(true);
            _recipePreviewMaterialBlocks[3].transform.GetChild(0).GetComponent<Image>().sprite =
                _currentRecipeData.MeansurementParameter4.DrinkData.Sprite;
            _recipePreviewMaterialBlocks[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                _currentRecipeData.MeansurementParameter4.DrinkData.Name;
        }

        OnClickPreview?.Invoke(_currentRecipeData);
    }

    public void OnExpandRecipeButton()
    {
        _hologramSummaryPanel.SetActive(false);
        _hologramPanelTransform.gameObject.GetComponent<RectTransform>()
            .DOSizeDelta(_hologramExpandPosition.sizeDelta, 0.3f);
        _hologramPanelTransform.gameObject.GetComponent<RectTransform>()
            .DOAnchorPos(_hologramExpandPosition.anchoredPosition, 0.2f)
            .OnComplete(() =>
            {
                _hologramRecipePanel.SetActive(true);
                _recipeCanvasGroup.alpha = 0f;
                _recipeCanvasGroup.DOFade(1f, 0.3f);
            });
    }
    
    public void CloseSummaryWindow()
    {
        GetComponent<CanvasGroup>().DOFade(0f, 0.3f)
            .OnComplete(() =>
            {
                _hologramPanelTransform.localScale = _hologramButtonPosTransform.localScale;
                _hologramPanelTransform.position = _hologramButtonPosTransform.position;
                _hologramPanelTransform.gameObject.GetComponent<RectTransform>().sizeDelta = _hologramExpandPosition.sizeDelta;
        
                _hologramHomePanel.SetActive(true);
                _hologramSummaryPanel.SetActive(false);
            })
            .OnKill(() =>
            {
                GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
                _hologramButton.gameObject.SetActive(true);
            });
    }
    
    public void EvaluateMaterialBlock(DrinkData drinkData, EMiniGameScore evaluateType)
    {
        if (drinkData == null)
        {
            return;
        }
        
        if (_currentRecipeData.MeansurementParameter1.DrinkData == drinkData)
        {
            _recipePreviewMaterialBlocks[0].GetComponent<Image>().sprite = _materialProcessSprite;
            _recipePreviewMaterialBlocks[0].transform.GetChild(2).gameObject.SetActive(true);
            _recipePreviewMaterialBlocks[0].transform.GetChild(2).GetComponent<Image>().sprite =
                _materialEvaulationSprites[(int)evaluateType];
        }
        
        if (_currentRecipeData.MeansurementParameter2.DrinkData == drinkData)
        {
            _recipePreviewMaterialBlocks[1].GetComponent<Image>().sprite = _materialProcessSprite;
            _recipePreviewMaterialBlocks[1].transform.GetChild(2).gameObject.SetActive(true);
            _recipePreviewMaterialBlocks[1].transform.GetChild(2).GetComponent<Image>().sprite =
                _materialEvaulationSprites[(int)evaluateType];
        }
        
        if (_currentRecipeData.MeansurementParameter3.DrinkData == drinkData)
        {
            _recipePreviewMaterialBlocks[2].GetComponent<Image>().sprite = _materialProcessSprite;
            _recipePreviewMaterialBlocks[2].transform.GetChild(2).gameObject.SetActive(true);
            _recipePreviewMaterialBlocks[2].transform.GetChild(2).GetComponent<Image>().sprite =
                _materialEvaulationSprites[(int)evaluateType];
        }
        
        if (_currentRecipeData.MeansurementParameter4.DrinkData == drinkData)
        {
            _recipePreviewMaterialBlocks[3].GetComponent<Image>().sprite = _materialProcessSprite;
            _recipePreviewMaterialBlocks[3].transform.GetChild(2).gameObject.SetActive(true);
            _recipePreviewMaterialBlocks[3].transform.GetChild(2).GetComponent<Image>().sprite =
                _materialEvaulationSprites[(int)evaluateType];
        }
    }

    public void __MiniGame_Reset__()
    {
        _recipePreviewMaterialBlocks[0].transform.GetChild(2).gameObject.SetActive(false);
        _recipePreviewMaterialBlocks[1].transform.GetChild(2).gameObject.SetActive(false);
        _recipePreviewMaterialBlocks[2].transform.GetChild(2).gameObject.SetActive(false);
        _recipePreviewMaterialBlocks[3].transform.GetChild(2).gameObject.SetActive(false);
    }

    public void LoadCocktailDatas(CocktailData.CocktailType cocktailType)
    {
        List<RecipeData> cocktailDatas = CocktailTypeTable.Instance().GetCocktailData(cocktailType);
        
        for (int i = 0; i < _cocktailButtonImages.Count; i++)
        {
            _cocktailButtonImages[i].sprite = null;
            _recipeDatas[i] = null;
        }
        for (int i = 0; i < _cocktailButtonImages.Count; i++)
        {
            _cocktailButtonImages[i].gameObject.SetActive(false);
        }

        if (cocktailDatas != null)
        {
            for (int i = 0; i < cocktailDatas.Count; i++)
            {
                _cocktailButtonImages[i].gameObject.SetActive(true);
                _cocktailButtonImages[i].sprite = cocktailDatas[i].Cocktail.CocktailSprite;
                _recipeDatas[i] = cocktailDatas[i];
            }
        }
    }

    public void LoadCocktailDetails(RecipeData recipeData)
    {
        _materialBlocks[0].SetActive(false);
        _materialBlocks[1].SetActive(false);
        _materialBlocks[2].SetActive(false);
        _materialBlocks[3].SetActive(false);
        _materialBlocks[4].SetActive(false);
        _materialBlocks[5].SetActive(false);
        
        if (recipeData == null)
        {
            _cocktailInfoImage.gameObject.SetActive(false);
            _cocktailInfoName.text = null;
            _cocktailInfoDescription.text = null;
            _cocktailInfoPrice.text = null;
            return;
        }
        
        _cocktailInfoImage.gameObject.SetActive(true);
        _cocktailInfoImage.sprite = recipeData.Cocktail.CocktailSprite;
        _cocktailInfoName.text = recipeData.Cocktail.CocktailName;
        _cocktailInfoDescription.text = recipeData.Cocktail.CockTailDescription;
        _cocktailInfoPrice.text = recipeData.Cocktail.CocktailPrice + "$";
        
        if (recipeData.MeansurementParameter1.DrinkData != null)
        {
            _materialBlocks[0].SetActive(true);
            _materialBlocks[0].transform.GetChild(0).GetComponent<Image>().sprite =
                recipeData.MeansurementParameter1.DrinkData.Sprite;
            _materialBlocks[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                recipeData.MeansurementParameter1.DrinkData.Name;
        }
        if (recipeData.MeansurementParameter2.DrinkData != null)
        {
            _materialBlocks[1].SetActive(true);
            _materialBlocks[1].transform.GetChild(0).GetComponent<Image>().sprite =
                recipeData.MeansurementParameter2.DrinkData.Sprite;
            _materialBlocks[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                recipeData.MeansurementParameter2.DrinkData.Name;
        }
        if (recipeData.MeansurementParameter3.DrinkData != null)
        {
            _materialBlocks[2].SetActive(true);
            _materialBlocks[2].transform.GetChild(0).GetComponent<Image>().sprite =
                recipeData.MeansurementParameter3.DrinkData.Sprite;
            _materialBlocks[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                recipeData.MeansurementParameter3.DrinkData.Name;
        }
        if (recipeData.MeansurementParameter4.DrinkData != null)
        {
            _materialBlocks[3].SetActive(true);
            _materialBlocks[3].transform.GetChild(0).GetComponent<Image>().sprite =
                recipeData.MeansurementParameter4.DrinkData.Sprite;
            _materialBlocks[3].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                recipeData.MeansurementParameter4.DrinkData.Name;
        }
    }
    
    public void OnClickCocktailCategory()
    {
        GameObject selectedCocktailButton = EventSystem.current.currentSelectedGameObject;
        string selectedCategoryButtonName = selectedCocktailButton.name;

        if (_cocktailCategoryColorTable.ContainsKey(selectedCategoryButtonName) is false)
        {
            return;
        }

        CocktailData.CocktailType cocktailType = _cocktailTypeTable[selectedCategoryButtonName];
        Color32 cocktailTypeColor = _cocktailCategoryColorTable[selectedCategoryButtonName];

        LoadCocktailDatas(cocktailType);
        
        LoadCocktailDetails(_recipeDatas[0]);
        _currentRecipeData = _recipeDatas[0];
        
        _selectedButtonBackground.GetComponent<Image>().DOColor(cocktailTypeColor, 0.2f);
        _cocktailSelectionBackground.GetComponent<Image>().DOColor(cocktailTypeColor,0.2f);
        
        _selectedButtonBackground.transform.DOMove(selectedCocktailButton.transform.position, 0.2f);

        _cocktailSelectionFrame.transform.DOMove(_cocktailFirstSelectionButton.transform.position, 0.2f);
    }

    public void OnClickCocktailSelectionButton()
    {
        GameObject selectedCocktailButton = EventSystem.current.currentSelectedGameObject;
        int selectedCocktailButtonIndex = selectedCocktailButton.transform.GetSiblingIndex();

        if (_recipeDatas[selectedCocktailButtonIndex] == null)
        {
            return;
        }

        _currentRecipeData = _recipeDatas[selectedCocktailButtonIndex];
        LoadCocktailDetails(_recipeDatas[selectedCocktailButtonIndex]);
        _cocktailSelectionFrame.transform.DOMove(selectedCocktailButton.transform.position, 0.2f);
    }

    public void AddTextBlock(string name, string script)
    {
        GameObject temp = Instantiate(_textBlockPrefab, _logWindowContent.transform);
        temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        temp.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = script;
    }
    public void ResetTextBlock()
    {
        while (_logWindowContent.transform.childCount != 0)
        {
            Destroy(_logWindowContent.transform.GetChild(0));
        }
    }
}
 