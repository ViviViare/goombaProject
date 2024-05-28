/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 02/05/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  ActiveFillUpdate.cs
//
//  This script updates the active fill UI elements to show how many cleared rooms it will be until
//  the active item can be used
//
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  Edits since script finished:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveFillUpdate : MonoBehaviour
{
    public static ActiveFillUpdate _instance;
    private void Awake()
    {
        _instance = this;
        _rectTransform = _activeItemCharge.GetComponent<RectTransform>();
    }

    [SerializeField] private Slider _activeItemCharge;
    [SerializeField] private Image _activeItemPrimary;
    [SerializeField] private Image _activeItemSecondary;
    [SerializeField] private Image _activeItemSecondaryMask;
    [SerializeField] private RawImage _activeItemFill;
    [SerializeField] private float _fillSpeedX = 0.5f;
    [SerializeField] private float _fillSpeedY = 0.33f;

    [Header("Growth Animation")]
    [SerializeField] private float _iconSizeOffset;
    [SerializeField] private float _iconAnimDuration;
    [SerializeField] private AnimationCurve _iconEasing;
    private RectTransform _rectTransform;

    private void Start()
    {
        UpdateChargeAmount(0);
        UpdateActive(null, 100);
        ToggleActiveSprite(false);
        
    }

    private void Update()
    {
        float offsetX = Time.time * _fillSpeedX;
        float offsetY = Time.time * _fillSpeedY;
        _activeItemFill.uvRect = new Rect(offsetX, offsetY, 1, 1);
    }

    public void ToggleActiveSprite(bool value)
    {
        _activeItemPrimary.enabled = value;
        _activeItemSecondary.enabled = value;
        _activeItemFill.enabled = value;
    }

    public void UpdateActive(Sprite image, float newMaxValue)
    {
        ToggleActiveSprite(true);

        _activeItemPrimary.sprite = image;
        _activeItemSecondary.sprite = image;
        _activeItemCharge.maxValue = newMaxValue;
        StartCoroutine(PlayGrowthAnimation());
    }

    public void UpdateChargeAmount(float chargeAmount)
    {
        _activeItemCharge.value = chargeAmount;
        _activeItemSecondaryMask.fillAmount = (chargeAmount / _activeItemCharge.maxValue);

        if (_activeItemCharge.value >= _activeItemCharge.maxValue) StartCoroutine(PlayGrowthAnimation());
    }

    private IEnumerator PlayGrowthAnimation()
    {
        float elapsedTime = 0f;

        // Cache the starting size of the UI element
        Vector3 startSize = _rectTransform.localScale;
        // Cache the end goal size for the UI element
        Vector3 targetSize = _rectTransform.localScale * _iconSizeOffset;

        while (elapsedTime < _iconAnimDuration)
        {
            float time = Mathf.Clamp01(elapsedTime / _iconAnimDuration);
            float curveValue = _iconEasing.Evaluate(time);

            // Lerp between the start size and the target size while comparing it to the defined curve value
            // the curve esentially acts to make the animation more interesting via Ease in and out.
            Vector2 currentSize = Vector2.Lerp(startSize, targetSize, curveValue);
            
            _rectTransform.localScale = currentSize;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rectTransform.localScale = startSize;
    }

}
