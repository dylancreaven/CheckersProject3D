﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CheckerBoard : MonoBehaviour
{
    public static CheckerBoard Instance { set; get; }
    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;


    public GameObject highlightContainer;

    public CanvasGroup alertCanvas;
    private float lastAlert;
    private bool alertActive;
    private bool gameIsOver;
    private float winTime;


    private Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0.125f, 0.5f);

    public bool isWhite;
    private bool isWhiteTurn;
    private bool isBlackTurn;
    private bool hasKilled;

    private Piece selectedPiece;
    private Piece randomPiece;
    private List<Piece> forcedPieces;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    int whitewins;
    int blackwins;


    private Player player;
    public int currentLevel;
    private void Start()
    {
        whitewins=PlayerPrefs.GetInt("WhiteWins");
        blackwins=PlayerPrefs.GetInt("BlackWins");
        currentLevel = whichLevel();
        alertCanvas.GetComponent<Image>().enabled = false;
        Instance = this;
        player = FindObjectOfType<Player>();

        foreach (Transform t in highlightContainer.transform)
        {

            t.position = Vector3.down * 100;
        }


        isWhite = true;
        Invoke("AlertIntro", 12f);
        Transform c = GameObject.Find("Canvas").transform;
        foreach (Transform t in c)
        {
            t.gameObject.SetActive(false);
        }

        c.GetChild(0).gameObject.SetActive(true);




        isWhiteTurn = true;
        isBlackTurn = false;
        GenerateBoard();
        forcedPieces = new List<Piece>();
    }

    private void Update()
    {
        //hotkeys to force a win to that i don't have to play through an entire game of checkers when testing
       /* if(Input.GetKeyDown(KeyCode.Space))
        {
            Victory(true); //test white wins
        }
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            Victory(false); //test black wins
        }*/
        if (gameIsOver)
        {
            if (Time.time - winTime > 3.0f)
            {
                SceneManager.LoadScene("Menu");
            }

            return;
        }

        foreach (Transform t in highlightContainer.transform)
        {
            t.Rotate(Vector3.up * 90 * Time.deltaTime);
        }

        UpdateAlert();
        UpdateMouseOver();
        if (currentLevel == 1)
        {
            if ((isWhite) ? isWhiteTurn : !isWhiteTurn)
            {
                int x = (int)mouseOver.x;
                int y = (int)mouseOver.y;

                if (selectedPiece != null)
                    UpdatePieceDrag(selectedPiece);

                if (Input.GetMouseButtonDown(0))
                    SelectPiece(x, y);

                if (Input.GetMouseButtonUp(0))
                    TryMove((int)startDrag.x, (int)startDrag.y, x, y);

            }
        }
        else
        {
			if(isWhite)
			{
				int x = (int)mouseOver.x;
                int y = (int)mouseOver.y;

                if (selectedPiece != null)
                    UpdatePieceDrag(selectedPiece);

                if (Input.GetMouseButtonDown(0))
                    SelectPiece(x, y);

                if (Input.GetMouseButtonUp(0))
                    TryMove((int)startDrag.x, (int)startDrag.y, x, y);

			}
            else if (isBlackTurn)
            {
				print("Black's turn, not white's");
				SelectPieceAI();
				TryMoveAI();
					

				
            }
        }
    }
	
    private static int whichLevel()
    {
        int y = SceneManager.GetActiveScene().buildIndex;
        if (y == 1)
        {
            print("In level 1v1");
        }
        else if (y == 2)
        {
            print("In level with AI");
        }
        return y;
    }
    private void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    private void UpdatePieceDrag(Piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    private void SelectPiece(int x, int y)
    {
        // Out of bounds
        if (x < 0 || x >= 8 || y < 0 || y >= 8)
            return;

        Piece p = pieces[x, y];
        if (p != null && p.isWhite == isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                // Look for the piece under our forcedPieces list
                if (forcedPieces.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }
	private void SelectPieceAI()
    {
		
		int randX = Random.Range(0,11);
		int randY = Random.Range(0,11);
        foreach (var x in pieces)
		{
			Debug.Log("Each piece: "+x);
		}
		Piece p = pieces[1,1];
		Debug.Log("Piece: "+p);
		while(p==null)
		{
			randX = Random.Range(0,11);
			randY = Random.Range(0,11);
			p = pieces[randX,randY];
		}
        if (p != null && p.isWhite != isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                // Look for the piece under our forcedPieces list
                if (forcedPieces.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }
	public void TryMoveAI()
    {
		int startX, startY =0;
        forcedPieces = ScanForPossibleMove();

		
		startX=(int)selectedPiece.transform.position.x;
		startY=(int)selectedPiece.transform.position.y;
      
		int endX = Random.Range(0,7);
		int endY = Random.Range(0,7);
        startDrag = new Vector2(startX, startY);
        endDrag = new Vector2(endX, endY);
        selectedPiece = pieces[startX, startY];

        // Out of bounds
        if (endX < 0 || endX >= 8 || endY < 0 || endY >= 8)
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece, startX, startY);

            startDrag = Vector2.zero;
            selectedPiece = null;
            Highlight();
            return;
        }

        if (selectedPiece != null)
        {
            // If it has not moved
            if (endDrag == startDrag)
            {
                MovePiece(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                Highlight();
                return;
            }

            // Check if it is a valid move
            if (selectedPiece.ValidMove(pieces, startX, startY, endX, endY))
            {
                // Did we kill anything?
                // If it is a jump
                if (Mathf.Abs(endX - startX) == 2)
                {
                    Piece p = pieces[(startX + endX) / 2, (startY + endY) / 2];
                    if (p != null)
                    {
                        pieces[(startX + endX) / 2, (startY + endY) / 2] = null;
                        DestroyImmediate(p.gameObject);
                        hasKilled = true;
                    }
                }

                // Were we supposed to kill anything?
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    MovePiece(selectedPiece, startX, startY);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    Highlight();
                    return;
                }

                pieces[endX, endY] = selectedPiece;
                pieces[startX, startY] = null;
                MovePiece(selectedPiece, endX, endY);

                EndTurn();
            }
            else
            {
                MovePiece(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                Highlight();
                return;
            }
        }

    }
    public void TryMove(int startX, int startY, int endX, int endY)
    {
        forcedPieces = ScanForPossibleMove();

        // Multiplayer Support
        startDrag = new Vector2(startX, startY);
        endDrag = new Vector2(endX, endY);
        selectedPiece = pieces[startX, startY];

        // Out of bounds
        if (endX < 0 || endX >= 8 || endY < 0 || endY >= 8)
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece, startX, startY);

            startDrag = Vector2.zero;
            selectedPiece = null;
            Highlight();
            return;
        }

        if (selectedPiece != null)
        {
            // If it has not moved
            if (endDrag == startDrag)
            {
                MovePiece(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                Highlight();
                return;
            }

            // Check if it is a valid move
            if (selectedPiece.ValidMove(pieces, startX, startY, endX, endY))
            {
                // Did we kill anything?
                // If it is a jump
                if (Mathf.Abs(endX - startX) == 2)
                {
                    Piece p = pieces[(startX + endX) / 2, (startY + endY) / 2];
                    if (p != null)
                    {
                        pieces[(startX + endX) / 2, (startY + endY) / 2] = null;
                        DestroyImmediate(p.gameObject);
                        hasKilled = true;
                    }
                }

                // Were we supposed to kill anything?
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    MovePiece(selectedPiece, startX, startY);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    Highlight();
                    return;
                }

                pieces[endX, endY] = selectedPiece;
                pieces[startX, startY] = null;
                MovePiece(selectedPiece, endX, endY);

                EndTurn();
            }
            else
            {
                MovePiece(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                Highlight();
                return;
            }
        }

    }

    private void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        // Promotions
        if (selectedPiece != null)
        {
            if (selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.GetComponentInChildren<Animator>().SetTrigger("FlipTrigger");
            }
            else if (!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.GetComponentInChildren<Animator>().SetTrigger("FlipTrigger");
            }
        }


        selectedPiece = null;
        startDrag = Vector2.zero;

        if (ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasKilled)
            return;

        isWhiteTurn = !isWhiteTurn;

        if (isWhiteTurn == true)
        {
            isBlackTurn = false;
           // print("White's Turn a.k.a Player's turn");
        }

        if (isWhiteTurn == false)
        {
            isBlackTurn = true;
            //print("Black's Turn a.k.a AI's turn");
        }

        if (!player) isWhite = !isWhite;

        hasKilled = false;
        CheckVictory();

        ScanForPossibleMove();
    }

    private void CheckVictory()
    {
        var ps = FindObjectsOfType<Piece>();
        bool hasWhite = false, hasBlack = false;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].isWhite)
                hasWhite = true;
            else
                hasBlack = true;
        }

        if (!hasWhite)
            Victory(false);
        if (!hasBlack)
            Victory(true);
    }

    private void Victory(bool isWhite)
    {
        winTime = Time.time;

        if (isWhite){
           whitewins++;
           PlayerPrefs.SetInt("WhiteWins",whitewins);
            Alert("White team has won");
        }
        else{
            blackwins++;
            PlayerPrefs.SetInt("BlackWins",blackwins);
            Alert("Black team has won");
        }

        gameIsOver = true;
    }

    private List<Piece> ScanForPossibleMove(Piece p, int x, int y)
    {
        forcedPieces = new List<Piece>();

        if (pieces[x, y].IsForceToMove(pieces, x, y))
            forcedPieces.Add(pieces[x, y]);

        Highlight();
        return forcedPieces;
    }
    private List<Piece> ScanForPossibleMove()
    {
        forcedPieces = new List<Piece>();

        // Check all the pieces
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                    if (pieces[i, j].IsForceToMove(pieces, i, j))
                        forcedPieces.Add(pieces[i, j]);
            }
        }

        Highlight();
        return forcedPieces;
    }

    private void Highlight()
    {
        foreach (Transform t in highlightContainer.transform)
        {
            t.position = Vector3.down * 100;
        }

        if (forcedPieces.Count > 0)
            highlightContainer.transform.GetChild(0).transform.position = forcedPieces[0].transform.position + Vector3.down * 0.1f;

        if (forcedPieces.Count > 1)
            highlightContainer.transform.GetChild(1).transform.position = forcedPieces[1].transform.position + Vector3.down * 0.1f;
    }

    private void GenerateBoard()
    {
        // Generate White team
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                // Generate our Piece
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }

        // Generate Black team
        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                // Generate our Piece
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }

    private void GeneratePiece(int x, int y)
    {
        bool isWhitePiece = (y > 3) ? false : true;
        GameObject go = Instantiate((isWhitePiece) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }

    public void Alert(string text)
    {
        alertCanvas.GetComponentInChildren<Text>().text = text;
        alertCanvas.GetComponent<Image>().color = Color.red;
        alertCanvas.alpha = 1;
        lastAlert = Time.time;
        alertActive = true;

    }
    public void AlertIntro()
    {
        alertCanvas.GetComponent<Image>().enabled = true;
        alertCanvas.GetComponentInChildren<Text>().text = "White Player's Turn";
        alertCanvas.alpha = 1;
        lastAlert = Time.time;
        alertActive = true;

    }

    public void UpdateAlert()
    {
        if (alertActive)
        {
            if (Time.time - lastAlert > 1.5f)
            {
                alertCanvas.alpha = 1 - ((Time.time - lastAlert) - 1.5f);

                if (Time.time - lastAlert > 2.5f)
                {
                    alertActive = false;
                }
            }
        }
    }
	private void AIMakeMove(Piece randomPiece){
		
		print("You gonnna make a move or what");
	}
}
