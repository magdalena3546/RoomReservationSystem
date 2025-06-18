using System;
using System.ServiceModel;

namespace RoomReservationSystem.Services
{
    [ServiceContract]
    public interface IRoomAvailabilityService
    {
        [OperationContract]
        bool CheckRoomAvailability(int roomId, DateTime startTime, DateTime endTime);
    }
}
