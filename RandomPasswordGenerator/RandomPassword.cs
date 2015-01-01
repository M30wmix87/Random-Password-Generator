using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomPasswordGenerator
{
    using System;
    using System.Security.Cryptography;
    /// <summary>
    /// Originally from http://www.obviex.com/samples/password.aspx
    /// This class can generate random passwords, which do not include ambiguous 
    /// characters, such as I, l, and 1. The generated password will be made of
    /// 7-bit ASCII symbols. Every four characters will include one lower case
    /// character, one upper case character, one number, and one special symbol
    /// (such as '%') in a random order. The password will always start with an
    /// alpha-numeric character; it will not start with a special symbol (we do
    /// this because some back-end systems do not like certain special
    /// characters in the first position).
    /// </summary>
    public class RandomPassword
    {
        // Define default min and max password lengths.
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH, DEFAULT_MAX_PASSWORD_LENGTH);
        }
        public static string Generate(int length, string PASSWORD_CHARS_SPECIAL)
        {
            return Generate(length, length, PASSWORD_CHARS_SPECIAL);
        }
        public static string Generate(int minLength, int maxLength)
        {
            // Define supported password characters divided into groups.
            // You can add (or remove) characters to (from) these groups.
            string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            string PASSWORD_CHARS_NUMERIC = "23456789";
            string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
            {
                return null;
            }

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][] 
            {
                PASSWORD_CHARS_LCASE.ToCharArray(),
                PASSWORD_CHARS_UCASE.ToCharArray(),
                PASSWORD_CHARS_NUMERIC.ToCharArray(),
                PASSWORD_CHARS_SPECIAL.ToCharArray()
            };

            int[] charsLeftInGroup = new int[charGroups.Length]; // Use this array to track the number of unused characters in each character group.
            for (int i = 0; i < charsLeftInGroup.Length; i++) // Initially, all characters in each group are not used.
            { 
                charsLeftInGroup[i] = charGroups[i].Length;
            }
            int[] leftGroupsOrder = new int[charGroups.Length]; // Use this array to track (iterate through) unused character groups.
            for (int i = 0; i < leftGroupsOrder.Length; i++) // Initially, all character groups are not used.
            { 
                leftGroupsOrder[i] = i;
            }

            byte[] randomBytes = new byte[4]; // Use a 4-byte array to fill it with random bytes and convert it then to an integer value.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes); // Generate 4 random bytes.
            int seed = BitConverter.ToInt32(randomBytes, 0); // Convert 4 bytes into a 32-bit integer value.
            Random random = new Random(seed);

            char[] password = null; // This array will hold password characters.

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
            {
                password = new char[random.Next(minLength, maxLength + 1)];
            }
            else
            { 
                password = new char[minLength];
            }

            int nextCharIdx; // Index of the next character to be added to password.
            int nextGroupIdx; // Index of the next character group to be processed.
            int nextLeftGroupsOrderIdx; // Index which will be used to track not processed character groups.
            int lastCharIdx; // Index of the last non-processed character in a group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1; // Index of the last non-processed group.

            for (int i = 0; i < password.Length; i++) // Generate password characters one at a time.
            {
                if (lastLeftGroupsOrderIdx == 0)
                {
                    nextLeftGroupsOrderIdx = 0;
                }
                else
                { 
                    nextLeftGroupsOrderIdx = random.Next(0,lastLeftGroupsOrderIdx);
                }
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx]; // Get the actual index of the character group, from which we will pick the next character.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1; // Get the index of the last unprocessed characters in this group.

                // If only one unprocessed character is left, pick it; otherwise, get a random character from the unused character list.
                if (lastCharIdx == 0)
                {
                    nextCharIdx = 0;
                }
                else
                { 
                    nextCharIdx = random.Next(0, lastCharIdx + 1);
                }
                password[i] = charGroups[nextGroupIdx][nextCharIdx]; // Add this character to the password.


                if (lastCharIdx == 0) // If we processed the last character in this group, start over.
                {
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                }
                else // There are more unprocessed characters left.
                {
                    // Swap processed character with the last unprocessed character so that we don't pick it until we process all characters in this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    charsLeftInGroup[nextGroupIdx]--; // Decrement the number of unprocessed characters in this group.
                }
                if (lastLeftGroupsOrderIdx == 0) // If we processed the last group, start all over.
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }
                else // There are more unprocessed groups left.
                {
                    // Swap processed group with the last unprocessed group so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    lastLeftGroupsOrderIdx--; // Decrement the number of unprocessed groups.
                }
            }
            return new string(password); // Convert password characters into a string and return the result.
        }
        public static string Generate(int minLength, int maxLength, string PASSWORD_CHARS_SPECIAL)
        {
            // Define supported password characters divided into groups. You can add (or remove) characters to (from) these groups.
            string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            string PASSWORD_CHARS_NUMERIC = "23456789";
            
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
            { 
                return null;
            }

            // Create a local array containing supported password characters grouped by types. You can remove character groups from this array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][] 
            {
                PASSWORD_CHARS_LCASE.ToCharArray(),
                PASSWORD_CHARS_UCASE.ToCharArray(),
                PASSWORD_CHARS_NUMERIC.ToCharArray(),
                PASSWORD_CHARS_SPECIAL.ToCharArray()
            };
            int[] charsLeftInGroup = new int[charGroups.Length]; // Use this array to track the number of unused characters in each character group.
            for (int i = 0; i < charsLeftInGroup.Length; i++) // Initially, all characters in each group are not used.
            { 
                charsLeftInGroup[i] = charGroups[i].Length;
            }
            int[] leftGroupsOrder = new int[charGroups.Length]; // Use this array to track (iterate through) unused character groups.
            for (int i = 0; i < leftGroupsOrder.Length; i++) // Initially, all character groups are not used.
            { 
                leftGroupsOrder[i] = i;
            }
            byte[] randomBytes = new byte[4]; // Use a 4-byte array to fill it with random bytes and convert it then to an integer value.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes); // Generate 4 random bytes.
            int seed = BitConverter.ToInt32(randomBytes, 0); // Convert 4 bytes into a 32-bit integer value.
            Random random = new Random(seed);
            char[] password = null; // This array will hold password characters.

            if (minLength < maxLength) // Allocate appropriate memory for the password.
            {
                password = new char[random.Next(minLength, maxLength + 1)];
            }
            else
            { 
                password = new char[minLength];
            }
            int nextCharIdx; // Index of the next character to be added to password.
            int nextGroupIdx; // Index of the next character group to be processed.
            int nextLeftGroupsOrderIdx; // Index which will be used to track not processed character groups.
            int lastCharIdx; // Index of the last non-processed character in a group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1; // Index of the last non-processed group.


            for (int i = 0; i < password.Length; i++) // Generate password characters one at a time.
            {
                if (lastLeftGroupsOrderIdx == 0)
                {
                    nextLeftGroupsOrderIdx = 0;
                }
                else
                { 
                    nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);
                }
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx]; // Get the actual index of the character group, from which we will pick the next character.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1; // Get the index of the last unprocessed characters in this group.
                // If only one unprocessed character is left, pick it; otherwise, get a random character from the unused character list.
                if (lastCharIdx == 0)
                {
                    nextCharIdx = 0;
                }
                else
                { 
                    nextCharIdx = random.Next(0, lastCharIdx + 1);
                }

                password[i] = charGroups[nextGroupIdx][nextCharIdx]; // Add this character to the password.
                if (lastCharIdx == 0) // If we processed the last character in this group, start over.
                {
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                }
                else // There are more unprocessed characters left.
                {
                    // Swap processed character with the last unprocessed character so that we don't pick it until we process all characters in this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    charsLeftInGroup[nextGroupIdx]--; // Decrement the number of unprocessed characters in this group.
                }
                if (lastLeftGroupsOrderIdx == 0) // If we processed the last group, start all over.
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                }
                else // There are more unprocessed groups left.
                {
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx) // Swap processed group with the last unprocessed group so that we don't pick it until we process all groups.
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    lastLeftGroupsOrderIdx--; // Decrement the number of unprocessed groups.
                }
            }
            return new string(password); // Convert password characters into a string and return the result.
        }
    }
}