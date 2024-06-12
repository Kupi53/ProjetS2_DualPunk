using UnityEngine;

public class PointerArrow : MonoBehaviour 
{
    public GameObject Target;
    private RectTransform _rectTransform;
    private GameObject _camera;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _camera = GameObject.FindGameObjectWithTag("Camera");
    }

    void Update()
    {
        Vector3 toPosition = Target.transform.position;
        Vector3 fromPosition = _camera.transform.position;
        fromPosition.z = 0;
        toPosition.z = 0;
        float angle =  Vector3.SignedAngle(fromPosition, toPosition, Vector3.forward);
        _rectTransform.rotation = Quaternion.AngleAxis(90, Vector3.forward) * Quaternion.LookRotation(Vector3.forward, toPosition - fromPosition);
    }
}