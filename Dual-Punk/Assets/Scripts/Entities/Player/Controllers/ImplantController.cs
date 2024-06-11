using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImplantController : MonoBehaviour
{
    #nullable enable
    public ImplantScript? NeuralinkImplant {get;set;}
    public ImplantScript? ExoSqueletonImplant {get;set;}
    public ImplantScript? ArmImplant {get;set;}
    public ImplantScript? BootsImplant {get;set;}
    private ImplantScript?[] _implants 
    {
        get
        {
            ImplantScript?[] implants = new ImplantScript?[4];
            implants[0] = NeuralinkImplant;
            implants[1] = ExoSqueletonImplant;
            implants[2] = ArmImplant;
            implants[3] = BootsImplant;
            return implants;
        }
    }
#nullable disable


    void Update()
    {
        foreach (var implant in _implants)
        {
            if (implant != null)
            {
                implant.Run();
            }
        }

        (bool, string) setIsEquipped = SetIsEquipped();

        if (setIsEquipped.Item1)
        {
            switch (setIsEquipped.Item2)
            {
                case "Organic":
                    SetOrganic();
                    break;
                case "Heavy":
                    SetHeavy();
                    break;
                case "3":
                    Set3();
                    break;
                case "4":
                    Set4();
                    break;
            }
        }
    }

    private (bool, string) SetIsEquipped()
    {
        (bool, string) result = (false, "");

        int implantNumber = 0;

        foreach (var implant in _implants)
        {
            if (implant != null)
            {
                if (result.Item2 == "")
                {
                    result.Item1 = true;
                    result.Item2 = implant.SetName;
                }
                else if (result.Item2 != implant.SetName)
                {
                    return (false, "");
                }

                implantNumber++;
            }
        }

        if (implantNumber != 4)
        {
            return (false, "");
        }

        return result;
    }

    private void SetOrganic()
    {
        //Debug.Log("SetOrganic");
    }

    private void SetHeavy()
    {
        //Debug.Log("SetHeavy");
    }

    private void Set3()
    {
        //Debug.Log("Set3");
    }

    private void Set4()
    {
        //Debug.Log("Set4");
    }
}
