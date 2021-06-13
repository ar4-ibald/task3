using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;


namespace task3
{
    class Program
    {
        class Validation
        {
            public string ErrorMessage { get; set; }
            public bool IsCorrect { get; set; }
            public Validation(bool condition, string errorMessage)
            {
                ErrorMessage = errorMessage;
                IsCorrect = condition;
            }
            public Validation()
            {
                ErrorMessage = "";
                IsCorrect = false;
            }
        }

        public static void DisplayMenu(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {args[i]}");
            }
        }

        public static List<string> GetRequiredHalf(int requiredAmount, string[] strings, ref int reqString)
        {
            var list = new List<string>();
            for (int i = 0; i < requiredAmount; i++)
            {
                list.Add(strings[reqString]);
                if (reqString == strings.Length - 1)
                    reqString = 0;
                else
                    reqString++;
            }
            return list;
        }

        public static string DetermineWinner(string[] moves, int userMove, int computerMove)
        {
            int averageAmountOfMoves = moves.Length / 2;

            int lastWinner = computerMove == moves.Length - 1 ? 0 : ++computerMove;
            var winners = new List<string>(GetRequiredHalf(averageAmountOfMoves, moves, ref lastWinner));

            int lastLoser = lastWinner;
            var losers = new List<string>(GetRequiredHalf(averageAmountOfMoves, moves, ref lastLoser));

            if (winners.Contains(moves[userMove])) return "You win";
            if (losers.Contains(moves[userMove])) return "You lose";
            else return "Tie";
        }

        static void Main(string[] Moves)
        {
            var InputValidation = new List<Validation>()
            {
                new Validation(Moves.Length % 2 != 0, "Error: An even number of moves. Example: Rock Paper Scissors"),
                new Validation(Moves.Length >= 3, "Error: The number of moves is less than three. Example: Rock Paper Scissors"),
                new Validation(Moves.Distinct().Count() == Moves.Length, "Error: The moves are repeated. Example: Rock Paper Scissors")
            };
            if (InputValidation.All(condition => condition.IsCorrect == true))
            {
                int KeyLength = 16;
                var BytesData = new byte[KeyLength];
                RandomNumberGenerator.Fill(BytesData);
                var hmac = new HMACSHA256(BytesData);
                int ComputerMove = RandomNumberGenerator.GetInt32(Moves.Length);                
                var hash = hmac.ComputeHash(BitConverter.GetBytes(ComputerMove));
                Console.WriteLine($"HMAC: {BitConverter.ToString(hash).Replace("-", "")}");

                int UserMove;
                Validation Input;
                while (true)
                {
                    Console.WriteLine("Make a move: ");
                    DisplayMenu(Moves);
                    Console.Write(">> ");
                    UserMove = int.Parse(Console.ReadLine());
                    Input = new Validation(UserMove > 0 && UserMove <= Moves.Length, $"Error: You entered a number out of the range of available values. You must enter a number from 1 to {Moves.Length}");
                    UserMove--;
                    if (Input.IsCorrect) { Console.WriteLine($"Your move: {Moves[UserMove]}"); break; }
                    else { Console.WriteLine(Input.ErrorMessage); continue; }
                };
                Console.WriteLine($"Computer move: {Moves[ComputerMove]}" +
                    $"\n{DetermineWinner(Moves, UserMove, ComputerMove)}" +
                    $"\nHMAC key: {BitConverter.ToString(hmac.Key).Replace("-", "")}");
            }
            else
            {
                var unfulfilledConditions = InputValidation.Select(condition => condition).Where(condition => condition.IsCorrect == false);
                foreach (Validation unCondition in unfulfilledConditions) Console.WriteLine(unCondition.ErrorMessage);
            }
        }        
    }
}