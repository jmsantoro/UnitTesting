namespace ClassLibrary
{
    public interface IEngine
    {
        bool Start();

        void Shutdown();

        bool IsRunning { get; set; }
    }
}