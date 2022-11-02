using System;
using System.Collections.Generic;
using System.Text;

namespace CarEvaluation
{
    public static class MainInfoClass
    {
        internal static User user;
        internal static List<User> userList;
        internal static InspectorPage iPage = new InspectorPage();
        internal static CarInspector carInspector = new CarInspector();
        internal static List<Status> statusList = new List<Status>();
        internal static string apiImageSource = "http://vibrant-sanderson.37-140-192-136.plesk.page";
        internal static string apiDBSource = "http://crazy-ramanujan.37-140-192-136.plesk.page/api";
    }
}
