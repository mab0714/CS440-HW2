using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace WordSudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            // Word Sudoku: Initial Board Data
            string sudokuBoardFile = "";
            try
            { 
                sudokuBoardFile = args[0];
            }
            catch
            {
                sudokuBoardFile = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\2\\grid1.txt";
            }

            char[,] sudokuBoardData = new char[9,9];
            Dictionary<string, int> variablePriority = new Dictionary<string, int>();
            List<string> givenHints = new List<string>();

            int x = 0;
            int y = 8;

            string[] lines = System.IO.File.ReadAllLines(sudokuBoardFile);

            // Assign the contents of the file to the board
            foreach (string line in lines)
            {

                List<char> sublist = new List<char>();
                foreach (char c in line.ToCharArray())
                {
                    sudokuBoardData[x, y] = c;                    
                    if (!c.Equals('_'))
                    {
                        givenHints.Add(x + "_" + y + "_" + c);                        
                    }
                    else
                    {
                        // only add variables, aka "_"
                        variablePriority.Add(x + "_" + y, (8 - x + y));
                    }
                    x++;
                }
                x = 0;
                y--;

            }

            // Word Sudoku: Word Bank
            string sudokuWordBankFile = "";
            try
            {
                sudokuWordBankFile = args[0];
            }
            catch
            {
                sudokuWordBankFile = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\2\\bank1.txt";
            }

            lines = System.IO.File.ReadAllLines(sudokuWordBankFile);
            List<string> sudokuWordBankList = new List<string>();
            foreach (string line in lines)
            {
                sudokuWordBankList.Add(line.ToUpper());
            }

            // sort list (longest first, ties are sorted alphabetically
            sudokuWordBankList = sudokuWordBankList.OrderByDescending(aux => aux.Length).ToList();

            List<String> sudokuUsedWordList = new List<String>();
            List<String> sudokuAssignedWordList = new List<string>();

            // Dictionary of possible values
            //Dictionary<string, List<string>> possibleValuesDict = new Dictionary<string, List<string>>();

            // Calculate possible values that don't violate any constraints
            //possibleValuesDict = calcPossibleValues(sudokuBoardData, sudokuWordBankList, sudokuUsedWordList, givenHints);

            List<Node> visitedNodes = new List<Node>();
            int refreshDelayMS = 1;
            bool found = false;



            //Console.WriteLine(args[0]);
            Thread.Sleep(refreshDelayMS);
            Console.Clear();
            
            // Start State 
            Display(sudokuBoardData);

            //string algorithm = "";
            //int value = 0;
            //bool keepAsking = true;
            //while (keepAsking)
            //{
            //    Console.WriteLine("Navigating through: " + sudokuBoardData);

            //    Console.WriteLine("What algorithm do you want to run? (1-4)");
            //    Console.WriteLine("1 for DFS");
            //    Console.WriteLine("2 for BFS");
            //    Console.WriteLine("3 for Greedy");
            //    Console.WriteLine("4 for A*");
            //    algorithm = Console.ReadLine(); // Read string from console
            //    if (int.TryParse(algorithm, out value)) // Try to parse the string as an integer
            //    {
            //        if (value > 4)
            //        {
            //            Console.WriteLine("Please enter value between 1 and 4!");
            //            continue;
            //        }
            //        else
            //        {
            //            keepAsking = false;
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("Not an integer!");
            //    }

            //}
            //keepAsking = true;
            //int maxDepth = 0;
            //while (keepAsking)
            //{
            //    Console.WriteLine("What depth do you want to run (integer)");
            //    algorithm = Console.ReadLine(); // Read string from console
            //    if (int.TryParse(algorithm, out maxDepth)) // Try to parse the string as an integer
            //    {
            //        keepAsking = false;
            //    }
            //    else
            //    {
            //        Console.WriteLine("Not an integer!");
            //    }

            //}


            ////Random rand = new Random();
            // Log start of search
            DateTime start = DateTime.Now;

            Node startStateNode = new Node(sudokuBoardData, sudokuWordBankList, sudokuUsedWordList, givenHints, 0, 0, variablePriority, null);
            Node currentNode = startStateNode;

            startStateNode.showNodeInfo();
            //startStateNode.findEligibleAssignments();

            //foreach (Node n in startStateNode.childNodes)
            //{
            //    n.showNodeInfo();
            //}
            // DFS

            List<Node> pathToGoalState = new List<Node>();
            List<Node> otherChildNodes = new List<Node>();

            found = findDFSBackTrackingPath(currentNode, visitedNodes, pathToGoalState, otherChildNodes);
            
            // Log end of search
            DateTime end = DateTime.Now;

            if (found)
            {

                Console.WriteLine("****************");
                Console.WriteLine("Summary: ");
                Console.WriteLine("Search Started: " + start);
                Console.WriteLine("Search Ended: " + end);
                Console.WriteLine("Duration: " + (end - start));
                Console.WriteLine("Nodes visited: " + visitedNodes.Count());
                Console.WriteLine("****************");

            }
            else
            {
                Console.WriteLine("****************");
                Console.WriteLine("Summary: ");
                Console.WriteLine("Search Started: " + start);
                Console.WriteLine("Search Ended: " + end);
                Console.WriteLine("Duration: " + (end - start));
                Console.WriteLine("Nodes visited: " + visitedNodes.Count());
                Console.WriteLine("****************");
            }

            Console.WriteLine("Press anykey to quit");
            Console.ReadKey();

        }

        static bool findDFSBackTrackingPath(Node currentNode,  List<Node> visitedNodes, List<Node> finalPathOfNodes, List<Node> otherChildNodes)
        {                        
            // In case of backtracking, no need to add a revisited node 
            // Perhaps a wall was hit, and backtracking is necessary.  No need to add the
            // revisited node again.
            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);
            }

            Console.Clear();
            Display(currentNode.SudokuBoardData);

            Node nextNode = new Node(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode);

            if (AssignmentComplete(currentNode.SudokuBoardData))
            {
                finalPathOfNodes.Clear();
                finalPathOfNodes.Add(currentNode);

                while (currentNode.parentNode != null)
                {

                    nextNode = currentNode.parentNode;
                    finalPathOfNodes.Add(nextNode);
                    currentNode = nextNode;
                }

                Console.Clear();
                Display(currentNode.SudokuBoardData);

                return true;
            }
            else
            {                
                currentNode.showNodeInfo();

                // Make assignment
                try
                {
                    // Choose most constraining variable
                    var keysWithMatchingValues = currentNode.CurrentVariablePriority.Where(p => p.Value == currentNode.CurrentVariablePriority.Values.Max()).Select(p => p.Key);

                    int VariableX = Int32.Parse(keysWithMatchingValues.ToList()[0].Split('_')[0]);
                    int VariableY = Int32.Parse(keysWithMatchingValues.ToList()[0].Split('_')[1]);

                    currentNode.x = VariableX;
                    currentNode.y = VariableY;

                    string chosenDirection = "";
                    string chosenWord = "";
                    int offset = 0;
                    // try to choose the best value
                    bool isAssignmentSafe = false;
                    while (!isAssignmentSafe)
                    {
                        // try the first one (this is ordered by file length)
                        currentNode.PossibleValuesDict = Node.calcPossibleValues(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints);
                        chosenDirection = currentNode.PossibleValuesDict[currentNode.x + "_" + currentNode.y][0].Split('_')[0].ToString().ToUpper();
                        chosenWord = currentNode.PossibleValuesDict[currentNode.x + "_" + currentNode.y][0].Split('_')[1].ToString().ToUpper();
                        offset = Int32.Parse(currentNode.PossibleValuesDict[currentNode.x + "_" + currentNode.y][0].Split('_')[2].ToString());

                        // Does this assignment cause inconsistency?
                        // Check if any value in the dictionary has no choices
                        // Linq Min
                        List<string> newArcWordList = new List<string>(currentNode.SudokuWordBankList);                        
                        List<string> newArcUsedWordList = new List<string>(currentNode.SudokuUsedWordList);

                        char[,] newArcBoard = new char[9,9];
                        for (int j=0; j<9; j++)
                        {
                            for (int k = 0; k < 9; k++)
                            {
                                newArcBoard[j, k] = currentNode.SudokuBoardData[j, k];
                            }
                        }

                        Node ArcCheckNode = new Node(newArcBoard, newArcWordList.ToList<string>(), newArcUsedWordList.ToList<string>(), currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode.parentNode);

                        currentNode.showNodeInfo();
                        ArcCheckNode.showNodeInfo();

                        if (chosenDirection.Equals("H"))
                        {
                            ArcCheckNode.SudokuBoardData = ArcCheckNode.updateBoard(ArcCheckNode.SudokuBoardData, chosenDirection, ArcCheckNode.x - offset, ArcCheckNode.y, chosenWord, ArcCheckNode.CurrentVariablePriority);
                        }
                        else
                        {
                            ArcCheckNode.SudokuBoardData = ArcCheckNode.updateBoard(ArcCheckNode.SudokuBoardData, chosenDirection, ArcCheckNode.x, ArcCheckNode.y - offset, chosenWord, ArcCheckNode.CurrentVariablePriority);
                        }

                        ArcCheckNode.SudokuUsedWordList.Add(chosenWord);
                        ArcCheckNode.SudokuWordBankList.Remove(chosenWord);                        
                        ArcCheckNode.PossibleValuesDict = Node.calcPossibleValues(ArcCheckNode.SudokuBoardData, ArcCheckNode.SudokuWordBankList, ArcCheckNode.SudokuUsedWordList, ArcCheckNode.GivenHints);
                        isAssignmentSafe = true;
                        // Loop all variables and check if locations left with '_' have options
                        for (int y = 8; y > 0; y--)
                        {               
                            for (int x = 0; x < 9; x++)
                            {
                                if (ArcCheckNode.SudokuBoardData[x, y] == '_')
                                {
                                    if (ArcCheckNode.PossibleValuesDict[x + "_" + y].Count == 0)
                                    {
                                        isAssignmentSafe = false;
                                        currentNode.PossibleValuesDict[currentNode.x + "_" + currentNode.y].Remove(currentNode.PossibleValuesDict[currentNode.x + "_" + currentNode.y][0].Split('_')[0]);
                                        break;
                                    }
                                };
                            }                                                               
                        }
                        
                    }
                    // Assignment is safe

                    if (chosenDirection.Equals("H"))
                    {
                        currentNode.SudokuBoardData = currentNode.updateBoard(currentNode.SudokuBoardData, chosenDirection, currentNode.x - offset, currentNode.y, chosenWord, currentNode.CurrentVariablePriority);
                        currentNode.Assignment = chosenWord;
                        currentNode.SudokuUsedWordList.Add(chosenWord);
                        currentNode.SudokuWordBankList.Remove(chosenWord);
                    }
                    else
                    {
                        currentNode.SudokuBoardData = currentNode.updateBoard(currentNode.SudokuBoardData, chosenDirection, currentNode.x, currentNode.y - offset, chosenWord, currentNode.CurrentVariablePriority);
                        currentNode.Assignment = chosenWord;
                        currentNode.SudokuUsedWordList.Add(chosenWord);
                        currentNode.SudokuWordBankList.Remove(chosenWord);
                    }
                }
                catch
                {
                    // maybe there is no more possible words
                    return false;

                }

                if (currentNode.childNodes == null)
                {
                    currentNode.childNodes = currentNode.findEligibleAssignments();
                }


                if (currentNode.childNodes != null && currentNode.childNodes.Count > 0)
                {

                    //// Mark childNodes as being already a child to some other parent.
                    //foreach (Node n in currentNode.childNodes)
                    //{
                    //    if (!otherChildNodes.Contains(n))
                    //    {
                    //        otherChildNodes.AddRange(currentNode.childNodes);
                    //    }
                    //}

                    //// Remove visited childNodes as repeatable options.
                    //foreach (Node n in visitedNodes)
                    //{
                    //    if (currentNode.childNodes.Contains(n))
                    //    {
                    //        currentNode.childNodes.Remove(n);
                    //    }
                    //}
                    // Any unvisited children should be visited next

                    // Choose currentNode variable (if it's best)
                    int mostConstrainingValue = 0;
                    var keysWithMatchingValues = currentNode.CurrentVariablePriority.Where(p => p.Value == currentNode.CurrentVariablePriority.Values.Max()).Select(p => p.Key);
                    Node tmpNode = new Node(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode);

                    if (currentNode.childNodes.Count > 0)
                    {
                        // Choose the nextNode
                        // choose most constraining variable 
                        foreach(Node child in currentNode.childNodes)
                        {
                            child.findEligibleAssignments();
                            if (child.CurrentVariablePriority.Values.Max() > mostConstrainingValue)
                            {
                                mostConstrainingValue = child.CurrentVariablePriority.Values.Max();
                                keysWithMatchingValues = child.CurrentVariablePriority.Where(p => p.Value == mostConstrainingValue).Select(p => p.Key);                                
                                tmpNode = child;
                            }
                            else if (child.CurrentVariablePriority.Values.Max() == mostConstrainingValue)
                            {
                                // Tie breaker, which has the fewest legal moves
                                // childNodes indicate the legal moves
                                int result = 0;
                                try
                                {
                                    Int32.TryParse(child.childNodes.Count().ToString(), out result);
                                    if (result < tmpNode.childNodes.Count)
                                    {
                                        mostConstrainingValue = child.CurrentVariablePriority.Values.Max();
                                        keysWithMatchingValues = child.CurrentVariablePriority.Where(p => p.Value == mostConstrainingValue).Select(p => p.Key);
                                        tmpNode = child;
                                    }
                                }
                                catch
                                {
                                    // no more legal moves
                                    // don't choose this value
                                    ;
                                }                                
                            }                            
                        }
                        
                        int nextVariableX = Int32.Parse(keysWithMatchingValues.ToList()[0].Split('_')[0]);
                        int nextVariableY = Int32.Parse(keysWithMatchingValues.ToList()[0].Split('_')[1]);
                        tmpNode.x = nextVariableX;
                        tmpNode.y = nextVariableY;

                        nextNode = tmpNode;
                        if (findDFSBackTrackingPath(nextNode, visitedNodes, finalPathOfNodes, otherChildNodes))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // If all childNodes are visited, then go back to parentNode.
                        // If parentNode is null, perhaps there is no goalState
                        if (currentNode.parentNode != null)
                        {
                            nextNode = currentNode.parentNode;
                            if (findDFSBackTrackingPath(nextNode, visitedNodes, finalPathOfNodes, otherChildNodes))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (currentNode.parentNode != null)
                    {
                        nextNode = currentNode.parentNode;
                        if (findDFSBackTrackingPath(nextNode, visitedNodes, finalPathOfNodes, otherChildNodes))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

            }

            return false;

        }


        //static bool findDFSPath(Node currentNode, Node goalStateNode, char[,] sudokuBoardData, List<Node> visitedNodes, List<Node> finalPathOfNodes, Random rand, List<Node> otherChildNodes, int refreshDelayMS)
        //{
        //    // In case of backtracking, no need to add a revisited node 
        //    // Perhaps a wall was hit, and backtracking is necessary.  No need to add the
        //    // revisited node again.
        //    if (!visitedNodes.Contains(currentNode))
        //    {
        //        visitedNodes.Add(currentNode);
        //    }

        //    Thread.Sleep(refreshDelayMS);
        //    Console.Clear();
        //    Display(sudokuBoardData);

        //    Node nextNode = new Node(0, 0, null);
        //    if (currentNode.Equals(goalStateNode))
        //    {
        //        finalPathOfNodes.Clear();
        //        finalPathOfNodes.Add(currentNode);

        //        while (currentNode.parentNode != null)
        //        {

        //            nextNode = currentNode.parentNode;
        //            finalPathOfNodes.Add(nextNode);
        //            currentNode = nextNode;
        //        }


        //        Thread.Sleep(refreshDelayMS);
        //        Console.Clear();
        //        Display(sudokuBoardData);

        //        return true;
        //    }
        //    else
        //    {
        //        if (currentNode.childNodes == null)
        //        {
        //            currentNode.childNodes = currentNode.findEligibleChildren(sudokuBoardData, otherChildNodes);
        //        }

        //        currentNode.showNodeInfo();

        //        int randNumber = 0;
        //        if (currentNode.childNodes != null && currentNode.childNodes.Count > 0)
        //        {

        //            // Mark childNodes as being already a child to some other parent.
        //            foreach (Node n in currentNode.childNodes)
        //            {
        //                if (!otherChildNodes.Contains(n))
        //                {
        //                    otherChildNodes.AddRange(currentNode.childNodes);
        //                }
        //            }

        //            // Remove visited childNodes as repeatable options.
        //            foreach (Node n in visitedNodes)
        //            {
        //                if (currentNode.childNodes.Contains(n))
        //                {
        //                    currentNode.childNodes.Remove(n);
        //                }
        //            }
        //            // Any unvisited children should be visited next
        //            if (currentNode.childNodes.Count > 0)
        //            {
        //                randNumber = rand.Next(0, currentNode.childNodes.Count - 1);

        //                nextNode = currentNode.childNodes[randNumber];
        //                if (findDFSPath(nextNode, goalStateNode, sudokuBoardData, visitedNodes, finalPathOfNodes, rand, otherChildNodes, refreshDelayMS))
        //                {
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                // If all childNodes are visited, then go back to parentNode.
        //                // If parentNode is null, perhaps there is no goalState
        //                if (currentNode.parentNode != null)
        //                {
        //                    nextNode = currentNode.parentNode;
        //                    if (findDFSPath(nextNode, goalStateNode, sudokuBoardData, visitedNodes, finalPathOfNodes, rand, otherChildNodes, refreshDelayMS))
        //                    {
        //                        return true;
        //                    }
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }

        //            //foreach (Node n in childNodes)
        //            //{
        //            //}
        //            //   n.showNodeInfo();
        //        }
        //        else
        //        {
        //            if (currentNode.parentNode != null)
        //            {
        //                nextNode = currentNode.parentNode;
        //                if (findDFSPath(nextNode, goalStateNode, sudokuBoardData, visitedNodes, finalPathOfNodes, rand, otherChildNodes, refreshDelayMS))
        //                {
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }

        //    }

        //    return false;

        //}

        //static Dictionary<string, List<string>> calcPossibleValues (char[,] tmpboard, List<string> wordList, List<string> usedWordList, List<string> givenHints)
        //{
        //    char[,] board = (char[,])tmpboard.Clone();
        //    Dictionary<string, List<string>> newDict = new Dictionary<string, List<string>>();
        //    // Loop through the board to get the variables
        //    for (int y = 8; y >= 0; y--)
        //    {
        //        for (int x = 0; x < 9; x++)
        //        {
        //            //if (board[x, y].Equals('_'))
        //            //{
        //                List<string> tmpList = getPossibleWords(board, x, y, wordList, usedWordList, givenHints);
        //                newDict.Add(x + "_" + y, tmpList);
        //            //}
        //        }
        //        Console.WriteLine();
        //    }

        //    return newDict;
        //}

        //static List<string> getPossibleWords(char[,] tmpboard, int x, int y, List<string> wordList, List<string> usedWordList, List<string> givenHints)
        //{
        //    char[,] board = (char[,])tmpboard.Clone();
        //    List<string> tmpList = new List<string>();
        //    int tmpX = x;
        //    // bool constraints that must be satisfied
        //    bool uniqueInRow = false;
        //    bool uniqueInCol = false;
        //    bool uniqueInCell = false; //3x3 cell
        //    bool satisfyRowLength = false;
        //    bool satisfyColLength = false;
        //    bool safeHints = false;

        //    foreach (string line in wordList)
        //    {
        //        satisfyRowLength = isSatisfyRowLength(x, line);
        //        satisfyColLength = isSatisfyColLength(y, line);
        //        // if it is NOT used yet
        //        if (!usedWordList.Contains(line))
        //        {
        //            // word too long for remaining x positions
        //            if (satisfyRowLength)
        //            {
        //                // Qualifies horizontally
        //                uniqueInCol = isUniqueInCol(board, x, y, line);
        //                uniqueInCell = isUniqueInCell(board, x, y, line, 'h');
        //                safeHints = isViolateHints(board, x, y, line, givenHints, 'h');

        //                if (uniqueInCol && uniqueInCell && safeHints)
        //                {
        //                    tmpList.Add("H_" + line);
        //                }
        //            }




        //            // word too long for remaining y positions
        //            if (satisfyColLength)
        //            {
        //                // Qualifies vertically                        
        //                uniqueInRow = isUniqueInRow(board, x, y, line);
        //                uniqueInCell = isUniqueInCell(board, x, y, line, 'v');
        //                safeHints = isViolateHints(board, x, y, line, givenHints, 'v');
        //                if (uniqueInRow && uniqueInCell && safeHints)
        //                {
        //                    tmpList.Add("V_" + line);
        //                }
        //            }




        //        }
        //    }


        //    return tmpList;
        //}

        //static bool isViolateHints (char[,] tmpboard, int x, int y, string word, List<string>givenHints, char direction)
        //{
        //    char[,] board = (char[,])tmpboard.Clone();

        //    // populate board
        //    if (direction.ToString().ToUpper().Equals("H"))
        //    {
        //        // update board
        //        for (int tmpX = x; tmpX < x+word.Length; tmpX++)
        //        {
        //            board[tmpX, y] = word[tmpX - x];
        //        }

        //    }
        //    else
        //    {
        //        int revY = 0;
        //        for (int tmpY = y; tmpY > y-word.Length; tmpY--)
        //        {
        //            board[x, tmpY] = word[revY];
        //            revY++;
        //        }
        //    }

        //    foreach (string hint in givenHints)
        //    {
        //        // check violations within the board
        //        // hint is in for X_Y_<char>
        //        int hintX = Int32.Parse(hint.Split('_')[0].ToString());
        //        int hintY = Int32.Parse(hint.Split('_')[1].ToString());
        //        char c = char.Parse(hint.Split('_')[2].ToString());

        //        // if board doesn't match any hint
        //        if (!board[hintX, hintY].Equals(c))
        //        {
        //            return false;
        //        }

        //    }

        //    return true;
        //}
        //static bool isSatisfyRowLength (int x, string word) 
        //{
        //    if (word.Length <= (9-x))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //static bool isSatisfyColLength(int y, string word)
        //{
        //    if (word.Length <= (y + 1))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //static bool isUniqueInRow (char[,] tmpboard, int x, int y, string word)
        //{
        //    char[,] board = (char[,]) tmpboard.Clone();

        //    // build a string consisting of all the letters in the row
        //    string line = "";
        //    // Y is in reverse due to orientation, so this reverts it back 
        //    int revY = 0;
        //    //EX: We are assigning the word CATS in a 9x9 table at y position 1, y doesn't matter since we check all x, 
        //    //    we want to investigate the uniqueness in the rows marked in with X
        //    //    [_________]
        //    //    [X________]
        //    //    [X________]
        //    //    [X________]
        //    //    [X________]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]

        //    // start at the proper y, and continue until the we exceed the length of the word
        //    for (int tmpY = y; tmpY > y-word.Length; tmpY--)
        //    {
        //        board[x, tmpY] = word[revY];
        //        // loop through all the columnns and build a string
        //        for (int tmpX = 0; tmpX < 9; tmpX++)
        //        {                    
        //            line = line + board[tmpX, tmpY];
        //        }
        //        revY++;
        //        // check for uniqueness in the string by filtering out distinct values of the linue using Linq
        //        // if the string returns to anything less than 9, then there was some duplication
        //        // and there is a violation of distinct letters in a row
        //        // remove '_'
        //        string tmpLine = "";
        //        if (line.Contains("_"))
        //        {
        //            tmpLine = line.Replace("_", "");
        //        }

        //        int tempLineLength = tmpLine.Length;
        //        if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
        //        {
        //            return false;
        //        }
        //        line = "";

        //    }
        //    // if we checked all columns without being false, we return true
        //    return true;
        //}

        //static bool isUniqueInCol(char[,] tmpboard, int x, int y,  string word)
        //{
        //    char[,] board = (char[,])tmpboard.Clone();
        //    // build a string consisting of all the letters in the column
        //    string line = "";

        //    //EX: We are assigning the word CATS in a 9x9 table at x position 1, y doesn't matter since we check all y, 
        //    //    we want to investigate the uniqueness in the columns marked in with X
        //    //    [_XXXX____]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]
        //    //    [_________]

        //    // start at the proper x, and continue until the we exceed the length of the word or end of the board
        //    int wordX = 0;
        //    for (int tmpX = x; tmpX < x + word.Length - 1; tmpX++)
        //    {
        //        board[tmpX, y] = word[wordX];
        //        wordX++;
        //        // loop through all the columnns and build a string
        //        for (int tmpY = 8; tmpY >= 0; tmpY--)
        //        {                    
        //            line = line + board[tmpX, tmpY];                    
        //        }
        //        // check for uniqueness in the string by filtering out distinct values of the linue using Linq
        //        // if the string returns to anything less than 9, then there was some duplication
        //        // and there is a violation of distinct letters in a row
        //        // remove '_'
        //        string tmpLine = "";
        //        if (line.Contains("_"))
        //        {
        //            tmpLine = line.Replace("_", "");
        //        }

        //        int tempLineLength = tmpLine.Length;
        //        if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
        //        {
        //            return false;
        //        }
        //        line = "";

        //    }
        //    // if we checked all columns without being false, we return true
        //    return true;            
        //}

        //static bool isUniqueInCell(char[,] tmpboard, int x, int y, string word, char direction)
        //{
        //    char[,] board = (char[,])tmpboard.Clone();
        //    // build a string consisting of all the letters in the cell
        //    string line = "";
        //    if (direction.ToString().ToUpper().Equals("H"))
        //    {
        //        // populate our board horizontally
        //        for (int w = x; w < word.Length; w++)
        //        {
        //            board[w, y] = word[w];
        //        }

        //        // check all variables impacted horizontally
        //        for (int tmpX = x; tmpX < x + word.Length; tmpX++)
        //        {
        //            // Consider X coodordinate range only til 9
        //            // EX: 0 1 2 3 4 5 6 7 8 
        //            // We investigate x = 2
        //            // If divide by 3 (3 spots per cell), we can calculate cell 1-3, then add one, so we aren't dealing with cell 0-2, but cell 1-3 instead
        //            int cellX = (int)Math.Floor((tmpX / 3.0)) + 1;

        //            // Now, since we don't have a 0, we can multiply by 3 to get our upper bound, then subract 1
        //            int maxX = cellX * 3 - 1;

        //            // Finally, we can subtract 2 to get the lower bound
        //            int minX = maxX - 2;

        //            // Same logic as above
        //            int cellY = (int)Math.Floor((y / 3.0)) + 1;
        //            int maxY = cellY * 3 - 1;
        //            int minY = maxY - 2;


        //            // loop through all the cell and build a string
        //            for (int tmpY = minY; tmpY <= maxY; tmpY++)
        //            {
        //                for (int tmpX2 = minX; tmpX2 <= maxX; tmpX2++)
        //                {                            
        //                    line = line + board[tmpX2, tmpY];
        //                }
        //            }

        //            // check for uniqueness in the string by filtering out distinct values of the linue using Linq
        //            // if the string returns to anything less than 9, then there was some duplication
        //            // and there is a violation of distinct letters in a row               
        //            // remove '_'
        //            string tmpLine = "";
        //            if (line.Contains("_"))
        //            {                        
        //                tmpLine = line.Replace("_", "");
        //            }

        //            int tempLineLength = tmpLine.Length;                    
        //            if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
        //            {
        //                return false;
        //            }
        //            line = "";
        //        }
        //    }
        //    else
        //    {
        //        // populate our board vertically
        //        int revY = 0;
        //        for (int w = y; w >= y + 1 - word.Length; w--)
        //        {
        //            board[x, w] = word[revY];
        //            revY++;
        //        }

        //        // check all variables impacted vertically
        //        for (int tmpY = y; tmpY > y + 1 - word.Length; tmpY--)
        //        {
        //            // Consider X coodordinate range only til 9
        //            // EX: 0 1 2 3 4 5 6 7 8 
        //            // We investigate x = 2
        //            // If divide by 3 (3 spots per cell), we can calculate cell 1-3, then add one, so we aren't dealing with cell 0-2, but cell 1-3 instead
        //            int cellY = (int)Math.Floor((tmpY / 3.0)) + 1;

        //            // Now, since we don't have a 0, we can multiply by 3 to get our upper bound, then subract 1
        //            int maxY = cellY * 3 - 1;

        //            // Finally, we can subtract 2 to get the lower bound
        //            int minY = maxY - 2;

        //            // Same logic as above
        //            int cellX = (int)Math.Floor((x / 3.0)) + 1;
        //            int maxX = cellX * 3 - 1;
        //            int minX = maxX - 2;


        //            // loop through all the cell and build a string
        //            for (int tmpY2 = minY; tmpY2 <= maxY; tmpY2++)
        //            {
        //                for (int tmpX = minX; tmpX <= maxX; tmpX++)
        //                {
        //                    line = line + board[tmpX, tmpY2];
        //                }
        //            }

        //            // check for uniqueness in the string by filtering out distinct values of the linue using Linq
        //            // if the string returns to anything less than 9, then there was some duplication
        //            // and there is a violation of distinct letters in a row               
        //            // remove '_'
        //            string tmpLine = "";
        //            if (line.Contains("_"))
        //            {
        //                tmpLine = line.Replace("_", "");
        //            }

        //            int tempLineLength = tmpLine.Length;
        //            if (tmpLine.ToCharArray().Distinct().ToArray().Length < tempLineLength)
        //            {
        //                return false;
        //            }
        //            line = "";
        //        }
        //    }

        //    return true;
        //}        

        static bool AssignmentComplete(char[,] board)
        {
            // Find any '_' to indicate incomplete assigments

            for (int y = 8; y > 0; y--)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (board[x, y] == '_')
                    {
                        return false;
                    };
                }
                
            }

            return true;
        }

        static void Display(char[,] board)
        {
            //
            // Display everything in the List.
            //
            Console.WriteLine("Word Sudoku Board:");

            for (int y = 8; y > 0; y--)
            {
                for (int x = 0; x < 9; x++)
                {
                    Console.Write(board[x,y]);
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
