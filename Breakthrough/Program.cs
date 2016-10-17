using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

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
            for (int i=0;i<7;i++)
            {
                for (int j = 0; j < 7; j++)
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