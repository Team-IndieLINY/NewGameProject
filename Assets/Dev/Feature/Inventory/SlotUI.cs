using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotUI : VisualElement
{
    private VisualElement _itemIcon;
    private Label _itemAmountLabel;
    private ProgressBar _palmingProgressBar;
    private CancellationTokenSource _cancellationTokenSource = new();

    public Slot Slot { get; set; }

    #region public method

    public SlotUI()
    {
        SetSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (Slot.Item == null)
        {
            _itemIcon.style.backgroundImage = null;
            _itemAmountLabel.text = " ";
            return;
        }

        _itemIcon.style.backgroundImage = Slot.Item.ItemData.ItemSprite.texture;

        if (Slot.Item is not CountableItem)
        {
            _itemAmountLabel.text = " ";
        }

        _itemAmountLabel.text = ((CountableItem)Slot.Item).CurrentAmount.ToString();
    }

    //해당 슬롯이 루팅되고 있는 지 알려주는 함수
    public bool IsLooting()
    {
        if (_palmingProgressBar.value != 0)
        {
            return true;
        }

        return false;
    }

    public void CancelLoot()
    {
        if (_cancellationTokenSource == null)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = null;
    }

    public void SetCancellaionTokenSoruce()
    {
        if (_cancellationTokenSource == null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }

    public async UniTask Loot()
    {
        SetCancellaionTokenSoruce();
        _palmingProgressBar.style.visibility = Visibility.Visible;

        while (_palmingProgressBar.value <= _palmingProgressBar.highValue)
        {
            try
            {
                await UniTask.Delay((int)Time.deltaTime * 1000, cancellationToken: _cancellationTokenSource.Token);
            }
            catch
            {
                _palmingProgressBar.style.visibility = Visibility.Hidden;
                _palmingProgressBar.value = 0;
                throw;
            }

            _palmingProgressBar.value +=
                _palmingProgressBar.highValue * Time.deltaTime / Slot.Item.ItemData.LootingTime;
        }

        _palmingProgressBar.style.visibility = Visibility.Hidden;
        _palmingProgressBar.value = 0;

        if (PlayerInventory.Instance.IsFull())
        {
            return;
        }

        if (Slot.Item is CountableItem)
        {
            Item item = new CountableItem(Slot.Item.ItemData as CountableItemData, 1);
            (item as CountableItem).SetAmount(1);

            PlayerInventory.Instance.AddItem(item);
        }
        else
        {
            // 셀 수 있는 아이템이 아니면 해당 타입 아이템으로 생성하여 추가해야함
        }

        Slot.RemoveItem();
    }

    #endregion

    #region private method

    private void SetSlotUI()
    {
        AddToClassList("slot");
        style.flexGrow = 0f;

        VisualElement itemFrameUI = new VisualElement();
        _itemIcon = new VisualElement();

        _itemAmountLabel = new Label();
        _itemAmountLabel.style.fontSize = 25;
        _itemAmountLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        _itemAmountLabel.style.alignSelf = Align.FlexEnd;
        _itemAmountLabel.pickingMode = PickingMode.Ignore;

        _palmingProgressBar = new ProgressBar();

        Add(itemFrameUI);

        itemFrameUI.AddToClassList("item_frame");
        itemFrameUI.Add(_itemIcon);
        itemFrameUI.pickingMode = PickingMode.Ignore;

        _itemIcon.AddToClassList("item_icon");
        _itemIcon.Add(_palmingProgressBar);
        _itemIcon.Add(_itemAmountLabel);
        _itemIcon.pickingMode = PickingMode.Ignore;

        _palmingProgressBar.style.visibility = Visibility.Hidden;
        _palmingProgressBar.pickingMode = PickingMode.Ignore;

        RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);
        RegisterCallback<ClickEvent>(OnClickEvent);
        RegisterCallback<MouseMoveEvent>(OnMouseMove);
    }

    private void OnMouseEnterEvent(MouseEnterEvent evt)
    {
        AddToClassList("highlighted_slot");
        
        //Caution: 인벤토리 종류가 확장되면 아래 코드는 위험할 수 있음
        Inventory instance = panel.visualTree.name == "PlayerInventory" ? PlayerInventory.Instance : BoxInventory.Instance;
        
        if (Slot.IsEmpty())
        {
            instance.CloseTooltipUI();
            return;
        }
        
        instance.SetTooltipUI(Slot.Item);
        instance.SetTooltipUIPosition(this.ChangeCoordinatesTo(parent, evt.localMousePosition));
        instance.OpenTooltipUI();
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        Inventory instance = panel.visualTree.name == "PlayerInventory" ? PlayerInventory.Instance : BoxInventory.Instance;
        
        instance.SetTooltipUIPosition(this.ChangeCoordinatesTo(parent, evt.localMousePosition));
    }

    private void OnClickEvent(ClickEvent evt)
    {
        if (Slot.IsEmpty())
            return;
        
        if (panel.visualTree.name == "BoxInventory")
        {
            RemoveFromClassList("highlighted_slot");

            if (_cancellationTokenSource == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            if (BoxInventory.Instance.IsLooting())
            {
                BoxInventory.Instance.CancelLooting();
                return;
            }
            
            if (_palmingProgressBar.value != 0)
            {
                CancelLoot();
            }
            else
            {
                //TODO: gajuho 해당 주석 확인시 제거
                //Loot(); <-- not awaited 경고를 발생시킴. _ 키워드 적극 사용 바람.
                _ = Loot();
            }
        }
        else if (panel.visualTree.name == "PlayerInventory")
        {
            if (BoxInventory.Instance.IsOpened() is false)
            {
                return;
            }
            RemoveFromClassList("highlighted_slot");
            
            if (_cancellationTokenSource == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            } 

            if (BoxInventory.Instance.IsLooting())
            {
                BoxInventory.Instance.CancelLooting();
                return;
            }

            Item item = Slot.Item.Clone() as Item;

            if (item is CountableItem)
            {
                (item as CountableItem).SetAmount(1);
            }
            
            BoxInventory.Instance.AddItem(item);
            Slot.RemoveItem();
        }
    }

    private void OnMouseLeaveEvent(MouseLeaveEvent evt)
    {
        RemoveFromClassList("highlighted_slot");
        
        //Caution: 인벤토리 종류가 확장되면 아래 코드는 위험할 수 있음
        Inventory instance = panel.visualTree.name == "PlayerInventory" ? PlayerInventory.Instance : BoxInventory.Instance;
        
        instance.CloseTooltipUI();
    }

    #endregion
}