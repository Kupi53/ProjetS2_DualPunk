using UnityEngine;
using UnityEngine.UI;

public class PointerArrow : MonoBehaviour 
{
    public GameObject Target;
    private RectTransform _rectTransform;
    private GameObject _camera;
    private Image _image;
    private float _bordersize = 200f;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _camera = GameObject.FindGameObjectWithTag("Camera");
        _image = GetComponent<Image>();
    }

    void Update()
    {
        Vector3 toPosition = Target.transform.position;
        Vector3 fromPosition = _camera.transform.position;
        fromPosition.z = 0;
        toPosition.z = 0;
        float angle =  Vector3.SignedAngle(fromPosition, toPosition, Vector3.forward);
        _rectTransform.rotation = Quaternion.AngleAxis(90, Vector3.forward) * Quaternion.LookRotation(Vector3.forward, toPosition - fromPosition);

        Vector3 targetPositionScreenPoint = _camera.GetComponentInChildren<Camera>().WorldToScreenPoint(toPosition);
        bool isOffScreen = targetPositionScreenPoint.x <= 0 || targetPositionScreenPoint.x >= Screen.width
                        || targetPositionScreenPoint.y <= 0 || targetPositionScreenPoint.y >= Screen.height;
        if (isOffScreen)
        {
            if (_image.enabled == false) _image.enabled = true;
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= _bordersize) cappedTargetScreenPosition.x = _bordersize;
            if (cappedTargetScreenPosition.x >= Screen.width-_bordersize) cappedTargetScreenPosition.x = Screen.width-_bordersize;
            if (cappedTargetScreenPosition.y <= -_bordersize) cappedTargetScreenPosition.y = _bordersize;
            if (cappedTargetScreenPosition.y >= Screen.height-_bordersize) cappedTargetScreenPosition.y = Screen.height-_bordersize;
            //Vector3 pointerWorldPosition = _camera.GetComponentInChildren<Camera>().ScreenToWorldPoint(cappedTargetScreenPosition);
            //pointerWorldPosition.z = 0;
            _rectTransform.position = cappedTargetScreenPosition;
        }
        else
        {
            if (_image.enabled == true) _image.enabled = false;
        }
    }
}