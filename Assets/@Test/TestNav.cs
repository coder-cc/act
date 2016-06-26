using UnityEngine;
using System.Collections;
using org.critterai.nav.u3d;
using org.critterai.nav;
//using org.critterai.samples;



public class TestNav : MonoBehaviour
{



    bool isInit = false;
    private NavGroup mHelper;
    private Vector3 hitPosition;

    private NavmeshPoint findPoint;


    string Msg = string.Empty;


    //private BufferHandler<uint> mPath;
    uint[] mPath = new uint[100];

    int mStraightIndex = 0;
    int mStraightCount = 0;
    Vector3[] mStraightPath = new Vector3[100];

    public enum SearchResult
    {
        /// <summary>
        /// No hit anywhere.
        /// </summary>
        Failed,
        /// <summary>
        /// Hit the geometry, but could not be constrained to the
        /// navigation mesh.
        /// </summary>
        HitGeometry,

        /// <summary>
        /// Hit the geometry and was able to contrain it to the
        /// navigation mesh.
        /// </summary>
        HitNavmesh
    }

    void Awake()
    {
        NavManagerProvider provider = (NavManagerProvider)FindObjectOfType(typeof(NavManagerProvider));

        if (provider == null)
        {
            Debug.LogError(string.Format("{0}: There is no {1} in the scene."
                , name, typeof(NavManagerProvider).Name));

            return;
        }


        NavManager manager = provider.CreateManager();

        if (manager == null)
        {
            Debug.LogError(string.Format("{0}: Could not get the navigation manager.", name));

            return;
        }

        mHelper = manager.NavGroup;

        //mPath = new BufferHandler<uint>();

        isInit = true;
    }


    void Update()
    {
        if (isInit)
        {
            if (Raycast())
            {
                Msg = string.Empty;

                NavmeshPoint resultPoint;

                //  离目标最近的合理位置
                if (NearestPoint(mHelper, ref hitPosition, out resultPoint) == SearchResult.HitNavmesh)
                {
                    Msg += string.Format("Point {0} PolyRef {1} Hit Navmesh {2}", resultPoint.point.ToString(), resultPoint.polyRef.ToString(), "true");
                }
                else
                {
                    Msg += string.Format("Point {0} PolyRef {1} Hit Navmesh {2}", resultPoint.point.ToString(), resultPoint.polyRef.ToString(), "false");
                    //Msg += " NearestPoint 失败!";
                }

                Vector3 pos = Vector3.right;

                int next;
                Msg += "\n";
                FindPath(mHelper, ref pos, ref hitPosition);
                for (int i = mStraightIndex; i < mStraightCount; i++)
                {
                    //next = i + 1;
                    //Debug.DrawLine(mStraightPath[i], mStraightPath[next < mStraightCount ? next : next - 1], Color.black);
                    if (i == 0)
                        Debug.DrawLine(Vector3.right, mStraightPath[0], Color.blue);
                    else
                        Debug.DrawLine(mStraightPath[i - 1], mStraightPath[i], Color.blue);

                    Msg += mStraightPath[i].ToString() + " ";
                }

            }
        }
    }


    void OnGUI()
    {

        GUI.Label(new Rect(0, 0, Screen.width * 0.5f, Screen.height * 0.5f), Msg);
    }


    private bool Raycast()
    {
        RaycastHit hit;
        bool hasHit = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, 100))
            hasHit = false;
        else
        {
            hitPosition = hit.point;
            hasHit = true;
        }

        return hasHit;
    }


    private SearchResult NearestPoint(
         NavGroup helper
        , ref Vector3 geomPoint
        , out NavmeshPoint navPoint
        )
    {


        NavStatus status = helper.query.GetNearestPoint(hitPosition, helper.extents, helper.filter, out navPoint);

        if (NavUtil.Failed(status))
            return SearchResult.HitGeometry;

        if (navPoint.polyRef == 0)
        {
            return SearchResult.HitGeometry;
        }

        return SearchResult.HitNavmesh;
    }


    private bool FindPath(
        NavGroup helper
        , ref Vector3 start
        , ref Vector3 end
        )
    {

        NavmeshPoint startPoint, endPoint;

        helper.query.GetNearestPoint(start, helper.extents, helper.filter, out startPoint);
        helper.query.GetNearestPoint(end, helper.extents, helper.filter, out endPoint);

        float hitPar;
        int hitCount;
        Vector3 hitNor;
        helper.query.Raycast(startPoint, end, helper.filter, out hitPar, out hitNor, mPath, out hitCount);

        if (hitNor.x != 0f || hitNor.y != 0f)
        {
            //  多路径
            helper.query.FindPath(ref startPoint, ref endPoint, helper.extents, helper.filter, mPath, out hitCount);
            helper.query.GetStraightPath(start, end, mPath, 0, hitCount, mStraightPath, null, null, out mStraightCount);
            mStraightIndex = 0;

            while (mStraightIndex < mStraightCount &&
                   (Mathf.Abs(mStraightPath[mStraightIndex].x - start.x) < 0.001f &&
                   Mathf.Abs(mStraightPath[mStraightIndex].y - start.y) < 0.001f))
                mStraightIndex++;

            //if (mStraightIndex < mStraightCount)
            //{

            //}
            //else
            //{

            //}
        }
        else
        {
            //  单一路经
            mStraightCount = 1;
            mStraightIndex = 0;
            mStraightPath[0] = end;
        }


        return true;
        //resultPath = new uint[] { };
        //NavStatus status = helper.query.FindPath(ref startPoint, ref endPoint, helper.extents, helper.filter, mPath, out count);

        //if (NavUtil.Failed(status))
        //    return SearchResult.HitGeometry;

        //return SearchResult.HitNavmesh;

    }

}
