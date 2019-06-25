using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHighlighter : MonoBehaviour
{
    private ITowerView _view;
    [SerializeField]
    private MaterialPropertyBlock block;

    [SerializeField]
    private Transform _root;
    //private Material _


    public Transform root
    {
        get { return _root; }
    }
	
    public void SetTower(ITowerView view)
    {
        _view = view;

        _view.gameObject.transform.localPosition = Vector3.zero;
        _view.gameObject.transform.rotation = Quaternion.identity;
        _view.gameObject.layer = 0;
        _view.transformTS.enableTransform = false;
        Renderer[] rendererList = _view.gameObject.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < rendererList.Length; ++i)
        {
            rendererList[i].material = _view.highlighterMaterial;
        }
    }
}
