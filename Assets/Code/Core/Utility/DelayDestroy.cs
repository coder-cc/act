using UnityEngine;
using System.Collections;

public class DelayDestroy : MonoBehaviour
{


    public string ResourceName { get; set; }

    public int TimeMs { get; set; }


    public System.Action<string, GameObject> OnCompleta;


    //void OnEnable()
    //{
    //    if (TimeMs.Equals(0))
    //        return;
    //    StartCoroutine(StartTime());
    //}


    public void Start()
    {
        StartCoroutine(StartTime());
    }
	

	IEnumerator StartTime ()
    {
        yield return new WaitForSeconds(TimeMs / 1000f);
        if (OnCompleta == null)
            Object.Destroy(gameObject);
        else
            OnCompleta(ResourceName, gameObject);
	}
}
