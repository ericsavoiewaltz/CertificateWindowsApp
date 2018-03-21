using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        Task t = new Task(DownloadPageAsync);
        t.Start();
        Console.WriteLine("Downloading page...");
        Console.ReadLine();
    }

    static async void DownloadPageAsync()
    {
        // ... Target page.
        string page = "https://lenel.re-qa.waltzlabs.com/lenel/v1/doors";

        X509Store userCaStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        X509Certificate2 clientCertificate = null;
        try
        {
            userCaStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificatesInStore = userCaStore.Certificates;
            X509Certificate2Collection findResult = certificatesInStore.Find(X509FindType.FindBySubjectName, "test-cert-1", true);
            if (findResult.Count == 1)
            {
                clientCertificate = findResult[0];
            }
            else
            {
                throw new Exception("Unable to locate the correct client certificate.");
            }    
        }
        catch
        {
            throw;
        }
        finally
        {
            userCaStore.Close();
        }

        Console.WriteLine(clientCertificate);
        WebRequestHandler handler = new WebRequestHandler();
        X509Certificate2 cert = clientCertificate;

        if (cert != null)
        {
            handler.ClientCertificates.Add(cert);
        }

        // ... Use HttpClient.
        using (HttpClient client = new HttpClient(handler))
        using (HttpResponseMessage response = await client.GetAsync(page))
        using (HttpContent content = response.Content)
        {
            // ... Read the string.
            string result = await content.ReadAsStringAsync();

            // ... Display the result.
            if (result != null &&
                result.Length >= 50)
            {
                Console.WriteLine(result);
            }
        }
    }
}