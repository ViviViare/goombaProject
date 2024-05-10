using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDescReader : MonoBehaviour
{
    [Header("Canvas Values")]
    [SerializeField] private Canvas _itemCanvas;
    [SerializeField] private Vector3 _showOffset;
    [SerializeField] private TMP_Text _crosshairText;
    
    [Header("Raycast Values")]
    [SerializeField] private LayerMask _itemMask;
    [SerializeField] private float _raycastLength;
    [SerializeField] private Transform _raycastOrigin;
    
    [Header("Cached Values")]
    private bool _lookingAtItem;
    private bool _itemLock;
    private ItemDescData _currentItemCheck;

    private void Start()
    {
        NotLookingAtItem();
    }

    private void Update()
    {        
        RaycastHit hit;
        if (Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward, out hit, _raycastLength, _itemMask))
        {  
            if (!_lookingAtItem) LookingAtItem(hit);
            _lookingAtItem = true;
            _itemLock = false;
            
        }
        else _lookingAtItem = false;
        
        if (!_lookingAtItem && !_itemLock) NotLookingAtItem();
    }

    private void LookingAtItem(RaycastHit hit)
    {
        _currentItemCheck = hit.transform.GetComponent<ItemDescData>();
        MoveCanvas();
    }

    private void NotLookingAtItem()
    {
        _itemLock = true;
        ToggleCanvas(false);
    }

    private void MoveCanvas()
    {
        Vector3 canvasPosition = _currentItemCheck.transform.position + _showOffset;
        _itemCanvas.transform.position = canvasPosition;
        UpdateCanvasDetails();
    }

    private void UpdateCanvasDetails()
    {
        _crosshairText.text = ($"{_currentItemCheck._name}");
        ToggleCanvas(true);
    }

    private void ToggleCanvas(bool value)
    {
        if (_currentItemCheck != null) _itemCanvas.enabled = _currentItemCheck._isActive ? value : false;
        
        _crosshairText.enabled = value;
    }


    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_raycastOrigin.position, _raycastOrigin.position + (_raycastOrigin.forward * _raycastLength)) ;
    }
    #endif


}
