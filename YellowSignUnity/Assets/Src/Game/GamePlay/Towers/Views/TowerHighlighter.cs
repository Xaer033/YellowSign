using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHighlighter : MonoBehaviour
{
    [SerializeField]
    private Transform _root;

	// Use this for initialization
	void Awake ()
    {
		
	}

    public Transform root
    {
        get { return _root; }
    }
	
}
