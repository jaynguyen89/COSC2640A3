using AmazonLibrary.Interfaces;
using Helper.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WinSCP;

namespace AmazonLibrary.Services {

    public sealed class EmrService : IEmrService {

        private readonly ILogger<EmrService> _logger;
        private readonly SessionOptions _sshOptions;

        public EmrService(ILogger<EmrService> logger, IOptions<AmazonOptions> options) {
            _logger = logger;

            _sshOptions = new SessionOptions {
                Protocol = Protocol.Sftp,
                HostName = options.Value.EmrMasterNodeEndpointUrl,
                UserName = options.Value.EmrMasterNodeEc2Username,
                SshHostKeyFingerprint = options.Value.KeyFingerprint,
                SshPrivateKeyPath = $@"{ SharedConstants.EmailTemplateFolderPath }AWSPrivateKey.ppk",
            };
        }
        
        public bool ExecuteCommandMapper() {
            _logger.LogInformation($"{ nameof(EmrService) }.{ nameof(ExecuteCommandMapper) }: Service starts.");
            using var session = new Session();
            session.Open(_sshOptions);

            if (!session.Opened) return false;
            
            var result = session.ExecuteCommand("/home/hadoop/dotnet/dotnet /home/hadoop/mapper/DataMapper.dll");

            session.Close();
            return result.IsSuccess;
        }

        public bool ExecuteCommandReducer() {
            _logger.LogInformation($"{ nameof(EmrService) }.{ nameof(ExecuteCommandMapper) }: Service starts.");
            using var session = new Session();
            session.Open(_sshOptions);

            if (!session.Opened) return false;
            
            var result = session.ExecuteCommand("/home/hadoop/dotnet/dotnet /home/hadoop/reducer/DataReducer.dll");

            session.Close();
            return result.IsSuccess;
        }
    }
}