//-----------------------------------------------------------------------
// <copyright file="AzureUploadService.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using Properties;

    /// <summary>
    /// Uploads doodls to Windows Azure.
    /// </summary>
    public class AzureUploadService : IUploadService
    {
        /// <summary>
        /// Uploads a doodl.
        /// </summary>
        /// <param name="doodlerName">The name to put on the doodl.</param>
        /// <param name="imageStream">The image stream to upload.</param>
        /// <param name="thumbnailStream">The thumbnail stream to upload.</param>
        /// <param name="inkStream">The ink stream to upload.</param>
        /// <returns>The URL of the uploaded doodl.</returns>
        public async Task<string> Upload(string doodlerName, Stream imageStream, Stream thumbnailStream, Stream inkStream)
        {
            var client = new HttpClient();
            var body = new MultipartFormDataContent();

            body.Add(new StringContent(doodlerName), "creator");
            body.Add(new StreamContent(imageStream), "displayImage", "doodl.png");
            body.Add(new StreamContent(thumbnailStream), "thumbnail", "doodl-thumb.png");
            body.Add(new StreamContent(inkStream), "ink", "doodl.isf");

            var response = await client.PostAsync(Settings.Default.UploadEndpoint, body);
            var serializer = new DataContractJsonSerializer(typeof(ResponseContract));
            var responseData = (ResponseContract)serializer.ReadObject(await response.Content.ReadAsStreamAsync());

            return responseData.Location;
        }

        [DataContract]
        private class ResponseContract
        {
            [DataMember(Name = "location")]
            public string Location { get; set; }
        }
    }
}
