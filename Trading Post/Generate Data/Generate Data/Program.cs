// <copyright file="Program.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;
    using DarkBond.TradingPost.Data.Properties;

    class Program
    {
        // Private Instance Fields
        private List<string> streetNameList = new List<string>();
        private List<string> address2List = new List<string>();
        private List<string> salutationList = new List<string>();
        private List<Location> locationList = new List<Location>();
        private List<string> femaleNameList = new List<string>();
        private List<string> femaleSalutationList = new List<string>();
        private List<string> maleNameList = new List<string>();
        private List<string> maleSalutationList = new List<string>();
        private List<string> lastNameList = new List<string>();
        private List<string> maleSuffixList = new List<string>();
        private List<string> phoneNumberList = new List<string>();
        private Dictionary<string, string> nicknameList = new Dictionary<string, string>();
        private Dictionary<int, int> trustSideMapOfRealConsumerIds = new Dictionary<int, int>();
        private Dictionary<int, string> debtSideMapOfRealConsumerIds = new Dictionary<int, string>();
        private Dictionary<int, int> debtSideMapOfRealCreditCardIds = new Dictionary<int, int>();
        private Dictionary<int, int> trustSideMapOfRealCreditCardIds = new Dictionary<int, int>();
        private List<int> consumerTrustIds = new List<int>();
        private List<int> consumerDebtIds = new List<int>();
        private DataModel dataModel = new DataModel();
        private DebtNegotiator[] dataSetDebtNegotiators;
        private DebtHolder[] dataSetDebtHolders;
        private Random random;
        private Fuzzers fuzzers;

        /// <summary>
        /// Creates the data files for Debt Holder and Debt Negotiator organizations
        /// </summary>
        static void Main(string[] args)
        {
            // Run the main program.
            Program program = new Program();
            program.Run();
        }

        /// <summary>
        /// Generates the data for testing.
        /// </summary>
        private void Run()
        {
            // Initialize a global random number generator.
            this.random = new Random(Settings.Default.UseTimeBasedRandomSeed ? DateTime.Now.Millisecond : 0);

            // This class will obfuscate the data to make it look more like real-world flat files.
            this.fuzzers = new Fuzzers(this.random, this.dataModel);

            // Make sure we start with a clean output directory.
            this.ResetOutputDirectory();

            // Validate the config data.
            this.ValidateConfigurationData();

            // Initialize the data sets for both sides of the transaction.
            this.dataSetDebtNegotiators = new DebtNegotiator[Settings.Default.NumberOfImportFilesPerOrganization];
            this.dataSetDebtHolders = new DebtHolder[Settings.Default.NumberOfImportFilesPerOrganization];
            for (int index = 0; index < Settings.Default.NumberOfImportFilesPerOrganization; index++)
            {
                this.dataSetDebtNegotiators[index] = new DebtNegotiator();
                this.dataSetDebtHolders[index] = new DebtHolder();
            }

            // Build cached lists based on the flat files 
            this.ReadStreetNames();
            this.ReadAddress2();
            this.ReadLocations();
            this.ReadSalutations();
            this.ReadNicknames();
            this.ReadLastNames();
            this.ReadMaleNames();
            this.ReadMaleSalutations();
            this.ReadFemaleNames();
            this.ReadFemaleSalutations();
            this.ReadMaleSuffixes();
            this.ReadPhoneNumbers();

            // Build a collection of random credit card issuers.
            this.BuildCreditCardIssuerTable();

            // Build the Consumer Table
            this.BuildConsumerTable();

            //Build the organization table
            this.BuildOrganizationTable();

            // Build the user table
            this.BuildUserTable();

            // Build the Consumer Trust table
            this.BuildConsumerTrustTable();

            // Throw if we need more matches than there are credit cards available to match on.  We can't do this 
            // check until after we build the Trust-Side credit card table since it's dynamically built with a 
            // random amount of credit cards, the number of which is at least the number of trust-side consumers
            if (Settings.Default.NumberOfMatches > this.dataModel.TrustCreditCard.Count)
            {
                throw new SystemException("Number of Matches cannot be greater than TrustCreditCard.Count");
            }

            // Build the Consumer Debt table
            this.BuildConsumerDebtTable();

            // Fuzz both Trust and Debt side data for the same consumer record 
            if (Settings.Default.CommonFuzzCount > 0)
            {
                this.fuzzers.FuzzConsumerTrustAndConsumerDebt();
            }

            // Fuzz the Trust-side data 
            if (Settings.Default.TrustSideFuzzCount > 0)
            {
                this.fuzzers.FuzzConsumerTrust(this.lastNameList);
            }

            // Fuzz the Debt-side data
            if (Settings.Default.DebtSideFuzzCount > 0)
            {
                this.fuzzers.FuzzConsumerDebt();
            }

            // Sanity check the match count 
            this.ValidateMatchCounts();

            // Write the generated and cleaned up values to XML files.
            this.WriteConsumer();

            // Update the organization table with working order counts based on the number and types 
            // of organizations that are in the table
            this.UpdateOrganizationTableWorkingOrderCounts();

            // Shuffle the lists of ConsumerTrust and ConsumerDebt IDs. This will allow us to have a 'random' 
            // distribution of the generated Trust and Debt records per Organization
            DataGenHelpers.RandomlyShuffleList(this.consumerTrustIds, this.random);
            DataGenHelpers.RandomlyShuffleList(this.consumerDebtIds, this.random);

            // As we build the ConsumerDebt and ConsumerTrust output files, we need to build the CreditCard output file in 
            // parallel. This is because (1) we need to keep the links between the credit cards and their respective Debt 
            // and Trust records and (2) the tenant (aka Organization) needs to be associated with both the security record
            // (ConsumerDebt or ConsumerTrust) and the credit cards.
            XDocument creditCardOutputFile = this.InitializeCreditCardOutputFile();
            this.WriteConsumerDebtOutputData(creditCardOutputFile);
            this.WriteConsumerTrustOutputData(creditCardOutputFile);
            this.WriteCreditCardDataFile(creditCardOutputFile);

            // Write out the Flattened XML files used to test the Import facility
            this.WriteFlatImportFiles();
        }

        /// <summary>
        /// Makes sure we start with a empty output directory
        /// </summary>
        private void ResetOutputDirectory()
        {
            // Make sure output directory exists
            string outputDir = Environment.CurrentDirectory + "\\" + Settings.Default.DataOutputLocation;
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Clean out the directory
            DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                File.SetAttributes(outputDir + fileInfo.Name, FileAttributes.Normal);
                File.Delete(outputDir + fileInfo.Name);
            }
        }

        /// <summary>
        /// Read in the Address 2 data from the resources
        /// </summary>
        private void ReadAddress2()
        {
            // Read the second line of an address into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.Address2Input)))
            {
                while (!streamReader.EndOfStream)
                    this.address2List.Add(streamReader.ReadLine());
            }
        }

        /// <summary>
        /// Read in the Male names from the resources
        /// </summary>
        private void ReadMaleNames()
        {
            // Read the male names into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.MaleNamesInput)))
            {
                while (!streamReader.EndOfStream)
                    this.maleNameList.Add(streamReader.ReadLine());
            }
        }

        /// <summary>
        /// Read in the Male salutations from the resources
        /// </summary>
        private void ReadMaleSalutations()
        {
            // Read the male salutations into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.MaleSalutationInput)))
            {
                while (!streamReader.EndOfStream)
                    this.maleSalutationList.Add(streamReader.ReadLine());
            }
        }

        /// <summary>
        /// Read in the Female salutations from the resources
        /// </summary>
        private void ReadFemaleSalutations()
        {
            // Read the female salutations into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.FemaleSalutationInput)))
            {
                while (!streamReader.EndOfStream)
                    this.femaleSalutationList.Add(streamReader.ReadLine());
            }
        }

        /// <summary>
        /// Read in the Female names from the resources
        /// </summary>
        private void ReadFemaleNames()
        {
            // Read the female names into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.FemaleNamesInput)))
            {
                while (!streamReader.EndOfStream)
                    this.femaleNameList.Add(streamReader.ReadLine());
            }
        }

        /// <summary>
        /// Read in the Suffixes from the resources
        /// </summary>
        private void ReadMaleSuffixes()
        {
            // Read the male suffixes into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.MaleSuffixInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    this.maleSuffixList.Add(streamReader.ReadLine());
                }
            }
        }

        /// <summary>
        /// Read in the Phone Numbers from the resources
        /// </summary>
        private void ReadPhoneNumbers()
        {
            // Read the random phone numbers into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.PhoneNumberInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    this.phoneNumberList.Add(streamReader.ReadLine());
                }
            }
        }

        /// <summary>
        /// Read in the Last Name data from the resources
        /// </summary>
        private void ReadLastNames()
        {
            // Read the last names into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.LastNameInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    this.lastNameList.Add(streamReader.ReadLine());
                }
            }
        }

        /// <summary>
        /// Read in the Salutation data from the resources
        /// </summary>
        private void ReadSalutations()
        {
            // Read the salutations into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.MaleSalutationInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    this.salutationList.Add(streamReader.ReadLine());
                }
            }
        }

        /// <summary>
        /// Read in the nicknames from the resources.
        /// </summary>
        private void ReadNicknames()
        {
            // Read the nicknames into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.NickNameInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    string[] nameMap = streamReader.ReadLine().Split(' ');
                    this.nicknameList.Add(nameMap[0], nameMap[1]);
                }
            }
        }

        /// <summary>
        /// Reads the Provinces from the resources.
        /// </summary>
        private void ReadLocations()
        {
            // Read the locations (city, state, zip) into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.LocationInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    // Split the lines into towns, states and zip codes.
                    string locationText = streamReader.ReadLine();
                    string[] locationFields = locationText.Split('\t');

                    // This record can be used to provide a random location for a given consumer.
                    this.locationList.Add(new Location(locationFields[0], locationFields[1], locationFields[2]));
                }
            }
        }

        /// <summary>
        /// Reads the Street Names from the resources.
        /// </summary>
        private void ReadStreetNames()
        {
            // Read the street names into a list.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.StreetInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    this.streetNameList.Add(streamReader.ReadLine());
                }
            }
        }

        /// <summary>
        /// Build the table of unique credit card issuers
        /// </summary>
        private void BuildCreditCardIssuerTable()
        {
            // This will read all the issuers into a table.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.CreditCardIssuerInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    // This line contains the raw data extracted from an on-line phone book database.
                    string rawCreditCardIssuer = streamReader.ReadLine();
                    DataModel.RealCreditCardIssuerRow creditCardIssuerRow =
                        this.dataModel.RealCreditCardIssuer.NewRealCreditCardIssuerRow();
                    creditCardIssuerRow.Name = rawCreditCardIssuer;
                    creditCardIssuerRow.ExternalId = rawCreditCardIssuer.ToUpper();
                    this.dataModel.RealCreditCardIssuer.AddRealCreditCardIssuerRow(creditCardIssuerRow);
                }
            }
        }

        /// <summary>
        /// Create a random credit card.
        /// </summary>
        /// <returns>A randomly generated credit card number.</returns>
        private string GenerateCreditCardNumber()
        {
            Char[] creditCardArray = new char[16];
            switch (this.random.Next(0, 3))
            {
                case 0:

                    // Visa CCN
                    creditCardArray[0] = Convert.ToChar('4');
                    for (int index = 1; index < creditCardArray.Length; index++)
                    {
                        creditCardArray[index] = Convert.ToChar('0' + this.random.Next(0, 10));
                    }

                    break;

                case 1:

                    // MasterCard CCN
                    creditCardArray[0] = Convert.ToChar('5');
                    for (int index = 1; index < creditCardArray.Length; index++)
                    {
                        creditCardArray[index] = Convert.ToChar('0' + this.random.Next(0, 10));
                    }

                    break;

                default:

                    // American Express CCN
                    creditCardArray[0] = Convert.ToChar('7');
                    for (int index = 1; index < creditCardArray.Length; index++)
                    {
                        creditCardArray[index] = Convert.ToChar('0' + this.random.Next(0, 10));
                    }

                    break;

            }

            return new string(creditCardArray);
        }

        /// <summary>
        /// Generates a complete set of consumer records from cached set of individual 'parts'. The number of records 
        /// created is determined by the config setting: NumberOfConsumerRecords
        /// </summary>
        private void BuildConsumerTable()
        {
            // Hash table of names based on First, Last, and Middle names.  We will use this to detect duplicate full names.  We use a dictionary
            // instead of list to keep searching through it close to an O(1) operation instead of O(n) as this list could get huge.
            Dictionary<string, string> uniqueConsumerNameList = new Dictionary<string, string>();
            string nameKey;

            // Hash table of SSN values.  Used to detect the off-chance we randomly generate duplicates.  We have to avoid duplicates when generating
            // the clean/real data.
            Dictionary<string, string> uniqueSsnList = new Dictionary<string, string>();

            // Informational: used to keep track of how many SSN values we generate.  If we generated more than the number of consumer records we
            // generate, that means the difference will tell us how many dupes we had
            int numSsnGenerations = 0;

            // While the number of consumer records is less than the amount required by the config settings:
            while (this.dataModel.RealConsumer.Count < Settings.Default.NumberOfConsumerRecords)
            {
                // Create a new consumer record 
                DataModel.RealConsumerRow consumerRow = this.dataModel.RealConsumer.NewRealConsumerRow();

                // Select a gender for the consumer.
                Gender gender = this.random.Next(2) == 0 ? Gender.Male : Gender.Female;

                // Salutation:  Depends on config data determining what percentage of consumer records need to have a salutation 
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithSalutations)
                {
                    // five out of 100 consumers will be a Doctor (gender-neutral salutation).
                    if (this.random.Next() == 0.95)
                    {
                        consumerRow.Salutation = "Dr";
                    }
                    else
                    {
                        consumerRow.Salutation = this.GenerateSalutation(gender);
                    }
                }

                // Generate the first name (and optional nickname).
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithFirstNames)
                {
                    consumerRow.FirstName = this.GenerateFirstName(gender);
                    string nickName;
                    if (this.nicknameList.TryGetValue(consumerRow.FirstName, out nickName))
                    {
                        consumerRow.Nickname = nickName;
                    }
                }

                // Generate the middle name.
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithMiddleNames)
                {
                    consumerRow.MiddleName = this.GenerateMiddleName();
                }

                // Generate the surname.
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithLastNames)
                {
                    consumerRow.LastName = this.GenerateLastName();
                }

                // Generate a suffix.
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithSuffixes)
                {
                    consumerRow.Suffix = this.GenerateSuffix(gender);
                }

                // Generate a key based on the consumer's full name
                nameKey = string.Format("LN:{0} FN:{1} MN:{2}",
                    consumerRow.LastName,
                    consumerRow.FirstName,
                    consumerRow.MiddleName);

                // Address 1 (Street Address) Depends on config data to determine what percentage of consumer records need to have it filled out
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithAddress1)
                {
                    consumerRow.Address1 = string.Format("{0} {1}", this.random.Next(199) + 1, this.streetNameList[this.random.Next(streetNameList.Count)]);
                }

                // Address2:  Depends on config data to determine what percentage of consumer records need to have an Address2 filled out
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithAddress2)
                {
                    consumerRow.Address2 = this.BuildAddress2();
                }

                // Generate a date of birth.
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithDateOfBirth)
                {
                    // Randomly generate
                    string dateText = string.Format("{0}/{1}/{2}", this.random.Next(1, 13), this.random.Next(1, 28), this.random.Next(1940, 1992));
                    consumerRow.DateOfBirth = Convert.ToDateTime(dateText);
                }

                // Employment Status:  Whether or not we know the employment status depends on the weight set in config data
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumersWithKnownEmployeeStatus)
                {
                    // if we know the status, base it on an unemployment rate:
                    consumerRow.IsEmployed = this.random.NextDouble() < Settings.Default.PercentageOfUnemployment;
                }

                // Build the phone number
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithPhoneNumber)
                {
                    string rawPhoneNumber = this.phoneNumberList[this.random.Next(0, this.phoneNumberList.Count - 1)];
                    consumerRow.PhoneNumber = string.Format("({0}) {1} {2}",
                                                            rawPhoneNumber.Substring(0, 3),  /* area code */
                                                            rawPhoneNumber.Substring(3, 3),  /* exchange */
                                                            rawPhoneNumber.Substring(6, 4)); /* target */
                }

                // Generate a random location for the consumer.
                Location location = this.locationList[this.random.Next(0, this.locationList.Count)];

                // City:  Depends on config data to determine what percentage of consumer records need to have this filled out
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithCity)
                {
                    consumerRow.City = location.City;
                }

                // City:  Depends on config data to determine what percentage of consumer records need to have this filled out
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithProvinceCode)
                {
                    consumerRow.ProvinceCode = location.ProvinceCode;
                }

                // City:  Depends on config data to determine what percentage of consumer records need to have this filled out
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithPostalCode)
                {
                    consumerRow.PostalCode = location.PostalCode;
                }

                // Bank data for the Consumer:  Depends on config data to determine what percentage of consumer records need to have this filled out
                if (this.random.NextDouble() < Settings.Default.PercentageOfConsumerRecordsWithBankAccountData)
                {
                    // Bank Account Number: usually anywhere from 6 to 19 digits. 
                    consumerRow.BankAccountNumber =
                        string.Format("{0:00000}", this.random.Next(100000)) +
                        string.Format("{0:00000}", this.random.Next(100000)) +
                        string.Format("{0:00000}", this.random.Next(100000)) +
                        string.Format("{0:0000}", this.random.Next(10000));

                    // Bank Routing Number: 9 digit number is the standard format length
                    consumerRow.BankRoutingNumber =
                        string.Format("{0:00000}", this.random.Next(100000)) +
                        string.Format("{0:0000}", this.random.Next(10000));
                }

                // Social Security Number:
                string ssn = null;
                do
                {
                    // get a randomly generated SSN
                    ssn = this.GenerateSocialSecurityNumber();

                    // increment the count of how many SSNs we generate 
                    ++numSsnGenerations;

                } while (uniqueSsnList.ContainsKey(ssn));
                uniqueSsnList.Add(ssn, nameKey);
                consumerRow.SocialSecurityNumber = ssn;

                // Add the consumer data to the table.
                this.dataModel.RealConsumer.AddRealConsumerRow(consumerRow);

                // If the consumer's name already exists in this list, then we have a person with the same full name as at least one other already
                // generated.  We want to keep track of these situations in order to gauge the accuracy of the match algorithm...so add a reference
                // to this record in the DuplicateConsumerNames table.
                if (uniqueConsumerNameList.ContainsKey(nameKey))
                {
                    // create a record to track the duplicate
                    DataModel.DuplicateConsumerNamesRow duplicateConsumerRow =
                        this.dataModel.DuplicateConsumerNames.NewDuplicateConsumerNamesRow();

                    // save the duplicated name
                    duplicateConsumerRow.RealConsumerFullName = nameKey;

                    // point to the consumer record that is a duplicate.
                    duplicateConsumerRow.RealConsumerId = consumerRow.RealConsumerId;
                }
                else
                {
                    // Add the name of the consumer as the key so we can quickly search for it in the list
                    uniqueConsumerNameList.Add(nameKey, consumerRow.SocialSecurityNumber);
                }

                // Generate credit cards for the consumer
                this.GenerateCreditCardsForConsumer(consumerRow);
            }
        }

        /// <summary>
        /// Generate a random Social Security Number
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        private string GenerateSocialSecurityNumber()
        {
            string ssn = null;

            // Generate a random social security number.
            if (Settings.Default.NumberOfConsumerRecords == 1)
            {
                // Generate the generic single SSN
                ssn = string.Format("{0:000-00-0000}", 12345678);
            }
            else
            {
                ssn = string.Format("{0:000-00-0000}", this.random.Next(1000000000));
            }

            return ssn;
        }

        /// <summary>
        /// Generate a suffix
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        private string GenerateSuffix(Gender gender)
        {
            string suffix = null;

            if (gender == Gender.Female)
            {
                // no-op right now
            }
            else /* male */
            {
                // get a random male suffix from the cached list
                suffix = this.maleSuffixList[this.random.Next(0, this.maleSuffixList.Count - 1)];
            }

            return suffix;
        }

        /// <summary>
        /// Generate a Middlename 
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        private string GenerateMiddleName()
        {
            string middleName = null;

            if (Settings.Default.NumberOfConsumerRecords == 1)
            {
                middleName = "Z";
            }
            else
            {
                // [skt] todo: right now we are only supplying an initial, if we need full names, we can pull them out of
                // the gender-specific first name files
                middleName = Convert.ToChar('A' + this.random.Next(0, 26)).ToString();
            }

            return middleName;
        }

        /// <summary>
        /// Generate a last name
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        private string GenerateLastName()
        {
            string lastName = null;

            if (Settings.Default.NumberOfConsumerRecords == 1)
            {
                lastName = "Doe";
            }
            else
            {
                // get random name out of the list
                lastName = this.lastNameList[this.random.Next(0, this.lastNameList.Count - 1)];

                // uppercase first letter, lowercase the rest
                lastName = lastName[0].ToString().ToUpper() + lastName.Substring(1, lastName.Length - 1).ToLower();
            }

            return lastName;
        }

        /// <summary>
        /// Return a first name based on the gender requirement parameter
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        private string GenerateFirstName(Gender gender)
        {
            string firstName = null;

            if (gender == Gender.Female)
            {
                if (Settings.Default.NumberOfConsumerRecords == 1)
                {
                    firstName = "Jane";
                }
                else
                {
                    // get a random female name from the cached list
                    firstName = this.femaleNameList[this.random.Next(0, this.femaleNameList.Count - 1)];
                }
            }
            else /* male */
            {
                if (Settings.Default.NumberOfConsumerRecords == 1)
                {
                    firstName = "John";
                }
                else
                {
                    // get a random male name from the cached list
                    firstName = this.maleNameList[this.random.Next(0, this.maleNameList.Count - 1)];
                }
            }

            // uppercase first letter, lowercase the rest
            firstName = firstName[0].ToString().ToUpper() + firstName.Substring(1, firstName.Length - 1).ToLower();

            return firstName;
        }

        /// <summary>
        /// Returns a random salutation based on the passed-in gender requirement.
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        private string GenerateSalutation(Gender gender)
        {
            string salutation = null;

            if (gender == Gender.Female)
            {
                salutation = this.femaleSalutationList[this.random.Next(femaleSalutationList.Count)];
            }
            else
            {
                salutation = this.maleSalutationList[this.random.Next(maleSalutationList.Count)];
            }

            return salutation;
        }

        /// <summary>
        /// Generate credit cards for the passed-in consumer 
        /// </summary>
        /// <param name="consumerRow"></param>
        /// <param name="random"></param>
        private void GenerateCreditCardsForConsumer(DataModel.RealConsumerRow consumerRow)
        {
            int numCreditCards;

            // Get the number of credit cards we need to generate
            if (Settings.Default.NumberOfCreditCardsPerConsumer == TestDataConfig.GenerateRandomNumberOfCreditCards)
            {
                // Generate between 1 and RandomCreditCardsPerConsumerMax credit cards per consumer.
                //    (RandomCreditCardsPerConsumerMax / 2) gives us a rough average per consumer
                numCreditCards = this.random.Next(1, Settings.Default.RandomCreditCardsPerConsumerMax + 1);
            }
            else
            {
                // constant value of credit cards to create per consumer
                numCreditCards = Settings.Default.NumberOfCreditCardsPerConsumer;
            }

            // go forth and generate the specified number of cards
            for (int index = 0; index < numCreditCards; index++)
            {
                DataModel.RealCreditCardRow creditCardRow =
                    this.dataModel.RealCreditCard.NewRealCreditCardRow();

                // Create the credit line record
                if (Settings.Default.NumberOfConsumerRecords == 1)
                {
                    // Ensure that the CCNs are unique and predictable when we are just dealing with one user

                    // This is typically some number associated with the Credit Card and the institution that issued it
                    creditCardRow.AccountCode = string.Format("{0}{1:000}", "111222", index + 1);

                    // this is the actual Credit Card Number
                    creditCardRow.OriginalAccountNumber = string.Format("{0}{1:000}", "6666555544443", index + 1);
                }
                else
                {
                    // AccountNumber is typically some number associated with the Credit Card and the institution that either 
                    // issued it or now owns it
                    creditCardRow.AccountCode = string.Format("{0:000000000}", this.random.Next(1000000000));

                    // this is the actual Credit Card Number 
                    creditCardRow.OriginalAccountNumber = GenerateCreditCardNumber();
                }

                creditCardRow.RealConsumerId = consumerRow.RealConsumerId;

                // Issuer:  whether or not we supply this optional data depends on the config setting's weight distribution
                if (this.random.NextDouble() < Settings.Default.PercentageOfCreditCardRecordsWithIssuerData)
                {
                    // Pick a random Issuer/DebtHolder/Originator (pick the term) for this credit card
                    creditCardRow.RealDebtHolder = this.dataModel.RealCreditCardIssuer[this.random.Next(0, this.dataModel.RealCreditCardIssuer.Count)].Name;
                }

                creditCardRow.ExternalId = Guid.NewGuid().ToString();
                creditCardRow.AccountBalance = Math.Round(Convert.ToDecimal(this.random.NextDouble() * 15000.0), 2);
                this.dataModel.RealCreditCard.AddRealCreditCardRow(creditCardRow);

                // Keep track of the credit cards created for this consumer.  It will make it much more efficient to find per-consumer CCs later on
                // rather than iterating through the credit card table, one record at a time.
                consumerRow.RealCreditCardList = StringHelpers.AppendTokenToCommaDelimitedString(
                    consumerRow.RealCreditCardList,
                    creditCardRow.RealCreditCardId.ToString());
            }
        }

        /// <summary>
        /// Builds a string for an Address 2 
        /// Examples: Apartment 100, 1st Floor, Suite 44-A, etc..
        /// </summary>
        /// <returns></returns>
        private string BuildAddress2()
        {
            if (this.random == null)
                return null;

            StringBuilder address2 = new StringBuilder();

            // get a random address from the available address2List
            string token = this.address2List[this.random.Next(address2List.Count)];

            // Build a floor address string and return
            if (token.ToLower() == "floor")
            {
                return BuildFloorSubAddress(token);
            }

            // do similar stuff for all non-'floor' types of address strings
            address2.Append(token);

            // around one out of every 50 addresses we will NOT put a space between the token and the number value
            if (this.random.Next(50) != 0)
            {
                address2.Append(" ");
            }

            // vary the height of the building
            int topFloor =
                (this.random.Next(5) == 0 ? TestDataConfig.BuildingFloors.SkyscraperFloors : TestDataConfig.BuildingFloors.LowriseFloors);

            address2.Append(Convert.ToString(this.random.Next(1, topFloor)));

            // around one out of 30 times we will put a dash followed by a letter (A thru G)
            if (this.random.Next(30) == 0)
            {
                address2.Append("-");
                address2.Append(Convert.ToChar('A' + this.random.Next(0, 7)));
            }

            return address2.ToString();
        }

        /// <summary>
        /// Build a string that cooresponds to which floor the address is part of
        /// i.e. 101 st Floor 2nd Floor, etc...
        /// </summary>
        /// <returns></returns>
        private string BuildFloorSubAddress(string token)
        {
            if (this.random == null)
                return null;

            StringBuilder address2 = new StringBuilder();
            string qualifier;

            // vary the height of the building
            int topFloor =
                (this.random.Next(5) == 0 ? TestDataConfig.BuildingFloors.SkyscraperFloors : TestDataConfig.BuildingFloors.LowriseFloors);

            string floorNumber = Convert.ToString(this.random.Next(1, topFloor));

            address2.Append(floorNumber);

            // around one out of every 75 addresses will put a space between the floor number and the qualifier
            // 1 st floor versus 1st floor
            if (this.random.Next(75) == 0)
            {
                address2.Append(" ");
            }

            // build a floor qualifier
            if (floorNumber.EndsWith("1"))
            {
                qualifier = "st";
            }
            else if (floorNumber.EndsWith("2"))
            {
                qualifier = "nd";
            }
            else if (floorNumber.EndsWith("3"))
            {
                qualifier = "rd";
            }
            else
            {
                qualifier = "th";
            }

            // append the qualifier 
            address2.Append(qualifier);

            // around one out of every 50 addresses we will NOT put a space between the qualifier and the next token
            if (this.random.Next(25) != 0)
            {
                address2.Append(" ");
            }

            // finally append the "floor" string (use token insted of literal to account for different character cases)
            address2.Append(token);

            return address2.ToString();
        }

        /// <summary>
        /// Read organization information from the resources.
        /// </summary>
        private void BuildOrganizationTable()
        {
            // Read the organizations from the data file.
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.OrganizationInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    // Read the next line from the file.  Skip empty and commented lines.
                    string organization = streamReader.ReadLine();
                    if (string.IsNullOrEmpty(organization) || organization.StartsWith("#"))
                    {
                        continue;
                    }

                    // Make sure the line is properly formatted.
                    string[] organizationParts = organization.Split(',');
                    if (organizationParts.Length < 2)
                    {
                        throw new SystemException(Settings.Default.OrganizationInput + " can't be parsed.");
                    }

                    // Create a new organization record.
                    DataModel.OrganizationRow organizationRow = this.dataModel.Organization.NewOrganizationRow();
                    organizationRow.EntityId = Guid.NewGuid();
                    organizationRow.Name = organizationParts[0].Trim();
                    OrganizationType organizationType = (OrganizationType)Enum.Parse(typeof(OrganizationType), organizationParts[1].Trim());
                    organizationRow.Type = (int)organizationType;

                    // Add the organization to the table
                    this.dataModel.Organization.AddOrganizationRow(organizationRow);
                }
            }
        }

        /// <summary>
        /// Read user information from the resources and populate the User Table 
        /// </summary>
        private void BuildUserTable()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.UserInput)))
            {
                while (!streamReader.EndOfStream)
                {
                    string[] rawUser = streamReader.ReadLine().Split(TestDataConfig.CsvDelimiter);

                    // skip any blank or commented out lines
                    if ((rawUser[0] == string.Empty) || (rawUser[0].Trim()[0] == TestDataConfig.CommentMarker))
                        continue;

                    // There has to be an organization to associate this user with...otherwise we will ignore it.
                    if (this.dataModel.Organization.Rows.Contains(rawUser[TestDataConfig.RawUserColumn.Organization].Trim()))
                    {
                        DataModel.UserRow userRow = this.dataModel.User.NewUserRow();

                        userRow.UserId = Guid.NewGuid();

                        userRow.Password = rawUser[TestDataConfig.RawUserColumn.Password].Trim();

                        userRow.Name = rawUser[TestDataConfig.RawUserColumn.Name].Trim();

                        userRow.ExternalId = userRow.Name.ToUpper();

                        userRow.Organization = rawUser[TestDataConfig.RawUserColumn.Organization].Trim();

                        userRow.Email = rawUser[TestDataConfig.RawUserColumn.Email].Trim();

                        this.dataModel.User.AddUserRow(userRow);

                        // I hearby nominate this user to represent the organization when we generate working orders 
                        // via (CreatedUserId)...Representative user is the ExternalId0 of the Entity Table
                        foreach (DataModel.OrganizationRow orgRow in this.dataModel.Organization)
                        {
                            if (orgRow.Name == userRow.Organization)
                            {
                                orgRow.RepresentativeUser = userRow.Name;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads raw consumer debt data into memory.  File source is defined by
        /// config setting: ConsumerDebtInput
        /// </summary>
        /// <param name="rawConsumerDebtCache"></param>
        private void ReadRawConsumerDebtData(List<string[]> rawConsumerDebtCache)
        {
            // Open a stream to a raw data file that has some information we will use to fill some ConsumerDebt fields 
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(Settings.Default.ConsumerDebtInput)))
            {
                // Read in each line of raw consumer debt data into memory. Each line of text is delimited 
                // so break the lines down into individual string tokens per line.
                while (!streamReader.EndOfStream)
                {
                    rawConsumerDebtCache.Add(streamReader.ReadLine().Split('\t'));
                }
            }
        }

        /// <summary>
        /// Randomly chooses a row of consumer debt data from the raw cache
        /// </summary>
        /// <param name="random"></param>
        /// <param name="rawConsumerDebtCache"></param>
        /// <returns></returns>
        private string[] GetRandomRawConsumerDebtData(List<string[]> rawConsumerDebtCache)
        {
            // return the array of strings for this one raw debt record
            return rawConsumerDebtCache[this.random.Next(0, rawConsumerDebtCache.Count)];
        }

        /// <summary>
        /// Generates the Consumer Trust data. 
        /// </summary>
        /// <param name="this.random"></param>
        private void BuildConsumerTrustTable()
        {
            // used to track the unique Id of a trust consumer row 
            int trustConsumerId;

            int numTrustRecordsToCreate;
            if (Settings.Default.NumberOfTrustRecords < 0)
            {
                // A negative value for this config setting means to choose a random amount of consumers
                // to pick from out of the RealConsumer table
                numTrustRecordsToCreate = this.random.Next(1, this.dataModel.RealConsumer.Count);
            }
            else
            {
                // use the literal amount specified in the config setting.
                numTrustRecordsToCreate = Settings.Default.NumberOfTrustRecords;
            }

            // Vendor Code 
            // [skt] for now just a GUID but it should be the org name or some other field out of the organization table
            string vendorCode = System.Guid.NewGuid().ToString();

            // Populate the Trust Table
            while (this.trustSideMapOfRealConsumerIds.Count < numTrustRecordsToCreate)
            {
                // randomly fetch a consumer out of the RealConsumer table
                DataModel.RealConsumerRow realConsumerRow =
                    this.dataModel.RealConsumer[this.random.Next(this.dataModel.RealConsumer.Count)];

                // if the real consumer has already been added to the trust side consumer table, try again
                if (this.trustSideMapOfRealConsumerIds.ContainsKey(realConsumerRow.RealConsumerId))
                    continue;

                // Add a Corresponding Consumer Record  
                //   trustConsumerId=the ID of the new row in the TrustSideConsumer table
                AddRealConsumerToTrustSideConsumerTable(realConsumerRow, out trustConsumerId);

                // keep track of the Real Consumer added to the trust side consumer table
                this.trustSideMapOfRealConsumerIds.Add(realConsumerRow.RealConsumerId, trustConsumerId);

                // Add the credit card(s) to the trust side consumer 
                AssignCreditCardsToTrustSideConsumer(realConsumerRow, trustConsumerId);

                // 
                // Create and Add a Trust record
                //
                DataModel.ConsumerTrustRow consumerTrustRow = this.dataModel.ConsumerTrust.NewConsumerTrustRow();

                // Customer Code:  This is the Negotiator's unique identifier per consumer.  We can map it here to the Id of the consumer 
                // added to the trust table since that Id is unique.
                consumerTrustRow.CustomerCode = trustConsumerId;

                // This will generate random balances for the checking and savings account.
                consumerTrustRow.SavingsBalance = Math.Round(Convert.ToDecimal(this.random.NextDouble() * this.random.Next(500, 50000)), 2);

                // This creates a random value for how much of the credit card can be deducted automatically from a checking account.
                switch (this.random.Next(0, 4))
                {
                    case 0:
                        consumerTrustRow.AutoSettlement = 0.40M;
                        break;

                    case 1:
                        consumerTrustRow.AutoSettlement = 0.45M;
                        break;

                    case 2:
                        consumerTrustRow.AutoSettlement = 0.50M;
                        break;

                    case 3:
                        consumerTrustRow.AutoSettlement = 0.60M;
                        break;
                }

                // Set a name for the Savings Entity (the custodial owner of the savings acct)
                //    For now the options are GCS or Self-Held and will be distributed 80/20 respectively
                if (this.random.NextDouble() < 0.8)
                {
                    consumerTrustRow.SavingsEntityCode = "GCS";
                }
                else
                {
                    consumerTrustRow.SavingsEntityCode = "Self-Held";
                }

                consumerTrustRow.VendorCode = vendorCode;

                // Add the consumer trust row to the ConsumerTrust table
                this.dataModel.ConsumerTrust.AddConsumerTrustRow(consumerTrustRow);

                // Keep track of the Trust Record IDs in a separate cache so we have easy/fast access to the list
                this.consumerTrustIds.Add(consumerTrustRow.ConsumerTrustId);
            }
        }

        /// <summary>
        /// Builds the Consumer Debt Record based on raw text data and data from the Consumer and CreditCard tables 
        /// </summary>
        /// <param name="random"></param>
        private void BuildConsumerDebtTable()
        {
            // Build a cache of the Consumer Debt data based on the supplied raw data file
            List<string[]> rawDebtRecords = new List<string[]>();
            ReadRawConsumerDebtData(rawDebtRecords);

            string[] rawConsumerDebtRecord;

            // Unique debtHolderAccountCode is needed for each Debt record. We got a number/pattern from GCS at one point
            // which is based on the config setting below. Use that as a starting point and just increment from there
            Int64 debtHolderAccountCode = Convert.ToInt64(Settings.Default.DebtHolderAccountNumberStartValue);

            int debtSideConsumerId;
            int numDebtRecordsToCreate;
            DataModel.RealCreditCardRow realCreditCardRow = null;

            // Determine how many debt records we need to create
            if (Settings.Default.NumberOfDebtRecords < 0)
            {
                // A negative value for this config setting means to choose a this.random amount of credit cards
                // to pick from out of the CreditCard table
                numDebtRecordsToCreate = this.random.Next(1, this.dataModel.RealCreditCard.Count);
            }
            else
            {
                // use the literal amount specified in the config setting.
                numDebtRecordsToCreate = Settings.Default.NumberOfDebtRecords;
            }

            // Vendor Code 
            // [skt] for now just a GUID but it should be the org name or some other field out of the organization table
            string vendorCode = System.Guid.NewGuid().ToString();

            // Build the Debt Records and add them to the Debt-Side Tables
            while (this.debtSideMapOfRealCreditCardIds.Count < numDebtRecordsToCreate)
            {
                bool currentRecordIsMatch = false;

                // the number of debt organizations affects how the matches get divied up...the matches are front loaded 
                // in the debt table.  We shuffle the list of them later on so they are evenly distributed 

                // Determine where to get the CC data from: Keep building matched records until the number of matched records added to 
                // the Debt side equals the required NumberOfMatches. In order to have guaranteed matches (fuzzing aside), 
                // we need to get records out of the Trust CC table which was built already
                if (this.debtSideMapOfRealCreditCardIds.Count < Settings.Default.NumberOfMatches)
                {
                    // ** Ensure a match (at least prior to fuzzing..which in and of itself could inadvertently create a match)

                    // get a random CC out of the Trust CC Table so we can have a guaranteed match between trust and debt sides
                    DataModel.TrustCreditCardRow trustCreditCardRow =
                        this.dataModel.TrustCreditCard[this.random.Next(this.dataModel.TrustCreditCard.Count)];

                    // In order to find out if we can match with this one, we need to get the corresponding
                    // Real Credit Card row based on the trust CC record we just pulled out.
                    realCreditCardRow = this.dataModel.RealCreditCard[trustCreditCardRow.RealCreditCardId];

                    // If the CC is already matched, then move on to the next random CC in the TrustCreditCard table
                    if (realCreditCardRow.IsMatched)
                        continue;
                    // [skt] we may want to ensure a higher amount of control over how multiple consumers' credit cards get 
                    // added to the debt side.  Could do this by trying another CC from the SAME CONSUMER here instead of 
                    // just grabbing another random CC out of the TrustCreditCard table, leaving duplicate consumers up to 'chance'  

                    // Denote that this current set of data is intended to be a matching one
                    currentRecordIsMatch = true;
                }
                else
                {
                    // ** Ensure we do NOT get a match (at least prior to fuzzing)

                    // Get the credit card data directly from the RealCreditCard table
                    realCreditCardRow = this.dataModel.RealCreditCard[this.random.Next(this.dataModel.RealCreditCard.Count)];

                    // We do NOT want to create ANY more matches at this point:+
                    //   If this particular credit card's consumer is in the TrustSideConsumer table, the consumer 
                    //   has already been matched via one or more of its credit cards...so try again
                    if (this.trustSideMapOfRealConsumerIds.ContainsKey(realCreditCardRow.RealConsumerId))
                        continue;
                }

                // if the credit card has already been added to the DebtCreditCard table, try again.  This check is 
                // needed because we are 'randomly' grabbing credit cards out of the CC tables so dups are possible.
                if (this.debtSideMapOfRealCreditCardIds.ContainsKey(realCreditCardRow.RealCreditCardId))
                    continue;

                // Check to see if we want to make sure we only one credit card from a consumer is allowed to 
                // be held by this debt organization. This is a corner case as it is entirely possible that a
                // holder organization purchase two separate baskets of CCs, each of which could contain a CC
                // from the same consumer (consumer owns a Macy's and BOA card)
                if ((Settings.Default.OneConsumerCreditCardPerDebtHolderOrganization) &&
                   (this.debtSideMapOfRealConsumerIds.ContainsKey(realCreditCardRow.RealConsumerId)))
                    continue;

                // Randomly pick a line from the raw Debt Record data to help fill in the rest of the consumer debt data
                rawConsumerDebtRecord = GetRandomRawConsumerDebtData(rawDebtRecords);

                // create a new ConsumerDebt row and start populating it
                DataModel.ConsumerDebtRow consumerDebtRow = this.dataModel.ConsumerDebt.NewConsumerDebtRow();

                // Create a unique consumer per debt record and get its Id back so we can use it to map to the Debt Side CC table
                AddRealConsumerToDebtSideConsumerTable(realCreditCardRow.RealConsumerRow, out debtSideConsumerId);

                // used to store the ID of the credit card created for the debt side CC table
                int debtSideCreditCardId;

                // Add the Real CreditCard to the DebtCreditCard table
                this.AssignCreditCardToDebtSideConsumer(realCreditCardRow, debtSideConsumerId, out debtSideCreditCardId);

                // Update the cache map of RealCreditCards to their Debt-Side representations
                this.debtSideMapOfRealCreditCardIds.Add(realCreditCardRow.RealCreditCardId, debtSideCreditCardId);

                // Assign a unique DataArchiveId and increment it for the next Debt record
                consumerDebtRow.AccountCode = debtHolderAccountCode++;

                // Date of Delinquency:
                DateTime dateOfDelinquency = DateTime.MinValue;
                if (TryGetDateOfDelinquencyFromRawDebtData(rawConsumerDebtRecord, consumerDebtRow, ref dateOfDelinquency))
                {
                    consumerDebtRow.DateOfDelinquency = dateOfDelinquency;
                }

                // Vendor Code: Unqiue per Debt Holder (consumer debt) organization
                consumerDebtRow.VendorCode = vendorCode;

                // Finally, the consumer debt record is added to the data model.              
                this.dataModel.ConsumerDebt.AddConsumerDebtRow(consumerDebtRow);

                // Keep track of the Debt Record IDs in a separate cache so we have easy/fast access to the list
                this.consumerDebtIds.Add(consumerDebtRow.ConsumerDebtId);

                // If we are working with a matched CC, increment the RealConsumer's match count by one and 
                // mark the RealCreditCard as being matched
                if (currentRecordIsMatch)
                {
                    this.dataModel.RealConsumer[realCreditCardRow.RealConsumerId].MatchCount++;
                    this.dataModel.RealCreditCard[realCreditCardRow.RealCreditCardId].IsMatched = true;
                }
            }

            // The rawDebtRecords cache is pretty large so it's worth it to force the garbage collector to clean it up here
            rawDebtRecords.Clear();
            rawDebtRecords = null;
            Utils.ForceGarbageCollection();
        }

        /// <summary>
        /// Extract the date of delinquency from the raw debt data
        /// </summary>
        /// <param name="rawConsumerDebtList"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        private bool TryGetDateOfDelinquencyFromRawDebtData
            (
            string[] rawConsumerDebtList,
            DataModel.ConsumerDebtRow consumerDebtRow,
            ref DateTime dateOfDelinquency
            )
        {
            bool retVal = false;

            if (rawConsumerDebtList[TestDataConfig.RawConsumerDebtColumn.DateOfDelinquency] != "NULL")
            {
                // we have a value in the raw data
                dateOfDelinquency = Convert.ToDateTime(rawConsumerDebtList[TestDataConfig.RawConsumerDebtColumn.DateOfDelinquency]);
                retVal = true;
            }
            else
            {
                // [skt] else: eventually make something up
            }

            return retVal;
        }

        /// <summary>
        /// Takes the passed in credit card row and adds it to the DebtCreditCard table
        /// </summary>
        /// <param name="creditCardRow"></param>
        /// <param name="debtConsumerId"></param>
        private void AssignCreditCardToDebtSideConsumer
            (
            DataModel.RealCreditCardRow realCreditCardRow,
            int debtConsumerId,
            out int debtCreditCardId
            )
        {
            DataModel.DebtCreditCardRow debtCreditcardRow =
                this.dataModel.DebtCreditCard.NewDebtCreditCardRow();

            // mark it as not fuzzed yet
            debtCreditcardRow.FuzzedFields = Convert.ToInt32(Fuzzers.CreditCardFuzzedFields.None);

            // map to the DebtConsumer row's ID
            debtCreditcardRow.DebtSideConsumerId = debtConsumerId;

            // AccountNumber is typically some number associated with the Credit Card and the institution that either issued it or now owns it
            debtCreditcardRow.AccountCode = string.Format("{0:000000000}", this.random.Next(1000000000));
            debtCreditcardRow.AccountBalance = realCreditCardRow.AccountBalance;
            debtCreditcardRow.RealDebtHolder = realCreditCardRow.RealDebtHolder;
            debtCreditcardRow.RealCreditCardId = realCreditCardRow.RealCreditCardId;

            // each credit card needs to be uniquely identified so as not to be overwritten in 
            // the DB (even if the same CC exists on both the Trust and Debt sides
            debtCreditcardRow.ExternalId = Guid.NewGuid().ToString();

            debtCreditcardRow.OriginalAccountNumber = realCreditCardRow.OriginalAccountNumber;

            // Add the new row to the DebtCreditCard table
            this.dataModel.DebtCreditCard.AddDebtCreditCardRow(debtCreditcardRow);

            // make sure we return the Id of this new credit card added 
            debtCreditCardId = debtCreditcardRow.DebtCreditCardId;
        }

        /// <summary>
        /// Takes the passed in realConsumerRow and adds that consumer's real credit card info to the TrustSideConsumer 
        /// </summary>
        /// <param name="realConsumerRow"></param>
        /// <param name="trustConsumerId"></param>
        private void AssignCreditCardsToTrustSideConsumer(DataModel.RealConsumerRow realConsumerRow, int trustConsumerId)
        {
            // Get the list of Real credit cards associcated with this consumer
            List<DataModel.RealCreditCardRow> realCreditCardList
                = GetRealCreditCardList(realConsumerRow);

            // Get the Consumer Trust's Consumer record so we can add the CC Id's to its list
            DataModel.TrustSideConsumerRow trustConsumer =
                this.dataModel.TrustSideConsumer[trustConsumerId];

            // Add the consumers' Real Credit Cards info to the TrustCreditCard table
            for (int index = 0; index < realCreditCardList.Count; index++)
            {
                DataModel.TrustCreditCardRow trustCreditCardRow =
                    this.dataModel.TrustCreditCard.NewTrustCreditCardRow();

                trustCreditCardRow.FuzzedFields = Convert.ToInt32(Fuzzers.CreditCardFuzzedFields.None);

                // map to the TrustConsumer row's ID
                trustCreditCardRow.TrustSideConsumerId = trustConsumerId;

                // use the real credit card's AccountNumber here. We will use a different number on the Debt side
                // to make sure they are different 
                trustCreditCardRow.AccountCode = realCreditCardList[index].AccountCode;
                trustCreditCardRow.AccountBalance = realCreditCardList[index].AccountBalance;
                trustCreditCardRow.RealDebtHolder = realCreditCardList[index].RealDebtHolder;
                trustCreditCardRow.RealCreditCardId = realCreditCardList[index].RealCreditCardId;

                // each credit card needs to be uniquely identified so as not to be overwritten in 
                // the DB (even if the same CC exists on both the Trust and Debt sides
                trustCreditCardRow.ExternalId = Guid.NewGuid().ToString();

                trustCreditCardRow.OriginalAccountNumber = realCreditCardList[index].OriginalAccountNumber;

                // Add the new row to the TrustCreditCard table
                this.dataModel.TrustCreditCard.AddTrustCreditCardRow(trustCreditCardRow);

                // Update the cache map of RealCreditCards to their Trust-Side representations
                this.trustSideMapOfRealCreditCardIds.Add(realCreditCardList[index].RealCreditCardId, trustCreditCardRow.TrustCreditCardId);

                // Keep track of the credit cards created for this Trust-Side consumer.  It will make it much more efficient
                // to find per-TrustConsumer CCs later on rather than iterating through the entire credit card table, one 
                // record at a time.
                trustConsumer.TrustSideCreditCardList =
                    StringHelpers.AppendTokenToCommaDelimitedString(trustConsumer.TrustSideCreditCardList,
                                                                    trustCreditCardRow.TrustCreditCardId.ToString());
            }
        }

        /// <summary>
        /// Return a list of all credit card rows in the RealCreditCard table based on the 
        /// passed-in Consumer row
        /// </summary>
        /// <param name="realConsumer"></param>
        /// <returns></returns>
        private List<DataModel.RealCreditCardRow> GetRealCreditCardList(
            DataModel.RealConsumerRow realConsumer)
        {
            // Initialize the list of RealCreditCardRows we will be returning
            List<DataModel.RealCreditCardRow> realCreditCardList
                = new List<DataModel.RealCreditCardRow>();

            // Get the list of Credit Card Id's this consumer 'owns'
            List<int> creditCardIdList =
                DataGenHelpers.GetIdsFromCsvList(realConsumer.RealCreditCardList);

            // Now populate the return value with the acutal list of credit card rows
            foreach (int creditCardId in creditCardIdList)
            {
                realCreditCardList.Add(this.dataModel.RealCreditCard[creditCardId]);
            }

            return realCreditCardList;
        }

        /// <summary>
        /// Return a list of all credit card rows in the TrustCreditCard table based on the 
        /// passed-in trust-side consumer row
        /// </summary>
        /// <param name="trustConsumer"></param>
        /// <returns></returns>
        private List<DataModel.TrustCreditCardRow>
            GetTrustCreditCardList(DataModel.TrustSideConsumerRow trustConsumer)
        {
            // Initialize the list of TrustCreditCardRows we will be returning
            List<DataModel.TrustCreditCardRow> trustCreditCardList
                = new List<DataModel.TrustCreditCardRow>();

            // Get the list of Credit Card Id's that this trust consumer 'owns'
            List<int> creditCardIdList =
                DataGenHelpers.GetIdsFromCsvList(trustConsumer.TrustSideCreditCardList);

            // Now populate the return value with the acutal list of credit card rows
            foreach (int creditCardId in creditCardIdList)
            {
                trustCreditCardList.Add(this.dataModel.TrustCreditCard[creditCardId]);
            }

            return trustCreditCardList;
        }

        /// <summary>
        /// Takes the passed in consumer row and adds it to the DebtSideConsumer table.
        /// </summary>
        /// <param name="consumerRow"></param>
        /// <param name="debtSideConsumerId"></param>
        private void AddRealConsumerToDebtSideConsumerTable(DataModel.RealConsumerRow consumerRow,
                                                            out int debtSideConsumerId)
        {
            var debtSideConsumerRow = this.dataModel.DebtSideConsumer.NewDebtSideConsumerRow();

            // map to the real consumer ID
            debtSideConsumerRow.RealConsumerId = consumerRow.RealConsumerId;

            debtSideConsumerRow.Address1 = consumerRow.Address1;
            debtSideConsumerRow.Address2 = consumerRow.Address2;
            debtSideConsumerRow.City = consumerRow.City;
            if (!consumerRow.IsDateOfBirthNull())
            {
                debtSideConsumerRow.DateOfBirth = consumerRow.DateOfBirth;
            }

            // [skt] would be nice to also include organization name here just like we use for the XML import files.
            // as it is for now the DebtSideConsumerId is enough of a unique value to use for this external Id
            debtSideConsumerRow.ExternalId = debtSideConsumerRow.DebtSideConsumerId.ToString();

            debtSideConsumerRow.FirstName = consumerRow.FirstName;
            debtSideConsumerRow.LastName = consumerRow.LastName;
            debtSideConsumerRow.MiddleName = consumerRow.MiddleName;

            if (!consumerRow.IsIsEmployedNull())
            {
                debtSideConsumerRow.IsEmployed = consumerRow.IsEmployed;
            }

            debtSideConsumerRow.PostalCode = consumerRow.PostalCode;
            debtSideConsumerRow.PhoneNumber = consumerRow.PhoneNumber;
            debtSideConsumerRow.ProvinceCode = consumerRow.ProvinceCode;
            debtSideConsumerRow.SocialSecurityNumber = consumerRow.SocialSecurityNumber;
            debtSideConsumerRow.Salutation = consumerRow.Salutation;
            debtSideConsumerRow.Suffix = consumerRow.Suffix;

            // keep track of what is fuzzed
            debtSideConsumerRow.FuzzedFields = Convert.ToInt32(Fuzzers.ConsumerFuzzedFields.None);

            this.dataModel.DebtSideConsumer.AddDebtSideConsumerRow(debtSideConsumerRow);

            // 'return' the unique Id of the row added
            debtSideConsumerId = debtSideConsumerRow.DebtSideConsumerId;

            // we also need to keep track of the RealConsumerId in case we want to prevent adding 
            // another Credit Card owned by the same consumer. 
            if (!this.debtSideMapOfRealConsumerIds.ContainsKey(consumerRow.RealConsumerId))
            {
                // Add consumer to the map with the first debt side version of the consumer
                this.debtSideMapOfRealConsumerIds.Add(consumerRow.RealConsumerId, debtSideConsumerId.ToString());
            }
            else
            {
                // Real Consumer has already been added to the map, so add the next debt side version of that consumer to it
                this.debtSideMapOfRealConsumerIds[consumerRow.RealConsumerId] =
                    StringHelpers.AppendTokenToCommaDelimitedString
                    (
                        this.debtSideMapOfRealConsumerIds[consumerRow.RealConsumerId],
                        debtSideConsumerId.ToString()
                    );
            }
        }

        /// <summary>
        /// Takes the passed in consumer row and adds it to the TrustSideConsumer table.
        /// </summary>
        /// <param name="consumerRow"></param>
        /// <param name="trustSideConsumerId"></param>
        private void AddRealConsumerToTrustSideConsumerTable(
            DataModel.RealConsumerRow consumerRow,
            out int trustSideConsumerId)
        {
            var trustSideConsumerRow = this.dataModel.TrustSideConsumer.NewTrustSideConsumerRow();

            // map to the real consumer ID
            trustSideConsumerRow.RealConsumerId = consumerRow.RealConsumerId;

            trustSideConsumerRow.Address1 = consumerRow.Address1;
            trustSideConsumerRow.Address2 = consumerRow.Address2;
            trustSideConsumerRow.City = consumerRow.City;

            if (!consumerRow.IsDateOfBirthNull())
            {
                trustSideConsumerRow.DateOfBirth = consumerRow.DateOfBirth;
            }

            // [skt] would be nice to have organization name here just like we use for the XML import files
            trustSideConsumerRow.ExternalId = trustSideConsumerRow.TrustSideConsumerId.ToString();

            trustSideConsumerRow.FirstName = consumerRow.FirstName;
            trustSideConsumerRow.LastName = consumerRow.LastName;
            trustSideConsumerRow.MiddleName = consumerRow.MiddleName;

            if (!consumerRow.IsIsEmployedNull())
            {
                trustSideConsumerRow.IsEmployed = consumerRow.IsEmployed;
            }
            trustSideConsumerRow.PostalCode = consumerRow.PostalCode;
            trustSideConsumerRow.PhoneNumber = consumerRow.PhoneNumber;
            trustSideConsumerRow.ProvinceCode = consumerRow.ProvinceCode;
            trustSideConsumerRow.SocialSecurityNumber = consumerRow.SocialSecurityNumber;
            trustSideConsumerRow.Salutation = consumerRow.Salutation;
            trustSideConsumerRow.Suffix = consumerRow.Suffix;

            trustSideConsumerRow.BankAccountNumber = consumerRow.BankAccountNumber;
            trustSideConsumerRow.BankRoutingNumber = consumerRow.BankRoutingNumber;

            // keep track of what is fuzzed
            trustSideConsumerRow.FuzzedFields = Convert.ToInt32(Fuzzers.ConsumerFuzzedFields.None);

            this.dataModel.TrustSideConsumer.AddTrustSideConsumerRow(trustSideConsumerRow);

            // 'return' the unique Id of the row added
            trustSideConsumerId = trustSideConsumerRow.TrustSideConsumerId;
        }

        /// <summary>
        /// Writes the Consumer data to an XML file.  Data comes from two separate tables (Trust side and Debt side) and is
        /// combined into one XML file to be imported by the Guardian Script Loader utility.
        /// </summary>
        private void WriteConsumer()
        {
            XDocument xDocument = new XDocument();
            xDocument.Add(new XElement("script", new XAttribute("name", "Consumer")));
            xDocument.Root.Add(
                new XElement(
                    "client",
                    new XAttribute("name", "DataModelClient"),
                    new XAttribute("type", "DataModelClient, FluidTrade.ClientDataModel"),
                    new XAttribute("endpoint", "TcpDataModelEndpoint")));

            // Trust side
            foreach (DataModel.TrustSideConsumerRow trustConsumer in this.dataModel.TrustSideConsumer.Rows)
            {
                // Due to the huge number of consumers, each Consumer Record is a seperate transaction.
                XElement transactionElement = new XElement("transaction");

                // Create the Consumer record.
                XElement methodElement = new XElement("method", new XAttribute("name", "CreateConsumerEx"), new XAttribute("client", "DataModelClient"));

                // SSN is not optional
                methodElement.Add(new XElement("parameter", new XAttribute("name", "socialSecurityNumber"), new XAttribute("value", trustConsumer.SocialSecurityNumber)));

                // Optional fields:

                if (!trustConsumer.IsFirstNameNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "firstName"), new XAttribute("value", trustConsumer.FirstName)));

                if (!trustConsumer.IsLastNameNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "lastName"), new XAttribute("value", trustConsumer.LastName)));

                if (!trustConsumer.IsSalutationNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "salutation"), new XAttribute("value", trustConsumer.Salutation)));

                if (!trustConsumer.IsMiddleNameNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "middleName"), new XAttribute("value", trustConsumer.MiddleName)));

                if (!trustConsumer.IsSuffixNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "suffix"), new XAttribute("value", trustConsumer.Suffix)));

                if (!trustConsumer.IsAddress1Null())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "address1"), new XAttribute("value", trustConsumer.Address1)));

                if (!trustConsumer.IsAddress2Null())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "address2"), new XAttribute("value", trustConsumer.Address2)));

                if (!trustConsumer.IsCityNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "city"), new XAttribute("value", trustConsumer.City)));

                if (!trustConsumer.IsDateOfBirthNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "dateOfBirth"), new XAttribute("value", trustConsumer.DateOfBirth)));

                if (!trustConsumer.IsIsEmployedNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "isEmployed"), new XAttribute("value", trustConsumer.IsEmployed)));

                if (!trustConsumer.IsPostalCodeNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "postalCode"), new XAttribute("value", trustConsumer.PostalCode)));

                if (!trustConsumer.IsPhoneNumberNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "phoneNumber"), new XAttribute("value", trustConsumer.PhoneNumber)));

                if (!trustConsumer.IsProvinceCodeNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "provinceKey"), new XAttribute("value", trustConsumer.ProvinceCode)));

                if (!trustConsumer.IsBankAccountNumberNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "bankAccountNumber"), new XAttribute("value", trustConsumer.BankAccountNumber)));

                if (!trustConsumer.IsBankRoutingNumberNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "bankRoutingNumber"), new XAttribute("value", trustConsumer.BankRoutingNumber)));

                // Now add the Trust-side specific parameters

                // The Consumer Trust records can have identical SSN to the Consumer Debt records.  In order to keep one set of consumers from overwriting
                // the other set when the data is loaded, the configuration parameter is used to drive the load process to use different external identifier
                // pools for importing.
                methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "Default")));

                // The Consumers added through the Consumer Trust database will use a different external identifier than the ones entered through the Consumer
                // Debt side of the transaction.  
                methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId0"), new XAttribute("value", trustConsumer.ExternalId)));

                // This completes the transaction.
                transactionElement.Add(methodElement);
                xDocument.Root.Add(transactionElement);
            }

            // Debt side
            foreach (DataModel.DebtSideConsumerRow debtConsumer in this.dataModel.DebtSideConsumer.Rows)
            {
                // Due to the huge number of consumers, each Consumer Record is a seperate transaction.
                XElement transactionElement = new XElement("transaction");

                // Create the Consumer record.
                XElement methodElement = new XElement("method", new XAttribute("name", "CreateConsumerEx"), new XAttribute("client", "DataModelClient"));

                // SSN is not optional:
                methodElement.Add(new XElement("parameter", new XAttribute("name", "socialSecurityNumber"), new XAttribute("value", debtConsumer.SocialSecurityNumber)));

                // Add common method elements (parameters)
                if (!debtConsumer.IsFirstNameNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "firstName"), new XAttribute("value", debtConsumer.FirstName)));

                if (!debtConsumer.IsLastNameNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "lastName"), new XAttribute("value", debtConsumer.LastName)));

                if (!debtConsumer.IsSalutationNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "salutation"), new XAttribute("value", debtConsumer.Salutation)));

                if (!debtConsumer.IsMiddleNameNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "middleName"), new XAttribute("value", debtConsumer.MiddleName)));

                if (!debtConsumer.IsSuffixNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "suffix"), new XAttribute("value", debtConsumer.Suffix)));

                if (!debtConsumer.IsAddress1Null())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "address1"), new XAttribute("value", debtConsumer.Address1)));

                if (!debtConsumer.IsAddress2Null())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "address2"), new XAttribute("value", debtConsumer.Address2)));

                if (!debtConsumer.IsCityNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "city"), new XAttribute("value", debtConsumer.City)));

                if (!debtConsumer.IsDateOfBirthNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "dateOfBirth"), new XAttribute("value", debtConsumer.DateOfBirth)));

                if (!debtConsumer.IsIsEmployedNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "isEmployed"), new XAttribute("value", debtConsumer.IsEmployed)));

                if (!debtConsumer.IsPostalCodeNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "postalCode"), new XAttribute("value", debtConsumer.PostalCode)));

                if (!debtConsumer.IsPhoneNumberNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "phoneNumber"), new XAttribute("value", debtConsumer.PhoneNumber)));

                if (!debtConsumer.IsProvinceCodeNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "provinceKey"), new XAttribute("value", debtConsumer.ProvinceCode)));
                //
                // Now add the Debt-side specific parameters
                //

                // The Consumer Trust records can have identical SSN to the Consumer Debt records.  In order to keep one set of consumers from overwriting
                // the other set when the data is loaded, the configuration parameter is used to drive the load process to use different external identifier
                // pools for importing.
                methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "DEBT BUYER")));

                // The Consumers added through the Consumer Trust database will use a different external identifier than the ones entered through the Consumer
                // Debt side of the transaction. 
                methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId1"), new XAttribute("value", debtConsumer.ExternalId)));

                // This completes the transaction.
                transactionElement.Add(methodElement);
                xDocument.Root.Add(transactionElement);
            }

            // Write the contents of the XDocument to disk.
            xDocument.Save(Settings.Default.DataOutputLocation +
                           Settings.Default.ConsumerOutput);
        }

        /// <summary>        
        /// Add the Consumer Debt (Debt Holder) credit card info to the CreditCard data file
        /// </summary>
        /// <param name="debtCreditCardRow"></param>
        /// <param name="creditCardOutputFile"></param>
        private void AddConsumerDebtCreditCardDataToOutputDoc(
            DataModel.DebtCreditCardRow debtCreditCardRow,
            DataModel.OrganizationRow organizationRow,
            XDocument creditCardOutputFile)
        {
            // Each Credit Card is a transaction
            XElement transactionElement = new XElement("transaction");

            // Create the Credit Card record.
            XElement methodElement = new XElement("method", new XAttribute("name", "CreateCreditCardEx"), new XAttribute("client", "DataModelClient"));

            // The Consumer Trust records can have identical SSN to the Consumer Debt records.  In order to keep one set of consumers from overwriting
            // the other set when the data is loaded, the configuration parameter is used to drive the load process to use different external identifier
            // pools for importing.
            methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "DEBT BUYER"))); //Debt-side="DEBT BUYER" todo: get rid of this hardcoded string

            // A credit card's Isssuer just some "tag" field and is the same no matter which side it is on (trust or debt or real) 
            if (!debtCreditCardRow.IsRealDebtHolderNull())
            {
                methodElement.Add(new XElement("parameter", new XAttribute("name", "debtHolder"),
                                               new XAttribute("value", debtCreditCardRow.RealDebtHolder)));
            }

            methodElement.Add(new XElement("parameter", new XAttribute("name", "consumerKey"), new XAttribute("value", debtCreditCardRow.DebtSideConsumerRow.ExternalId)));
            methodElement.Add(new XElement("parameter", new XAttribute("name", "accountNumber"), new XAttribute("value", debtCreditCardRow.AccountCode)));
            methodElement.Add(new XElement("parameter", new XAttribute("name", "originalAccountNumber"), new XAttribute("value", debtCreditCardRow.OriginalAccountNumber)));

            methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId0"), new XAttribute("value", debtCreditCardRow.ExternalId)));
            methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId1"), new XAttribute("value", debtCreditCardRow.ExternalId)));

            if (!debtCreditCardRow.IsAccountBalanceNull())
                methodElement.Add(new XElement("parameter", new XAttribute("name", "accountBalance"), new XAttribute("value", debtCreditCardRow.AccountBalance)));

            // This completes the transaction.
            transactionElement.Add(methodElement);
            creditCardOutputFile.Root.Add(transactionElement);
        }

        /// <summary>
        /// Add the Consumer Trust (Debt Negotiator) credit card info to the CreditCard data file
        /// </summary>
        /// <param name="consumerTrustRow"></param>
        /// <param name="organizationRow"></param>
        /// <param name="creditCardOutputFile"></param>
        private void AddConsumerTrustCreditCardDataToOutputDoc(
            DataModel.ConsumerTrustRow consumerTrustRow,
            DataModel.OrganizationRow organizationRow,
            XDocument creditCardOutputFile)
        {
            // Get the credit cards associated with this trust record
            var trustCreditCards = GetTrustCreditCardList(consumerTrustRow.TrustSideConsumerRow);

            // for evey credit card that this consumer has in the trust record:
            foreach (var trustCreditCardRow in trustCreditCards)
            {
                // Each Credit Card is a transaction
                XElement transactionElement = new XElement("transaction");

                // Create the Credit Card record.
                XElement methodElement = new XElement("method", new XAttribute("name", "CreateCreditCardEx"), new XAttribute("client", "DataModelClient"));

                // The Consumer Trust records can have identical SSN to the Consumer Debt records.  In order to keep one set of consumers from overwriting
                // the other set when the data is loaded, the configuration parameter is used to drive the load process to use different external identifier
                // pools for importing.
                methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "Default"))); //Trust-side="default"

                // A credit card's Isssuer just some "tag" field and is the same no matter which side it is on (trust or debt or real)
                if (!trustCreditCardRow.IsRealDebtHolderNull())
                {
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "debtHolder"),
                                                   new XAttribute("value", trustCreditCardRow.RealDebtHolder)));
                }

                methodElement.Add(new XElement("parameter", new XAttribute("name", "consumerKey"),
                                                            new XAttribute("value", trustCreditCardRow.TrustSideConsumerRow.ExternalId)));

                methodElement.Add(new XElement("parameter", new XAttribute("name", "accountNumber"), new XAttribute("value", trustCreditCardRow.AccountCode)));
                methodElement.Add(new XElement("parameter", new XAttribute("name", "originalAccountNumber"), new XAttribute("value", trustCreditCardRow.OriginalAccountNumber)));
                methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId1"), new XAttribute("value", trustCreditCardRow.ExternalId)));

                if (!trustCreditCardRow.IsAccountBalanceNull())
                    methodElement.Add(new XElement("parameter", new XAttribute("name", "accountBalance"), new XAttribute("value", trustCreditCardRow.AccountBalance)));

                // This completes the transaction.
                transactionElement.Add(methodElement);
                creditCardOutputFile.Root.Add(transactionElement);
            }
        }

        /// <summary>
        /// Set up the Credit Card document that gets consumed by the script loader 
        /// </summary>
        /// <returns></returns>
        private XDocument InitializeCreditCardOutputFile()
        {
            // The Credit Card data is written out to a file format that is compatible with the Guardian script loader utility.
            XDocument creditCardDocument = new XDocument();
            creditCardDocument.Add(new XElement("script", new XAttribute("name", "Credit Card")));
            creditCardDocument.Root.Add(
                new XElement(
                    "client",
                    new XAttribute("name", "DataModelClient"),
                    new XAttribute("type", "DataModelClient, FluidTrade.ClientDataModel"),
                    new XAttribute("endpoint", "TcpDataModelEndpoint")));

            return creditCardDocument;
        }

        /// <summary>
        /// Saves the contents of the Credit Card output file we have been building out to disk
        /// </summary>
        /// <param name="creditCardOutputFile"></param>
        private void WriteCreditCardDataFile(XDocument creditCardOutputFile)
        {
            // Write the contents of the XDocument to disk.
            creditCardOutputFile.Save(Settings.Default.DataOutputLocation +
                Settings.Default.CreditCardOutput);
        }

        /// <summary>
        /// Write the Consumer Debt (Debt Holder) and associated CreditCard information to an XML file to be 
        /// consumed by Script Loader
        /// </summary>
        /// <param name="creditCardDocument"></param>
        private void WriteConsumerDebtOutputData(XDocument creditCardDocument)
        {
            // Create the Consumer Debt Output file
            XDocument consumerDebtOutputFile = this.CreateNewScriptLoaderInputFile(OrganizationType.DebtBuyer.ToString());

            // Distribute the ConsumerDebt orders evenly across Debt Holder organizations (tenants)
            foreach (DataModel.OrganizationRow organizationRow in this.dataModel.Organization)
            {
                // Only care about DebtHolder organizations here
                if (organizationRow.Type == (int)OrganizationType.DebtBuyer)
                {
                    // Create the output file that will contain the working orders for this Debt Holder organization
                    XDocument workingOrderOutputFile = this.CreateNewScriptLoaderInputFile(organizationRow.Name + " Orders");

                    // Get the list of Debt Classes associated with the organization (so we can evenly divide the orders between them)
                    // Index used to grab a Debt record out of a randomly shuffled list
                    int rndDebtHolderIndex;
                    for (int index = organizationRow.StartingOrderId; index <= organizationRow.EndingOrderId; index++)
                    {
                        // get the next debt row from the randomly shuffled list
                        rndDebtHolderIndex = this.consumerDebtIds[index];
                        DataModel.ConsumerDebtRow consumerDebtRow =
                            this.dataModel.ConsumerDebt[rndDebtHolderIndex];

                        // Wrap the related records in a transaction.
                        XElement transactionElement = new XElement("transaction");

                        // These methods are to be committed as a unit and represent a "Consumer Debt" security.
                        transactionElement.Add(ConsumerDebt.CreateEntityRecord(consumerDebtRow, organizationRow));
                        transactionElement.Add(ConsumerDebt.CreateSecurityRecord(consumerDebtRow, organizationRow));
                        transactionElement.Add(ConsumerDebt.CreateConsumerDebtRecord(consumerDebtRow, organizationRow));

                        // This completes the transaction for the ConsumerDebt record
                        consumerDebtOutputFile.Root.Add(transactionElement);

                        // Now add the associated Credit Card record to the Credit Card document 
                        // (there is always only 1 CC per Debt Holder record)
                        AddConsumerDebtCreditCardDataToOutputDoc(consumerDebtRow.DebtCreditCardRow, organizationRow, creditCardDocument);

                        // Write out the appropriate working order for the current organization (tenant) we are working with here.  
                        // Working orders themselves are not tenanted but this DataGen application divides up the working orders 
                        // across output files named after the organizations.  This gives us some flexibility when using script 
                        // loader control which organizations get working orders loaded for them or not.
                        this.GenerateDebtHolderWorkingOrder(workingOrderOutputFile, organizationRow, consumerDebtRow);
                    }

                    // Write out the corresponding working order file for the current organization
                    WriteWorkingOrdersToDisk(organizationRow, workingOrderOutputFile);
                }
            }

            // Write the Consumer Debt (DebtHolder) output to disk. This set of data is used to load the ConsumerDebt table 
            consumerDebtOutputFile.Save(Settings.Default.DataOutputLocation +
                Settings.Default.ConsumerDebtOutput);
        }

        /// <summary>
        /// Write the Consumer Trust (Debt Negotiator) and associated CreditCard information to an XML file to be 
        /// consumed by Script Loader
        /// </summary>
        /// <param name="creditCardDocument"></param>
        private void WriteConsumerTrustOutputData(XDocument creditCardDocument)
        {
            // Create the Consumer Trust Output file
            XDocument consumerTrustOutputFile = CreateNewScriptLoaderInputFile(OrganizationType.DebtNegotiator.ToString());

            // Distribute the ConsumerTrust orders evenly across Debt Negotiator organizations (tenants)
            foreach (DataModel.OrganizationRow organizationRow in this.dataModel.Organization)
            {
                // Only care about Debt Negotiator organizations here
                if (organizationRow.Type == (int)OrganizationType.DebtNegotiator)
                {
                    // Create the output file that will contain the working orders for this Debt Holder organization
                    XDocument workingOrderOutputFile = this.CreateNewScriptLoaderInputFile(organizationRow.Name + " Orders");

                    // Index used to grab a Debt record out of a randomly shuffled list
                    int rndDebtHolderIndex;
                    for (int index = organizationRow.StartingOrderId; index <= organizationRow.EndingOrderId; index++)
                    {
                        // get the next trust row from the randomly shuffled list
                        rndDebtHolderIndex = this.consumerTrustIds[index];
                        DataModel.ConsumerTrustRow consumerTrustRow =
                            this.dataModel.ConsumerTrust[rndDebtHolderIndex];

                        // Wrap the related records in a transaction.
                        XElement transactionElement = new XElement("transaction");

                        // These methods are to be committed as a unit and represent a "Consumer Debt" security.
                        transactionElement.Add(ConsumerTrust.CreateEntityRecord(consumerTrustRow, organizationRow));
                        transactionElement.Add(ConsumerTrust.CreateSecurityRecord(consumerTrustRow, organizationRow));
                        transactionElement.Add(ConsumerTrust.CreateConsumerTrustRecord(consumerTrustRow, organizationRow));

                        // This completes the transaction for the ConsumerTrust record
                        consumerTrustOutputFile.Root.Add(transactionElement);

                        // Now add the associated Credit Card record to the Credit Card document 
                        // (there is always only 1 CC per Debt Holder record)
                        AddConsumerTrustCreditCardDataToOutputDoc(consumerTrustRow, organizationRow, creditCardDocument);

                        // Write out the appropriate working order for the current organization (tenant) we are working with here.  Working orders
                        // themselves are not tenanted but this DataGen application divides up the working orders across output files named after the
                        // organizations.  This gives us some flexibility when using script loader control which organizations get working orders
                        // loaded for them or not.
                        this.GenerateDebtNegotiatorWorkingOrder(workingOrderOutputFile, organizationRow, consumerTrustRow);
                    }

                    // Write out the corresponding working order file for the current organization
                    WriteWorkingOrdersToDisk(organizationRow, workingOrderOutputFile);
                }
            }

            // Write the contents of the XDocument to disk.
            consumerTrustOutputFile.Save(Settings.Default.DataOutputLocation +
                           Settings.Default.ConsumerTrustOutput);
        }

        /// <summary>
        /// Returns the number of organizations of a particular type.
        /// </summary>
        /// <param name="organizationType">The organization's type.</param>
        /// <returns></returns>
        private int GetNumberOfOrganizations(OrganizationType organizationType)
        {
            // Go through each row in the Organization table and see if it is the type we are looking for
            int count = 0;
            foreach (DataModel.OrganizationRow organizationRow in this.dataModel.Organization)
            {
                if (organizationRow.Type == (int)organizationType)
                {
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// Initialize a file compatible with the Script Loader input format
        /// </summary>
        /// <param name="organizationType"></param>
        /// <returns></returns>
        private XDocument CreateNewScriptLoaderInputFile(string scriptName)
        {
            XDocument inputFile = new XDocument();

            // The data is written out to a file format that is compatible with the Guardian script loader utility.
            inputFile.Add(new XElement("script", new XAttribute("name", scriptName)));
            inputFile.Root.Add(
                new XElement(
                    "client",
                    new XAttribute("name", "DataModelClient"),
                    new XAttribute("type", "DataModelClient, FluidTrade.ClientDataModel"),
                    new XAttribute("endpoint", "TcpDataModelEndpoint")));

            return inputFile;
        }

        /// <summary>
        /// Takes the passed-in working order doc and divides the working orders in it among 
        /// separate files 
        /// </summary>
        /// <param name="originalWorkingOrderDoc"></param>
        /// <param name="organizationName"></param>
        private void SplitUpWorkingOrderFile(XDocument originalWorkingOrderDoc, string organizationName)
        {
            // one (or fewer) files really isn't dividing up the orders into multiple files...just return
            if (Settings.Default.NumberOfWorkingOrderFilesPerOrganization <= 1)
                return;

            // Generate an array of working order docs, the length of which is based on the config setting'
            // that dictates how many separate working order files we should generate per organization
            XDocument[] workingOrderDocs =
                GetNewWorkingOrderDocuments(
                Settings.Default.NumberOfWorkingOrderFilesPerOrganization,
                organizationName);

            // Get the transaction elements from the document.  Each transaction element in this XML file equates
            // to a working order.
            IEnumerable<XElement> originalTransactions =
                from element in originalWorkingOrderDoc.Root.Elements()
                where element.Name == "transaction"
                select element;

            int docIndex = 0;

            // for all the original transactions: 
            foreach (XElement transaction in originalTransactions)
            {
                // divide them up between the newly created files
                workingOrderDocs[docIndex].Root.Add(transaction);

                // circular index through the available files
                docIndex = (++docIndex >= workingOrderDocs.Length ? 0 : docIndex);
            }

            // reset docIndex to help name our files
            docIndex = 1;

            // now save each document to disk
            foreach (XDocument doc in workingOrderDocs)
            {
                doc.Save(Settings.Default.DataOutputLocation +
                    organizationName + " Orders_" + (docIndex++) + ".xml");
            }
        }

        /// <summary>
        /// Write out organization-specific working order data to disk
        /// </summary>
        /// <param name="organizationRow"></param>
        /// <param name="workingOrderOutputFile"></param>
        private void WriteWorkingOrdersToDisk(
            DataModel.OrganizationRow organizationRow,
            XDocument workingOrderOutputFile)
        {
            // Write out the Organization-specific script file to disk
            workingOrderOutputFile.Save(Settings.Default.DataOutputLocation + organizationRow.Name + " Orders.xml");

            // See if we need to split up the organization's working orders into a bunch of small
            // ones.  this helps us tests importing them in parallel via script loader
            if (Settings.Default.SplitUpOrganizationWorkingOrders)
            {
                SplitUpWorkingOrderFile(workingOrderOutputFile, organizationRow.Name);
            }
        }

        /// <summary>
        /// Return an array of initialized working order documents
        /// </summary>
        /// <param name="numDocs"></param>
        /// <returns></returns>
        public XDocument[] GetNewWorkingOrderDocuments(int numDocs, string name)
        {
            XDocument[] docs = new XDocument[numDocs];

            for (int index = 0; index < numDocs; index++)
            {
                docs[index] = this.CreateNewScriptLoaderInputFile(name + " Orders_" + (index + 1).ToString());
            }

            return docs;
        }

        /// <summary>
        /// Writes out the flat files used to test the Import functionality that customers will use to load the data into the system
        /// </summary>
        /// <param name="random"></param>
        private void WriteFlatImportFiles()
        {
            // For each organization in the Organization table, write out the import file.
            foreach (DataModel.OrganizationRow organizationRow in this.dataModel.Organization)
            {
                switch ((OrganizationType)organizationRow.Type)
                {
                    case OrganizationType.DebtNegotiator:

                        this.GenerateDebtNegotiatorXmlImportFile(organizationRow);
                        break;

                    case OrganizationType.DebtBuyer:

                        this.GenerateDebtHolderXmlImportFile(organizationRow);
                        break;
                }
            }
        }

        /// <summary>
        /// Write out the Trust-side, XML-based data that will be used for importing into the DB.  This is 
        /// essentially the data format that Debt Negotiators will be using to populate the system.
        /// </summary>
        /// <param name="random"></param>
        private void GenerateDebtNegotiatorXmlImportFile(DataModel.OrganizationRow organizationRow)
        {
            DebtNegotiator.DebtNegotiatorRecordRow negotiatorRecord;

            // Make sure the tables are clear since this method can be called from within a loop.
            for (int i = 0; i < Settings.Default.NumberOfImportFilesPerOrganization; i++)
            {
                this.dataSetDebtNegotiators[i].DebtNegotiatorRecord.Clear();
            }

            int rndConsumerTrustIndex = 0;
            int importFileIndex = 0;

            // create an XML-based record for each ConsumerTrust's CreditCard row based on the sequence specified in the organization table
            for (int index = organizationRow.StartingOrderId; index <= organizationRow.EndingOrderId; index++)
            {
                // get the next trust row from the randomly shuffled list
                rndConsumerTrustIndex = this.consumerTrustIds[index];
                DataModel.ConsumerTrustRow trustRow = this.dataModel.ConsumerTrust[rndConsumerTrustIndex];

                // get the credit cards associated with this trust record
                var trustCreditCards = GetTrustCreditCardList(trustRow.TrustSideConsumerRow);

                // for every credit card that this consumer has in the trust record:
                foreach (var trustCreditCardRow in trustCreditCards)
                {
                    // create a new row
                    negotiatorRecord = this.dataSetDebtNegotiators[importFileIndex].DebtNegotiatorRecord.NewDebtNegotiatorRecordRow();

                    // required fields 
                    negotiatorRecord.SocialSecurityNumber = trustRow.TrustSideConsumerRow.SocialSecurityNumber;
                    negotiatorRecord.OriginalAccountNumber = trustCreditCardRow.OriginalAccountNumber;

                    negotiatorRecord.AccountCode = trustCreditCardRow.AccountCode;
                    negotiatorRecord.AccountBalance = trustCreditCardRow.AccountBalance;

                    // 'optional' fields - Consumer
                    negotiatorRecord.Salutation = trustRow.TrustSideConsumerRow.Salutation;
                    negotiatorRecord.FirstName = trustRow.TrustSideConsumerRow.FirstName;
                    negotiatorRecord.LastName = trustRow.TrustSideConsumerRow.LastName;
                    negotiatorRecord.MiddleName = trustRow.TrustSideConsumerRow.MiddleName;
                    negotiatorRecord.Suffix = trustRow.TrustSideConsumerRow.Suffix;
                    negotiatorRecord.Address1 = trustRow.TrustSideConsumerRow.Address1;
                    negotiatorRecord.Address2 = trustRow.TrustSideConsumerRow.Address2;
                    negotiatorRecord.City = trustRow.TrustSideConsumerRow.City;
                    negotiatorRecord.PostalCode = trustRow.TrustSideConsumerRow.PostalCode;
                    negotiatorRecord.PhoneNumber = trustRow.TrustSideConsumerRow.PhoneNumber;
                    negotiatorRecord.ProvinceCode = trustRow.TrustSideConsumerRow.ProvinceCode;

                    // assign a unique customer code which is unique per trust organization, hence using organization name 
                    // as a part of the ID string.
                    negotiatorRecord.CustomerCode = string.Format("{0}-{1}", organizationRow.Name, trustRow.CustomerCode);

                    if (!trustRow.TrustSideConsumerRow.IsDateOfBirthNull())
                    {
                        // ensure a date string with format YYYY-MM-DD
                        negotiatorRecord.DateOfBirth = trustRow.TrustSideConsumerRow.DateOfBirth.ToString("u").Substring(0, 10);
                    }

                    if (!trustRow.TrustSideConsumerRow.IsIsEmployedNull())
                        negotiatorRecord.IsEmployed = trustRow.TrustSideConsumerRow.IsEmployed;

                    // 'optional' fields - CreditCard

                    negotiatorRecord.DebtHolder = trustCreditCardRow.RealDebtHolder;

                    // 'optional' fields - ConsumerTrust

                    negotiatorRecord.SavingsBalance = trustRow.SavingsBalance;
                    negotiatorRecord.SavingsEntityCode = trustRow.SavingsEntityCode;
                    negotiatorRecord.BankAccountNumber = trustRow.TrustSideConsumerRow.BankAccountNumber;
                    negotiatorRecord.BankRoutingNumber = trustRow.TrustSideConsumerRow.BankRoutingNumber;

                    // Vendor Code (unique per debt holder organization)
                    negotiatorRecord.VendorCode = organizationRow.Name;

                    // add the row 
                    this.dataSetDebtNegotiators[importFileIndex].DebtNegotiatorRecord.AddDebtNegotiatorRecordRow(negotiatorRecord);
                }

                // circular index through the available import file 'slots'
                importFileIndex = (++importFileIndex >= Settings.Default.NumberOfImportFilesPerOrganization
                    ? 0 : importFileIndex);
            }

            Stopwatch s = new Stopwatch();
            s.Restart();

            // write out the import files to disk
            this.WriteFlatImportFilesToDisk(organizationRow.Name, OrganizationType.DebtNegotiator);

            s.Stop();
        }

        /// <summary>
        /// Write the flat import file(s) out to disk
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="organizationType"></param>
        private void WriteFlatImportFilesToDisk(string organizationName, OrganizationType organizationType)
        {
            if (Settings.Default.NumberOfImportFilesPerOrganization > 1)
            {
                // we have to write out multiple files, give them a unique number in the file name
                for (int importFileIndex = 0;
                     importFileIndex < Settings.Default.NumberOfImportFilesPerOrganization;
                     importFileIndex++)
                {
                    // Write the XML representation of the tables out to disk
                    using (StreamWriter sw =
                        new StreamWriter(Settings.Default.DataOutputLocation + organizationName +
                                         " Import_" + (importFileIndex + 1) + ".xml"))
                    {
                        // Write out the XML import file
                        if (organizationType == (int)OrganizationType.DebtNegotiator)
                        {
                            this.dataSetDebtNegotiators[importFileIndex].DebtNegotiatorRecord.WriteXml(sw);
                        }

                        if (organizationType == OrganizationType.DebtBuyer)
                        {
                            this.dataSetDebtHolders[importFileIndex].DebtHolderRecord.WriteXml(sw);
                        }
                    }
                }
            }
            else
            {
                // we have just one file, don't put any numbers in the file name
                using (StreamWriter sw =
                    new StreamWriter(Settings.Default.DataOutputLocation + organizationName +
                                     " Import.xml"))
                {
                    // Write out the XML import file
                    if (organizationType == OrganizationType.DebtNegotiator)
                    {
                        // Write out the XML import file
                        this.dataSetDebtNegotiators[0].DebtNegotiatorRecord.WriteXml(sw);
                    }
                    if (organizationType == OrganizationType.DebtBuyer)
                    {
                        this.dataSetDebtHolders[0].DebtHolderRecord.WriteXml(sw);
                    }
                }
            }
        }

        /// <summary>
        /// Write out the Debt-side, XML-based data that will be used for importing into the DB.  This is 
        /// essentially the data format that Debt Holders will be using to populate the system.
        /// </summary>
        /// <param name="random"></param>
        /// <param name="organizationName"></param>
        private void GenerateDebtHolderXmlImportFile(DataModel.OrganizationRow organizationRow)
        {
            // make sure the table(s) are clear since this method can be called from within a loop.
            for (int i = 0; i < Settings.Default.NumberOfImportFilesPerOrganization; i++)
            {
                this.dataSetDebtHolders[i].DebtHolderRecord.Clear();
            }

            int rndDebtHolderIndex = 0;

            int importFileIndex = 0;

            // create an xml-based record for each ConsumerDebt row in the sequence specified in the organization table
            for (int index = organizationRow.StartingOrderId; index <= organizationRow.EndingOrderId; index++)
            {
                // get the next debt row from the randomly shuffled list
                rndDebtHolderIndex = this.consumerDebtIds[index];
                DataModel.ConsumerDebtRow debtRow =
                    this.dataModel.ConsumerDebt[rndDebtHolderIndex];

                // create a new holder record row
                DebtHolder.DebtHolderRecordRow holderRecord = this.dataSetDebtHolders[importFileIndex].DebtHolderRecord.NewDebtHolderRecordRow();

                // required fields 
                holderRecord.SocialSecurityNumber = debtRow.DebtCreditCardRow.DebtSideConsumerRow.SocialSecurityNumber;
                holderRecord.OriginalAccountNumber = debtRow.DebtCreditCardRow.OriginalAccountNumber;

                // 'optional' fields - Consumer
                holderRecord.FirstName = debtRow.DebtCreditCardRow.DebtSideConsumerRow.FirstName;
                holderRecord.LastName = debtRow.DebtCreditCardRow.DebtSideConsumerRow.LastName;
                holderRecord.MiddleName = debtRow.DebtCreditCardRow.DebtSideConsumerRow.MiddleName;
                holderRecord.Suffix = debtRow.DebtCreditCardRow.DebtSideConsumerRow.Suffix;
                holderRecord.Address1 = debtRow.DebtCreditCardRow.DebtSideConsumerRow.Address1;
                holderRecord.Address2 = debtRow.DebtCreditCardRow.DebtSideConsumerRow.Address2;
                holderRecord.City = debtRow.DebtCreditCardRow.DebtSideConsumerRow.City;
                holderRecord.PostalCode = debtRow.DebtCreditCardRow.DebtSideConsumerRow.PostalCode;
                holderRecord.PhoneNumber = debtRow.DebtCreditCardRow.DebtSideConsumerRow.PhoneNumber;
                holderRecord.ProvinceCode = debtRow.DebtCreditCardRow.DebtSideConsumerRow.ProvinceCode;

                if (!debtRow.DebtCreditCardRow.DebtSideConsumerRow.IsDateOfBirthNull())
                {
                    // ensure a date string with format yyyy-mm-dd
                    holderRecord.DateOfBirth = debtRow.DebtCreditCardRow.DebtSideConsumerRow.DateOfBirth.ToString("u").Substring(0, 10);
                }

                // ensure a date string with format yyyy-mm-dd
                if (!debtRow.IsDateOfDelinquencyNull())
                {
                    holderRecord.DateOfDelinquency = debtRow.DateOfDelinquency.ToString("u").Substring(0, 10);
                }

                //  - CreditCard

                holderRecord.AccountCode = debtRow.DebtCreditCardRow.AccountCode;
                holderRecord.AccountBalance = debtRow.DebtCreditCardRow.AccountBalance;
                holderRecord.OriginalAccountNumber = debtRow.DebtCreditCardRow.OriginalAccountNumber;

                holderRecord.DebtHolder = debtRow.DebtCreditCardRow.RealDebtHolder;

                // Vendor Code (unique per debt holder organization)
                holderRecord.VendorCode = organizationRow.Name;

                this.dataSetDebtHolders[importFileIndex].DebtHolderRecord.AddDebtHolderRecordRow(holderRecord);

                // circular index through the available import file 'slots'
                importFileIndex = (++importFileIndex >= Settings.Default.NumberOfImportFilesPerOrganization
                    ? 0 : importFileIndex);
            }

            Stopwatch s = new Stopwatch();
            s.Restart();

            // write out the import files to disk
            WriteFlatImportFilesToDisk(organizationRow.Name, OrganizationType.DebtBuyer);

            s.Stop();
        }

        /// <summary>
        /// Updates the Organization table with information on how to divide up the working orders 
        /// among the existing organizations based on type (Holder or Negotiator) and how many 
        /// organizations of said type exist.
        /// </summary>
        private void UpdateOrganizationTableWorkingOrderCounts()
        {
            int numDebtHolderOrganizations = this.GetNumberOfOrganizations(OrganizationType.DebtBuyer);
            int numDebtNegotiatorOrganizations = this.GetNumberOfOrganizations(OrganizationType.DebtNegotiator);

            // make sure we have at least one organization on each side of the Chinese wall.
            if (numDebtNegotiatorOrganizations < 1 || numDebtHolderOrganizations < 1)
            {
                throw new SystemException(string.Format("Need at least one organization on each side of the wall"));
            }

            // break down the Debt Holder orders evenly among organizations
            int debtHolderOrdersPerOrganization = this.dataModel.ConsumerDebt.Count() / numDebtHolderOrganizations;
            int debtHolderOrdersPerOrganizationRemainder = this.dataModel.ConsumerDebt.Count() % numDebtHolderOrganizations;
            int debtNegotiatorsOrdersPerOrganization = this.dataModel.ConsumerTrust.Count() / numDebtNegotiatorOrganizations;
            int debtNegotiatorsOrdersPerOrganizationRemainder = this.dataModel.ConsumerTrust.Count() % numDebtNegotiatorOrganizations;
            int negotiatorOrgCount = 0;  // keeps track of the number of Negotiation organizations
            int holderOrgCount = 0;      // keeps track of the number of Holder organizations
            int negotiatorIdMarker = 0;  // marks the Id of the debt negotiation record currently in 'context'
            int holderIdMarker = 0;      // marks the Id of the debt holder record currently in 'context'

            // Go through each organization in the table and divide up the orders among the organization types
            foreach (DataModel.OrganizationRow organizationRow in this.dataModel.Organization)
            {
                // Assign the number of working orders per Debt Negotiator organization 
                if (organizationRow.Type == (int)OrganizationType.DebtNegotiator)
                {
                    organizationRow.WorkingOrderCount = debtNegotiatorsOrdersPerOrganization;

                    // if 1) there are > 1 Negotiation organizations and
                    //    2) we reached the last negotiator organization 
                    // then give any 'extra' (remainder) working order(s) to the last negotiator organization                                        
                    if ((numDebtNegotiatorOrganizations > 1) &&
                        (++negotiatorOrgCount == numDebtNegotiatorOrganizations))
                    {
                        organizationRow.WorkingOrderCount += debtNegotiatorsOrdersPerOrganizationRemainder;
                    }

                    // use the start and end order Ids to divide orders up between multiple organizations
                    organizationRow.StartingOrderId = negotiatorIdMarker;
                    organizationRow.EndingOrderId = negotiatorIdMarker + organizationRow.WorkingOrderCount - 1;
                    negotiatorIdMarker = organizationRow.EndingOrderId + 1;
                }

                // Assign the number of working orders per Debt Holder organization 
                else
                {
                    if (organizationRow.Type == (int)OrganizationType.DebtBuyer)
                    {
                        organizationRow.WorkingOrderCount = debtHolderOrdersPerOrganization;

                        // if 1) there are > 1 holder organizations and
                        //    2) we reached the last holder organization 
                        // then give any 'extra' (remainder) working order(s) to the last holder organization    
                        if (numDebtHolderOrganizations > 1 && ++holderOrgCount == numDebtHolderOrganizations)
                        {
                            organizationRow.WorkingOrderCount += debtHolderOrdersPerOrganizationRemainder;
                        }

                        // use the start and end order Ids to divide orders up between multiple organizations
                        organizationRow.StartingOrderId = holderIdMarker;
                        organizationRow.EndingOrderId = holderIdMarker + organizationRow.WorkingOrderCount - 1;
                        holderIdMarker = organizationRow.EndingOrderId + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Generate a Debt Negotiator working order
        /// </summary>
        /// <param name="workingOrderDoc"></param>
        /// <param name="organizationRow"></param>
        private void GenerateDebtNegotiatorWorkingOrder(
            XDocument workingOrderDoc,
            DataModel.OrganizationRow organizationRow,
            DataModel.ConsumerTrustRow consumerTrustRow)
        {
            XElement transactionElement = new XElement("transaction");

            // Generate a unique ID for this working order
            Guid workingOrderId = Guid.NewGuid();

            // Generate a unique blotter key for this order based on the debt class it is associated with
            string blotterKey = "BLOTTER";
            transactionElement.Add(ConsumerTrust.CreateWorkingOrder(consumerTrustRow, workingOrderId, blotterKey, organizationRow));

            workingOrderDoc.Root.Add(transactionElement);
        }

        /// <summary>
        /// Generate a Debt Holder working order
        /// </summary>
        /// <param name="debtClasses"></param>
        /// <param name="debtClassIndex"></param>
        /// <param name="workingOrderDoc"></param>
        /// <param name="organizationRow"></param>
        /// <param name="consumerDebtRow"></param>
        private void GenerateDebtHolderWorkingOrder(
            XDocument workingOrderDoc,
            DataModel.OrganizationRow organizationRow,
            DataModel.ConsumerDebtRow consumerDebtRow)
        {
            XElement transactionElement = new XElement("transaction");

            // Generate a unique ID for this working order
            Guid workingOrderId = Guid.NewGuid();

            // Generate a unique blotter key for this order based on the debt class it is associated with
            string blotterKey = "BLOTTER";
            transactionElement.Add(ConsumerDebt.CreateWorkingOrder(consumerDebtRow, workingOrderId, blotterKey, organizationRow));
            workingOrderDoc.Root.Add(transactionElement);
        }

        /// <summary>
        /// Validate any config data as needed
        /// </summary>
        private void ValidateConfigurationData()
        {
            /// Debt and Trust record counts
            if (Settings.Default.NumberOfTrustRecords >= Settings.Default.NumberOfConsumerRecords ||
                Settings.Default.NumberOfDebtRecords >= Settings.Default.NumberOfConsumerRecords)
            {
                throw new SystemException(
                    "NumberOfTrustRecords and NumberOfDebtRecords must be less than or equal to NumberOfConsumerRecords");
            }

            // Match count must be less than or equal to the number of cosumers in the TrustConsumer table...assuming there
            // is only one CC per consumer at a bare minimum
            if (Settings.Default.NumberOfMatches > Settings.Default.NumberOfTrustRecords ||
                Settings.Default.NumberOfMatches > Settings.Default.NumberOfDebtRecords)
            {
                throw new SystemException(
                    "NumberOfMatches must be less than or equal to NumberOfTrustRecords and less than or equal to NumberOfDebtRecords");
            }

            // The total number of consumers must be greater or equal to the amount of Trust Records + amount of Debt Records 
            // minus the numer of matches between the two.  This assures we have enough non-matching consumers to divide up on either side
            if (Settings.Default.NumberOfConsumerRecords < (Settings.Default.NumberOfTrustRecords +
                                                                       Settings.Default.NumberOfDebtRecords -
                                                                       Settings.Default.NumberOfMatches))
            {
                throw new SystemException(
                    "NumberOfConsumerRecords must be greater than or equal to the amount of Trust Records" +
                    " + amount of Debt Records minus the numer of matches between the two.");
            }

            // Fuzz counts need to be within a valid range of 0 to total number of matches
            if (!Settings.Default.TrustSideFuzzCount.IsWithinRange(0, Settings.Default.NumberOfMatches) ||
                !Settings.Default.DebtSideFuzzCount.IsWithinRange(0, Settings.Default.NumberOfMatches) ||
                !Settings.Default.CommonFuzzCount.IsWithinRange(0, Settings.Default.NumberOfMatches))
            {
                throw new SystemException(
                    "Fuzz Counts must be less than or equal to the specified number of matches and greater than or equal to zero");
            }

            // make sure TrustSideNicknamePercentage is valid
            if (!Settings.Default.TrustSideFuzzNicknamePercentage.IsWithinRange(0, 100))
            {
                throw new SystemException(
                    "TrustSideNicknamePercentage must be a valid percentage value (0-100)");
            }

            // make sure DebtSideNicknamePercentage is valid
            if (!Settings.Default.DebtSideFuzzNicknamePercentage.IsWithinRange(0, 100))
            {
                throw new SystemException(
                    "DebtSideNicknamePercentage must be a valid percentage value (0-100)");
            }

            // make sure at least one import file will be created per organization
            if (Settings.Default.NumberOfImportFilesPerOrganization < 1)
            {
                throw new SystemException(
                    "Need at least one Import File per organization");
            }
        }

        /// <summary>
        /// Sort of a unit test to make sure the matching count integrity holds
        /// </summary>
        private void ValidateMatchCounts()
        {
            // Sanity Check / Debugging Unit Test to ensure match numbers are correct
            int realConsumerMatches = 0;
            foreach (DataModel.RealConsumerRow realConsumer in this.dataModel.RealConsumer)
            {
                realConsumerMatches += realConsumer.MatchCount;
            }

            if (realConsumerMatches != Settings.Default.NumberOfMatches)
                throw new SystemException("RealConsumer Match Count <> Settings.Default.NumberOfMatches");

            // skt: need to count matches on both sides of the chinese wall

        }
    }
}
