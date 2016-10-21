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
            int maxDepth = 2;
            int turn = -1;
            Node startNode = new Node(1, 4, breakthroughBoard, players["Player1"]);
            startNode.depth = 0;
            Node currNode = new Node(1, 4, breakthroughBoard, players["Player1"], startNode);
            List<Node> frontierNodes = new List<Node>();
            List<Node> nextFrontierNodes = new List<Node>();
            List<List<Node>> everyLevelNodes = new List<List<Node>>();
            frontierNodes.Add(currNode);

            bool reachMaxDepth = false;
            //bool backToRoot = false;

            //startNode.showNodeInfo();
            //Console.ReadLine();
            /**********************************TEST********************************/
            //startNode.showNodeInfo();
            //for (currdepth=0; currdepth<=maxDepth; currdepth++)
            //{
            //    foreach (Node frontierNode in frontierNodes)
            //    {
            //        List<Node> nextLevelNodes = new List<Node>();
            //        foreach (Node tempNode in frontierNode.getSuccessor())
            //        {
            //            tempNode.parentNode = frontierNode;
            //            tempNode.depth = currdepth + 1;
            //            tempNode.turn = -turn;
            //            nextLevelNodes.Add(tempNode);
            //            nextFrontierNodes.Add(tempNode);
            //            frontierNode.childNodes = nextLevelNodes;
            //            tempNode.showNodeInfo();
            //        }
            //        Console.WriteLine(nextLevelNodes.Count);
            //        //frontierNode.childNodes.AddRange(nextLevelNodes);
            //    }
            //    frontierNodes = nextFrontierNodes;
            //    currdepth++;
            //}
            //Console.ReadLine();
            /************************************TEST*********************************/



            /*****************************************Test Minimax**************************************/
            Node rootNode = new Node();
            List<Node> bottomNodes = new List<Node>();
            rootNode.evalFunctionValue = 0;
            rootNode.turn = -1;
            List<Node> leafNodes = new List<Node>();
            List<Node> leafleafNodes1 = new List<Node>();
            List<Node> leafleafNodes2 = new List<Node>();
            Node node1 = new Node(); node1.evalFunctionValue = 1; node1.depth = 1;
            Node node2 = new Node(); node2.evalFunctionValue = 2; node2.depth = 1;
            Node node3 = new Node(); node3.evalFunctionValue = 3; node3.depth = 2;
            Node node4 = new Node(); node4.evalFunctionValue = 4; node4.depth = 2;
            Node node5 = new Node(); node5.evalFunctionValue = 5; node5.depth = 2;
            Node node6 = new Node(); node6.evalFunctionValue = 6; node6.depth = 2;

            leafNodes.Add(node1);leafNodes.Add(node2);
            leafleafNodes1.Add(node3); leafleafNodes1.Add(node4);
            leafleafNodes2.Add(node5); leafleafNodes2.Add(node6);
            bottomNodes.Add(node3); bottomNodes.Add(node4); bottomNodes.Add(node5); bottomNodes.Add(node6);

            rootNode.childNodes = leafNodes;
            node1.childNodes = leafleafNodes1;
            node2.childNodes = leafleafNodes2;

            node1.parentNode = rootNode;
            node2.parentNode = rootNode;
            node3.parentNode = node1;
            node4.parentNode = node1;
            node5.parentNode = node2;
            node6.parentNode = node2;

            Node temp = new Node();
            List<Node> tempList = new List<Node>();
            temp.evalFunctionValue = 1;
            temp = alphabeta(rootNode, bottomNodes, maxDepth);
            Console.WriteLine(temp.evalFunctionValue);
            Console.ReadLine();


            /*****************************************Tree Building**************************************/


            //// forward-propagation
            //while (!reachMaxDepth)
            //{
            //    // build the tree until reach the max depth we set
            //    if (!(currdepth < maxDepth))
            //    {
            //        reachMaxDepth = true;
            //    }
            //    everyLevelNodes[currdepth] = frontierNodes; // store every level of nodes
            //    foreach (Node frontierNode in frontierNodes)
            //    {
            //        // dealing with every node on the frontier
            //        List<Node> nextLevelNodes = new List<Node>();
            //        foreach (Node tempNode in frontierNode.getSuccessor())
            //        {
            //            // dealing with every possible child node related to each node on frontier
            //            tempNode.parentNode = frontierNode;
            //            tempNode.depth = currdepth + 1;
            //            tempNode.turn = -turn; // flip the turn to change player
            //            nextLevelNodes.Add(tempNode);
            //            nextFrontierNodes.Add(tempNode);
            //            frontierNode.childNodes = nextLevelNodes; // add these nodes on the next level to that node on frontier as its child-nodes
            //        }
            //        //frontierNode.childNodes.AddRange(nextLevelNodes);
            //    }
            //    frontierNodes = nextFrontierNodes; // get the new frontier
            //    currdepth++;
            //}

            // back-propagation

            //while (!backToRoot)
            //{
            //    // backtrack the nodes using minimax algorithm until back to the root node
            //    List<Node> formerLevelNodes = new List<Node>();
            //    foreach (Node tempNode in frontierNodes)
            //    {
            //        // dealing with every node on the frontier
            //        // at the beginning, these nodes on the frontier are just the latest nodes we got from the forward propagation
            //        // which means: these nodes are the deepest nodes within the max depth
            //        tempNode.evalFunctionValue = tempNode.calcEvalFunctionValue();
            //        if (tempNode.parentNode.turn == players["Player1"])
            //        {
            //            // For player1, find the node of max value
            //            if (tempNode.evalFunctionValue > tempNode.parentNode.evalFunctionValue)
            //            {
            //                tempNode.parentNode.evalFunctionValue = tempNode.evalFunctionValue;
            //            }
            //        }
            //        if (tempNode.parentNode.turn == players["Player2"])
            //        {
            //            // For player2, find the node of min value
            //            if (tempNode.evalFunctionValue < tempNode.parentNode.evalFunctionValue)
            //            {
            //                tempNode.parentNode.evalFunctionValue = tempNode.evalFunctionValue;
            //            }
            //        }
            //        if (!formerLevelNodes.Contains(tempNode.parentNode))
            //        {
            //            // obtain the parent nodes of nodes we dealt with above
            //            // Since several "tempNode"s may have one parent node, we need to avoid adding repeated nodes
            //            formerLevelNodes.Add(tempNode.parentNode);
            //        }
            //    }
            //    currdepth--;
            //    turn = -turn;
            //    if (currdepth == 0)
            //    {
            //        // quit when reach the root
            //        backToRoot = true;

            //    }
            //    frontierNodes = formerLevelNodes; // update the frontier by parent nodes we obtained
            //}
        }


        // minimax function
        // Player1: offensive-minimax
        // Player2: deffensive-minimax
        //public Node minimax(int turn, List<Node> frontierNodes, Dictionary<string, int> players, int currdepth, bool backToRoot = false)
        //{
        //    List<Node> nextNode = new List<Node>();
        //    while (!backToRoot)
        //    {
        //        // backtrack the nodes using minimax algorithm until back to the root node
        //        List<Node> formerLevelNodes = new List<Node>();
        //        foreach (Node tempNode in frontierNodes)
        //        {
        //            // dealing with every node on the frontier
        //            // at the beginning, these nodes on the frontier are just the latest nodes we got from the forward propagation
        //            // which means: these nodes are the deepest nodes within the max depth
        //            tempNode.evalFunctionValue = tempNode.calcEvalFunctionValue(tempNode);
        //            if (tempNode.parentNode.turn == players["Player1"])
        //            {
        //                // For player1, find the node of max value
        //                if (tempNode.evalFunctionValue > tempNode.parentNode.evalFunctionValue)
        //                {
        //                    tempNode.parentNode.evalFunctionValue = tempNode.evalFunctionValue;
        //                }
        //            }
        //            if (tempNode.parentNode.turn == players["Player2"])
        //            {
        //                // For player2, find the node of min value
        //                if (tempNode.evalFunctionValue < tempNode.parentNode.evalFunctionValue)
        //                {
        //                    tempNode.parentNode.evalFunctionValue = tempNode.evalFunctionValue;
        //                }
        //            }
        //            if (!formerLevelNodes.Contains(tempNode.parentNode))
        //            {
        //                // obtain the parent nodes of nodes we dealt with above
        //                // Since several "tempNode"s may have one parent node, we need to avoid adding repeated nodes
        //                formerLevelNodes.Add(tempNode.parentNode);
        //            }
        //        }
        //        currdepth--;
        //        turn = -turn;
        //        if (currdepth == 0)
        //        {
        //            // quit when reach the root
        //            backToRoot = true;
        //            nextNode = nextNode.OrderByDescending(o => o.evalFunctionValue).ToList();
        //        }
        //        frontierNodes = formerLevelNodes; // update the frontier by parent nodes we obtained
        //    }
        //    return nextNode[0];
        //}

        static public Node alphabeta(Node rootNode, List<Node> bottomNodes, int maxDepth)
        {
            float v = 0;
            if (rootNode.turn == -1)
            {
                v = maxValueAB(rootNode, maxDepth, -10000, 10000);
            }
            else
            {
                v = minValueAB(rootNode, maxDepth, -10000, 10000);
            }
            return rootNode.findNodesOfValue(v, bottomNodes, maxDepth);
        }

        static public float maxValueAB(Node tempNode, int maxDepth, float a, float b)
        {
            float v = -10000;
            if (tempNode.depth == maxDepth)
            {
                return tempNode.evalFunctionValue;
            }
            if (tempNode.childNodes != null)
            {
                foreach (Node temptempNode in tempNode.childNodes)
                {
                    v = Math.Max(v, minValueAB(temptempNode, maxDepth, a, b));
                    if (v >= b)
                    {
                        return v;
                    }
                    a = Math.Max(a, v);
                }
                return v;
            }
            else
            {
                return 0;
            }
        }

        static public float minValueAB(Node tempNode, int maxDepth, float a, float b)
        {
            float v = 10000;
            if (tempNode.depth == maxDepth)
            {
                return tempNode.evalFunctionValue;
            }
            if (tempNode.childNodes != null)
            {
                foreach (Node temptempNode in tempNode.childNodes)
                {
                    v = Math.Min(v, maxValueAB(temptempNode, maxDepth, a, b));
                    if (v <= a)
                    {
                        return v;
                    }
                    b = Math.Min(b, v);
                }
                return v;
            }
            else
            {
                return v;
            }
        }


        static public Node minimax(Node rootNode, List<Node> bottomNodes, int maxDepth)
        {
            float v = 0;
            if (rootNode.turn == -1)
            {
                v = maxValue(rootNode, maxDepth);
            }
            else
            {
                v = minValue(rootNode, maxDepth);
            }
            return rootNode.findNodesOfValue(v, bottomNodes, maxDepth); 
        }

        static public float maxValue(Node tempNode, int maxDepth)
        {
            float v = -10000;
            if (tempNode.depth == maxDepth)
            {
                return tempNode.evalFunctionValue;
            }
            if (tempNode.childNodes != null)
            {
                foreach (Node temptempNode in tempNode.childNodes)
                {
                    v = Math.Max(v, minValue(temptempNode, maxDepth));
                }
                return v;
            }
            else
            {
                return 0;
            }
        }

        static public float minValue(Node tempNode, int maxDepth)
        {
            float v = 10000;
            if (tempNode.depth == maxDepth)
            {
                return tempNode.evalFunctionValue;
            }
            if (tempNode.childNodes != null)
            {
                foreach (Node temptempNode in tempNode.childNodes)
                {
                    v = Math.Min(v, maxValue(temptempNode, maxDepth));
                }
                return v;
            }
            else
            {
                return v;
            }
        }

        /********************************************OLD (HARD-CODE) VERSION********************************************/
        /*****************************************minimax(), max(), min() for Emma**************************************/
    //    static public Node minimax(Node rootNode)
    //    {
    //        if (rootNode.turn == 1)
    //        {
    //            Node minmaxminNode = new Node();
    //            if (rootNode.childNodes != null)
    //            {
    //                foreach (Node tempNode in rootNode.childNodes)
    //                {
    //                    tempNode.evalFunctionValue = maxValue(tempNode.childNodes);
    //                }
    //                if (rootNode.childNodes != null)
    //                {
    //                    foreach (Node tempNode in rootNode.childNodes)
    //                    {
    //                        if (tempNode.evalFunctionValue == minValue(rootNode.childNodes))
    //                        {
    //                            minmaxminNode = tempNode;
    //                            break;
    //                        }
    //                        else
    //                        {
    //                            continue;
    //                        }
    //                    }
    //                }
    //            }
    //            return minmaxminNode;
    //        }
    //        else
    //        {
    //            Node maxminmaxNode = new Node();
    //            if (rootNode.childNodes != null)
    //            {
    //                foreach (Node tempNode in rootNode.childNodes)
    //                {
    //                    tempNode.evalFunctionValue = minValue(tempNode.childNodes);
    //                    //Console.WriteLine(tempNode.evalFunctionValue);
    //                }
    //            }
    //            if (rootNode.childNodes != null)
    //            {
    //                foreach (Node tempNode in rootNode.childNodes)
    //                {
    //                    if (tempNode.evalFunctionValue == maxValue(rootNode.childNodes))
    //                    {
    //                        maxminmaxNode = tempNode;
    //                        //Console.WriteLine(maxminmaxNode.evalFunctionValue);
    //                        break;
    //                    }
    //                    else
    //                    {
    //                        continue;
    //                    }
    //                }

    //            }
    //            return maxminmaxNode;
    //        }
    //    }

    //    static public float maxValue(List<Node> Nodes)
    //    {
    //        // maximizer
    //        List<Node> sortList = new List<Node>();
    //        sortList = Nodes.OrderByDescending(o => o.evalFunctionValue).ToList();
    //        Node maxValueNode = sortList[0];
    //        return maxValueNode.evalFunctionValue;
    //    }

    //    static public float minValue(List<Node> Nodes)
    //    {
    //        // minimizer
    //        List<Node> sortList = new List<Node>();
    //        sortList = Nodes.OrderBy(o => o.evalFunctionValue).ToList();
    //        Node minValueNode = sortList[0];
    //        return minValueNode.evalFunctionValue;
    //    }
    }
}
