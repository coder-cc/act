using Aqua.InputEvent;
using Core.Unit;
using CSTools;
using UnityEngine;

namespace Core.Controller
{
    public class TestController : Singleton<TestController>
    {

        public ActionUnit unit_1;


        public void Init()
        {
            ActionHelp.Instance.Load();

           
            UnitInfo info = new UnitInfo() { HierarchyName = "001", ResourcePath = "Assets/LocalResources/Actor/Model/Actor_meiying", RoleID = 1003};
            unit_1 = new ActionUnit(info);
            unit_1.Init();

            InputManager.instance.Init(unit_1);

            CameraController.Instance.Init(true);
            CameraController.Instance.StartUnitCamera(unit_1);
        }


        public void Update()
        {
            CameraController.Instance.Update();
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
