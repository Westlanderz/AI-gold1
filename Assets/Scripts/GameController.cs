using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Does general operations that are global, such as calling minimax algo, converting the board from int to string, restarting the game, etc...
/// </summary>
public class GameController : MonoBehaviour {
	MiniMax minimaxAlgo = new MiniMax();
	public GameObject[] positionArr;
	//opponent to ai
	public static string playersChoice = "";
	private Dictionary<string, bool> gameBoard; //TODO: fill this with the gameboard postions and if they are clicked - send for minimax
	//player
	public Dictionary<BoardPossibleWinScenarios, int> playerBoard = new Dictionary<BoardPossibleWinScenarios, int>();
	//ai
	public Dictionary<BoardPossibleWinScenarios, int> opponentBoard = new Dictionary<BoardPossibleWinScenarios, int>();
	public enum BoardPossibleWinScenarios { firstRow, secondRow, thirdRow, firstColumn, secondColumn, thirdColumn, forwardDiagonal, backDiagonal };
	/// <summary>
	/// Used to map strings to int to convert the board from a data structure of Strings to int and otherwise
	/// </summary>
	private Dictionary<string, int> boardStringIntegerMapper = new Dictionary<string, int>
		{
			{ "[0,0]", 0 }, { "[0,1]", 1 }, { "[0,2]", 2 },
			{ "[1,0]", 3 }, { "[1,1]", 4 }, { "[1,2]", 5 },
			{ "[2,0]", 6 }, { "[2,1]", 7 }, { "[2,2]", 8 },
		};
	public Dictionary<int, string> boardIntegerStringMapper = new Dictionary<int, string>
		{
			{0, "[0,0]"}, {1, "[0,1]"}, {2,  "[0,2]" },
			{3, "[1,0]" }, {4, "[1,1]" }, {5, "[1,2]" },
			{6, "[2,0]" }, {7, "[2,1]" }, {8, "[2,2]" },
		};
		
	public bool gameOver = false;

	void Awake() {
		InitBoard();
		InitializePlayersScore();
	}

	void Update() {
		if(!gameOver) {
			IsGameOver();
			gameOver = true;
		}
		if(gameOver && Input.GetKeyDown(KeyCode.R)) {
			RestartGame();
		}
	}

	private void InitBoard() {
		positionArr = GameObject.FindGameObjectsWithTag("Position");
	}

	public void InitializePlayersScore() {
		int score = 0;
		foreach(BoardPossibleWinScenarios boardPossibleWinScenario in Enum.GetValues(typeof(BoardPossibleWinScenarios))) {
			playerBoard.Add(boardPossibleWinScenario, score);
			opponentBoard.Add(boardPossibleWinScenario, score);
		}
	}

	//TODO: finish, remember to check for tie as well !
	public bool IsGameOver() {
		bool isGameOver = false;
		foreach(KeyValuePair<BoardPossibleWinScenarios, int> boardPosition in playerBoard) {
			if(playerBoard[boardPosition.Key].Equals(3)) {
				Debug.Log("player win");
				isGameOver = true;
			}
		}

		foreach(KeyValuePair<BoardPossibleWinScenarios, int> boardPosition in opponentBoard) {
			if(opponentBoard[boardPosition.Key].Equals(3))
			{
				Debug.Log("opponent win");
				isGameOver = true;
			}
		}

		int isTie = 0;
		foreach (GameObject o in positionArr) {
			Position parentPosition = o.GetComponent<Position>();
			isTie = parentPosition.isClicked ? isTie + 1 : isTie;
		}

		if (isTie == 9) {
			Debug.Log("tie");
			isGameOver = true;
		}

		if(isGameOver) {
			for(int i = 0; i < positionArr.Length; i++) {
				if(!positionArr[i].GetComponent<Position>().isClicked) {
					for(int j = 0; j < positionArr[i].transform.childCount; j++) {
						GameObject xOrCircle = positionArr[i].transform.GetChild(j).gameObject;
						if(xOrCircle.name.Equals(playersChoice)) {
							xOrCircle.SetActive(false);
						}
					}
				}
			}
		}
		return isGameOver;
	}

	public void RestartGame() {
		Debug.Log("restarting game");
		playersChoice = "";
		//reset wins
		foreach(BoardPossibleWinScenarios boardPossibleWinScenario in Enum.GetValues(typeof(BoardPossibleWinScenarios))) {
			playerBoard[boardPossibleWinScenario] = 0;
			opponentBoard[boardPossibleWinScenario] = 0;
		}
		//reset gamobjects
		for(int i = 0; i < this.positionArr.Length; i++) {
			positionArr[i].GetComponent<Position>().isClicked = false;
			for(int j = 0; j < this.positionArr[i].transform.childCount; j++) {
				GameObject xOrCircle = this.positionArr[i].transform.GetChild(j).gameObject;
				xOrCircle.GetComponent<ColorControl>().SetColor(xOrCircle.GetComponent<ColorControl>().AlphaColor);
				xOrCircle.SetActive(true);
			}
		}
	}

	public void IncrementPositionScore(string positionToIncrement, Dictionary<BoardPossibleWinScenarios, int> board) {
		switch (positionToIncrement) {
			case "[0,0]":
				board[BoardPossibleWinScenarios.firstRow]++;
				board[BoardPossibleWinScenarios.firstColumn]++;
				board[BoardPossibleWinScenarios.backDiagonal]++;
				break;
			case "[0,1]":
				board[BoardPossibleWinScenarios.firstRow]++;
				board[BoardPossibleWinScenarios.secondColumn]++;
				break;
			case "[0,2]":
				board[BoardPossibleWinScenarios.firstRow]++;
				board[BoardPossibleWinScenarios.thirdColumn]++;
				board[BoardPossibleWinScenarios.forwardDiagonal]++;
				break;
			case "[1,0]":
				board[BoardPossibleWinScenarios.secondRow]++;
				board[BoardPossibleWinScenarios.firstColumn]++;
				break;
			case "[1,1]":
				board[BoardPossibleWinScenarios.secondRow]++;
				board[BoardPossibleWinScenarios.secondColumn]++;
				board[BoardPossibleWinScenarios.forwardDiagonal]++;
				board[BoardPossibleWinScenarios.backDiagonal]++;
				break;
			case "[1,2]":
				board[BoardPossibleWinScenarios.secondRow]++;
				board[BoardPossibleWinScenarios.thirdColumn]++;
				break;
			case "[2,0]":
				board[BoardPossibleWinScenarios.thirdRow]++;
				board[BoardPossibleWinScenarios.firstColumn]++;
				board[BoardPossibleWinScenarios.forwardDiagonal]++;
				break;
			case "[2,1]":
				board[BoardPossibleWinScenarios.thirdRow]++;
				board[BoardPossibleWinScenarios.secondColumn]++;
				break;
			case "[2,2]":
				board[BoardPossibleWinScenarios.thirdRow]++;
				board[BoardPossibleWinScenarios.thirdColumn]++;
				board[BoardPossibleWinScenarios.backDiagonal]++;
				break;
		}
	}

	/// <summary>
	/// Plays the AI turn
	/// </summary>
	public int PlayAiTurn(GameObject gameObject) {
		int aiPlayChoice = GameController.playersChoice.Equals("X") ? -1 : 1;
		minimaxAlgo.aiPick = aiPlayChoice;
		
		EndTurnPosition bestPositionToEndTurn = minimaxAlgo.GetBestPosition(aiPlayChoice, ConvertBoardToInt(gameObject), 0);
		int newBestPositionToPick = bestPositionToEndTurn.position;
		
		//hide ai ui choices from player
		foreach(GameObject boardPositions in this.positionArr) {
			foreach(KeyValuePair<string, int> integerStringMapperEntry in boardStringIntegerMapper) {
				if(integerStringMapperEntry.Key == boardPositions.name && integerStringMapperEntry.Value == newBestPositionToPick) {
					if(GameController.playersChoice != boardPositions.transform.GetChild(0).gameObject.name) {
						gameObject.GetComponent<MouseEvents>().renderPositionClicked(boardPositions.transform.GetChild(0).gameObject);
						gameObject.GetComponent<ColorControl>().RenderAiOtherChoiceInvisible(boardPositions);
					} else {
						gameObject.GetComponent<MouseEvents>().renderPositionClicked(boardPositions.transform.GetChild(1).gameObject);
						gameObject.GetComponent<ColorControl>().RenderAiOtherChoiceInvisible(boardPositions);
					}
				}
			}
		}
		return newBestPositionToPick;
	}

	/// <summary>
	/// Converts the board from strings of [x,y] to integers for the minimax algo
	/// </summary>
	private int[] ConvertBoardToInt(GameObject gameObject) {
		int[] boardAsIntegers = new int[9];
		Color32 invisibleColor = gameObject.GetComponent<ColorControl>().InvisibleColor;
		foreach(GameObject position in this.positionArr) {
			foreach(KeyValuePair<string, int> integerStringMapperEntry in boardStringIntegerMapper) {
				if(integerStringMapperEntry.Key == position.name) {
					if(!position.GetComponent<Position>().isClicked) {
						boardAsIntegers[integerStringMapperEntry.Value] = 0;
					} else {
						if(!position.transform.GetChild(0).GetComponent<ColorControl>().GetColor().Equals(invisibleColor) && position.transform.GetChild(0).gameObject.name == "X") {
							boardAsIntegers[integerStringMapperEntry.Value] = 1;

						} else {
							boardAsIntegers[integerStringMapperEntry.Value] = -1;
						}
					}
				}
			}
		}
		return boardAsIntegers;
	}
}
