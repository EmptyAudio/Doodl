//-----------------------------------------------------------------------
// <copyright file="AzureDoodlService.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using Properties;

    /// <summary>
    /// Uploads doodls to Windows Azure.
    /// </summary>
    public class AzureDoodlService : IDoodlService
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

            var response = await client.PostAsync(Settings.Default.UploadEndpoint, body).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(UploadResponseContract));
            var responseData = (UploadResponseContract)serializer.ReadObject(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

            return responseData.Location;
        }

        /// <summary>
        /// Uploads an edit of a doodl.
        /// </summary>
        /// <param name="original">The ID of the original doodl.</param>
        /// <param name="doodlerName">The name to put on the doodl.</param>
        /// <param name="imageStream">The image stream to upload.</param>
        /// <param name="thumbnailStream">The thumbnail stream to upload.</param>
        /// <param name="inkStream">The ink stream to upload.</param>
        /// <returns>The URL of the uploaded doodl.</returns>
        public async Task<string> UploadEdit(Guid original, string doodlerName, Stream imageStream, Stream thumbnailStream, Stream inkStream)
        {
            var client = new HttpClient();
            var body = new MultipartFormDataContent();

            body.Add(new StringContent(original.ToString()), "original");
            body.Add(new StringContent(doodlerName), "creator");
            body.Add(new StreamContent(imageStream), "displayImage", "doodl.png");
            body.Add(new StreamContent(thumbnailStream), "thumbnail", "doodl-thumb.png");
            body.Add(new StreamContent(inkStream), "ink", "doodl.isf");

            var response = await client.PostAsync(Settings.Default.UploadEditEndpoint, body).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(UploadResponseContract));
            var responseData = (UploadResponseContract)serializer.ReadObject(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

            return responseData.Location;
        }

        /// <summary>
        /// Retrieves the source strokes for a given uploaded doodl.
        /// </summary>
        /// <param name="id">The ID of the doodl to retrieve.</param>
        /// <returns>A stream of ink data for the given uploaded doodl.</returns>
        public async Task<Stream> GetStrokesFor(Guid id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(string.Format(Settings.Default.GetDoodlEndpointFormat, id)).ConfigureAwait(false);
            var serializer = new DataContractJsonSerializer(typeof(GetDoodlResponseContract));
            var responseData = (GetDoodlResponseContract)serializer.ReadObject(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            var inkResponse = await client.GetAsync(responseData.InkLocation).ConfigureAwait(false);

            return await inkResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
        }

        [DataContract]
        private class UploadResponseContract
        {
            [DataMember(Name = "location")]
            public string Location { get; set; }
        }

        [DataContract]
        private class GetDoodlResponseContract
        {
            [DataMember(Name = "inkLocation")]
            public string InkLocation { get; set; }
        }
    }
}
