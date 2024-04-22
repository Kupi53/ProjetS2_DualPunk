using UnityEngine;

public class EnbleDisableButton : MonoBehaviour
{
    [SerializeField] private DescriptionManager descriptions;

    void Start(){
        descriptions.enabled = false;
    }

    public void ChangeActiveness(){
        if(descriptions.enabled == true){
            descriptions.enabled = false;
        }
        else{
            descriptions.enabled = true;
        }
    }


}
