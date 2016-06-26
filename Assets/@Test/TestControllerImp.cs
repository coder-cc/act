using Core.Controller;
using UnityEngine;
using System.Collections;

public class TestControllerImp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
        TestController.Instance.Init();
	}
	
	// Update is called once per frame
	void Update () {
        TestController.Instance.Update();
	}


    void OnGUI()
    {
        TestController.Instance.OnGUI();
    }
}
