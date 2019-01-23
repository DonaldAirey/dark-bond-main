// <copyright file="Importer.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Transactions;
    using System.Xml.Linq;
    using DarkBond.ClientModel;
    using DarkBond.LicenseManager.ImportService;

    /// <summary>
    /// Runs a file-based script of operations against the data model.
    /// </summary>
    [Export]
    public class Importer : IDisposable
    {
        /// <summary>
        /// Default amount of time (in seconds) between ticks when the progress indicator is giving feedback.
        /// </summary>
        private const int DefaultTickTime = 10;

        /// <summary>
        /// The channel binding.
        /// </summary>
        private Binding binding;

        /// <summary>
        /// The security token.
        /// </summary>
        private ClientSecurityToken clientSecurityToken;

        /// <summary>
        /// The endpoint address.
        /// </summary>
        private EndpointAddress endpointAddress;

        /// <summary>
        /// The name of the file to load.
        /// </summary>
        private string fileNameField;

        /// <summary>
        /// The client that handles the import service requests.
        /// </summary>
        private ImportServiceClient importServiceClient;

        /// <summary>
        /// A stack of the transactions.
        /// </summary>
        private Stack<TransactionScope> tranactionScopeStack = new Stack<TransactionScope>();

        /// <summary>
        /// The amount of information output to the console.
        /// </summary>
        private Verbosity verbosityField = Verbosity.Minimal;

        /// <summary>
        /// Initializes a new instance of the <see cref="Importer"/> class.
        /// </summary>
        /// <param name="binding">The channel binding.</param>
        /// <param name="endpointAddress">The endpoint address.</param>
        /// <param name="clientSecurityToken">The security token.</param>
        public Importer(Binding binding, EndpointAddress endpointAddress, ClientSecurityToken clientSecurityToken)
        {
            // Initialize the object.
            this.binding = binding;
            this.endpointAddress = endpointAddress;
            this.clientSecurityToken = clientSecurityToken;

            // Initialize the client.
            this.OnChannelFaulted(this, new EventArgs());

            // This is used to print the statistics at the end of the execution.
            this.MethodCount = 0;
        }

        /// <summary>
        /// Element types to describe values used by parameters.
        /// </summary>
        private enum ParameterElementType
        {
            /// <summary>
            /// An instruction to import an entire file for the parameter.
            /// </summary>
            Import,

            /// <summary>
            /// Load a binary file as a Base64 string.
            /// </summary>
            Load,

            /// <summary>
            /// No special processing of the parameter.
            /// </summary>
            None,

            /// <summary>
            /// Load a text file.
            /// </summary>
            Value
        }

        /// <summary>
        /// Gets or sets a value indicating whether the script had errors.
        /// </summary>
        internal bool HasErrors { get; set; }

        /// <summary>
        /// Gets or sets the number of method executed.
        /// </summary>
        internal int MethodCount { get; set; }

        /// <summary>
        /// Gets or sets the user-friendly name of the script.
        /// </summary>
        internal string ScriptName { get; set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Call the virtual method to allow derived classes to clean up resources.
            this.Dispose(true);

            // Since we took care of cleaning up the resources, there is no need to call the finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Loads the script from the given file name.
        /// </summary>
        /// <param name="fileName">The location of the script.</param>
        /// <param name="verbosity">The amount of information to display on the console.</param>
        internal void Load(string fileName, Verbosity verbosity)
        {
            // Initialize the object.
            this.fileNameField = fileName;
            this.verbosityField = verbosity;

            // This flag is set when an error occurs anywhere in the processing of the XML file.
            this.HasErrors = false;

            // This will display the name of the loaded file when verbose output is requested.
            if (this.verbosityField == Verbosity.Verbose)
            {
                Trace.TraceInformation(string.Format(CultureInfo.CurrentCulture, Import.Properties.Resources.LoadingMessage, this.fileNameField));
            }

            // Load the script.
            XDocument xDocument = XDocument.Load(this.fileNameField);

            // The script name is stored in the root node.  The name is used in status and debugging messages.
            XAttribute nameAttribute = xDocument.Root.Attribute("name");
            this.ScriptName = nameAttribute == null ? "<unnamed>" : nameAttribute.Value;

            // The script contains statements that are interpreted and executed.  Most of the statements are 'methods' which are CRUD operations that
            // are executed against the data model.  There are also statements that define a client channel for connecting to the server and
            // statements that bind a group of statements into a single transaction.
            foreach (XElement xElement in xDocument.Root.Elements())
            {
                try
                {
                    // The element name defines how the node is handled.
                    switch (xElement.Name.LocalName)
                    {
                        case "method":

                            // Execute a single method (without a transaction).
                            if (this.ExecuteMethod(xElement))
                            {
                                this.MethodCount++;
                            }

                            break;

                        case "transaction":

                            // This will create the child elements of this node as a single unit.
                            this.ExecuteTransaction(xElement);
                            break;
                    }
                }
                catch (MessageSecurityException messageSecurityException)
                {
                    // Extract the real reason this failed to run on the server and log it.
                    Exception innerException = messageSecurityException.InnerException;
                    Trace.TraceError(string.Format(CultureInfo.CurrentCulture, "{0} {1}", innerException.Message, innerException.StackTrace));

                    // A security exception is a terminal exception.  Throw it again to end the script.
                    throw innerException;
                }
                catch (CommunicationException communicationException)
                {
                    Trace.TraceError(communicationException.Message);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">true to indicate that the object is being disposed, false to indicate that the object is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Dispose of the managed resources.
            if (disposing)
            {
                ((IDisposable)this.importServiceClient).Dispose();
            }
        }

        /// <summary>
        /// Converts an XElement describing a value to a native CLR value.
        /// </summary>
        /// <param name="type">The target data type.</param>
        /// <param name="parameterElement">An XElement containing a string representation of a value.</param>
        /// <param name="directoryName">The directory where external files can be found.</param>
        /// <returns>The native CLR value of the described value.</returns>
        private static object ConvertElement(Type type, XElement parameterElement, string directoryName)
        {
            // If the target parameter is an array, then construct a vector parameter.
            if (type == typeof(string[]))
            {
                // The values can be found in a single attribute of the 'parameter' element or be listed as children.  This list collects both methods of describing
                // values and constructs a single array when all elements and attributes are parsed.
                List<string> valueList = new List<string>();

                // An attribute can be used to describe a value.  An optional 'Type' attribute can specify what type of conversion is used to evaluate the CLR value.
                XAttribute valueAttribute = parameterElement.Attribute("value");
                if (valueAttribute != null)
                {
                    valueList.Add(valueAttribute.Value);
                }

                // It is possible to specify the value using the content of an XML element or through an "import" statement.  This will cycle through any nodes of
                // the parameter looking for additional nodes containing the data for the parameter.
                foreach (XObject xObject in parameterElement.Nodes())
                {
                    // This uses the element content as the value for the parameter.
                    XText xText = xObject as XText;
                    if (xText != null)
                    {
                        valueList.Add(xText.Value);
                    }

                    // Elements can be nested inside the parameter element to greater detail to the parameter.
                    XElement xElement = xObject as XElement;
                    if (xElement != null)
                    {
                        // Values for a key can be specified as child elements of the parameter.
                        if (xElement.Name == "value")
                        {
                            valueList.Add(xElement.Value);
                        }

                        // This special instruction allows the value of a parameter to come from an external file.  This is used primary to load XML content into a
                        // record.
                        if (xElement.Name == "import")
                        {
                            XAttribute xAttribute = xElement.Attribute("path");
                            string path = Path.IsPathRooted(xAttribute.Value) ? xAttribute.Value : Path.Combine(directoryName, xAttribute.Value);
                            XDocument xDocument = XDocument.Load(path);
                            valueList.Add(xDocument.ToString());
                        }

                        // A 'load' element will read a binary resource into a byte array.
                        if (xElement.Name == "load")
                        {
                            XAttribute xAttribute = xElement.Attribute("path");
                            string path = Path.IsPathRooted(xAttribute.Value) ? xAttribute.Value : Path.Combine(directoryName, xAttribute.Value);
                            byte[] binaryData;
                            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                binaryData = new byte[fileStream.Length];
                                fileStream.Read(binaryData, 0, Convert.ToInt32(fileStream.Length));
                            }

                            return Convert.ToBase64String(binaryData);
                        }
                    }
                }

                // This array is most often used as a key to find a record in a table.
                return valueList.ToArray();
            }
            else
            {
                // If no elements are specified, then we'll simply convert the attribute into a value for this parameter.  This is by far the most
                // common method of providing values for parameter.
                XAttribute valueAttribute = parameterElement.Attribute("value");
                return valueAttribute == null ? string.Empty : valueAttribute.Value;
            }
        }

        /// <summary>
        /// Process a transaction.
        /// </summary>
        /// <param name="xElement">The node containing the transaction.</param>
        private void ExecuteTransaction(XElement xElement)
        {
            // Create an explicit required transaction for the methods found at this node that will wait 10 minutes before timing out.
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10)))
            {
                // Transactions can be nested.  When they are nested, the transaction will only complete when the most outer layer has completed.
                this.tranactionScopeStack.Push(transactionScope);

                // These variables are used to count the number of successful methods.  If a transaction fails, then all the methods in the transaction have
                // failed.  The total count of methods executed by the class does not reflect the failed methods in the failed transaction.
                bool isSuccessful = true;
                int methodCount = 0;

                // This will execute each of the methods in the transaction.  If a single method fails, then all the methods, even the successful ones, will be
                // rolled back.
                foreach (XElement childElement in xElement.Elements())
                {
                    // We don't want continue to execute methods or sub-transactions after an error has invalidated the current transaction.
                    if (isSuccessful)
                    {
                        // The element name defines how the node is handled.
                        switch (childElement.Name.LocalName)
                        {
                            case "method":

                                // Execute a single method (without a transaction).
                                if (this.ExecuteMethod(childElement))
                                {
                                    methodCount++;
                                }
                                else
                                {
                                    isSuccessful = false;
                                }

                                break;

                            case "transaction":

                                // This will create the child elements of this node as a single unit.
                                this.ExecuteTransaction(childElement);
                                break;
                        }
                    }
                }

                // At this point, all the methods were successful and the transaction can be committed and the global counter of good methods reflects the
                // successes.  If we don't call the 'Complete' method, then the transaction will be rolled back implicitly when this scope exits.
                if (isSuccessful)
                {
                    this.MethodCount += methodCount;
                    transactionScope.Complete();
                }

                // This will restore the previous transaction scope when all the elements of this scope have been evaluated.
                this.tranactionScopeStack.Pop();
            }
        }

        /// <summary>
        /// Creates a method plan from the parameters listed.
        /// </summary>
        /// <param name="methodElement">An XML node where the method and parameters are found.</param>
        /// <returns>A value indicating whether the method has executed without errors.</returns>
        private bool ExecuteMethod(XElement methodElement)
        {
            // Indicates the success or failure of an individual method execution.
            bool isSuccessful = false;

            try
            {
                // Reflection is used here to find the method to be executed.
                XAttribute methodNameAttribute = methodElement.Attribute("name");
                MethodInfo methodInfo = this.importServiceClient.GetType().GetMethod(methodNameAttribute.Value);
                if (methodInfo == null)
                {
                    throw new InvalidOperationException("The method " + methodNameAttribute.Value + " isn't part of the library");
                }

                // This will pull apart the XML that contains the parameters and construct a dictionary of method arguments.
                Dictionary<string, XElement> parameterDictionary = new Dictionary<string, XElement>();
                foreach (XElement parameterElement in methodElement.Elements("parameter"))
                {
                    XAttribute parameterNameAttribute = parameterElement.Attribute("name");
                    if (parameterDictionary.ContainsKey(parameterNameAttribute.Value))
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                "The parameter {0} has already been added.",
                                parameterNameAttribute.Value));
                    }

                    parameterDictionary.Add(parameterNameAttribute.Value, parameterElement);
                }

                // This will correlate the parameter in the XML with the parameter of the actual method found through reflection and convert the
                // parameter into the proper destination type.  The end result is a parameter array that is compatible with a reflection call to the
                // method specified in the XML.
                ParameterInfo[] parameterInfoArray = methodInfo.GetParameters();
                object[] parameterArray = new object[parameterInfoArray.Length];
                for (int index = 0; index < parameterInfoArray.Length; index++)
                {
                    ParameterInfo parameterInfo = parameterInfoArray[index];
                    XElement parameterElement;
                    if (parameterDictionary.TryGetValue(parameterInfo.Name, out parameterElement))
                    {
                        parameterArray[index] = Importer.ConvertElement(
                            parameterInfo.ParameterType,
                            parameterElement,
                            Path.GetDirectoryName(this.fileNameField));
                    }
                }

                try
                {
                    // At this point, the only thing left to do is call the method using the parsed parameters.
                    methodInfo.Invoke(this.importServiceClient, parameterArray);
                }
                catch (TargetInvocationException targetInvocationException)
                {
                    // We use reflection to execute the method, so naturally we're going to get a reflection error.  This will dig out the real
                    // reason that this method failed and throw it.
                    throw targetInvocationException.InnerException;
                }

                // The method invocation was successful at this point.
                isSuccessful = true;
            }
            catch (FaultException<RecordNotFoundFault> recordNotFoundException)
            {
                // The record wasn't found.
                Trace.TraceError(
                    Import.Properties.Resources.RecordNotFoundError,
                    recordNotFoundException.Detail.TableName,
                    CommonConversion.FromArray(recordNotFoundException.Detail.KeyElements));
            }
            catch (FaultException<InvalidOperationFault> invalidOperationFaultException)
            {
                // An invalid operation occurred.
                Trace.TraceError("Invalid Operation: {0}", invalidOperationFaultException.Detail.Message);
            }
            catch (FaultException<ConstraintFault> constraintFaultException)
            {
                // An invalid operation occurred.
                Trace.TraceError("Constraint Violation: Operation {0} violated {1}.", constraintFaultException.Detail.Operation, constraintFaultException.Detail.Constraint);
            }
            catch (FaultException<FormatFault> formatFaultException)
            {
                // The string arguments couldn't be converted to native types on the service.
                Trace.TraceError("Format Error: {0}", formatFaultException.Detail.Message);
            }
            catch (FaultException<ExceptionDetail> exceptionDetail)
            {
                // This is a general purpose exception for debugging.
                Trace.TraceError(exceptionDetail.Message);
            }

            // This is the final indication of whether the method was successful or not.
            return isSuccessful;
        }

        /// <summary>
        /// Handles a faulted (or uninitialized) channel.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="eventArgs">The event data.</param>
        private void OnChannelFaulted(object sender, EventArgs eventArgs)
        {
            this.importServiceClient = new ImportServiceClient(this.binding, this.endpointAddress, this.clientSecurityToken);
            this.importServiceClient.InnerChannel.Faulted += this.OnChannelFaulted;
        }
    }
}