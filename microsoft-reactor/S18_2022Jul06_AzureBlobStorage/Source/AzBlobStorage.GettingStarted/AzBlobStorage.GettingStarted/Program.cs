﻿// See https://aka.ms/new-console-template for more information
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

IConfiguration _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets("42B3177E-1CA6-448F-8F9A-1294955F5337")
    .Build();

// Copy the connection string from the portal in the variable below.
string storageConnectionString = _configuration["AzStorage:BlobStorageConnectionString"];
BlobServiceClient blobServiceClient = new(storageConnectionString);

string containerName = _configuration["AzStorage:BlobContainerName"];
var containerClient = await CreateContainerAsync(blobServiceClient, containerName);

await UploadBlobAsync(containerClient, _configuration["AzStorage:TextFileLocalPath"]);
await UploadBlobAsync(containerClient, _configuration["AzStorage:PngFileLocalPath"]);

Console.WriteLine("\n\nPress any key ...");

static async Task<BlobContainerClient> CreateContainerAsync(BlobServiceClient blobServiceClient, string containerName)
{
    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    await containerClient.CreateIfNotExistsAsync();

    return containerClient;
}

static async Task UploadBlobAsync(BlobContainerClient containerClient, string localFilePath)
{
    var fileName = Path.GetFileName(localFilePath);
    BlobClient blobClient = containerClient.GetBlobClient(fileName);

    // Set the blob's content type so that the browser knows to treat it as an image.
    var fileUploadResults = await blobClient.UploadAsync(File.OpenRead($"{localFilePath}"), true);

    if(fileUploadResults.GetRawResponse().Status == 201)
    {
        Console.WriteLine($"{fileName} uploaded to the {blobClient.Uri} location");
    }
}