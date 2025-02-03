// using System.Net.Http;
// using System.Threading.Tasks;
// using AutoMapper;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.FileProviders;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using Workmanager.Api.Controllers;
// using Workmanager.ApiTest.Di;
// using Xunit;
//
//
// namespace Workmanager.ApiTest.Controllers;
// [Collection(nameof(SystemTestCollectionDefinition))]
// public class FilesControllerTest {
//
//    private readonly FilesController _filesController;
//    private readonly IWebHostEnvironment _hostingEnvironment;   
//    private readonly IMapper _mapper;
//    private readonly ILogger<FilesController> _logger;
//    private readonly string _images;
//
//    private readonly SeedWorkmanager _seed;
//
//    public FilesControllerTest() {
//
//
//       IServiceCollection serviceCollection = new ServiceCollection();
//       serviceCollection.AddPersistenceTest();
//       serviceCollection.AddControllersTest();
//
//       ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider()
//          ?? throw new Exception("Failed to build Serviceprovider");
//       
//       var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
//       var contentRootPath = Path.Combine(home, "testing", "contentRoot");
//       if(! Directory.Exists(contentRootPath)) Directory.CreateDirectory(contentRootPath);
//       var webRootPath = Path.Combine(home, "testing", "webRoot");      
//       if(! Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);
//       _images = Path.Combine(home, "testing", "images");      
//       if(! Directory.Exists(_images)) Directory.CreateDirectory(_images);
//       _hostingEnvironment = new WebHostingEnvironmentMock(
//          environmentName: "Test",
//          applicationName: "Test",
//          contentRootPath: contentRootPath,
//          contentRootFileProvider: new PhysicalFileProvider(contentRootPath),
//          webRootPath: webRootPath,
//          webRootFileProvider: new PhysicalFileProvider(webRootPath)
//       );
//       
//       _mapper = serviceProvider.GetRequiredService<IMapper>()
//          ?? throw new Exception("Failed to create an instance of IMapper");
//       _logger = serviceProvider.GetRequiredService<ILogger<FilesController>>()
//          ?? throw new Exception("Failed to create an instance of ILogger<FilesController>");
//       
//       _filesController = new FilesController(_hostingEnvironment);
//       
//
//       _seed = new SeedWorkmanager();
//    }
//
//    [Fact]
//    public async Task UploadLargeFile_InvalidRequest_ReturnsUnsupportedMediaTypeResult() {
//
//       // Arrange
//       var apiURL = "http://localhost:50492/home/upload";
//       const string filename = "D:\\samplefile.docx";
//
//       HttpClient _client = new HttpClient();
//
//       // Instead of JSON body, multipart form data will be sent as request body.
//       var httpContent = new MultipartFormDataContent();
//       var fileContent = new ByteArrayContent(File.ReadAllBytes(filename));
//     
//       string boundary = MultipartFormDataHelper.GenerateBoundary();
//       string contentType = $"multipart/form-data; boundary={boundary}";
//
//       
//       fileContent.Headers.ContentType =  System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/form-data");
//
//       // Add File property with file content
//       httpContent.Add(fileContent, "file", filename);
//
//       // Add id property with its value
//       httpContent.Add(new StringContent("789"), "id");
//
//       // Add title property with its value.
//       httpContent.Add(new StringContent("Some title value"), "title");
//
//       // send POST request.
//       var response = await _client.PostAsync(apiURL, httpContent);
//       response.EnsureSuccessStatusCode();
//       var responseContent = await response.Content.ReadAsStringAsync();
//
//       // output the response content to the console.
//       Console.WriteLine(responseContent);
//
//    }
//
//    [Fact]
//    public async Task UploadLargeFile_ValidRequest_ReturnsOkResult() {
//
//
//       
//       var hostBuilder = new WebHostBuilder().ConfigureServices
//             
//          .ConfigureWebHost(webBuilder =>
//          {
//             webBuilder.UseStartup<YourStartupClass>(); // Replace with the actual name of your startup class
//             webBuilder.UseTestServer();
//          });
//       
//       
//
//       using (var server = new TestServer(hostBuilder)) {
//          var client = server.CreateClient();
//          var uri = new Uri("http://localhost:5010/{Program.ApiUrlPath}/files");
//
//          // Arrange
//
//
//          string boundary = MultipartFormDataHelper.GenerateBoundary();
//          string contentType = $"multipart/form-data; boundary={boundary}";
//          client.DefaultRequestHeaders.Add("Content-Type", contentType);
//
//          var httpContext = new DefaultHttpContext();
//          httpContext.Request.ContentType = contentType;
//          /*
//          var fileContent = @"boundary \r\n" +
//             $"Content-Disposition: form-data; name={path}"+"\r\n" +
//             "Content-Type: application/octet-stream\r\n\r\n" +
//             "File content here\r\n" +
//             boundary + "--\r\n";
//          */
//
//          using (MultipartFormDataContent multipartContent = new(boundary)) {
//             byte[] bytesFromFile = await File.ReadAllBytesAsync($"{_images}/Kuh.jpg");
//             ByteArrayContent fileContent = new(bytesFromFile);
//             fileContent.Headers.ContentType =
//                new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
//             multipartContent.Add(fileContent, "file", "Kuh.jpg");
//             Stream stream = await multipartContent.ReadAsStreamAsync();
//
//             httpContext.Request.Body = stream;
//             httpContext.Request.ContentLength = stream.Length;
//
//             await client.PostAsync(uri, multipartContent);
//             //await _filesController.UploadLargeFile();
//
//          }
//
//       }
//
//    }
// }
//
//
// // Mock class for IHostEnvironment
// public class WebHostingEnvironmentMock(
//    string environmentName,
//    string applicationName,
//    string contentRootPath,
//    IFileProvider contentRootFileProvider,
//    string webRootPath = "",
//    IFileProvider webRootFileProvider = default!
// ) : IWebHostEnvironment {
//    public string EnvironmentName { get; set; } = environmentName;
//    public string ApplicationName { get; set; } = applicationName;
//    public string ContentRootPath { get; set; } = contentRootPath;
//    public IFileProvider ContentRootFileProvider { get; set; } = contentRootFileProvider;
//    public string WebRootPath { get; set; } = webRootPath;
//    public IFileProvider WebRootFileProvider { get; set; } = webRootFileProvider;
// }
//
//
// public class MultipartFormDataHelper {
//    public static string GenerateBoundary() {
//       // You can use a combination of a timestamp and a random string to create a unique boundary
//       string timestamp = DateTime.Now.Ticks.ToString("x");
//       string randomString = Guid.NewGuid().ToString("N");
//
//       // Combine them to create a unique boundary
//       string boundary = $"--------{timestamp}{randomString}";
//       
//       if(boundary.Length>70) 
//          boundary = boundary.Substring(0, 70);
//        
//       
//       return boundary;
//    }
// }