using System;
namespace Core.Infrastructure.Helper
{
    public class IdentityHelper
    {
        public class Formula
        {
            public const int StandardPrice = 1;
            public const int PlusPrice = 2;
            public const int CheckIn = 3;
            public const int CheckOut = 4;
        }

        public class Timeline
        {
            public const int Free = 1;
            public const int Hour = 2;
            public const int HalfDay = 3;
            public const int Day = 4;
        }

        public class ServiceType
        {
            public const int Hour = 1;
            public const int Day = 2;
            public const int Month = 3;
        }

        public class BookingStatus
        {
            public const int New = 1;
            public const int Accepted = 2;
            public const int Processed = 3;
            public const int Cancel = 4;
            public const int Reject = 5;
            public const int CustomerCancel = 6;
            public const int Done = 10;
        }

        public class TransportationStatus
        {
            public const int New = 1;
            public const int Processing = 2;
            public const int Processed = 3;
            public const int Delivering = 4;
            public const int RejectDelivery = 5;
            public const int DeliveryComplete = 6;
            public const int Recalling = 7;
            public const int RejectRecall = 8;
            public const int HandOvering = 9;
            public const int Recalled = 10;
            public const int Took = 11;
            public const int Done = 20;
            public const int Cancel = 50;
        }

        public class VehicleStatus
        {
            public const int Free = 1;
            public const int Processed = 2;
            public const int Moving = 3;
            public const int Broken = 4;
            public const int Maintenance = 5;
        }

        public class UserType
        {
            public const int Administator = 1;
            public const int Employee = 2;
            public const int Customer = 3;
        }
    }
}
