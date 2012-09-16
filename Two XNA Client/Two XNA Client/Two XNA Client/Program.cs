using System;

namespace Two_XNA_Client
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TwoClient game = new TwoClient())
            {
                game.Run();
            }
        }
    }
#endif
}

