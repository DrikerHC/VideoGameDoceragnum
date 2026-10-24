using System.Collections;
using System.Collections.Generic;
using Doceragnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    private const string GainColor = "#44DD66";
    private const string LossColor = "#EE4444";

    [SerializeField] private TextMeshProUGUI _MoneyText;
    [SerializeField] private TextMeshProUGUI _HappinessText;
    [SerializeField] private TextMeshProUGUI _RepitationText;
    [SerializeField] private TextMeshProUGUI _DayText;

    [Header("Day / Phase")]
    [SerializeField] private Image _PhaseIcon;
    [SerializeField] private Sprite[] _phaseSprites;

    [Header("Stat Icons")]
    [SerializeField] private Image _HappinessIcon;
    [SerializeField] private Sprite[] _happinessSprites;
    [SerializeField] private Image _ReputationIcon;
    [SerializeField] private Sprite _reputationPositiveSprite;
    [SerializeField] private Sprite _reputationNegativeSprite;
    [SerializeField] private Sprite _moneyFeedbackIcon;

    [Header("Stat Change Feedback")]
    [SerializeField] private GameObject _feedbackPanel;
    [SerializeField] private TextMeshProUGUI _feedbackText;
    [SerializeField] private float _feedbackDuration = 3f;
    [SerializeField] private Vector2 _feedbackPadding = new(24f, 18f);
    [SerializeField] private float _feedbackIconSize = 28f;

    private readonly List<PendingFeedback> _pendingFeedback = new();
    private readonly List<FeedbackRow> _feedbackRows = new();
    private RectTransform _feedbackPanelRect;
    private RectTransform _feedbackContent;
    private GameObject _feedbackRowTemplate;
    private Coroutine _batchCoroutine;
    private Coroutine _hideFeedbackCoroutine;
    private TMP_FontAsset _feedbackFont;
    private float _feedbackFontSize = 20f;

    private struct PendingFeedback
    {
        public bool IsStatChange;
        public StoreItemSO.BonusType Type;
        public int Delta;
        public string Message;
    }

    private sealed class FeedbackRow
    {
        public GameObject Root;
        public Image Icon;
        public TextMeshProUGUI Amount;
    }

    void Awake()
    {
        if (_feedbackText != null)
        {
            _feedbackFont = _feedbackText.font;
            _feedbackFontSize = _feedbackText.fontSize;
            _feedbackText.gameObject.SetActive(false);
        }
        else if (_MoneyText != null)
        {
            _feedbackFont = _MoneyText.font;
            _feedbackFontSize = _MoneyText.fontSize;
        }

        if (_feedbackPanel != null)
        {
            _feedbackPanelRect = _feedbackPanel.GetComponent<RectTransform>();
            _feedbackPanel.SetActive(false);
            SetupFeedbackRows();
        }
    }

    void Start()
    {
        UpdateUI(InGameDataManager.PlayerData.DineroActual, InGameDataManager.PlayerData.felicidadActual,
            InGameDataManager.PlayerData.ReputacionActual);
        UpdateDay(InGameDataManager.Data.DayCount);
        UpdatePhase(InGameDataManager.Data.CurrentPhase);

        GlobalEventHUD.OnValueChanged += HandleChange;
        GlobalEventHUD.OnMessage += HandleMessage;
        GlobalEventHUD.OnDayChanged += HandleDayChanged;
        GlobalEventHUD.OnPhaseChanged += HandlePhaseChanged;
    }

    void OnDestroy()
    {
        GlobalEventHUD.OnValueChanged -= HandleChange;
        GlobalEventHUD.OnMessage -= HandleMessage;
        GlobalEventHUD.OnDayChanged -= HandleDayChanged;
        GlobalEventHUD.OnPhaseChanged -= HandlePhaseChanged;
    }

    private void HandleChange(int prev, int curr, StoreItemSO.BonusType type)
    {
        switch (type)
        {
            case StoreItemSO.BonusType.HAPPINESS:
                UpdateHappiness(curr);
                break;
            case StoreItemSO.BonusType.MONEY:
                UpdateMoney(curr);
                break;
            case StoreItemSO.BonusType.REPUTATION:
                UpdateRep(curr);
                break;
        }

        int delta = curr - prev;
        if (delta != 0)
            ShowStatFeedback(type, delta);
    }

    private void HandleMessage(string message)
    {
        ShowFeedback(message);
    }

    public void UpdateUI(int money, int happiness, int reputation)
    {
        UpdateMoney(money);
        UpdateHappiness(happiness);
        UpdateRep(reputation);
    }

    private void UpdateMoney(int money)
    {
        _MoneyText.text = money + " x";
    }

    private void UpdateHappiness(int happiness)
    {
        _HappinessText.text = happiness + " x";
        UpdateHappinessIcon(happiness);
    }

    private void UpdateRep(int rep)
    {
        _RepitationText.text = rep + " x";
        UpdateReputationIcon(rep);
    }

    private void UpdateHappinessIcon(int happiness)
    {
        if (_HappinessIcon == null || _happinessSprites == null || _happinessSprites.Length == 0)
            return;

        int max = InGameDataManager.PlayerData.felicidadMax;
        if (max <= 0)
            max = 100;

        happiness = Mathf.Clamp(happiness, 0, max);
        int index = GetHappinessTierIndex(happiness, max);
        _HappinessIcon.sprite = _happinessSprites[index];
    }

    private static int GetHappinessTierIndex(int happiness, int max)
    {
        int lowThreshold = max / 4;
        int highThreshold = (max * 3) / 4;

        if (happiness <= lowThreshold)
            return 0;

        if (happiness <= highThreshold)
            return 1;

        return 2;
    }

    private void UpdateReputationIcon(int reputation)
    {
        if (_ReputationIcon == null)
            return;

        _ReputationIcon.sprite = reputation >= 0
            ? _reputationPositiveSprite
            : _reputationNegativeSprite;
    }

    private void HandleDayChanged(int day)
    {
        UpdateDay(day);
    }

    private void HandlePhaseChanged(int phase)
    {
        UpdatePhase(phase);
    }

    private void UpdateDay(int day)
    {
        if (_DayText != null)
            _DayText.text = "Day " + day.ToString();
    }

    private void UpdatePhase(int phase)
    {
        if (_PhaseIcon == null || _phaseSprites == null || _phaseSprites.Length == 0)
            return;

        int index = Mathf.Clamp(phase - 1, 0, _phaseSprites.Length - 1);
        _PhaseIcon.sprite = _phaseSprites[index];
    }

    private void ShowStatFeedback(StoreItemSO.BonusType type, int delta)
    {
        _pendingFeedback.Add(new PendingFeedback
        {
            IsStatChange = true,
            Type = type,
            Delta = delta
        });
        QueueFeedbackFlush();
    }

    private void ShowFeedback(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        _pendingFeedback.Add(new PendingFeedback
        {
            IsStatChange = false,
            Message = message
        });
        QueueFeedbackFlush();
    }

    private void QueueFeedbackFlush()
    {
        if (_feedbackPanel == null || _feedbackContent == null)
            return;

        if (_hideFeedbackCoroutine != null)
        {
            StopCoroutine(_hideFeedbackCoroutine);
            _hideFeedbackCoroutine = null;
            ClearFeedbackRows();
        }

        if (_batchCoroutine != null)
            StopCoroutine(_batchCoroutine);

        _batchCoroutine = StartCoroutine(FlushFeedbackNextFrame());
    }

    private IEnumerator FlushFeedbackNextFrame()
    {
        yield return null;
        _batchCoroutine = null;

        if (_pendingFeedback.Count == 0)
            yield break;

        PopulateFeedbackRows();
        _pendingFeedback.Clear();
        _feedbackPanel.SetActive(true);
        RefreshFeedbackLayout();

        _hideFeedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay());
    }

    private void PopulateFeedbackRows()
    {
        ClearFeedbackRows();

        for (int i = 0; i < _pendingFeedback.Count; i++)
        {
            var pending = _pendingFeedback[i];
            var row = GetFeedbackRow(i);
            row.Root.SetActive(true);

            if (pending.IsStatChange)
            {
                row.Icon.gameObject.SetActive(true);
                row.Icon.sprite = GetFeedbackIcon(pending.Type, pending.Delta);
                row.Amount.text = FormatDelta(pending.Delta);
            }
            else
            {
                row.Icon.gameObject.SetActive(false);
                row.Amount.text = pending.Message;
                row.Amount.color = Color.white;
            }
        }
    }

    private FeedbackRow GetFeedbackRow(int index)
    {
        while (_feedbackRows.Count <= index)
            _feedbackRows.Add(CreateFeedbackRow());

        return _feedbackRows[index];
    }

    private void ClearFeedbackRows()
    {
        foreach (var row in _feedbackRows)
            row.Root.SetActive(false);
    }

    private Sprite GetFeedbackIcon(StoreItemSO.BonusType type, int delta)
    {
        return type switch
        {
            StoreItemSO.BonusType.MONEY => _moneyFeedbackIcon,
            StoreItemSO.BonusType.HAPPINESS => GetHappinessFeedbackIcon(delta),
            StoreItemSO.BonusType.REPUTATION => delta >= 0
                ? _reputationPositiveSprite
                : _reputationNegativeSprite,
            _ => null
        };
    }

    private Sprite GetHappinessFeedbackIcon(int delta)
    {
        if (_happinessSprites == null || _happinessSprites.Length == 0)
            return null;

        if (_happinessSprites.Length == 1)
            return _happinessSprites[0];

        return delta >= 0
            ? _happinessSprites[_happinessSprites.Length - 1]
            : _happinessSprites[0];
    }

    private static string FormatDelta(int delta)
    {
        string color = delta > 0 ? GainColor : LossColor;
        string sign = delta > 0 ? "+" : string.Empty;
        return $"<color={color}>{sign}{delta}</color>";
    }

    private void SetupFeedbackRows()
    {
        var contentGo = new GameObject("FeedbackContent", typeof(RectTransform));
        _feedbackContent = contentGo.GetComponent<RectTransform>();
        _feedbackContent.SetParent(_feedbackPanel.transform, false);
        _feedbackContent.anchorMin = new Vector2(0.5f, 0.5f);
        _feedbackContent.anchorMax = new Vector2(0.5f, 0.5f);
        _feedbackContent.pivot = new Vector2(0.5f, 0.5f);
        _feedbackContent.anchoredPosition = Vector2.zero;

        var layout = contentGo.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 6f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.padding = new RectOffset(4, 4, 4, 4);

        var fitter = contentGo.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        _feedbackRowTemplate = CreateFeedbackRowObject(_feedbackContent);
        _feedbackRowTemplate.SetActive(false);
    }

    private GameObject CreateFeedbackRowObject(Transform parent)
    {
        var rowGo = new GameObject("RowTemplate", typeof(RectTransform));
        rowGo.transform.SetParent(parent, false);

        var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        var iconGo = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconGo.transform.SetParent(rowGo.transform, false);
        var iconRect = iconGo.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(_feedbackIconSize, _feedbackIconSize);
        var icon = iconGo.GetComponent<Image>();
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        var amountGo = new GameObject("Amount", typeof(RectTransform), typeof(CanvasRenderer));
        amountGo.transform.SetParent(rowGo.transform, false);
        var amount = amountGo.AddComponent<TextMeshProUGUI>();
        amount.font = _feedbackFont;
        amount.fontSize = _feedbackFontSize;
        amount.alignment = TextAlignmentOptions.MidlineLeft;
        amount.richText = true;
        amount.raycastTarget = false;
        amount.color = Color.white;

        return rowGo;
    }

    private FeedbackRow CreateFeedbackRow()
    {
        var rowGo = Instantiate(_feedbackRowTemplate, _feedbackContent);
        rowGo.SetActive(false);

        var amount = rowGo.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        amount.font = _feedbackFont;
        amount.fontSize = _feedbackFontSize;

        return new FeedbackRow
        {
            Root = rowGo,
            Icon = rowGo.transform.Find("Icon").GetComponent<Image>(),
            Amount = amount
        };
    }

    private void RefreshFeedbackLayout()
    {
        if (_feedbackPanelRect == null || _feedbackContent == null)
            return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_feedbackContent);
        _feedbackPanelRect.sizeDelta = new Vector2(
            _feedbackContent.sizeDelta.x + _feedbackPadding.x,
            _feedbackContent.sizeDelta.y + _feedbackPadding.y);
    }

    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(_feedbackDuration);
        _feedbackPanel.SetActive(false);
        ClearFeedbackRows();
        _hideFeedbackCoroutine = null;
    }
}
