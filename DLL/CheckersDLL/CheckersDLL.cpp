#include "CheckersDLL.h"

extern "C"{
	
	void ChangeDepth(int val)
	{
		depth = val;
	}

	int GetDepth()
	{
		return depth;
	}

	int GetPiece(unsigned int x, unsigned int y)
	{	
		return (int)Board.getPieceChar((y * 8) + x);
	}

	void ResetBoard()
	{
		Board = CheckersBoard();
	}

	void Clear()
	{
		Board.clear();
	}

	void SearchMove(bool playerID)
	{
		Board = Search.search(&Board, playerID, depth);
	}

	void UpdateBoardWithMove(unsigned int xDst, unsigned int yDst, unsigned int xSrc, unsigned int ySrc)
	{
		int destination = xDst + (yDst * 8);
		int source = xSrc + (ySrc * 8);
		Board.move(destination, source);
	}

	void UpdateBoardWithCapture(unsigned int xDst, unsigned int yDst,unsigned int xSrc, unsigned int ySrc)
	{
		int destination = xDst + (yDst * 8);
		int source = xSrc + (ySrc * 8);
		int victim = ((xDst + xSrc) / 2) + (((yDst + ySrc) / 2) * 8);

		Board.captureMove(destination, victim, source);
	}

	void SetP1Man(unsigned int index)
	{
		Board.addP1Man(index);
	}

	void SetP2Man(unsigned int index)
	{
		Board.addP2Man(index);
	}

	void SetP1King(unsigned int index)
	{
		Board.addP1King(index);
	}

	void SetP2King(unsigned int index)
	{
		Board.addP2King(index);
	}

	void GetNextBoard()
	{
		nextBoard = getOptimalNextBoard(startBoard);

		startBoard = nextBoard;
	}

	void SwapTurn()
	{
		if (!nextBoard.getP1Up())
			nextBoard.swapPlayers();
	}
}








/*
CheckersBoard nextBoard;
CheckersBoard startBoard;//(0x00002DFF,0xFF930000,0);


	
if (!nextBoard.getP1Up())
	nextBoard.swapPlayers();
	*/
	

