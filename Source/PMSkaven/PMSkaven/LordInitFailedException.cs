// LordInitFailedException.cs created by Iron Wolf for PMSkaven on 03/26/2020 5:39 PM
// last updated 03/26/2020  5:39 PM

namespace PMSkaven
{
    public class LordInitFailedException : System.Exception
    {
        public LordInitFailedException(string message) : base(message)
        {
        }

        public LordInitFailedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}