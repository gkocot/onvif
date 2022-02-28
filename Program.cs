using System;
using System.ServiceModel;
using ServiceReference;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using Newtonsoft.Json;

// Generate Client Proxy class.
// https://docs.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-svcutil-guide?tabs=dotnetsvcutil2x
// dotnet tool install --global dotnet-svcutil
// dotnet-svcutil https://www.onvif.org/ver10/device/wsdl/devicemgmt.wsdl

namespace WSDLExperiment01
{
    class Program
    {
        static Binding CreateBinding()
        {
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
            // Doesn't work because the BasicHttpBinding is sending SOAP 1.1 (we need 1.2).
            // Binding binding = new BasicHttpBinding();
            Binding binding = CreateBinding();
            EndpointAddress endpointAddress = new EndpointAddress(
                        new Uri("http://192.168.5.99/onvif/device_service"));
            var client = new DeviceClient(binding, endpointAddress);
            GetServicesResponse resp = await client.GetServicesAsync(true);
            Console.WriteLine($"{JsonConvert.SerializeObject(resp, Formatting.Indented)}");
        }
    }
}
