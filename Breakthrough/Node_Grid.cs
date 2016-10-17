using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Breakthrough
{



    public class Grid
    {
        // building a Grid class to represent each grid on the chessboard
        public int _x;
        public int _y;

        public Grid(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        private int x
        {
            get { return this._x; }
            set { this._x = value; }
        }
        private int y
        {
            get { return this._y; }
            set { this._y = value; }
        }
    }





    public class Node : IEquatable<Node>
    {
        // Piece location
        private Grid _grid;
        private int _x;
        private int _y;

        private int[,] _chessBoard; // input the initial chess board "board.txt"
        private Node _parentNode;  // reference to parent node
        private List<Node> _childNodes; // list of child nodes
        private Node _OtherNnode;

        private float _evalFunctionValue; //the difference between # offenders and # deffenders

        private int _turn; // claim which turn this node belongs, define "offender" as "true" & "deffender" as "false"
        // Player1's home is located up
        // Player2's home is located down

        // public bool _isInitialized;
        // private char _role; // "o" or "x"
        private int _depth;

        public Node(Grid grid, int[,] chessBoard)
        {
            this._grid = grid;
            this._chessBoard = chessBoard;
            //this._isInitialized = true;
        }

        public Node(Grid grid, int[,] chessBoard, int turn)
        {
            this._grid = grid;
            this._chessBoard = chessBoard;
            this._turn = turn;
            //this._isInitialized = true;
        }

        public Node(Grid grid, int[,] chessBoard, Node parentNode)
        {
            this._grid = grid;
            this._chessBoard = chessBoard;
            this._parentNode = parentNode;
            //this._isInitialized = true;
        }

        public Grid grid
        {
            get { return this._grid; }
            set { this._grid = value; }
        }

        public int[,] chessBoard
        {
            get { return this._chessBoard; }
            set { this._chessBoard = value; }
        }

        public int turn
        {
            get { return this._turn; }
            set { this._turn = value; }
        }

        public Node parentNode
        {
            get { return this._parentNode; }
            set { this._parentNode = value; }

        }

        public List<Node> childNodes
        {
            get { return this._childNodes; }
            set { this._childNodes = value; }

        }

        //public bool Equals(Node n)
        //{

        //    for (int y = 0; y < 8; y++)
        //    {
        //        for (int x = 0; x < 8; x++)
        //        {
        //            if (!this._heuristicValue.Equals(n._heuristicValue))
        //            {
        //                return false;
        //            }
        //        }
        //    }

        //    return true;
        //}

        private int calcNumOfPlayer1(int[,] chessBoard)
        {
            // calculateing number of player1 remaining on the board
            int numOfPlayer1 = 0;
            foreach (int item in chessBoard)
                if (item == -1)
            {
                {
                    numOfPlayer1++;
                }
            }
            return numOfPlayer1;
        }

        private int calcNumOfPlayer2(int[,] chessBoard)
        {
            // calculateing number of pieces remaining on the board
            int numOfPlayer2 = 0;
            foreach (int item in chessBoard)
            {
                if (item == -1)
                {
                    numOfPlayer2++;
                }
            }
            return numOfPlayer2;
        }

        private bool isWalkable(Grid grid, int turn)
        {
            // checking whether piece at [x, y] is walkable
            try
            {
                if (this._chessBoard[grid._x - 1 * turn, grid._y - 1] == 0 || this._chessBoard[grid._x - 1 * turn, grid._y] == 0 || this._chessBoard[grid._x - 1 * turn, grid._y + 1] == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private int calcNumOfThreats(Grid grid, int turn)
        {
            // calculating number of threats
            int numOfThreats = 0;
            for (int i=0;  i<7; i++)
            {
                for (int j=0; j<7; j++)
                {
                    if (this._chessBoard[i, j] == -turn && (i - grid._x) * turn < 0)
                    {
                        if (isWalkable(grid, turn))
                        {
                            numOfThreats++;
                        }
                    }
                }
            }
            return numOfThreats;
        }



        private List<int []> findThreats(Grid grid, int turn)
        {
            // finding all threats for piece at [x, y] and saving them in a list
            // var threat = new Tuple<int, int>;
            int[] threat;
            threat = new int[2];
            List <int []> threatsList = new List<int []>();
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (this._chessBoard[i, j] == -turn && (i - grid._x) * turn < 0)
                    {
                        if (isWalkable(grid, turn))
                        {
                            threat[0] = i;
                            threat[1] = j;
                            threatsList.Add(threat);
                        }
                    }
                }
            }
            return threatsList;
        }

        private float calcMeanManhattanDistance(Grid grid, int turn)
        {
            // calculating mean Manhattan distance used in the evaluation function
            int i = 0;
            int j = 0;
            int sumManhattanDistance = 0;
            //List<int> threats = new List<int>();
            List<int []> threatsList = findThreats(grid, turn);
            foreach (int[] position in threatsList)
            {
                i = position[0];
                j = position[1];
                sumManhattanDistance += calcManhattanDistance(grid, i, j);
            }
            return sumManhattanDistance / threatsList.Count;
        }
        
        private int calcManhattanDistance(Grid grid, int m, int n)
        {
            // calculating Manhattan distance
            int z = 0;
            int xdelta = 0;
            int ydelta = 0;

            xdelta = Math.Abs(grid._x - m);
            ydelta = Math.Abs(grid._y - n);

            z = xdelta + ydelta;
            return z;
        }

        private int numOfPieces(int turn)
        {
            // calculate number of pieces
            int numOfPieces = 0;
            foreach (int item in this._chessBoard)
            {
                if (item == turn)
                {
                    numOfPieces++;
                }
            }
            return numOfPieces;
        }

        private int numOfThreats(Grid grid, int turn)
        {
            List<int[]> listOfThreats = new List<int[]>();
            listOfThreats = this.findThreats(grid, turn);
            return listOfThreats.Count;
        }

        private int maxManhattanDistToHomeBase(Grid grid, int turn)
        {
            // calculating the Manhattan distance of the furthest piece of PlayerX
            int temp = 0;
            if (turn == -1)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (this._chessBoard[i, j] == turn)
                        {
                            if (i > temp)
                            {
                                temp = i;
                            }
                        }
                    }
                }
            }
            if (turn == -1)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (this._chessBoard[i, j] == turn)
                        {
                            if (i < temp)
                            {
                                temp = i;
                            }
                        }
                    }
                }
            }
            return temp;
        }

        private int numOfEnemyToCapture(Grid grid, int turn)
        {
            // calculating number of enemy's pieces to capture
            int numOfEnemyToCapture = 0;
            if (this._chessBoard[grid._x - turn, grid._y - 1] == turn)
            {
                numOfEnemyToCapture++;
            }
            if (this._chessBoard[grid._x - turn, grid._y] == turn)
            {
                numOfEnemyToCapture++;
            }
            if (this._chessBoard[grid._x - turn, grid._y + 1] == turn)
            {
                numOfEnemyToCapture++;
            }
            return numOfEnemyToCapture;
        }

        private int calcEvalFunctionValue(Grid grid, int turn)
        {
            // calculating heuristic value of one of the three collums, which respectively shifted by +1/0/-1
            return 0;
        }

        public void addChild(Node tmpNode)
        {
            try
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<Node>();
                }
                _childNodes.Add(tmpNode);  //Null reference

            }
            catch (Exception e)
            {
                Console.WriteLine("Error adding Child for x: " + tmpNode._x + ", y: " + tmpNode._y + " due to: " + e.InnerException + " " + e.Message);
            }
        }

        public Node maxValue(Node currentNode)
        {
            // maximizer
            List<Node> sortList = new List<Node>();
            sortList = currentNode.childNodes.OrderByDescending(o => o._evalFunctionValue).ToList();
            Node nextNode = sortList[0];
            return nextNode;
        }

        public Node miniValue(Node currentNode)
        {
            // minimizer
            List<Node> sortList = new List<Node>();
            sortList = currentNode.childNodes.OrderBy(o => o._evalFunctionValue).ToList();
            Node nextNode = sortList[0];
            return nextNode;
        }

        

        public void showNodeInfo()
        {
            Console.WriteLine("******************");
            Console.WriteLine("Current Node ");
            Console.WriteLine("Variable to assign: ");
            Console.WriteLine(" X Coordinate: " + this._x);
            Console.WriteLine(" Y Coordinate: " + this._y);

            Console.WriteLine("Breakthrough Board:");

            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    Console.Write(this._chessBoard[x, y]);
                }
                Console.WriteLine();
            }
            //Console.WriteLine(this.Assignment);
            Console.WriteLine("******************");
        }

        public bool Equals(Node other)
        {
            return ((IEquatable<Node>)_parentNode).Equals(other);
        }

    }



}
