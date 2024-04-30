using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImplantController : MonoBehaviour
{
    #nullable enable
    public ImplantScript? NeuralinkImplant {private get;set;}
    public ImplantScript? ExoSqueletonImplant {private get;set;}
    public ImplantScript? ArmImplant {private get;set;}
    public ImplantScript? BootsImplant {private get;set;}
    private ImplantScript?[] _implants {
        get{
            ImplantScript?[] implants = new ImplantScript?[4];
            implants[0] = NeuralinkImplant;
            implants[1] = ExoSqueletonImplant;
            implants[2] = ArmImplant;
            implants[3] = BootsImplant;
            return implants;
        }
    }

    void Update(){
        foreach (var implant in _implants){
            if (implant != null){
                Debug.Log(implant.Type.ToString());
                implant.Run();
            }
        }
    }

}
