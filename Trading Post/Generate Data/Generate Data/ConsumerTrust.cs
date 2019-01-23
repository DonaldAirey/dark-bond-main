﻿// <copyright file="ConsumerTrust.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.Data
{
    using System;
    using System.Xml.Linq;

    class ConsumerTrust
	{

        /// <summary>
        /// Create a method invocation to add the Entity record.
        /// </summary>
        /// <param name="consumerTrustRow"></param>
        /// <param name="organizationRow"></param>
        /// <returns></returns>
        public static XElement CreateEntityRecord(
            DataModel.ConsumerTrustRow consumerTrustRow, 
            DataModel.OrganizationRow organizationRow)
		{

			//<method name="CreateEntityEx" client="DataModelClient">
			XElement methodElement = new XElement("method", new XAttribute("name", "CreateEntityEx"), new XAttribute("client", "DataModelClient"));

			//  <parameter name="configurationId" value="Default" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "Default")));

			//  <parameter name="createdTime" value="10/12/009 12:42:02" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "createdTime"), new XAttribute("value", DateTime.UtcNow.ToString())));

            //  <parameter name="externalId0" value="" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId0"),
                new XAttribute("value", Convert.ToString(consumerTrustRow.TrustSideConsumerRow.ExternalId))));

			//  <parameter name="externalId7" value="" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId7"), 
                new XAttribute("value", Convert.ToString(consumerTrustRow.TrustSideConsumerRow.ExternalId))));

			//  <parameter name="hidden" value="false" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "hidden"), new XAttribute("value", "False")));

			//  <parameter name="imageKey" value="OBJECT" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "imageKey"), new XAttribute("value", "OBJECT")));

			//  <parameter name="modifiedTime" value="10/12/009 12:42:02" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "modifiedTime"), new XAttribute("value", DateTime.UtcNow.ToString())));

			//  <parameter name="name" value="OCCIDENTAL PETE CORP" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "name"), new XAttribute("value", consumerTrustRow.SavingsEntityCode)));

			//  <parameter name="readOnly" value="false" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "readOnly"), new XAttribute("value", "False")));

			//  <parameter name="typeKey" value="DEBT" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "typeKey"), new XAttribute("value", "CONSUMER TRUST")));

			//</method>
			return methodElement;

		}
        
        /// <summary>
        /// Create a method invocation to add the security record.
        /// </summary>
        /// <param name="consumerTrustRow"></param>
        /// <param name="organizationRow"></param>
        /// <returns></returns>
        public static XElement CreateSecurityRecord(
            DataModel.ConsumerTrustRow consumerTrustRow, 
            DataModel.OrganizationRow organizationRow)
		{

			//<method name="CreateSecurityEx" client="DataModelClient">
			XElement methodElement = new XElement("method", new XAttribute("name", "CreateSecurityEx"), new XAttribute("client", "DataModelClient"));

			//  <parameter name="configurationId" value="CUSIP" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "Default")));

			//  <parameter name="countryKey" value="US" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "countryKey"), new XAttribute("value", "US")));

			//  <parameter name="entityKey" value="674599BF1" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "entityKey"), new XAttribute("value", consumerTrustRow.TrustSideConsumerRow.ExternalId)));

			//  <parameter name="priceFactor" value="1.0" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "priceFactor"), new XAttribute("value", Convert.ToString(1.0))));

			//  <parameter name="quantityFactor" value="1.0" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "quantityFactor"), new XAttribute("value", Convert.ToString(1.0))));

			//  <parameter name="symbol" value="674599BF1" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "symbol"), new XAttribute("value", consumerTrustRow.TrustSideConsumerRow.ExternalId)));

			//  <parameter name="volumeCategoryKey" value="UNKNOWN" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "volumeCategoryKey"), new XAttribute("value", "UNKNOWN")));

			//</method>
			return methodElement;
		}
        
        /// <summary>
        /// Creates a method invocation to create a Consumer Debt record.
        /// </summary>
        /// <param name="consumerTrustRow"></param>
        /// <param name="organizationRow"></param>
        /// <returns></returns>
        public static XElement CreateConsumerTrustRecord(
            DataModel.ConsumerTrustRow consumerTrustRow, 
            DataModel.OrganizationRow organizationRow)
		{
			// Create the Consumer record.
			XElement methodElement = new XElement("method", new XAttribute("name", "CreateConsumerTrustEx"), new XAttribute("client", "DataModelClient"));

			// Savings Balance
			methodElement.Add(new XElement("parameter", new XAttribute("name", "savingsBalance"), new XAttribute("value", Convert.ToString(consumerTrustRow.SavingsBalance))));

			// Configuration Id
			methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "Default")));

			// Consumer Key
            methodElement.Add(new XElement("parameter", new XAttribute("name", "consumerKey"), new XAttribute("value", consumerTrustRow.TrustSideConsumerRow.ExternalId)));

			// Security Key
			methodElement.Add(new XElement("parameter", new XAttribute("name", "securityKey"), new XAttribute("value", consumerTrustRow.TrustSideConsumerRow.ExternalId)));

            // VendorCode
            methodElement.Add(new XElement("parameter", new XAttribute("name", "vendorCode"), new XAttribute("value", consumerTrustRow.VendorCode)));

            // This element represents a method invocation to create a Consumer Debt record.
			return methodElement;
		}

        
        /// <summary>
        /// Generate the working order.
        /// </summary>
        /// <param name="consumerTrustRow"></param>
        /// <param name="workingOrderId"></param>
        /// <param name="blotterKey"></param>
        /// <param name="organizationRow"></param>
        /// <returns>The XML describing the invocation of a method to add a working order.</returns>
		public static XElement CreateWorkingOrder(
            DataModel.ConsumerTrustRow consumerTrustRow, 
            Guid workingOrderId, 
            String blotterKey, 
            DataModel.OrganizationRow organizationRow)
		{ 
			//<method name="CreateWorkingOrderEx" client="DataModelClient">
			XElement methodElement = new XElement("method", new XAttribute("name", "CreateWorkingOrderEx"), new XAttribute("client", "DataModelClient"));

			//  <parameter name="blotterKey" value="HILDEGARD KOHLER BLOTTER" />
			methodElement.Add(new XElement("parameter", new XAttribute("name","blotterKey"), new XAttribute("value", blotterKey)));

			//  <parameter name="configurationId" value="CUSIP" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "Default")));

			//  <parameter name="createdTime" value="9/4/2008 10:11:11 AM" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "createdTime"), new XAttribute("value", Convert.ToString(DateTime.Now))));

			//  <parameter name="crossingKey" value="ALWAYS MATCH" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "crossingKey"), new XAttribute("value", "ALWAYS MATCH")));

			//  <parameter name="externalId0" value="{7dc57574-d275-4210-8410-44539ecb0ba3}" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId0"), new XAttribute("value", workingOrderId.ToString("B"))));

			//  <parameter name="isBrokerMatch" value="true" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "isBrokerMatch"), new XAttribute("value", "true")));

			//  <parameter name="isHedgeMatch" value="true" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "isHedgeMatch"), new XAttribute("value", "true")));

			//  <parameter name="isInstitutionMatch" value="true" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "isInstitutionMatch"), new XAttribute("value", "true")));

			//  <parameter name="modifiedTime" value="9/4/2008 10:11:11 AM" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "modifiedTime"), new XAttribute("value", Convert.ToString(DateTime.Now))));

			//  <parameter name="orderTypeKey" value="MKT" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "orderTypeKey"), new XAttribute("value", "MKT")));

			//  <parameter name="securityKeyBySecurityId" value="925524AJ9" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "securityKeyBySecurityId"), new XAttribute("value", consumerTrustRow.TrustSideConsumerRow.ExternalId)));

			//  <parameter name="securityKeyBySettlementId" value="USD" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "securityKeyBySettlementId"), new XAttribute("value", "USD")));

			//  <parameter name="settlementDate" value="9/9/2008 10:11:11 AM" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "settlementDate"), new XAttribute("value", Convert.ToString(DateTime.Now))));

			//  <parameter name="sideKey" value="SELL" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "sideKey"), new XAttribute("value", "SELL")));

            //  <parameter name="statusKey" value="SUBMITTED" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "statusKey"), new XAttribute("value", "SUBMITTED")));

			//  <parameter name="timeInForceKey" value="GTC" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "timeInForceKey"), new XAttribute("value", "GTC")));

			//  <parameter name="tradeDate" value="9/4/2008 10:11:11 AM" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "tradeDate"), new XAttribute("value", Convert.ToString(DateTime.Now))));

			//  <parameter name="userKeyByCreatedUserId" value="" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "userKey"), new XAttribute("value", organizationRow.RepresentativeUser)));

			//</method>
			return methodElement;

		}

		/// <summary>
		/// Generate the working order.
		/// </summary>
		/// <param name="consumerTrustRow">The Consumer Debt row that is to be turned into a working order.</param>
		/// <returns>The XML describing the invocation of a method to add a working order.</returns>
		public static XElement CreateSourceOrder(DataModel.ConsumerTrustRow consumerTrustRow, Guid workingOrderId, String blotterKey)
		{

			//<method name="CreateSourceOrderEx" client="DataModelClient">
			XElement methodElement = new XElement("method", new XAttribute("name", "CreateSourceOrderEx"), new XAttribute("client", "DataModelClient"));

			//  <parameter name="blotterKey" value="UNIQUE KEY" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "blotterKey"), new XAttribute("value", blotterKey)));

			//  <parameter name="configurationId" value="CUSIP" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "configurationId"), new XAttribute("value", "Default")));

			//  <parameter name="createdTime" value="2008-09-04T10:11:11.488-04:00" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "createdTime"), new XAttribute("value", Convert.ToString(DateTime.Now))));

			//  <parameter name="externalId0" value="{01b28ac8-3e0f-465d-8ea0-4ced19630f88}" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "externalId0"), new XAttribute("value", Guid.NewGuid().ToString("B"))));

			//  <parameter name="modifiedTime" value="9/4/2008 10:11:11 AM" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "modifiedTime"), new XAttribute("value", Convert.ToString(DateTime.Now))));

			//  <parameter name="orderedQuantity" value="4000.0" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "orderedQuantity"), new XAttribute("value", Convert.ToString(0.0M))));

			//  <parameter name="orderTypeKey" value="MKT" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "orderTypeKey"), new XAttribute("value", "MKT")));

			//  <parameter name="securityKeyBySecurityId" value="925524AJ9" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "securityKeyBySecurityId"), new XAttribute("value", consumerTrustRow.TrustSideConsumerRow.ExternalId)));

			//  <parameter name="securityKeyBySettlementId" value="USD" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "securityKeyBySettlementId"), new XAttribute("value", "USD")));

			//  <parameter name="settlementDate" value="2008-09-09T10:11:11.486-04:00" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "settlementDate"), new XAttribute("value", Convert.ToString(DateTime.Now))));

			//  <parameter name="sideKey" value="SELL" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "sideKey"), new XAttribute("value", "SELL")));

            //  <parameter name="statusKey" value="SUBMITTED" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "statusKey"), new XAttribute("value", "SUBMITTED")));

			//  <parameter name="timeInForceKey" value="GTC" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "timeInForceKey"), new XAttribute("value", "GTC")));

			//  <parameter name="tradeDate" value="9/4/2008 10:11:11 AM" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "tradeDate"), new XAttribute("value", Convert.ToString(DateTime.Now))));

            //  <parameter name="userKey" value="" />
            methodElement.Add(new XElement("parameter", new XAttribute("name", "userKey"), new XAttribute("value", "Karen Holden")));

			//  <parameter name="workingOrderKey" value="{7dc57574-d275-4210-8410-44539ecb0ba3}" />
			methodElement.Add(new XElement("parameter", new XAttribute("name", "workingOrderKey"), new XAttribute("value", workingOrderId.ToString("B"))));

			//</method>
			return methodElement;

		}

	}

}
