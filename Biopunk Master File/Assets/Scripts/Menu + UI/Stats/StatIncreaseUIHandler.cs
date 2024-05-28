/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 09/05/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  StatIncreaseUIHandler.cs
//
//  Script that updates the stat UI: Damage and Speed stats.
//  
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatIncreaseUIHandler : MonoBehaviour
{
    public static StatIncreaseUIHandler _instance;
    private void Awake()
    {
        _instance = this;
    }
    [SerializeField] private StatUISetup _speedStat;
    [SerializeField] private StatUISetup _damageStat;

    [SerializeField] private float _arrowOffset;
    [SerializeField] private float _arrowAnimDuration;
    [SerializeField] private AnimationCurve _arrowEasing;
    [SerializeField] private Color _colourIncrease, _colourDecrease;

    [System.Serializable]
    public class StatUISetup
    {
        public GameObject _iconGo;
        [ShowOnly] public float _currentValue;
        public TMP_Text _textIncrease;
        public Transform _arrowsParent;
    }

    private void Start()
    {
        // Default the stats
        UpdateText(_speedStat._textIncrease, 0);
        UpdateText(_damageStat._textIncrease, 0);
    }

    public void IncreaseSpeedStat(float amount)
    {
        _speedStat._currentValue += amount;
        UpdateText(_speedStat._textIncrease, _speedStat._currentValue);
        ArrowAnimation(_speedStat, true);
    }

    public void IncreaseDamageStat(float amount)
    {
        _damageStat._currentValue += amount;
        UpdateText(_damageStat._textIncrease, _damageStat._currentValue);
        ArrowAnimation(_damageStat, true);
    }

    private void ArrowAnimation(StatUISetup stat, bool goingUp)
    {
        List<RectTransform> childArrows = stat._arrowsParent.GetChildrenOfType<RectTransform>();
         
        foreach (RectTransform arrow in childArrows)
        {
            StartCoroutine(PlayArrowAnimation(arrow, goingUp));
        }
         
    }

    private IEnumerator PlayArrowAnimation(RectTransform arrow, bool goingUp)
    {
        float elapsedTime = 0f;
        
        float targetAlpha = goingUp ? 1f : 0f;
        arrow.rotation = goingUp ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 0f, 180f);

        // If the goingUp bool is equal to true then use colourIncrease, otherwise use colourDecrease
        arrow.GetComponent<Image>().color = goingUp ? _colourIncrease : _colourDecrease;
        float trueOffset = goingUp ? _arrowOffset : -_arrowOffset;

        Vector2 startPosition = arrow.anchoredPosition;
        Vector2 targetPosition = new Vector2(startPosition.x, startPosition.y + trueOffset);

        Image arrowImage = arrow.GetComponent<Image>(); 

        while (elapsedTime < _arrowAnimDuration)
        {
            float time = Mathf.Clamp01(elapsedTime / _arrowAnimDuration);
            float curveValue = _arrowEasing.Evaluate(time);

            Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            
            arrow.anchoredPosition = currentPosition;

            float newAlpha = Mathf.Lerp(1f, 0f, curveValue);
            arrowImage.ChangeAlpha(newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        arrow.anchoredPosition = startPosition;
    }

    public void DecreaseSpeedStat(float amount)
    {
        _speedStat._currentValue -= amount;
        UpdateText(_speedStat._textIncrease, _speedStat._currentValue);
        ArrowAnimation(_speedStat, false);
    }

    public void DecreaseDamageStat(float amount)
    {
        _damageStat._currentValue -= amount;
        UpdateText(_damageStat._textIncrease, _damageStat._currentValue);
        ArrowAnimation(_damageStat, false);
    }

    private void UpdateText(TMP_Text text, float amount)
    {
        if (amount > 0)
        {
            text.text = ($"+{amount}%");
        }
        else
        {
            text.text = "";
        }
    }

}
