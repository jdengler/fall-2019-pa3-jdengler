using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Fall2019PA3
{
    class Program
    {
        const string PLAYER = "PLAYER";
        const string DEALER = "DEALER";
        public static bool REACHED_300 = false;

        static void Main(string[] args)
        {
            int gil = 50;
            int choice;
            int lost = 0;

            //lost while loop to close game when bal reaches 0
            while (lost == 0)
            {
                //gets menu choice and calls appropriate method
                //each game method returns -1 if the users balance reaches 0 and is stored in int lost to determine if program should be quit
                choice = Menu();
                switch (choice)
                {
                    case 1: lost = Slots(ref gil); break;

                    case 2:
                        //does not allow user to play dice game if the dont have enough gil to guess once (gil < 3)
                        if (gil < 3)
                        {
                            Console.WriteLine("\n\tError: Cannot play dice game with balance less than 3 gil. Please select a different game: ");
                            break;
                        }
                        lost = Dice(ref gil);
                        break;

                    case 3: lost = Roulette(ref gil); break;

                    case 4: lost = Blackjack(ref gil); break;

                    case 5: return;
                }
            }

            return;
        }

        //Prints menu and records user input. Returns int choice.
        static int Menu()
        {
            int choice = 0;
            Console.WriteLine("\n-----\tJolly Jackpot Land Menu\t-----\n");

            //loop to ensure user inputs a valid menu choice
            while (choice < 1 || choice > 5)
            {
                Console.WriteLine("(1)\tSlot Machine" +
                                "\n(2)\tDice" +
                                "\n(3)\tRoulette" +
                                "\n(4)\tBlackjack" +
                                "\n(5)\tExit Program" +
                                "\n\nPlease enter number to select option: ");
                choice = int.Parse(Console.ReadLine());

                if (choice < 1 || choice > 5)
                    Console.WriteLine("\n\tError: Invalid input. Please enter (1), (2), (3), (4), or (5).");
            }
            return choice;
        }

        //Slot game
        static int Slots(ref int gil)
        {
            Console.WriteLine(" --------------------------- Slot Rules ----------------------------- ");
            Console.WriteLine("| > Enter how much gil to risk                                       |"); 
            Console.WriteLine("| > Computer randomly rolls 3 slots, each with 6 possible results    |");
            Console.WriteLine("| > Program outputs result and updates winnings                      |");
            Console.WriteLine("|    > If 2 slots match, bet is doubled                              |");
            Console.WriteLine("|    > If 3 slots match, bet is tripled                              |");
            Console.WriteLine("|____________________________________________________________________|");

            bool keepPlaying = true;
            String[] slotOptions = new String[6] { " Elephant ", " Computer ", " Football ", "  Resume  ", " Capstone ", "  Crimson " };
            String[] result = new String[3];
            int winnings = 0;
            int bet;

            Random rand = new Random();
            int rng;
            int count;

            //keep playing loop for game persistence
            while (keepPlaying)
            {
                count = 0;
                Console.WriteLine("\nCurrent balance: " + gil + " gil");
                bet = Bet(gil);
                gil -= bet;
                winnings -= bet;

                //rolls the 3 slots
                for (int i = 0; i < 3; i++)
                {
                    rng = rand.Next(0, 6);
                    result[i] = slotOptions[rng];
                }

                //writes the 3 slots
                Console.WriteLine(" __________   __________   __________");
                Console.WriteLine("|          | |          | |          |");
                Console.WriteLine("|" + result[0] + "| |" + result[1] + "| |" + result[2] + "|");
                Console.WriteLine("|__________| |__________| |__________|\n");

                //sets count to number of matches in slots
                if (result[0] == result[1])
                    count++;
                if (result[0] == result[2])
                    count++;
                if (result[1] == result[2])
                    count++;

                //win conditions based on count
                switch (count)
                {
                    case 0: Console.WriteLine("No matching slots. You lost " + bet + " gil."); bet = 0; break;

                    case 1: Console.WriteLine("One matching pair. You won " + (bet *= 2) + " gil."); break;

                    case 3: Console.WriteLine("Jackpot!. You won " + (bet *= 3) + " gil."); break;
                }

                //set winnings and gil
                winnings += bet;
                gil += bet;
                Console.WriteLine("\tTotal winnings from session: " + winnings + " gil");

                //endgame condition
                int chc = EndGame(gil);
                if (chc == 2)
                    keepPlaying = false;
                if (chc == -1)
                    return -1;

            }
            return 0;
        }

        //Dice game
        static int Dice(ref int gil)
        {
            Random rand = new Random();
            int rng = 0;
            bool keepPlaying = true;
            int winnings = 0;

            Console.WriteLine(" ------------------------- Dice Rules --------------------------- ");
            Console.WriteLine("| > 5 six-sided dice are rolled for a sum of 5-30                |");
            Console.WriteLine("| > Each guess deducts 3 gil from balance                        |");
            Console.WriteLine("| > Max of 4 guesses allowed, or less with low balance           |");
            Console.WriteLine("| > Program reports too high or too low if guessed incorrectly   |");
            Console.WriteLine("| > Correctly guess the sum to win 50 gil                        |");
            Console.WriteLine("| > Program outputs result and updates winnings                  |");
            Console.WriteLine("|________________________________________________________________|");

            //keep playing loop for game persistence
            while (keepPlaying)
            {
                Console.WriteLine("Current balance: " + gil + " gil");

                //if user has less than 12 gil, sets their guess limit to gil/3
                int guessLimit = 4;
                if (gil < 12)
                {
                    guessLimit = gil / 3;
                    Console.WriteLine("\n\tBecause gil balance = " + gil + ", you will only get " + guessLimit + " guesses instead of 4.");
                }

                int rollSum = 0;
                int guesses = 1;
                int currGuess;

                //rolls 5 six-sided dice and calculates roll sum
                for (int i = 0; i < 5; i++)
                {
                    rng = rand.Next(1, 7);
                    rollSum += rng;
                }

                //loop for however many guesses user has (up to 4)
                while (guesses <= guessLimit)
                {
                    Console.WriteLine("\nGuess #" + guesses + ": ");
                    currGuess = int.Parse(Console.ReadLine());

                    //deducts 3 gil for each guess
                    winnings -= 3;
                    gil -= 3;

                    //guess conditions
                    if (currGuess < rollSum)
                        Console.WriteLine("Too low!");

                    if (currGuess > rollSum)
                        Console.WriteLine("Too high!");

                    //win condition
                    if (currGuess == rollSum)
                    {
                        Console.WriteLine("Correct! You won 50 gil!");
                        winnings += 50;
                        gil += 50;
                        Console.WriteLine("\tTotal winnings from session: " + winnings + " gil");
                        break;
                    }

                    //lose condition
                    if (guesses == guessLimit && currGuess != rollSum)
                    {
                        Console.WriteLine("Failed to guess correctly within " + guessLimit + " turns. You lost " + (guessLimit * 3) + " gil. The correct answer was " + rollSum + ".");
                    }

                    guesses++;
                }

                //check again for balance < 3 to see if user can afford to play another round
                if (gil < 3)
                {
                    Console.WriteLine("\nCannot play dice game with balance less than 3 gil. Returning to menu...");
                    keepPlaying = false;
                }

                //endgame condition
                int chc = EndGame(gil);
                if (chc == 2)
                    keepPlaying = false;
                if (chc == -1)
                    return -1;
            }
            return 0;
        }

        //Roulette game
        static int Roulette(ref int gil)
        {
            //establish wheel & colours
            char[] wheel = new char[36];
            for (int i = 1; i <= 36; i++)
            {
                if (i <= 10 || (i >= 19 && i <= 28))
                {
                    if (i % 2 == 0)
                        wheel[i - 1] = 'B';
                    if (i % 2 == 1)
                        wheel[i - 1] = 'R';
                }

                if ((i >= 11 && i <= 18) || (i >= 29 && i <= 36))
                {
                    if (i % 2 == 0)
                        wheel[i - 1] = 'R';
                    if (i % 2 == 1)
                        wheel[i - 1] = 'B';
                }
            }
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" --------------------- Roulette Rules ----------------------- ");
            Console.WriteLine("| > Enter how much gil to risk                               |");
            Console.WriteLine("| > Enter character to bet on a color                        |");
            Console.WriteLine("|         (R) - Red                                          |");
            Console.WriteLine("|         (B) - Black                                        |");
            Console.WriteLine("| > Computer randomly rolls 1-36 for corresponding color     |");
            Console.WriteLine("| > Bet is double if guessed correctly                       |");
            Console.WriteLine("| > Program outputs result and updates winnings              |");
            Console.WriteLine("|____________________________________________________________|");

            bool keepPlaying = true;
            int winnings = 0;

            //keep playing loop for game persistence
            while (keepPlaying)
            {
                Console.WriteLine("\nCurrent balance: " + gil + " gil");
                int bet = Bet(gil);
                char guess = '~';

                //loop for guess validity (must enter r or b)
                while (guess != 'R' && guess != 'B')
                {
                    Console.WriteLine("\nPlease enter what color you would like to guess: (R) for Red or (B) for Black)");
                    guess = Console.ReadLine().ToUpper()[0];

                    if (guess != 'R' && guess != 'B')
                        Console.WriteLine("\n\tError: Invalid input. Please enter (R) or (B).");
                }

                //rolls spot on roulette wheel
                Random rand = new Random();
                int rng = rand.Next(0, 36);

                //win condition determined and winnings/bet set
                if (guess == wheel[rng])
                {
                    Console.WriteLine("Correct! You won " + bet * 2 + " gil.");
                    winnings += bet;
                    gil += bet;
                }
                else
                {
                    Console.WriteLine("Incorrect! You lose " + bet + " gil.");
                    winnings -= bet;
                    gil -= bet;
                }
                Console.WriteLine("\tTotal winnings from session: " + winnings + " gil");

                //Endgame condition
                int chc = EndGame(gil);
                if (chc == 2)
                    keepPlaying = false;
                if (chc == -1)
                    return -1;
            }
            return 0;
        }

        //Blackjack game
        static int Blackjack(ref int gil)
        {
            //creates deck string array and populates it with 52 unique cards
            String[] deck = new String[52];
            PopDeck(ref deck);

            Console.WriteLine(" ------------------------------ Blackjack Rules --------------------------------- ");
            Console.WriteLine("| > Enter how much gil to risk                                                   |");
            Console.WriteLine("| > Computer randomly shuffles 52 card deck                                      |");
            Console.WriteLine("| > Player and dealer are each dealt two cards and printed to screen             |");
            Console.WriteLine("|    > Dealers first card is hidden while user plays                             |");
            Console.WriteLine("| > Hit to be dealt another card until satisfied with hand                       |");
            Console.WriteLine("|    > Cards are worth their numeric value, face cards are worth 10              |");
            Console.WriteLine("|    > Ace is worth 11 or 1 depending on overall hand total                      |");
            Console.WriteLine("|    > Exceeding 21 is a bust and bet is lost                                    |");
            Console.WriteLine("| > When finished hitting, dealer continually hits if they are below 17          |");
            Console.WriteLine("| > Program outputs result and updates winnings                                  |");
            Console.WriteLine("|    > If user is closer to 21 than dealer without busting, user bet is doubled  |");
            Console.WriteLine("|    > If dealer is closer to 21 than user without busting, user bet is lost     |");
            Console.WriteLine("|    > If user and dealer hands are equivalent, bet is returned to user balance  |");
            Console.WriteLine("|________________________________________________________________________________|");

            bool keepPlaying = true;
            int winnings = 0;

            //keep playing loop for game persistence
            while (keepPlaying)
            {
                deck = ShuffleDeck(deck);

                //Establish player and dealer hands as ArrayLists
                ArrayList playerHand = new ArrayList();
                int playerSum;
                ArrayList dealerHand = new ArrayList();
                int dealerSum;

                int dealCnt = 1;    //start at one to dispose top card

                Console.WriteLine("\nCurrent balance: " + gil + " gil");
                int bet = Bet(gil);

                //first round of deals
                playerHand.Add(deck[dealCnt++]);
                dealerHand.Add(deck[dealCnt++]);

                //second round of deals
                playerHand.Add(deck[dealCnt++]);
                dealerHand.Add(deck[dealCnt++]);
                playerSum = HandVal(playerHand);


                //loop for user to chose to 'hit' for another card or stay
                char hit = 'Y';
                while (hit == 'Y')
                {
                    //clears console and prints both player and dealer hands
                    Console.Clear();
                    PrintHand(playerHand, PLAYER, playerSum);
                    PrintHand(dealerHand, DEALER, -1);

                    //user bust condition
                    if (playerSum > 21)
                    {
                        Console.WriteLine("You bust! You lose " + bet + " gil.");
                        winnings -= bet;
                        gil -= bet;
                        break;
                    }

                    Console.WriteLine("Hit? Y/N:");
                    hit = Console.ReadLine().ToUpper()[0];

                    //adds a new card to user hand and updates playersum if they hit
                    if (hit == 'Y')
                    {
                        playerHand.Add(deck[dealCnt++]);
                        playerSum = HandVal(playerHand);
                    }
                }

                //if (user didnt bust)
                if (playerSum <= 21)
                {
                    Console.Clear();
                    dealerSum = HandVal(dealerHand);
                    PrintHand(playerHand, PLAYER, playerSum);
                    PrintHand(dealerHand, DEALER, dealerSum);

                    //loop for dealer to continually hit while dealersum is less than 17
                    while (dealerSum < 17)
                    {
                        //adds card to dealer hand and updates dealersum
                        Thread.Sleep(1500);
                        dealerHand.Add(deck[dealCnt++]);
                        dealerSum = HandVal(dealerHand);

                        Console.Clear();
                        PrintHand(playerHand, PLAYER, playerSum);
                        PrintHand(dealerHand, DEALER, dealerSum);
                    }

                    //win condition if-else tree
                    if (dealerSum > 21)
                    {   //dealer busted condition
                        Console.WriteLine("The dealer bust! You won " + bet + " gil.");
                        winnings += bet;
                        gil += bet;
                    }
                    else if (playerSum > dealerSum)
                    {   //player win condition
                        Console.WriteLine("Your hand beats the dealers hand! You won " + bet * 2 + " gil.");
                        winnings += bet;
                        gil += bet;
                    }
                    else if (playerSum < dealerSum)
                    {   //dealer win condition
                        Console.WriteLine("The dealers hand beats your hand! You lost " + bet + " gil.");
                        winnings -= bet;
                        gil -= bet;
                    }
                    else if (playerSum == dealerSum)
                    {   //tie win condition
                        Console.WriteLine("The hands are tied! " + bet + " gil was returned to your balance.");
                    }
                }

                Console.WriteLine("\tTotal winnings from session: " + winnings + " gil");

                //endgame condition
                int chc = EndGame(gil);
                if (chc == 2)
                    keepPlaying = false;
                if (chc == -1)
                    return -1;
            }
            return 0;
        }

        //calculates and returns value of hand
        static int HandVal(ArrayList hand)
        {
            //multi-dim array of form [card id, card value] for each of the 13 possible cards in a deck
            String[,] cardVals = { { " A", "11" }, { " 2", "2" }, { " 3", "3" }, { " 4", "4" }, { " 5", "5" }, { " 6", "6" }, { " 7", "7" }, { " 8", "8" }, { " 9", "9" }, { "10", "10" }, { " J", "10" }, { " Q", "10" }, { " K", "10" } };

            //array for the value of each card in hand
            int[] handVals = new int[hand.Count];
            int sum = 0;
            bool hasAce = false;

            //loop to count each card in hand
            for (int i = 0; i < hand.Count; i++)
            {
                //sets current cards id
                String id = hand[i].ToString().Split(',')[0];

                //loop to search through cardVals multi-dim array for corresponding id and sets handVals index to appropriate amount
                for (int j = 0; j < 13; j++)
                {
                    if (id == cardVals[j, 0])
                    {
                        handVals[i] = int.Parse(cardVals[j, 1]);
                    }
                }

                //confirms existence of ace in hand for determining value of 1 or 11
                if (handVals[i] == 11)
                    hasAce = true;

                //sets ace value to 1 if ace is in hand (worth 11) and if hand value exceeds 21
                if (sum + handVals[i] > 21 && hasAce)
                {
                    sum -= 10;
                    hasAce = false;
                }
                sum += handVals[i];
            }
            return sum;
        }

        //populates deck with 52 unique cards
        static void PopDeck(ref String[] deck)
        {
            String[] ids = { " A", " 2", " 3", " 4", " 5", " 6", " 7", " 8", " 9", "10", " J", " Q", " K" };
            String[] suits = { ",♠", ",♥", ",♣", ",♦" };
            int cardCount = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    deck[cardCount] = ids[j] + suits[i];
                    cardCount++;
                }
            }
        }

        //ripple shuffles 30x for increased randomness and returns shuffled deck
        static String[] ShuffleDeck(String[] deck)
        {
            for (int i = 0; i < 30; i++)
                deck = RippleShuffle(deck);

            return deck;
        }

        //shuffle method that splits deck in half and randomly alternates card placement. returns shuffled deck
        static String[] RippleShuffle(String[] deck)
        {
            String[] shuffled = new String[52];
            Random rand = new Random();
            int rng = 0;

            //split in half for 26 pairings
            for (int i = 0; i < 26; i++)
            {
                rng = rand.Next(0, 2);

                //top card/bottom card randomized decision for each pairing
                switch (rng)
                {
                    case (0):
                        shuffled[i * 2] = deck[i];
                        shuffled[(i * 2) + 1] = deck[i + 26];
                        break;

                    case (1):
                        shuffled[i * 2] = deck[i + 26];
                        shuffled[(i * 2) + 1] = deck[i];
                        break;
                }
            }
            return shuffled;
        }

        //receives card string and returns respective ascii card stored as a multi-dim array
        static char[,] CardBuilder(string card)
        {
            String[] splitCard = card.Split(',');
            String id = splitCard[0];
            char suit = splitCard[1][0];
            //establishes card outline 'stencil'
            String[] lines = {  "╔═══════════╗ ",
                                "║           ║ ",
                                "║           ║ ",
                                "║           ║ ",
                                "║           ║ ",
                                "║           ║ ",
                                "║           ║ ",
                                "║           ║ ",
                                "║           ║ ",
                                "╚═══════════╝ " };

            //sets each index in matrix to return to corresponding char for card outline
            char[,] matrix = new char[10, 14];
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    matrix[i, j] = lines[i][j];
                }
            }

            //setting specific indices for card suit and id
            if (id[0] == ' ')   //if-else for 10 card's b/c only card with a 2-char id
                matrix[1, 2] = id[1];
            else
            {
                matrix[1, 2] = id[0];
                matrix[1, 3] = id[1];
            }
            matrix[8, 9] = id[0];
            matrix[8, 10] = id[1];
            matrix[5, 6] = suit;

            return matrix;
        }

        //method to emulate dealers first card as face-down until dealers turn
        static char[,] HiddenCard(char[,] cardMat)
        {
            for (int r = 1; r < 9; r++)
            {
                for (int c = 1; c < 12; c++)
                {
                    cardMat[r, c] = '░';
                }
            }
            return cardMat;
        }

        //receives hand as an arraylist and creates an array of appended cards to show full hand on one line
        static char[,] HandMatrix(ArrayList hand, bool hide)
        {
            char[,] handMat = new char[10, (14 * hand.Count)];
            char[,] tempCardMat = new char[10, 14];

            //for each card in hand...
            for (int i = 0; i < hand.Count; i++)
            {
                tempCardMat = CardBuilder(hand[i].ToString());
                //sets first card to hidden if bool statement passed in as true
                if (hide && i == 0)
                    tempCardMat = HiddenCard(tempCardMat);

                //appends lines for each card
                for (int r = 0; r < 10; r++)
                    for (int c = 0; c < 14; c++)
                        handMat[r, c + (i * 14)] = tempCardMat[r, c];
            }
            return handMat;
        }

        //prints hand out along with name and sum
        static void PrintHand(ArrayList hand, String who, int sum)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            char[,] handMat = new char[10, (14 * hand.Count)];

            //sum is only passed in as -1 if it is dealers hand and players turn
            if (sum == -1)
            {
                Console.WriteLine(who + "'S HAND\tSUM: ?");
                handMat = HandMatrix(hand, true);
            }
            else
            {
                Console.WriteLine(who + "'S HAND\tSUM: " + sum);
                handMat = HandMatrix(hand, false);
            }

            //print each char in order
            for (int r = 0; r < 10; r++)
            {
                for (int c = 0; c < (14 * hand.Count); c++)
                {
                    Console.Write(handMat[r, c]);
                }
                Console.Write("\n");
            }

        }

        //prompts user for amount to bet and returns bet amount
        static int Bet(int gil)
        {
            bool valid = false;
            int bet = 0;
            String input;

            //loop for bet validity
            while (!valid)
            {
                Console.WriteLine("\nPlease enter amount of gil to bet: ");
                input = Console.ReadLine();

                if (input.All(char.IsDigit))
                    bet = int.Parse(input);

                if (bet <= 0)
                    Console.WriteLine("\n\tError: Bet must be a positive number.");

                else if (bet > gil)
                    Console.WriteLine("\n\tError: Cannot bet more than balance.");

                else
                    valid = true;
            }

            return bet;
        }

        //establishes the end game condition and returns int that identifies corresponding condition
        static int EndGame(int gil)
        {
            int chc = 0;
            String input;

            //user bankrupt condition
            if (gil == 0)
            {
                Console.Write("\nYou lost! Your gil balance = 0.\n\nExitting game in ");
                for (int i = 3; i > 0; i--)
                {
                    Console.Write(i + "... ");
                    Thread.Sleep(1000);
                }

                return -1;
            }

            //user reached 300 for first time condition
            if (gil >= 300 && !REACHED_300)
            {
                Console.WriteLine("\nCongratulations! You reached your goal of 300 gil!");
                REACHED_300 = true;
                return 2;
            }

            //loop for input validity
            while (chc < 1 || chc > 2)
            {
                Console.WriteLine("\nWould you like to keep playing?\n(1)\tYes\n(2)\tReturn to menu");
                input = Console.ReadLine();

                if (input.All(char.IsDigit))
                    chc = int.Parse(input);

                if (chc < 1 || chc > 2)
                    Console.WriteLine("\n\tError: Invalid input. Please enter (1) or (2).");
            }

            //clears console if user chooses to return to menu
            if (chc == 2)
                Console.Clear();

            return chc;
        }
    }
}
