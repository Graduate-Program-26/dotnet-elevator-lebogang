using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;


// for mocking serviceProvider with full Mediatr pipeline registered
public class MediatRPipeline
{

    private PassengerElevatorFactory _passengerElevatorFactory = new PassengerElevatorFactory();
    private Floor CreateFloor(int floorNumber = 1)  => new Floor(floorNumber);

    public IServiceProvider BuildProvider(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplicationServices();
        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(repo => repo.GetFloors()).Returns(new List<Floor> {CreateFloor(0)} );
        mockFloorRepo.Setup(repo => repo.GetFloor(It.IsAny<int>()));

        var mockDispatch = new Mock<IDispatchController>();
        mockDispatch.Setup(dispatch => dispatch.FindBestElevator(It.IsAny<List<IElevator>>(), It.IsAny<int>(), It.IsAny<ElevatorDirection>())).Returns(_passengerElevatorFactory.CreateElevator(0));

        services.AddSingleton(mockFloorRepo.Object);
        services.AddSingleton(mockDispatch.Object);

        // tests such as those that need IRepo can confgure their own registrations such thatdependicies are more clear in the test
        configure?.Invoke(services);
        
        return services.BuildServiceProvider();
    }


    public IMediator GetMediator(IServiceProvider provider) => provider.GetRequiredService<IMediator>();
}