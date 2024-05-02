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

    void Update()
    {
        foreach (var implant in _implants)
        {
            if (implant != null)
            {
                implant.Run();
            }
        }

        (bool, int?) setIsEquipped = SetIsEquipped();

        if (setIsEquipped.Item1)
        {
            switch (setIsEquipped.Item2)
            {
                case 1:
                    Set1();
                    break;
                case 2:
                    Set2();
                    break;
                case 3:
                    Set3();
                    break;
                case 4:
                    Set4();
                    break;
            }
        }
    }

    private (bool, int?) SetIsEquipped()
    {
        (bool, int?) result = (false, null);

        int implantNumber = 0;

        foreach (var implant in _implants)
        {
            if (implant != null)
            {
                if (result.Item2 == null)
                {
                    result.Item1 = true;
                    result.Item2 = implant.SetNumber;
                }
                else if (result.Item2 != implant.SetNumber)
                {
                    return (false, null);
                }

                implantNumber++;
            }
        }

        if (implantNumber != 4)
        {
            return (false, null);
        }

        return result;
    }

    private void Set1()
    {
        Debug.Log("Set1");
    }

    private void Set2()
    {
        Debug.Log("Set2");
    }

    private void Set3()
    {
        Debug.Log("Set3");
    }

    private void Set4()
    {
        Debug.Log("Set4");
    }
}
