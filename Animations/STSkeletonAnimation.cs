using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Core.Animations
{
    /// <summary>
    /// Represents a animation that uses bones to animate given a skeleton.
    /// </summary>
    public class STSkeletonAnimation : STAnimation
    {
        /// <summary>
        /// Gets the active skeleton visbile in the scene that may be used for animation.
        /// </summary>
        /// <returns></returns>
        public virtual STSkeleton GetActiveSkeleton()
        {
            return null;
        }
    }
}
