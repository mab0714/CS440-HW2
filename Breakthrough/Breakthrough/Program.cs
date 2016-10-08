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
            string breakthroughBoardFile = "";
            breakthroughBoardFile = "D:\\UIUC\\ECE448-Artificial_Intelligence\\HW2\\Projects\\Breakthrough\\Breakthrough\\borad.txt";
            char[,] breakthroughBoardData = new char[8, 8];
            Dictionary<bool, char> roles = new Dictionary<bool, char>();
            List<Node> units = new List<Node>();
            List<Node> remainingWorkers = new List<Node>();

            int x = 0;
            int y = 0;

            string[] lines = System.IO.File.ReadAllLines(breakthroughBoardFile);

            // Assign the contents of the file to the board
            foreach (string line in lines)
            {
                List<char> sublist = new List<char>();
                foreach (char c in line.ToCharArray())
                {
                    breakthroughBoardData[x, y] = c;

                    if (!c.Equals('_'))
                    {
                        remainingWorkers.Add(new Node(x, y, c));
                    }
                    x++;
                }
                x = 0;
                y++;
            }

            List<Node> nextOptions = new List<Node>();
            Node startNode = new Node(1, 1, 'o');
            Node currentNode = startNode;
            List<Node> finalPathOfNodes = new List<Node>(); // propagate the process of game
            char role = ' ';
            roles.Add(true, 'o');
            roles.Add(false, 'x');

            bool status = false;
            int maxDepth = 9;  // depth limited
            int tempDepth = 0;
            bool roleIndex = true;
            nextOptions.Add(currentNode);
            while (!status) // move one worker step by step
            {
                if (findEligibleChilderen)
                {
                    status = findNextMove(nextOptions, breakthroughBoardData, finalPathOfNodes, maxDepth);
                    tempDepth++;
                    role = roles[!roleIndex]; // move "o" and "x" in turns
                }
            }
            
        }



        public bool findNextMove(List<Node> nextOptions, char [,] boardData, List<Node> finalPathOfNodes, char role, int tempDepth)
        {
            // implement each move of each worker
            Node currentNode = new Node(0, 0, 0, null);
            Node nextNode = new Node(0, 0, 0, null);

            if (role == 'o')
            {
                nextOptions.Add(miniValue(currentNode));
            }

            if (role == 'x')
            {
                nextOptions.Add(MaxValue(currentNode));
            }

            if (currentNode._depth > tempDepth)
            {
                return false;
            }

        }
    }
}