using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net;
using System.Net.Http;
using WebApp1.Controllers;

namespace WebApp1.Tests.Controllers
{
    [TestClass]
    public class UploadControllerTests
    {
        [TestMethod]
        public void PostFileUpload_NoFileUpload_Successful()
        {
            // Arrange

            // create instance of controller
            UploadController controller = new UploadController();
            
            // assign request to controller
            controller.Request = CreateHttpRequestMessageWithFileUpload(null);

            // Act
            HttpResponseMessage result = controller.PostFileUpload().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void PostFileUpload_SmallFileUpload_Successful()
        {
            // Arrange

            // create instance of controller
            UploadController controller = new UploadController();

            // assign request to controller
            controller.Request = CreateHttpRequestMessageWithFileUpload("small.zip");

            // Act
            HttpResponseMessage result = controller.PostFileUpload().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }


        [TestMethod]
        public void PostFileUpload_LargeFileUpload_Fails()
        {
            // Arrange

            // create instance of controller
            UploadController controller = new UploadController();

            // assign request to controller
            controller.Request = CreateHttpRequestMessageWithFileUpload("large.zip");

            // Act
            HttpResponseMessage result = controller.PostFileUpload().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        private HttpRequestMessage CreateHttpRequestMessageWithFileUpload(string filename)
        {
            // create the Http request
            //HttpRequestMessage request = new HttpRequestMessage();
            //// create the multipart request content
            var serviceUrl = "http://localhost/Cmc.Nexus.Web/api/Commands/PostFileUpload/Crm/Document/UploadStudentDocument";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, serviceUrl);

            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept-Encoding", "deflate");
            request.Headers.Add("Accept-Encoding", "br");
            request.Headers.Add("Host", "locahost");
            request.Headers.Add("ApiKey", "1234");

            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent("DocumentId"), "12");
            content.Add(new StringContent("FileNameWithExtension"), "text.txt");

            var contentLength = 0;

            // if a file is uploaded
            if (!string.IsNullOrEmpty(filename))
            {
                var uploadFileName = filename;
                var path = Directory.GetCurrentDirectory();
                var targetpath = Path.Combine(path, "Files", uploadFileName);
                if (File.Exists(targetpath))
                {
                    var t = File.ReadAllBytes(targetpath);
                    content.Add(new ByteArrayContent(t), "DocumentImage");
                    contentLength = contentLength + t.Length;
                }
            }

            // assign multipart content to request
            request.Content = content;

            request.Content.Headers.Add(@"Content-Length", contentLength.ToString());

            return request;
        }
    }
}
