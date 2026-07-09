using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 

public class Tutorial : MonoBehaviour {
	public const string FirstLevelTutorialDoneKey = "first_level_tutorial_done";

	public static bool bTutorial = false;
	public static bool bPause = false;
	 
 
	Animator animTutorial;
	 

	public static float timeLeftToShowHelp = 3;
	public static float ShowHelpPeriod = 3;
	bool bHidden = true;
 
	public static Tutorial Instance;
	public static Transform copyPositionTransform;

	Color visibleCol = new Color(1,1,1,.5f);
	Color hiddenCol = new Color(1,1,1,0);
	Coroutine dragGestureCoroutine;
	Transform dragTarget;
	Vector3 dragWorldOffset;
	bool bDragGesture;

	void Awake()
	{
		animTutorial = transform.GetComponent<Animator>();
	}

	void Start () {
		
		Instance = this;
		bPause = false;
		animTutorial = transform.GetComponent<Animator>();
		ShowHelpPeriod = 3;
		timeLeftToShowHelp = 3;
  
 
	}

	void Update()
	{
		if(bTutorial && !bDragGesture)
		{
			 if(copyPositionTransform!= null)	transform.position = copyPositionTransform.position;
			else if(copyPositionTransform == null)	StopTutorial(); 
		}
	}

	public void StartTutorial()
	{
		bDragGesture = false;
		bTutorial = true;
		InvokeRepeating("TestTutorial",1,9);
	}

	public void StartDragTutorial(Transform target, Vector3 worldOffset)
	{
		if (target == null)
		{
			StopTutorial();
			return;
		}

		bTutorial = true;
		bDragGesture = true;
		dragTarget = target;
		dragWorldOffset = worldOffset;
		copyPositionTransform = target;

		if (dragGestureCoroutine != null)
			StopCoroutine(dragGestureCoroutine);

		dragGestureCoroutine = StartCoroutine(IEPlayDragGesture());
	}

	public void StopTutorial()
	{
		bTutorial = false;
		bDragGesture = false;
		copyPositionTransform = null;
		dragTarget = null;
		CancelInvoke();
		if (dragGestureCoroutine != null)
		{
			StopCoroutine(dragGestureCoroutine);
			dragGestureCoroutine = null;
		}
		GameObject.Destroy(this.gameObject);
	}

	public static bool IsFirstLevelTutorialDone()
	{
		return CPlayerPrefs.GetBool(FirstLevelTutorialDoneKey);
	}

	public static void SetFirstLevelTutorialDone(bool done)
	{
		CPlayerPrefs.SetBool(FirstLevelTutorialDoneKey, done);
		CPlayerPrefs.Save();
	}

	public static void ResetFirstLevelTutorialDone()
	{
		CPlayerPrefs.DeleteKey(FirstLevelTutorialDoneKey);
		PlayerPrefs.DeleteKey(FirstLevelTutorialDoneKey);
	}
	 
	void TestTutorial()
	{
		if(bTutorial) StartCoroutine("StartPointingAndHide");
	}

	IEnumerator  StartPointingAndHide(  )
	{
		bHidden = false;
		animTutorial.ResetTrigger("Hide");
		animTutorial.SetTrigger("moveStart");
		yield return new WaitForSeconds(5);
		animTutorial.ResetTrigger("moveStart");
		animTutorial.SetTrigger("Hide");
		timeLeftToShowHelp = ShowHelpPeriod;
		bHidden = true;
	}

	IEnumerator IEPlayDragGesture()
	{
		while (bTutorial && dragTarget != null)
		{
			Vector3 begin = dragTarget.position;
			Vector3 end = begin + dragWorldOffset;

			transform.position = begin;
			SetAnimTrigger("moveStart");
			yield return new WaitForSeconds(0.25f);

			SetAnimTrigger("move");

			float time = 0;
			float duration = 0.8f;
			while (time < duration && dragTarget != null)
			{
				time += Time.deltaTime;
				transform.position = Vector3.Lerp(begin, end, time / duration);
				yield return null;
			}

			SetAnimTrigger("moveEnd");
			yield return new WaitForSeconds(0.35f);
			SetAnimTrigger("Hide");
			yield return new WaitForSeconds(0.6f);
		}

		if (bTutorial)
			StopTutorial();
	}

	void SetAnimTrigger(string triggerName)
	{
		if (animTutorial == null)
			return;

		animTutorial.ResetTrigger("moveStart");
		animTutorial.ResetTrigger("move");
		animTutorial.ResetTrigger("moveEnd");
		animTutorial.ResetTrigger("Hide");
		animTutorial.SetTrigger(triggerName);
	}

	public void AnimEventMoveEnd()
	{
	}
 
	 
}
