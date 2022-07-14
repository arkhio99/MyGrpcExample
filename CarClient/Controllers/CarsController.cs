using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace CarClient.Controllers
{
    [Route("api/v1/[controller]")]
    public class CarsController : Controller
    {
        private Channel _channel;

        public CarsController(Channel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        [HttpPost]
        [Route("BuildOne")]
        public async Task<Car> BuildCar([FromBody] BuildCarRequest intent)
        {
            var client = new CarBuilder.CarBuilderClient(_channel);
            return await client.BuildCarAsync(intent);
        }

        [HttpPost]
        [Route("BuildSeveral")]
        public async Task<List<Car>> BuildCars([FromBody] IEnumerable<BuildCarRequest> intents)
        {
            var ex = new List<BuildCarRequest>
            {
                new BuildCarRequest
                {
                    SportCarMotor = new SportCarMotor
                    {
                        Velocity = 1,
                        HorseForce = 2,
                    },
                },
                new BuildCarRequest
                {
                    RegularCarMotor = new RegularCarMotor
                    {
                        Velocity = 1,
                        HorseForce = 2,
                    }
                },
            };

            var client = new CarBuilder.CarBuilderClient(_channel);
            var Cars = new List<Car>();
            using (var call = client.BuildCars())
            {
                var responseReader = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        Cars.Add(call.ResponseStream.Current);
                    }
                });

                foreach (var intent in intents)
                {
                    await call.RequestStream.WriteAsync(intent);
                }

                await call.RequestStream.CompleteAsync();
                await responseReader;
            }

            return Cars;
        }
    }
}
