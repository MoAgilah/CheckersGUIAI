#ifndef CheckersDLL_EXPORT
#define CheckersDLL_EXPORT

#define CHECKERS __declspec(dllimport)

#include "humanPlayer.hpp"
#include "checkersboard.cpp"
#include "CheckersSearch.hpp"
#include "../Cuda/support.h"
#include "../Cuda/kernel.hu"

extern "C"{
	CheckersBoard Board;
	CheckersSearch Search;
	int depth = 9;

	CHECKERS void ChangeDepth(int val);
	CHECKERS int GetDepth();

	CHECKERS int GetPiece(unsigned int x, unsigned int y);
	CHECKERS void ResetBoard();
	CHECKERS void Clear();
	CHECKERS void SearchMove(bool playerID);
	CHECKERS void UpdateBoardWithMove(unsigned int xDst, unsigned int yDst, unsigned int xSrc, unsigned int ySrc);
	CHECKERS void UpdateBoardWithCapture(unsigned int xDst, unsigned int yDst, unsigned int xSrc, unsigned int ySrc);

	CHECKERS void SetP1Man(unsigned int index);
	CHECKERS void SetP2Man(unsigned int index);

	CHECKERS void SetP1King(unsigned int index);
	CHECKERS void SetP2King(unsigned int index);

	CheckersBoard nextBoard;
	CheckersBoard startBoard;

	CHECKERS void GetNextBoard();
	CHECKERS void SwapTurn();
}

#endif