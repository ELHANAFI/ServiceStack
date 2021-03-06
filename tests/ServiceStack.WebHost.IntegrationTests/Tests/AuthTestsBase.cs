using System;
using System.Net;
using NUnit.Framework;
using ServiceStack.Clients;
using ServiceStack.Clients;
using ServiceStack.Text;

namespace ServiceStack.WebHost.IntegrationTests.Tests
{
	public class AuthTestsBase
	{
	    public const string BaseUri = Config.ServiceStackBaseUri;
		public const string AdminEmail = "admin@servicestack.com";
	    public const string AuthSecret = "secretz";
		private const string AdminPassword = "E8828A3E26884CE0B345D0D2DFED358A";

		private IServiceClient serviceClient;
		public IServiceClient ServiceClient
		{
			get
			{
				return serviceClient ?? (serviceClient = new JsonServiceClient(BaseUri));
			}
		}

		public Register CreateAdminUser()
		{
			var registration = new Register {
				UserName = "Admin",
				DisplayName = "The Admin User",
				Email = AdminEmail, //this email is automatically assigned as Admin in Web.Config
				FirstName = "Admin",
				LastName = "User",
				Password = AdminPassword,
			};
			try
			{
				ServiceClient.Send(registration);
			}
			catch (WebServiceException ex)
			{
				("Error while creating Admin User: " + ex.Message).Print();
				ex.ResponseDto.PrintDump();
			}
			return registration;
		}

		public JsonServiceClient Login(string userName, string password)
		{
			var client = new JsonServiceClient(BaseUri);
			client.Send(new Authenticate {
				UserName = userName,
				Password = password,
				RememberMe = true,
			});

			return client;
		}

		public JsonServiceClient AuthenticateWithAdminUser()
		{
			var registration = CreateAdminUser();
			var adminServiceClient = new JsonServiceClient(BaseUri);
			adminServiceClient.Send(new Authenticate {
				UserName = registration.UserName,
				Password = registration.Password,
				RememberMe = true,
			});

			return adminServiceClient;
		}

		protected void AssertUnAuthorized(WebServiceException webEx)
		{
			Assert.That(webEx.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
			Assert.That(webEx.StatusDescription, Is.EqualTo(HttpStatusCode.Unauthorized.ToString()));
		}

	}

}