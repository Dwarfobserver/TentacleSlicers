using TentacleSlicers.graphics;

namespace TentacleSlicers.maps
{
    public class WallTransparency : TransparencyHandler
    {
        private readonly WallTransparencyHitbox _ownerHitbox;

        public WallTransparency(Wall owner)
        {
            _ownerHitbox = new WallTransparencyHitbox(owner);
        }

        public override void Tick(int ms)
        {
            Opacity = _ownerHitbox.Opacity;
        }
    }
}