// <copyright file="Fuzzers.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Fuzzers
    {
        private DataModel dataSetDebtDataGen;
        public Dictionary<string, FuzzMethod> FuzzedFields = new Dictionary<string, FuzzMethod>();
        private Random random;

        [Flags]
        public enum ConsumerFuzzedFields
        {
            None = 0,
            FirstName = 1,
            LastName = 2,
            SocialSecurityNumber = 4,
        }

        [Flags]
        public enum CreditCardFuzzedFields
        {
            None = 0,
            CreditCardNumber = 1
        }

        [Flags]
        public enum FuzzMethod
        {
            None = 0,
            StrippedDownToFirstCharacterOnly = 1,  // Moe Syzlak -> M Syzlak
            MissingRandomMiddleCharacter = 2,      // Moe Syzlak -> Moe Sylak
            AdjacentCharactersSwapped = 4,         // 12345 -> 12435
            RandomCharactersSwapped = 8,           // 12345 -> 52341
            NicknameSubstitution = 16,             // Johnathan Smith -> Jack Smith
            Swapped = 32,                          // Clancy Wiggum -> Wiggum Clancy
            Truncated = 64,                        // Joe Schmoe -> Joe Schm
            MissingFirstCharacter = 128,           // Joe Schmoe -> Joe chmoe
            MissingLastCharacter = 256,            // Joe Schmoe -> Joe Schmoe
            CompletelyNewValue = 512,              // Joe Schmoe -> Joe Donuts
            CompletelyNewValueHyphenated = 1024,   // Jill Schmoe -> Jill Schmoe-Syzlak (poor girl)
            RandomCharacterAdded = 2048            // Bart Simpson -> Bart Simptson
        }

        [Flags]
        public enum FuzzSize
        {
            SingleCharacter = 0,
            TwoCharacters = 1,
            ThreeCharacters = 2,
            RandomCharacters = 4,
            AllCharacters = 8
        }

        [Flags]
        public enum StringFuzzType
        {
            Insertion = 1,     // Hans Moleman -> Hanns Moleman
            Substitution = 2,  // Selma Bouvier -> Selma Boovier
            Deletion = 4,      // Milhouse VanHouten -> Milhouse Houten
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fuzzers"/> class.
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <param name="dataSetDebtDataGen">The DataSet used to generate the debt holder data.</param>
        /// <param name="lastNameList">A list of last names.</param>
        public Fuzzers(Random random, DataModel dataSetDebtDataGen)
        {
            // Initialize the object.
            this.random = random;
            this.dataSetDebtDataGen = dataSetDebtDataGen;
        }

        /// <summary>
        ///  Walks through the ConsumerTrust records and fuzzes them according to the configurable settings
        /// </summary>
        public void FuzzConsumerTrust(List<string> lastNameList)
        {
            // Initialize the counters.
            Int32 fuzzCount = 0;
            Int32 nicknameAttempts = 0;

            // do until we have reached the number of records that are supposed to be fuzzed
            while (fuzzCount < Properties.Settings.Default.TrustSideFuzzCount)
            {
                // Get a random Credit Card out of the TrustSideCreditCard table
                DataModel.TrustCreditCardRow trustCreditCardRow =
                    this.dataSetDebtDataGen.TrustCreditCard[this.random.Next(this.dataSetDebtDataGen.TrustCreditCard.Count)];

                // Ensure that the credit card is a matching one. The reason we fuzz only match candidates is so we can compare
                // the ACTUAL results of the server's actual matching algorithm to our EXPECTED value of matches.
                if (!this.dataSetDebtDataGen.RealCreditCard[trustCreditCardRow.RealCreditCardId].IsMatched)
                    continue;

                // Ensure that the Credit Card we retrieved (OR its Consumer) has not already been fuzzed by either the 
                // Common Fuzz heuristics or here in this method.  IF FuzzedFields is any value other than "none" the 
                // record has been fuzzed.
                if (trustCreditCardRow.FuzzedFields != Convert.ToInt32(CreditCardFuzzedFields.None) ||
                    trustCreditCardRow.TrustSideConsumerRow.FuzzedFields != Convert.ToInt32(ConsumerFuzzedFields.None))
                    continue;
                              
                // *** If we get to this point, we have a candiate for fuzzing ***

                // Get the Credit Card's corresponding consumer 
                DataModel.TrustSideConsumerRow trustConsumer = trustCreditCardRow.TrustSideConsumerRow;
                
                // fulfill the 'constant' requirements first 
                if (!TrustNickNamePercentageIsMet(fuzzCount))
                {
                    // We're not always guaranteed that a particular consumer is going to have a shortened first name
                    // or nickname.  Therefore we use this clause to force a known percentage to be fulfilled by picking 
                    // through and finding the consumer records that actually do have them.

                    string nickname;

                    if (TryGetRealConsumerNickName(trustConsumer.RealConsumerId, out nickname))
                    {
                        trustConsumer.FirstName = nickname;
                        trustConsumer.FuzzedFields = Convert.ToInt32(ConsumerFuzzedFields.FirstName);
                        trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.NicknameSubstitution);
                    }
                    else
                    {
                        // This consumer did not have a nickname so go get another one and try again

                        // We may not have enough first names to satisfy the nickname criteria. A 'reasonable enough' number 
                        // of attempts is three times the size of the TrustSideConsumerTable.  
                        // Note:  [skt] I've run across some seeds that cause this exception to be thrown.  This is relatively
                        //        rare.  The easiest thing to do is just pick another random seed and gen the data again.
                        if (++nicknameAttempts > this.dataSetDebtDataGen.TrustSideConsumer.Count * 3)
                        {
                            throw new SystemException(
                                String.Format(
                                    " Data Generation Failed. Exceeded the number of valid attempts ({0}) to reach nickname percentage.",
                                    this.dataSetDebtDataGen.TrustSideConsumer.Count *  3));
                        }

                        continue;
                    }
                }
                else // fulfill the remaining fuzzCount with different variations of fuzzed data
                {
                    // roll the dice to see what gets fuzzzed
                    switch (this.random.Next(1, 7)) 
                    {
                            // one third of the time we will fuzz the SSN
                        case 1:
                        case 2:
                            {
                                // 30 percent of the time we fuzz the SSN, it will be character subtraction
                                if (this.random.Next(100).IsWithinRange(0, 29))
                                {
                                    // flip a coin to determine whether or not we omit a middle character or one on the end
                                    if (this.random.Next(100).IsWithinRange(0, 49))
                                    {
                                        // omit a character in the middle
                                        FuzzSocialSecurityNumber(trustConsumer, FuzzMethod.MissingRandomMiddleCharacter, this.random);
                                        trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.MissingRandomMiddleCharacter);
                                    }
                                    else
                                    {
                                        // omit an endpoint character: Flip a coin to determine which end
                                        if (this.random.Next(100).IsWithinRange(0, 49))
                                        {
                                            // first char
                                            FuzzSocialSecurityNumber(trustConsumer, FuzzMethod.MissingFirstCharacter, this.random);
                                            trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.MissingFirstCharacter);
                                        }
                                        else
                                        {
                                            // last char
                                            FuzzSocialSecurityNumber(trustConsumer, FuzzMethod.MissingLastCharacter, this.random);
                                            trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.MissingLastCharacter);
                                        }
                                    }
                                }
                                // 70 percent of the time we will fat finger this field
                                else
                                {
                                    // determine the fat-finger-technique
                                    if (this.random.Next(100).IsWithinRange(0, 74))
                                    {
                                        // 'accidentally' swapping characters 75% of the time
                                        FuzzSocialSecurityNumber(trustConsumer, FuzzMethod.AdjacentCharactersSwapped, this.random);
                                        trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.AdjacentCharactersSwapped);
                                    }
                                    else
                                    {
                                        // adding an extra 'random' character 25% of the time
                                        FuzzSocialSecurityNumber(trustConsumer, FuzzMethod.RandomCharacterAdded, this.random);
                                        trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.RandomCharacterAdded);
                                    }
                                }

                                // mark that the SSN was fuzzed
                                trustConsumer.FuzzedFields = Convert.ToInt32(ConsumerFuzzedFields.SocialSecurityNumber);
                                break;
                            }

                            // one third of the time we will fuzz the matched consumer's CCN
                        case 3:
                        case 4:
                            {
                                // 25 percent of the time we fuzz the CCN, it will be character subtraction
                                if (this.random.Next(100).IsWithinRange(0, 24))
                                {
                                    // flip a coin to determine whether or not we omit a middle character or one on the end
                                    if (this.random.Next(100).IsWithinRange(0, 49))
                                    {
                                        // omit a character in the middle
                                        FuzzCreditCardNumber(trustCreditCardRow, FuzzMethod.MissingRandomMiddleCharacter, this.random);
                                        trustCreditCardRow.FuzzMethod = Convert.ToInt32(FuzzMethod.MissingRandomMiddleCharacter);
                                    }
                                    else
                                    {
                                        // omit an endpoint character: Flip a coin to determine which end
                                        if (this.random.Next(100).IsWithinRange(0, 49))
                                        {
                                            // first char
                                            FuzzCreditCardNumber(trustCreditCardRow, FuzzMethod.MissingFirstCharacter, this.random);
                                            trustCreditCardRow.FuzzMethod = Convert.ToInt32(FuzzMethod.MissingFirstCharacter);
                                        }
                                        else
                                        {
                                            // last char
                                            FuzzCreditCardNumber(trustCreditCardRow, FuzzMethod.MissingLastCharacter, this.random);
                                            trustCreditCardRow.FuzzMethod = Convert.ToInt32(FuzzMethod.MissingLastCharacter);
                                        }
                                    }                                        
                                }

                                // 75 percent of the time we will fat finger this field:
                                else
                                {
                                    // determine the fat-finger-technique
                                    if (this.random.Next(100).IsWithinRange(0, 74))
                                    {
                                        // 'accidentally' swapping characters 75% of the time
                                        FuzzCreditCardNumber(trustCreditCardRow, FuzzMethod.AdjacentCharactersSwapped, this.random);
                                        trustCreditCardRow.FuzzMethod = Convert.ToInt32(FuzzMethod.AdjacentCharactersSwapped);
                                    }
                                    else
                                    {
                                        // adding an extra 'random' character 25% of the time
                                        FuzzCreditCardNumber(trustCreditCardRow, FuzzMethod.RandomCharacterAdded, this.random);
                                        trustCreditCardRow.FuzzMethod = Convert.ToInt32(FuzzMethod.RandomCharacterAdded);
                                    }
                                }

                                // mark the credit card to say that its CCN has been fuzzed
                                trustCreditCardRow.FuzzedFields = Convert.ToInt32(CreditCardFuzzedFields.CreditCardNumber);

                                break;
                            }

                            // one sixth of the time we will fuzz both
                        case 5:
                            {
                                // fat fingering - accidentally swapping characters
                                FuzzSocialSecurityNumber(trustConsumer, FuzzMethod.AdjacentCharactersSwapped, this.random);
                                FuzzCreditCardNumber(trustCreditCardRow, FuzzMethod.AdjacentCharactersSwapped, this.random);

                                // mark that both SSN and CCN were fuzzed
                                trustConsumer.FuzzedFields = Convert.ToInt32(ConsumerFuzzedFields.SocialSecurityNumber);
                                trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.AdjacentCharactersSwapped);

                                trustCreditCardRow.FuzzedFields = Convert.ToInt32(CreditCardFuzzedFields.CreditCardNumber);
                                trustCreditCardRow.FuzzMethod = Convert.ToInt32(FuzzMethod.AdjacentCharactersSwapped);
                                
                                break;
                            }

                            // One-sixth of the time we will muck with the consumer's name. 
                        case 6:
                            {
                                // If both FN/LN fields are empty...
                                if (trustConsumer.FirstName == null && trustConsumer.LastName == null)
                                {
                                    // add a couple this.random characters into the LN field
                                    trustConsumer.LastName = Convert.ToString(DataGenHelpers.GetRandomAlphaNumericCharacter(this.random));
                                    trustConsumer.LastName += Convert.ToString(DataGenHelpers.GetRandomAlphaNumericCharacter(this.random));
                                    
                                    // mark that the last name has been fuzzed with a whole new name
                                    trustConsumer.FuzzedFields = Convert.ToInt32(ConsumerFuzzedFields.LastName);
                                    trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.CompletelyNewValue);
                                }

                                // Flip a 3-sided coin (http://www.statisticool.com/3sided.htm) to determine how we muck with the name 
                                switch (this.random.Next(1, 4))
                                {
                                    case 1: // First Name Muck
                                        {
                                            if (trustConsumer.FirstName == null)
                                            {
                                                // If the field is empty, add a random character 
                                                trustConsumer.FirstName =
                                                    Convert.ToString(DataGenHelpers.GetRandomAlphaNumericCharacter(this.random));
                                                trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.CompletelyNewValue);
                                            }
                                            else if (trustConsumer.FirstName.Length == 1)
                                            {
                                                // FN is already one char in length, so swap it with another (i.e, a screwed-up first initial)
                                                trustConsumer.FirstName = 
                                                    Convert.ToString(DataGenHelpers.RandomCharacterReplacement(trustConsumer.FirstName[0], this.random));
                                                trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.CompletelyNewValue);
                                            }
                                            else
                                            {
                                                // first name is > 1 characters, strip it down to just one
                                                trustConsumer.FirstName = trustConsumer.FirstName.Substring(0, 1);
                                                trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.StrippedDownToFirstCharacterOnly);
                                            }
                                            // mark that the firstname has been fuzzed
                                            trustConsumer.FuzzedFields = Convert.ToInt32(ConsumerFuzzedFields.FirstName);
                                            break;
                                        }
                                    case 2: // Last Name Muck
                                        {
                                            // Replace the last name altogether (someone got married/divorced/etc...)
                                         
                                            String newLastName = lastNameList[this.random.Next(0, lastNameList.Count - 1)];
                                            while (newLastName == trustConsumer.LastName)
                                            {
                                                // if we happen to get the same name, try again
                                                newLastName = lastNameList[this.random.Next(0, lastNameList.Count - 1)];
                                            }

                                            // Flip a coin to see if we hyphenate or replace it altogether
                                            //   [skt] can refine this by gender if the need arises (put a gender column in the consumer tables)
                                            if (this.random.Next(100).IsWithinRange(0, 49))
                                            {
                                                // hyphenate 
                                                trustConsumer.LastName = String.Format("{0}-{1}", trustConsumer.LastName, 
                                                   newLastName[0] + newLastName.Substring(1, newLastName.Length - 1).ToLower());
                                                trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.CompletelyNewValueHyphenated);
                                            }
                                            else
                                            {
                                                // new name, no hyphenation
                                                trustConsumer.LastName = newLastName.ToLower();
                                                trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.CompletelyNewValue);
                                            }

                                            // mark that the last name has been fuzzed with a whole new name
                                            trustConsumer.FuzzedFields = Convert.ToInt32(ConsumerFuzzedFields.LastName);
                                            break;
                                        }
                                    case 3: // swap FN and LN
                                        {
                                            string tmp = trustConsumer.FirstName;
                                            trustConsumer.FirstName = trustConsumer.LastName;
                                            trustConsumer.LastName = tmp;

                                            // mark that both FN and LN were fuzzed
                                            trustConsumer.FuzzedFields = Convert.ToInt32(ConsumerFuzzedFields.FirstName) |
                                                                         Convert.ToInt32(ConsumerFuzzedFields.LastName);

                                            trustConsumer.FuzzMethod = Convert.ToInt32(FuzzMethod.Swapped);

                                            break;
                                        }
                                }

                                break;
                            }
                        }
                }

                // increment the count of how many records we fuzzed
                ++fuzzCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realConsumerId"></param>
        /// <param name="nickname"></param>
        /// <returns></returns>
        private Boolean TryGetRealConsumerNickName(int realConsumerId, out string nickname)
        {
            nickname = this.dataSetDebtDataGen.RealConsumer[realConsumerId].Nickname;
            
            if (nickname != null)
                return true;

            return false;
        }

        /// <summary>
        /// If the fuzzCount percentage is less than the percentage we are looking for on the Trust side, return false.
        /// otherwise we have met the percentage we are looking for so return true
        /// </summary>
        /// <param name="fuzzCount"></param>
        /// <returns></returns>
        private Boolean TrustNickNamePercentageIsMet(int fuzzCount)
        {
            if (Properties.Settings.Default.TrustSideFuzzCount != 0) // avoid a divbyzero
            {
                if (((double) fuzzCount/(double) Properties.Settings.Default.TrustSideFuzzCount) <
                    (double) Properties.Settings.Default.TrustSideFuzzNicknamePercentage / 100)
            
                    return false;
            }

            return true;
        }

        /// <summary>
        ///  Walks through the ConsumerDebt records and fuzzes them
        /// </summary>
        public void FuzzConsumerDebt()
        {

            // Just fuzz the First Name 
            if (Properties.Settings.Default.NumberOfConsumerRecords == 1)
            {
                this.dataSetDebtDataGen.TrustSideConsumer[0].LastName = "Doah";
            }

            // remove some fields that the trust side usually will have rather than the debt side:
            //  Middle, Suffix, etc...
            //      For fields not part of the matching algorithm, maybe we need a different method (CleanseDebtHolderData?) 
            //      because it's for aesthetics rather than fuzzing to test the matching

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        public void FuzzConsumerTrustAndConsumerDebt()
        {
           
        }

        /// <summary>
        /// Fuzzes the SSN of the passed in DEBT-side consumer according to the passed-in fuzz rules 
        /// </summary>
        /// <param name="debtConsumer"></param>
        /// <param name="fuzzMethod"></param>
        /// <param name="random"></param>
        private void FuzzSocialSecurityNumber (DataModel.DebtSideConsumerRow debtConsumer, FuzzMethod fuzzMethod, Random random)
        {
            debtConsumer.SocialSecurityNumber = 
                FuzzSocialSecurityNumber(debtConsumer.SocialSecurityNumber, fuzzMethod, random);
        }

        /// <summary>
        /// Fuzzes the SSN of the passed in TRUST-side consumer according to the passed-in fuzz rules 
        /// </summary>
        /// <param name="trustConsumer"></param>
        /// <param name="fuzzMethod"></param>
        /// <param name="random"></param>
        private void FuzzSocialSecurityNumber(DataModel.TrustSideConsumerRow trustConsumer, FuzzMethod fuzzMethod, Random random)
        {
            trustConsumer.SocialSecurityNumber = 
                FuzzSocialSecurityNumber(trustConsumer.SocialSecurityNumber, fuzzMethod, random);
        }

        /// <summary>
        /// Fuzz the passed in SSN string
        /// </summary>
        /// <param name="socialSecurityNumber"></param>
        /// <returns></returns>
        private string FuzzSocialSecurityNumber(String socialSecurityNumber, FuzzMethod fuzzMethod, Random random)
        {
            string fuzzedSsn = null;

            // if all the characters in the string are the same, this is essentially a no-op
            if (socialSecurityNumber.HasAllTheSameCharacters())
                return socialSecurityNumber;

            // instance of the random class used for mixing up the list.
            if (this.random == null)
            {
                this.random = new Random(DateTime.Now.Millisecond);
            }

            // remove the dashes in the ssn
            char[] rawSsn = DataGenHelpers.GetRawSsn(socialSecurityNumber).ToCharArray();

            // Initialize string fuzzer 
            StringFuzzer fuzzer = new StringFuzzer(this.random);

            // Fuzz Accordingly:
            switch (fuzzMethod)
            {
                case (FuzzMethod.AdjacentCharactersSwapped): 
                    {
                        // Starting from a random position in a string, swap two unique and adjacent characters in that string. 
                        // If the string happens to have all the same characters, there is not much we can do 
                        fuzzer = new StringFuzzer(this.random);
                        fuzzedSsn = fuzzer.SwapTwoUniqueAdjacentCharacters(rawSsn);
                        fuzzedSsn = DataGenHelpers.BuildFormattedSsn(fuzzedSsn);
                        break;
                    }

                case (FuzzMethod.MissingRandomMiddleCharacter):
                    {
                        // simply remove a character from a random position within the SSN. The two end characters are preserved
                        Char firstChar = rawSsn.First();
                        Char lastChar = rawSsn.Last();

                        // delete a random character between the begining and end characters of the string   
                        String midSsn = fuzzer.DeleteRandomCharacter(new String(rawSsn, 1, rawSsn.Length - 2));

                        // put the pieces back together again
                        fuzzedSsn = String.Format("{0}{1}{2}", firstChar, midSsn, lastChar);
                        fuzzedSsn = DataGenHelpers.BuildFormattedSsn(fuzzedSsn);
                        break;
                    }
                case (FuzzMethod.MissingFirstCharacter):
                    {
                        fuzzedSsn = socialSecurityNumber.Remove(0, 1);
                        break;
                    }
                case(FuzzMethod.MissingLastCharacter):
                    {
                        fuzzedSsn = socialSecurityNumber.Remove(socialSecurityNumber.Length - 1, 1);
                        break;                     
                    }
                case (FuzzMethod.RandomCharacterAdded):
                    {
                        fuzzedSsn = fuzzer.InsertRandomCharacter(socialSecurityNumber);
                        break;
                    }
            }

            return fuzzedSsn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trustCreditCardRow"></param>
        /// <param name="fuzzMethod"></param>
        /// <param name="random"></param>
        private void FuzzCreditCardNumber(DataModel.TrustCreditCardRow trustCreditCardRow, FuzzMethod fuzzMethod, Random random)
        {
            trustCreditCardRow.OriginalAccountNumber = 
                FuzzCreditCardNumber(trustCreditCardRow.OriginalAccountNumber, fuzzMethod, this.random);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debtCreditCardRow"></param>
        /// <param name="fuzzMethod"></param>
        /// <param name="random"></param>
        private void FuzzCreditCardNumber(DataModel.DebtCreditCardRow debtCreditCardRow, FuzzMethod fuzzMethod, Random random)
        {
            debtCreditCardRow.OriginalAccountNumber =
                FuzzCreditCardNumber(debtCreditCardRow.OriginalAccountNumber, fuzzMethod, this.random);
        }


        /// <summary>
        /// Fuzzes the passed in CCN
        /// </summary>
        /// <param name="creditCardNumber"></param>
        /// <param name="fuzzRules"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public String FuzzCreditCardNumber(String creditCardNumber, FuzzMethod fuzzMethod, Random random)
        {
            String fuzzedCCN = null;
            StringFuzzer fuzzer = new StringFuzzer(this.random);

            switch (fuzzMethod)
            {
                case (FuzzMethod.AdjacentCharactersSwapped):
                    {
                        // swap two adjacent/Unique characters (no fuzzRules yet)
                        fuzzedCCN = fuzzer.SwapTwoUniqueAdjacentCharacters(creditCardNumber);
                        break; 
                    }
                case (FuzzMethod.MissingRandomMiddleCharacter):
                    {
                        // simply remove a character from a random position within the CCN. The two end characters are preserved
                        Char firstChar = creditCardNumber.First();
                        Char lastChar = creditCardNumber.Last();

                        // delete a random character between the begining and end characters of the string                       
                        String midCcn = fuzzer.DeleteRandomCharacter(creditCardNumber.Substring(1, creditCardNumber.Length - 2));

                        // put the pieces back together again
                        fuzzedCCN = String.Format("{0}{1}{2}", firstChar, midCcn, lastChar);
                        break;
                    }
                case (FuzzMethod.MissingFirstCharacter):
                    {
                        fuzzedCCN = creditCardNumber.Remove(0, 1);
                        break;
                    }
                case (FuzzMethod.MissingLastCharacter):
                    {
                        fuzzedCCN = creditCardNumber.Remove(creditCardNumber.Length - 1, 1);
                        break;
                    }
                case (FuzzMethod.RandomCharacterAdded):
                    {
                        fuzzedCCN = fuzzer.InsertRandomCharacter(creditCardNumber);
                        break;
                    }
            }

            return fuzzedCCN;
        }
    
    } // Class WindoMain



    /// <summary>
    /// 
    /// </summary>
    public sealed class StringFuzzer
    {
        private Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringFuzzer"/> class.
        /// </summary>
        /// <param name="random"></param>
        public StringFuzzer(Random random)
        {
            // Initalize the object.
            this.random = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Randomly swap two adjacent characters in a string. Does not distinguish identical characters
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public String SwapTwoAdjacentCharacters(Char[] source)
        {
            return SwapTwoAdjacentCharacters(new String(source));
        }

        /// <summary>
        /// Randomly swap two adjacent characters in a string. Does not distinguish identical characters
        /// </summary>
        /// <returns></returns>
        public String SwapTwoAdjacentCharacters(String source)
        {
            int i;
            int j;

            Char[] chars = source.ToCharArray();

            i = this.random.Next(0, source.Length);

            // if we pick the last character in the string, use the first
            // character in the string to swap with.
            if (i == source.Length - 1)
            {
                j = 0;
            }
            else
            {
                j = i + 1; //get the next character 
            }

            Char tmp = chars[j];
            chars[j] = chars[i];
            chars[i] = tmp;

            return new String(chars);
        }

        /// <summary>
        /// Randomly swap two unique characters in a string. If the string has all the same characters, then this 
        /// is essentially a no-op
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public String SwapTwoUniqueCharacters(Char[] source)
        {
            return SwapTwoUniqueCharacters(new String(source));
        }

        /// <summary>
        /// Randomly swap two uniqe characters in a string.  If the string has all the same characters, then this 
        /// is essentially a no-op
        /// </summary>
        /// <returns></returns>
        public String SwapTwoUniqueCharacters(String source)
        {
            // if all the characters are the same, just return the original source
            if (source.HasAllTheSameCharacters())
                return source;

            // randomly pick a couple characters
            int i = this.random.Next(0, source.Length);
            int j = this.random.Next(0, source.Length);

            Char[] chars = source.ToCharArray();

            // As long as the string has at least two unique characters, we will bang out of this loop
            // [skt] this is kind of inefficient
            while (chars[i] == chars[j])
            {                
                j = this.random.Next(0, source.Length);
            }
         
            // swap the characters
            Char tmp = chars[j];
            chars[j] = chars[i];
            chars[i] = tmp;

            return new String(chars);
        }

        /// <summary>
        /// Starting from a random element in a string, swap two unique and adjacent characters in that string. 
        /// If the string has all the same characters, then this is essentially a no-op
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public String SwapTwoUniqueAdjacentCharacters(Char[] source)
        {
            return SwapTwoUniqueAdjacentCharacters(new String(source));
        }

        /// <summary>
        /// Starting from a random element in a string, swap two unique and adjacent characters in that string. 
        /// If the string has all the same characters, then this is essentially a no-op
        /// </summary>
        /// <returns></returns>
        public String SwapTwoUniqueAdjacentCharacters(String source)
        {
            int positionA, positionB;

            // if all the characters are the same, just return the original source
            if (source.HasAllTheSameCharacters())
                return source;

            Char[] chars = source.ToCharArray();

            // Randomly pick a character to start with
            positionA = this.random.Next(0, source.Length - 1);
                
            // pick the next/adjacent character (if at the end of the string, start at the beginging)
            positionB = ((positionA == source.Length - 1) ? 0 : positionA + 1);          

            // while the adjacent characters are equivalent
            while (chars[positionA] == chars[positionB])
            {
                // increment the starting position (if at the end of the string, start at the beginging)
                positionA = ((positionA + 1 > source.Length - 1) ? positionA = 0 : positionA + 1);

                // pick the next/adjacent character (if at the end of the string, start at the beginging)
                positionB = ((positionA == source.Length - 1) ? 0 : positionA + 1);

                // As long as the string has at least two unique characters, we will bang out of this loop.
                // And per the check at the begining of this method, there will be at least two unique
                // characters in the string.

                // [skt] this is kind of inefficient
            }

            // swap the characters
            Char tmp = chars[positionB];
            chars[positionB] = chars[positionA];
            chars[positionA] = tmp;

            return new String(chars);

        }

        /// <summary>
        /// Deletes a random character from the passed in source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public String DeleteRandomCharacter(Char[] source)
        {
            return DeleteRandomCharacter(new String(source));         
        }

        /// <summary>
        /// Deletes a random character from the passed in source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public String DeleteRandomCharacter(String source)
        {
            // Randomly pick a character to remove
            StringBuilder sb = new StringBuilder(source);
            sb.Remove(this.random.Next(0, source.Length - 1), 1);

            return sb.ToString();
        }

        /// <summary>
        /// Insert a random character in the passed in source
        /// </summary>
        /// <param name="soure"></param>
        /// <returns></returns>
        public String InsertRandomCharacter(String source)
        {
            // Ramdonly pick a spot to insert at.  Weigh the positional end point a bit heavier than the overall 
            // length of the source string so we get some more chances that characters get appended 
            // to the end of the string.
            Int32 pos = this.random.Next(0, source.Length + 1);

            // Pick a random character to insert
            Char c = DataGenHelpers.GetRandomAlphaNumericCharacter(this.random);

            StringBuilder sb = new StringBuilder(source);

            if (pos >= source.Length)
            {
                sb.Append(c);
            }
            else
            {
                sb.Insert(pos, c);
            }

            return sb.ToString();
        }
    }
}