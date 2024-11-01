using Shared.net.structs;

namespace Shared.net;

public class SlasherPacket
{
    public OtherPosition OtherPosition { get; set; }
    public CurrentPosition CurrentPosition { get; set; }
}