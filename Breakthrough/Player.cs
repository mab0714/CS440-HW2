using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakthrough
{
    class Player
    {
        private int _x;
        private int _y;

        private int[,] _chessBoard;
        private int _strategy;
        private string _algorithm;
        private int _depthToPredict;

        public Player(int x, int y, int [,] chessBoard, char strategy, string algorithm, int depthToPredict)
        {
            this._x = x;
            this._y = y;
            this._chessBoard = chessBoard;
            this._strategy = strategy;
            this._algorithm = algorithm;
            this._depthToPredict = depthToPredict;
        }

        public int x
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public int y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public int[,] chessBoard
        {
            get { return this._chessBoard; }
            set { this._chessBoard = value; }
        }

        public int strategy
        {
            get { return this._strategy; }
            set { this._strategy = value; }
        }

        public string algorithm
        {
            get { return this._algorithm; }
            set { this._algorithm = value; }
        }

        public int depth
        {
            get { return this._depthToPredict; }
            set { this._depthToPredict = value; }
        }
    }
}
