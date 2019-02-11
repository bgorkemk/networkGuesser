//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class PlayerManager : NetworkBehaviour
//{

//	public QuestionList QuestionList = new QuestionList();

//	Button Option_1;
//	Button Option_2;
//	Button Option_3;
//	Button Option_4;
//	GameObject NextQuestionButtonGO;
//	GameObject IpCanvas;
//	GameObject Timer;


//	bool RunOnce = true;
//	bool DidIAnswer = false;
//	public bool CanvasShow = false;
//	public bool ButtonShow = false;
//	string checkText = "";


//	void FindButtons()
//	{
//		Option_1 = GameObject.Find("Option_1").GetComponent<Button>();
//		Option_2 = GameObject.Find("Option_2").GetComponent<Button>();
//		Option_3 = GameObject.Find("Option_3").GetComponent<Button>();
//		Option_4 = GameObject.Find("Option_4").GetComponent<Button>();
//	}
//	void SetClickerForButtons()
//	{
//		Option_1.onClick.AddListener(option1Clicked);
//		Option_2.onClick.AddListener(option2Clicked);
//		Option_3.onClick.AddListener(option3Clicked);
//		Option_4.onClick.AddListener(option4Clicked);
//	}

//	void Start()
//	{

//		IpCanvas = GameObject.Find("IPCanvas");
//		Timer = GameObject.Find("Timer");

//		NextQuestionButtonGO = GameObject.Find("NextButton");

//		getQuestionList();

//		FindButtons();
//		SetClickerForButtons();

//	}

//	private void getQuestionList()
//	{
//		//JSON Datayı ceken fonksiyon
//		TextAsset asset = Resources.Load("history") as TextAsset;

//		if (asset != null)
//		{
//			QuestionList = JsonUtility.FromJson<QuestionList>(asset.text);
//		}
//		else
//		{
//			Debug.Log("Assets are null!");
//		}
//	}

//	void Update()
//	{
		
//	}
	

//	private void CanvasEnabled(bool ISIT)
//	{
//		// Bu Arayüz Canvasının kontrol(show/hide) fonksiyonu!
//		if (ISIT)
//		{
//			IpCanvas.GetComponent<Canvas>().enabled = ISIT;
//		}
//		else
//		{
//			IpCanvas.GetComponent<Canvas>().enabled = ISIT;
//		}

//		CanvasShow = ISIT;
//	}
//	private void ButtonEnabled(bool ISIT)
//	{
//		// Bu Arayüz Canvasındaki BUTTONun kontrol(show/hide) fonksiyonu!
//		if (ISIT)
//		{
//			NextQuestionButtonGO.GetComponent<Canvas>().enabled = ISIT;
//		}
//		else
//		{
//			NextQuestionButtonGO.GetComponent<Canvas>().enabled = ISIT;
//		}

//		ButtonShow = ISIT;
//	}

//	public void option1Clicked()
//	{
//		if (hasAuthority)
//		{
//			checkText = Option_1.GetComponentInChildren<Text>().text;
//			Debug.Log("Sorunun Cevabı: " + QuestionList.Question[GameManager.SelectedQuestion].answer.ToString() + " . Senin sectigin cevap: " + checkText);
//			CheckMyAnswer(checkText);

//			CanvasEnabled(true);
//			ButtonEnabled(true);
//		}
//	}
//	public void option2Clicked()
//	{
//		if (hasAuthority)
//		{
//			checkText = Option_2.GetComponentInChildren<Text>().text;
//			Debug.Log("Sorunun Cevabı: " + QuestionList.Question[GameManager.SelectedQuestion].answer.ToString() + " . Senin sectigin cevap: " + checkText);
//			CheckMyAnswer(checkText);

//			CanvasEnabled(true);
//			ButtonEnabled(true);
//		}
//	}
//	public void option3Clicked()
//	{
//		if (hasAuthority)
//		{
//			checkText = Option_3.GetComponentInChildren<Text>().text;
//			Debug.Log("Sorunun Cevabı: " + QuestionList.Question[GameManager.SelectedQuestion].answer.ToString() + " . Senin sectigin cevap: " + checkText);
//			CheckMyAnswer(checkText);


//			CanvasEnabled(true);
//			ButtonEnabled(true);
//		}
//	}
//	public void option4Clicked()
//	{
//		if (hasAuthority)
//		{
//			checkText = Option_4.GetComponentInChildren<Text>().text;
//			Debug.Log("Sorunun Cevabı: " + QuestionList.Question[GameManager.SelectedQuestion].answer.ToString() + " . Senin sectigin cevap: " + checkText);
//			CheckMyAnswer(checkText);

//			CanvasEnabled(true);
//			ButtonEnabled(true);
//		}
//	}

	
//	void CheckMyAnswer(string text)
//	{
//		string answer = text;

//		if (QuestionList.Question[GameManager.SelectedQuestion].answer.ToString() == answer)
//		{
//			IpCanvas.GetComponentInChildren<Text>().text = "Doğru Cevap";
//			//Debug.Log("Doğru Cevap");
//		}
//		else
//		{
//			IpCanvas.GetComponentInChildren<Text>().text = "Yanlış Cevap";
//			//Debug.Log("Yanlış Cevap");
//		}
//	}
//}
