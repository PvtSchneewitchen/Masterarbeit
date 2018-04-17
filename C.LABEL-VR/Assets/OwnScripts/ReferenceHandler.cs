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

    [SerializeField]
    private SessionHandler SessionHandler;

    [SerializeField]
    private VRTK_Pointer LeftPointer;

    [SerializeField]
    private VRTK_Pointer RightPointer;

    [SerializeField]
    private VRTK_StraightPointerRenderer LeftPointerRenderer;

    [SerializeField]
    private VRTK_StraightPointerRenderer RightPointerRenderer;

    [SerializeField]
    private VRTK_UIPointer RightUiPointer;

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

    public VRTK_UIPointer GetRightUiPointer()
    {
        return RightUiPointer;
    }

    public SessionHandler GetSessionHandler()
    {
        return SessionHandler;
    }
}

