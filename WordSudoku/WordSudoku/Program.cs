﻿using System;
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
            Display(sudokuBoardData, givenHints);

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

            //startStateNode.showNodeInfo();
            //startStateNode.findEligibleAssignments();

            //foreach (Node n in startStateNode.childNodes)
            //{
            //    n.showNodeInfo();
            //}
            // DFS

            List<Node> pathToGoalState = new List<Node>();
            List<Node> otherChildNodes = new List<Node>();
            int pruningStrategy = 2;
            found = findDFSBackTrackingPath(currentNode, visitedNodes, pathToGoalState, otherChildNodes, pruningStrategy);
            //found = findDFSBackTrackingPath2(currentNode, visitedNodes, pathToGoalState, otherChildNodes, pruningStrategy);

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
                if (pruningStrategy == 0)
                {
                    Console.WriteLine("Brute Force");
                }
                else if (pruningStrategy == 1)
                {
                    Console.WriteLine("Forward Checking");
                }
                else if (pruningStrategy == 2)
                {
                    Console.WriteLine("Arc Consistency");
                }

                Console.WriteLine(" Search Started: " + start);
                Console.WriteLine(" Search Ended: " + end);
                Console.WriteLine(" Duration: " + (end - start));
                Console.WriteLine(" Nodes visited: " + visitedNodes.Count());
                // Display the finalPath backwards
                pathToGoalState.Reverse();
                foreach (Node n in pathToGoalState)
                {
                    n.showNodeInfo();
                    orderOfAssignment.Add(n.Assignment);
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
                if (pruningStrategy == 0)
                {
                    Console.WriteLine("Brute Force");
                }
                else if (pruningStrategy == 1)
                {
                    Console.WriteLine("Forward Checking");
                }
                else if (pruningStrategy == 2)
                {
                    Console.WriteLine("Arc Consistency");
                }                

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


        static bool findDFSBackTrackingPath2(Node currentNode, List<Node> visitedNodes, List<Node> finalPathOfNodes, List<Node> otherChildNodes, int pruning)
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
                Display(currentNode.SudokuBoardData, currentNode.GivenHints);

                return true;
            }
            else
            {

                // Make assignment
                try
                {

                    // try to choose the best variable
                    // Minimum remaining values dictate which is the most constrained variables.                    
                    currentNode.PossibleValuesDict = Node.calcPossibleSpots(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints);

                    // Choose word with least amount of available spots (most constrained), if there is a tie, choose the longest word as the more constraining variable
                    //int? minValues = null;
                    string mostConstrainedVariable = "";
                    Node mostConstrainedVariableNode = null;
                    Dictionary<string, int> dictOfMostConstrainingVariableNodes = new Dictionary<string, int>();
                    Dictionary<string, int> dictGroupOfMostConstrainingVariableNodes = new Dictionary<string, int>();

                    int cnt = 0;
                    foreach (KeyValuePair<string, List<String>> variable in currentNode.PossibleValuesDict)
                    {
                        dictOfMostConstrainingVariableNodes.Add(variable.Key, variable.Value.Count);
                    }

                    // Loop through values with the lowest remaining values
                    int min = dictOfMostConstrainingVariableNodes.Min(entry => entry.Value);
                    Dictionary<string, int> listOfMostConstrainedVariableNodes = new Dictionary<string, int>();
                    Dictionary<string, int> listOfValues = new Dictionary<string, int>();

                    var keysWithSameMRV = dictOfMostConstrainingVariableNodes.Where(i => i.Value.Equals(min));



                    //// TODO    
                    //// Somehow sort by key length, to  choose more constraining variable as tiebreaker?
                    //List<string> sortedVariables = new List<string>();
                    //foreach (KeyValuePair<string, int> kvp in keysWithSameMRV)
                    //{
                    //    sortedVariables.Add(kvp.Key);
                    //}

                    //sortedVariables = sortedVariables.OrderByDescending(s => s.Length).ToList();

                    //// Call ARC Routine
                    ////AC3(currentNode.SudokuWordBankList, currentNode.PossibleValuesDict, currentNode.SudokuBoardData, currentNode.SudokuUsedWordList, currentNode.GivenHints);

                    ////Choose the best value
                    //// Loop through values that are least constraining
                    //List<string> listOfTempValues = new List<string>();
                    //int cntConstraints = 0;
                    //for (int p = 0; p < currentNode.PossibleValuesDict[sortedVariables[0]].Count; p++ )
                    ////foreach (string position in currentNode.PossibleValuesDict[sortedVariables[0]])
                    //{

                    //    //if (reducedDomain.Contains(position))
                    //    //{
                    //        // Looping through reduced domains
                    //        foreach(string remainingWord in currentNode.SudokuWordBankList)
                    //        {
                    //            if (!remainingWord.Equals(sortedVariables[0]))
                    //            {
                    //                // Checking impact of the remaining values
                    //                char[,] tempBoard = (char[,])currentNode.SudokuBoardData.Clone();
                    //            // Update board
                    //            //string direction = position.Split('_')[0].ToString().ToUpper();
                    //            //int x = Int32.Parse(position.Split('_')[1]);
                    //            //int y = Int32.Parse(position.Split('_')[2]);
                    //            string direction = currentNode.PossibleValuesDict[sortedVariables[0]][p].Split('_')[0].ToString().ToUpper();
                    //            int x = Int32.Parse(currentNode.PossibleValuesDict[sortedVariables[0]][p].Split('_')[1]);
                    //            int y = Int32.Parse(currentNode.PossibleValuesDict[sortedVariables[0]][p].Split('_')[2]);

                    //            tempBoard = Node.updateBoardAC3(tempBoard, direction, x, y, sortedVariables[0]);

                    //            try
                    //            {
                    //                listOfTempValues = Node.calcPossibleSpots(tempBoard, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints)[remainingWord];
                    //                // {H_1_3, etc}
                    //                // Add the counts per word per position
                    //                // 
                    //                if (listOfValues.ContainsKey(currentNode.PossibleValuesDict[sortedVariables[0]][p]))
                    //                {
                    //                    cntConstraints = listOfValues[currentNode.PossibleValuesDict[sortedVariables[0]][p]] + listOfTempValues.Count;
                    //                    listOfValues.Remove(currentNode.PossibleValuesDict[sortedVariables[0]][p]);
                    //                    listOfValues.Add(currentNode.PossibleValuesDict[sortedVariables[0]][p], cntConstraints);
                    //                }
                    //                else
                    //                {
                    //                    listOfValues.Add(currentNode.PossibleValuesDict[sortedVariables[0]][p], listOfTempValues.Count);
                    //                }
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                ;
                    //                currentNode.PossibleValuesDict[sortedVariables[0]].Remove(currentNode.PossibleValuesDict[sortedVariables[0]][p]);
                    //                p = -1;
                    //                break;
                    //            }



                    //            }
                    //        }

                    //    //}
                    //}

                    ////// Loop through positions with the most remaining values
                    //int max = 0;
                    //try
                    //{
                    //    max = listOfValues.Max(entry => entry.Value);
                    //}
                    //catch
                    //{
                    //    return false;
                    //}

                    //var keys = new List<string>(listOfValues.Keys);
                    ////foreach (string key in keys.ToList())
                    //for (int k = 0; k < keys.Count; k++)
                    //{
                    //    if (listOfValues[keys[k]] == max)
                    //    {
                    //        nextNode.Word = sortedVariables[0];
                    //        nextNode.SudokuUsedWordList.Add(sortedVariables[0]);
                    //        nextNode.SudokuWordBankList.Remove(sortedVariables[0]);
                    //        nextNode.x = Int32.Parse(keys[k].Split('_')[1]);
                    //        nextNode.y = Int32.Parse(keys[k].Split('_')[2]);
                    //        nextNode.Assignment = keys[k] + ": " + nextNode.Word;
                    //        nextNode.SudokuBoardData = nextNode.updateBoard(nextNode.SudokuBoardData, keys[k].Split('_')[0], nextNode.x, nextNode.y, nextNode.Word, nextNode.CurrentVariablePriority);
                    //        if (findDFSBackTrackingPath2(nextNode, visitedNodes, finalPathOfNodes, otherChildNodes, pruning))
                    //        {
                    //            return true;
                    //        }
                    //        else
                    //        {

                    //            listOfValues.Remove(keys[k]);
                    //            keys = new List<string>(listOfValues.Keys);
                    //            if (keys.Count < 0)
                    //            {
                    //                max = listOfValues.Max(entry => entry.Value);
                    //            }
                    //            k = -1;
                    //        }
                    //    }
                    //}
                    ////foreach (KeyValuePair<string, int> kvpValues in listOfValues.Where(i => i.Value.Equals(max)))
                    ////{
                    ////    nextNode.Word = sortedVariables[0];
                    ////    nextNode.SudokuUsedWordList.Add(sortedVariables[0]);
                    ////    nextNode.SudokuWordBankList.Remove(sortedVariables[0]);
                    ////    nextNode.x = Int32.Parse(kvpValues.Key.Split('_')[1]);
                    ////    nextNode.y = Int32.Parse(kvpValues.Key.Split('_')[2]);
                    ////    nextNode.Assignment = kvpValues.Key + ": " + nextNode.Word;
                    ////    nextNode.SudokuBoardData = nextNode.updateBoard(nextNode.SudokuBoardData, kvpValues.Key.Split('_')[0], nextNode.x, nextNode.y, nextNode.Word, nextNode.CurrentVariablePriority);
                    ////    if (findDFSBackTrackingPath2(nextNode, visitedNodes, finalPathOfNodes, otherChildNodes, pruning))
                    ////    {
                    ////        return true;
                    ////    }
                    ////    else
                    ////    {
                    ////        listOfValues.Remove(kvpValues.Key);
                    ////    }

                    ////}

                    //return false;


                    foreach (KeyValuePair<string, int> kvp in keysWithSameMRV)
                    {
                        // Loop through values that are least constraining
                        foreach (string position in currentNode.PossibleValuesDict[kvp.Key])
                        {
                            // Simulate Choosing one
                            List<string> newArcWordList = new List<string>(currentNode.SudokuWordBankList);
                            List<string> newArcUsedWordList = new List<string>(currentNode.SudokuUsedWordList);

                            char[,] newArcBoard = new char[9, 9];
                            for (int j = 0; j < 9; j++)
                            {
                                for (int k = 0; k < 9; k++)
                                {
                                    newArcBoard[j, k] = currentNode.SudokuBoardData[j, k];
                                }
                            }

                            Node ArcCheckNode = new Node(newArcBoard, newArcWordList.ToList<string>(), newArcUsedWordList.ToList<string>(), currentNode.GivenHints, currentNode.x, currentNode.y, currentNode.CurrentVariablePriority, currentNode.parentNode);
                            ArcCheckNode.x = Int32.Parse(position.Split('_')[0]);
                            ArcCheckNode.y = Int32.Parse(position.Split('_')[1]);
                            ArcCheckNode.Word = kvp.Key;
                            ArcCheckNode.SudokuBoardData = ArcCheckNode.updateBoard(ArcCheckNode.SudokuBoardData, kvp.Key.Split('_')[0], ArcCheckNode.x, ArcCheckNode.y, ArcCheckNode.Word.Split('_')[1], ArcCheckNode.CurrentVariablePriority);
                            ArcCheckNode.SudokuWordBankList.Remove(ArcCheckNode.Word);
                            ArcCheckNode.SudokuUsedWordList.Add(ArcCheckNode.Word);
                            ArcCheckNode.PossibleValuesDict = Node.calcPossibleSpots(ArcCheckNode.SudokuBoardData, ArcCheckNode.SudokuWordBankList, ArcCheckNode.SudokuUsedWordList, ArcCheckNode.GivenHints);

                            bool isAssignmentSafe = true;
                            int cntConstraintImpact = 0;
                            // Count impact on remaining words, if any are left 0, inconsistent.
                            foreach (KeyValuePair<string, List<string>> ArchKvp in ArcCheckNode.PossibleValuesDict)
                            {
                                if (ArchKvp.Value.Count == 0)
                                {
                                    currentNode.PossibleValuesDict.Remove(position);
                                    isAssignmentSafe = false;
                                }
                                else
                                {
                                    cntConstraintImpact = cntConstraintImpact + ArchKvp.Value.Count;
                                }
                            }
                            if (!isAssignmentSafe)
                            {
                                continue;
                            }
                            else
                            {
                                listOfValues.Add(position, cntConstraintImpact);
                            }

                        }

                        // Loop through positions with the most remaining values
                        int min2 = listOfValues.Min(entry => entry.Value);
                        foreach (KeyValuePair<string, int> kvpValues in listOfValues.Where(i => i.Value.Equals(min2)))
                        {
                            nextNode.Word = kvp.Key.Split('_')[1];
                            nextNode.SudokuUsedWordList.Add(nextNode.Word);
                            nextNode.SudokuWordBankList.Remove(nextNode.Word);
                            nextNode.x = Int32.Parse(kvpValues.Key.Split('_')[0]);
                            nextNode.y = Int32.Parse(kvpValues.Key.Split('_')[1]);
                            nextNode.Assignment = kvp.Key.Split('_')[0]+ "," + kvpValues.Key.Replace('_',',') + ": " + nextNode.Word;
                            nextNode.SudokuBoardData = nextNode.updateBoard(nextNode.SudokuBoardData, kvp.Key.Split('_')[0], nextNode.x, nextNode.y, nextNode.Word, nextNode.CurrentVariablePriority);
                            if (findDFSBackTrackingPath(nextNode, visitedNodes, finalPathOfNodes, otherChildNodes, pruning))
                            {
                                return true;
                            }
                            else
                            {
                                ;
                            }

                        }

                        //    //min = dictOfMostConstrainingVariableNodes.Min(entry => entry.Value);

                        //    // 
                        //}


                        //foreach (KeyValuePair<string, int> kvp in dictOfMostConstrainingVariableNodes)
                        //{
                        //    string word = kvp.Key.Split('_')[1];

                        //    try
                        //    {
                        //        cnt = dictGroupOfMostConstrainingVariableNodes[word];
                        //        dictGroupOfMostConstrainingVariableNodes.Remove(word);
                        //        dictGroupOfMostConstrainingVariableNodes.Add(word, cnt + kvp.Value);

                        //    }
                        //    catch
                        //    {

                        //        dictGroupOfMostConstrainingVariableNodes.Add(word, kvp.Value);
                        //    }
                        //}








                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }
            return false;

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
                Display(currentNode.SudokuBoardData, currentNode.GivenHints);

                return true;
            }
            else
            {                

                // Make assignment
                try
                {

                    currentNode.showNodeInfo();

                    string chosenDirection = "";
                    string chosenWord = "";
                    int offset = 0;
                    
                    // try to choose the best variable
                    // Minimum remaining values dictate which is the most constrained variables.                    
                    currentNode.PossibleValuesDict = Node.calcPossibleSpots(currentNode.SudokuBoardData, currentNode.SudokuWordBankList, currentNode.SudokuUsedWordList, currentNode.GivenHints);

                    // Choose word with least amount of available spots (most constrained), if there is a tie?                    
                    string mostConstrainedVariable = "";
                    Node mostConstrainedVariableNode = null;
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
                                if (n.Assignment.Equals("V,7,7: COQUETRY"))
                                {
                                    ;
                                }
                                n.PossibleValuesDict = Node.calcPossibleSpots(n.SudokuBoardData, n.SudokuWordBankList, n.SudokuUsedWordList, n.GivenHints);
                                // forward check to see if there is a possible value for all words
                                // if not, don't include it in the options for values
            
                                if (n.RemainingSpotsForAllWords)
                                {
                                    dictOfLeastConstrainingValueNodes.Add(n, n.PossibleRemainingSpots);
                                }
                                else
                                {
                                    ;
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
                                if (nodesOfLeastConstrainingValues[lrv].Assignment.Equals("H,0,7: OBSTINACY"))
                                {
                                    ;
                                }
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
            //// remove a word?  grid 3 hit here
            //currentNode.SudokuWordBankList.Remove(currentNode.SudokuWordBankList[currentNode.SudokuWordBankList.Count-1]);
            //if (findDFSBackTrackingPath(currentNode, visitedNodes, finalPathOfNodes, otherChildNodes))
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

        static void AC3(List<string> remainingWords, Dictionary<string, List<string>> dictValues, char[,] board, List<string> usedWords, List<string> givenHints)
        {

            List<string> queOfArcs = new List<string>();
            // Populate List 
            foreach (string word1 in remainingWords)
            {
                foreach (string word2 in remainingWords)
                {
                    if (!word1.Equals(word2)) {
                        if (!queOfArcs.Contains(word1 + "_" + word2))
                        {
                            queOfArcs.Add(word1 + "_" + word2);
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
            }
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

                        if (direction.Equals("H"))
                        {
                            foreach (KeyValuePair<string, List<string>> kvp in dictValues)
                            {
                                // kvp {WORD, {LIST OF VALUES}}
                                foreach (string location in kvp.Value)
                                {
                                    // check words impacted in the columns spanning the word
                                    // since word can't go H_0_8
                                    // any impacted words need to be added to the queue for rechecking
                                    int tmpX = Int32.Parse(location.Split('_')[1]);
                                    int tmpY = Int32.Parse(location.Split('_')[2]);

                                    if (tmpY == y)
                                    {
                                        //if (tmpX >= x || tmpX <= (x + word1.Length - 1))
                                        //{
                                            if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                                            {
                                                queOfArcs.Add(kvp.Key + "_" + word1);
                                            }
                                        //}                                        
                                    }                                    
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, List<string>> kvp in dictValues)
                            {
                                // kvp {WORD, {LIST OF VALUES}}
                                if (!kvp.Key.Equals(word1))
                                {
                                    foreach (string location in kvp.Value)
                                    {
                                        // check words impacted in the rows spanning the word
                                        // since word can't go V_0_8
                                        // any impacted words need to be added to the queue for rechecking
                                        int tmpX = Int32.Parse(location.Split('_')[1]);
                                        int tmpY = Int32.Parse(location.Split('_')[2]);

                                        if (tmpX == x)
                                        {
                                            //if (tmpY >= y || tmpY <= (y - word1.Length - 1))
                                            //{
                                            if (!queOfArcs.Contains(kvp.Key + "_" + word1))
                                            {
                                                queOfArcs.Add(kvp.Key + "_" + word1);
                                            }
                                            //}
                                        }
                                    }
                                }
                            }

                        }
                    }
                    
                }
                endMin = dictValues.Values.Min(i => i.Count);
                endTotal = dictValues.Values.Sum(i => i.Count);
            }
            Console.WriteLine(startMin + " " + endMin);
            Console.WriteLine(startTotal + " " + endTotal);
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
                try
                {
                    possibleValuesWord2 = Node.calcPossibleSpots(newBoard, remainingWords, usedWords, givenHints)[word2];
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
    }
}
