using System.Windows;
using System.Windows.Controls;
using PlanEditor.Entities;

namespace PlanEditor
{
    /// <summary>
    /// Interaction logic for FindWindowxaml.xaml
    /// </summary>
    public partial class FindWindow : Window
    {
        private readonly Building _building;

        public FindWindow(Building building)
        {
            InitializeComponent();

            _building = building;

            var itemRooms = new ComboBoxItem { Content = "Помещение" };
            var itemPortals = new ComboBoxItem { Content = "Двери" };
            var itemStairways = new ComboBoxItem { Content = "Лестницы" };

            ObjectBox.Items.Add(itemRooms);
            ObjectBox.Items.Add(itemPortals);
            ObjectBox.Items.Add(itemStairways);
        }

        private void Click_Find(object sender, RoutedEventArgs e)
        {
            int id = -1;
            if (!int.TryParse(TextSearch.Text, out id))
            {
                ErrorLabel.Content = "Неверно указано значение";
                return;
            }
            ErrorLabel.Name = "";
            switch (ObjectBox.SelectedIndex)
            {
                case 0:
                    Rooms(id);
                    break;
                case 1:
                    Portals(id);
                    break;
                case 2:
                    Stairways(id);
                    break;
                default:
                    ErrorLabel.Content = "Необходимо выбрать объект";
                    break;
            }
        }

        private void Rooms(int id)
        {
            var isFound = false;
            int num = 0;
            for(int i = 0; i < _building.Places.Count; ++i)
            {
                for (int r = 0; r < _building.Places[i].Count; ++r)
                {
                    ++num;
                    if (num == id)
                    {
                        var place = _building.Places[i][r];
                        place.Select();
                        isFound = true;
                        break;
                    }
                }
                if (isFound) break;
            }

            if (!isFound)
                ErrorLabel.Content = "Объект не найден";
        }

        private void Portals(int id)
        {
            var isFound = false;
            int num = 0;
            for (int i = 0; i < _building.Portals.Count; ++i)
            {
                for (int r = 0; r < _building.Portals[i].Count; ++r)
                {
                    ++num;
                    if (num == id)
                    {
                        var portal = _building.Portals[i][r];
                        portal.Select();
                        isFound = true;
                        break;
                    }
                }

                if (isFound) break;
            }

            if (!isFound)
                ErrorLabel.Content = "Объект не найден";
        }

        private void Stairways(int id)
        {
            var isFound = false;
            int num = 0;
            for (int i = 0; i < _building.Stairways.Count; ++i)
            {
                ++num;
                if (num == id)
                {
                    var place = _building.Stairways[i];
                    place.Select();
                    isFound = true;
                }
            }

            if (!isFound)
                ErrorLabel.Content = "Объект не найден";
        }
    }
}
