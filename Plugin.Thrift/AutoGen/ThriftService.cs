namespace Plugin.Thrift;

public partial class ThriftService
{
    private void Init()
    {
        // UserService
        _manager.RegisterPool(protocol => new UserService.Client(protocol));
    }
}