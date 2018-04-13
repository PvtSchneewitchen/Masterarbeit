using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRTK;

class ReferenceHandler : MonoBehaviour
{
    public static ReferenceHandler Instance { get; private set; }

    public VRTK_Pointer LeftPointer;
    public VRTK_Pointer RightPointer;
    public VRTK_StraightPointerRenderer LeftPointerRenderer;
    public VRTK_StraightPointerRenderer RightPointerRenderer;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public VRTK_Pointer GetLeftPointer()
    {
        return LeftPointer;
    }

    public VRTK_Pointer GetRightPointer()
    {
        return RightPointer;
    }

    public VRTK_StraightPointerRenderer GetLeftPointerRenderer()
    {
        return LeftPointerRenderer;
    }

    public VRTK_StraightPointerRenderer GetRightPointerRenderer()
    {
        return RightPointerRenderer;
    }
}

