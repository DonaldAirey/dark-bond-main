// <copyright file="Organization.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.Data
{
    using System.Xml.Linq;

    class Organization
    {
        public static XElement CreateEntityRecord(DataModel.OrganizationRow organizationRow)
        {
            return Script.CreateEntityRecord(
                organizationRow.Name,
                ((OrganizationType)organizationRow.Type).ToString(),
                organizationRow.Name,
                ((OrganizationType)organizationRow.Type).ToString());
        }

        public static XElement CreateDebtClass(DataModel.OrganizationRow organizationRow, string debtClass)
        {

            XElement transactionElement = new XElement("transaction");

            transactionElement.Add(Script.CreateEntityRecord(organizationRow.Name + " " + debtClass.ToUpper(), "BLOTTER", debtClass, "DEBT CLASS"));
            transactionElement.Add(Script.CreateTreeRelation(organizationRow.Name, organizationRow.Name + " " + debtClass.ToUpper()));
            transactionElement.Add(
                    Script.CreateMethod("CreateBlotterEx",
                        Script.CreateParameter("configurationId", "Default"),
                        Script.CreateParameter("entityKey", organizationRow.Name + " " + debtClass.ToUpper() + " BLOTTER"),
                        Script.CreateParameter("partyTypeKey", "USE PARENT")));
            transactionElement.Add(
                    Script.CreateMethod("CreateBranchEx",
                        Script.CreateParameter("blotterKey", organizationRow.Name + " " + debtClass.ToUpper() + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("shortName", debtClass)));
            return transactionElement;
        }

        public static XElement CreateConsumerTrustBlotterConfig(string debtClass)
        {

            XElement transactionElement = new XElement("transaction");
            string externalId = debtClass.ToUpper();

            // These don't really need to be in a single transaction, but it makes them easier to return.
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " DESTINATION ORDER"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "DESTINATION ORDER REPORT"),
                        Script.CreateParameter("reportTypeKey", "DESTINATION ORDER")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " DESTINATION ORDER DETAIL"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "DESTINATION ORDER DETAIL REPORT"),
                        Script.CreateParameter("reportTypeKey", "DESTINATION ORDER DETAIL")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " EXECUTION"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "EXECUTION REPORT"),
                        Script.CreateParameter("reportTypeKey", "EXECUTION")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " EXECUTION DETAIL"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "EXECUTION DETAIL REPORT"),
                        Script.CreateParameter("reportTypeKey", "EXECUTION DETAIL")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " MATCH"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "CONSUMER TRUST MATCH REPORT"),
                        Script.CreateParameter("reportTypeKey", "MATCH")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " SOURCE ORDER"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "SOURCE ORDER REPORT"),
                        Script.CreateParameter("reportTypeKey", "SOURCE ORDER")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " SOURCE ORDER DETAIL"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "SOURCE ORDER DETAIL REPORT"),
                        Script.CreateParameter("reportTypeKey", "SOURCE ORDER DETAIL")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " WORKING ORDER"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "CONSUMER TRUST WORKING ORDER REPORT"),
                        Script.CreateParameter("reportTypeKey", "WORKING ORDER"),
                        Script.CreateParameter("schemaId", "WORKING ORDER HEADER")));

            return transactionElement;

        }

        public static XElement CreateConsumerDebtBlotterConfig(string debtClass)
        {

            XElement transactionElement = new XElement("transaction");
            string externalId = debtClass.ToUpper();

            // These don't really need to be in a single transaction, but it makes them easier to return.
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " DESTINATION ORDER"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "DESTINATION ORDER REPORT"),
                        Script.CreateParameter("reportTypeKey", "DESTINATION ORDER")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " DESTINATION ORDER DETAIL"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "DESTINATION ORDER DETAIL REPORT"),
                        Script.CreateParameter("reportTypeKey", "DESTINATION ORDER DETAIL")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " EXECUTION"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "EXECUTION REPORT"),
                        Script.CreateParameter("reportTypeKey", "EXECUTION")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " EXECUTION DETAIL"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "EXECUTION DETAIL REPORT"),
                        Script.CreateParameter("reportTypeKey", "EXECUTION DETAIL")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " MATCH"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "CONSUMER TRUST MATCH REPORT"),
                        Script.CreateParameter("reportTypeKey", "MATCH")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " SOURCE ORDER"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "SOURCE ORDER REPORT"),
                        Script.CreateParameter("reportTypeKey", "SOURCE ORDER")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " SOURCE ORDER DETAIL"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "SOURCE ORDER DETAIL REPORT"),
                        Script.CreateParameter("reportTypeKey", "SOURCE ORDER DETAIL")));
            transactionElement.Add(Script.CreateMethod(
                        "CreateBlotterConfigurationEx",
                        Script.CreateParameter("externalId0", externalId + " WORKING ORDER"),
                        Script.CreateParameter("blotterKey", externalId + " BLOTTER"),
                        Script.CreateParameter("configurationId", "default"),
                        Script.CreateParameter("reportKey", "CONSUMER DEBT WORKING ORDER REPORT"),
                        Script.CreateParameter("reportTypeKey", "WORKING ORDER"),
                        Script.CreateParameter("schemaId", "WORKING ORDER HEADER")));
            return transactionElement;
        }
    }
}
