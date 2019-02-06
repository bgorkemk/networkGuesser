#pragma warning disable CS0618

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
	const int TURN_TIME = 15;
	private void Start()
	{
		IpCanvas = GameObject.Find("IPCanvas");
		Timer = GameObject.Find("Timer");
		NextQuestionButton = IpCanvas.GetComponentInChildren<Button>();
		NextQuestionButtonGO = GameObject.Find("NextButton");
		NextQuestionButtonGO.GetComponent<Canvas>().enabled = false;

		NextQuestionButton.onClick.AddListener(setFlag);

		QuestionText = GameObject.Find("Question");
		Option_1 = GameObject.Find("Option_1").GetComponent<Button>();
		Option_2 = GameObject.Find("Option_2").GetComponent<Button>();
		Option_3 = GameObject.Find("Option_3").GetComponent<Button>();
		Option_4 = GameObject.Find("Option_4").GetComponent<Button>();

		Option_1.onClick.AddListener(option1Clicked);
		Option_2.onClick.AddListener(option2Clicked);
		Option_3.onClick.AddListener(option3Clicked);
		Option_4.onClick.AddListener(option4Clicked);
		if (isServer)
		{
			getQuestionList();
		}
	}
	
	public static QuestionList QuestionList = new QuestionList();

	[SyncVar]
	int rndmQuestionNumber;

	[SyncVar]
	int rndmOptionNumber;

	[SyncVar]
	float TurnTimer = TURN_TIME;

	Button NextQuestionButton;
	GameObject NextQuestionButtonGO;
	GameObject IpCanvas;
	GameObject Timer;

	bool RunOnce = true;
	bool DidIAnswer = false;

	string checkText = "";
	
	GameObject QuestionText;

	private Button Option_1;
	private Button Option_2;
	private Button Option_3;
	private Button Option_4;

	private void getQuestionList()
	{
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
		if (RunOnce)
		{
			if (isLocalPlayer)
			{
				if (isServer)
				{
					//GameObject[] GOS = GameObject.FindGameObjectsWithTag("Player");
					//foreach (GameObject Player in GOS)
					//{
					//	Debug.Log("Buldum..");
					//}
					TurnTimer = TURN_TIME;
					rndmQuestionNumber = Random.Range(0, QuestionList.Question.Count + 1);
					rndmOptionNumber = Random.Range(1, 5);

					StartCoroutine(setQuestionCT(rndmQuestionNumber));
				}
			}

			RunOnce = false;

		}

		if (isServer)
		{
			TurnTimer -= Time.deltaTime;
			RpcSetTurnTimer(TurnTimer);

			if (TurnTimer <= 0)
			{
				//Debug.Log("TUR BİTTİ");	
				TurnTimer = 0;
				checkAnswer("");
				NextQuestionButtonGO.GetComponent<Canvas>().enabled = true;
			}

			if (IpCanvas.GetComponent<Canvas>().enabled == true)
			{
				//TODO: Diger oyuncuların cevaplarını kontrol et eger hepsi cevaplamıssa timer baslat
				if (DidIAnswer)
				{
					NextQuestionButtonGO.GetComponent<Canvas>().enabled = true;
				}
			}
		}
		else
		{
			if (TurnTimer <= 0)
			{
				//Debug.Log("TUR BİTTİ");	
				TurnTimer = 0;
				checkAnswer("");		
			}

			if (IpCanvas.GetComponent<Canvas>().enabled == true)
			{
				//TODO: Diger oyuncuların cevaplarını kontrol et eger hepsi cevaplamıssa timer baslat
				if (DidIAnswer)
				{

				}
			}
		}

	}

	IEnumerator setQuestionCT(int rndm)
	{
		yield return new WaitForSeconds(1);

		RpcsetQuestion(rndm);

	}

	IEnumerator setUpNextQuestionCT()
	{
		yield return new WaitForSeconds(5);

		if (isServer)
		{
			RunOnce = true;
		}
	}

	[ClientRpc]
	private void RpcsetQuestion(int number)
	{
		//TODO: Bir sekilde client ve server icin soru olusturulurken cevaplar eşleşmiyor. Düzelt
		DidIAnswer = false;
		IpCanvas.GetComponent<Canvas>().enabled = false;
		NextQuestionButtonGO.GetComponent<Canvas>().enabled = false;

		QuestionText.GetComponentInChildren<Text>().text = QuestionList.Question[number].question;
		Debug.Log("Sorunun Cevabı: " + QuestionList.Question[rndmQuestionNumber].answer.ToString());
		GameObject.Find("Option_" + rndmOptionNumber).GetComponentInChildren<Text>().text = QuestionList.Question[rndmQuestionNumber].answer.ToString();
		generateOtherOptions();
	}

	[ClientRpc]
	private void RpcSetTurnTimer(float x)
	{
		Timer.GetComponent<Text>().text = x.ToString("0");
	}

	void setFlag()
	{
		RunOnce = true;
		TurnTimer = 15;
	}

	void generateOtherOptions()
	{
		int min = 0;
		int max = 0;
		if (QuestionList.Question[rndmQuestionNumber].answer > 2000)
		{
			max = 400;
			generateOptionsTexts(min, max);
		}
		else if (QuestionList.Question[rndmQuestionNumber].answer > 1000)
		{
			max = 100;
			generateOptionsTexts(min, max);
		}
		else if (QuestionList.Question[rndmQuestionNumber].answer > 500)
		{
			max = 500;
			generateOptionsTexts(min, max);
		}
		else if (QuestionList.Question[rndmQuestionNumber].answer > 100)
		{
			max = 100;
			generateOptionsTexts(min, max);
		}
		else if (QuestionList.Question[rndmQuestionNumber].answer > 10)
		{
			max = 10;
			generateOptionsTexts(min, max);
		}
		else
		{
			max = 3;
			generateOptionsTexts(min, max);
		}
	}

	void generateOptionsTexts(int min, int max)
	{
		if (rndmOptionNumber == 1)
		{
			generateText(1, min, max);
		}
		else if (rndmOptionNumber == 2)
		{
			generateText(2, min, max);
		}
		else if (rndmOptionNumber == 3)
		{
			generateText(3, min, max);
		}
		else if (rndmOptionNumber == 4)
		{
			generateText(4, min, max);
		}
	}

	void generateText(int index, int min, int max)
	{
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
					GameObject.Find("Option_" + i).GetComponentInChildren<Text>().text = (QuestionList.Question[rndmQuestionNumber].answer + rndm).ToString();
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
					GameObject.Find("Option_" + i).GetComponentInChildren<Text>().text = (QuestionList.Question[rndmQuestionNumber].answer - rndm).ToString();
				}
			}
		}


	}

	public void option1Clicked()
	{
		checkText = Option_1.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}
	public void option2Clicked()
	{
		checkText = Option_2.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}
	public void option3Clicked()
	{
		checkText = Option_3.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}
	public void option4Clicked()
	{
		checkText = Option_4.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}

	void checkAnswer(string text)
	{
		DidIAnswer = true;
		string answer = text;

		if (isLocalPlayer)
		{
			IpCanvas.GetComponent<Canvas>().enabled = true;

			if (QuestionList.Question[rndmQuestionNumber].answer.ToString() == text)
			{
				IpCanvas.GetComponentInChildren<Text>().text = "Doğru Cevap";
				Debug.Log("Doğru Cevap");
			}
			else
			{
				IpCanvas.GetComponentInChildren<Text>().text = "Yanlış Cevap";
				Debug.Log("Yanlış Cevap");
			}

		}

	}
}
