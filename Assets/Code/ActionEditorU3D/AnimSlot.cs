using System;
using System.ComponentModel;

namespace ActionEditorU3D
{

    [Serializable]
    public class AnimSlot
    {

        private string mAnimation = string.Empty;
        [DisplayName("动画"), Description("动画名称")]
        public String Animation { get { return mAnimation; } set { mAnimation = value; } }


        private int mStart = 0;
        [DisplayName("Start"), Description("动画起始百分比")]
        public int Start { get { return mStart; } set { mStart = value; }}


        private int mEnd = 100;
        [DisplayName("End"), Description("动画结束百分比")]
        public int End { get { return mEnd; } set { mEnd = value; } }


        private int mWeight = 1;
        [DisplayName("End"), Description("动画权重")]
        public int Weight { get { return mWeight; } set { mWeight = value; } }
        
    }
}
