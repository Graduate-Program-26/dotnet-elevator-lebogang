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

        services.AddApplicationServices();

        var mockElevatorRepo = new Mock<IElevatorRepo>();
        mockElevatorRepo.Setup(repo => repo.GetElevators()).Returns(new List<IElevator>
        {
            _passengerElevatorFactory.CreateElevator(0)
        });

        mockElevatorRepo.Setup(repo => repo.GetElevator(It.IsAny<Guid>())).Returns((IElevator?)null);
        

        var mockFloorRepo = new Mock<IFloorRepo>();
        mockFloorRepo.Setup(repo => repo.GetFloors()).Returns(new List<Floor> {CreateFloor(0)} );
        mockFloorRepo.Setup(repo => repo.GetFloor(It.IsAny<int>()));

        var mockDispatch = new Mock<IDispatchController>();
        mockDispatch.Setup(dispatch => dispatch.FindBestElevator(It.IsAny<List<IElevator>>(), It.IsAny<int>(), It.IsAny<ElevatorDirection>())).Returns(_passengerElevatorFactory.CreateElevator(0));

        services.AddSingleton(mockElevatorRepo.Object);
        services.AddSingleton(mockFloorRepo.Object);
        services.AddSingleton(mockDispatch.Object);


        return services.BuildServiceProvider();
    }


    public IMediator GetMediator(IServiceProvider provider) => provider.GetRequiredService<IMediator>();
}