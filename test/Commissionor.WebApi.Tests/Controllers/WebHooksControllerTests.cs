using System.Threading.Tasks;
using Commissionor.WebApi.Controllers;
using Commissionor.WebApi.Models;
using Commissionor.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Commissionor.WebApi.Tests
{
    public class WebHooksControllerTests
    {

        [Fact]
        public async Task OnTapEvent_fires_received_event()
        {
            // Arrange
            var controller = new WebHooksController();
            var mockEventSource = new Mock<IEventSource>();
            var tapEvent = new TapEvent()
            {
                DeviceId = "DeviceId",
                ReaderId = "ReaderId"
            };

            // Act
            var result = await controller.OnTapEvent(mockEventSource.Object, tapEvent);

            // Assert
            Assert.IsType<OkResult>(result);
            mockEventSource.Verify(eventSource => eventSource.FireEvent(tapEvent), Times.Once());
        }
    }
}
