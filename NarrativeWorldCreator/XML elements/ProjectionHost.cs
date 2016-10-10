using NarrativeWorldCreator.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public class ProjectionHost : D3D11Host
    {
        public override void Initialize()
        {
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public override void Draw(TimeSpan gameTime)
        {
            base.GraphicsDeviceManager.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Purple);

        }

        public override void Update(TimeSpan gameTime)
        {
        }
    }
}
