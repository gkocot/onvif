using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using Newtonsoft.Json;
using CommandLine;

using Grdc.Ims.ElementsVideoOnvifClient;
using Grdc.Ims.ElementsVideoOnvifClient.Device;
using Grdc.Ims.ElementsVideoOnvifClient.Recording;

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
        static async Task Main(string[] args)
        {
            var options = new Options();
            var parseOptionsResult = Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => options = o);

            if (parseOptionsResult.Tag == ParserResultType.NotParsed)
            {
                // Help text requested, or parsing failed. Exit.
                return;
            }

            // GOOD
            // var deviceClient = await OnvifClientFactory.CreateDeviceClientAsync(options.Host, options.User, options.Password);
            // GetServicesResponse getServicesResponse = await deviceClient.GetServicesAsync(false);
            // Console.WriteLine($"{JsonConvert.SerializeObject(getServicesResponse, Formatting.Indented)}");

            var recordingPortClient = await OnvifClientFactory.CreateRecordingPortClient(options.Host, options.User, options.Password);

            // GOOD
            // await recordingPortClient.SetRecordingJobModeAsync("Job-2", "Active");

            // GOOD
            // await recordingPortClient.DeleteRecordingJobAsync("Job-1");

            // GOOD
            GetRecordingJobsResponse getRecordingJobsResponse = await recordingPortClient.GetRecordingJobsAsync();
            Console.WriteLine($"{JsonConvert.SerializeObject(getRecordingJobsResponse, Formatting.Indented)}");

            // GOOD
            // RecordingJobConfiguration recordingJobConfiguration = new RecordingJobConfiguration();
            // recordingJobConfiguration.RecordingToken = "Recording-1"; // TBD GetRecordings;
            // recordingJobConfiguration.Mode = "Active";
            // recordingJobConfiguration.Priority = 1;
            // RecordingJobSource recordingJobSource = new RecordingJobSource();
            // SourceReference sourceReference = new SourceReference();
            // sourceReference.Token = "DefaultProfile-02"; // TBD Use Media service to GetProfile, GetProfiles, GetCompatible...
            // recordingJobSource.SourceToken = sourceReference;
            // recordingJobConfiguration.Source = new RecordingJobSource[] { recordingJobSource };
            // CreateRecordingJobRequest createRecordingJobRequest = new CreateRecordingJobRequest(recordingJobConfiguration);
            // CreateRecordingJobResponse createRecordingJobResponse = await recordingPortClient.CreateRecordingJobAsync(createRecordingJobRequest);
            // Console.WriteLine($"{JsonConvert.SerializeObject(createRecordingJobResponse, Formatting.Indented)}");            
        }
    }
}
