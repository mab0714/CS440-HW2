using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Breakthrough
{
    class Program
    {
        static void Main(string[] args)
        {
            // Breakthrough: Initial Board Data
            int[,] breakthroughBoard = new int[8, 8];

            int x = 0;
            int y = 0;

            // Initialize the chessboard
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i == 0 || i == 1)
                    {
                        breakthroughBoard[i, j] = -1;
                    }
                    else if (i == 6 || i == 7)
                    {
                        breakthroughBoard[i, j] = 1;
                    }
                    else
                    {
                        breakthroughBoard[i, j] = 0;
                    }
                }
            }

            Dictionary<string, int> players = new Dictionary<string, int>();
            players.Add("Player1", -1);
            players.Add("Player2", 1);
            int currdepth = 0;
            int maxDepth = 3;
            int turn = -1;
            Node startNode = new Node(1, 4, breakthroughBoard, players["Player1"]);
            startNode.depth = 0;
            Node currNode = new Node(1, 4, breakthroughBoard, players["Player1"], startNode);
            List<Node> frontierNodes = new List<Node>();
            List<Node> nextFrontierNodes = new List<Node>();
            //List<Node> nextLevelNodes = new List<Node>();
            frontierNodes.Add(currNode);

            bool reachMaxDepth = false;
            bool backToRoot = false;

            //startNode.showNodeInfo();
            

            // Player1: offensive-minimax
            // Player2: deffensive-minimax

            // forward-propagation
            while (!reachMaxDepth)
            {
                if (!(currdepth < maxDepth))
                {
                    reachMaxDepth = true;
                }
                foreach (Node frontierNode in frontierNodes)
                {
                    List<Node> nextLevelNodes = new List<Node>();
                    foreach (Node tempNode in frontierNode.getSuccessor(breakthroughBoard, turn))
                    {
                        tempNode.parentNode = frontierNode;
                        tempNode.depth = currdepth+1;
                        tempNode.turn = -turn;
                        nextLevelNodes.Add(tempNode);
                        nextFrontierNodes.Add(tempNode);
                        frontierNode.childNodes.Add(tempNode);
                    }
                    //frontierNode.childNodes.AddRange(nextLevelNodes);
                }
                frontierNodes = nextFrontierNodes;
                currdepth++;
            }


            // back-propagation
            
            while (!backToRoot)
            {
                List<Node> formerLevelNodes = new List<Node>();
                foreach (Node tempNode in frontierNodes)
                {
                    tempNode.evalFunctionValue = tempNode.calcEvalFunctionValue();
                    if (tempNode.parentNode.turn == players["Player1"])
                    {
                        if (tempNode.evalFunctionValue > tempNode.parentNode.evalFunctionValue)
                        {
                            tempNode.parentNode.evalFunctionValue = tempNode.evalFunctionValue;
                        }
                    }
                    if (tempNode.parentNode.turn == players["Player2"])
                    {
                        if (tempNode.evalFunctionValue < tempNode.parentNode.evalFunctionValue)
                        {
                            tempNode.parentNode.evalFunctionValue = tempNode.evalFunctionValue;
                        }
                    }
                    if (!formerLevelNodes.Contains(tempNode.parentNode))
                    {
                        formerLevelNodes.Add(tempNode.parentNode);
                    }
                }
                currdepth--;
                if (currdepth == 0)
                {
                    backToRoot = true;
                }
                frontierNodes = formerLevelNodes;
            }
            



    //        List<Node> nextOptions = new List<Node>();
    //        Node startNode = new Node(0, 0, 'o');
    //        Node currentNode = startNode;
    //        List<Node> finalPathOfNodes = new List<Node>(); // propagate the process of game
    //        char role = ' ';
    //        roles.Add(true, 'o');
    //        roles.Add(false, 'x');

    //        bool status = false;
    //        int maxDepth = 0;  // depth limited
    //        int tempDepth = 0;
    //        bool roleIndex = true;
    //        nextOptions.Add(currentNode);
    //        while (!status) // move one worker step by step
    //        {
    //                status = findNextMoveDLS(nextOptions, breakthroughBoardData, finalPathOfNodes, maxDepth);
    //                tempDepth++;
    //                role = roles[!roleIndex]; // move "o" and "x" in turns
    //        }

    //        if (visited all nodes)
    //        {
    //            propagateNodes();
    //        }
            
    //    }



    //    public bool propagateNode(List<Node> nextOptions, char [,] boardData, List<Node> finalPathOfNodes, char role, int tempDepth)
    //    {
    //        // implement each move of each worker
    //        Node currentNode = new Node(0, 0, 0, null);
    //        Node nextNode = new Node(0, 0, 0, null);

    //        if (role == 'o')
    //        {
    //            nextOptions.Add(miniValue(currentNode));
    //        }

    //        if (role == 'x')
    //        {
    //            nextOptions.Add(MaxValue(currentNode));
    //        }

    //        if (currentNode._depth > tempDepth)
    //        {
    //            return false;
    //        }

    //    }
        }
    }
}
