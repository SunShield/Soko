using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Utils
{
    public class InverseMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                var m = new Material(base.materialForRendering);
                m.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return m;
            }
        }
    }
}