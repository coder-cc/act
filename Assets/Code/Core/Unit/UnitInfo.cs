using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Unit
{
    public class UnitInfo
    {
        private int mRoleID;
        private string mHierarchyName;
        private string mResourcePath;


        public string HierarchyName
        {
            set { mHierarchyName = value; }
            get { return mHierarchyName; }
        }

        public string ResourcePath
        {
            set { mResourcePath = value; }
            get { return mResourcePath; }
        }


        public int RoleID
        {
            set { mRoleID = value; }
            get { return mRoleID; }
        }


    }
}
