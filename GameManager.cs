#pragma warning disable CS0618

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
	//VARIABLES
	const int TURN_TIME = 30;

	public static QuestionList QuestionList = new QuestionList();

	[SyncVar] public int rndmQuestionNumber;
	[SyncVar] public int rndmOptionNumber;
	[SyncVar] public float TurnTimer;

	Button NextQuestionButton;
	GameObject NextQuestionButtonGO;
	GameObject IpCanvas;
	GameObject Timer;
	GameObject QuestionText;
	Button Option_1;
	Button Option_2;
	Button Option_3;
	Button Option_4;

	bool RunOnce = true;
	bool DidIAnswer = false;
	public bool CanvasShow = false;
	public bool ButtonShow = false;
	string checkText = "";

	// FUNCTIONS
	private void Start()
	{
		IpCanvas = GameObject.Find("IPCanvas");
		Timer = GameObject.Find("Timer");

		NextQuestionButton = IpCanvas.GetComponentInChildren<Button>();
		NextQuestionButton.onClick.AddListener(startNextTurn);
		NextQuestionButtonGO = GameObject.Find("NextButton");

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
	private void setQuestionAndOption()
	{
		// Bu fonksiyon rastgele 1 soru ve 1 secenek seciyor.
		rndmQuestionNumber = Random.Range(0, QuestionList.Question.Count + 1);
		rndmOptionNumber = Random.Range(1, 5);
	}

	private void Update()
	{
		ServerUpdate();
		ClientUpdate();
	}
	// TODO: MUHTEMELEN SERVERDA AYARLANAN SORULAR SYNC OLMUYOR
	// TODO: rndmQuestionNumber ve rndmOptionNumber sync olmuyor bir yolunu bul. !!!
	void ServerUpdate()
	{
		// Bu fonksiyon sadece Serverda calısır. Server aynı zamanda Clientsa'da calısır.
		if (isServer == false)
		{
			return;
		}	
		if (isServer)
		{			
			TurnTimer -= Time.deltaTime;
			Timer.GetComponent<Text>().text = TurnTimer.ToString("0");
			if (RunOnce)
			{
				TurnTimer = TURN_TIME;
				setQuestionAndOption();
				StartCoroutine(RpcSetQuestionIE(rndmQuestionNumber, rndmOptionNumber));
				RunOnce = false;
			}
			if (TurnTimer <= 0)
			{
				TurnTimer = 0;
				//Debug.Log("TUR BİTTİ");					
				checkAnswer("");

			}
			if (CanvasShow)
			{
				//TODO: Diger oyuncuların cevaplarını kontrol et eger hepsi cevaplamıssa timer baslat
				if (DidIAnswer)
				{
					ButtonEnabled(true);
				}
			}
			//Debug.Log("Ben Server ve Clientım");

		}
	}
	void ClientUpdate()
	{
		//Bu fonksiyon sadece ve sadece Clientlarda calısır, Eğer Client ve Server ise calısmaz..
		if (isServer == true)
		{
			return;
		}
		Timer.GetComponent<Text>().text = TurnTimer.ToString("0");

		if (TurnTimer <= 0)
		{
			TurnTimer = 0;
			checkAnswer("");
		}
		//Debug.Log("Ben Clientım");
	}

	IEnumerator RpcSetQuestionIE(int question, int option)
	{
		yield return new WaitForSeconds(0.0f);
		RpcSetQuestion(question, option);
	}

	[ClientRpc]
	private void RpcSetQuestion(int question, int option)
	{
		//TODO: Bir sekilde client ve server icin soru olusturulurken cevaplar eşleşmiyor. Eşleşiyo olabilir ama buttonların icindeki textleri almıyor!!
		DidIAnswer = false;
		CanvasEnabled(false);
		ButtonEnabled(false);

		QuestionText.GetComponentInChildren<Text>().text = QuestionList.Question[question].question;

		GameObject.Find("Option_" + option).GetComponentInChildren<Text>().text = QuestionList.Question[question].answer.ToString();
		generateOtherOptions();
	}

	void startNextTurn()
	{
		ButtonEnabled(false);
		RunOnce = true;
		TurnTimer = TURN_TIME;
	}
	void generateOtherOptions()
	{
		// Bu fonksiyon secilen sorunun cevabını alıp eger cevap belli aralıklardan büyükse 
		// ona göre MİN MAX olusturup generateOptionsTexts ine MİN MAX değerlerini gönderiyor
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
		// Bu fonksiyon her bir şık için almış olduğu MİN MAX değerlerini 
		// o şıkkın Texti oluşturulması için generateText fonksiyonuna gönderiyor.
		for (int i = 1; i <= 4; i++)
		{
			if (rndmOptionNumber == i)
			{
				generateText(i, min, max);
			}
		}
	}
	void generateText(int index, int min, int max)
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
		if (hasAuthority)
		{
			checkText = Option_1.GetComponentInChildren<Text>().text;
			checkAnswer(checkText);
		}
	}
	public void option2Clicked()
	{
		if (hasAuthority)
		{
			checkText = Option_2.GetComponentInChildren<Text>().text;
			checkAnswer(checkText);
		}
	}
	public void option3Clicked()
	{
		if (hasAuthority)
		{
			checkText = Option_3.GetComponentInChildren<Text>().text;
			checkAnswer(checkText);
		}
	}
	public void option4Clicked()
	{
		if (hasAuthority)
		{
			checkText = Option_4.GetComponentInChildren<Text>().text;
			checkAnswer(checkText);
		}
	}


	void checkAnswer(string text)
	{
		DidIAnswer = true;
		string answer = text;

		CanvasEnabled(true);
		ButtonEnabled(true);

		if (QuestionList.Question[rndmQuestionNumber].answer.ToString() == answer)
		{
			IpCanvas.GetComponentInChildren<Text>().text = "Doğru Cevap";
			//Debug.Log("Doğru Cevap");
		}
		else
		{
			IpCanvas.GetComponentInChildren<Text>().text = "Yanlış Cevap";
			//Debug.Log("Yanlış Cevap");
		}
	}
}
