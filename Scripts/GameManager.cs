#pragma warning disable CS0618

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
	//VARIABLES
	const int TURN_TIME = 50;

	public static QuestionList QuestionList = new QuestionList();

	public int rndmQuestionNumber;
	public int rndmOptionNumber;

	Button NextQuestionButton;
	GameObject NextQuestionButtonGO;
	GameObject IpCanvas;
	GameObject Timer;
	GameObject QuestionText;
	GameObject Player;
	Button Option_1;
	Button Option_2;
	Button Option_3;
	Button Option_4;

	[NonSerialized]
	public bool DidIAnswer = false;

	[NonSerialized]
	public bool CanvasShow = false;
	[NonSerialized]
	public bool ButtonShow = false;
	[NonSerialized]
	public bool RunOnce = true;

	string checkText = "";
	public float TurnTimer;

	// FUNCTIONS
	private void Start()
	{		
		IpCanvas = GameObject.Find("IPCanvas");
		Timer = GameObject.Find("Timer");

		NextQuestionButton = IpCanvas.GetComponentInChildren<Button>();
		NextQuestionButton.onClick.AddListener(startNextTurn);
		NextQuestionButtonGO = GameObject.Find("NextButton");
		Player = GameObject.Find("Player");
		QuestionText = GameObject.Find("Question");
		Option_1 = GameObject.Find("Option_1").GetComponent<Button>();
		Option_2 = GameObject.Find("Option_2").GetComponent<Button>();
		Option_3 = GameObject.Find("Option_3").GetComponent<Button>();
		Option_4 = GameObject.Find("Option_4").GetComponent<Button>();

		Option_1.onClick.AddListener(option1Clicked);
		Option_2.onClick.AddListener(option2Clicked);
		Option_3.onClick.AddListener(option3Clicked);
		Option_4.onClick.AddListener(option4Clicked);

		CanvasEnabled(false);
		ButtonEnabled(false);
		getQuestionList();
	}
	private void CanvasEnabled(bool ISIT)
	{
		// Bu Arayüz Canvasının kontrol(show/hide) fonksiyonu!
		if (ISIT)
		{
			IpCanvas.GetComponent<Canvas>().enabled = ISIT;
		}
		else
		{
			IpCanvas.GetComponent<Canvas>().enabled = ISIT;
		}

		CanvasShow = ISIT;
	}
	private void ButtonEnabled(bool ISIT)
	{
		// Bu Arayüz Canvasındaki BUTTONun kontrol(show/hide) fonksiyonu!
		if (ISIT)
		{
			NextQuestionButtonGO.GetComponent<Canvas>().enabled = ISIT;
		}
		else
		{
			NextQuestionButtonGO.GetComponent<Canvas>().enabled = ISIT;
		}

		ButtonShow = ISIT;
	}
	private void getQuestionList()
	{
		//JSON Datayı ceken fonksiyon
		TextAsset asset = Resources.Load("history") as TextAsset;

		if (asset != null)
		{
			QuestionList = JsonUtility.FromJson<QuestionList>(asset.text);
		}
		else
		{
			Debug.Log("Assets are null!");
		}
	}


	private void Update()
	{
		// SERVERSA 5 ER 5 ER EKLİYOR VE SYNCLİYOR
		// CLİENTSA 400 EKLİYOR AMA SYNCLEMİYOR COMMANDDE CALISIYO NE ALAKA!!		

		ServerUpdate();
	}

	void ServerUpdate()
	{
		// Bu fonksiyon sadece Serverda calısır. Server aynı zamanda Clientsa'da calısır.
		if (isServer)
		{
			if (isLocalPlayer)
			{
				TurnTimer -= Time.deltaTime;
				RpcTurnTimerRemaining(TurnTimer);

				if (RunOnce)
				{
					startTheGame();
				}
				if (TurnTimer <= 0)
				{
					TurnTimer = 0;
					RpcCheckAnswer("");
					//Debug.Log("TUR BİTTİ");
				}
				if (CanvasShow)
				{
					//TODO: Diger oyuncuların cevaplarını kontrol et eger hepsi cevaplamıssa timer baslat
					if (DidIAnswer)
					{
						ButtonEnabled(true);
					}
				}
			}
		}
	}

	IEnumerator RpcSetQuestionIE()
	{
		yield return new WaitForSeconds(0.5f);

		RpcSetQuestion(rndmQuestionNumber, rndmOptionNumber);
	}
	[ClientRpc]
	void RpcSetQuestion(int question, int option)
	{
		Debug.Log("SERVER TARAFINDAN GÖNDERİLEN " + question + " " + option + " " + "İLE SORUMU OLUŞTURDUM");
		//Debug.Log("TURN TIMERDAN GELEN DEGER İLE SÜREYİ AYARLADIM: " + TurnTimer);
		DidIAnswer = false;
		CanvasEnabled(false);
		ButtonEnabled(false);

		QuestionText.GetComponentInChildren<Text>().text = QuestionList.Question[question].question;

		GameObject.Find("Option_" + option).GetComponentInChildren<Text>().text = QuestionList.Question[question].answer.ToString();

		generateOtherOptions(question, option);
	}
	[ClientRpc]
	void RpcTurnTimerRemaining(float turntimer)
	{
		Timer.GetComponent<Text>().text = turntimer.ToString("0");
	}

	[ClientRpc]
	void RpcCheckAnswer(string text)
	{
		DidIAnswer = true;
		string answer = text;

		CanvasEnabled(true);
		ButtonEnabled(true);

		if (QuestionList.Question[rndmQuestionNumber].answer.ToString() == answer)
		{
			IpCanvas.GetComponentInChildren<Text>().text = "Doğru Cevap";
			Debug.Log("Doğru Cevap");
		}
		else
		{
			IpCanvas.GetComponentInChildren<Text>().text = "Yanlış Cevap";
			Debug.Log("Yanlış Cevap: " + text + " , " + " Doğru Cevap: " + QuestionList.Question[rndmQuestionNumber].answer.ToString());
		}
	}


	void generateOtherOptions(int question, int option)
	{
		// Bu fonksiyon secilen sorunun cevabını alıp eger cevap belli aralıklardan büyükse 
		// ona göre MİN MAX olusturup generateOptionsTexts ine MİN MAX değerlerini gönderiyor
		int min = 0;
		int max = 0;
		if (QuestionList.Question[question].answer > 2000)
		{
			max = 400;
			generateOptionsTexts(min, max, question, option);
		}
		else if (QuestionList.Question[question].answer > 1000)
		{
			max = 100;
			generateOptionsTexts(min, max, question, option);
		}
		else if (QuestionList.Question[question].answer > 500)
		{
			max = 500;
			generateOptionsTexts(min, max, question, option);
		}
		else if (QuestionList.Question[question].answer > 100)
		{
			max = 100;
			generateOptionsTexts(min, max, question, option);
		}
		else if (QuestionList.Question[question].answer > 10)
		{
			max = 10;
			generateOptionsTexts(min, max, question, option);
		}
		else
		{
			max = 3;
			generateOptionsTexts(min, max, question, option);
		}
	}
	void generateOptionsTexts(int min, int max, int question, int option)
	{
		// Bu fonksiyon her bir şık için almış olduğu MİN MAX değerlerini 
		// o şıkkın Texti oluşturulması için generateText fonksiyonuna gönderiyor.
		for (int i = 1; i <= 4; i++)
		{
			if (option == i)
			{
				generateText(i, min, max, question);
			}
		}
	}
	void generateText(int index, int min, int max, int question)
	{
		// Bu fonksiyon dışarıdan gelen doğru cevap şıkkı MİN ve MAX değerini işleyip olusturduğu random sayının
		// 2ye bölümüne göre çıkartıyor, topluyor ve doğru cevap ise hic ellemiyor bu şekilde 4 şık olusturuyor.
		int rndmEven = Random.Range(1, 10);

		for (int i = 1; i <= 4; i++)
		{
			int rndm = Random.Range(min, max);
			if (rndmEven % 2 == 0)
			{
				if (i == index)
				{
					continue;
				}
				else
				{
					GameObject.Find("Option_" + i).GetComponentInChildren<Text>().text = (QuestionList.Question[question].answer + rndm).ToString();
				}
			}
			else
			{
				if (i == index)
				{
					continue;
				}
				else
				{
					GameObject.Find("Option_" + i).GetComponentInChildren<Text>().text = (QuestionList.Question[question].answer - rndm).ToString();
				}
			}
		}


	}

	void startTheGame()
	{
		TurnTimer = TURN_TIME;
		rndmQuestionNumber = Random.Range(0, QuestionList.Question.Count + 1);
		rndmOptionNumber = Random.Range(1, 5);
		

		StartCoroutine(RpcSetQuestionIE());
		RunOnce = false;
	}
	void startNextTurn()
	{
		// Only if Host clicks Button
		ButtonEnabled(false);
		RunOnce = true;
		TurnTimer = TURN_TIME;
	}

	public void option1Clicked()
	{
		if (isLocalPlayer)
		{
			
			this.checkText = Option_1.GetComponentInChildren<Text>().text;
			Debug.Log("CEVABIM: " + checkText);
			checkAnswer(this.checkText);
		}
	}
	public void option2Clicked()
	{
		if (isLocalPlayer)
		{
			checkText = Option_2.GetComponentInChildren<Text>().text;
			checkAnswer(checkText);
		}
	}
	public void option3Clicked()
	{
		if (isLocalPlayer)
		{
			checkText = Option_3.GetComponentInChildren<Text>().text;
			checkAnswer(checkText);
		}
	}
	public void option4Clicked()
	{
		if (isLocalPlayer)
		{
			checkText = Option_4.GetComponentInChildren<Text>().text;
			checkAnswer(checkText);
		}
	}


	void checkAnswer(string text)
	{

		DidIAnswer = true;
		
		CanvasEnabled(true);
		ButtonEnabled(true);
		Debug.Log(Player.GetComponent<GameManager>().rndmQuestionNumber);

		if (QuestionList.Question[Player.GetComponent<GameManager>().rndmQuestionNumber].answer.ToString() == text)
		{			
			IpCanvas.GetComponentInChildren<Text>().text = "Doğru Cevap";
			//Debug.Log("Doğru Cevap");
		}
		else
		{
			IpCanvas.GetComponentInChildren<Text>().text = "Yanlış Cevap";
			//Debug.Log("Yanlış Cevap: " + text + " , " + " Doğru Cevap: " + QuestionList.Question[rndmQuestionNumber].answer.ToString());
		}
	}
}