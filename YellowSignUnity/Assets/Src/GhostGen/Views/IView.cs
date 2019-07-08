using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IView
{ 
    void OnViewDispose();
    
    GameObject gameObject { get; }
}
