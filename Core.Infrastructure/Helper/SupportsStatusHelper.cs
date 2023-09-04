using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Infrastructure.Helper
{
    public class SupportsStatusHelper
    {
        public SupportsStatusHelper()
        {

        }
        public static int NEW = 1;
        public static int PROCESSING = 2;
        public static int FORWARD = 3;
        public static int DONE = 4;
    }
}
