using AmazonLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace AmazonLibrary.Services {

    public sealed class EmrService : IEmrService {

        private readonly ILogger<EmrService> _logger;
        private readonly ConnectionInfo _sshInfo;
        
        public EmrService(ILogger<EmrService> logger, IOptions<AmazonOptions> options) {
            _logger = logger;

            _sshInfo = new ConnectionInfo(
                options.Value.EmrMasterNodeEndpointUrl,
                options.Value.EmrMasterNodeEc2Username,
                new PrivateKeyAuthenticationMethod(
                    options.Value.EmrMasterNodeEc2Username,
                    new PrivateKeyFile("/home/ubuntu/cosc2640a3/AWSPrivateKey.pem")
                )
            );
        }
        
        public bool ExecuteCommandMapper() {
            _logger.LogInformation($"{ nameof(EmrService) }.{ nameof(ExecuteCommandMapper) }: Service starts.");

            using var session = new SshClient(_sshInfo);
            session.Connect();

            if (!session.IsConnected) return false;
            var result = session.RunCommand("/home/hadoop/dotnet/dotnet /home/hadoop/mapper/DataMapper.dll");

            session.Disconnect();
            return true;
        }

        public bool ExecuteCommandReducer() {
            _logger.LogInformation($"{ nameof(EmrService) }.{ nameof(ExecuteCommandMapper) }: Service starts.");
            
            using var session = new SshClient(_sshInfo);
            session.Connect();

            if (!session.IsConnected) return false;
            var result = session.RunCommand("/home/hadoop/dotnet/dotnet /home/hadoop/reducer/DataReducer.dll");

            session.Disconnect();
            return true;
        }
    }
}