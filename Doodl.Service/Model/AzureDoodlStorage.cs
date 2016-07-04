//-----------------------------------------------------------------------
// <copyright file="AzureDoodlStorage.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Provides doodl storage using a Windows Azure Storage account.
    /// </summary>
    public class AzureDoodlStorage : IDoodlStorage
    {
        private readonly IDoodlRenderer renderer;
        private readonly IOptions<AzureDoodlStorageOptions> options;
        private readonly CloudBlobClient blobClient;
        private readonly CloudTableClient tableClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureDoodlStorage"/> class.
        /// </summary>
        /// <param name="renderer">The doodl render to use.</param>
        /// <param name="options">The configuration options to use.</param>
        public AzureDoodlStorage(IDoodlRenderer renderer, IOptions<AzureDoodlStorageOptions> options)
        {
            this.renderer = renderer;
            this.options = options;

            var connection = CloudStorageAccount.Parse(this.options.Value.ConnectionString);

            this.blobClient = connection.CreateCloudBlobClient();
            this.tableClient = connection.CreateCloudTableClient();
        }

        private BlobRequestOptions BlobRequestOptions => new BlobRequestOptions();

        private TableRequestOptions TableRequestOptions => new TableRequestOptions();

        /// <summary>
        /// Uploads a doodl asynchronously.
        /// </summary>
        /// <param name="creator">The creator of the doodl.</param>
        /// <param name="displayImage">The display image stream of the doodl.</param>
        /// <param name="thumbnail">The thumbnail stream of the doodl.</param>
        /// <param name="ink">The source ink stream of the doodl.</param>
        /// <param name="backgroundImage">The optional background image behind the ink of the doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The uploaded doodl's display location.</returns>
        public async Task<string> UploadDoodlAsync(
            string creator,
            Stream displayImage,
            Stream thumbnail,
            Stream ink,
            Stream backgroundImage,
            CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var operationContext = new OperationContext()
            {
                ClientRequestID = id.ToString(),
            };

            var doodl = await this.UploadDoodlCoreAsync(
                id,
                creator,
                null,
                displayImage,
                thumbnail,
                ink,
                backgroundImage,
                operationContext,
                cancellationToken);

            return doodl.Location;
        }

        /// <summary>
        /// Uploads an edit of a doodl asynchronously.
        /// </summary>
        /// <param name="original">The identifier of the original doodl.</param>
        /// <param name="creator">The creator of the edit.</param>
        /// <param name="displayImage">The display image stream of the doodl.</param>
        /// <param name="thumbnail">The thumbnail stream of the doodl.</param>
        /// <param name="ink">The source ink stream of the doodl.</param>
        /// <param name="backgroundImage">The optional background image behind the ink of the doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The uploaded doodl's display location.</returns>
        public async Task<string> UploadDoodlEditAsync(
            Doodl original,
            string creator,
            Stream displayImage,
            Stream thumbnail,
            Stream ink,
            Stream backgroundImage,
            CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var operationContext = new OperationContext()
            {
                ClientRequestID = id.ToString(),
            };

            var doodl = await this.UploadDoodlCoreAsync(
                id,
                creator,
                original,
                displayImage,
                thumbnail,
                ink,
                backgroundImage,
                operationContext,
                cancellationToken);

            var doodlEditsTable = await this.EnsureDoodlEditsTable(operationContext, cancellationToken);

            doodl.PartitionKey = original.ID.ToString();

            await doodlEditsTable.ExecuteAsync(TableOperation.Insert(doodl), this.TableRequestOptions, operationContext);

            var editsPageBlob = new CloudBlockBlob(new Uri(original.Location), this.blobClient.Credentials);

            var edits = await this.GetEditsAsync(doodlEditsTable, original, operationContext, cancellationToken);

            var grandOriginal = original.Original.HasValue ? await this.GetDoodlAsync(original.Original.Value, cancellationToken) : null;

            await this.UploadBlobAsync(
                editsPageBlob,
                await this.renderer.RenderDoodlDisplayAsync(original, grandOriginal, edits, cancellationToken),
                "text/html",
                operationContext,
                cancellationToken);

            return doodl.Location;
        }

        /// <summary>
        /// Retrieves a doodl asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the doodl to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The retrieved doodl.</returns>
        public async Task<Doodl> GetDoodlAsync(Guid id, CancellationToken cancellationToken)
        {
            var operationContext = new OperationContext()
            {
                ClientRequestID = Guid.NewGuid().ToString(),
            };

            var doodlTable = await this.EnsureDoodlTable(operationContext, cancellationToken);

            return (Doodl)(await doodlTable.ExecuteAsync(TableOperation.Retrieve<Doodl>(id.ToString(), id.ToString()))).Result;
        }

        private async Task<Doodl> UploadDoodlCoreAsync(
            Guid id,
            string creator,
            Doodl original,
            Stream displayImage,
            Stream thumbnail,
            Stream ink,
            Stream backgroundImage,
            OperationContext operationContext,
            CancellationToken cancellationToken)
        {
            var container = await this.EnsureContainer(operationContext, cancellationToken);

            var displayId = Convert.ToBase64String(id.ToByteArray()).Substring(0, 22);
            var displayImageBlob = container.GetBlockBlobReference(string.Format(this.options.Value.DisplayImageFormat, displayId));
            var thumbnailBlob = container.GetBlockBlobReference(string.Format(this.options.Value.ThumbnailFormat, displayId));
            var inkBlob = container.GetBlockBlobReference(string.Format(this.options.Value.InkFormat, displayId));
            var backgroundImageBlob = container.GetBlockBlobReference(string.Format(this.options.Value.BackgroundFormat, displayId));
            var displayPageBlob = container.GetBlockBlobReference(string.Format(this.options.Value.DisplayPageFormat, displayId));

            await this.UploadBlobAsync(displayImageBlob, displayImage, "image/png", operationContext, cancellationToken);
            await this.UploadBlobAsync(thumbnailBlob, thumbnail, "image/png", operationContext, cancellationToken);
            await this.UploadBlobAsync(inkBlob, ink, "application/x-ms-ink", operationContext, cancellationToken);
            await this.UploadBlobAsync(backgroundImageBlob, backgroundImage, "image/png", operationContext, cancellationToken);

            var doodl = new Doodl()
            {
                ID = id,
                Creator = creator,
                Original = original?.ID,
                DisplayImageLocation = displayImageBlob.Uri.AbsoluteUri,
                ThumbnailLocation = thumbnailBlob.Uri.AbsoluteUri,
                InkLocation = inkBlob.Uri.AbsoluteUri,
                BackgroundLocation = backgroundImageBlob.Uri.AbsoluteUri,
                Location = displayPageBlob.Uri.AbsoluteUri,
                PartitionKey = id.ToString(),
                Timestamp = DateTimeOffset.UtcNow,
                RowKey = id.ToString(),
            };

            await this.UploadBlobAsync(
                displayPageBlob,
                await this.renderer.RenderDoodlDisplayAsync(doodl, original, Enumerable.Empty<Doodl>(), cancellationToken),
                "text/html",
                operationContext,
                cancellationToken);

            var doodlTable = await this.EnsureDoodlTable(operationContext, cancellationToken);

            await doodlTable.ExecuteAsync(TableOperation.Insert(doodl), this.TableRequestOptions, operationContext, cancellationToken);

            return doodl;
        }

        private async Task<CloudBlobContainer> EnsureContainer(OperationContext operationContext, CancellationToken cancellationToken)
        {
            var container = this.blobClient.GetContainerReference(string.Format(this.options.Value.ContainerFormat, DateTimeOffset.UtcNow));

            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, this.BlobRequestOptions, operationContext, cancellationToken);

            return container;
        }

        private async Task UploadBlobAsync(CloudBlockBlob blob, Stream stream, string contentType, OperationContext operationContext, CancellationToken cancellationToken)
        {
            if (stream != null)
            {
                await blob.UploadFromStreamAsync(stream, null, this.BlobRequestOptions, operationContext, cancellationToken);

                blob.Properties.ContentType = contentType;

                await blob.SetPropertiesAsync(null, this.BlobRequestOptions, operationContext, cancellationToken);
            }
        }

        private async Task UploadBlobAsync(CloudBlockBlob blob, string data, string contentType, OperationContext operationContext, CancellationToken cancellationToken)
        {
            var operation = new OperationContext();

            await blob.UploadTextAsync(data, Encoding.UTF8, null, this.BlobRequestOptions, operation, cancellationToken);

            blob.Properties.ContentType = contentType;

            await blob.SetPropertiesAsync(null, this.BlobRequestOptions, operation, cancellationToken);
        }

        private async Task<CloudTable> EnsureDoodlTable(OperationContext operationContext, CancellationToken cancellationToken)
        {
            var doodlTable = this.tableClient.GetTableReference(this.options.Value.DoodlTableName);

            await doodlTable.CreateIfNotExistsAsync(this.TableRequestOptions, operationContext, cancellationToken);

            return doodlTable;
        }

        private async Task<CloudTable> EnsureDoodlEditsTable(OperationContext operationContext, CancellationToken cancellationToken)
        {
            var doodlEditsTable = this.tableClient.GetTableReference(this.options.Value.DoodlEditsTableName);

            await doodlEditsTable.CreateIfNotExistsAsync(this.TableRequestOptions, operationContext, cancellationToken);

            return doodlEditsTable;
        }

        private async Task<IEnumerable<Doodl>> GetEditsAsync(CloudTable doodlEditsTable, Doodl original, OperationContext operationContext, CancellationToken cancellationToken)
        {
            var query = new TableQuery<Doodl>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, original.ID.ToString()));
            var results = new List<Doodl>();

            var resultSegment = await doodlEditsTable.ExecuteQuerySegmentedAsync<Doodl>(
                query,
                null,
                this.TableRequestOptions,
                operationContext,
                cancellationToken);

            while (resultSegment.ContinuationToken != null)
            {
                results.AddRange(resultSegment);

                resultSegment = await doodlEditsTable.ExecuteQuerySegmentedAsync<Doodl>(
                    query,
                    resultSegment.ContinuationToken,
                    this.TableRequestOptions,
                    operationContext,
                    cancellationToken);
            }

            results.AddRange(resultSegment);

            return results;
        }
    }
}
