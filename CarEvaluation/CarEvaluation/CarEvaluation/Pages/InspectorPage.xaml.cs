
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarEvaluation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectorPage : TabbedPage
    {
        List<Car> cars = new List<Car>();
        public InspectorPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            MainInfoClass.iPage = this;
            if (MainInfoClass.user != null)
            {
              
                if (MainInfoClass.user.RoleId == 1)
                {
                    Children.Add(new CarInspector(true)
                    {
                        Title = "Ожидают оценку",
                        IconImageSource = "carM.png",
                    });
                    Children.Add(new CarInspector("",true)
                    {
                        Title = "История",
                        IconImageSource = "car.png",
                    });

                }
                else if (MainInfoClass.user.RoleId == 2)
                {
                    Children.Add(new CarInspector(true, 2)
                    {
                        Title = "Активные",
                        IconImageSource = "carSearch.png",

                    });
                    Children.Add(new CarInspector(2, true)
                    {
                        Title = "История",
                        IconImageSource = "car.png",

                    });
                }
                else if(MainInfoClass.user.RoleId == 3)
                {
                    Children.Add(new CarInspector(2)
                    {
                        Title = "В наличии",
                        IconImageSource = "car.png",
                    });
                    Children.Add(new CarInspector("")
                    {
                        Title = "Забронированные",
                        IconImageSource = "carReserve.png",
                    });
                }
                else if (MainInfoClass.user.RoleId == 4)
                {
                    Children.Add(new CarInspector(true)
                    {
                        Title = "Ожидают",
                        IconImageSource = "carM.png",
                    });
                    Children.Add(new CarInspector(2)
                    {
                        Title = "В наличии",
                        IconImageSource = "car.png",
                    });
                    Children.Add(new CarInspector("")
                    {
                        Title = "Зарезерв.",
                        IconImageSource = "carReserve.png",
                    });
                    Children.Add(new CarInspector("",2)
                    {
                        Title = "Прочее",
                        IconImageSource = "car.png",
                    });
                }
                Children.Add(new PersonPage
                {
                    Title = "Профиль",
                    IconImageSource = "person.png",
                });
            }
        }
    }
}