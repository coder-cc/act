using Aqua.InputEvent;
using Core.Unit;
using CSTools;
using UnityEngine;

namespace Core.Controller
{
    public class TestController : Singleton<TestController>
    {

        private ActionUnit unit_1;


        public void Init()
        {
            ActionHelp.Instance.Load();

            InputManager.instance.Init();

            UnitInfo info = new UnitInfo() { HierarchyName = "001", ResourcePath = "Assets/Art/Prefab/Model/Player/Actor_meiying", RoleID = 1003};
            unit_1 = new ActionUnit(info);
            unit_1.Init();
            //unit_1.pl
        }


        public void Update()
        {
            InputManager.instance.Update(Time.deltaTime);
            if (unit_1 != null)
                unit_1.Update(Time.deltaTime);
        }



        public void OnGUI()
        {
            //InputManager.instance.OnGUI();
        }


        public void LateUpdate()
        {
            
        }


    }
}
