using System;
using System.ServiceModel;
using ServiceReference;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using Newtonsoft.Json;
using CommandLine;

// Generate Client Proxy class.
// https://docs.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-svcutil-guide?tabs=dotnetsvcutil2x
// dotnet tool install --global dotnet-svcutil
// dotnet-svcutil https://www.onvif.org/ver10/device/wsdl/devicemgmt.wsdl

// dotnet run -- --host 192.168.5.114 --user admin --password visio1234

namespace OnvifTool
{
    public class Options
    {
        [Option(Required = false, Default = "192.168.5.99", HelpText = "Host IP address.")]
        public string Host { get; set; }

        [Option(Required = false, Default = "root", HelpText = "User name.")]
        public string User { get; set; }

        [Option(Required = false, Default = "root", HelpText = "Password.")]
        public string Password { get; set; }
    }

    class Program
    {
        static Binding CreateBinding()
        {
            // THIS WORKS IF AUTHENTICATION NOT NEEDED, I DIDN'T FIND A WAY TO MAKE IT WORK WITH DIGEST AUTHENTICATION.
            // var binding = new WSHttpBinding(SecurityMode.None);

            // THIS DOESN'T WORK, THROWS AN EXCEPTION COMLAINING THAT HTTPS ENDPOINT INSTEAD OF HTTP IS EXPECTED.
            // var binding = new WSHttpBinding(SecurityMode.Transport);
            // binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Digest;
            // return binding;

            var binding = new CustomBinding();
            // The default TextMessageEncodingBindingElement() constructor creates MessageVersion of WS-Addressing 1.0 and SOAP 1.2.
            // The default text encoding is the UTF-8 format.
            var textBindingElement = new TextMessageEncodingBindingElement();
            // So we actually don't need this.
            // var textBindingElement = new TextMessageEncodingBindingElement {
            // 	MessageVersion = MessageVersion.CreateVersion (EnvelopeVersion.Soap12, AddressingVersion.None)
            // };
            var httpBindingElement = new HttpTransportBindingElement
            {
                AllowCookies = true,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };

            // Needed for the endpoints that require authentication.
            httpBindingElement.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;

            binding.Elements.Add(textBindingElement);
            binding.Elements.Add(httpBindingElement);

            return binding;
        }

        // public static async Task<DeviceClient> CreateDeviceClientAsync (Uri uri, string username, string password)
        // {
        // 	var binding = CreateBinding ();
        // 	var endpoint = new EndpointAddress (uri);
        // 	var device = new DeviceClient (binding, endpoint);
        // 	var time_shift = await GetDeviceTimeShift (device);

        // 	device = new DeviceClient (binding, endpoint);
        // 	device.ChannelFactory.Endpoint.EndpointBehaviors.Clear ();
        // 	device.ChannelFactory.Endpoint.EndpointBehaviors.Add (new SoapSecurityHeaderBehavior (username, password, time_shift));

        // 	// Connectivity Test
        // 	await device.OpenAsync ();

        // 	return device;
        // }

        // public static async Task<DeviceClient> CreateDeviceClientAsync (string host, string username, string password)
        // {
        // 	return await CreateDeviceClientAsync (new Uri ($"http://{host}/onvif/device_service"), username, password);
        // }

        static async Task Main(string[] args)
        {
            var options = new Options();
            var parseOptionsResult = Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => options = o);

            if (parseOptionsResult.Tag == ParserResultType.NotParsed)
            {
                // Help text requested, or parsing failed. Exit.
                return;
            }

            // Doesn't work because the BasicHttpBinding is sending SOAP 1.1 (we need 1.2).
            // Binding binding = new BasicHttpBinding();
            Binding binding = CreateBinding();

            string endpoint = $"http://{options.Host}/onvif/device_service";
            Console.WriteLine(endpoint);

            EndpointAddress endpointAddress = new EndpointAddress(
                // AXIS 192.168.5.99, 500 Internal Server Error.
                // TruVision 192.168.5.114, returns empty StorageConfigurations[].
                new Uri(endpoint));
            var client = new DeviceClient(binding, endpointAddress);

            // THIS DOESN'T WORK.
            // client.ClientCredentials.UserName.UserName = "admin";
            // client.ClientCredentials.UserName.Password = "visio1234";

            // THIS WORKS FOR DIGEST AUTHENTICATION.
            client.ClientCredentials.HttpDigest.ClientCredential.UserName = options.User;
            client.ClientCredentials.HttpDigest.ClientCredential.Password = options.Password;

            // THIS WORKS, ENDPOINT THAT REQUIRES NO AUTHENTICATION.
            // GetServicesResponse resp = await client.GetServicesAsync(true);
            // Console.WriteLine($"{JsonConvert.SerializeObject(resp, Formatting.Indented)}");

            try
            {
                Console.WriteLine("GetStorageConfigurations");
                GetStorageConfigurationsResponse resp = await client.GetStorageConfigurationsAsync();
                Console.WriteLine($"{JsonConvert.SerializeObject(resp, Formatting.Indented)}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
