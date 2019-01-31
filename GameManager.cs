using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameManager : NetworkBehaviour
{
	public static QuestionList QuestionList = new QuestionList();
	[SyncVar]
	int rndmQuestionNumber;
	[SyncVar]
	int rndmOptionNumber;
	bool flag = true;
	string checkText = "";

	[SyncVar]
	float TimeLeft = 10;

	private float nextActionTime = 0.0f;
	public float period = 1f;


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

	private void Start()
	{
		QuestionText = GameObject.Find("Question");
		Option_1 = GameObject.Find("Option_1").GetComponent<Button>();
		Option_2 = GameObject.Find("Option_2").GetComponent<Button>();
		Option_3 = GameObject.Find("Option_3").GetComponent<Button>();
		Option_4 = GameObject.Find("Option_4").GetComponent<Button>();
		Option_1.onClick.AddListener(option1Clicked);
		Option_2.onClick.AddListener(option2Clicked);
		Option_3.onClick.AddListener(option3Clicked);
		Option_4.onClick.AddListener(option4Clicked);
		getQuestionList();
	}
	private void Update()
	{
		if (flag)
		{
			if (isLocalPlayer)
			{
				if (isServer)
				{
					rndmQuestionNumber = Random.Range(0, QuestionList.Question.Count + 1);
					rndmOptionNumber = Random.Range(1, 5);

					StartCoroutine(setQuestionTimer(rndmQuestionNumber));
				}

			}

			flag = false;
		}
		if (isServer)
		{
			if (Time.time > nextActionTime)
			{
				nextActionTime += period;
				TimeLeft--;
				Debug.Log(TimeLeft);
				if (TimeLeft == 0)
				{
					TimeLeft = 10;
				}
			}
		}
	}

	void option1Clicked()
	{
		checkText = Option_1.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}
	void option2Clicked()
	{
		checkText = Option_2.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}
	void option3Clicked()
	{
		checkText = Option_3.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}
	void option4Clicked()
	{
		checkText = Option_4.GetComponentInChildren<Text>().text;
		checkAnswer(checkText);
	}
	void checkAnswer(string text)
	{
		if (isLocalPlayer)
		{

		}		
		string answer = text;
		//Debug.Log(text);
		//Debug.Log(QuestionList.Question[rndmQuestionNumber].answer.ToString());

		if (QuestionList.Question[rndmQuestionNumber].answer.ToString() == text)
		{
			Debug.Log("Doğru Cevap");
			StartCoroutine(setUpNextQuestion());			
		}
		else
		{
			Debug.Log("Yanlış Cevap");
		}
	}

	IEnumerator setUpNextQuestion()
	{
		yield return new WaitForSeconds(3);
		if (isServer)
		{
			flag = true;
		}
	}

	IEnumerator setQuestionTimer(int rndm)
	{
		yield return new WaitForSeconds(2);
		RpcsetQuestion(rndm);
	}

	[ClientRpc]
	private void RpcsetQuestion(int number)
	{
		QuestionText.GetComponentInChildren<Text>().text = QuestionList.Question[number].question;
		GameObject.Find("Option_" + rndmOptionNumber).GetComponentInChildren<Text>().text = QuestionList.Question[rndmQuestionNumber].answer.ToString();
		generateOtherOptions();
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
}
