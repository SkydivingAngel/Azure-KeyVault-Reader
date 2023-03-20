using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KeyVaultManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace KeyVaultManager.Controllers
{
    [ApiController]
    [Route("api")]
    public class KeyVaultController : ControllerBase
    {
        private readonly string kvUri = "https://gaspariazurekeyvault.vault.azure.net/";

        public KeyVaultController()
        {

        }

        [HttpGet, Route("keys")]
        public async Task<IActionResult> Keys()
        {
            try
            {
                var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

                List<StoredSecret> storedSecrets = new List<StoredSecret>();

                List<Page<SecretProperties>> secrets = client.GetPropertiesOfSecrets().AsPages().ToList();

                for (int i = 0; i < secrets.Count; i++)
                {
                    var elenco = secrets[i].Values.ToList();

                    for (int j = 0; j < elenco.Count; j++)
                    {
                        var response = await client.GetSecretAsync(elenco[j].Name).ConfigureAwait(false);

                        storedSecrets.Add(new StoredSecret
                        {
                            Name = elenco[j].Name,
                            Value = response.Value.Value
                        });
                    }
                }
                return await Task.FromResult(Ok(storedSecrets));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Ok(ex.ToString()));
            }
        }
    }
}
