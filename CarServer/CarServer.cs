using Grpc.Core;

namespace CarServer
{
    public class CarServer : CarBuilder.CarBuilderBase
    {
        public override Task<Car> BuildCar(BuildCarRequest request, ServerCallContext context)
        {
            Console.WriteLine("One car requested");
            return Task.FromResult(BuildCar(request));
        }

        public override async Task BuildCars(IAsyncStreamReader<BuildCarRequest> requestStream, IServerStreamWriter<Car> responseStream, ServerCallContext context)
        {
            Console.WriteLine("Several cars requested");
            while (await requestStream.MoveNext())
            {
                await responseStream.WriteAsync(BuildCar(requestStream.Current));
            }
        }

        private Car BuildCar(BuildCarRequest intent)
        {
            return new Car
            {
                Velocity = intent.CarKind == 0 ? intent.SportCarMotor.Velocity : intent.RegularCarMotor.Velocity,
                HorseForce = intent.CarKind == 0 ? intent.SportCarMotor.HorseForce : intent.RegularCarMotor.HorseForce,
                Status = (Car.Types.BuildStatus)(new Random().Next() % 3),
            };
        }
    }
}
