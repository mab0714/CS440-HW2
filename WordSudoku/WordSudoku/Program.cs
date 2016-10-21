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
            //Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            string sudokuBoardFile = "";
            try
            { 
                sudokuBoardFile = args[0];
            }
            catch
            {
                sudokuBoardFile = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\2\\grid2.txt";
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
                sudokuWordBankFile = "I:\\Backup\\Masters\\UIUC\\2016\\Fall\\CS_440\\Homework\\2\\bank2.txt";
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

            List<Node> visitedNodes = new List<Node>();
            int refreshDelayMS = 1;
            bool found = false;

            //Console.WriteLine(args[0]);
            Thread.Sleep(refreshDelayMS);
            Console.Clear();
            
            // Start State 
            Display(sudokuBoardData, givenHints);

            string algorithm = "";
            int value = 0;
            bool keepAsking = true;
            while (keepAsking)
            {
                Console.WriteLine("Any pruning for DFS? (1-2)");
                Console.WriteLine("1 for Brute Force");
                Console.WriteLine("2 for Forward Checking");                
                algorithm = Console.ReadLine(); // Read string from console
                if (int.TryParse(algorithm, out value)) // Try to parse the string as an integer
                {
                    if (value != 1 && value != 2)
                    {
                        Console.WriteLine("Please enter value between 1 and 2!");
                        continue;
                    }
                    else
                    {
                        keepAsking = false;
                    }
                }
                else
                {
                    Console.WriteLine("Not an integer!");
                }

            }


            // Log start of search
            DateTime start = DateTime.Now;

            Node startStateNode = new Node(sudokuBoardData, sudokuWordBankList, sudokuUsedWordList, givenHints, 0, 0, variablePriority, null);
            Node currentNode = startStateNode;
            currentNode.PossibleValuesDict = Node.calcPossibleSpots(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints);

            List<Node> pathToGoalState = new List<Node>();
            List<Node> otherChildNodes = new List<Node>();
            int pruningStrategy = value;

            //Dictionary<List<Node>, int> allSolutions = new Dictionary<List<Node>, int>();

            //List<string> tmpSudokuWordBankList = new List<string>(sudokuWordBankList);

            //for (int i = 0; i < sudokuWordBankList.Count; i++)                                 
            //{
            //    currentNode.SudokuWordBankList = tmpSudokuWordBankList;
            //    currentNode.PossibleValuesDict = Node.calcPossibleSpots(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints);
            //    found = findDFSBackTrackingPath(currentNode, visitedNodes, pathToGoalState, otherChildNodes, pruningStrategy);

            //    if (found)
            //    {
            //        allSolutions.Add(pathToGoalState, pathToGoalState.Count());
            //    }
            //    tmpSudokuWordBankList = new List<string>(sudokuWordBankList);
            //    tmpSudokuWordBankList.Remove(tmpSudokuWordBankList[i]);                

            //}

   
            //found = findDFSBackTrackingPath(currentNode, visitedNodes, pathToGoalState, otherChildNodes, pruningStrategy);
            found = findDFSBackTrackingPathARC(currentNode, visitedNodes, pathToGoalState, otherChildNodes, pruningStrategy);            

            // Log end of search
            DateTime end = DateTime.Now;
            List<string> orderOfAssignment = new List<string>();
            if (found)
            {
                Console.Clear();
                Console.WriteLine("****************");
                Console.WriteLine("SOLUTION FOUND!");
                Console.WriteLine("Summary: ");
                Console.WriteLine(" Board: " + sudokuBoardFile);
                Console.WriteLine(" WordBank: " + sudokuWordBankFile);
                Console.Write(" Pruning Strategy: ");
                if (pruningStrategy == 1)
                {
                    Console.WriteLine("Brute Force");
                }
                else if (pruningStrategy == 2)
                {
                    Console.WriteLine("Forward Checking");
                }
                //else if (pruningStrategy == 2)
                //{
                //    Console.WriteLine("Arc Consistency");
                //}

                Console.WriteLine(" Search Started: " + start);
                Console.WriteLine(" Search Ended: " + end);
                Console.WriteLine(" Duration: " + (end - start));
                Console.WriteLine(" Nodes visited: " + visitedNodes.Count());
                // Display the finalPath backwards
                pathToGoalState.Reverse();
                foreach (Node n in pathToGoalState)
                {
                    n.showNodeInfo();
                    if (n.Assignment != null)
                    {
                        orderOfAssignment.Add(n.Assignment);
                    }
                }
                Console.WriteLine("Assignment: ");
                foreach (string assignment in orderOfAssignment)
                {
                    Console.WriteLine(assignment);
                }
                Console.WriteLine("****************");

            }
            else
            {
                Console.Clear();
                Console.WriteLine("****************");
                Console.WriteLine("NO SOLUTION FOUND!");
                Console.WriteLine("Summary: ");
                Console.WriteLine(" Board: " + sudokuBoardFile);
                Console.WriteLine(" WordBank: " + sudokuWordBankFile);
                Console.Write(" Pruning Strategy: ");
                if (pruningStrategy == 1)
                {
                    Console.WriteLine("Brute Force");
                }
                else if (pruningStrategy == 2)
                {
                    Console.WriteLine("Forward Checking");
                }
                //else if (pruningStrategy == 3)
                //{
                //    Console.WriteLine("Arc Consistency");
                //}                

                Console.WriteLine(" Search Started: " + start);
                Console.WriteLine(" Search Ended: " + end);
                Console.WriteLine(" Duration: " + (end - start));
                Console.WriteLine(" Nodes visited: " + visitedNodes.Count());
                Console.WriteLine("****************");
            }

        do
            {
                Console.WriteLine("Press q to quit");
            } while (Console.ReadKey().KeyChar != 'q');

        }

        static bool findDFSBackTrackingPath(Node currentNode,  List<Node> visitedNodes, List<Node> finalPathOfNodes, List<Node> otherChildNodes, int pruning)
        {                        

            // In case of backtracking, no need to add a revisited node 
            // Perhaps a wall was hit, and backtracking is necessary.  No need to add the
            // revisited node again.
            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);
            }

            Console.Clear();
            Display(currentNode.SudokuBoardData, currentNode.GivenHints);

            Node nextNode = new Node(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode);

            if (AssignmentComplete(currentNode.SudokuBoardData)) // || currentNode.SudokuWordBankList.Count == 0)
            {
                //if (currentNode.SudokuWordBankList.Count == 0)
                //{
                //    return false;
                //}
                finalPathOfNodes.Clear();
                finalPathOfNodes.Add(currentNode);

                while (currentNode.parentNode != null)
                {
                    nextNode = currentNode.parentNode;
                    currentNode = nextNode;
                    finalPathOfNodes.Add(nextNode);                        
                }

                Console.Clear();
                Display(currentNode.SudokuBoardData, currentNode.GivenHints);

                return true;
            }
            else
            {                

                // Make assignment
                try
                {

                    currentNode.showNodeInfo();

                    //string chosenDirection = "";
                    //string chosenWord = "";
                    //int offset = 0;

                    // try to choose the best variable
                    // Minimum remaining values dictate which is the most constrained variables.          
                    if (currentNode.PossibleValuesDict == null)
                    {
                        currentNode.PossibleValuesDict = Node.calcPossibleSpots(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints);
                    }

                    // Choose word with least amount of available spots (most constrained), if there is a tie?                    
                    Dictionary<string, int> dictOfMostConstrainingVariables = new Dictionary<string, int>();
                    Dictionary<string, int> sortedDictOfMostConstrainingVariables = new Dictionary<string, int>();

                    foreach (KeyValuePair<string, List<String>> variable in currentNode.PossibleValuesDict)
                    {
                        dictOfMostConstrainingVariables.Add(variable.Key, variable.Value.Count);
                    }

                    //int min = dictOfMostConstrainingVariableNodes.Min(entry => entry.Value);
                    //Dictionary<string, int> listOfValues = new Dictionary<string, int>();

                    // Assume most constraining value is the longer words
                    var sortedMostConstrainingVariables = dictOfMostConstrainingVariables.OrderBy(a => a.Value).ThenByDescending(b=>b.Key.Length);

                    // Reduce domain?  // Forward checking, Arc Consistency

                    // Loop through values with the lowest remaining values
                    sortedDictOfMostConstrainingVariables = sortedMostConstrainingVariables.ToDictionary(r => r.Key, r => r.Value);
                    List<string> keys = new List<string>(sortedDictOfMostConstrainingVariables.Keys);
                    for (int mrv = 0; mrv < sortedDictOfMostConstrainingVariables.Count; mrv++)
                    {
                        // What is most constraining value?  This is assumed to be longest word...already sorted by this
                        // Which value to choose?  Which value gives leaves the most remaining values for other nodes?
                        
                        nextNode.Word = keys[mrv];                       
                        nextNode.findEligibleSpots();

                        Dictionary<Node, int> dictOfLeastConstrainingValueNodes = new Dictionary<Node, int>();
                        Dictionary<Node, int> sortedDictOfLeastConstrainingValueNodes = new Dictionary<Node, int>();

                        // if no children found, backtrack...return false
                        if (nextNode.childNodes != null && nextNode.childNodes.Count > 0)
                        {
                            // loop through most constraining value
                            foreach (Node n in nextNode.childNodes)
                            {         

                                n.parentNode = currentNode;
                                if (n.SudokuWordBankList.Contains(n.Word))
                                {
                                    n.SudokuWordBankList.Remove(n.Word);
                                }
                                if (!n.SudokuWordBankList.Contains(n.Word))
                                {
                                    n.SudokuUsedWordList.Add(n.Word);
                                }
                                n.PossibleValuesDict = Node.calcPossibleSpots(n.SudokuBoardData, n.SudokuWordBankList, n.SudokuUsedWordList, n.GivenHints);

                                // forward check to see if there is a possible value for all words
                                // if not, don't include it in the options for values

                                if (pruning == 2)
                                {
                                    if (n.RemainingSpotsForAllWords)
                                    {
                                        dictOfLeastConstrainingValueNodes.Add(n, n.PossibleRemainingSpots);
                                    }
                                    else
                                    {
                                        ;
                                    }
                                }
                                else
                                {
                                    dictOfLeastConstrainingValueNodes.Add(n, n.PossibleRemainingSpots);
                                }
                            }

                            // Assume most constraining value is the longer words
                            // Sort order of this changes the results dramatically.  Actually returns better data for 1.1 by used the MostConstrainingValues
                            var sortedLeastConstrainingValues = dictOfLeastConstrainingValueNodes.OrderByDescending(a => a.Value);

                            // Loop through values with the least constraining values aka highest possible values
                            sortedDictOfLeastConstrainingValueNodes = sortedLeastConstrainingValues.ToDictionary(r => r.Key, r => r.Value);

                            List<Node> nodesOfLeastConstrainingValues = new List<Node>(sortedDictOfLeastConstrainingValueNodes.Keys);
                            for (int lrv = 0; lrv < nodesOfLeastConstrainingValues.Count; lrv++)
                            {
                                if (findDFSBackTrackingPath(nodesOfLeastConstrainingValues[lrv], visitedNodes, finalPathOfNodes, otherChildNodes, pruning))
                                {
                                    return true;
                                }
                                else
                                {
                                    nodesOfLeastConstrainingValues.Remove(nodesOfLeastConstrainingValues[lrv]);
                                    lrv = -1;
                                }

                            }
                            // No values worked, back track

                            return false;
                        }
                        //else
                        //{
                        //    return false;
                        //}
                                        
                    }

                    

                }
                catch (Exception e)
                {

                    ;

                }

            }

            return false;

        }

        static bool findDFSBackTrackingPathARC(Node currentNode, List<Node> visitedNodes, List<Node> finalPathOfNodes, List<Node> otherChildNodes, int pruning)
        {

            // In case of backtracking, no need to add a revisited node 
            // Perhaps a wall was hit, and backtracking is necessary.  No need to add the
            // revisited node again.
            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);
            }

            Console.Clear();
            Display(currentNode.SudokuBoardData, currentNode.GivenHints);

            Node nextNode = new Node(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode);

            if (AssignmentComplete(currentNode.SudokuBoardData)) // || currentNode.SudokuWordBankList.Count == 0)
            {
                //if (currentNode.SudokuWordBankList.Count == 0)
                //{
                //    return false;
                //}
                finalPathOfNodes.Clear();
                finalPathOfNodes.Add(currentNode);

                while (currentNode.parentNode != null)
                {
                    nextNode = currentNode.parentNode;
                    currentNode = nextNode;
                    finalPathOfNodes.Add(nextNode);
                }

                Console.Clear();
                Display(currentNode.SudokuBoardData, currentNode.GivenHints);

                return true;
            }
            else
            {

                // Make assignment
                try
                {

                    currentNode.showNodeInfo();

                    //string chosenDirection = "";
                    //string chosenWord = "";
                    //int offset = 0;

                    // try to choose the best variable
                    // Minimum remaining values dictate which is the most constrained variables.          
                    if (currentNode.PossibleValuesDict == null)
                    {
                        currentNode.PossibleValuesDict = Node.calcPossibleSpots(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints);
                    }

                    // Choose word with least amount of available spots (most constrained), if there is a tie?                    
                    Dictionary<string, int> dictOfMostConstrainingVariables = new Dictionary<string, int>();
                    Dictionary<string, int> sortedDictOfMostConstrainingVariables = new Dictionary<string, int>();

                    foreach (KeyValuePair<string, List<String>> variable in currentNode.PossibleValuesDict)
                    {
                        dictOfMostConstrainingVariables.Add(variable.Key, variable.Value.Count);
                    }

                    //int min = dictOfMostConstrainingVariableNodes.Min(entry => entry.Value);
                    //Dictionary<string, int> listOfValues = new Dictionary<string, int>();

                    // Assume most constraining value is the longer words
                    var sortedMostConstrainingVariables = dictOfMostConstrainingVariables.OrderBy(a => a.Value).ThenByDescending(b => b.Key.Length);

                    // Reduce domain?  // Forward checking, Arc Consistency

                    // Loop through values with the lowest remaining values
                    sortedDictOfMostConstrainingVariables = sortedMostConstrainingVariables.ToDictionary(r => r.Key, r => r.Value);
                    List<string> keys = new List<string>(sortedDictOfMostConstrainingVariables.Keys);
                    for (int mrv = 0; mrv < sortedDictOfMostConstrainingVariables.Count; mrv++)
                    {
                        // What is most constraining value?  This is assumed to be longest word...already sorted by this
                        // Which value to choose?  Which value gives leaves the most remaining values for other nodes?

                        nextNode.Word = keys[mrv];
        
                        //// AC***********************************************
                        //AC3(nextNode.Word, nextNode.SudokuWordBankList, currentNode.PossibleValuesDict, nextNode.SudokuBoardData, nextNode.SudokuUsedWordList, nextNode.GivenHints);                        

                        //try
                        //{
                        //    if (currentNode.PossibleValuesDict[nextNode.Word].Count == 0)
                        //    {
                        //        // this value may not have a gobal solution
                        //        // add dummy node to tree
                        //        Node dummyNode = new Node(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode);
                        //        dummyNode.Assignment = " (N/A): " + nextNode.Word;
                        //        dummyNode.parentNode = currentNode;
                        //        nextNode.parentNode = dummyNode;
                        //        // go to next variable in the for loop above
                        //        break;
                        //    }
                        //}
                        //catch
                        //{
                        //    // this value may not have a gobal solution
                        //    // add dummy node to tree
                        //    Node dummyNode = new Node(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode);
                        //    dummyNode.Assignment = " (N/A): " + nextNode.Word;
                        //    dummyNode.parentNode = currentNode;
                        //    nextNode.parentNode = dummyNode;
                        //    // go to next variable in the for loop above
                        //    break;
                        //}
                        //**************************************************
                        nextNode.PossibleValuesDict = currentNode.PossibleValuesDict;
                        nextNode.findEligibleSpots();

                        Dictionary<Node, int> dictOfLeastConstrainingValueNodes = new Dictionary<Node, int>();
                        Dictionary<Node, int> sortedDictOfLeastConstrainingValueNodes = new Dictionary<Node, int>();

                        // if no children found, backtrack...return false
                        if (nextNode.childNodes != null && nextNode.childNodes.Count > 0)
                        {
                            // loop through most constraining value
                            foreach (Node n in nextNode.childNodes)
                            {
                                if (n.Assignment.Equals("V,3,7: OVENBIRD"))
                                {
                                    ;
                                }
                                n.parentNode = currentNode;
                                if (n.SudokuWordBankList.Contains(n.Word))
                                {
                                    n.SudokuWordBankList.Remove(n.Word);
                                }
                                if (!n.SudokuWordBankList.Contains(n.Word))
                                {
                                    n.SudokuUsedWordList.Add(n.Word);
                                }
                                n.PossibleValuesDict = Node.calcPossibleSpots(n.SudokuBoardData, n.SudokuWordBankList, n.SudokuUsedWordList, n.GivenHints);

                                //// AC***********************************************
                                //bool isSafe = AC3_2(n.Assignment, n.SudokuWordBankList, n.PossibleValuesDict, n.SudokuBoardData, n.SudokuUsedWordList, n.GivenHints);       

                                // forward check to see if there is a possible value for all words
                                // if not, don't include it in the options for values

                                if (pruning == 2)
                                {
                                    if (n.RemainingSpotsForAllWords)
                                    //if (isSafe)
                                    {
                                        dictOfLeastConstrainingValueNodes.Add(n, n.PossibleRemainingSpots);
                                    }
                                    else
                                    {
                                        ;
                                    }
                                }
                                else
                                {
                                    dictOfLeastConstrainingValueNodes.Add(n, n.PossibleRemainingSpots);
                                }
                            }

                            // Assume most constraining value is the longer words
                            // Sort order of this changes the results dramatically.  Actually returns better data for 1.1 by used the MostConstrainingValues
                            var sortedLeastConstrainingValues = dictOfLeastConstrainingValueNodes.OrderByDescending(a => a.Value);

                            // Loop through values with the least constraining values aka highest possible values
                            sortedDictOfLeastConstrainingValueNodes = sortedLeastConstrainingValues.ToDictionary(r => r.Key, r => r.Value);

                            List<Node> nodesOfLeastConstrainingValues = new List<Node>(sortedDictOfLeastConstrainingValueNodes.Keys);
                            for (int lrv = 0; lrv < nodesOfLeastConstrainingValues.Count; lrv++)
                            {
                                if (findDFSBackTrackingPathARC(nodesOfLeastConstrainingValues[lrv], visitedNodes, finalPathOfNodes, otherChildNodes, pruning))
                                {
                                    return true;
                                }
                                else
                                {
                                    nodesOfLeastConstrainingValues.Remove(nodesOfLeastConstrainingValues[lrv]);
                                    lrv = -1;
                                }

                            }
                            // No values worked, back track
                            //Node tmpNode = new Node(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode);                            
                            //tmpNode.Assignment = " (N/A): " + nextNode.Word;
                            //tmpNode.SudokuWordBankList.Remove(nextNode.Word);
                            //nextNode.PossibleValuesDict.Remove(nextNode.Word);
                            //nextNode.parentNode = tmpNode;
                            //if (findDFSBackTrackingPath(nextNode, visitedNodes, finalPathOfNodes, otherChildNodes, pruning))
                            //{
                            //    return true;
                            //}

                            return false;
                        }
                        //else
                        //{
                        //    return false;
                        //}


                    }



                }
                catch (Exception e)
                {

                    ;

                }

            }
            // remove a word?  grid 3 hit here
            //currentNode.SudokuWordBankList.Remove(currentNode.SudokuWordBankList[currentNode.SudokuWordBankList.Count - 1]);
            //if (findDFSBackTrackingPath(currentNode, visitedNodes, finalPathOfNodes, otherChildNodes, pruning))
            //{
            //    return true;
            //}
            return false;

        }

        static bool AssignmentComplete(char[,] board)
        {
            // Find any '_' to indicate incomplete assigments

            for (int y = 8; y >= 0; y--)
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
        
        static void Display(char[,] board, List<string> givenHints)
        {
            //
            // Display everything in the List.
            //
            Console.WriteLine("Word Sudoku Board:");

            for (int y = 8; y >= 0; y--)
            {
                for (int x = 0; x < 9; x++)
                {
                    foreach (string hint in givenHints)
                    {
                        if (Int32.Parse(hint.Split('_')[0]) == x && Int32.Parse(hint.Split('_')[1]) == y)
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                        }
                        
                    }
                    Console.Write(board[x,y]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void AC3(string myChosenWord, List<string> remainingWords, Dictionary<string, List<string>> dictValues, char[,] board, List<string> usedWords, List<string> givenHints)
        {

            List<string> queOfArcs = new List<string>();
            // Populate List 
            //foreach (string word1 in remainingWords)
            //{
                foreach (string word2 in remainingWords)
                {
                    //if (!word1.Equals(word2)) {
                    if (!myChosenWord.Equals(word2))
                    {
                    //if (!queOfArcs.Contains(word1 + "_" + word2))
                    //{
                    //    queOfArcs.Add(word1 + "_" + word2);
                    //}
                        if (!queOfArcs.Contains(myChosenWord + "_" + word2))
                        {
                            queOfArcs.Add(myChosenWord + "_" + word2);
                        }
                    }
                }
                //if (!word.Equals(myWord))
                //{
                //    if (!queOfArcs.Contains(myWord + "_" + word))
                //    {
                //        queOfArcs.Add(myWord + "_" + word);
                //    }
                //}
            //}
            int startMin = dictValues.Values.Min(i => i.Count);
            int startTotal = dictValues.Values.Sum(i => i.Count);
            int endMin = 0;
            int endTotal = 0;
            while (queOfArcs.Count() > 0)
            {

                string word1 = queOfArcs[0].Split('_')[0];
                string word2 = queOfArcs[0].Split('_')[1];

                queOfArcs.Remove(word1 + "_" + word2);

                List<string> removedValues = new List<string>();
                if (removeInconsistentValues(word1, word2, dictValues, board, remainingWords, usedWords, givenHints, removedValues))
                {
                    // will have to check what was removed
                    // check words impacted by removing a letter of the word, add to queue
                    
                    foreach (string removedValue in removedValues)
                    {
                        // value H_0_8
                        string direction = removedValue.Split('_')[0].ToString().ToUpper();
                        int x = Int32.Parse(removedValue.Split('_')[1]);
                        int y = Int32.Parse(removedValue.Split('_')[2]);

                        // check 3x3 cell

                        // Consider X coodordinate range only til 9
                        // EX: 0 1 2 3 4 5 6 7 8 
                        // We investigate x = 2
                        // If divide by 3 (3 spots per cell), we can calculate cell 1-3, then add one, so we aren't dealing with cell 0-2, but cell 1-3 instead
                        int cellX = (int)Math.Floor((x / 3.0)) + 1;

                        // Now, since we don't have a 0, we can multiply by 3 to get our upper bound, then subract 1
                        int maxX = cellX * 3 - 1;

                        // Finally, we can subtract 2 to get the lower bound
                        int minX = maxX - 2;

                        // Same logic as above
                        int cellY = (int)Math.Floor((y / 3.0)) + 1;
                        int maxY = cellY * 3 - 1;
                        int minY = maxY - 2;

                        //if (direction.Equals("H"))
                        //{
                        foreach (KeyValuePair<string, List<string>> kvp in dictValues)
                            {
                            // kvp {WORD, {LIST OF VALUES}}
                            if (!kvp.Key.Equals(word1))
                            {
                                foreach (string location in kvp.Value)
                                {
                                    // check words impacted in the columns spanning the word
                                    // since word can't go H_0_8
                                    // any impacted words need to be added to the queue for rechecking
                                    int tmpX = Int32.Parse(location.Split('_')[1]);
                                    int tmpY = Int32.Parse(location.Split('_')[2]);

                                    // check if row/column
                                    if (tmpY == y || tmpX == x)
                                    {
                                        //if (tmpX >= x || tmpX <= (x + word1.Length - 1))
                                        //{
                                        if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                                        {
                                            queOfArcs.Add(kvp.Key + "_" + word1);
                                        }
                                        //}                                        
                                    }

                                    // CHECK 3x3 cell, is this needed?  I'm already adding X/Y

                                    // loop through all the cell and build a string
                                    for (int tmpY2 = minY; tmpY2 <= maxY; tmpY2++)
                                    {
                                        for (int tmpX2 = minX; tmpX2 <= maxX; tmpX2++)
                                        {
                                            if (tmpX == tmpX2 || tmpY == tmpY2)
                                            {
                                                if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                                                {
                                                    queOfArcs.Add(kvp.Key + "_" + word1);
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                            }
                        //}
                        //else
                        //{
                        //    foreach (KeyValuePair<string, List<string>> kvp in dictValues)
                        //    {
                        //        // kvp {WORD, {LIST OF VALUES}}
                        //        if (!kvp.Key.Equals(word1))
                        //        {
                        //            foreach (string location in kvp.Value)
                        //            {
                        //                // check words impacted in the rows spanning the word
                        //                // since word can't go V_0_8
                        //                // any impacted words need to be added to the queue for rechecking
                        //                int tmpX = Int32.Parse(location.Split('_')[1]);
                        //                int tmpY = Int32.Parse(location.Split('_')[2]);

                        //                if (tmpX == x)
                        //                {
                        //                    //if (tmpY >= y || tmpY <= (y - word1.Length - 1))
                        //                    //{
                        //                    if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                        //                    {
                        //                        queOfArcs.Add(kvp.Key + "_" + word1);
                        //                    }
                        //                    //}
                        //                }
                        //            }
                        //        }
                        //    }

                        //}
                    }
                    
                }
                endMin = dictValues.Values.Min(i => i.Count);
                endTotal = dictValues.Values.Sum(i => i.Count);
            }
            Console.WriteLine(startMin + " " + endMin);
            Console.WriteLine(startTotal + " " + endTotal);
        }

        static bool AC3_2(string myChosenWord, List<string> remainingWords, Dictionary<string, List<string>> dictValues, char[,] board, List<string> usedWords, List<string> givenHints)
        {

            string d = myChosenWord.Split(',')[0];
            int x1 = Int32.Parse(myChosenWord.Split(',')[1]);
            int y1 = Int32.Parse(myChosenWord.Split(',')[2].Split(':')[0]);
            string word = myChosenWord.Split(',')[2].Split(':')[1].Trim();

            List<string> queOfArcs = new List<string>();
            // Populate List 
            //foreach (string word1 in remainingWords)
            //{
            foreach (string word2 in remainingWords)
            {
                //if (!word1.Equals(word2)) {
                if (!word.Equals(word2))
                {
                    //if (!queOfArcs.Contains(word1 + "_" + word2))
                    //{
                    //    queOfArcs.Add(word1 + "_" + word2);
                    //}
                    if (!queOfArcs.Contains(word + "_" + word2))
                    {
                        queOfArcs.Add(word + "_" + word2);
                    }
                }
            }
            //if (!word.Equals(myWord))
            //{
            //    if (!queOfArcs.Contains(myWord + "_" + word))
            //    {
            //        queOfArcs.Add(myWord + "_" + word);
            //    }
            //}
            //}
            int startMin = dictValues.Values.Min(i => i.Count);
            int startTotal = dictValues.Values.Sum(i => i.Count);
            int endMin = 0;
            int endTotal = 0;
            while (queOfArcs.Count() > 0)
            {

                string word1 = queOfArcs[0].Split('_')[0];
                string word2 = queOfArcs[0].Split('_')[1];

                queOfArcs.Remove(word1 + "_" + word2);

                List<string> removedValues = new List<string>();
                if (removeInconsistentValues_2(word1, word2, dictValues, board, remainingWords, usedWords, givenHints, removedValues, d, x1, y1))
                {
                    // will have to check what was removed
                    // check words impacted by removing a letter of the word, add to queue
                    return false;

                    //foreach (string removedValue in removedValues)
                    //{
                    //    // value H_0_8
                    //    string direction = removedValue.Split('_')[0].ToString().ToUpper();
                    //    int x = Int32.Parse(removedValue.Split('_')[1]);
                    //    int y = Int32.Parse(removedValue.Split('_')[2]);

                    //    // check 3x3 cell

                    //    // Consider X coodordinate range only til 9
                    //    // EX: 0 1 2 3 4 5 6 7 8 
                    //    // We investigate x = 2
                    //    // If divide by 3 (3 spots per cell), we can calculate cell 1-3, then add one, so we aren't dealing with cell 0-2, but cell 1-3 instead
                    //    int cellX = (int)Math.Floor((x / 3.0)) + 1;

                    //    // Now, since we don't have a 0, we can multiply by 3 to get our upper bound, then subract 1
                    //    int maxX = cellX * 3 - 1;

                    //    // Finally, we can subtract 2 to get the lower bound
                    //    int minX = maxX - 2;

                    //    // Same logic as above
                    //    int cellY = (int)Math.Floor((y / 3.0)) + 1;
                    //    int maxY = cellY * 3 - 1;
                    //    int minY = maxY - 2;

                    //    //if (direction.Equals("H"))
                    //    //{
                    //    foreach (KeyValuePair<string, List<string>> kvp in dictValues)
                    //    {
                    //        // kvp {WORD, {LIST OF VALUES}}
                    //        if (!kvp.Key.Equals(word1))
                    //        {
                    //            foreach (string location in kvp.Value)
                    //            {
                    //                // check words impacted in the columns spanning the word
                    //                // since word can't go H_0_8
                    //                // any impacted words need to be added to the queue for rechecking
                    //                int tmpX = Int32.Parse(location.Split('_')[1]);
                    //                int tmpY = Int32.Parse(location.Split('_')[2]);

                    //                // check if row/column
                    //                if (tmpY == y || tmpX == x)
                    //                {
                    //                    //if (tmpX >= x || tmpX <= (x + word1.Length - 1))
                    //                    //{
                    //                    if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                    //                    {
                    //                        queOfArcs.Add(kvp.Key + "_" + word1);
                    //                    }
                    //                    //}                                        
                    //                }

                    //                // CHECK 3x3 cell, is this needed?  I'm already adding X/Y

                    //                // loop through all the cell and build a string
                    //                for (int tmpY2 = minY; tmpY2 <= maxY; tmpY2++)
                    //                {
                    //                    for (int tmpX2 = minX; tmpX2 <= maxX; tmpX2++)
                    //                    {
                    //                        if (tmpX == tmpX2 || tmpY == tmpY2)
                    //                        {
                    //                            if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                    //                            {
                    //                                queOfArcs.Add(kvp.Key + "_" + word1);
                    //                            }

                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //    //}
                    //    //else
                    //    //{
                    //    //    foreach (KeyValuePair<string, List<string>> kvp in dictValues)
                    //    //    {
                    //    //        // kvp {WORD, {LIST OF VALUES}}
                    //    //        if (!kvp.Key.Equals(word1))
                    //    //        {
                    //    //            foreach (string location in kvp.Value)
                    //    //            {
                    //    //                // check words impacted in the rows spanning the word
                    //    //                // since word can't go V_0_8
                    //    //                // any impacted words need to be added to the queue for rechecking
                    //    //                int tmpX = Int32.Parse(location.Split('_')[1]);
                    //    //                int tmpY = Int32.Parse(location.Split('_')[2]);

                    //    //                if (tmpX == x)
                    //    //                {
                    //    //                    //if (tmpY >= y || tmpY <= (y - word1.Length - 1))
                    //    //                    //{
                    //    //                    if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                    //    //                    {
                    //    //                        queOfArcs.Add(kvp.Key + "_" + word1);
                    //    //                    }
                    //    //                    //}
                    //    //                }
                    //    //            }
                    //    //        }
                    //    //    }

                    //    //}
                    //}

                }
                endMin = dictValues.Values.Min(i => i.Count);
                endTotal = dictValues.Values.Sum(i => i.Count);
            }
            Console.WriteLine(startMin + " " + endMin);
            Console.WriteLine(startTotal + " " + endTotal);
            return true;
        }

        static bool removeInconsistentValues(string word1, string word2, Dictionary<string, List<string>> dictValues, char[,] board, List<string> remainingWords, List<string> usedWords, List<string> givenHints, List<string> removedValues)
        {
            bool removed = false;
            List<string> possibleValuesWord1 = dictValues[word1];
            List<string> possibleValuesWord2 = dictValues[word2];
            char[,] tmpboard = (char[,])board.Clone();
            char[,] newBoard = (char[,])board.Clone();
            foreach (string value in possibleValuesWord1.ToList())
            {
                // value H_0_8
                string direction = value.Split('_')[0].ToString().ToUpper();
                int x = Int32.Parse(value.Split('_')[1]);
                int y = Int32.Parse(value.Split('_')[2]);

                newBoard = (char[,])tmpboard.Clone();
                newBoard = Node.updateBoardAC3(newBoard, direction, x, y, word1);
                List<string> tmpRemainingWords = new List<string>(remainingWords);
                List<string> tmpUsedWords = new List<string>(usedWords);
                tmpRemainingWords.Remove(word1);
                tmpUsedWords.Add(word1);
                try
                {
                    possibleValuesWord2 = Node.calcPossibleSpots(newBoard, tmpRemainingWords, tmpUsedWords, givenHints)[word2];
                    if (possibleValuesWord2.Count == 0)
                    {
                        possibleValuesWord1.Remove(value);
                        removedValues.Add(value);
                        removed = true;
                    }
                }
                catch
                {
                    possibleValuesWord1.Remove(value);                
                    removedValues.Add(value);
                    removed = true;
                }
            }
            return removed;
        }

        static bool removeInconsistentValues_2(string word1, string word2, Dictionary<string, List<string>> dictValues, char[,] board, List<string> remainingWords, List<string> usedWords, List<string> givenHints, List<string> removedValues, string direction, int x, int y)
        {
            bool removed = false;
            //List<string> possibleValuesWord1 = dictValues[word1];
            List<string> possibleValuesWord2 = dictValues[word2];
            char[,] tmpboard = (char[,])board.Clone();
            char[,] newBoard = (char[,])board.Clone();
            //foreach (string value in possibleValuesWord1.ToList())
            //{
                // value H_0_8
                //string direction = value.Split('_')[0].ToString().ToUpper();
                //int x = Int32.Parse(value.Split('_')[1]);
                //int y = Int32.Parse(value.Split('_')[2]);

                newBoard = (char[,])tmpboard.Clone();
                newBoard = Node.updateBoardAC3(newBoard, direction, x, y, word1);
                List<string> tmpRemainingWords = new List<string>(remainingWords);
                List<string> tmpUsedWords = new List<string>(usedWords);
                tmpRemainingWords.Remove(word1);
                tmpUsedWords.Add(word1);

                
                try
                {
                    possibleValuesWord2 = Node.calcPossibleSpots(newBoard, tmpRemainingWords, tmpUsedWords, givenHints)[word2];
                    if (possibleValuesWord2.Count == 0)
                    {
                        //possibleValuesWord1.Remove(value);
                        //removedValues.Add(value);
                        removed = true;
                    }
                }
                catch
                {
                //    possibleValuesWord1.Remove(value);
                //    removedValues.Add(value);
                    removed = true;
                }
            //}
            return removed;
        }

    }
}
