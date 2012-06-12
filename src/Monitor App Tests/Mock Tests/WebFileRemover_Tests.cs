using System;
using System.Net;
using NUnit.Framework;

/*
 * Solution: SiteDocs Challenge Console App
 * Author: Helmut Wollenberg
 * Date: June 10-11, 2012
 */

namespace Monitor_App_Tests.Mock_Tests
{
    [TestFixture]
    public class WebFileRemover_Tests
    {
        private string _fileWebLocation;

        [SetUp]
        public void InitTest()
        {
            _fileWebLocation = "http://testurl.com/Image_Test.png"; // Sample Value.
        }

        [Test]
        public void Deleted_File_From_Web_Server()
        {
            var remover = new WebFileRemover();

            try
            {
                Assert.IsTrue(remover.RemoveTheFile(_fileWebLocation));
            }
            catch (Exception ex)
            {
                if ((ex is WebException) || (ex is InvalidOperationException) || (ex is NotSupportedException))
                {
                    Assert.Fail();
                }
                else
                {
                    throw;
                }
            }
            
        }

        /*
         * WebFileRemover stub implementation.
         * Uses an HttpWebRequest with DELETE Method.
         */
        public class WebFileRemover
        {
            public bool RemoveTheFile(string fileURI)
            {
                // Create the request.
                var request = (HttpWebRequest) WebRequest.Create(fileURI);
                request.Method = "DELETE";
                request.ContentType = "application/x-www-form-urlencoded";

                // Get the response from sending the request.
                var response = (HttpWebResponse) request.GetResponse();

                return response.StatusCode.Equals(HttpStatusCode.OK);
            }
        }

        /*
         * A stub (based on WebRequest) containing methods used by the WebFileRemover.
         */
        public abstract class WebRequest
        {
            public abstract string Method { get; set; }
            public abstract string ContentType { get; set; }

            public static WebRequest Create(string requestURI)
            {
                return new HttpWebRequest();
            }
        }

        /*
         * A stub (based on HttpWebRequest) containing methods used by the WebFileRemover.
         */
        public class HttpWebRequest : WebRequest
        {
            private readonly HttpWebResponse _webResponse = new HttpWebResponse();

            public override string Method { get; set; }
            public override string ContentType { get; set; }

            public HttpWebResponse GetResponse()
            {
                _webResponse.StatusCode = HttpStatusCode.OK;
                return _webResponse;
            }
        }

        /*
         * A stub (based on HttpWebResponse) containing methods used by the WebFileRemover.
         */
        public class HttpWebResponse
        {
            public HttpStatusCode StatusCode { get; set; }

            public HttpWebResponse GetResponse()
            {
                return this;
            }
        }
    }
}
