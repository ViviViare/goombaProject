using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveItemFill : MonoBehaviour
{
    public static PassiveItemFill _instance;
    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] private Slider _activeItemCharge;
    [SerializeField] private Image _activeItemPrimary;
    [SerializeField] private Image _activeItemSecondary;
    [SerializeField] private Image _activeItemSecondaryMask;
    [SerializeField] private RawImage _activeItemFill;
    [SerializeField] private float _fillSpeedX = 0.5f;
    [SerializeField] private float _fillSpeedY = 0.33f;
    [SerializeField] private float _iconSizeOffset;
    [SerializeField] private float _iconAnimDuration;
    [SerializeField] private AnimationCurve _iconEasing;
    private RectTransform _rectTransform;
    public PickupType _currentPassive;


    private void Start()
    {
        UpdateChargeAmount(0);
        UpdatePassive(null, 100);
        TogglePassiveSprite(false);
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float offsetX = Time.time * _fillSpeedX;
        float offsetY = Time.time * _fillSpeedY;
        _activeItemFill.uvRect = new Rect(offsetX, offsetY, 1, 1);
    }

    public void TogglePassiveSprite(bool value)
    {
        _activeItemPrimary.enabled = value;
        _activeItemSecondary.enabled = value;
        _activeItemFill.enabled = value;
    }
    
    public void UpdatePassive(Sprite image, float newMaxValue)
    {
        TogglePassiveSprite(true);

        _activeItemPrimary.sprite = image;
        _activeItemSecondary.sprite = image;
        _activeItemCharge.maxValue = newMaxValue;
        UpdateChargeAmount(newMaxValue);
        StartCoroutine(PlayGrowthAnimation());
    }

    public void UpdateChargeAmount(float chargeAmount)
    {
        _activeItemCharge.value = chargeAmount;
        _activeItemSecondaryMask.fillAmount = (chargeAmount / _activeItemCharge.maxValue);
    }

    private IEnumerator PlayGrowthAnimation()
    {
        float elapsedTime = 0f;

        Vector3 startSize = _rectTransform.localScale;
        Vector3 targetSize = _rectTransform.localScale * _iconSizeOffset;

        while (elapsedTime < _iconAnimDuration)
        {
            float time = Mathf.Clamp01(elapsedTime / _iconAnimDuration);
            float curveValue = _iconEasing.Evaluate(time);

            Vector2 currentSize = Vector2.Lerp(startSize, targetSize, curveValue);
                        
            _rectTransform.localScale = currentSize;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rectTransform.localScale = startSize;
    }

}
