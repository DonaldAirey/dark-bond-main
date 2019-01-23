// <copyright file="ShellViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.TradingPost.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Configuration;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using DarkBond.Navigation;
    using DarkBond.ServiceModel;
    using DarkBond.TradingPost.Common.Strings;
    using DarkBond.ViewModels;
    using DarkBond.ViewModels.Input;

    /// <summary>
    /// The view model for the application shell.
    /// </summary>
    public class ShellViewModel : ShellViewModelBase
    {
        // Create a client for HTTP communication.
        private HttpClient client;

        private CompositionContext compositionContext;

        private string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];

        private string tenant = ConfigurationManager.AppSettings["ida:Tenant"];

        private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        public ShellViewModel(CompositionContext compositionContext, INavigationService navigationService) : base(navigationService)
        {
            // Initialize the object.
            this.compositionContext = compositionContext;

            // Initialize the HTTP channel.
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(@"http://localhost:53729/");
            this.client.DefaultRequestHeaders.Accept.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Run the test when the user is signed in.
            GlobalCommands.SignedIn.RegisterCommand(new DelegateCommand(this.RunAsync));
        }

        /// <summary>
        /// Gets the title of the application.
        /// </summary>
        public string Title
        {
            get
            {
                return Labels.ApplicationName;
            }
        }

        private async void RunAsync()
        {
            ClientSecurityToken clientSecurityToken = this.compositionContext.GetExport<ClientSecurityToken>();
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientSecurityToken.Value as string);

            List<Product> products = await this.GetProductsAsync();

            Guid productId = Guid.Parse("{779ba43c-83e4-4bde-b400-584863e47a3a}");

            try
            {
                HttpResponseMessage httpResponseMessage = await this.InsertProductAsync(
                    productId,
                    "Washing Machine",
                    "Home Goods");
            }
            catch { }

            Product product = await this.GetProductAsync(productId);

            product.Name = "Way New Washing Machine";

            try
            {
                HttpResponseMessage httpResponseMessage = await this.UpdateProductAsync(
                    productId,
                    product.Name,
                    product.Description,
                    product.RowVersion);
            }
            catch { }

            products = await this.GetProductsAsync();
            System.Diagnostics.Debug.WriteLine("Total Products Before Delete = " + products.Count.ToString());

            product = await this.GetProductAsync(productId);
            HttpResponseMessage deleteResponseMessage = await this.DeleteProductAsync(product.ProductId, product.RowVersion);

            products = await this.GetProductsAsync();
            System.Diagnostics.Debug.WriteLine("Total Products After Delete = " + products.Count.ToString());
        }

        private async Task<List<Product>> GetProductsAsync()
        {
            List<Product> products = null;

            HttpResponseMessage response = await client.GetAsync("products");
            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<List<Product>>();
            }

            return products;
        }

        private async Task<Product> GetProductAsync(Guid productId)
        {
            Product product = null;
            HttpResponseMessage response = await this.client.GetAsync("products/" + productId.ToString());
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }

            return product;
        }

        private async Task<HttpResponseMessage> DeleteProductAsync(Guid productId, long rowVersion)
        {
            this.client.DefaultRequestHeaders.IfMatch.Clear();
            this.client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue($"\"{rowVersion}\""));
            HttpResponseMessage httpResponseMessage = await this.client.DeleteAsync("products/" + productId.ToString());
            httpResponseMessage.EnsureSuccessStatusCode();
            return httpResponseMessage;
        }

        private async Task<HttpResponseMessage> InsertProductAsync(Guid productId, string name, string description)
        {
            Product product = new Product
            {
                ProductId = productId,
                Name = name,
                Description = description
            };

            HttpResponseMessage httpResponseMessage = await this.client.PutAsJsonAsync<Product>("products/", product);
            httpResponseMessage.EnsureSuccessStatusCode();

            return httpResponseMessage;
        }

        private async Task<HttpResponseMessage> UpdateProductAsync(Guid productId, string name, string description, long rowVersion)
        {
            Product product = new Product
            {
                ProductId = productId,
                Name = name,
                Description = description
            };

            this.client.DefaultRequestHeaders.Add("If-Match", $"\"{rowVersion}\"");
            HttpResponseMessage httpResponseMessage = await this.client.PostAsJsonAsync<Product>("products/" + productId.ToString(), product);
            httpResponseMessage.EnsureSuccessStatusCode();

            return httpResponseMessage;
        }
    }
}