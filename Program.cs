using System;
using System.IO;

namespace Karcher_MadLibs
{
    class Program
    {
        /* Author: Jonathan Karcher
         * Purpose: Let the player play a Mad Libs game based on a seperate text document
         * Restrictions: The document must be in english and plain text
         */
        static void Main(string[] args)
        {
            #region Initialization
            string file = "c:\\templates\\MadLibsTemplate.txt";
            // How many stories do we have in the text doc
            int numberOfStories = 0;
            // what is your name
            string name = "";
            // general response that does not need to be recorded
            string input = "";
            // counter to see how many times the player has entered the wrong value for yes, no questions
            int yesNoCounter = 0;
            // create a random
            Random rand = new Random();
            // How many stories are in the document
            using (StreamReader sr = File.OpenText(file))
            {
                while ((sr.ReadLine()) != null)
                {
                    numberOfStories++;
                }
            }
            // prompts for the user encourage the user to enter yes or no
            string[] YesNoResponces = { "Come on _, try harder." ,"That’s definitely not 'yes' or 'no'. Try again _.", "Really _?  You can’t type yes or no?", "You can do better than that!", "Your options are yes and no... I told you that right?", "At some point I have to assume this is intentional." };
            #endregion
            #region Game
            // greet the player
            Console.WriteLine("Hello, would you like to play Mad Libs\nyes or no");
            input = Console.ReadLine();
            // if they didnt enter an acceptable value
            while((input.ToLower() != "yes") && (input.ToLower() != "no"))
            {
                yesNoCounter++;
                // tell the player what they did wrong
                if (yesNoCounter != 3)
                {
                    Console.WriteLine("You must eventually type \"yes\" or \"no\" to move on with the program.");
                }
                // give the player a random prompt
                else
                {
                    // we have to restrict the responces to [3] and later since we dont know the players name yet
                    Console.WriteLine(YesNoResponces[rand.Next(3, YesNoResponces.Length)]);
                    yesNoCounter = 0;
                }
                input = Console.ReadLine();
            }
            // if they want to play the game
            if(input.ToLower() == "yes")
            {
                // ask the player for their name
                Console.WriteLine("Enter your name");
                name = Console.ReadLine();
                // once you have their name add their name to the yes no prompts
                for(int i = 0; i < YesNoResponces.Length; i++)
                {
                    YesNoResponces[i] = AddPlayerNameToResponce(YesNoResponces[i], name);
                }
                // begin the game
                do
                {
                    PlayTheGame(GetAStory(file, numberOfStories));
                    // would they like to play again
                    Console.WriteLine("Would you like to play again?\nyes or no");
                    input = Console.ReadLine();
                    // if they didnt enter an acceptable value
                    while ((input.ToLower() != "yes") && (input.ToLower() != "no"))
                    {
                        yesNoCounter++;
                        // tell the player what they did wrong
                        if (yesNoCounter != 3)
                        {
                            Console.WriteLine("You must eventually type \"yes\" or \"no\" to move on with the program.");
                        }
                        else
                        {
                            // we have now have access to all of the yes no responces
                            Console.WriteLine(YesNoResponces[rand.Next(0, YesNoResponces.Length)]);
                            yesNoCounter = 0;
                        }
                        input = Console.ReadLine();
                    }
                } while (input.ToLower() != "no");
                // they dont want to play again
                Console.WriteLine("Goodbye");
            }
            // they dont want to play the game
            else
            {
                Console.WriteLine("Goodbye");
            }
            #endregion
        }
        /* Method: GetAStory
         * Purpose: Get a String array containing all of the words in the chosen story seperated by spaces
         * Restrictions: None
         */
        static String[] GetAStory(string file, int numberOfStories)
        {
            // general input
            string input = "";
            // what story was chosen
            int storyChoice = 0;
            // raw loaded story
            string resultString = "";
            // was the number entered valid
            bool valid = false;
            // ask them what story they want to play, repeat if they didnt enter an integer
            do
            {
                Console.WriteLine("Choose your story by chosing a number between 1 and " + numberOfStories);
                input = Console.ReadLine();
                // if they dont enter a number the defult is false
                Int32.TryParse(input, out storyChoice);
                // if they dont enter a valid number
                if(storyChoice < 1)
                {
                    valid = false;
                }
                else if(storyChoice > numberOfStories)
                {
                    valid = false;
                }
                else
                {
                    valid = true;
                }
            } while (!valid);
            // get the story
            using (StreamReader sr = File.OpenText(file))
            {
                for (int i = 0; i < storyChoice; i++)
                {
                    resultString = sr.ReadLine();
                }
            }
            // split the story up into words
            return resultString.Split(' ');
        }
        /* Method: PlayTheGame
         * Purpose: The player will be prompted for the words that need to be replaced in the entered story and output the edited story.
         * Restrictions: None
         */
        static void PlayTheGame(String[] splitStory)
        {
            // how many words does the player have to enter
            int replacementValues = 0;
            // a counter used to make sure the right word goes in the right place
            int replacementCounter = 0;
            // checks if the previous action was a new line
            bool newLine = true;
            // find out how many words the player will have to enter for this story
            foreach (string s in splitStory)
            {
                // all words that need to be replaced begin with {
                if (s[0] == '{')
                {
                    replacementValues++;
                }
            }
            // create a new array of the size of the number of words that need to be entered
            String[] replacementString = new String[replacementValues];
            // fill in the new array with the words that the player entered in the order that they appear in the story
            foreach (string s in splitStory)
            {
                // does the string need to be replaced
                if (s[0] == '{')
                {
                    replacementValues++;
                    string temp = "";
                    for (int i = 0; i < s.Length; i++)
                    {
                        // edit the display to remove the {}_,.!? chars from the players view wihtout editing the string or the txt file
                        if ((s[i] != '{') && (s[i] != '}') && (s[i] != '_') && (s[i] != ',') && (s[i] != '.') && (s[i] != '!') && (s[i] != '?'))
                        {
                            temp = temp + s[i];
                        }
                        if(s[i] == '_')
                        {
                            temp = temp + " ";
                        }
                    }
                    Console.WriteLine("Please enter a " + temp);
                    // assign the replacement to the replacement string array
                    replacementString[replacementCounter] = Console.ReadLine();
                    replacementCounter++;
                }
            }
            // start the replacement array counter over
            replacementCounter = 0;
            // begin the story
            foreach (string s in splitStory)
            {
                // if we find a control character then we replace it with the coresponding replacement string
                if (s[0] == '{')
                {
                    if (newLine)
                    {
                        Console.Write(replacementString[replacementCounter] + " ");
                        newLine = false;
                    }
                    else
                    {
                        Console.Write(" " + replacementString[replacementCounter]);
                    }
                        replacementCounter++;
                }
                // if we find a new line char then we enter a new line
                else if (s[0] == '\\' && s[1] == 'n')
                {
                    Console.WriteLine();
                    newLine = true;
                }
                // otherwise we get the word stored 
                else
                {
                    if(newLine)
                    {
                        Console.Write(s);
                        newLine = false;
                    }
                    else if (s[0] != '.')
                    {
                        Console.Write(" " + s);
                    }
                    else
                    {
                        Console.Write(s);
                    }
                }
            }
            Console.WriteLine();
        }
        /* Method: AddPlayerNameToResponce
         * Purpose: add the player name to the responces that are intended to include the player name
         * Restrictions: None
         */
        static string AddPlayerNameToResponce(string YesNoResponces, string name)
        {
            string temp = "";

            for (int i = 0; i < YesNoResponces.Length; i++)
            {
                // if we find _ in the string then that is where i want to enter the player name
                if (YesNoResponces[i] == '_')
                {
                    for (int j = 0; j < name.Length; j++)
                    {
                        temp = temp + name[j];
                    }
                }
                // otherwise i just want to enter the respronce as is
                else
                {
                    temp = temp + YesNoResponces[i];
                }
            }
            return temp;
        }
    }
}
