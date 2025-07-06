using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class WFX_Demo_DeleteAfterDelay : MonoBehaviour
{
	public float time = 1.0f;
	public float timer;
	
	private Coroutine coroutine;
	
	private void OnEnable()
	{
		coroutine = StartCoroutine(Shoot());
	}
	
	private void OnDisable()
	{
		if  (coroutine != null)
			StopCoroutine(coroutine);
	}

	private IEnumerator Shoot()
	{
		do
		{
			timer -= Time.deltaTime;
			yield return null;
		}
		while(timer > 0);
		timer = time;
		gameObject.SetActive(false);
	}
}
