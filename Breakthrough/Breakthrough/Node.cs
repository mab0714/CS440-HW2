using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Breakthrough
{
    public class Node : IEquatable<Node>
    {
        // Node location
        private int _x;
        private int _y;

        // Unused
        //private char _val;

        private char[,] _chessBoard; // input the initial chess board "board.txt"
        private List<Node> _remainingWorkers; // workers remaining on the board
        private Node _parentNode;  // reference to parent node
        private List<Node> _childNodes; // list of child nodes
        private Node _OtherNnode;

        private int _heuristicValue; //the difference between # offenders and # deffenders

        private string _turn; // claim which turn this node belongs, define "offender" as "true" & "deffender" as "false"

        public bool _isInitialized;
        private char _role; // "o" or "x"
        private int _depth;

        public Node(int x, int y)
        {
            this._x = x;
            this._y = y;
            this._isInitialized = true;
        }

        public Node(int x, int y, char _role)
        {
            this._x = x;
            this._y = y;
            this._role = role;
            this._isInitialized = true;
        }

        public Node(int x, int y, float _value, Node parentNode)
        {
            this._x = x;
            this._y = y;
            this._parentNode = parentNode;
            this._isInitialized = true;
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

        public char role
        {
            get { return this._role; }
            set { this._role = value; }
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

        private int calcManhattanDistance(int x, int y)
        {
            // calculating Manhattan distance
            int z = 0;
            int xdelta = 0;
            int ydelta = 0;

            xdelta = Math.Abs(x - this._OtherNnode.x);
            ydelta = Math.Abs(y - this._OtherNnode.y);

            z = xdelta + ydelta;
            return z;
        }

        private int calcHeuristicValue(Node currentNode, List<Node> remainingWorkers, int bias)
        {
            //calculating heuristic value of one of the three collums, which respectively shifted by +1/0/-1
            int currX = currentNode.x;
            int numO = 0;
            int numX = 0;
            int H = 0;

            foreach (Node n in remainingWorkers)
            {
                if (n.x == currX + bias)
                {
                    if (n.role == 'o')
                    {
                        numO++;
                    }
                    if (n.role == 'x')
                    {
                        numX++;
                    }
                }
            }
            H = numO - numX;
            return H;
        }


        //public List<Node> checkEligibleMove(char [,] _chessBoard)
        //{
        //    // See N   check if parentNode is not null, then check i'm not going to revisit a parent
        //    Node tmpNode = new Node(this._x, this._y - 1, this);

        //    if (isWalkable(this._x, this._y - 1, mazeBoard))
        //    {
        //        tmpNode.g = tmpNode.parentNode.g + 1;
        //        tmpNode.h = calcManhattanDistance(this._x, this.y - 1);
        //        tmpNode.f = tmpNode.g + tmpNode.h;
        //        tmpNode.goalStateNode = this.goalStateNode;
        //        AddChild(tmpNode);
        //    }

        //    // see e
        //    Node tmpnode = new node(this._x + 1, this._y, this);
        //    if (iswalkable(this._x + 1, this._y, mazeboard))
        //    {
        //        tmpnode.g = tmpnode.parentnode.g + 1;
        //        tmpnode.h = calcmanhattandistance(this._x + 1, this._y);
        //        tmpnode.f = tmpnode.g + tmpnode.h;
        //        tmpnode.goalstatenode = this.goalstatenode;
        //        addchild(tmpnode);
        //    }

        //    // see s
        //    tmpnode = new node(this._x, this._y + 1, this);
        //    if (iswalkable(this._x, this._y + 1, mazeboard))
        //    {
        //        tmpnode.g = tmpnode.parentnode.g + 1;
        //        tmpnode.h = calcmanhattandistance(this._x, this._y + 1);
        //        tmpnode.f = tmpnode.g + tmpnode.h;
        //        tmpnode.goalstatenode = this.goalstatenode;
        //        addchild(tmpnode);
        //    }

        //    //see w
        //    tmpnode = new node(this._x - 1, this._y, this);
        //    if (iswalkable(this._x - 1, this._y, mazeboard))
        //    {
        //        tmpnode.g = tmpnode.parentnode.g + 1;
        //        tmpnode.h = calcmanhattandistance(this._x - 1, this._y);
        //        tmpnode.f = tmpnode.g + tmpnode.h;
        //        tmpnode.goalstatenode = this.goalstatenode;
        //        addchild(tmpnode);
        //    }

        //    if (this._parentnode != null && this._childnodes != null)
        //    {
        //        _childnodes.remove(this._parentnode);
        //    }
        //    return _childnodes;
        //}


        private bool iswalkable(int x, int y, List<Node> remainingWorkers)
        {
            if(remainingWorkers.Contains(new Node(x, y)))
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        //public char[,] updateBoard(char[,] board, string direction, int x, int y, string word, Dictionary<string, int> newVariablePriority)
        //{
        //    char[] charWord = word.ToCharArray();
        //    int charWordX = 0;
        //    int charWordY = 0;
        //    if (direction.ToUpper().Equals("H"))
        //    {
        //        for (int tmpX = x; tmpX < charWord.Length; tmpX++)
        //        {
        //            board[tmpX, y] = charWord[charWordX];
        //            newVariablePriority.Remove(tmpX + "_" + y);
        //            charWordX++;
        //        }
        //    }
        //    else
        //    {
        //        for (int tmpY = y; tmpY > y - charWord.Length; tmpY--)
        //        {
        //            board[x, tmpY] = charWord[charWordY];
        //            newVariablePriority.Remove(x + "_" + tmpY);
        //            charWordY++;
        //        }
        //    }

            //// recalculate variablePriority
            //for (int tmpX = 0; tmpX <= 8; tmpX++)
            //{
            //    for (int tmpY = 8; tmpX >= 0; tmpY--)
            //    {
            //        if (c.Equals('_'))
            //        {
            //            newVariablePriority.Add(tmpX + "_" + tmpY, (8 - tmpX + tmpY));
            //        }
            //    }
            //}

        //    return board;
        //}

        public void AddChild(Node tmpNode)
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
            sortList = currentNode.childNodes.OrderByDescending(o => o._heuristicValue).ToList();
            Node nextNode = sortList[0];
            return nextNode;
        }

        public Node miniValue(Node currentNode)
        {
            // minimizer
            List<Node> sortList = new List<Node>();
            sortList = currentNode.childNodes.OrderBy(o => o._heuristicValue).ToList();
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

            for (int y = 0; y > 8; y++)
            {
                for (int x = 0; x < 8; x++)
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
