using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 

public class Tutorial : MonoBehaviour {
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

	void Start () {
		
		Instance = this;
		bPause = false;
		if(Application.loadedLevelName.StartsWith("Game") )
		{
			animTutorial = transform.GetComponent<Animator>();
			 

// 			if(  GameData.TutorialShown == 0)  
// 			{
				ShowHelpPeriod = 3;
				timeLeftToShowHelp = 3;
// 			}
//			else 
//			{
//				PeriodToShowHelp = 8;
//				timeWaitToShowHelp = 8;
//			}
		 
		 
		}
  
 
	}

	void Update()
	{
		if(bTutorial)
		{
			 if(copyPositionTransform!= null)	transform.position = copyPositionTransform.position;
			else if(copyPositionTransform == null)	StopTutorial(); 
		}
	}

	public void StartTutorial()
	{
		bTutorial = true;
		InvokeRepeating("TestTutorial",1,9);
	}

	public void StopTutorial()
	{
		bTutorial = false;
		CancelInvoke();
		GameObject.Destroy(this.gameObject);
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
 
	 
}
