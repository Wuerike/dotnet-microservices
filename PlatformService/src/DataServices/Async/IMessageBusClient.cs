using PlatformService.Dtos;

namespace PlatformService.DataServices.Async
{
    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishDto p);
    }
}